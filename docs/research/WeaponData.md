# Weapon data and separate test weapons

The brief asks for three gold weapons that can be compared with vanilla equivalents. A visual gold marker alone does not prove isolation: the game must identify the selected weapon and route only that identifier to new behavior.

## Candidate route

FusionFix documents file overriding in its [README](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix/blob/master/readme.md). Its source/config includes episodic weapon support. This suggests a route using `weaponinfo.xml` and unused or episode-specific weapon identifiers. **[HYPOTHESIS]** Slot availability, model mapping, mission interactions, save persistence, and coexistence with vanilla weapons need T-007 testing.

## T-007 questions

1. Which weapon IDs are truly unused in IV and EFLC on CE 1.2.0.59?
2. Can each be given/spawned and distinguished through ScriptHookDotNet?
3. Can an overload provide a separate model/texture without overwriting a vanilla asset?
4. What happens after save/load, death, cutscene, vehicle entry, cover, and mission weapon changes?

Do not ship ripped assets or assert a spare slot is safe before the test. A lawful temporary visual marker can be used in a private spike; a reusable finish path is a later design task.
