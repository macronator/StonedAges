using System;
using System.IO;

namespace Engine;

public class MAPFile
{
    private int width;

    private int height;

    private MapTile[] tiles;

    private int id;

    private string name;

    public MapTile this[int x, int y]
    {
        get
        {
            return tiles[y * width + x];
        }
        set
        {
            tiles[y * width + x] = value;
        }
    }

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

    public int ID
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    public MapTile[] Tiles => tiles;

    public int Height
    {
        get
        {
            return height;
        }
        set
        {
            height = value;
        }
    }

    public int Width
    {
        get
        {
            return width;
        }
        set
        {
            width = value;
        }
    }

    public static MAPFile FromFile(string file)
    {
        MAPFile mAPFile = LoadMap(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
        mAPFile.id = Convert.ToInt32(Path.GetFileNameWithoutExtension(file).Remove(0, 3));
        return mAPFile;
    }

    public static MAPFile FromFile(string file, int width, int height)
    {
        MAPFile mAPFile = LoadMap(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
        mAPFile.width = width;
        mAPFile.height = height;
        mAPFile.id = Convert.ToInt32(Path.GetFileNameWithoutExtension(file).Remove(0, 3));
        return mAPFile;
    }

    public static MAPFile FromRawData(byte[] data)
    {
        return LoadMap(new MemoryStream(data));
    }

    public static MAPFile FromRawData(byte[] data, int width, int height)
    {
        MAPFile mAPFile = LoadMap(new MemoryStream(data));
        mAPFile.width = width;
        mAPFile.height = height;
        return mAPFile;
    }

    private static MAPFile LoadMap(Stream stream)
    {
        stream.Seek(0L, SeekOrigin.Begin);
        BinaryReader binaryReader = new BinaryReader(stream);
        int num = (int)(binaryReader.BaseStream.Length / 6);
        MAPFile mAPFile = new MAPFile();
        mAPFile.tiles = new MapTile[num];
        for (int i = 0; i < num; i++)
        {
            ushort floor = binaryReader.ReadUInt16();
            ushort leftWall = binaryReader.ReadUInt16();
            ushort rightWall = binaryReader.ReadUInt16();
            mAPFile.tiles[i] = new MapTile(floor, leftWall, rightWall);
        }
        binaryReader.Close();
        return mAPFile;
    }

    public override string ToString()
    {
        return "{Name = " + name + ", ID = " + id + ", Width = " + width + ", Height = " + height + "}";
    }
}
