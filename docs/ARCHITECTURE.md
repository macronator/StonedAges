# Architecture

A high-level map of the codebase so you can find your way around before contributing. For the build/run
steps see the [README](../README.md); for the cleanup conventions see [CONTRIBUTING](../CONTRIBUTING.md).

---

## The big picture

Stoned Ages is a 2-D, sprite-based game **client** for a Dark Ages–style server. It opens an SDL window with
an OpenGL context, loads sprites/tiles/sounds from packed data archives, connects to a game server over TCP,
and renders the world + UI in an immediate-mode loop.

The solution (`Stoned Ages.slnx`) has **two projects**:

| Project | Output | Role |
|---|---|---|
| **`Engine/`** | `Engine.dll` | Reusable graphics / data / IO library (no game rules). |
| **`StonedAges/`** | `Stoned Ages.exe` | The game client: window, game loop, screens, networking, gameplay. |

`StonedAges` references `Engine`. Both target **.NET Framework 4.0** (x86 `WinExe` for the client).

---

## Engine (`Engine/`)

The lower layer — everything that isn't game-specific rules.

- **Archive / format readers** — decode the proprietary game-data formats into usable images:
  - **DAT** — the archive container (a `.dat` packs many named entries).
  - **EPF** — sprite frames (palette-indexed; combined with a palette to produce RGBA).
  - **HPF / MPF / SPF** — additional sprite/graphic formats used by tiles, maps, and effects.
  - **Palette** — the colour tables EPF/HPF indices map through.
- **`TextureManager`** — loads and caches GL textures by name (decoding EPF/HPF + palette on demand). Most of
  the client asks for textures here with a string key like `"item001_F0_C0"`.
- **`SoundManager`** — audio via irrKlang.
- **`Input`** — keyboard/mouse state.
- **`Font` / `TextField`** — text rendering and editable text fields.
- **`Tileset` / `DAGraphics` / `Renderer`** — tile/sprite composition and the GL draw helpers.

See **[`FORMATS.md`](FORMATS.md)** for the binary layouts (DAT archive, EPF/HPF/MPF sprites, palettes, maps),
documented from these readers.

---

## StonedAges (`StonedAges/`)

The game client itself.

- **`GameWindow`** — creates the SDL window + GL context, owns the main loop and the `ClientSocket`, and holds
  global flags like `ConnectedToServer`. **This file has been fully cleaned and is the style exemplar — read it
  first.**
- **`Program`** — process entry point.
- **Screen states** — `StartMenuState`, `StoryState`, `CreditsState`, `CreateMenuState`, … — the front-end
  flow (title screen, character create, story/credits). Each is a small state with its own update/render.
- **`GameState.cs` (~29k lines)** — the **in-game state**: the heart of the client. A single `partial class`
  holding the per-frame update, input handling, rendering, **every network packet handler and sender**, and all
  the gameplay/UI logic (entities, map, inventory, profiles, boards, dialog, combat).

---

## The frame loop

`GameWindow` drives the loop: each frame it calls **`Update(elapsedSeconds)`** then **`Render()`** on the
active state. In menus that's the relevant `*State`; in-game it's `GameState`:

- **`GameState.Update`** — reads input, advances timers/animations, processes queued work, and reacts to game
  state. (One of the "giants" still being cleaned.)
- **`GameState.Render`** — draws the map, entities, and the UI overlay (orbs, bars, menus, tooltips). (Also a
  "giant".)

---

## Networking / wire protocol

The client speaks a **Dark Ages–style binary protocol** over a TCP socket (`GameWindow.ClientSocket`).
You won't find a written opcode table yet — but the code follows two clear conventions:

- **Incoming packets → `…P` handler methods** (the trailing `P` = "packet"). Each takes a decoded packet
  struct and applies it to game state. Examples: `SpellAnimationP`, `EmoteP`, `BodyMoveP`, `RemoveEntityP`,
  `SystemMessageP`.
- **Outgoing packets → `Send…` / `Request…` methods.** The common shape is:
  1. fill a `XxxS` struct with the fields,
  2. accumulate the byte length as you go (`packetLen`),
  3. wrap it in a `XxxPacket(struct, packetLen)`,
  4. `GameWindow.ClientSocket.Send(packet.Data)`.

  Examples: `SendProfileData`, `SendDisplayPlayer`, `RequestBoard`, `SendNewPost`.

The packet **structs** (`XxxS`) and **packet classes** (`XxxPacket`) live in the client. The **opcode↔handler
dispatch** (which byte selects which `…P` handler) lives in the socket read loop — start there to produce a
real opcode reference. `ServerConnect()` holds the server address (the original server is offline; point it at
your own).

> See **[`PROTOCOL.md`](PROTOCOL.md)** for the framing, the incoming opcode table, the handshake, the string
> encoding, and how to find each outgoing opcode — the basis for building a compatible server.

---

## Game data (loaded at runtime — not in this repo)

The client loads everything by **relative path** from its working directory. None of it ships here; you
[supply your own](../README.md#-you-must-supply-your-own-game-data).

| Folder | Contents |
|---|---|
| `dats/` | Packed archives (sprites, tiles, sounds) in the DAT format; decoded by the Engine readers. |
| `maps/` | Map definitions. |
| `music/` | `.ogg` tracks. |
| `jsons/` | Data tables (items, npcs, spells, skills, actions, dialog, maps, guilds, monsters). |
| `Images/` | UI / loose graphics. |
| `players/` | Local character saves — `players/<name>/<name>.txt` (JSON), loaded by `InitializePlayer`. |

### Data tables (the `*DB` dictionaries)

`GameState` keeps the `jsons/` tables in memory as `_itemsDB`, `_npcsDB`, `_spellsDB`, `_skillsDB`,
`_actionsDB`, `_dialogDB`, `_mapsDB`, `_guildsDB`, `_monstersDB` and looks rows up by key, e.g.
`_itemsDB["items"]` then `item.Value<string>("name")`. These JSON shapes are effectively the content schema —
preserve the keys when cleaning code that reads them.

---

## Navigating `GameState.cs`

It's one big `partial class`, but it's organized by responsibility. Useful entry points:

- **`InitializePlayer`** — loads a save into the live `Player` and builds the in-memory item bank from
  `_itemsDB`. A good, self-contained read.
- **The `…P` handlers** — small, one-per-packet; each shows how one server message mutates state.
- **The `Send…` / `Request…` senders** — small, one-per-packet; each shows how the client builds a request.
- **`Update` / `Render` / `UpdateInput`** — the per-frame giants (large; being cleaned sub-block by sub-block).
- **`ChatMsg`** — the slash-command dispatcher (the in-game command surface).

When in doubt, search for the texture key or JSON key you care about — most features are wired by string name.
