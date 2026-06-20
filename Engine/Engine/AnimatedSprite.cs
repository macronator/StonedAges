using System.Drawing;

namespace Engine;

public class AnimatedSprite : Sprite
{
    private int _framesX;

    private int _framesY;

    private int _currentFrame;

    private double _currentFrameTime = 0.03;

    public double Speed { get; set; }

    public bool Looping { get; set; }

    public bool Finished { get; set; }

    public AnimatedSprite()
    {
        Looping = false;
        Finished = false;
        Speed = 0.03;
        _currentFrameTime = Speed;
    }

    public System.Drawing.Point GetIndexFromFrame(int frame)
    {
        System.Drawing.Point result = default(System.Drawing.Point);
        result.Y = frame / _framesX;
        result.X = frame - result.Y * _framesY;
        return result;
    }

    private void UpdateUVs()
    {
        System.Drawing.Point indexFromFrame = GetIndexFromFrame(_currentFrame);
        float num = 1f / (float)_framesX;
        float num2 = 1f / (float)_framesY;
        SetUVs(new Point((float)indexFromFrame.X * num, (float)indexFromFrame.Y * num2), new Point((float)(indexFromFrame.X + 1) * num, (float)(indexFromFrame.Y + 1) * num2));
    }

    public void SetAnimation(int framesX, int framesY)
    {
        _framesX = framesX;
        _framesY = framesY;
        UpdateUVs();
    }

    private int GetFrameCount()
    {
        return _framesX * _framesY;
    }

    public void AdvanceFrame()
    {
        int frameCount = GetFrameCount();
        _currentFrame = (_currentFrame + 1) % frameCount;
    }

    public int GetCurrentFrame()
    {
        return _currentFrame;
    }

    public void Update(double elapsedTime)
    {
        if (_currentFrame == GetFrameCount() - 1 && !Looping)
        {
            Finished = true;
            return;
        }
        _currentFrameTime -= elapsedTime;
        if (_currentFrameTime < 0.0)
        {
            AdvanceFrame();
            _currentFrameTime = Speed;
            UpdateUVs();
        }
    }
}
