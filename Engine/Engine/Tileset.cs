using System.Collections.Generic;
using System.IO;

namespace Engine;

public class Tileset
{
    public const int TileWidth = 56;

    public const int TileHeight = 27;

    public const int TileSize = 1512;

    private List<byte[]> tiles = new List<byte[]>();

    private string name;

    private string filename;

    private int tileCount;

    public byte[] this[int index]
    {
        get
        {
            return tiles[index];
        }
        set
        {
            tiles[index] = value;
        }
    }

    public string FileName => filename;

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public byte[][] Tiles => tiles.ToArray();

    public int TileCount => tileCount;

    public static Tileset FromFile(string file)
    {
        Tileset tileset = LoadTiles(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
        tileset.name = Path.GetFileNameWithoutExtension(file).ToUpper();
        tileset.filename = file;
        return tileset;
    }

    public static Tileset FromRawData(byte[] data)
    {
        Tileset tileset = LoadTiles(new MemoryStream(data));
        tileset.name = "Unknown Tileset";
        return tileset;
    }

    public static Tileset FromArchive(string file, DATArchive archive)
    {
        if (!archive.Contains(file))
        {
            return null;
        }
        Tileset tileset = LoadTiles(new MemoryStream(archive.ExtractFile(file)));
        tileset.name = Path.GetFileNameWithoutExtension(file).ToUpper();
        tileset.filename = file;
        return tileset;
    }

    public static Tileset FromArchive(string file, bool ignoreCase, DATArchive archive)
    {
        if (!archive.Contains(file, ignoreCase))
        {
            return null;
        }
        Tileset tileset = LoadTiles(new MemoryStream(archive.ExtractFile(file, ignoreCase)));
        tileset.name = Path.GetFileNameWithoutExtension(file).ToUpper();
        tileset.filename = file;
        return tileset;
    }

    private static Tileset LoadTiles(Stream stream)
    {
        stream.Seek(0L, SeekOrigin.Begin);
        BinaryReader binaryReader = new BinaryReader(stream);
        Tileset tileset = new Tileset();
        tileset.tileCount = (int)(binaryReader.BaseStream.Length / 1512);
        for (int i = 0; i < tileset.tileCount; i++)
        {
            byte[] item = binaryReader.ReadBytes(1512);
            tileset.tiles.Add(item);
        }
        binaryReader.Close();
        return tileset;
    }

    public override string ToString()
    {
        return "{Name = " + name + ", Tiles = " + tiles.Count + "}";
    }
}
