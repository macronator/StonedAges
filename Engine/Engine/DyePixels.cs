using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Engine;

public class DyePixels
{
    private Dictionary<int, SixColors> _sixColorsDatabase = new Dictionary<int, SixColors>();

    public SixColors Get(int sixColorID)
    {
        return _sixColorsDatabase[sixColorID];
    }

    public SixColors GetCustom(int sixColorID)
    {
        SixColors sixColors = new SixColors(sixColorID);
        if (sixColorID == 11)
        {
            sixColors.color1 = System.Drawing.Color.FromArgb(23, 23, 35);
            sixColors.color2 = System.Drawing.Color.FromArgb(31, 31, 31);
            sixColors.color3 = System.Drawing.Color.FromArgb(35, 35, 51);
            sixColors.color4 = System.Drawing.Color.FromArgb(51, 51, 51);
            sixColors.color5 = System.Drawing.Color.FromArgb(67, 67, 67);
        }
        return sixColors;
    }

    public static DyePixels FromRawData(byte[] data)
    {
        return LoadPalette(data);
    }

    public static DyePixels FromArchive(string file, DATArchive archive)
    {
        if (!archive.Contains(file, ignoreCase: true))
        {
            return null;
        }
        return FromRawData(archive.ExtractFile(file));
    }

    private static DyePixels LoadPalette(byte[] data)
    {
        string @string = Encoding.ASCII.GetString(data);
        DyePixels dyePixels = new DyePixels();
        string[] array = @string.Split('\n');
        for (int i = 1; i < 504; i += 7)
        {
            int num = int.Parse(array[i]);
            SixColors sixColors = new SixColors(num);
            string[] array2 = array[i + 1].Split(',');
            sixColors.color1 = System.Drawing.Color.FromArgb(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]));
            array2 = array[i + 2].Split(',');
            int num2 = int.Parse(array2[1]);
            if (num2 == 491)
            {
                num2 = 235;
            }
            sixColors.color2 = System.Drawing.Color.FromArgb(int.Parse(array2[0]), num2, int.Parse(array2[2]));
            array2 = array[i + 3].Split(',');
            sixColors.color3 = System.Drawing.Color.FromArgb(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]));
            array2 = array[i + 4].Split(',');
            sixColors.color4 = System.Drawing.Color.FromArgb(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]));
            array2 = array[i + 5].Split(',');
            sixColors.color5 = System.Drawing.Color.FromArgb(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]));
            array2 = array[i + 6].Split(',');
            sixColors.color6 = System.Drawing.Color.FromArgb(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]));
            dyePixels._sixColorsDatabase.Add(num, sixColors);
        }
        return dyePixels;
    }
}
