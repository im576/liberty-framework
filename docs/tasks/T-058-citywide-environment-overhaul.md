# T-058: Citywide environment overhaul

Status: IN-PROGRESS

## Owner direction (2026-10-02)

Begin the approved visual overhaul across all Liberty City. One accountable implementation and one integrated candidate covering all weather/time conditions; avoid agent handoffs and elongated separate passes. This supersedes the earlier art-only hold and station-only prototype sequence for this work. Existing gameplay and density OFF remain intact. No new workers dispatched.

## Scope and acceptance

Citywide lighting, weather appearance and color: gritty grey/rain atmosphere, natural color and readable brighter nights with stronger existing light/sign accents. Eight weather types, eleven time samples, both source timecycle tables. Reuse FusionFix/DXVK and the existing mood generator. Sky/cloud RGB controls use verified installed X360-layout header columns. No added geometry or fictional light fixtures.

Approved references: results-local/art-direction/20261001/ai-tuning/star-junction-batch-v02.json and station-batch/station-batch-v01.json; Night C is the approved night direction. Actual appearance is judged against these targets, not merely against a brighter numeric preset.

One integrated capture batch covers all five benchmark conditions at the station, Star Junction, Hove Beach shops and East Hook docks. Global file overrides apply citywide; four sampled sites do not prove every interior/mission or pixel-equivalence to the AI images. Material/light/sky capability gaps found in review remain part of the overhaul; this initial implementation must not be called the full completed remaster.

## Changes

- `config/mood.json`: author all weather/time conditions together; remove blanket dark/desaturated night direction, preserve readable ambient light and natural accents, neutral grey/rain skies.
- `tools/mood/MoodTimecycle.cs`: optional sky/cloud RGB overrides and validation against the verified column layout.
- `tools/install-mood.ps1`: use the machine-wide game lock, validate pristine sources, stage output before mutation, exact pre-install backup/receipt and `-Rollback`.
- `tools/mood/check-citywide.py`: actual source/generator coverage, unchanged unrelated columns, extended-field isolation, deterministic output and invalid RGB rejection.
- `tools/mood/Run-CitywideReview.ps1`: one bounded installation/capture run under the game lock, dynamic player-position cleanup and exact rollback on runtime/cleanup failure. Restarting an already-open game requires human authorization.

## Evidence

See `results-local/citywide-overhaul/` for generator, installer rollback, runtime and visual review receipts. Status stays IN-PROGRESS until runtime screenshots are inspected; NEEDS-PLAYTEST is not owner acceptance.

Owner authorized restart on continuation. Three Steam candidate startup attempts failed before engine load and rolled back; a restored-baseline control reached gameplay with a FusionFix shader-warning acknowledgment. One controlled candidate retry and one direct-local launch also failed before gameplay and rolled back. Windows fault records report 0xc0000005/unknown module; the preserved module list includes MTLX/DXVK/ASI loader but no FusionFix. This suggests an early startup problem, not a proven timecycle cause. No candidate screenshots exist and visual acceptance remains unproven. Exact original timecycle hashes are restored; see final-mood-restoration.json. Do not mark the overhaul complete.

The direct-local diagnostic's boot.txt contains the earlier baseline boot because its helper had no new session boundary. That receipt is explicitly invalid as candidate readiness evidence; its pending command failed after process exit. Subsequent runtime work must use Start-GameReady's session boundary or filter by the new launch timestamp.

The final two baseline-return launch attempts produced no observed GTAIV process. Baseline files remain restored and no game/test process or lock remains. Stop automatic retry loops. Next external-state check: owner launches GTA IV normally; then continue the candidate's actual runtime/visual review. No full-overhaul completion or candidate screenshot pass is claimed.

## Human test steps

1. Launch GTA IV with the installed candidate. Walk and drive through Broker and Algonquin in ordinary free roam.
2. Check cloudy/rain daylight: grey atmosphere and distance depth, readable pedestrians/platform shade, natural material colors.
3. Check clear daylight and dusk: distinct sunlight and dusk, with no sudden color/exposure jumps during the clock cycle.
4. Check night, including rain: readable unlit roads/subjects; existing signs/lights retain color, text and local separation without broad bloom washout.
5. Enter/exit a safehouse or tunnel and run a mission/cutscene that changes weather. Check return to normal gameplay and HUD/reticle readability.
6. Compare the single review page's original/AI target/actual frames. Record remaining gaps and feel/performance feedback.
7. With the game closed, `tools/install-mood.ps1 -GameDirectory <game> -Rollback` restores the exact pre-install look. `-Restore` instead explicitly restores pristine FusionFix and is not the owner's prior-baseline rollback.

## Remaining acceptance

Moving gameplay/presented frame-time comparison, interior/mission exposure and owner visual sign-off. Texture/material upgrades and localized lighting are not implemented by the global grade; annotate actual target gaps before selecting those changes.
