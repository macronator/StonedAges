# Game data formats

The `Engine/` project decodes the proprietary Dark Ages data formats. This documents the layouts **as the
readers actually parse them**, so you can decode or repack assets you own. Source of truth: the
`Engine/Engine/*.cs` reader classes named per section. **All integers are little-endian.**

> None of this data ships with the repo — you supply your own (see the [README](../README.md)).

---

## DAT — archive container
*`DATArchive.cs`, `DATFileEntry.cs`*

A `.dat` packs many named files behind a flat table of contents.

| Offset | Size | Field |
|---|---|---|
| 0 | 4 | `int32` fileCount (real entries = `fileCount - 1`) |
| 4 | … | TOC entries |
| … | … | file-data blobs |

Each **TOC entry**: `uint32` dataOffset, then a **13-byte ASCII name** (NUL-padded; truncated at the first NUL).
A file's bytes run from its `dataOffset` to the **next** entry's `dataOffset` (the final entry's offset doubles as
the end marker). Most callers match names case-insensitively. Entries are `*.epf`, `*.hpf`, `*.mpf`, `*.pal`, etc.

---

## Palette (`.pal`) — 256-colour table
*`Palette256.cs`*

**768 bytes: 256 entries × 3 bytes (R, G, B).** Index `i` → `Color.FromArgb(R, G, B)`. EPF/HPF/MPF pixels are
indices into one of these palettes.

---

## EPF — sprite sheet (UI, items, tiles, bodies)
*`EPFImage.cs`, `EPFFrame.cs`*

**Header (12 bytes):** `uint16` frameCount, `uint16` width, `uint16` height, `uint16` (unknown), `uint32`
tocOffset *(relative — add 12 for the absolute position)*. Palette-indexed pixel data follows the header; the TOC
sits at the end of the file.

**TOC — `frameCount × 16 bytes`:**

| Size | Field |
|---|---|
| 2 | left |
| 2 | top |
| 2 | right |
| 2 | bottom |
| 4 | pixelOffset *(relative; +12)* |
| 4 | nextOffset *(relative; +12)* |

Each frame is `(right − left) × (bottom − top)` bytes of palette indices at `pixelOffset`, and carries its
bounding box (top/left + width/height) for placement.

---

## HPF — compressed 28-px tile graphic
*`HPFImage.cs`, `HPFCompression.cs`*

Begins with the 4-byte signature `0xFF02AA55` (bytes `55 AA 02 FF`). The **entire file** is run through
`HPFCompression.Decompress` (a custom RLE-style codec — see that class for the algorithm). The decompressed
stream is **8 bytes of header + palette-indexed pixels**. Width is the standard **28**; `height = pixels.Length /
28`. Used for map tiles.

---

## MPF — animated sprite (monsters / NPCs)
*`MPFImage.cs`, `MPFFrame.cs`*

Optional prefix: if the first `int32` is `-1` (`0xFFFFFFFF`) it's the "FF" variant and a `uint32` follows;
otherwise rewind 4. Then:

| Size | Field |
|---|---|
| 1 | frameCount |
| 2 | width |
| 2 | height |
| 4 | dataSize (size of the pixel block at the end of the file) |
| 1 | walkStart |
| 1 | walkLength |
| 2 | newFormatFlag (`0xFFFF` ⇒ "new" layout) |

**Animation ranges** — *new* layout: idleStart, idleLength, idle2Length, unknown, attack1Start, attack1Length,
attack2Start, attack2Length, attack3Start, attack3Length (each 1 byte). *Old* layout (rewind 2): attack1Start,
attack1Length, idleStart, idleLength, idle2Length, unknown.

**Per-frame TOC (`frameCount` entries):** `uint16` left, top, right, bottom, xOffset, yOffset; `uint32`
dataOffset. Frame pixels are `(right − left) × (bottom − top)` bytes at `(fileLength − dataSize) + dataOffset`.
A TOC entry whose left **and** top are both `0xFFFF` is a **palette marker** — it selects `mns{dataOffset:000}.pal`;
otherwise the default `mns000.pal` is used. The walk/idle/attack ranges index into the frame list.

---

## MAP (`.map`) — map tile grid
*`MAPFile.cs`, `MapTile.cs`*

A flat array of tiles, **6 bytes each** (`tileCount = fileLength / 6`): `uint16` floor, `uint16` leftWall,
`uint16` rightWall (tile/sprite ids). Width and height are **not stored in the file** — the caller supplies the
dimensions. Tile `(x, y)` = index `y * width + x`.

---

## EFA — effect animation
*`EFA_File.cs`, `EFA_Frame.cs`, `EFA_Frame_Header.cs`*

Spell / visual-effect animations. Layout lives in those three classes — documenting it fully (header + per-frame
header + pixels) is a good next contribution.

---

## A note on "SPF"

Older notes mention an `SPF` format, but **this Engine has no SPF reader** — sprites are **EPF** (sheets),
**MPF** (animated monsters), and **HPF** (compressed tiles). SPF assets would need a new reader.

---

## The decode pipeline

```
DATArchive.FromFile(".dat")          open an archive
   -> ExtractFile(name)              pull one raw entry
   -> EPFImage / HPFImage / MPFImage parse into frames of PALETTE INDICES
   -> Palette256                     map indices -> RGB
   -> DAGraphics / TextureManager    compose -> a GL texture (cached by name)
```

Start at `TextureManager.cs` to see it wired together end to end.
