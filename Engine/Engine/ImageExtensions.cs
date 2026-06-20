using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Engine;

public static class ImageExtensions
{
    public static byte[] ToByteArray(this Image image, ImageFormat format)
    {
        using MemoryStream memoryStream = new MemoryStream();
        image.Save(memoryStream, format);
        return memoryStream.ToArray();
    }

    public static byte[] ConvertImageBytes(byte[] imageBytes, ImageFormat imageFormat)
    {
        byte[] array = new byte[0];
        FileStream fileStream = new FileStream("empty." + imageFormat, FileMode.Create);
        using MemoryStream memoryStream = new MemoryStream(imageBytes);
        fileStream.Write(array, 0, array.Length);
        byte[] array2 = new byte[16384];
        int count;
        while ((count = fileStream.Read(array2, 0, array2.Length)) > 0)
        {
            memoryStream.Write(array2, 0, count);
        }
        array = memoryStream.ToArray();
        fileStream.Close();
        memoryStream.Close();
        return array;
    }
}
