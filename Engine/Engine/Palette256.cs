using System.Drawing;
using System.IO;

namespace Engine;

public class Palette256
{
    private System.Drawing.Color[] colors = new System.Drawing.Color[256];

    public System.Drawing.Color this[int index]
    {
        get
        {
            return colors[index];
        }
        set
        {
            colors[index] = value;
        }
    }

    public System.Drawing.Color[] Colors => colors;

    public static Palette256 FromFile(string file)
    {
        return LoadPalette(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public static Palette256 FromRawData(byte[] data)
    {
        return LoadPalette(new MemoryStream(data));
    }

    public static Palette256 FromArchive(string file, DATArchive archive)
    {
        if (!archive.Contains(file, ignoreCase: true))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file));
    }

    private static Palette256 LoadPalette(Stream stream)
    {
        stream.Seek(0L, SeekOrigin.Begin);
        BinaryReader binaryReader = new BinaryReader(stream);
        Palette256 palette = new Palette256();
        for (int i = 0; i < 256; i++)
        {
            ref System.Drawing.Color reference = ref palette.colors[i];
            reference = System.Drawing.Color.FromArgb(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
        }
        return palette;
    }
}
