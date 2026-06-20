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
