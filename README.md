# Stoned Ages

A 2-D, Dark Ages–engine fan game **client** (C# / .NET Framework 4.0; OpenGL via Tao, SDL window,
irrKlang audio).

> **Status — community source reconstruction.** The original source was lost; this was recovered by
> decompiling the original binaries (PDB-assisted, so type/method/local names and structure are
> faithful) and is being tidied to read like ordinary hand-written code. **It builds and runs.**

## ⚠️ You must supply your own game data

This repository is **code only**. It does **not** include the game data (`dats/`, `maps/`, `music/`,
`jsons/`, `Images/`) — those are third-party assets and are not redistributed here. To run the client
you must provide your own copy of that data next to the built executable (see **Running**).

## Building

Requires the **.NET SDK** (or Visual Studio 2022):

```
dotnet build "Stoned Ages.slnx" -c Release
```

Output: `StonedAges/bin/Release/net40/Stoned Ages.exe` (+ `Engine.dll`).

Dependencies are vendored in **`lib/`** (Tao OpenGl / Platform.Windows — MIT; SDL — zlib;
Newtonsoft.Json — MIT) **except irrKlang**, which is proprietary and is **not** included. Download
*irrKlang .NET 4* from <https://www.ambiera.com/irrklang/> and drop `irrKlang.NET4.dll` into `lib/`
before building.

## Running

Place the following next to the built `Stoned Ages.exe`:

- Data folders: `dats/`, `maps/`, `music/` (`.ogg`, including one named `68.ogg`), `jsons/`,
  `Images/`, `players/`
- Native runtime libraries: `SDL.dll`, `OpenAL32.dll`, `alut.dll`, and irrKlang's native DLLs
- `font.fnt`, `font_0.png`, `vers.xml`

> The client connects to a game server in `ServerConnect()`; the original server is offline. Point it
> at your own server to play online, or run it locally as far as your data allows.

## Project layout

- **`Engine/`** — the graphics/data/IO library: DAT/EPF/HPF/MPF/SPF/palette readers, `TextureManager`,
  `SoundManager`, input, fonts, tileset, rendering helpers.
- **`StonedAges/`** — the game client: `GameWindow` + game loop, the menu/game states, networking
  (`ClientSocket`), entities, maps, inventory/UI.

## Architecture

A 2-D client over two projects: **`Engine/`** (graphics/data/IO library) and **`StonedAges/`** (the game
client + the ~29k-line `GameState.cs`). The client runs an `Update`/`Render` frame loop, loads sprites from the
`dats/` archives through the Engine readers, and speaks a Dark Ages–style binary protocol to a server.

See **[`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)** for the full map — including how to navigate
`GameState.cs` and how the packet handlers (`…P`) and senders (`Send…`) work.

## Contributing

See **[`CONTRIBUTING.md`](CONTRIBUTING.md)**. Because this is a reconstruction recovered from decompiled
binaries, much of the work is making the code read like hand-written source **without changing behavior**: keep
the build at **0 errors / 0 warnings**, rename locals per-method (never blind find-replace across a file), and
don't commit game data or the proprietary `irrKlang` binary. `GameWindow.cs` is the cleaned-up style exemplar.

## Roadmap / where to help

Good ways to take this further:

- **Document the wire protocol** — turn the `…P` handlers and `Send…`/`Request…` senders into a written opcode
  table, so a compatible server can be built.
- **A compatible server** — the protocol is Dark Ages–style; pairing the client with an open server is what
  makes it playable again.
- **Externalize the server address** — `ServerConnect()` hardcodes the (offline) original; moving it to a config
  file lets the client work against any server out of the box.
- **Finish the de-decompile** — the remaining "giants" in `GameState.cs` (`Update`, `UpdateInput`,
  `InitializeMenu`, `Render`, `DialogPopup`, `ChatMsg`, combat). See CONTRIBUTING for the recipe.
- **Document the Engine data formats** — the DAT/EPF/HPF/MPF/SPF/palette readers are the spec; a written format
  reference and asset tooling would help everyone.
- **Modernize** — once the net40 cleanup is far enough along, a parallel port to a current .NET.

## Credits

- **Original game & engine: Sostratos.**
- Source reconstruction & cleanup: this repository.

## Disclaimer

Fan / preservation project. **Not** affiliated with, endorsed by, or connected to KRU Interactive /
Nexon or any rights holder. "Dark Ages" and any game assets are the property of their respective
owners; you must own a legitimate copy of any data you use with this client. Provided for educational
and preservation purposes only.

## License

See [`LICENSE`](LICENSE) — MIT, crediting the original author. This is a community reconstruction; if
the original author requests removal, that will be honored.
