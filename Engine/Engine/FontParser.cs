using System;
using System.Collections.Generic;
using System.IO;

namespace Engine;

public class FontParser
{
    private static int HeaderSize = 4;

    private static int GetValue(string s)
    {
        string s2 = s.Substring(s.IndexOf('=') + 1);
        return int.Parse(s2);
    }

    public static Dictionary<char, CharacterData> Parse(string filePath)
    {
        Dictionary<char, CharacterData> dictionary = new Dictionary<char, CharacterData>();
        string[] array = File.ReadAllLines(filePath);
        for (int i = HeaderSize; i < array.Length; i++)
        {
            string text = array[i];
            string[] array2 = text.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            CharacterData characterData = new CharacterData();
            characterData.Id = GetValue(array2[1]);
            characterData.X = GetValue(array2[2]);
            characterData.Y = GetValue(array2[3]);
            characterData.Width = GetValue(array2[4]);
            characterData.Height = GetValue(array2[5]);
            characterData.XOffset = GetValue(array2[6]);
            characterData.YOffset = GetValue(array2[7]);
            characterData.XAdvance = GetValue(array2[8]);
            CharacterData characterData2 = characterData;
            dictionary.Add((char)characterData2.Id, characterData2);
        }
        return dictionary;
    }
}
