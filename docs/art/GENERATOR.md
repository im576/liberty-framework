# Instructions for the image-generating agent

You generate images for the Liberty Vanilla+ project (a GTA IV remaster mod). You only read requests and write images;
you never edit game code, other requests or approved/rejected art.

1. `git pull`. Open every `docs/art/requests/ART-*.json` whose `status` is `requested`. Work in `priority` order
   (`P0` first), then by id.
2. For each request, generate from `prompt` exactly, honouring `styleConstraints`, `prohibited`, `aspectRatio` and
   `transparency` (`alpha`/`cutout` need a real alpha channel: PNG with transparency, not a white or black background).
   Generate at `dimensions` or larger **at the same aspect ratio** (square requests: 1024x1024 is fine for 512 and 1024). For `kind: concept` any size your generator supports is fine; use the nearest aspect ratio. One to four candidates.
3. Save them as PNG in `art/generated/<id>/r<revision>/` (the request's `outputPath` plus `r` and its `revision`), named
   `<outputName without .png>_1.png`, `_2.png`, ... Do not write anywhere else.
4. Register them: `python tools/art/artq.py register <id> --by <your name> --model <model and version> --notes "<seed or
   settings, anything unusual>"`. If you cannot run Python, append to the request's `history` an entry
   `{"at": <UTC ISO time>, "event": "generated", "revision": <revision>, "by": <you>, "model": <model>,
   "prompt": <the prompt used>, "files": [{"path": <repo-relative path>}]}` and set `status` to `generated`.
5. Commit only those files and that request change, with the message `ART-NNN r<revision>: generated`, and push.
6. If a prompt cannot be followed (policy refusal, impossible constraint), do not improvise: add a history entry with
   `"event": "blocked"` and the reason in `notes`, leave `status` as `requested`, commit and push.
