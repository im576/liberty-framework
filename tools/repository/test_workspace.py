import importlib.util
import json
import pathlib
import subprocess
import tempfile
import unittest

spec = importlib.util.spec_from_file_location('workspace', pathlib.Path(__file__).with_name('workspace.py'))
workspace = importlib.util.module_from_spec(spec)
spec.loader.exec_module(workspace)


class WorkspaceTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.base = pathlib.Path(self.temp.name)
        self.framework = self.repo('framework')
        self.mod = self.repo('mod')
        self.write(self.framework, 'tools/build.ps1', 'framework-build')
        self.write(self.framework, 'tools/verify.ps1', 'framework-verify')
        self.write(self.framework, 'tools/verify/Program.cs', 'framework-tests')
        self.write(self.framework, 'config/engine.json', '{}')
        self.write(self.framework, 'src/LibertyFramework/Engine/Host.cs', 'engine')
        self.write(self.mod, 'src/LibertyPlus/Hud/HudModule.cs', 'mod-hud')
        self.write(self.mod, 'tools/build.ps1', 'mod-build')
        self.write(self.mod, 'tools/integration/build.ps1', 'combined-build')
        self.write(self.mod, 'tools/integration/verify.ps1', 'combined-verify')
        self.write(self.mod, 'tools/integration/VerifyProgram.cs', 'combined-tests')
        self.write(self.mod, 'config/runtime/engine.json', '{"profile":"preview"}')

    def tearDown(self):
        self.temp.cleanup()

    def repo(self, name):
        root = self.base/name
        root.mkdir()
        subprocess.run(['git','init','-q',str(root)],check=True)
        subprocess.run(['git','-C',str(root),'-c','user.name=Fixture','-c','user.email=fixture@example.invalid',
                        'commit','--allow-empty','-qm','fixture'],check=True)
        self.write(root,'.gitignore','results-local/\n*.dll\n')
        return root

    def write(self, root, name, text):
        path=root/name
        path.parent.mkdir(parents=True,exist_ok=True)
        path.write_text(text)

    def test_separate_sources_and_explicit_overrides(self):
        root=workspace.create(self.framework,self.mod)
        self.assertEqual((root/'src/LibertyPlus/Hud/HudModule.cs').read_text(),'mod-hud')
        self.assertEqual((root/'src/LibertyFramework/Hud/HudModule.cs').read_text(),'mod-hud')
        self.assertEqual((root/'tools/build-framework.ps1').read_text(),'framework-build')
        self.assertEqual((root/'tools/build.ps1').read_text(),'combined-build')
        data=json.loads((root/'workspace.json').read_text())
        self.assertEqual(data['ownership']['src/LibertyFramework/Hud/HudModule.cs']['repo'],str(self.mod))
        self.assertEqual(data['ownership']['src/LibertyFramework/Engine/Host.cs']['repo'],str(self.framework))

    def test_ignored_outputs_and_game_files_are_not_exported(self):
        self.write(self.mod,'bad.dll','ignored-binary')
        self.write(self.mod,'results-local/log.txt','ignored-log')
        root=workspace.create(self.framework,self.mod)
        self.assertFalse((root/'bad.dll').exists())
        self.assertNotIn('results-local/log.txt',json.loads((root/'workspace.json').read_text())['repositories']['mod']['files'])

    def test_untracked_source_changes_are_hashed(self):
        root=workspace.create(self.framework,self.mod)
        data=json.loads((root/'workspace.json').read_text())
        self.assertTrue(data['repositories']['mod']['dirty'])
        self.assertIn('src/LibertyPlus/Hud/HudModule.cs',data['repositories']['mod']['files'])

    def test_collision_is_rejected(self):
        self.write(self.framework,'config/weapon.json','framework')
        self.write(self.mod,'config/weapon.json','mod')
        with self.assertRaisesRegex(ValueError,'Ownership collision'):
            workspace.create(self.framework,self.mod)

    def test_workspace_is_never_reused_or_overwritten(self):
        first=workspace.create(self.framework,self.mod)
        (first/'sentinel.txt').write_text('preserve')
        second=workspace.create(self.framework,self.mod)
        self.assertNotEqual(first,second)
        self.assertEqual((first/'sentinel.txt').read_text(),'preserve')


if __name__=='__main__':
    unittest.main()
