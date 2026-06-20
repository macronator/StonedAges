using Tao.OpenGl;

namespace Engine;

public class Batch
{
    private const int MaxVertexNumber = 1000;

    private const int VertexDimensions = 3;

    private const int ColorDimensions = 4;

    private const int UVDimensions = 2;

    private Vector[] _vertexPositions = new Vector[1000];

    private Color[] _vertexColors = new Color[1000];

    private Point[] _vertexUVs = new Point[1000];

    private int _batchSize;

    public void AddSprite(Sprite sprite, byte colorize = 0)
    {
        if (colorize != 0)
        {
            Gl.glBlendFunc(1, 771);
        }
        if (sprite.VertexPositions.Length + _batchSize > 1000)
        {
            Draw();
        }
        for (int i = 0; i < sprite.VertexPositions.Length; i++)
        {
            ref Vector reference = ref _vertexPositions[_batchSize + i];
            reference = sprite.VertexPositions[i];
            switch (colorize)
            {
                case 1:
                    {
                        ref Color reference4 = ref _vertexColors[_batchSize + i];
                        reference4 = Color.LightBlue;
                        break;
                    }
                case 2:
                    {
                        ref Color reference3 = ref _vertexColors[_batchSize + i];
                        reference3 = Color.Purple;
                        break;
                    }
                default:
                    {
                        ref Color reference2 = ref _vertexColors[_batchSize + i];
                        reference2 = sprite.VertexColors[i];
                        break;
                    }
            }
            ref Point reference5 = ref _vertexUVs[_batchSize + i];
            reference5 = sprite.VertexUVs[i];
        }
        _batchSize += sprite.VertexPositions.Length;
    }

    private void SetupPointers()
    {
        Gl.glEnableClientState(32886);
        Gl.glEnableClientState(32884);
        Gl.glEnableClientState(32888);
        Gl.glVertexPointer(3, 5130, 0, _vertexPositions);
        Gl.glColorPointer(4, 5126, 0, _vertexColors);
        Gl.glTexCoordPointer(2, 5126, 0, _vertexUVs);
    }

    public void Draw()
    {
        if (_batchSize != 0)
        {
            SetupPointers();
            Gl.glDrawArrays(4, 0, _batchSize);
            _batchSize = 0;
            Gl.glEnable(3553);
            Gl.glBlendFunc(770, 771);
        }
    }
}
