# Wire protocol

The Stoned Ages client talks to its server over a single TCP socket using a Dark Ages–style binary framing.
This documents what the **client** implements — enough to start building a compatible server.

**Source of truth:** [`ClientSocket.cs`](../StonedAges/StonedAges/ClientSocket.cs) (socket + framing),
[`PacketHandler.cs`](../StonedAges/StonedAges/PacketHandler.cs) (incoming dispatch), `PacketStructure.cs` (the
read/write helpers + header), and the per-packet `*Packet.cs` files / `*S` structs.

---

## Framing

All integers are **little-endian**. Every packet:

| Offset | Size | Field |
|---:|---:|---|
| 0 | 2 | **Length** (uint16) — body length; the total buffer is `4 + body` |
| 2 | 2 | **Opcode** (uint16) — selects the handler |
| 4 | … | **Body** — opcode-specific |

The body always starts at offset **4**. `PacketHandler.Handle` switches on the opcode at offset 2 (the length at
offset 0 is written by senders, but the client's dispatch keys only on the opcode). A packet's **outgoing opcode
is the second argument to its `base(totalLength, opcode)` constructor** — e.g. `RequestBoardPacket` is opcode
**38**.

### Strings

Strings are **length-prefixed**, then raw bytes:

- short fields — a **1-byte** length, then that many bytes (`WriteByte((byte)s.Length); WriteString(s)`);
- long fields (e.g. a board post body) — a **2-byte (uint16)** length, then the bytes.

---

## Connection handshake

Immediately after the socket connects, the client sends a 4-byte packet — length `4`, **opcode 1**, no body
(`ClientSocket.ConnectCallback`). Treat opcode **1** (client→server) as "hello / I'm here". If the primary
connect fails, the client retries once against a fallback LAN address before giving up.

---

## Incoming opcodes (server → client)

From `PacketHandler.Handle`. Most packets are **queued** onto a `gs.*Packets` list and applied on the main
thread during `GameState.Update` (so the receive thread never mutates game state directly); a few are applied
immediately.

| Opcode | Packet type | Handling |
|---:|---|---|
| 4 | `LocationPacket` | queue → `locationPackets` (entity position) |
| 5 | `PlayerIDPacket` | **now** — set `ClientID` |
| 7 | `EntityMapInfoPacket` | **now** — `PortToPlayer(number, x, y)` |
| 8 | `GroupRequestPacket` | **now** — `GroupRequest(name, type)` |
| 12 | `BodyMovementPacket` | queue → `bodyMovementPackets` |
| 14 | `RemoveEntityPacket` | queue → `removeEntityPackets` |
| 20 | `MessagePacket` | queue → `messagePackets` (chat) |
| 21 | `SysMessagePacket` | **now** — `SystemMessageP(msgType, message)` |
| 28 | `DisplayProfilePacket` | queue → `displayProfilePackets` |
| 30 | `UserListPacket` | **now** — `UpdateUserList(list)` |
| 33 | `DisplayPlayerPacket` | queue → `displayPlayerPackets` |
| 34 | `DisplayEntitiesPacket` | queue → `displayEntitiesPackets` |
| 35 | `SpellAnimationPacket` | queue → `spellAnimationPackets` |
| 36 | `ProjectilePacket` | queue → `projectilePackets` |
| 39 | `DisplayBoardPacket` | queue → `displayBoardPackets` |
| 100 | `JsonVersPacket` | **now** — JSON data-version check; exits to the patcher on mismatch |
| 102 | `ReceivedClientVersionPacket` | **now** — client-build check; warns + exits if stale |

Unknown opcodes are ignored (the switch has no `default`).

---

## Outgoing packets (client → server)

Built in `GameState` by `Send…` / `Request…` methods: fill a `…S` struct, accumulate the body length, wrap it
in a `…Packet`, and call `ClientSocket.Send(packet.Data)`. To find any sender's **opcode**, read the
`base(…, opcode)` call in its `*Packet.cs`.

| Sender | Packet | Opcode |
|---|---|---:|
| (connect handshake) | — | 1 |
| `RequestBoard`, `SendNewPost` | `RequestBoardPacket` | 38 |
| `SendProfileData` | `DisplayProfilePacket` | *(its `base(…)` arg)* |
| `SendDisplayPlayer` | `DisplayPlayerPacket` | *(its `base(…)` arg)* |

…plus movement, chat, spell/skill use, item, group, etc. Each `*Packet`'s `get`/`set Board`-style property shows
the exact body layout (offsets start at 4); `RequestBoardPacket` is a good worked example (board types 2/3/5/6).

---

## Gatekeeper opcodes (a minimal server must satisfy these)

- **100 `JsonVers`** — the server advertises the version/timestamp of each JSON data table; if the client's
  copies don't match, it launches the patcher and exits.
- **102 `ReceivedClientVersion`** — the server sends the expected client build (`MM.dd.yy-HH:mm:ss`); a mismatch
  shows a warning and exits.

Either satisfy these from your server, or patch the checks out of `PacketHandler.cs`.

---

## Extending this reference

What's left to fully nail down: enumerate every **outgoing** opcode (the `base(…, opcode)` arg in each
`*Packet.cs`) and the **per-field byte layout** of each `…S` struct. The primitives all live in
`PacketStructure.cs` — `ReadByte` / `ReadUShort` / `ReadString` and their `Write*` mirrors — so each packet's
property `get` (decode) and `set` (encode) is a complete field-by-field spec for that opcode.
