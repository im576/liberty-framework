# T-032 serialized class words versus current CE executable

Read-only static check of the owner's GTAIV.exe, version 1.2.0.59, SHA-256 `08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D`. PE image base is `0x00400000`; its `.text` section spans RVA `0x00001000` to `0x00A72EFA` by virtual size. `.rdata` starts at RVA `0x00A73000`.

The six raw first words observed at CE WBD dictionary targets (`0x0069C19C`, `0x0069D56C`, `0x0069BBEC`, `0x0069AAF4`, `0x0069D9E4`, `0x0069D7F4`) all numerically fall inside this executable's `.text` section, not its `.rdata` section. Interpreting one as a PE virtual address also lands at arbitrary instruction positions; no constructor/function entry is established. A raw 32-bit-address scan found no matching literal in `.text`, `.rdata` or `.data`, which does not exclude indirect or transformed dispatch.

The safe name for these values is **serialized class words**. Their grouping across 3,239 indexed targets is reproducible, but equality to a current CE runtime vtable or constructor address is unproven. The executable's resource fixup/type dispatch and the meaning of target fields `+0x8C`, `+0xB0`, `+0xD0`, `+0xE0` remain open. This check supplies no array count, stride, shape semantics or authored collision permission.
