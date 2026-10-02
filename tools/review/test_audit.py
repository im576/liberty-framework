import json
from pathlib import Path
import tempfile
import unittest
from audit import summarize_receipts


class ReceiptTests(unittest.TestCase):
    def test_duplicates_modes_and_missing_timing(self):
        with tempfile.TemporaryDirectory() as folder:
            paths = [Path(folder) / f"{n}.json" for n in range(4)]
            receipt = {"run": {"id": "one", "commit": "abc", "mode": "quick"},
                       "checks": [{"id": "gore", "status": "FAIL", "seconds": 90}]}
            for path in paths[:2]:
                path.write_text(json.dumps(receipt))
            receipt["run"]["mode"] = "full"
            paths[2].write_text(json.dumps(receipt))
            paths[3].write_text("{bad json")
            report = summarize_receipts(paths)
            self.assertEqual(report["counts"], {"FAIL": 2})
            self.assertEqual(report["modes"], {"quick": 1, "full": 1})
            self.assertEqual(len(report["duplicates"]), 1)
            self.assertEqual(len(report["errors"]), 1)
            self.assertIsNone(report["runs"][0]["unattributedSeconds"])

    def test_conflicting_receipts_are_visible(self):
        with tempfile.TemporaryDirectory() as folder:
            paths = []
            for n, status in enumerate(("FAIL", "PASS")):
                path = Path(folder) / f"{n}.json"
                path.write_text(json.dumps({"run": {"id": "one", "commit": "abc", "mode": "full",
                    "startedUtc": "2026-10-01T00:00:00Z", "finishedUtc": "2026-10-01T00:02:00Z"},
                    "checks": [{"id": "gore", "status": status, "seconds": 90}]}))
                paths.append(path)
            report = summarize_receipts(paths)
            self.assertEqual(len(report["conflictingReceipts"]), 1)
            self.assertEqual(report["runs"][0]["unattributedSeconds"], 30)


if __name__ == "__main__":
    unittest.main()
