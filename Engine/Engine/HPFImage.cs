using System;
using System.IO;
using System.Windows.Forms;

namespace Engine;

public class HPFImage
{
    private int width;

    private int height;

    private byte[] rawData;

    private byte[] headerData;

    public byte[] HeaderData => headerData;

    public byte[] RawData => rawData;

    public int Height => height;

    public int Width => width;

    public static HPFImage FromFile(string file)
    {
        return LoadHPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public static HPFImage FromRawData(byte[] data)
    {
        return LoadHPF(new MemoryStream(data));
    }

    public static HPFImage FromArchive(string file, DATArchive archive)
    {
        if (!archive.Contains(file))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file));
    }

    public static HPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
    {
        if (!archive.Contains(file, ignoreCase))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file, ignoreCase));
    }

    private static HPFImage LoadHPF(Stream stream)
    {
        stream.Seek(0L, SeekOrigin.Begin);
        BinaryReader binaryReader = new BinaryReader(stream);
        uint num = binaryReader.ReadUInt32();
        binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
        if (num != 4278364757u)
        {
            MessageBox.Show("Invalid file format, does not contain HPF signature bytes.");
            throw new ArgumentException("Invalid file format, does not contain HPF signature bytes.");
        }
        HPFImage hPFImage = new HPFImage();
        byte[] array = HPFCompression.Decompress(binaryReader.ReadBytes((int)binaryReader.BaseStream.Length));
        binaryReader.Close();
        BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(array));
        hPFImage.headerData = binaryReader2.ReadBytes(8);
        hPFImage.rawData = binaryReader2.ReadBytes(array.Length - 8);
        binaryReader2.Close();
        hPFImage.width = 28;
        if (hPFImage.rawData.Length % hPFImage.width != 0)
        {
            MessageBox.Show("HPF file does not use the standard 28 pixel width.");
            throw new ArgumentException("HPF file does not use the standard 28 pixel width.");
        }
        hPFImage.height = hPFImage.rawData.Length / hPFImage.width;
        return hPFImage;
    }

    public override string ToString()
    {
        return "{Width = " + width + ", Height = " + height + "}";
    }
}
