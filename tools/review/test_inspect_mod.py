from pathlib import Path
import tempfile
import unittest
import zipfile
from inspect_mod import inspect


class ArchiveTests(unittest.TestCase):
    def test_inspection_is_non_executing_and_reports_traversal(self):
        with tempfile.TemporaryDirectory() as folder:
            archive = Path(folder) / "mod.zip"
            with zipfile.ZipFile(archive, "w") as output:
                output.writestr("src/MOD.CS", "// example")
                output.writestr("mod.asi", b"not executable")
                output.writestr("LICENSE", "MIT test fixture")
                output.writestr("../outside", "must not extract")
            report = inspect(archive)
            self.assertEqual(len(report["sourceFiles"]), 1)
            self.assertEqual(len(report["binaries"]), 1)
            self.assertEqual(report["unsafeEntryPaths"], ["../outside"])
            self.assertEqual(report["licenseNotices"][0]["text"], "MIT test fixture")
            self.assertEqual(list(Path(folder).iterdir()), [archive])


if __name__ == "__main__":
    unittest.main()
