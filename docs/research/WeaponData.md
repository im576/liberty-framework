# Weapon data and separate test weapons

The brief asks for three gold weapons that can be compared with vanilla equivalents. A visual gold marker alone does not prove isolation: the game must identify the selected weapon and route only that identifier to new behavior.

## Candidate route

FusionFix documents file overriding in its [README](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix/blob/v5.0.1/readme.md). Its [v5.0.1 `ExtendedLimits` source](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix/blob/v5.0.1/source/limits.ixx) registers new weapon-name hashes at numeric ID 58 and above. The installed configuration has `ExtendedLimits=1`. In the base IV story, `common/data/default.dat` loads `common:/data/weaponinfo.xml`; a mirrored `update/common/data/WeaponInfo.xml` is the candidate override path. The first staged override clones the pistol entry under `LF_GOLD_PISTOL`, retaining `w_glock` model and vanilla stats. This is only an identity probe, with no gold art or custom handling.

The installed IV, TLAD, and TBoGT `WeaponInfo.xml` files define episodic entries only through `EPISODIC_21`. The installed FusionFix config has `EpisodicWeapons=0`. The previous `EPISODIC_22`–`EPISODIC_24` idea is unproven and is not in the test package. **[HYPOTHESIS]** The custom name is assigned ID 58 and can be selected by ScriptHookDotNet; in-game testing must establish that, inventory behavior, save persistence, and gameplay compatibility.

## T-007 questions

1. Does FusionFix assign `LF_GOLD_PISTOL` ID 58 on this exact installation, and can ScriptHookDotNet read and select it?
2. Can each be given/spawned and distinguished through ScriptHookDotNet?
3. Can an overload provide a separate model/texture without overwriting a vanilla asset?
4. What happens after save/load, death, cutscene, vehicle entry, cover, and mission weapon changes?

Do not ship ripped assets or assert a spare slot is safe before the test. A lawful temporary visual marker can be used in a private spike; a reusable finish path is a later design task.
