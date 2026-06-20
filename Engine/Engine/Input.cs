namespace Engine;

public class Input
{
    public Mouse Mouse { get; set; }

    public Keyboard Keyboard { get; set; }

    public void Update(double elapsedTime)
    {
        Mouse.Update(elapsedTime);
        Keyboard.Process();
    }
}
