using System;
using System.Collections.Generic;
using System.IO;
using IrrKlang;

namespace Engine;

public class SoundManager : IDisposable
{
    public ISoundEngine sengine;

    public Dictionary<string, ISoundSource> _soundIdentifier = new Dictionary<string, ISoundSource>();

    public void Dispose()
    {
        foreach (ISoundSource value in _soundIdentifier.Values)
        {
            value.Dispose();
        }
        if (sengine != null)
        {
            sengine.Dispose();
        }
    }

    public SoundManager()
    {
        sengine = new ISoundEngine();
    }

    public void LoadSound(string soundId, string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        byte[] array = new byte[fileStream.Length];
        try
        {
            fileStream.Read(array, 0, Convert.ToInt32(fileStream.Length));
            fileStream.Close();
        }
        catch
        {
            fileStream.Close();
        }
        LoadSound(soundId, path, array);
    }

    public void LoadSound(string soundId, string name, byte[] dataArr)
    {
        if (sengine != null)
        {
            ISoundSource value = sengine.AddSoundSourceFromMemory(dataArr, soundId);
            _soundIdentifier.Add(soundId, value);
        }
    }
}
