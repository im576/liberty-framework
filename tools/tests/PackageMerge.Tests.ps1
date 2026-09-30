# tools/PackageMerge.psm1: how package-phase2.ps1 merges files the owner's install already has.
Import-Module (Join-Path $script:RepoRoot 'tools/PackageMerge.psm1') -Force

$template = '{ "schemaVersion": 1, "locations": [ { "id": "gun_test_range", "x": 1041.0, "y": -568.3, "z": 20.0, "heading": 0.0, "snap": "pavement" }, { "id": "s1_boabo", "x": 890.8, "y": 416.8, "z": 13.1, "heading": 296.0, "snap": "none" }, { "id": "s1_east_hook", "x": 833.3, "y": -157.0, "z": 6.0, "heading": 335.0, "snap": "none" } ] }'
$installed = '{ "schemaVersion": 1, "locations": [ { "id": "gun_test_range", "x": 1.5, "y": 2.5, "z": 3.5, "heading": 90.0, "snap": "none" }, { "id": "my_spot", "x": 5.0, "y": 6.0, "z": 7.0, "heading": 0.0, "snap": "none" } ] }'
$merge = Merge-LocationFiles $template $installed
$result = $merge.Json | ConvertFrom-Json
$byId = @{}; foreach ($l in @($result.locations)) { $byId[[string]$l.id] = $l }
Test-That 'locations merge: missing capture points are added' ($merge.Added.Count -eq 2 -and $byId.ContainsKey('s1_boabo') -and $byId.ContainsKey('s1_east_hook'))
Test-That 'locations merge: the owner''s saved gun test range is kept, not overwritten' ($byId['gun_test_range'].x -eq 1.5 -and $byId['gun_test_range'].heading -eq 90.0 -and $byId['gun_test_range'].snap -eq 'none')
Test-That 'locations merge: the owner''s own entries are kept' ($byId.ContainsKey('my_spot') -and @($result.locations).Count -eq 4)
$again = Merge-LocationFiles $template $merge.Json
Test-That 'locations merge: merging again adds nothing (idempotent)' ($again.Added.Count -eq 0 -and @(($again.Json | ConvertFrom-Json).locations).Count -eq 4)
$single = Merge-LocationFiles '{ "schemaVersion": 1, "locations": [ { "id": "a", "x": 1, "y": 2, "z": 3, "heading": 0, "snap": "none" } ] }' '{ "schemaVersion": 1, "locations": [] }'
Test-That 'locations merge: a one-entry result is still a JSON array' ($single.Json -match '"locations":\s*\[') $single.Json
$threw = $false
try { Merge-LocationFiles '{ "schemaVersion": 2, "locations": [] }' $installed | Out-Null } catch { $threw = $true }
Test-That 'locations merge: a different schemaVersion is refused, not merged' $threw
