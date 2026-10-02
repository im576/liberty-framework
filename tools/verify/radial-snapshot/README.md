# Focused radial opening regression

Run `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/tests/Run-RadialSnapshotChecks.ps1` from the lane root.
Requires the pinned compiler and an existing `sdk/Liberty.Sdk/bin/Liberty.Sdk.dll`; prints that DLL's SHA256.
Compiles only actual UiService, RadialMenuView, MenuInput, ResourceLedger, StorageWheel and supporting contracts/logic
plus these test boundaries into ignored `results-local/offline/lane-b-radial-snapshot/OfflineVerify.exe`.
No SDK/full engine/native build, full verifier, game files, installation or game launch.

The identical harness accepts `-BaselineRevision 2544b12` to extract pre-fix production sources into the ignored
baseline folder. That revision is expected to fail: it publishes a radial without evaluating an initial snapshot.
Tests inspect the real publication list and first draw, selected centre/highlight before any Update, input reads and
actions, owner context, actual ledger cleanup, failure isolation and the actual storage caller's first selected slot.
The native boundary throws on every call; art construction throws outside the simulated owner tick. Drawing must
use the prepared snapshot without invoking owner callbacks, input reads or art construction.

SHDN, Canvas/TextureStore/RadialArt, input hardware, gunsmith descriptors and engine dispatch are spies.
The RunAs spy models owner context, fault logging and ledger release; it does not execute production module stop,
SHDN thread scheduling, texture IO, controller polling or native control locks. No runtime latency/budget acceptance
can be inferred. The production weapon module caller is source-reviewed; this focused assembly includes the actual
storage caller, not the complete weapon module. All real runtime gates remain pending a scheduled full run.
