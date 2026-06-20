using System.IO;

namespace Engine;

public class EPFImage
{
    private int expectedFrames;

    private int width;

    private int height;

    private int unknown;

    private long tocAddress;

    private EPFFrame[] frames;

    public EPFFrame this[int index]
    {
        get
        {
            return frames[index];
        }
        set
        {
            frames[index] = value;
        }
    }

    public EPFFrame[] Frames => frames;

    public long TOCAddress => tocAddress;

    public int Unknown => unknown;

    public int Height => height;

    public int Width => width;

    public int ExpectedFrames => expectedFrames;

    public static EPFImage FromFile(string file)
    {
        return LoadEPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public static EPFImage FromRawData(byte[] data)
    {
        return LoadEPF(new MemoryStream(data));
    }

    public static EPFImage FromArchive(string file, DATArchive archive)
    {
        if (!archive.Contains(file))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file));
    }

    public static EPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
    {
        if (!archive.Contains(file, ignoreCase))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file, ignoreCase));
    }

    private static EPFImage LoadEPF(Stream stream)
    {
        stream.Seek(0L, SeekOrigin.Begin);
        BinaryReader binaryReader = new BinaryReader(stream);
        EPFImage ePFImage = new EPFImage();
        ePFImage.expectedFrames = binaryReader.ReadUInt16();
        ePFImage.width = binaryReader.ReadUInt16();
        ePFImage.height = binaryReader.ReadUInt16();
        ePFImage.unknown = binaryReader.ReadUInt16();
        ePFImage.tocAddress = binaryReader.ReadUInt32() + 12;
        if (ePFImage.expectedFrames <= 0)
        {
            return ePFImage;
        }
        ePFImage.frames = new EPFFrame[ePFImage.expectedFrames];
        for (int i = 0; i < ePFImage.expectedFrames; i++)
        {
            binaryReader.BaseStream.Seek(ePFImage.tocAddress + i * 16, SeekOrigin.Begin);
            int num = binaryReader.ReadUInt16();
            int num2 = binaryReader.ReadUInt16();
            int num3 = binaryReader.ReadUInt16();
            int num4 = binaryReader.ReadUInt16();
            int num5 = num4 - num2;
            int num6 = num3 - num;
            uint num7 = binaryReader.ReadUInt32() + 12;
            uint num8 = binaryReader.ReadUInt32() + 12;
            binaryReader.BaseStream.Seek(num7, SeekOrigin.Begin);
            byte[] rawData = ((num8 - num7 == num5 * num6) ? binaryReader.ReadBytes((int)(num8 - num7)) : binaryReader.ReadBytes((int)(ePFImage.tocAddress - num7)));
            ePFImage.frames[i] = new EPFFrame(num2, num, num5, num6, rawData);
        }
        return ePFImage;
    }

    public override string ToString()
    {
        return "{Frames = " + expectedFrames + ", Width = " + width + ", Height = " + height + ", TOC Address = 0x" + tocAddress.ToString("X").PadLeft(8, '0') + "}";
    }
}
