# Merges used by package-phase2.ps1 when a config file already exists in the owner's install (tests: tools/tests/PackageMerge.Tests.ps1).
# Compatible with Windows PowerShell 5.1 and PowerShell 7.

# config/devtools/locations.json: the installed file is the owner's (DevTools "Set Gun Test Range Here" rewrites it), so it
# is never replaced. Entries of the repository file whose id is missing are appended (new capture points reach an existing
# install); entries that already exist keep the owner's values. Returns the merged JSON text and the ids that were added.
function Merge-LocationFiles([string] $TemplateJson, [string] $InstalledJson) {
    $template = $TemplateJson | ConvertFrom-Json
    $saved = $InstalledJson | ConvertFrom-Json
    if ($saved.schemaVersion -ne $template.schemaVersion) { throw 'Cannot merge locations: schemaVersion differs' }
    $locations = New-Object System.Collections.Generic.List[object]
    $known = @{}
    foreach ($location in @($saved.locations)) { $locations.Add($location); $known[[string]$location.id] = $true }
    $added = New-Object System.Collections.Generic.List[string]
    foreach ($location in @($template.locations)) {
        if ($known.ContainsKey([string]$location.id)) { continue }
        $locations.Add($location)
        $added.Add([string]$location.id)
    }
    $saved.locations = $locations.ToArray()
    return @{ Json = ($saved | ConvertTo-Json -Depth 32); Added = $added.ToArray() }
}

Export-ModuleMember -Function Merge-LocationFiles
