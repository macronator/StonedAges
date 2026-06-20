namespace Engine;

public class Sprite
{
    internal const int VertexAmount = 6;

    public Vector _windowOffset = default(Vector);

    private Vector[] _vertexPositions = new Vector[6];

    private Color[] _vertexColors = new Color[6];

    private Point[] _vertexUVs = new Point[6];

    private Texture _texture = default(Texture);

    public bool _flip;

    public bool _rotate;

    private double _scaleX = 1.0;

    private double _scaleY = 1.0;

    private double _rotation;

    private double _positionX;

    private double _positionY;

    public Texture Texture
    {
        get
        {
            return _texture;
        }
        set
        {
            _texture = value;
            InitVertexPositions(GetTopLeft(), _texture.Width, _texture.Height);
            if (_rotation != 0.0)
            {
                SetRotation(_rotation);
            }
            SetScale(_scaleX, _scaleY);
        }
    }

    public Vector[] VertexPositions => _vertexPositions;

    public Color[] VertexColors => _vertexColors;

    public Point[] VertexUVs => _vertexUVs;

    public Sprite(double x = 0.0, double y = 0.0)
    {
        InitVertexPositions(new Vector(x, y, 0.0), 0.0, 0.0);
        SetColor(new Color(1f, 1f, 1f, 1f));
        SetUVs(new Point(0f, 0f), new Point(1f, 1f));
    }

    public bool CollidesWith(double rectx, double recty)
    {
        if (rectx >= GetPosition().X && recty >= GetPosition().Y && rectx <= GetPosition().X + (double)_texture.Width && recty <= GetPosition().Y + (double)_texture.Height)
        {
            return true;
        }
        return false;
    }

    public Vector GetPosition()
    {
        return GetTopLeft();
    }

    private Vector GetTopLeft()
    {
        return new Vector(_vertexPositions[0].X, _vertexPositions[0].Y, _vertexPositions[0].Z);
    }

    private Vector GetCenter()
    {
        double num = GetWidth() / 2.0;
        double num2 = GetHeight() / 2.0;
        return new Vector(_vertexPositions[0].X + num, _vertexPositions[0].Y - num2, _vertexPositions[0].Z);
    }

    private void InitVertexPositions(Vector position, double width, double height)
    {
        double positionY = _positionY;
        double num = _positionX;
        double y = _positionY + height * _scaleX;
        double num2 = _positionX + width * _scaleY;
        if (_flip)
        {
            double num3 = num;
            num = num2;
            num2 = num3;
        }
        if (_rotate)
        {
            ref Vector reference = ref _vertexPositions[0];
            reference = new Vector(num, y, position.Z);
            ref Vector reference2 = ref _vertexPositions[1];
            reference2 = new Vector(num, positionY, position.Z);
            ref Vector reference3 = ref _vertexPositions[2];
            reference3 = new Vector(num2, y, position.Z);
            ref Vector reference4 = ref _vertexPositions[3];
            reference4 = new Vector(num, positionY, position.Z);
            ref Vector reference5 = ref _vertexPositions[4];
            reference5 = new Vector(num2, positionY, position.Z);
            ref Vector reference6 = ref _vertexPositions[5];
            reference6 = new Vector(num2, y, position.Z);
        }
        else
        {
            ref Vector reference7 = ref _vertexPositions[0];
            reference7 = new Vector(num, positionY, position.Z);
            ref Vector reference8 = ref _vertexPositions[1];
            reference8 = new Vector(num2, positionY, position.Z);
            ref Vector reference9 = ref _vertexPositions[2];
            reference9 = new Vector(num, y, position.Z);
            ref Vector reference10 = ref _vertexPositions[3];
            reference10 = new Vector(num2, positionY, position.Z);
            ref Vector reference11 = ref _vertexPositions[4];
            reference11 = new Vector(num2, y, position.Z);
            ref Vector reference12 = ref _vertexPositions[5];
            reference12 = new Vector(num, y, position.Z);
        }
    }

    public double GetWidth()
    {
        return _vertexPositions[1].X - _vertexPositions[0].X;
    }

    public double GetHeight()
    {
        return _vertexPositions[2].Y - _vertexPositions[0].Y;
    }

    public void SetWidth(float width)
    {
        InitVertexPositions(GetTopLeft(), width, GetHeight());
        _texture.Width = (int)width;
    }

    public void SetHeight(float height)
    {
        InitVertexPositions(GetTopLeft(), GetWidth(), height);
        _texture.Height = (int)height;
    }

    public void ApplyMatrix(Matrix m)
    {
        for (int i = 0; i < VertexPositions.Length; i++)
        {
            VertexPositions[i] *= m;
        }
    }

    public void SetPosition(Vector position, bool ignore = false)
    {
        Matrix matrix = new Matrix();
        matrix.SetTranslation(new Vector(_positionX, _positionY, 0.0));
        ApplyMatrix(matrix.Inverse());
        matrix.SetTranslation(position);
        ApplyMatrix(matrix);
        if (!ignore)
        {
            _positionX = position.X;
            _positionY = position.Y;
        }
    }

    public void SetScale(double x, double y)
    {
        double positionX = _positionX;
        double positionY = _positionY;
        SetPosition(0.0, 0.0);
        Matrix matrix = new Matrix();
        matrix.SetScale(new Vector(_scaleX, _scaleY, 1.0));
        matrix = matrix.Inverse();
        ApplyMatrix(matrix);
        matrix = new Matrix();
        matrix.SetScale(new Vector(x, y, 1.0));
        ApplyMatrix(matrix);
        SetPosition(positionX, positionY);
        _scaleX = x;
        _scaleY = y;
    }

    public void SetRotation(double rotation)
    {
        double positionX = _positionX;
        double positionY = _positionY;
        SetPosition(0.0, 0.0);
        Matrix matrix = new Matrix();
        matrix.SetRotate(new Vector(0.0, 0.0, 1.0), _rotation);
        ApplyMatrix(matrix.Inverse());
        matrix = new Matrix();
        matrix.SetRotate(new Vector(0.0, 0.0, 1.0), rotation);
        ApplyMatrix(matrix);
        SetPosition(positionX, positionY);
        _rotation = rotation;
    }

    public void SetPosition(double x, double y, bool ignore = false)
    {
        SetPosition(new Vector(x, y, 0.0), ignore);
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < 6; i++)
        {
            _vertexColors[i] = color;
        }
    }

    public void SetUVs(Point topLeft, Point bottomRight)
    {
        _vertexUVs[0] = topLeft;
        ref Point reference = ref _vertexUVs[1];
        reference = new Point(bottomRight.X, topLeft.Y);
        ref Point reference2 = ref _vertexUVs[2];
        reference2 = new Point(topLeft.X, bottomRight.Y);
        ref Point reference3 = ref _vertexUVs[3];
        reference3 = new Point(bottomRight.X, topLeft.Y);
        _vertexUVs[4] = bottomRight;
        ref Point reference4 = ref _vertexUVs[5];
        reference4 = new Point(topLeft.X, bottomRight.Y);
    }
}
