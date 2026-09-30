#!/usr/bin/env python3
"""Liberty art-request queue (docs/art/README.md).

The repository is the source of truth for every image request and generated asset:
  docs/art/requests/ART-NNN.json   one request, its prompt history and review log
  art/generated/ART-NNN/rN/        what the image agent produced for revision N
  art/approved/ART-NNN/            the approved image, and prepped/ (texture-ready output)
  art/rejected/ART-NNN/rN/         rejected generations, kept for provenance

Commands (run from anywhere; --root selects another repository root, used by the self-test):
  new --title T --prompt P --use U --size WxH [--transparency none|alpha|cutout] [--priority P0|P1|P2]
      [--kind texture|ui|overlay|decal|concept] [--consumer PATH]      create the next ART-NNN (status requested)
  list [--status S]                  one line per request
  validate                           every request well-formed, files and statuses consistent (exit 1 on problems)
  register ID --by NAME [--model M] [--notes N]   record the files in art/generated/ID/r<rev>/ (status generated)
  check ID                           technical check of the current revision's files against the request
  approve ID FILE [--notes N]        move one generated file to art/approved/ID/ (status approved)
  reject ID [--notes N] [--revise-prompt P]   move the revision to art/rejected/; with a new prompt, queue revision+1
  prep ID                            resize/convert the approved image to the request's final size and channels into
                                     art/approved/ID/prepped/, copy it to the consumer path (status prepped)
  mark ID integrated [--notes N]     the asset is verified in game (status integrated)
  selftest                           the whole lifecycle in a temporary repository
"""
import argparse
import datetime
import hashlib
import json
import os
import re
import shutil
import sys
import tempfile

STATUSES = ("requested", "generated", "approved", "rejected", "prepped", "integrated")
TRANSPARENCY = ("none", "alpha", "cutout")
PRIORITIES = ("P0", "P1", "P2")
KINDS = ("texture", "ui", "overlay", "decal", "concept")
REQUIRED = ("id", "title", "status", "priority", "kind", "prompt", "intendedUse", "dimensions", "aspectRatio",
            "transparency", "styleConstraints", "prohibited", "outputPath", "outputName", "revision", "history")
ID_PATTERN = re.compile(r"^ART-\d{3,}$")
IMAGE_EXTENSIONS = (".png", ".jpg", ".jpeg", ".webp")
# Game textures: power-of-two sides up to this size; larger needs "oversizeReason" (STAGE1.md: no blanket 4K).
MAX_TEXTURE_SIDE = 2048
ASPECT_TOLERANCE = 0.02


class Queue:
    def __init__(self, root):
        self.root = os.path.abspath(root)
        self.requests = os.path.join(self.root, "docs", "art", "requests")
        self.style = os.path.join(self.root, "docs", "art", "style.json")

    def path(self, *parts):
        return os.path.join(self.root, *parts)

    def request_file(self, art_id):
        return os.path.join(self.requests, art_id + ".json")

    def ids(self):
        if not os.path.isdir(self.requests):
            return []
        return sorted(f[:-5] for f in os.listdir(self.requests) if ID_PATTERN.match(f[:-5]) and f.endswith(".json"))

    def load(self, art_id):
        with open(self.request_file(art_id), encoding="utf-8") as handle:
            return json.load(handle)

    def save(self, request):
        os.makedirs(self.requests, exist_ok=True)
        with open(self.request_file(request["id"]), "w", encoding="utf-8", newline="\n") as handle:
            json.dump(request, handle, indent=2, ensure_ascii=False)
            handle.write("\n")

    def generated_dir(self, request, revision=None):
        return self.path("art", "generated", request["id"], "r%d" % (revision or request["revision"]))


def now():
    return datetime.datetime.now(datetime.timezone.utc).replace(microsecond=0).isoformat()


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as handle:
        for block in iter(lambda: handle.read(1 << 20), b""):
            digest.update(block)
    return digest.hexdigest()


def image_info(path):
    from PIL import Image
    with Image.open(path) as image:
        has_alpha = image.mode in ("RGBA", "LA", "PA") or (image.mode == "P" and "transparency" in image.info)
        return {"width": image.width, "height": image.height, "mode": image.mode, "hasAlpha": has_alpha}


def file_record(queue, path):
    record = {"path": os.path.relpath(path, queue.root).replace(os.sep, "/"), "sha256": sha256(path)}
    try:
        record.update(image_info(path))
    except Exception as error:  # an unreadable image is recorded, and check/validate report it
        record["error"] = str(error)
    return record


def log(request, event, by, notes=None, **extra):
    entry = {"at": now(), "event": event, "revision": request["revision"], "by": by}
    if notes:
        entry["notes"] = notes
    entry.update(extra)
    request["history"].append(entry)


def power_of_two(value):
    return value > 0 and value & (value - 1) == 0


def problems_for(queue, request, expected_id=None):
    problems = []
    rid = request.get("id", "?")
    for field in REQUIRED:
        if field not in request:
            problems.append("%s: missing %s" % (rid, field))
    if problems:
        return problems
    if expected_id and rid != expected_id:
        problems.append("%s: file name says %s" % (rid, expected_id))
    if not ID_PATTERN.match(rid):
        problems.append("%s: id must look like ART-001" % rid)
    if request["status"] not in STATUSES:
        problems.append("%s: status %r not in %s" % (rid, request["status"], STATUSES))
    if request["priority"] not in PRIORITIES:
        problems.append("%s: priority %r not in %s" % (rid, request["priority"], PRIORITIES))
    if request["kind"] not in KINDS:
        problems.append("%s: kind %r not in %s" % (rid, request["kind"], KINDS))
    if request["transparency"] not in TRANSPARENCY:
        problems.append("%s: transparency %r not in %s" % (rid, request["transparency"], TRANSPARENCY))
    if not str(request["prompt"]).strip() or not str(request["intendedUse"]).strip():
        problems.append("%s: prompt and intendedUse must not be empty" % rid)
    if not request["styleConstraints"] or not request["prohibited"]:
        problems.append("%s: styleConstraints and prohibited must not be empty" % rid)
    dims = request["dimensions"]
    width, height = dims.get("width", 0), dims.get("height", 0)
    if width <= 0 or height <= 0:
        problems.append("%s: dimensions must be positive" % rid)
    elif request["kind"] in ("texture", "decal"):
        if not (power_of_two(width) and power_of_two(height)):
            problems.append("%s: %s sides must be powers of two (%dx%d)" % (rid, request["kind"], width, height))
        if max(width, height) > MAX_TEXTURE_SIDE and not request.get("oversizeReason"):
            problems.append("%s: over %d px needs oversizeReason" % (rid, MAX_TEXTURE_SIDE))
    expected_output = "art/generated/%s/" % rid
    if request["outputPath"] != expected_output:
        problems.append("%s: outputPath must be %s" % (rid, expected_output))
    if request["status"] in ("approved", "prepped", "integrated") and not request.get("approvedFile"):
        problems.append("%s: status %s without approvedFile" % (rid, request["status"]))
    if request.get("approvedFile") and not os.path.isfile(queue.path(request["approvedFile"])):
        problems.append("%s: approvedFile %s is missing" % (rid, request["approvedFile"]))
    if request["status"] == "generated" and not generated_files(queue, request):
        problems.append("%s: status generated but %s is empty" % (rid, queue.generated_dir(request)))
    if request["status"] in ("prepped", "integrated") and not os.path.isfile(prepped_path(queue, request)):
        problems.append("%s: status %s but the prepped file is missing" % (rid, request["status"]))
    return problems


def generated_files(queue, request):
    folder = queue.generated_dir(request)
    if not os.path.isdir(folder):
        return []
    return sorted(os.path.join(folder, f) for f in os.listdir(folder) if f.lower().endswith(IMAGE_EXTENSIONS))


def prepped_path(queue, request):
    return queue.path("art", "approved", request["id"], "prepped", os.path.splitext(request["outputName"])[0] + ".png")


def technical_problems(request, info):
    problems = []
    want_w, want_h = request["dimensions"]["width"], request["dimensions"]["height"]
    if "error" in info:
        return ["unreadable image: " + info["error"]]
    if abs(info["width"] / info["height"] - want_w / want_h) > ASPECT_TOLERANCE * (want_w / want_h):
        problems.append("aspect %dx%d does not match %dx%d" % (info["width"], info["height"], want_w, want_h))
    if info["width"] < want_w or info["height"] < want_h:
        problems.append("%dx%d is smaller than the final %dx%d (would be upscaled)" % (info["width"], info["height"], want_w, want_h))
    if request["transparency"] != "none" and not info["hasAlpha"]:
        problems.append("transparency %s requested but the image has no alpha channel" % request["transparency"])
    return problems


# ---------------------------------------------------------------- commands

def cmd_new(queue, args):
    with open(queue.style, encoding="utf-8") as handle:
        style = json.load(handle)
    numbers = [int(i.split("-")[1]) for i in queue.ids()]
    art_id = "ART-%03d" % ((max(numbers) if numbers else 0) + 1)
    width, height = (int(v) for v in args.size.lower().split("x"))
    gcd = _gcd(width, height)
    request = {
        "id": art_id, "title": args.title, "status": "requested", "priority": args.priority, "kind": args.kind,
        "prompt": args.prompt, "intendedUse": args.use,
        "dimensions": {"width": width, "height": height}, "aspectRatio": "%d:%d" % (width // gcd, height // gcd),
        "transparency": args.transparency,
        "styleConstraints": list(style["styleConstraints"]), "prohibited": list(style["prohibited"]),
        "outputPath": "art/generated/%s/" % art_id, "outputName": args.name or art_id.lower().replace("-", "_") + ".png",
        "consumer": args.consumer, "revision": 1, "history": [],
    }
    log(request, "requested", args.by, prompt=args.prompt)
    problems = problems_for(queue, request)
    if problems:
        raise SystemExit("\n".join(problems))
    queue.save(request)
    print("%s requested: %s" % (art_id, queue.request_file(art_id)))
    return 0


def _gcd(a, b):
    while b:
        a, b = b, a % b
    return a or 1


def cmd_list(queue, args):
    for art_id in queue.ids():
        r = queue.load(art_id)
        if args.status and r["status"] != args.status:
            continue
        print("%-8s %-10s %-3s r%-2d %-8s %s" % (r["id"], r["status"], r["priority"], r["revision"], r["kind"], r["title"]))
    return 0


def cmd_validate(queue, args):
    problems = []
    for art_id in queue.ids():
        try:
            problems += problems_for(queue, queue.load(art_id), art_id)
        except (ValueError, KeyError) as error:
            problems.append("%s: unreadable request (%s)" % (art_id, error))
    for area in ("generated", "approved", "rejected"):
        base = queue.path("art", area)
        if os.path.isdir(base):
            for name in os.listdir(base):
                if ID_PATTERN.match(name) and name not in queue.ids():
                    problems.append("art/%s/%s has no request" % (area, name))
    for problem in problems:
        print("ERROR " + problem)
    print("art-queue: %s requests=%d problems=%d" % ("ok" if not problems else "FAILED", len(queue.ids()), len(problems)))
    return 1 if problems else 0


def cmd_register(queue, args):
    request = queue.load(args.id)
    files = generated_files(queue, request)
    if not files:
        raise SystemExit("no images in %s" % queue.generated_dir(request))
    records = [file_record(queue, f) for f in files]
    log(request, "generated", args.by, args.notes, model=args.model, prompt=request["prompt"], files=records)
    request["status"] = "generated"
    queue.save(request)
    print("%s r%d: %d file(s) registered" % (args.id, request["revision"], len(records)))
    return 0


def cmd_check(queue, args):
    request = queue.load(args.id)
    files = generated_files(queue, request)
    if not files:
        raise SystemExit("no images in %s" % queue.generated_dir(request))
    worst = 0
    for f in files:
        issues = technical_problems(request, file_record(queue, f))
        print("%s: %s" % (os.path.basename(f), "ok" if not issues else "; ".join(issues)))
        worst = worst or (1 if issues else 0)
    return worst


def cmd_approve(queue, args):
    request = queue.load(args.id)
    source = os.path.join(queue.generated_dir(request), args.file)
    if not os.path.isfile(source):
        raise SystemExit("not found: " + source)
    issues = technical_problems(request, file_record(queue, source))
    if issues and not args.force:
        raise SystemExit("technical check failed (use --force to approve anyway): " + "; ".join(issues))
    folder = queue.path("art", "approved", request["id"])
    os.makedirs(folder, exist_ok=True)
    target = os.path.join(folder, "r%d_%s" % (request["revision"], args.file))
    shutil.copy2(source, target)
    record = file_record(queue, target)
    request["approvedFile"] = record["path"]
    request["status"] = "approved"
    log(request, "approved", args.by, args.notes, files=[record], technicalIssues=issues or None)
    queue.save(request)
    print("%s approved: %s" % (args.id, record["path"]))
    return 0


def cmd_reject(queue, args):
    request = queue.load(args.id)
    source = queue.generated_dir(request)
    moved = []
    if os.path.isdir(source):
        target = queue.path("art", "rejected", request["id"], "r%d" % request["revision"])
        os.makedirs(os.path.dirname(target), exist_ok=True)
        if os.path.exists(target):
            shutil.rmtree(target)
        shutil.move(source, target)
        moved = [file_record(queue, os.path.join(target, f)) for f in sorted(os.listdir(target))]
    log(request, "rejected", args.by, args.notes, files=moved)
    if args.revise_prompt:
        request["revision"] += 1
        request["prompt"] = args.revise_prompt
        request["status"] = "requested"
        log(request, "requested", args.by, "revised after rejection", prompt=args.revise_prompt)
    else:
        request["status"] = "rejected"
    queue.save(request)
    print("%s rejected; status %s, revision %d" % (args.id, request["status"], request["revision"]))
    return 0


def cmd_prep(queue, args):
    from PIL import Image
    request = queue.load(args.id)
    if request["status"] not in ("approved", "prepped", "integrated"):
        raise SystemExit("%s is %s; approve it first" % (args.id, request["status"]))
    width, height = request["dimensions"]["width"], request["dimensions"]["height"]
    mode = "RGB" if request["transparency"] == "none" else "RGBA"
    with Image.open(queue.path(request["approvedFile"])) as image:
        prepared = image.convert(mode)
        if prepared.size != (width, height):
            prepared = prepared.resize((width, height), Image.LANCZOS)
        if request["transparency"] == "cutout":
            alpha = prepared.getchannel("A").point(lambda v: 255 if v >= 128 else 0)
            prepared.putalpha(alpha)
        target = prepped_path(queue, request)
        os.makedirs(os.path.dirname(target), exist_ok=True)
        prepared.save(target, "PNG")
    outputs = [file_record(queue, target)]
    consumer = request.get("consumer")
    if consumer:
        destination = queue.path(consumer)
        os.makedirs(os.path.dirname(destination), exist_ok=True)
        shutil.copy2(target, destination)
        outputs.append(file_record(queue, destination))
    request["status"] = "prepped"
    log(request, "prepped", args.by, "final %dx%d %s; mipmaps and DXT compression are done by the content compiler" % (width, height, mode), files=outputs)
    queue.save(request)
    print("%s prepped: %s" % (args.id, ", ".join(o["path"] for o in outputs)))
    return 0


def cmd_mark(queue, args):
    request = queue.load(args.id)
    if args.status != "integrated" or request["status"] != "prepped":
        raise SystemExit("only a prepped request can be marked integrated")
    request["status"] = "integrated"
    log(request, "integrated", args.by, args.notes)
    queue.save(request)
    print("%s integrated" % args.id)
    return 0


def cmd_selftest(_queue, _args):
    from PIL import Image
    here = os.path.dirname(os.path.abspath(__file__))
    real_style = os.path.join(here, "..", "..", "docs", "art", "style.json")
    failures, passes = [], []

    def expect(condition, label):
        print(("  ok   " if condition else "  FAIL ") + label)
        (passes if condition else failures).append(label)

    with tempfile.TemporaryDirectory() as root:
        os.makedirs(os.path.join(root, "docs", "art"))
        shutil.copy(real_style, os.path.join(root, "docs", "art", "style.json"))
        q = Queue(root)
        run = lambda *argv: main(["--root", root] + list(argv))
        expect(run("new", "--title", "Test asphalt", "--prompt", "wet asphalt", "--use", "road texture", "--size", "256x256",
                   "--consumer", "content/props/x/x.png") == 0, "new creates ART-001")
        expect(q.ids() == ["ART-001"], "request file exists")
        try:
            run("new", "--title", "bad", "--prompt", "p", "--use", "u", "--size", "300x200")
            expect(False, "non power-of-two texture refused")
        except SystemExit:
            expect(True, "non power-of-two texture refused")
        r = q.load("ART-001")
        expect(r["styleConstraints"] and r["prohibited"] and r["outputPath"] == "art/generated/ART-001/", "style and output path filled")
        gen = q.generated_dir(r)
        os.makedirs(gen)
        Image.new("RGB", (512, 512), (60, 60, 64)).save(os.path.join(gen, "a.png"))
        Image.new("RGB", (512, 300), (60, 60, 64)).save(os.path.join(gen, "b.png"))
        expect(run("register", "ART-001", "--by", "image-agent", "--model", "test") == 0, "register")
        expect(q.load("ART-001")["status"] == "generated", "status generated")
        expect(run("check", "ART-001") == 1, "check flags the wrong aspect of b.png")
        try:
            run("approve", "ART-001", "b.png")
            expect(False, "approve refuses a technically bad file")
        except SystemExit:
            expect(True, "approve refuses a technically bad file")
        expect(run("reject", "ART-001", "--notes", "too clean", "--revise-prompt", "wet cracked asphalt") == 0, "reject with revision")
        r = q.load("ART-001")
        expect(r["revision"] == 2 and r["status"] == "requested" and os.path.isdir(os.path.join(root, "art", "rejected", "ART-001", "r1")),
               "revision 2 queued, r1 kept in rejected")
        os.makedirs(q.generated_dir(r))
        Image.new("RGB", (512, 512), (50, 52, 55)).save(os.path.join(q.generated_dir(r), "c.png"))
        run("register", "ART-001", "--by", "image-agent")
        expect(run("approve", "ART-001", "c.png", "--notes", "good") == 0, "approve")
        expect(run("prep", "ART-001") == 0, "prep")
        with Image.open(prepped_path(q, q.load("ART-001"))) as prepped:
            expect(prepped.size == (256, 256) and prepped.mode == "RGB", "prepped at final size, no alpha")
        expect(os.path.isfile(os.path.join(root, "content", "props", "x", "x.png")), "copied to the consumer path")
        r = q.load("ART-001")
        events = [h["event"] for h in r["history"]]
        expect(events == ["requested", "generated", "rejected", "requested", "generated", "approved", "prepped"], "full history: " + ",".join(events))
        expect(all("sha256" in f for h in r["history"] for f in h.get("files", [])), "every file has a hash")
        expect(run("validate") == 0, "validate passes")
        os.makedirs(os.path.join(root, "art", "generated", "ART-099"))
        expect(run("validate") == 1, "validate catches an orphan folder")
    print("art-queue selftest: %s passed=%d failed=%d" % ("ok" if not failures else "FAILED", len(passes), len(failures)))
    return 1 if failures else 0


def main(argv=None):
    parser = argparse.ArgumentParser(description="Liberty art-request queue")
    parser.add_argument("--root", default=os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", ".."))
    common = argparse.ArgumentParser(add_help=False)
    common.add_argument("--by", default="claude", help="who runs this step (recorded in the history)")
    sub = parser.add_subparsers(dest="command", required=True)
    add = lambda name: sub.add_parser(name, parents=[common])
    p = add("new")
    p.add_argument("--title", required=True); p.add_argument("--prompt", required=True); p.add_argument("--use", required=True)
    p.add_argument("--size", required=True); p.add_argument("--transparency", default="none", choices=TRANSPARENCY)
    p.add_argument("--priority", default="P1", choices=PRIORITIES); p.add_argument("--kind", default="texture", choices=KINDS)
    p.add_argument("--consumer"); p.add_argument("--name")
    p = add("list"); p.add_argument("--status", choices=STATUSES)
    add("validate")
    p = add("register"); p.add_argument("id"); p.add_argument("--model"); p.add_argument("--notes")
    p = add("check"); p.add_argument("id")
    p = add("approve"); p.add_argument("id"); p.add_argument("file"); p.add_argument("--notes"); p.add_argument("--force", action="store_true")
    p = add("reject"); p.add_argument("id"); p.add_argument("--notes"); p.add_argument("--revise-prompt")
    p = add("prep"); p.add_argument("id")
    p = add("mark"); p.add_argument("id"); p.add_argument("status", choices=("integrated",)); p.add_argument("--notes")
    add("selftest")
    args = parser.parse_args(argv)
    queue = Queue(args.root)
    return globals()["cmd_" + args.command](queue, args)


if __name__ == "__main__":
    sys.exit(main())
