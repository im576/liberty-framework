# Exercise the real host command reader against a delayed, partially published reply.
# Extract only this function so the fixture cannot launch/control a real game or load UI helpers.
& {
$ErrorActionPreference = 'Stop'
$repo = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$parseErrors = $null
$parseTokens = $null
$ast = [Management.Automation.Language.Parser]::ParseFile((Join-Path $repo 'tools/autopilot/Autopilot.psm1'), [ref]$parseTokens, [ref]$parseErrors)
if ($parseErrors.Count) { throw 'Autopilot source syntax error.' }
$definition = $ast.Find({ param($node) $node -is [Management.Automation.Language.FunctionDefinitionAst] -and $node.Name -eq 'Invoke-EngineCommand' }, $true)
. ([scriptblock]::Create($definition.Extent.Text))
function Get-GameProcess { return $true }
$fixture = Join-Path ([IO.Path]::GetTempPath()) ('lf-reply-test-' + [Guid]::NewGuid().ToString('N'))
$script:Game = $fixture
$channel = Join-Path $fixture 'scripts/LibertyFramework/autopilot'
New-Item -ItemType Directory -Path (Join-Path $channel 'inbox'), (Join-Path $channel 'outbox') -Force | Out-Null
$publisher = Start-Job -ArgumentList $channel -ScriptBlock {
    param($channel)
    $deadline = [DateTime]::UtcNow.AddSeconds(20)
    do {
        $command = Get-ChildItem -LiteralPath (Join-Path $channel 'inbox') -Filter '*.cmd' | Select-Object -First 1
        if (-not $command) { Start-Sleep -Milliseconds 50 }
    } while (-not $command -and [DateTime]::UtcNow -lt $deadline)
    if (-not $command) { throw 'No fixture command received.' }
    $output = Join-Path $channel ('outbox/' + $command.BaseName + '.out')
    [IO.File]::WriteAllText($output, '')
    Start-Sleep -Milliseconds 750
    [IO.File]::WriteAllLines($output, @('alive => player alive health=100'))
    Start-Sleep -Milliseconds 750
    [IO.File]::WriteAllText($output, "alive => player alive health=100`n" + 'pos => 1')
    Start-Sleep -Milliseconds 750
    [IO.File]::WriteAllLines($output, @('alive => player alive health=100', 'pos => 1 2 3 heading 0'))
}
try {
    $reply = @(Invoke-EngineCommand @('alive', '# ignored', '', 'pos') -TimeoutSeconds 20)
    if ($reply.Count -ne 2 -or $reply[1] -ne 'pos => 1 2 3 heading 0') { throw 'Empty or partial reply was accepted.' }
    $publisher | Wait-Job -Timeout 5 | Out-Null
    $publisher | Receive-Job -ErrorAction Stop | Out-Null
    if (Get-ChildItem -LiteralPath (Join-Path $channel 'outbox') -Filter '*.out') { throw 'Completed reply was not consumed.' }
    if (Get-Command Test-That -ErrorAction SilentlyContinue) {
        Test-That 'real command reader waits for empty, partial and unterminated replies; ignores comments and consumes complete reply' $true
    } else {
        Write-Host 'PASS: delayed empty/partial replies remain pending; complete two-command reply consumed; blank/comment commands excluded.'
    }
} finally {
    $publisher | Stop-Job
    $publisher | Remove-Job
    $resolved = [IO.Path]::GetFullPath($fixture)
    if (-not $resolved.StartsWith([IO.Path]::GetFullPath([IO.Path]::GetTempPath()), [StringComparison]::OrdinalIgnoreCase)) { throw 'Fixture cleanup path escaped temp.' }
    Remove-Item -LiteralPath $resolved -Recurse -Force
}
}
