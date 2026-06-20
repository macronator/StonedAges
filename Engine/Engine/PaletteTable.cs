using System;
using System.Collections.Generic;
using System.IO;

namespace Engine;

public class PaletteTable
{
    private List<PaletteTableEntry> entries = new List<PaletteTableEntry>();

    private Dictionary<int, Palette256> palettes = new Dictionary<int, Palette256>();

    private List<PaletteTableEntry> overrides = new List<PaletteTableEntry>();

    public Palette256 this[int index]
    {
        get
        {
            int num = 0;
            foreach (PaletteTableEntry @override in overrides)
            {
                if (index >= @override.Min && index <= @override.Max)
                {
                    num = @override.Palette;
                }
            }
            foreach (PaletteTableEntry entry in entries)
            {
                if (index >= entry.Min && index <= entry.Max)
                {
                    num = entry.Palette;
                }
            }
            if (num >= 1000)
            {
                num -= 1000;
            }
            return palettes[num];
        }
    }

    public PaletteTable(string pattern, DATArchive dat)
    {
        LoadTables(pattern, dat);
        LoadPalettes(pattern, dat);
    }

    private int LoadTableInternal(Stream stream)
    {
        stream.Seek(0L, SeekOrigin.Begin);
        StreamReader streamReader = new StreamReader(stream);
        entries.Clear();
        while (!streamReader.EndOfStream)
        {
            string[] array = streamReader.ReadLine().Split(' ');
            if (array.Length == 3)
            {
                entries.Add(new PaletteTableEntry(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2])));
            }
            else if (array.Length == 2)
            {
                int num = Convert.ToInt32(array[0]);
                int max = num;
                int palette = Convert.ToInt32(array[1]);
                entries.Add(new PaletteTableEntry(num, max, palette));
            }
        }
        streamReader.Close();
        return entries.Count;
    }

    public int LoadPalettes(string pattern, DATArchive archive)
    {
        palettes.Clear();
        DATFileEntry[] files = archive.Files;
        foreach (DATFileEntry dATFileEntry in files)
        {
            if (dATFileEntry.Name.ToUpper().EndsWith(".PAL") && dATFileEntry.Name.ToUpper().StartsWith(pattern.ToUpper()))
            {
                palettes.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(dATFileEntry.Name).Remove(0, pattern.Length)), Palette256.FromArchive(dATFileEntry.Name, archive));
            }
        }
        return palettes.Count;
    }

    public int LoadPalettes(string pattern, string path)
    {
        string[] files = Directory.GetFiles(path, "*.PAL", SearchOption.TopDirectoryOnly);
        palettes.Clear();
        string[] array = files;
        foreach (string text in array)
        {
            if (Path.GetFileName(text).ToUpper().EndsWith(".PAL") && Path.GetFileName(text).ToUpper().StartsWith(pattern.ToUpper()))
            {
                palettes.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(text).Remove(0, pattern.Length)), Palette256.FromFile(text));
            }
        }
        return palettes.Count;
    }

    public int LoadTables(string pattern, DATArchive archive)
    {
        entries.Clear();
        DATFileEntry[] files = archive.Files;
        foreach (DATFileEntry dATFileEntry in files)
        {
            if (!dATFileEntry.Name.ToUpper().EndsWith(".TBL") || !dATFileEntry.Name.ToUpper().StartsWith(pattern.ToUpper()))
            {
                continue;
            }
            string text = Path.GetFileNameWithoutExtension(dATFileEntry.Name).Remove(0, pattern.Length);
            if (!(text != "ani") || !(text != "ect"))
            {
                continue;
            }
            StreamReader streamReader = new StreamReader(new MemoryStream(archive.ExtractFile(dATFileEntry)));
            while (!streamReader.EndOfStream)
            {
                string[] array = streamReader.ReadLine().Split(' ');
                if (array.Length == 3)
                {
                    int min = Convert.ToInt32(array[0]);
                    int max = Convert.ToInt32(array[1]);
                    int palette = Convert.ToInt32(array[2]);
                    int result = 0;
                    if (int.TryParse(text, out result))
                    {
                        overrides.Add(new PaletteTableEntry(min, max, palette));
                    }
                    else
                    {
                        entries.Add(new PaletteTableEntry(min, max, palette));
                    }
                }
                else if (array.Length == 2)
                {
                    int num = Convert.ToInt32(array[0]);
                    int max2 = num;
                    int palette2 = Convert.ToInt32(array[1]);
                    int result2 = 0;
                    if (int.TryParse(text, out result2))
                    {
                        overrides.Add(new PaletteTableEntry(num, max2, palette2));
                    }
                    else
                    {
                        entries.Add(new PaletteTableEntry(num, max2, palette2));
                    }
                }
            }
            streamReader.Close();
        }
        return entries.Count;
    }

    public int LoadTables(string pattern, string path)
    {
        string[] files = Directory.GetFiles(path, pattern + "*.TBL", SearchOption.TopDirectoryOnly);
        entries.Clear();
        string[] array = files;
        foreach (string path2 in array)
        {
            if (!Path.GetFileName(path2).ToUpper().EndsWith(".TBL") || !Path.GetFileName(path2).ToUpper().StartsWith(pattern.ToUpper()))
            {
                continue;
            }
            string text = Path.GetFileNameWithoutExtension(path2).Remove(0, pattern.Length);
            if (!(text != "ani"))
            {
                continue;
            }
            string[] array2 = File.ReadAllLines(path2);
            foreach (string text2 in array2)
            {
                char[] separator = new char[1] { ' ' };
                string[] array3 = text2.Split(separator);
                if (array3.Length == 3)
                {
                    int min = Convert.ToInt32(array3[0]);
                    int max = Convert.ToInt32(array3[1]);
                    int palette = Convert.ToInt32(array3[2]);
                    int result = 0;
                    if (int.TryParse(text, out result))
                    {
                        overrides.Add(new PaletteTableEntry(min, max, palette));
                    }
                    else
                    {
                        entries.Add(new PaletteTableEntry(min, max, palette));
                    }
                }
                else if (array3.Length == 2)
                {
                    int num = Convert.ToInt32(array3[0]);
                    int max2 = num;
                    int palette2 = Convert.ToInt32(array3[1]);
                    int result2 = 0;
                    if (int.TryParse(text, out result2))
                    {
                        overrides.Add(new PaletteTableEntry(num, max2, palette2));
                    }
                    else
                    {
                        entries.Add(new PaletteTableEntry(num, max2, palette2));
                    }
                }
            }
        }
        return entries.Count;
    }

    public override string ToString()
    {
        return "{Entries = " + entries.Count + ", Palettes = " + (string)(object)palettes.Count + "}";
    }
}
