using System.Drawing;

namespace Engine;

public struct Color
{
    public float R { get; set; }

    public float G { get; set; }

    public float B { get; set; }

    public float A { get; set; }

    public static Color BrightGreen => Text.Colors(0, 252, 0);

    public static Color Orange => Text.Colors(240, 140, 24);

    public static Color Invisible => Text.Colors(248, 252, 248, 0);

    public static Color SaddleBrown => Text.Colors(160, 64, 0);

    public static Color DarkOrange => Text.Colors(232, 108, 8);

    public static Color LightBlue => Text.Colors(120, 164, 240);

    public static Color Green => Text.Colors(0, 96, 0);

    public static Color Yellow => Text.Colors(248, 228, 56);

    public static Color GuildGreen => Text.Colors(112, 180, 168);

    public static Color Gray2 => Text.Colors(184, 184, 184);

    public static Color White => Text.Colors(248, 252, 248);

    public static Color Black => Text.Colors(0, 0, 8);

    public static Color Red => Text.Colors(200, 0, 16);

    public static Color Purple => Text.Colors(System.Drawing.Color.Purple);

    public Color(float r, float g, float b, float a)
    {
        this = default(Color);
        R = r;
        G = g;
        B = b;
        A = a;
    }
}
