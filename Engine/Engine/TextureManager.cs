using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace Engine;

public class TextureManager : IDisposable
{
    private ConcurrentDictionary<string, Texture> _textureDatabase = new ConcurrentDictionary<string, Texture>();

    private Dictionary<string, EPFImage> _epfImages = new Dictionary<string, EPFImage>();

    private Dictionary<string, Image> _loadedEPFS = new Dictionary<string, Image>();

    private Dictionary<string, Image> _loadedHPFS = new Dictionary<string, Image>();

    private Dictionary<string, Image> _loadedMPFS = new Dictionary<string, Image>();

    private static int[] _oldFlips = new int[178]
    {
        19, 20, 21, 22, 23, 26, 29, 32, 35, 37,
        43, 44, 56, 57, 58, 70, 71, 72, 73, 74,
        75, 76, 77, 78, 83, 84, 93, 94, 95, 96,
        106, 107, 108, 109, 110, 111, 112, 113, 114, 115,
        116, 117, 118, 119, 120, 121, 122, 123, 124, 125,
        126, 127, 128, 129, 130, 131, 132, 133, 134, 135,
        142, 155, 161, 162, 163, 172, 173, 174, 221, 222,
        223, 224, 225, 226, 230, 231, 232, 233, 234, 244,
        245, 246, 247, 248, 249, 250, 403, 434, 435, 436,
        437, 438, 439, 440, 441, 442, 443, 444, 445, 446,
        447, 448, 449, 450, 451, 452, 453, 454, 455, 456,
        493, 502, 516, 517, 518, 519, 520, 521, 522, 524,
        525, 526, 528, 530, 567, 568, 569, 570, 571, 572,
        573, 574, 575, 576, 577, 578, 579, 585, 586, 587,
        588, 589, 599, 601, 603, 604, 614, 616, 617, 618,
        619, 620, 621, 622, 623, 624, 629, 630, 631, 632,
        633, 671, 673, 674, 677, 679, 686, 688, 689, 720,
        731, 735, 736, 739, 789, 832, 894, 924
    };

    private static int[] _mydaFlips = new int[6] { 158, 232, 233, 234, 238, 239 };

    private static DATArchive oldDat = DATArchive.FromFile("dats\\DarkAges.dat");

    private static Palette256 oldPal = Palette256.FromArchive("legend.pal", oldDat);

    private static Tileset tilesb = Tileset.FromArchive("TILEA.BMP", oldDat);

    private static DATArchive mydaDat = DATArchive.FromFile("dats\\Legends.dat");

    private static DATArchive newDat = DATArchive.FromFile("dats\\Legend.dat");

    private static DyePixels _dyeColors = DyePixels.FromArchive("color0.tbl", newDat);

    private static DATArchive setoaDat = DATArchive.FromFile("dats\\setoa.dat");

    private static DATArchive iaDat = DATArchive.FromFile("dats\\ia.dat");

    private static DATArchive hadesDat = DATArchive.FromFile("dats\\hades.dat");

    private static DATArchive seoDat = DATArchive.FromFile("dats\\seo.dat");

    private static DATArchive khanmadDat = DATArchive.FromFile("dats\\khanmad.dat");

    private static DATArchive khanmehDat = DATArchive.FromFile("dats\\khanmeh.dat");

    private static DATArchive khanmimDat = DATArchive.FromFile("dats\\khanmim.dat");

    private static DATArchive khanmnsDat = DATArchive.FromFile("dats\\khanmns.dat");

    private static DATArchive khanmtzDat = DATArchive.FromFile("dats\\khanmtz.dat");

    private static DATArchive khanpalDat = DATArchive.FromFile("dats\\khanpal.dat");

    private static DATArchive khanwadDat = DATArchive.FromFile("dats\\khanwad.dat");

    private static DATArchive khanwehDat = DATArchive.FromFile("dats\\khanweh.dat");

    private static DATArchive khanwimDat = DATArchive.FromFile("dats\\khanwim.dat");

    private static DATArchive khanwnsDat = DATArchive.FromFile("dats\\khanwns.dat");

    private static DATArchive khanwtzDat = DATArchive.FromFile("dats\\khanwtz.dat");

    private static DATArchive rohDat = DATArchive.FromFile("dats\\roh.dat");

    private static byte[] sotp = iaDat.ExtractFile("sotp.dat");

    private static Tileset tilesa = Tileset.FromArchive("TILEA.BMP", seoDat);

    private static PaletteTable tileTable = new PaletteTable("mpt", seoDat);

    private static PaletteTable wallTable = new PaletteTable("stc", iaDat);

    private static EffectTable effectTable = new EffectTable(rohDat);

    private static PaletteTable effPalTable = new PaletteTable("eff", rohDat);

    private static int[] _badMPFS = new int[19]
    {
        257, 258, 259, 260, 261, 262, 265, 268, 269, 270,
        272, 274, 275, 278, 283, 284, 285, 286, 287
    };

    private static int[] _badMydaMPFS = new int[13]
    {
        34, 35, 40, 43, 99, 100, 101, 102, 103, 104,
        105, 224, 235
    };

    private static int[] _badNewMPFS = new int[11]
    {
        195, 676, 690, 691, 699, 700, 701, 709, 713, 714,
        721
    };

    private static int[] _openDoors = new int[62]
    {
        3218, 3219, 3158, 3159, 3126, 3127, 3098, 2903, 2904, 2923,
        2924, 2999, 3000, 1996, 1997, 2003, 2167, 2168, 2169, 2192,
        2193, 2194, 2231, 2232, 2233, 2264, 2265, 2266, 2295, 2296,
        2297, 2324, 2325, 2326, 2432, 2465, 2952, 2977, 3025, 3067,
        3186, 3187, 4767, 4768, 2680, 2681, 2695, 2696, 2721, 2722,
        2734, 2735, 2768, 2769, 2783, 2784, 2857, 2858, 2859, 2881,
        2882, 2883
    };

    private static int[] _closedDoors = new int[64]
    {
        3210, 3211, 3150, 3151, 3119, 3118, 3090, 2897, 2898, 2929,
        2930, 2993, 2994, 1993, 1994, 2000, 2163, 2164, 2165, 2196,
        2197, 2198, 2227, 2228, 2229, 2260, 2261, 2262, 2291, 2292,
        2293, 2328, 2329, 2330, 2435, 2436, 2437, 2461, 2946, 2971,
        3019, 3059, 3178, 3179, 4765, 4766, 2673, 2674, 2688, 2689,
        2714, 2715, 2727, 2728, 2761, 2762, 2776, 2777, 2850, 2851,
        2852, 2874, 2875, 2876
    };

    private static Dictionary<int, int> _altDoors = new Dictionary<int, int>
    {
        { 913, 916 },
        { 914, 917 },
        { 915, 918 },
        { 916, 913 },
        { 917, 914 },
        { 918, 915 },
        { 919, 922 },
        { 920, 923 },
        { 921, 924 },
        { 922, 919 },
        { 923, 920 },
        { 924, 921 },
        { 1993, 1996 },
        { 1996, 1993 },
        { 1994, 1997 },
        { 1997, 1994 },
        { 2000, 2003 },
        { 2003, 2000 },
        { 2001, 2004 },
        { 2004, 2001 },
        { 2163, 2167 },
        { 2167, 2163 },
        { 2164, 2168 },
        { 2168, 2164 },
        { 2165, 2169 },
        { 2169, 2165 },
        { 2196, 2192 },
        { 2192, 2196 },
        { 2197, 2193 },
        { 2193, 2197 },
        { 2198, 2194 },
        { 2194, 2198 },
        { 2227, 2231 },
        { 2231, 2227 },
        { 2228, 2232 },
        { 2232, 2228 },
        { 2229, 2233 },
        { 2233, 2229 },
        { 2260, 2264 },
        { 2264, 2260 },
        { 2261, 2265 },
        { 2265, 2261 },
        { 2262, 2266 },
        { 2266, 2262 },
        { 2291, 2295 },
        { 2295, 2291 },
        { 2292, 2296 },
        { 2296, 2292 },
        { 2293, 2297 },
        { 2297, 2293 },
        { 2328, 2324 },
        { 2324, 2328 },
        { 2329, 2325 },
        { 2325, 2329 },
        { 2330, 2326 },
        { 2326, 2330 },
        { 2436, 2432 },
        { 2432, 2436 },
        { 2461, 2465 },
        { 2465, 2461 },
        { 2673, 2680 },
        { 2680, 2673 },
        { 2674, 2681 },
        { 2681, 2674 },
        { 2688, 2695 },
        { 2695, 2688 },
        { 2689, 2696 },
        { 2696, 2689 },
        { 2714, 2721 },
        { 2721, 2714 },
        { 2715, 2722 },
        { 2722, 2715 },
        { 2727, 2734 },
        { 2734, 2727 },
        { 2728, 2735 },
        { 2735, 2728 },
        { 2761, 2768 },
        { 2768, 2761 },
        { 2762, 2769 },
        { 2769, 2762 },
        { 2776, 2783 },
        { 2783, 2776 },
        { 2777, 2784 },
        { 2784, 2777 },
        { 2850, 2857 },
        { 2857, 2850 },
        { 2851, 2858 },
        { 2858, 2851 },
        { 2852, 2859 },
        { 2859, 2852 },
        { 2874, 2881 },
        { 2881, 2874 },
        { 2875, 2882 },
        { 2882, 2875 },
        { 2876, 2883 },
        { 2883, 2876 },
        { 2897, 2903 },
        { 2903, 2897 },
        { 2898, 2904 },
        { 2904, 2898 },
        { 2923, 2929 },
        { 2929, 2923 },
        { 2924, 2930 },
        { 2930, 2924 },
        { 2946, 2952 },
        { 2952, 2946 },
        { 2971, 2977 },
        { 2977, 2971 },
        { 2993, 2999 },
        { 2999, 2993 },
        { 2994, 3000 },
        { 3000, 2994 },
        { 3019, 3025 },
        { 3025, 3019 },
        { 3059, 3067 },
        { 3067, 3059 },
        { 3090, 3098 },
        { 3098, 3090 },
        { 3118, 3126 },
        { 3126, 3118 },
        { 3119, 3127 },
        { 3127, 3119 },
        { 3150, 3158 },
        { 3158, 3150 },
        { 3151, 3159 },
        { 3159, 3151 },
        { 3178, 3186 },
        { 3186, 3178 },
        { 3179, 3187 },
        { 3187, 3179 },
        { 3210, 3218 },
        { 3218, 3210 },
        { 3211, 3219 },
        { 3219, 3211 },
        { 4765, 4767 },
        { 4767, 4765 },
        { 4766, 4768 },
        { 4768, 4766 }
    };

    public Texture Get(string textureId, string type = ".epf", string source = "old", MPFImage mImage = null, bool khan = false, bool bloody = false)
    {
        if (_loadedEPFS.ContainsKey(textureId))
        {
            LoadTexture(textureId, _loadedEPFS[textureId]);
            return _textureDatabase[textureId];
        }
        if (_loadedHPFS.ContainsKey(textureId))
        {
            LoadTexture(textureId, _loadedHPFS[textureId]);
            return _textureDatabase[textureId];
        }
        if (_loadedMPFS.ContainsKey(textureId))
        {
            LoadTexture(textureId, _loadedMPFS[textureId]);
            return _textureDatabase[textureId];
        }
        if (!_textureDatabase.ContainsKey(textureId))
        {
            int color = 0;
            string text = textureId.Substring(textureId.IndexOf("_F") + 2, 3);
            if (text.Contains("_"))
            {
                text = text.Remove(text.IndexOf("_"));
            }
            string text2 = textureId.Remove(textureId.IndexOf("_F"));
            if (textureId.Contains("_C"))
            {
                color = int.Parse(textureId.Substring(textureId.IndexOf("_C") + 2));
            }
            if (khan)
            {
                LoadKhanTexture(text2 + type, color, source, bloody);
            }
            else
            {
                LoadEPFTexture(text2 + type, color, source, mImage, int.Parse(text));
            }
        }
        if (!_textureDatabase.ContainsKey(textureId))
        {
            _textureDatabase.GetOrAdd(textureId, default(Texture));
        }
        return _textureDatabase[textureId];
    }

    public EPFImage getEPFImage(string id)
    {
        if (_epfImages.ContainsKey(id))
        {
            return _epfImages[id];
        }
        return null;
    }

    public int AltDoor(int key)
    {
        int value = 0;
        _altDoors.TryGetValue(key, out value);
        return value;
    }

    public int[] mydaFlips()
    {
        return _mydaFlips;
    }

    public int[] oldFlips()
    {
        return _oldFlips;
    }

    public int[] badMydaMPFS()
    {
        return _badMydaMPFS;
    }

    public int[] badMPFS()
    {
        return _badMPFS;
    }

    public int[] badNewMPFS()
    {
        return _badNewMPFS;
    }

    public bool openDoors(int wall)
    {
        if (_openDoors.Contains(wall))
        {
            return true;
        }
        return false;
    }

    public bool closedDoors(int wall)
    {
        if (_closedDoors.Contains(wall))
        {
            return true;
        }
        return false;
    }

    public EffectTable EffectTable()
    {
        return effectTable;
    }

    public byte[] SOTP()
    {
        return sotp;
    }

    public Palette256 Pal(string pal)
    {
        return oldPal;
    }

    public DATArchive Dat(string dat)
    {
        return dat switch
        {
            "oldDat" => oldDat,
            "hadesDat" => hadesDat,
            "mydaDat" => mydaDat,
            _ => null,
        };
    }

    public int Count(string text)
    {
        int num = 0;
        foreach (string key in _textureDatabase.Keys)
        {
            if (key.Contains(text))
            {
                num++;
            }
        }
        return num;
    }

    private unsafe void ProcessUsingLockbitsAndUnsafeAndParallel(Bitmap processedBitmap, SixColors defPurple, SixColors newColor)
    {
        try
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
            int bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int height = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            byte* PtrFirstPixel = (byte*)(void*)bitmapData.Scan0;
            Parallel.For(0, height, delegate (int y)
            {
                byte* ptr = PtrFirstPixel + (nint)y * (nint)bitmapData.Stride;
                for (int i = 0; i < widthInBytes; i += bytesPerPixel)
                {
                    int num = ptr[i];
                    int num2 = ptr[i + 1];
                    int num3 = ptr[i + 2];
                    int num4 = ptr[i + 3];
                    if (num == defPurple.color1.B && num2 == defPurple.color1.G && num3 == defPurple.color1.R && num4 == defPurple.color1.A)
                    {
                        ptr[i] = newColor.color1.B;
                        ptr[i + 1] = newColor.color1.G;
                        ptr[i + 2] = newColor.color1.R;
                        ptr[i + 3] = newColor.color1.A;
                    }
                    else if (num == defPurple.color2.B && num2 == defPurple.color2.G && num3 == defPurple.color2.R && num4 == defPurple.color2.A)
                    {
                        ptr[i] = newColor.color2.B;
                        ptr[i + 1] = newColor.color2.G;
                        ptr[i + 2] = newColor.color2.R;
                        ptr[i + 3] = newColor.color2.A;
                    }
                    else if (num == defPurple.color3.B && num2 == defPurple.color3.G && num3 == defPurple.color3.R && num4 == defPurple.color3.A)
                    {
                        ptr[i] = newColor.color3.B;
                        ptr[i + 1] = newColor.color3.G;
                        ptr[i + 2] = newColor.color3.R;
                        ptr[i + 3] = newColor.color3.A;
                    }
                    else if (num == defPurple.color4.B && num2 == defPurple.color4.G && num3 == defPurple.color4.R && num4 == defPurple.color4.A)
                    {
                        ptr[i] = newColor.color4.B;
                        ptr[i + 1] = newColor.color4.G;
                        ptr[i + 2] = newColor.color4.R;
                        ptr[i + 3] = newColor.color4.A;
                    }
                    else if (num == defPurple.color5.B && num2 == defPurple.color5.G && num3 == defPurple.color5.R && num4 == defPurple.color5.A)
                    {
                        ptr[i] = newColor.color5.B;
                        ptr[i + 1] = newColor.color5.G;
                        ptr[i + 2] = newColor.color5.R;
                        ptr[i + 3] = newColor.color5.A;
                    }
                    else if (num == defPurple.color6.B && num2 == defPurple.color6.G && num3 == defPurple.color6.R && num4 == defPurple.color6.A)
                    {
                        ptr[i] = newColor.color6.B;
                        ptr[i + 1] = newColor.color6.G;
                        ptr[i + 2] = newColor.color6.R;
                        ptr[i + 3] = newColor.color6.A;
                    }
                    else if (num == 0 && num2 == 0 && num3 == 255)
                    {
                        ptr[i] = byte.MaxValue;
                        ptr[i + 1] = byte.MaxValue;
                        ptr[i + 2] = byte.MaxValue;
                        ptr[i + 3] = newColor.color6.A;
                    }
                }
            });
            processedBitmap.UnlockBits(bitmapData);
        }
        catch
        {
        }
    }

    public bool zLoadKhanTexture(string name, int color = 0, string source = "old", bool bloody = false)
    {
        string text = name.Substring(0, 5);
        text[0].ToString();
        text[1].ToString();
        int.Parse(text.Substring(2));
        string text2 = text;
        text = "Spritesheets\\" + text + ".png";
        Console.WriteLine(text);
        bool flag = true;
        if (!File.Exists(text))
        {
            flag = false;
        }
        Bitmap bitmap = new Bitmap(1, 1);
        if (flag)
        {
            bitmap = new Bitmap(text);
        }
        if (color > 0)
        {
            SixColors defPurple = _dyeColors.Get(0);
            SixColors newColor = _dyeColors.Get(color);
            ProcessUsingLockbitsAndUnsafeAndParallel(bitmap, defPurple, newColor);
        }
        int num = 0;
        int num2 = 0;
        string text3 = "";
        if (source == "new")
        {
            text3 = "_new";
        }
        if (source == "myda")
        {
            text3 = "_myda";
        }
        string text4 = "";
        if (bloody)
        {
            text4 = "_B";
        }
        for (int i = 1; i <= 10; i++)
        {
            string text5 = text2 + "01_F" + (i - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text5, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text5, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int j = 1; j <= 4; j++)
        {
            string text6 = text2 + "02_F" + (j - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text6, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text6, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int k = 1; k <= 2; k++)
        {
            string text7 = text2 + "03_F" + (k - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text7, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text7, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int l = 3; l <= 6; l++)
        {
            string text8 = text2 + "03_F" + (l - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text8, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text8, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int m = 7; m <= 10; m++)
        {
            string text9 = text2 + "03_F" + (m - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text9, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text9, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int n = 1; n <= 10; n++)
        {
            string text10 = text2 + "b_F" + (n - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text10, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text10, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num3 = 11; num3 <= 24; num3++)
        {
            string text11 = text2 + "b_F" + (num3 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text11, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text11, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num4 = 25; num4 <= 32; num4++)
        {
            string text12 = text2 + "b_F" + (num4 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text12, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text12, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num5 = 33; num5 <= 42; num5++)
        {
            string text13 = text2 + "b_F" + (num5 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text13, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text13, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num6 = 43; num6 <= 50; num6++)
        {
            string text14 = text2 + "b_F" + (num6 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text14, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text14, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num7 = 51; num7 <= 54; num7++)
        {
            string text15 = text2 + "b_F" + (num7 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text15, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text15, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num8 = 55; num8 <= 60; num8++)
        {
            string text16 = text2 + "b_F" + (num8 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text16, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text16, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num9 = 61; num9 <= 66; num9++)
        {
            string text17 = text2 + "b_F" + (num9 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text17, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text17, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num10 = 67; num10 <= 68; num10++)
        {
            string text18 = text2 + "b_F" + (num10 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text18, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text18, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num11 = 1; num11 <= 8; num11++)
        {
            string text19 = text2 + "c_F" + (num11 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text19, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text19, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num12 = 9; num12 <= 14; num12++)
        {
            string text20 = text2 + "c_F" + (num12 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text20, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text20, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num13 = 15; num13 <= 22; num13++)
        {
            string text21 = text2 + "c_F" + (num13 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text21, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text21, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num14 = 23; num14 <= 38; num14++)
        {
            string text22 = text2 + "c_F" + (num14 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text22, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text22, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num15 = 39; num15 <= 42; num15++)
        {
            string text23 = text2 + "c_F" + (num15 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text23, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text23, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num16 = 43; num16 <= 48; num16++)
        {
            string text24 = text2 + "c_F" + (num16 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text24, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text24, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num17 = 49; num17 <= 54; num17++)
        {
            string text25 = text2 + "c_F" + (num17 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text25, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text25, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num18 = 55; num18 <= 58; num18++)
        {
            string text26 = text2 + "c_F" + (num18 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text26, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text26, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num19 = 59; num19 <= 64; num19++)
        {
            string text27 = text2 + "c_F" + (num19 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text27, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text27, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        for (int num20 = 65; num20 <= 70; num20++)
        {
            string text28 = text2 + "c_F" + (num20 - 1) + text4 + text3 + "_C" + color;
            if (flag)
            {
                LoadTexture(text28, num, num2, bitmap);
            }
            else
            {
                _textureDatabase.GetOrAdd(text28, default(Texture));
            }
            num += 111;
        }
        num = 0;
        num2 += 111;
        bitmap.Dispose();
        return true;
    }

    public void LoadTexture(string textureId, int x, int y, Bitmap bmp)
    {
        int textures = 0;
        Gl.glGenTextures(1, out textures);
        Gl.glBindTexture(3553, textures);
        Bitmap bitmap = new Bitmap(111, 111);
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.DrawImage(bmp, 0, 0, new Rectangle(x, y, 111, 111), GraphicsUnit.Pixel);
        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        Gl.glTexImage2D(3553, 0, 6408, bitmapData.Width, bitmapData.Height, 0, 32993, 5121, bitmapData.Scan0);
        bitmap.UnlockBits(bitmapData);
        Gl.glTexParameteri(3553, 10241, 9728);
        Gl.glTexParameteri(3553, 10240, 9728);
        _textureDatabase.GetOrAdd(textureId, new Texture(textures, bitmap.Width, bitmap.Height));
        graphics.Dispose();
        bitmap.Dispose();
    }

    public bool LoadKhanTexture(string name, int color = 0, string source = "old", bool bloody = false)
    {
        DATArchive archive = mydaDat;
        Palette256 palette = oldPal;
        SixColors defPurple = _dyeColors.Get(0);
        SixColors newColor = _dyeColors.Get(color);
        int cstmColorID = 0;
        _ = name[0];
        char c = name[1];
        int num = int.Parse(name.Substring(2, 3));
        char c2 = name[5];
        if (c2 == '0')
        {
            c2 = name[6];
        }
        if ((c == 'u' || c == 'a') && (num == 11 || num == 51 || num == 52 || num == 53 || num == 54))
        {
            cstmColorID = 11;
            defPurple = _dyeColors.GetCustom(11);
        }
        if (c == 'w' && (num > 11 || (num <= 11 && source != "myda")))
        {
            archive = oldDat;
        }
        EPFImage ePFImage = EPFImage.FromArchive(name, ignoreCase: true, archive);
        if (source == "new" || ePFImage == null || name.Contains("l007"))
        {
            int num2 = int.Parse(name.Substring(2, 3));
            if (name.StartsWith("m"))
            {
                switch (c)
                {
                    case 'a':
                    case 'b':
                    case 'c':
                        archive = khanmadDat;
                        break;
                    case 'g':
                    case 'h':
                        archive = khanmehDat;
                        break;
                    case 'i':
                    case 'j':
                    case 'l':
                        archive = khanmimDat;
                        break;
                    case 'n':
                    case 'o':
                    case 's':
                        archive = khanmnsDat;
                        break;
                    case 'u':
                    case 'w':
                        archive = khanmtzDat;
                        break;
                }
            }
            else
            {
                switch (c)
                {
                    case 'a':
                    case 'b':
                    case 'c':
                        archive = khanwadDat;
                        break;
                    case 'g':
                    case 'h':
                        archive = khanwehDat;
                        break;
                    case 'i':
                    case 'j':
                    case 'l':
                        archive = khanwimDat;
                        break;
                    case 'n':
                    case 'o':
                    case 's':
                        archive = khanwnsDat;
                        break;
                    case 'u':
                    case 'w':
                        archive = khanwtzDat;
                        break;
                }
            }
            palette = (name.Contains("o001") ? Palette256.FromArchive("palm000.pal", khanpalDat) : ((c != 'u' || num2 != 333) ? Palette256.FromArchive("palb00.pal", khanpalDat) : Palette256.FromArchive("palu001.pal", khanpalDat)));
        }
        if (source == "myda")
        {
            archive = mydaDat;
        }
        if (name.Contains("mb005"))
        {
            palette = Palette256.FromArchive("palb001.pal", khanpalDat);
            archive = khanmadDat;
        }
        if (name.Contains("wb006"))
        {
            palette = Palette256.FromArchive("palb002.pal", khanpalDat);
            archive = khanwadDat;
        }
        ePFImage = EPFImage.FromArchive(name, ignoreCase: true, archive);
        if (ePFImage == null)
        {
            return false;
        }
        int num3 = 0;
        int width = 57;
        int height = 85;
        if (name.StartsWith("mw") || name.StartsWith("ww") || name.StartsWith("mc") || name.StartsWith("wc") || name.StartsWith("mg") || name.StartsWith("wg"))
        {
            EPFImage value = EPFImage.FromArchive(name.Remove(5) + "04.epf", ignoreCase: true, archive);
            if (!_epfImages.ContainsKey(name.Remove(5) + "04.epf"))
            {
                _epfImages.Add(name.Remove(5) + "04.epf", value);
            }
            width = 111;
        }
        string text = "";
        if (source == "new")
        {
            text = "_new";
        }
        if (source == "myda")
        {
            text = "_myda";
        }
        string text2 = "";
        if (bloody)
        {
            text2 = "_B";
        }
        string text3 = "epf, Width: " + ePFImage.Width + ", Height: " + ePFImage.Height + "\n";
        EPFFrame[] frames = ePFImage.Frames;
        foreach (EPFFrame ePFFrame in frames)
        {
            Size canvas = new Size(width, height);
            Image image = DAGraphics.RenderImage(ePFFrame, palette, canvas, defPurple, newColor, bloody, cstmColorID);
            LoadTexture(name.Remove(name.IndexOf(".epf")) + "_F" + num3 + text2 + text + "_C" + color, image);
            string text4 = text3;
            text3 = text4 + "frame " + num3 + ", Width: " + ePFFrame.Width + ", Height: " + ePFFrame.Height + " dataL: " + ePFFrame.RawData.Length + "\n";
            num3++;
        }
        return true;
    }

    public bool LoadEPFTexture(string name, int color = 0, string source = "old", MPFImage mImage = null, int? theframe = null)
    {
        DATArchive archive = oldDat;
        SixColors defPurple = _dyeColors.Get(0);
        SixColors newColor = _dyeColors.Get(color);
        int cstmColorID = 0;
        if (name.Contains(".hpf"))
        {
            int num = int.Parse(name.Substring(3, 5));
            Palette256 palette = oldPal;
            HPFImage hPFImage = HPFImage.FromArchive("stc" + num.ToString("00000") + ".hpf", oldDat);
            if (hPFImage == null || source == "new")
            {
                palette = wallTable[num + 1];
                hPFImage = HPFImage.FromArchive("stc" + num.ToString("00000") + ".hpf", iaDat);
            }
            Image image = DAGraphics.SimpleRender(hPFImage.RawData, palette, hPFImage.Width, hPFImage.Height);
            LoadTexture(name.Remove(name.IndexOf(".hpf")) + ".hpf_F0_C0", image);
            return true;
        }
        if (name.Contains(".tile"))
        {
            int num2 = int.Parse(name.Substring(3, 5));
            LoadTexture(image: (num2 > 0 && source != "new" && num2 < 6100) ? ((num2 - 1 > tilesb.TileCount) ? DAGraphics.SimpleRender(tilesa.Tiles[num2 - 1], tileTable[num2 + 2], 56, 27) : DAGraphics.SimpleRender(tilesb.Tiles[num2 - 1], oldPal, 56, 27)) : ((!(source == "new") && num2 < 6100) ? DAGraphics.SimpleRender(tilesb.Tiles[num2], oldPal, 56, 27) : DAGraphics.SimpleRender(tilesa.Tiles[num2 - 1], tileTable[num2 + 2], 56, 27)), textureId: name.Remove(name.IndexOf(".tile")) + "_F0_C0");
            return true;
        }
        if (name.Contains(".mpf"))
        {
            if (mImage == null)
            {
                return false;
            }
            string text = "";
            if (source == "new")
            {
                text = "_new";
            }
            else if (source == "myda")
            {
                text = "_myda";
            }
            Palette256 palette2 = Palette256.FromArchive(mImage.Palette, hadesDat);
            if (mImage.Name == "MNS581.mpf")
            {
                palette2 = oldPal;
            }
            int num3 = 0;
            MPFFrame[] frames = mImage.Frames;
            foreach (MPFFrame mpf in frames)
            {
                Image image3 = DAGraphics.RenderImage(mpf, palette2, new Size(mImage.Width, mImage.Height));
                LoadTexture(name.Remove(name.IndexOf(".mpf")) + "_F" + num3 + text + "_C0", image3);
                num3++;
            }
            return true;
        }
        if (name.Contains(".epf"))
        {
            if (name.Contains("efct"))
            {
                archive = rohDat;
                int num4 = int.Parse(name.Substring(4, 3));
                if (num4 >= 231 && num4 <= 309)
                {
                    name = name.Replace(".epf", ".efa");
                    return false;
                }
            }
            if (name.StartsWith("item0") && source == "new")
            {
                archive = newDat;
            }
            else if (name.StartsWith("item0") && source == "myda")
            {
                archive = mydaDat;
            }
            else if (name.StartsWith("emot01") && theframe >= 11)
            {
                archive = newDat;
            }
            if (name.StartsWith("mask"))
            {
                archive = newDat;
            }
            if ((name.StartsWith("skill0") || name.StartsWith("spell0")) && source == "new")
            {
                archive = setoaDat;
            }
            if (name.StartsWith("mefc"))
            {
                archive = rohDat;
            }
            if (name.StartsWith("nation"))
            {
                archive = mydaDat;
            }
            EPFImage ePFImage = EPFImage.FromArchive(name, ignoreCase: true, archive);
            if (ePFImage == null)
            {
                return false;
            }
            int num5 = 0;
            EPFFrame[] frames2 = ePFImage.Frames;
            foreach (EPFFrame epf in frames2)
            {
                if ((!name.StartsWith("item0") && !name.StartsWith("emot01")) || !theframe.HasValue || num5 == theframe)
                {
                    if (name.StartsWith("item0"))
                    {
                        int num6 = int.Parse(name.Substring(4, 3));
                        if ((num6 == 1 && num5 == 105) || (num6 == 2 && (num5 == 140 || num5 == 141 || num5 == 142 || num5 == 143)))
                        {
                            cstmColorID = 11;
                            defPurple = _dyeColors.GetCustom(11);
                        }
                    }
                    Image image4;
                    if (name == "staff.epf")
                    {
                        image4 = DAGraphics.RenderImage(epf, Palette256.FromArchive("staff.pal", archive), new Size(ePFImage.Width, ePFImage.Height));
                    }
                    else if (name == "field001.epf")
                    {
                        image4 = DAGraphics.RenderImage(epf, Palette256.FromArchive("field001.pal", archive), new Size(ePFImage.Width, ePFImage.Height));
                    }
                    else if (name.StartsWith("efct"))
                    {
                        int index = int.Parse(name.Substring(4, 3));
                        Palette256 palette3 = effPalTable[index];
                        if (palette3 == null)
                        {
                            palette3 = oldPal;
                        }
                        image4 = DAGraphics.RenderImage(epf, palette3, new Size(ePFImage.Width, ePFImage.Height));
                    }
                    else if (name.StartsWith("item0") && source == "new")
                    {
                        Palette256 palette4 = Palette256.FromArchive("item000.pal", newDat);
                        if (name.StartsWith("item010"))
                        {
                            palette4 = Palette256.FromArchive("item001.pal", newDat);
                        }
                        else if (name.StartsWith("item011"))
                        {
                            palette4 = Palette256.FromArchive("item002.pal", newDat);
                        }
                        else if (name.StartsWith("item012"))
                        {
                            palette4 = Palette256.FromArchive("item003.pal", newDat);
                        }
                        else if (name.StartsWith("item014"))
                        {
                            palette4 = Palette256.FromArchive("item004.pal", newDat);
                        }
                        else if (name.StartsWith("item015"))
                        {
                            palette4 = Palette256.FromArchive("item001.pal", newDat);
                        }
                        else if (name.StartsWith("item016"))
                        {
                            palette4 = Palette256.FromArchive("item006.pal", newDat);
                        }
                        else if (name.StartsWith("item017"))
                        {
                            palette4 = Palette256.FromArchive("item007.pal", newDat);
                        }
                        else if (name.StartsWith("item018"))
                        {
                            palette4 = Palette256.FromArchive("item008.pal", newDat);
                        }
                        else if (name.StartsWith("item019"))
                        {
                            palette4 = Palette256.FromArchive("item009.pal", newDat);
                        }
                        else if (name.StartsWith("item020"))
                        {
                            palette4 = Palette256.FromArchive("item010.pal", newDat);
                        }
                        else if (name.StartsWith("item021"))
                        {
                            palette4 = Palette256.FromArchive("item012.pal", newDat);
                        }
                        else if (name.StartsWith("item022"))
                        {
                            palette4 = Palette256.FromArchive("item011.pal", newDat);
                        }
                        else if (name.StartsWith("item024"))
                        {
                            palette4 = Palette256.FromArchive("item013.pal", newDat);
                        }
                        else if (name.StartsWith("item025"))
                        {
                            palette4 = Palette256.FromArchive("item014.pal", newDat);
                        }
                        else if (name.StartsWith("item026"))
                        {
                            palette4 = Palette256.FromArchive("item015.pal", newDat);
                        }
                        else if (name.StartsWith("item027"))
                        {
                            palette4 = Palette256.FromArchive("item016.pal", newDat);
                        }
                        else if (name.StartsWith("item030"))
                        {
                            palette4 = Palette256.FromArchive("item017.pal", newDat);
                        }
                        else if (name.StartsWith("item032"))
                        {
                            palette4 = Palette256.FromArchive("item018.pal", newDat);
                        }
                        else if (name.StartsWith("item033"))
                        {
                            palette4 = Palette256.FromArchive("item019.pal", newDat);
                        }
                        else if (name.StartsWith("item034"))
                        {
                            palette4 = Palette256.FromArchive("item020.pal", newDat);
                        }
                        else if (name.StartsWith("item035"))
                        {
                            palette4 = Palette256.FromArchive("item021.pal", newDat);
                        }
                        else if (name.StartsWith("item036"))
                        {
                            palette4 = Palette256.FromArchive("item022.pal", newDat);
                        }
                        else if (name.StartsWith("item037"))
                        {
                            palette4 = Palette256.FromArchive("item023.pal", newDat);
                        }
                        else if (name.StartsWith("item038"))
                        {
                            palette4 = Palette256.FromArchive("item024.pal", newDat);
                        }
                        else if (name.StartsWith("item039"))
                        {
                            palette4 = Palette256.FromArchive("item025.pal", newDat);
                        }
                        else if (name.StartsWith("item040"))
                        {
                            palette4 = Palette256.FromArchive("item026.pal", newDat);
                        }
                        else if (name.StartsWith("item042"))
                        {
                            palette4 = Palette256.FromArchive("item027.pal", newDat);
                        }
                        else if (name.StartsWith("item043"))
                        {
                            palette4 = Palette256.FromArchive("item028.pal", newDat);
                        }
                        else if (name.StartsWith("item044"))
                        {
                            palette4 = Palette256.FromArchive("item030.pal", newDat);
                        }
                        else if (name.StartsWith("item045"))
                        {
                            palette4 = Palette256.FromArchive("item029.pal", newDat);
                        }
                        else if (name.StartsWith("item047"))
                        {
                            palette4 = Palette256.FromArchive("item031.pal", newDat);
                        }
                        else if (name.StartsWith("item048"))
                        {
                            palette4 = Palette256.FromArchive("item032.pal", newDat);
                        }
                        else if (name.StartsWith("item049"))
                        {
                            palette4 = Palette256.FromArchive("item033.pal", newDat);
                        }
                        else if (name.StartsWith("item053"))
                        {
                            palette4 = Palette256.FromArchive("item034.pal", newDat);
                        }
                        image4 = ((!name.StartsWith("item054")) ? DAGraphics.RenderImage(epf, palette4, new Size(ePFImage.Width, ePFImage.Height), defPurple, newColor, bloody: false, cstmColorID) : DAGraphics.RenderImage(epf, palette4, new Size(33, 33), defPurple, newColor, bloody: false, cstmColorID));
                    }
                    else if (name.StartsWith("emot01"))
                    {
                        if (num5 >= 11)
                        {
                            Palette256 palette5 = Palette256.FromArchive("palm000.pal", khanpalDat);
                            image4 = DAGraphics.RenderImage(epf, palette5, new Size(57, 85), defPurple, newColor, bloody: false, cstmColorID);
                        }
                        else
                        {
                            image4 = DAGraphics.RenderImage(epf, oldPal, new Size(ePFImage.Width, ePFImage.Height), defPurple, newColor, bloody: false, cstmColorID);
                        }
                    }
                    else if ((name.StartsWith("skill0") || name.StartsWith("spell0")) && source == "new")
                    {
                        Palette256 palette6 = Palette256.FromArchive("gui06.pal", archive);
                        image4 = DAGraphics.RenderImage(epf, palette6, new Size(ePFImage.Width, ePFImage.Height), defPurple, newColor);
                    }
                    else
                    {
                        image4 = DAGraphics.RenderImage(epf, oldPal, new Size(ePFImage.Width, ePFImage.Height), defPurple, newColor, bloody: false, cstmColorID);
                    }
                    string text2 = "";
                    if (source == "new")
                    {
                        text2 = "_new";
                    }
                    if (source == "myda")
                    {
                        text2 = "_myda";
                    }
                    LoadTexture(name.Remove(name.IndexOf(".epf")) + "_F" + num5 + text2 + "_C" + color, image4);
                }
                num5++;
            }
            return true;
        }
        return false;
    }

    public void LoadAllEPFS()
    {
        List<string> list = new List<string>();
        list.Clear();
        for (int i = 1; i <= 6; i++)
        {
            list.Add("item" + i.ToString("000") + ".epf");
        }
        foreach (string item in list)
        {
            EPFImage ePFImage = EPFImage.FromArchive(item, ignoreCase: true, oldDat);
            if (ePFImage == null)
            {
                continue;
            }
            int num = 0;
            EPFFrame[] frames = ePFImage.Frames;
            foreach (EPFFrame epf in frames)
            {
                Image image = DAGraphics.RenderImage(epf, oldPal, new Size(ePFImage.Width, ePFImage.Height));
                if (image != null)
                {
                    LoadTexture(item.Remove(item.IndexOf(".epf")) + "_F" + num + "_C0", image);
                }
                num++;
            }
        }
        list.Clear();
        for (int k = 1; k <= 28; k++)
        {
            list.Add("item" + k.ToString("000") + ".epf");
        }
        foreach (string item2 in list)
        {
            EPFImage ePFImage2 = EPFImage.FromArchive(item2, ignoreCase: true, mydaDat);
            if (ePFImage2 == null)
            {
                continue;
            }
            int num2 = 0;
            EPFFrame[] frames2 = ePFImage2.Frames;
            foreach (EPFFrame epf2 in frames2)
            {
                Image image2 = DAGraphics.RenderImage(epf2, oldPal, new Size(ePFImage2.Width, ePFImage2.Height));
                if (image2 != null)
                {
                    LoadTexture(item2.Remove(item2.IndexOf(".epf")) + "_F" + num2 + "_myda_C0", image2);
                }
                num2++;
            }
        }
        list.Clear();
        for (int m = 1; m <= 24; m++)
        {
            list.Add("item" + m.ToString("000") + ".epf");
        }
        for (int n = 26; n <= 54; n++)
        {
            list.Add("item" + n.ToString("000") + ".epf");
        }
        foreach (string item3 in list)
        {
            EPFImage ePFImage3 = EPFImage.FromArchive(item3, ignoreCase: true, newDat);
            if (ePFImage3 == null)
            {
                continue;
            }
            int num3 = 0;
            Palette256 palette = Palette256.FromArchive("item000.pal", newDat);
            if (item3.StartsWith("item010"))
            {
                palette = Palette256.FromArchive("item001.pal", newDat);
            }
            else if (item3.StartsWith("item011"))
            {
                palette = Palette256.FromArchive("item002.pal", newDat);
            }
            else if (item3.StartsWith("item012"))
            {
                palette = Palette256.FromArchive("item003.pal", newDat);
            }
            else if (item3.StartsWith("item014"))
            {
                palette = Palette256.FromArchive("item004.pal", newDat);
            }
            else if (item3.StartsWith("item015"))
            {
                palette = Palette256.FromArchive("item001.pal", newDat);
            }
            else if (item3.StartsWith("item016"))
            {
                palette = Palette256.FromArchive("item006.pal", newDat);
            }
            else if (item3.StartsWith("item017"))
            {
                palette = Palette256.FromArchive("item007.pal", newDat);
            }
            else if (item3.StartsWith("item018"))
            {
                palette = Palette256.FromArchive("item008.pal", newDat);
            }
            else if (item3.StartsWith("item019"))
            {
                palette = Palette256.FromArchive("item009.pal", newDat);
            }
            else if (item3.StartsWith("item020"))
            {
                palette = Palette256.FromArchive("item010.pal", newDat);
            }
            else if (item3.StartsWith("item021"))
            {
                palette = Palette256.FromArchive("item012.pal", newDat);
            }
            else if (item3.StartsWith("item022"))
            {
                palette = Palette256.FromArchive("item011.pal", newDat);
            }
            else if (item3.StartsWith("item024"))
            {
                palette = Palette256.FromArchive("item013.pal", newDat);
            }
            else if (item3.StartsWith("item025"))
            {
                palette = Palette256.FromArchive("item014.pal", newDat);
            }
            else if (item3.StartsWith("item026"))
            {
                palette = Palette256.FromArchive("item015.pal", newDat);
            }
            else if (item3.StartsWith("item027"))
            {
                palette = Palette256.FromArchive("item016.pal", newDat);
            }
            else if (item3.StartsWith("item030"))
            {
                palette = Palette256.FromArchive("item017.pal", newDat);
            }
            else if (item3.StartsWith("item032"))
            {
                palette = Palette256.FromArchive("item018.pal", newDat);
            }
            else if (item3.StartsWith("item033"))
            {
                palette = Palette256.FromArchive("item019.pal", newDat);
            }
            else if (item3.StartsWith("item034"))
            {
                palette = Palette256.FromArchive("item020.pal", newDat);
            }
            else if (item3.StartsWith("item035"))
            {
                palette = Palette256.FromArchive("item021.pal", newDat);
            }
            else if (item3.StartsWith("item036"))
            {
                palette = Palette256.FromArchive("item022.pal", newDat);
            }
            else if (item3.StartsWith("item037"))
            {
                palette = Palette256.FromArchive("item023.pal", newDat);
            }
            else if (item3.StartsWith("item038"))
            {
                palette = Palette256.FromArchive("item024.pal", newDat);
            }
            else if (item3.StartsWith("item039"))
            {
                palette = Palette256.FromArchive("item025.pal", newDat);
            }
            else if (item3.StartsWith("item040"))
            {
                palette = Palette256.FromArchive("item026.pal", newDat);
            }
            else if (item3.StartsWith("item042"))
            {
                palette = Palette256.FromArchive("item027.pal", newDat);
            }
            else if (item3.StartsWith("item043"))
            {
                palette = Palette256.FromArchive("item028.pal", newDat);
            }
            else if (item3.StartsWith("item044"))
            {
                palette = Palette256.FromArchive("item030.pal", newDat);
            }
            else if (item3.StartsWith("item045"))
            {
                palette = Palette256.FromArchive("item029.pal", newDat);
            }
            else if (item3.StartsWith("item047"))
            {
                palette = Palette256.FromArchive("item031.pal", newDat);
            }
            else if (item3.StartsWith("item048"))
            {
                palette = Palette256.FromArchive("item032.pal", newDat);
            }
            else if (item3.StartsWith("item049"))
            {
                palette = Palette256.FromArchive("item033.pal", newDat);
            }
            else if (item3.StartsWith("item053"))
            {
                palette = Palette256.FromArchive("item034.pal", newDat);
            }
            EPFFrame[] frames3 = ePFImage3.Frames;
            foreach (EPFFrame epf3 in frames3)
            {
                Image image3 = null;
                image3 = ((!item3.StartsWith("item054")) ? DAGraphics.RenderImage(epf3, palette, new Size(ePFImage3.Width, ePFImage3.Height)) : DAGraphics.RenderImage(epf3, palette, new Size(33, 33)));
                if (image3 != null)
                {
                    LoadTexture(item3.Remove(item3.IndexOf(".epf")) + "_F" + num3 + "_new_C0", image3);
                }
                num3++;
            }
        }
    }

    public void LoadAllHPFS()
    {
        List<string> list = new List<string>();
        list.Clear();
        for (int i = 0; i < 6100; i++)
        {
            list.Add("stc" + i.ToString("00000") + ".tile");
        }
        foreach (string item in list)
        {
            int num = int.Parse(item.Substring(3, 5));
            Image image = ((num <= 0 || num >= 6100) ? DAGraphics.SimpleRender(tilesb.Tiles[num], oldPal, 56, 27) : ((num - 1 > tilesb.TileCount) ? DAGraphics.SimpleRender(tilesa.Tiles[num - 1], tileTable[num + 2], 56, 27) : DAGraphics.SimpleRender(tilesb.Tiles[num - 1], oldPal, 56, 27)));
            if (image != null)
            {
                _loadedHPFS.Add(item.Remove(item.IndexOf(".tile")) + "_F0_C0", image);
            }
        }
        list.Clear();
        for (int j = 0; j <= 7248; j++)
        {
            list.Add("stc" + j.ToString("00000") + ".hpf");
        }
        foreach (string item2 in list)
        {
            HPFImage hPFImage = HPFImage.FromArchive(item2, oldDat);
            if (hPFImage != null)
            {
                Image image2 = null;
                image2 = DAGraphics.SimpleRender(hPFImage.RawData, oldPal, hPFImage.Width, hPFImage.Height);
                if (image2 != null)
                {
                    _loadedHPFS.Add(item2 + "_F0_C0", image2);
                }
            }
        }
    }

    public void LoadAllMPFS()
    {
        List<string> list = new List<string>();
        list.Clear();
        for (int i = 1; i <= 328; i++)
        {
            list.Add("mns" + i.ToString("000") + ".mpf");
        }
        foreach (string item in list)
        {
            MPFImage mPFImage = MPFImage.FromArchive(item, ignoreCase: true, oldDat);
            if (mPFImage != null)
            {
                int num = 0;
                MPFFrame[] frames = mPFImage.Frames;
                foreach (MPFFrame mpf in frames)
                {
                    Image image = DAGraphics.RenderImage(mpf, oldPal, new Size(mPFImage.Width, mPFImage.Height));
                    LoadTexture(item.Remove(item.IndexOf(".mpf")) + "_F" + num + "_C0", image);
                    num++;
                }
            }
        }
    }

    public void Dispose()
    {
        foreach (Texture value in _textureDatabase.Values)
        {
            Gl.glDeleteTextures(1, new int[1] { value.Id });
        }
    }

    public void LoadTexture(string textureId, string path)
    {
        int textures = 0;
        Gl.glGenTextures(1, out textures);
        Gl.glBindTexture(3553, textures);
        Bitmap bitmap = new Bitmap(path);
        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        Gl.glTexImage2D(3553, 0, 6408, bitmapData.Width, bitmapData.Height, 0, 32993, 5121, bitmapData.Scan0);
        bitmap.UnlockBits(bitmapData);
        Gl.glTexParameteri(3553, 10241, 9728);
        Gl.glTexParameteri(3553, 10240, 9728);
        _textureDatabase.GetOrAdd(textureId, new Texture(textures, bitmap.Width, bitmap.Height));
        bitmap.Dispose();
    }

    public void LoadTexture(string textureId, Image image)
    {
        if (image == null)
        {
            _textureDatabase.GetOrAdd(textureId, default(Texture));
        }
        else if (!_textureDatabase.ContainsKey(textureId))
        {
            int textures = 0;
            Gl.glGenTextures(1, out textures);
            Gl.glBindTexture(3553, textures);
            Bitmap bitmap = (Bitmap)image;
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Gl.glTexImage2D(3553, 0, 6408, bitmapData.Width, bitmapData.Height, 0, 32993, 5121, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);
            Gl.glTexParameteri(3553, 10241, 9728);
            Gl.glTexParameteri(3553, 10240, 9728);
            if (!_textureDatabase.ContainsKey(textureId))
            {
                _textureDatabase.GetOrAdd(textureId, new Texture(textures, bitmap.Width, bitmap.Height));
            }
            bitmap.Dispose();
        }
    }
}
