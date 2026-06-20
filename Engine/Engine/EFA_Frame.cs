using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Engine;

public sealed class EFA_Frame
{
    private EFA_Frame_Header m_header;

    private Bitmap m_bitmap;

    public Bitmap FrameBitmap => m_bitmap;

    public int Size => (int)m_header.Size;

    public int ByteCount => (int)m_header.ByteCount;

    public int PixelWidth => (int)m_header.Width / 2;

    public int PixelHeight => ByteCount / PixelWidth;

    public EFA_Frame(EFA_Frame_Header h)
    {
        m_header = h;
        m_bitmap = new Bitmap(PixelWidth, PixelHeight, PixelFormat.Format16bppRgb565);
    }

    public void Render(byte[] rawBits)
    {
        BitmapData bitmapData = FrameBitmap.LockBits(new Rectangle(0, 0, PixelWidth, PixelHeight), ImageLockMode.ReadWrite, FrameBitmap.PixelFormat);
        IntPtr scan = bitmapData.Scan0;
        Marshal.Copy(rawBits, 0, scan, ByteCount);
        FrameBitmap.UnlockBits(bitmapData);
    }
}
