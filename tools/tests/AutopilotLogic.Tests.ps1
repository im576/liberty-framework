# Unit tests for tools/autopilot/AutopilotLogic.psm1: the decisions that make an autopilot PASS trustworthy.
Import-Module (Join-Path $script:RepoRoot 'tools/autopilot/AutopilotLogic.psm1') -Force

# expect line parsing
$e = ConvertFrom-ExpectLine 'expect "selftest_done passed=\d+ failed=0" 90'
Test-That 'expect: quoted pattern with timeout' ($e.Pattern -eq 'selftest_done passed=\d+ failed=0' -and $e.TimeoutSeconds -eq 90)
$e = ConvertFrom-ExpectLine 'expect holster_sling_attached'
Test-That 'expect: bare pattern, default 20 s' ($e.Pattern -eq 'holster_sling_attached' -and $e.TimeoutSeconds -eq 20)
Test-That 'expect: invalid regex is rejected, not matched' ($null -eq (ConvertFrom-ExpectLine 'expect "event (unclosed" 5'))
Test-That 'expect: missing pattern is rejected' ($null -eq (ConvertFrom-ExpectLine 'expect'))

# Which log line an expect may accept
$log = @(
    '2026-09-26T10:00:00.000Z [INFO] autopilot_spawned count=2',
    '2026-09-26T10:00:05.000Z [INFO] command source=file:cmd_1.cmd line="spawn 2 5" reply="ok"',
    '2026-09-26T10:00:05.100Z [INFO] autopilot_spawned count=1'
)
Test-That 'stale line before the mark is not accepted' ($null -eq (Find-ExpectedLine $log 1 'autopilot_spawned count=2'))
Test-That 'line written after the mark is accepted' ((Find-ExpectedLine $log 1 'autopilot_spawned count=1') -eq $log[2])
Test-That 'the same line is accepted when it is after the mark' ((Find-ExpectedLine $log 0 'autopilot_spawned count=2') -eq $log[0])
$echo = @('2026-09-26T10:00:05.000Z [INFO] command source=file:cmd_2.cmd line="gore arm" reply="ok"')
Test-That 'a pattern matching only the command as typed is not accepted' ($null -eq (Find-ExpectedLine $echo 0 'gore arm'))
$reply = @('2026-09-26T10:00:05.000Z [INFO] command source=file:cmd_3.cmd line="owned autopilot" reply="autopilot: nothing"')
Test-That 'a pattern that needs the reply is accepted (hot-reload scenario)' ((Find-ExpectedLine $reply 0 'line=.owned autopilot. reply=.autopilot: nothing') -eq $reply[0])
Test-That 'a reply-only pattern is accepted (inspector-review scenario)' ((Find-ExpectedLine @('2026-09-26T10:00:05.000Z [INFO] command source=file:c line="inspector on" reply="inspector on (F10)"') 0 'reply=.inspector on') -ne $null)
Test-That 'a log that shrank since the mark is read from its start' ((Find-ExpectedLine @('x [INFO] engine_booted') 5 'engine_booted') -eq 'x [INFO] engine_booted')
Test-That 'no log lines: nothing accepted' ($null -eq (Find-ExpectedLine @() 0 'anything'))

# Command replies
Test-That 'reply ok is not a failure' (-not (Test-CommandFailed @('god on => ok')))
Test-That 'reply error is a failure' (Test-CommandFailed @('ray down => error NullReferenceException'))
Test-That 'unknown command is a failure' (Test-CommandFailed @('expcet foo => unknown command expcet'))
Test-That 'command of a stopped module is a failure' (Test-CommandFailed @('spawn 1 => module autopilot is not running'))
Test-That 'no reply is a failure' (Test-CommandFailed @())
Test-That 'a reply mentioning errors=0 is not a failure' (-not (Test-CommandFailed @('raystats => raycast available=True errors=0')))

# Scenario status
Test-That 'all steps passed, game alive: PASS' ((Get-ScenarioStatus 5 0 $true '') -eq 'PASS')
Test-That 'a failed step: FAIL' ((Get-ScenarioStatus 5 1 $true '') -eq 'FAIL')
Test-That 'game gone at the end: CRASH even if every step passed' ((Get-ScenarioStatus 5 0 $false '') -eq 'CRASH')
Test-That 'nothing executed: ERROR, never PASS' ((Get-ScenarioStatus 0 0 $true '') -eq 'ERROR')
Test-That 'runner error: ERROR' ((Get-ScenarioStatus 3 0 $true 'launch failed') -eq 'ERROR')
Test-That 'PASS with log errors needs review' ((Get-ReviewStatus 'PASS' 2) -eq 'NEEDS-REVIEW')
Test-That 'FAIL stays FAIL with log errors' ((Get-ReviewStatus 'FAIL' 2) -eq 'FAIL')

# Reading a scenario's result (the old suite called this output PASS because it contains the word "pass")
$resultFile = Join-Path $script:Scratch 'result.json'
[IO.File]::WriteAllText($resultFile, '{ "status": "FAIL", "summary": "FAIL; steps=4 failed=1" }')
$output = "scenario bypass-check: expect passed_through: OK`nAUTOPILOT_RESULT $resultFile"
Test-That 'status comes from result.json, not from words in the output' ((Read-ScenarioResult $output).Status -eq 'FAIL')
Test-That 'no AUTOPILOT_RESULT line: ERROR' ((Read-ScenarioResult 'scenario x: PASS').Status -eq 'ERROR')
Test-That 'two AUTOPILOT_RESULT lines: ERROR' ((Read-ScenarioResult "AUTOPILOT_RESULT $resultFile`nAUTOPILOT_RESULT $resultFile").Status -eq 'ERROR')
Test-That 'missing result file: ERROR' ((Read-ScenarioResult ('AUTOPILOT_RESULT ' + (Join-Path $script:Scratch 'nope.json'))).Status -eq 'ERROR')
[IO.File]::WriteAllText($resultFile, '{ "status": "GREAT" }')
Test-That 'unknown status in the result: ERROR' ((Read-ScenarioResult "AUTOPILOT_RESULT $resultFile").Status -eq 'ERROR')

# Scenario values from a probe of the same run ({probe:<id>:<field>})
$probes = Join-Path $script:Scratch 'probes'
New-Item -ItemType Directory -Force -Path $probes | Out-Null
[IO.File]::WriteAllText((Join-Path $probes 'PROBE-collision.json'), '{ "propCandidates": ["", "cj_crate_1", "cj_crate_2"], "empty": [], "odd": ["a b"] }')
Test-That 'probe value: the first non-empty list element replaces the token' ((Resolve-ScenarioLine 'spawnprop {probe:PROBE-collision:propCandidates} 3 0' $probes) -eq 'spawnprop cj_crate_1 3 0')
Test-That 'probe value: a line without a token is unchanged' ((Resolve-ScenarioLine 'rayto prop objects' $probes) -eq 'rayto prop objects')
$thrown = ''; try { Resolve-ScenarioLine 'spawnprop {probe:PROBE-collision:propCandidates} 3 0' '' | Out-Null } catch { $thrown = $_.Exception.Message }
Test-That 'probe value: no probe folder (run without the probe) fails the step' ($thrown -like 'needs PROBE-collision*') $thrown
$thrown = ''; try { Resolve-ScenarioLine 'x {probe:PROBE-drawables:any}' $probes | Out-Null } catch { $thrown = $_.Exception.Message }
Test-That 'probe value: a missing report fails the step' ($thrown -like '*PROBE-drawables.json is missing*') $thrown
$thrown = ''; try { Resolve-ScenarioLine 'x {probe:PROBE-collision:empty}' $probes | Out-Null } catch { $thrown = $_.Exception.Message }
Test-That 'probe value: an empty list fails the step (never a guessed value)' ($thrown -like '*no value*') $thrown
$thrown = ''; try { Resolve-ScenarioLine 'x {probe:PROBE-collision:missing}' $probes | Out-Null } catch { $thrown = $_.Exception.Message }
Test-That 'probe value: a missing field fails the step' ($thrown -like "*no field 'missing'*") $thrown
$thrown = ''; try { Resolve-ScenarioLine 'x {probe:PROBE-collision:odd}' $probes | Out-Null } catch { $thrown = $_.Exception.Message }
Test-That 'probe value: a value that is not one word is refused (no command injection)' ($thrown -like '*not a plain word*') $thrown
