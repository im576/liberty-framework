# T-011 — Gold carbine and gold shotgun

Status: **READY** (Codex Agent B). Add two variants to `assets/finishes/finishes.json` using the existing finish pipeline; no code change expected.

- `lf_gold_carbine` from `w_m4`: textures `bm_m4a1` diffuse, `bm_m4a1_s` specular, `icon` icon (keep `bm_m4a1_n`); IDE `gun@ak47`, draw distance 50, `CM_WEAPONS_M4`; `weaponInfoType` `LF_GOLD_CARBINE`.
- `lf_gold_shotgun` from `w_shotgun`: `cj_shotgun_comp` diffuse, `cj_shotgun_comp_s` specular, `icon` icon (keep `_n`); `gun@shotgun`, 50, `CM_WEAPONS_SHOTGUN`; `LF_GOLD_SHOTGUN`.
(Texture names were listed from the installed `weapons.img`; IDE values from `common/data/default.ide`.)

Acceptance: `tools/build-finishes.ps1` succeeds (read-only on the game), previews for both exist under `staging/phase1/previews`, a verify check asserts both variants, config docs updated. Human test: both guns look gold in hand and on the HUD icon.