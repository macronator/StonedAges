# Contributing to Stoned Ages

Thanks for taking an interest. This is a **community source reconstruction** of a 2-D Dark Ages–engine
game client: the original source was lost, and this code was recovered by decompiling the surviving
binaries (PDB-assisted, so type/method/local names and structure are faithful). It **builds and runs**.

There are two kinds of contribution, and both are welcome:

1. **Use it as a starting point** — fork it and build your own client/server, salvage the engine, study
   the protocol, port it somewhere new.
2. **Help finish the cleanup** — a lot of the code still reads like a decompiler dump; making it read like
   hand-written source (without changing behavior) is the main ongoing effort.

---

## TL;DR

- Target framework is **.NET Framework 4.0**. Build: `dotnet build "Stoned Ages.slnx" -c Release`.
- **Keep the build at 0 errors / 0 warnings.** The build is the safety net — verify after every change.
- **Preserve behavior.** This is a faithful reconstruction; behavior-changing PRs should be called out and
  justified, not slipped in during a cleanup.
- **Don't commit game data, saves, PDBs, or `irrKlang`** — the `.gitignore` handles this; don't override it.

---

## Building & running

See [README](README.md) for the full steps. In short:

```
dotnet build "Stoned Ages.slnx" -c Release
```

Dependencies are vendored in **`lib/`** except **irrKlang** (proprietary) — download *irrKlang .NET 4*
from <https://www.ambiera.com/irrklang/> and drop `irrKlang.NET4.dll` into `lib/` before building.

To **run**, the executable needs the game data folders (`dats/ maps/ music/ jsons/ Images/ players/`) and
native runtime DLLs next to it — **you supply your own data** (it is not redistributed here). A successful
launch renders the dragon start menu (~380–410 MB resident).

---

## Ground rules

- **net40 only.** No language/runtime features newer than the framework: **no tuples** (`System.ValueTuple`),
  no `Span`/`Index`/`Range`, no `record`, no nullable-reference annotations. The compiler usually catches these.
- **Don't rename public / internal / protected members or types.** They're cross-file contracts (and several
  mirror the wire protocol). Rename **locals and private members only**.
- **No blind find-replace across a file.** A local named `num` in method A is unrelated to `num` in method B,
  and short names collide with substrings (`_text`, `.Text`, `ChangeText`) and JSON keys (`"text"`). Scope every
  rename to a single method.

---

## The "de-decompile" cleanup (the main ongoing effort)

The decompiler returned real names but decompiler-shaped bodies: mechanical locals (`num`, `text`, `flag`,
`item`, `jToken`, `array`, `value`, `result`), magic numbers, and copy-paste blocks. The goal is to make each
method read like a person wrote it — **identical behavior, clearer code**.

**`StonedAges/StonedAges/GameWindow.cs` is the exemplar — read it first and match its style.**

### Per-method recipe

1. Add an XML `<summary>` on non-obvious methods.
2. Rename mechanical locals to meaningful names — **per method/scope**.
3. Name magic numbers. OpenGL constants live in `Tao.OpenGl.Gl` (e.g. `GL_TEXTURE_2D` = 3553,
   `GL_PROJECTION` = 5889, `GL_MODELVIEW` = 5888, `GL_LINE_LOOP` = 2). `TextureManager` / `Renderer` /
   `DAGraphics` are full of these.
4. Collapse copy-paste blocks into loops or small lookup tables.
5. Add `#region`s where they aid navigation (every `#region` needs a matching `#endregion`, or it won't compile).
6. **Rebuild → must stay 0/0. Then launch-test.**

### Gotchas (learned the hard way)

- **Quote-guard / dot-guard regex renames.** Renaming `text` → `itemName` must not touch the JSON key `"text"`
  (guard `(?<!")\btext\b(?!")`) or a property access `entity.text` (guard `(?<!\.)\btext\b`). After a batched
  rename, **assert** the JSON keys and property accesses survived before trusting it.
- **String literals bite.** A `"item"` substring inside a built texture name will be caught by a naïve
  `\bitem\b` rename — quote-guard it.
- **Preserve line endings.** This tree is CRLF; tools that rewrite files should not flip them.

---

## Where to start (by difficulty)

- **Easy** — add `<summary>` docs and rename locals in any not-yet-cleaned method. Build stays 0/0, low risk.
- **Medium** — tidy and document the **Engine** archive/format readers (DAT / EPF / HPF / MPF / SPF / palette).
  These decode the proprietary data formats and are under-documented.
- **Hard — the "giants" in `GameState.cs`.** `Update`, `UpdateInput`, `InitializeMenu`, `Render`,
  `DialogPopup`, the `ChatMsg` slash-command dispatcher, and the combat methods (`CastLoadedSpellOnTarget`,
  `UseScript`). They're multi-scope and behavior-sensitive — clean **sub-block by sub-block**, rebuilding after
  each. Do not method-wide-regex them.

See [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) for a map of the codebase before you dive in.

---

## Bigger ways to help (recommendations)

- **Document the wire protocol.** The incoming `…P` handlers and outgoing `Send…`/`Request…` methods describe
  the packets in code; a written opcode reference would let people build a compatible server.
- **Externalize the server address.** `ServerConnect()` hardcodes the (offline) original server. Moving it to a
  config file makes the client immediately usable against any compatible server.
- **A compatible server.** The protocol is Dark Ages–style; pairing this client with an open server is the
  single biggest unlock for actually playing.
- **Asset tooling.** Open tools to build the `dats/` archives from assets a user legitimately owns.
- **Modernize.** Once the net40 cleanup is far enough along, a parallel port to a current .NET (and away from
  the Tao bindings) is a natural next step.

---

## Verifying a change

```
dotnet build "Stoned Ages.slnx" -c Release    # must be 0 errors / 0 warnings
```

Then run it (see README → **Running**) and confirm it still renders the start menu. A green build plus a clean
launch is the bar for a cleanup PR.

---

## A note on scope & licensing

This is a fan / preservation project, **not** affiliated with KRU Interactive / Nexon or any rights holder.
Don't add game assets or proprietary binaries to the repo. Code is MIT (see [`LICENSE`](LICENSE)); the original
game and engine are credited to **Sostratos**.
