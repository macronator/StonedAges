namespace Engine;

public class Sound
{
    public int Channel { get; set; }

    public bool FailedToPlay => Channel == -1;

    public Sound(int channel)
    {
        Channel = channel;
    }
}
