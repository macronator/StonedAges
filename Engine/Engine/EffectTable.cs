using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine;

public class EffectTable
{
    private List<List<int>> entries = new List<List<int>>();

    public List<int> this[int index] => entries[index];

    public EffectTable(DATArchive dat)
    {
        LoadTables(dat);
    }

    public int LoadTables(DATArchive archive)
    {
        entries.Clear();
        DATFileEntry[] files = archive.Files;
        foreach (DATFileEntry dATFileEntry in files)
        {
            if (!(dATFileEntry.Name == "effect.tbl"))
            {
                continue;
            }
            StreamReader streamReader = new StreamReader(new MemoryStream(archive.ExtractFile(dATFileEntry)));
            streamReader.ReadLine();
            while (!streamReader.EndOfStream)
            {
                List<int> list = new List<int>();
                string[] array = streamReader.ReadLine().Trim().Split(' ');
                string[] array2 = array;
                foreach (string value in array2)
                {
                    list.Add(Convert.ToInt32(value));
                }
                if (entries.Count == 61 || entries.Count == 62 || entries.Count == 69 || entries.Count == 165 || entries.Count == 191 || entries.Count == 203)
                {
                    list.Remove(list.Last());
                    list.Remove(list.Last());
                }
                else if (entries.Count == 106)
                {
                    list.Remove(list.Last());
                }
                else if (entries.Count == 149)
                {
                    list.Clear();
                    list.Add(1);
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(1);
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                }
                entries.Add(list);
            }
            streamReader.Close();
        }
        return entries.Count;
    }
}
