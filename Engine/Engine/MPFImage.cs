using System.IO;

namespace Engine;

public class MPFImage
{
    private int expectedFrames;

    private int width;

    private int height;

    private MPFFrame[] frames;

    private uint expectedDataSize;

    private uint ffUnknown;

    private bool isNewFormat;

    private bool isFFFormat;

    private string palette;

    private string name;

    private int size;

    private byte walklength;

    private byte walkstart;

    private byte attack1length;

    private byte attack1start;

    private byte attack2length;

    private byte attack2start;

    private byte attack3length;

    private byte attack3start;

    private byte idlelength;

    private byte idlestart;

    private byte idle2length;

    private byte unknownb;

    public byte WalkStart
    {
        get
        {
            return walkstart;
        }
        set
        {
            walkstart = value;
        }
    }

    public byte WalkLength
    {
        get
        {
            return walklength;
        }
        set
        {
            walklength = value;
        }
    }

    public byte IdleStart
    {
        get
        {
            return idlestart;
        }
        set
        {
            idlestart = value;
        }
    }

    public byte IdleLength
    {
        get
        {
            return idlelength;
        }
        set
        {
            idlelength = value;
        }
    }

    public byte Attack1Start
    {
        get
        {
            return attack1start;
        }
        set
        {
            attack1start = value;
        }
    }

    public byte Attack1Length
    {
        get
        {
            return attack1length;
        }
        set
        {
            attack1length = value;
        }
    }

    public byte Attack2Start
    {
        get
        {
            return attack2start;
        }
        set
        {
            attack2start = value;
        }
    }

    public byte Attack2Length
    {
        get
        {
            return attack2length;
        }
        set
        {
            attack2length = value;
        }
    }

    public byte Attack3Start
    {
        get
        {
            return attack3start;
        }
        set
        {
            attack3start = value;
        }
    }

    public byte Attack3Length
    {
        get
        {
            return attack3length;
        }
        set
        {
            attack3length = value;
        }
    }

    public byte Idle2Length
    {
        get
        {
            return idle2length;
        }
        set
        {
            idle2length = value;
        }
    }

    public byte UnknownB
    {
        get
        {
            return unknownb;
        }
        set
        {
            unknownb = value;
        }
    }

    public int Size => size;

    public string Palette => palette;

    public string Name => name;

    public MPFFrame this[int index]
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

    public bool IsFFFormat => isFFFormat;

    public bool IsNewFormat => isNewFormat;

    public uint FFUnknown => ffUnknown;

    public uint ExpectedDataSize => expectedDataSize;

    public MPFFrame[] Frames => frames;

    public int Height => height;

    public int Width => width;

    public int ExpectedFrames => expectedFrames;

    public static MPFImage FromRawData(byte[] data, string file)
    {
        return LoadMPF(new MemoryStream(data), file, data.Length);
    }

    public static MPFImage FromArchive(string file, DATArchive archive)
    {
        if (!archive.Contains(file, ignoreCase: true))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file), file);
    }

    public static MPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
    {
        if (!archive.Contains(file, ignoreCase))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file, ignoreCase), file);
    }

    private static MPFImage LoadMPF(Stream stream, string file, int size)
    {
        stream.Seek(0L, SeekOrigin.Begin);
        BinaryReader binaryReader = new BinaryReader(stream);
        MPFImage mPFImage = new MPFImage();
        mPFImage.name = file;
        mPFImage.size = size;
        if (binaryReader.ReadInt32() == -1)
        {
            mPFImage.isFFFormat = true;
            mPFImage.ffUnknown = binaryReader.ReadUInt32();
        }
        else
        {
            binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
        }
        mPFImage.expectedFrames = binaryReader.ReadByte();
        mPFImage.frames = new MPFFrame[mPFImage.expectedFrames];
        mPFImage.width = binaryReader.ReadUInt16();
        mPFImage.height = binaryReader.ReadUInt16();
        mPFImage.expectedDataSize = binaryReader.ReadUInt32();
        mPFImage.walkstart = binaryReader.ReadByte();
        mPFImage.walklength = binaryReader.ReadByte();
        mPFImage.isNewFormat = binaryReader.ReadUInt16() == ushort.MaxValue;
        if (mPFImage.isNewFormat)
        {
            mPFImage.idlestart = binaryReader.ReadByte();
            mPFImage.idlelength = binaryReader.ReadByte();
            mPFImage.idle2length = binaryReader.ReadByte();
            mPFImage.unknownb = binaryReader.ReadByte();
            mPFImage.attack1start = binaryReader.ReadByte();
            mPFImage.attack1length = binaryReader.ReadByte();
            mPFImage.attack2start = binaryReader.ReadByte();
            mPFImage.attack2length = binaryReader.ReadByte();
            mPFImage.attack3start = binaryReader.ReadByte();
            mPFImage.attack3length = binaryReader.ReadByte();
        }
        else
        {
            binaryReader.BaseStream.Seek(-2L, SeekOrigin.Current);
            mPFImage.attack1start = binaryReader.ReadByte();
            mPFImage.attack1length = binaryReader.ReadByte();
            mPFImage.idlestart = binaryReader.ReadByte();
            mPFImage.idlelength = binaryReader.ReadByte();
            mPFImage.idle2length = binaryReader.ReadByte();
            mPFImage.unknownb = binaryReader.ReadByte();
        }
        long num = binaryReader.BaseStream.Length - mPFImage.expectedDataSize;
        for (int i = 0; i < mPFImage.expectedFrames; i++)
        {
            int num2 = binaryReader.ReadUInt16();
            int num3 = binaryReader.ReadUInt16();
            int num4 = binaryReader.ReadUInt16();
            int num5 = binaryReader.ReadUInt16();
            int num6 = num4 - num2;
            int num7 = num5 - num3;
            int xOffset = binaryReader.ReadUInt16();
            int yOffset = binaryReader.ReadUInt16();
            long num8 = binaryReader.ReadUInt32();
            if (num2 == 65535 && num3 == 65535)
            {
                mPFImage.palette = "mns" + num8.ToString("d3") + ".pal";
                mPFImage.expectedFrames--;
            }
            else
            {
                mPFImage.palette = "mns000.pal";
            }
            byte[] rawData = null;
            if (num7 > 0 && num6 > 0)
            {
                long position = binaryReader.BaseStream.Position;
                binaryReader.BaseStream.Seek(num + num8, SeekOrigin.Begin);
                rawData = binaryReader.ReadBytes(num7 * num6);
                binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
            }
            mPFImage.frames[i] = new MPFFrame(num2, num3, num6, num7, xOffset, yOffset, rawData);
        }
        binaryReader.Close();
        return mPFImage;
    }

    public override string ToString()
    {
        return "{Frames = " + expectedFrames + ", Width = " + width + ", Height = " + height + ", Unknown = 0x}";
    }
}
