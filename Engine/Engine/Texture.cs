namespace Engine;

public struct Texture
{
    public int Id { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string TextureId { get; set; }

    public Texture(int id, int width, int height)
    {
        this = default(Texture);
        Id = id;
        Width = width;
        Height = height;
    }
}
