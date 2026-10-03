import importlib.util
import json
import pathlib
import tempfile
import unittest

spec=importlib.util.spec_from_file_location('fingerprint',pathlib.Path(__file__).with_name('fingerprint.py'))
fingerprint=importlib.util.module_from_spec(spec)
spec.loader.exec_module(fingerprint)


class FingerprintTests(unittest.TestCase):
    def test_contract_changes_but_line_endings_do_not(self):
        with tempfile.TemporaryDirectory() as temp:
            root=pathlib.Path(temp)
            source=root/'sdk/Api.cs'
            source.parent.mkdir()
            source.write_bytes(b'contract\r\n')
            (root/'workspace.json').write_text(json.dumps({'repositories':{'framework':{'files':{'sdk/Api.cs':'unused'}}}}))
            first=fingerprint.fingerprint(root)
            source.write_bytes(b'contract\n')
            self.assertEqual(first,fingerprint.fingerprint(root))
            source.write_bytes(b'changed contract\n')
            self.assertNotEqual(first['sourceSha256'],fingerprint.fingerprint(root)['sourceSha256'])

    def test_mod_mirrors_do_not_enter_contract(self):
        with tempfile.TemporaryDirectory() as temp:
            root=pathlib.Path(temp)
            source=root/'sdk/Api.cs'; source.parent.mkdir(); source.write_text('sdk')
            mod=root/'src/LibertyFramework/Hud/Mod.cs'; mod.parent.mkdir(parents=True); mod.write_text('mod')
            (root/'workspace.json').write_text(json.dumps({'repositories':{'framework':{'files':{'sdk/Api.cs':'unused'}}}}))
            self.assertEqual(fingerprint.fingerprint(root)['sourceFiles'],1)


if __name__=='__main__':
    unittest.main()
