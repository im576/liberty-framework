# Last check/export/build result per scene. Memory only: results are cheap to recompute and must not be saved into the
# .blend file (or leak into glTF extras).


class Result:
    def __init__(self):
        self.issues = []   # checks.Issue: the add-on's checks first, then LibertyContent's output
        self.status = ""   # "", "checked", "exported", "ok", "invalid", "readback-failed", "failed"
        self.message = ""
        self.folder = ""   # asset folder the last export wrote
        self.report = ""   # report.json of the last build
        self.preview = ""  # geometry preview read back from the compiled .wdr
        self.texture = ""  # DXT1 texture decoded back from the compiled .wtd
        self.log = ""      # LibertyContent console output


_results = {}


def get(scene):
    return _results.setdefault(scene.name, Result())


def reset(scene):
    _results[scene.name] = Result()
    return _results[scene.name]


def clear():
    _results.clear()
