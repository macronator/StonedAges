using System.IO;
using System.Text;

namespace Engine;

public class DATArchive
{
    private int expectedFiles;

    private string filename;

    private DATFileEntry[] files;

    public int ExpectedFiles => expectedFiles;

    public string FileName
    {
        get
        {
            return filename;
        }
        set
        {
            filename = value;
        }
    }

    public DATFileEntry[] Files => files;

    public DATFileEntry this[int index]
    {
        get
        {
            return files[index];
        }
        set
        {
            files[index] = value;
        }
    }

    public bool Contains(string name)
    {
        DATFileEntry[] array = files;
        foreach (DATFileEntry dATFileEntry in array)
        {
            if (dATFileEntry.Name == name)
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(string name, bool ignoreCase)
    {
        DATFileEntry[] array = files;
        foreach (DATFileEntry dATFileEntry in array)
        {
            if (ignoreCase)
            {
                if (dATFileEntry.Name.ToUpper() == name.ToUpper())
                {
                    return true;
                }
            }
            else if (dATFileEntry.Name == name)
            {
                return true;
            }
        }
        return false;
    }

    public byte[] ExtractFile(DATFileEntry entry)
    {
        if (!Contains(entry.Name))
        {
            return null;
        }
        FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryReader binaryReader = new BinaryReader(input);
        binaryReader.BaseStream.Seek(entry.StartAddress, SeekOrigin.Begin);
        byte[] result = binaryReader.ReadBytes((int)entry.FileSize);
        binaryReader.Close();
        return result;
    }

    public byte[] ExtractFile(string name)
    {
        if (!Contains(name))
        {
            return null;
        }
        FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryReader binaryReader = new BinaryReader(input);
        int num = IndexOf(name);
        binaryReader.BaseStream.Seek(files[num].StartAddress, SeekOrigin.Begin);
        byte[] result = binaryReader.ReadBytes((int)files[num].FileSize);
        binaryReader.Close();
        return result;
    }

    public byte[] ExtractFile(string name, bool ignoreCase)
    {
        if (!Contains(name, ignoreCase))
        {
            return null;
        }
        FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryReader binaryReader = new BinaryReader(input);
        int num = IndexOf(name, ignoreCase);
        binaryReader.BaseStream.Seek(files[num].StartAddress, SeekOrigin.Begin);
        byte[] result = binaryReader.ReadBytes((int)files[num].FileSize);
        binaryReader.Close();
        return result;
    }

    public static DATArchive FromFile(string file)
    {
        FileStream input = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryReader binaryReader = new BinaryReader(input);
        DATArchive dATArchive = new DATArchive();
        dATArchive.filename = file;
        dATArchive.expectedFiles = binaryReader.ReadInt32();
        dATArchive.files = new DATFileEntry[dATArchive.expectedFiles - 1];
        for (int i = 0; i < dATArchive.expectedFiles - 1; i++)
        {
            long startAddress = binaryReader.ReadUInt32();
            string text = Encoding.ASCII.GetString(binaryReader.ReadBytes(13));
            long endAddress = binaryReader.ReadUInt32();
            binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
            int num = text.IndexOf('\0');
            if (num != -1)
            {
                text = text.Remove(num, 13 - num);
            }
            dATArchive.files[i] = new DATFileEntry(text, startAddress, endAddress);
        }
        binaryReader.Close();
        return dATArchive;
    }

    public int IndexOf(string name)
    {
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name == name)
            {
                return i;
            }
        }
        return -1;
    }

    public int IndexOf(string name, bool ignoreCase)
    {
        for (int i = 0; i < files.Length; i++)
        {
            if (ignoreCase)
            {
                if (files[i].Name.ToUpper() == name.ToUpper())
                {
                    return i;
                }
            }
            else if (files[i].Name == name)
            {
                return i;
            }
        }
        return -1;
    }

    public override string ToString()
    {
        return "{Name = " + filename + ", Files = " + expectedFiles + "}";
    }
}
