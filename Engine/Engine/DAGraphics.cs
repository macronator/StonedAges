using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Engine;

public class DAGraphics
{
    public static Bitmap RenderImage(EPFFrame epf, Palette256 palette, Size canvas, SixColors defPurple = null, SixColors newColor = null, bool bloody = false, int cstmColorID = 0)
    {
        if (epf.Width > 0 && epf.Height > 0)
        {
            return FrameRender(epf.RawData, palette, ImageType.EPF, canvas, new Rectangle(epf.Left, epf.Top, epf.Width, epf.Height), defPurple, newColor, bloody, cstmColorID);
        }
        return new Bitmap(30, 30);
    }

    public static Bitmap RenderImage(MPFFrame mpf, Palette256 palette, Size canvas)
    {
        return FrameRender(mpf.RawData, palette, ImageType.MPF, canvas, new Rectangle(mpf.Left, mpf.Top, mpf.Width, mpf.Height));
    }

    public unsafe static Bitmap SimpleRender(byte[] data, Palette256 palette, int width, int height)
    {
        try
        {
            Bitmap bitmap = new Bitmap(width, height);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Parallel.For(0, bitmapdata.Height, delegate (int index1)
            {
                byte* ptr = (byte*)(void*)((IntPtr)(void*)bitmapdata.Scan0 + index1 * bitmapdata.Stride);
                for (int i = 0; i < bitmapdata.Width; i++)
                {
                    int num = data[index1 * width + i];
                    if (num > 0)
                    {
                        ptr[(nint)i * (nint)4] = palette[num].B;
                        ptr[i * 4 + 1] = palette[num].G;
                        ptr[i * 4 + 2] = palette[num].R;
                        ptr[i * 4 + 3] = palette[num].A;
                    }
                }
            });
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }
        catch (Exception)
        {
            Console.WriteLine("SimpleRender threw an exception.");
            return null;
        }
    }

    public unsafe static Bitmap FrameRender(byte[] data, Palette256 palette, ImageType type, Size canvas, Rectangle rectFrame, SixColors defPurple = null, SixColors newColor = null, bool bloody = false, int cstmColorID = 0)
    {
        try
        {
            if (canvas.Width <= 0 || canvas.Height <= 0)
            {
                return null;
            }
            Bitmap bitmap = new Bitmap(canvas.Width, canvas.Height);
            if (rectFrame.X + rectFrame.Width > canvas.Width || rectFrame.Y + rectFrame.Height > canvas.Height)
            {
                rectFrame = Rectangle.FromLTRB(0, 0, rectFrame.Width, rectFrame.Height);
            }
            BitmapData bitmapdata = bitmap.LockBits(rectFrame, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Parallel.For(0, rectFrame.Height, delegate (int index1)
            {
                byte* ptr = (byte*)(void*)((IntPtr)(void*)bitmapdata.Scan0 + index1 * bitmapdata.Stride);
                for (int i = 0; i < rectFrame.Width; i++)
                {
                    int num = data[index1 * rectFrame.Width + i];
                    if (num > 0)
                    {
                        if (bitmapdata.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            if (defPurple != null && newColor != null)
                            {
                                if (cstmColorID == 11)
                                {
                                    if (palette[num] == defPurple.color1)
                                    {
                                        ptr[(nint)i * (nint)4] = newColor.color1.B;
                                        ptr[i * 4 + 1] = newColor.color1.G;
                                        ptr[i * 4 + 2] = newColor.color1.R;
                                        ptr[i * 4 + 3] = newColor.color1.A;
                                    }
                                    else if (palette[num] == defPurple.color2)
                                    {
                                        ptr[(nint)i * (nint)4] = newColor.color2.B;
                                        ptr[i * 4 + 1] = newColor.color2.G;
                                        ptr[i * 4 + 2] = newColor.color2.R;
                                        ptr[i * 4 + 3] = newColor.color2.A;
                                    }
                                    else if (palette[num] == defPurple.color3)
                                    {
                                        ptr[(nint)i * (nint)4] = newColor.color3.B;
                                        ptr[i * 4 + 1] = newColor.color3.G;
                                        ptr[i * 4 + 2] = newColor.color3.R;
                                        ptr[i * 4 + 3] = newColor.color3.A;
                                    }
                                    else if (palette[num] == defPurple.color4)
                                    {
                                        ptr[(nint)i * (nint)4] = newColor.color3.B;
                                        ptr[i * 4 + 1] = newColor.color3.G;
                                        ptr[i * 4 + 2] = newColor.color3.R;
                                        ptr[i * 4 + 3] = newColor.color3.A;
                                    }
                                    else if (palette[num] == defPurple.color5)
                                    {
                                        ptr[(nint)i * (nint)4] = newColor.color4.B;
                                        ptr[i * 4 + 1] = newColor.color4.G;
                                        ptr[i * 4 + 2] = newColor.color4.R;
                                        ptr[i * 4 + 3] = newColor.color4.A;
                                    }
                                    else
                                    {
                                        ptr[(nint)i * (nint)4] = palette[num].B;
                                        ptr[i * 4 + 1] = palette[num].G;
                                        ptr[i * 4 + 2] = palette[num].R;
                                        ptr[i * 4 + 3] = palette[num].A;
                                    }
                                }
                                else if (palette[num] == defPurple.color1)
                                {
                                    ptr[(nint)i * (nint)4] = newColor.color1.B;
                                    ptr[i * 4 + 1] = newColor.color1.G;
                                    ptr[i * 4 + 2] = newColor.color1.R;
                                    ptr[i * 4 + 3] = newColor.color1.A;
                                }
                                else if (palette[num] == defPurple.color2)
                                {
                                    ptr[(nint)i * (nint)4] = newColor.color2.B;
                                    ptr[i * 4 + 1] = newColor.color2.G;
                                    ptr[i * 4 + 2] = newColor.color2.R;
                                    ptr[i * 4 + 3] = newColor.color2.A;
                                }
                                else if (palette[num] == defPurple.color3)
                                {
                                    ptr[(nint)i * (nint)4] = newColor.color3.B;
                                    ptr[i * 4 + 1] = newColor.color3.G;
                                    ptr[i * 4 + 2] = newColor.color3.R;
                                    ptr[i * 4 + 3] = newColor.color3.A;
                                }
                                else if (palette[num] == defPurple.color4)
                                {
                                    ptr[(nint)i * (nint)4] = newColor.color4.B;
                                    ptr[i * 4 + 1] = newColor.color4.G;
                                    ptr[i * 4 + 2] = newColor.color4.R;
                                    ptr[i * 4 + 3] = newColor.color4.A;
                                }
                                else if (palette[num] == defPurple.color5)
                                {
                                    ptr[(nint)i * (nint)4] = newColor.color5.B;
                                    ptr[i * 4 + 1] = newColor.color5.G;
                                    ptr[i * 4 + 2] = newColor.color5.R;
                                    ptr[i * 4 + 3] = newColor.color5.A;
                                }
                                else if (palette[num] == defPurple.color6)
                                {
                                    ptr[(nint)i * (nint)4] = newColor.color6.B;
                                    ptr[i * 4 + 1] = newColor.color6.G;
                                    ptr[i * 4 + 2] = newColor.color6.R;
                                    ptr[i * 4 + 3] = newColor.color6.A;
                                }
                                else
                                {
                                    ptr[(nint)i * (nint)4] = palette[num].B;
                                    ptr[i * 4 + 1] = palette[num].G;
                                    ptr[i * 4 + 2] = palette[num].R;
                                    ptr[i * 4 + 3] = palette[num].A;
                                }
                                if (!bloody && palette[num].R == byte.MaxValue && palette[num].G == 0 && palette[num].B == 0)
                                {
                                    ptr[(nint)i * (nint)4] = byte.MaxValue;
                                    ptr[i * 4 + 1] = byte.MaxValue;
                                    ptr[i * 4 + 2] = byte.MaxValue;
                                    ptr[i * 4 + 3] = palette[num].A;
                                }
                            }
                            else
                            {
                                ptr[(nint)i * (nint)4] = palette[num].B;
                                ptr[i * 4 + 1] = palette[num].G;
                                ptr[i * 4 + 2] = palette[num].R;
                                ptr[i * 4 + 3] = palette[num].A;
                            }
                        }
                        else if (bitmapdata.PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            ptr[(nint)i * (nint)3] = palette[num].B;
                            ptr[i * 3 + 1] = palette[num].G;
                            ptr[i * 3 + 2] = palette[num].R;
                        }
                    }
                }
            });
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }
        catch (ArgumentException)
        {
            if (rectFrame.Left == 0 && rectFrame.Top == 0)
            {
                return null;
            }
            Console.WriteLine("FrameRender threw an exception.");
            return null;
        }
    }
}
