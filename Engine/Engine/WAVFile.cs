namespace Engine;

public class WAVFile
{
    public static byte[] FromArchive(string file, DATArchive archive)
    {
        if (!archive.Contains(file))
        {
            return null;
        }
        return archive.ExtractFile(file);
    }
}
