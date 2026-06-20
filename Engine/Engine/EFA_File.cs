using System.IO;

namespace Engine;

public sealed class EFA_File
{
    private uint m_framecount;

    private EFA_Frame[] m_frames;

    private string m_fileName;

    public EFA_Frame this[int index]
    {
        get
        {
            return Frames[index];
        }
        set
        {
            Frames[index] = value;
        }
    }

    public uint FrameCount => m_framecount;

    public EFA_Frame[] Frames => m_frames;

    public string FileName => m_fileName;

    private void FrameHeadersFromReader(BinaryReader reader)
    {
        for (int i = 0; i < FrameCount; i++)
        {
            EFA_Frame_Header h = EFA_Frame_Header.FromBinaryReaderBlock(reader);
            Frames[i] = new EFA_Frame(h);
        }
    }

    private void FrameDataFromReader(BinaryReader reader)
    {
        for (int i = 0; i < FrameCount; i++)
        {
            Frames[i].Render(reader.ReadBytes(Frames[i].Size));
        }
    }

    public static EFA_File FromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return null;
        }
        BinaryReader binaryReader = new BinaryReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
        if (binaryReader == null)
        {
            return null;
        }
        EFA_File eFA_File = new EFA_File();
        eFA_File.m_fileName = fileName;
        binaryReader.ReadUInt32();
        eFA_File.m_framecount = binaryReader.ReadUInt32();
        binaryReader.ReadBytes(56);
        eFA_File.m_frames = new EFA_Frame[eFA_File.FrameCount];
        eFA_File.FrameHeadersFromReader(binaryReader);
        eFA_File.FrameDataFromReader(binaryReader);
        binaryReader.Close();
        return eFA_File;
    }

    public static EFA_File FromArchive(string fileName, DATArchive dat)
    {
        return new EFA_File();
    }
}
