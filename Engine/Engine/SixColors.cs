using System.Drawing;

namespace Engine;

public class SixColors
{
    public System.Drawing.Color color1 = default(System.Drawing.Color);

    public System.Drawing.Color color2 = default(System.Drawing.Color);

    public System.Drawing.Color color3 = default(System.Drawing.Color);

    public System.Drawing.Color color4 = default(System.Drawing.Color);

    public System.Drawing.Color color5 = default(System.Drawing.Color);

    public System.Drawing.Color color6 = default(System.Drawing.Color);

    public int ID { get; set; }

    public SixColors(int id)
    {
        ID = id;
    }
}
