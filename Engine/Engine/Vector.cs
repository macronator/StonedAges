using System;

namespace Engine;

public struct Vector
{
    public static Vector Zero = new Vector(0.0, 0.0, 0.0);

    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }

    public Vector(double x, double y, double z)
    {
        this = default(Vector);
        X = x;
        Y = y;
        Z = z;
    }

    public Vector Add(Vector r)
    {
        return new Vector(X + r.X, Y + r.Y, Z + r.Z);
    }

    public static Vector operator +(Vector v1, Vector v2)
    {
        return v1.Add(v2);
    }

    public Vector Subtract(Vector r)
    {
        return new Vector(X - r.X, Y - r.Y, Z - r.Z);
    }

    public static Vector operator -(Vector v1, Vector v2)
    {
        return v1.Subtract(v2);
    }

    public Vector Multiply(double v)
    {
        return new Vector(X * v, Y * v, Z * v);
    }

    public static Vector operator *(Vector v, double s)
    {
        return v.Multiply(s);
    }

    public Vector Normalize(Vector v)
    {
        double num = v.Length();
        if (num != 0.0)
        {
            return new Vector(v.X / num, v.Y / num, v.Z / num);
        }
        return new Vector(0.0, 0.0, 0.0);
    }

    public double DotProduct(Vector v)
    {
        return v.X * X + Y * v.Y + Z * v.Z;
    }

    public static double operator *(Vector v1, Vector v2)
    {
        return v1.DotProduct(v2);
    }

    public Vector CrossProduct(Vector v)
    {
        double x = Y * v.Z - Z * v.Y;
        double y = Z * v.X - X * v.Z;
        double z = X * v.Y - Y * v.X;
        return new Vector(x, y, z);
    }

    public override string ToString()
    {
        return $"X:{X}, Y:{Y}, Z:{Z}";
    }

    public double Length()
    {
        return Math.Sqrt(LengthSquared());
    }

    public double LengthSquared()
    {
        return X * X + Y * Y + Z * Z;
    }

    public bool Equals(Vector v)
    {
        if (X == v.X && Y == v.Y)
        {
            return Z == v.Z;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (int)X ^ (int)Y ^ (int)Z;
    }

    public static bool operator ==(Vector v1, Vector v2)
    {
        if (object.ReferenceEquals(v1, v2))
        {
            return true;
        }
        return v1.Equals(v2);
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector)
        {
            return Equals((Vector)obj);
        }
        return base.Equals(obj);
    }

    public static bool operator !=(Vector v1, Vector v2)
    {
        return !v1.Equals(v2);
    }
}
