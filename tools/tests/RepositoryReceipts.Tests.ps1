# Additional source identity is preserved through the existing installed-build receipt.
$module = Import-Module (Join-Path $script:RepoRoot 'tools/local/GameLock.psm1') -Force -PassThru
$fixture = Join-Path $script:Scratch 'repository-receipt'
$repositories = [ordered]@{
    framework = @{ repo = 'framework'; commit = '0123456789abcdef'; files = @{ 'sdk/source.cs' = 'sdkhash' } }
    mod = @{ repo = 'LibertyPlus'; commit = 'abcdef0123456789'; files = @{ 'src/Hud.cs' = 'hudhash' } }
}
Write-InstalledBuild -GameDirectory $fixture -Commit '0123456789abcdef' -Dirty $false -Repositories $repositories
$receipt = Read-InstalledBuild $fixture
Test-That 'split receipt preserves framework revision' ($receipt.repositories.framework.commit -eq '0123456789abcdef')
Test-That 'split receipt preserves mod revision' ($receipt.repositories.mod.commit -eq 'abcdef0123456789')
Test-That 'split receipt preserves nested input hashes' ($receipt.repositories.mod.files.'src/Hud.cs' -eq 'hudhash')
Test-That 'split receipt retains legacy short identity' ($receipt.commit -eq '0123456' -and -not $receipt.dirty)
Write-InstalledBuild -GameDirectory $fixture -Commit '0123456'
$legacy = Read-InstalledBuild $fixture
Test-That 'single-repo receipt remains backward compatible' ($legacy.commit -eq '0123456' -and -not $legacy.PSObject.Properties['repositories'])
