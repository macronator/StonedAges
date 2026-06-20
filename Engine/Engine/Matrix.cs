using System;

namespace Engine;

public class Matrix
{
    private double _m11;

    private double _m12;

    private double _m13;

    private double _m21;

    private double _m22;

    private double _m23;

    private double _m31;

    private double _m32;

    private double _m33;

    private double _m41;

    private double _m42;

    private double _m43;

    public static readonly Matrix Identity = new Matrix(new Vector(1.0, 0.0, 0.0), new Vector(0.0, 1.0, 0.0), new Vector(0.0, 0.0, 1.0), new Vector(0.0, 0.0, 1.0));

    public Matrix()
        : this(Identity)
    {
    }

    public Matrix(Matrix m)
    {
        _m11 = m._m11;
        _m12 = m._m12;
        _m13 = m._m13;
        _m21 = m._m21;
        _m22 = m._m22;
        _m23 = m._m23;
        _m31 = m._m31;
        _m32 = m._m32;
        _m33 = m._m33;
        _m41 = m._m41;
        _m42 = m._m42;
        _m43 = m._m43;
    }

    public Matrix(Vector x, Vector y, Vector z, Vector o)
    {
        _m11 = x.X;
        _m12 = x.Y;
        _m13 = x.Z;
        _m21 = y.X;
        _m22 = y.Y;
        _m23 = y.Z;
        _m31 = z.X;
        _m32 = z.Y;
        _m33 = z.Z;
        _m41 = o.X;
        _m42 = o.Y;
        _m43 = o.Z;
    }

    public static Matrix operator *(Matrix mA, Matrix mB)
    {
        Matrix matrix = new Matrix();
        matrix._m11 = mA._m11 * mB._m11 + mA._m12 * mB._m21 + mA._m13 * mB._m31;
        matrix._m12 = mA._m11 * mB._m12 + mA._m12 * mB._m22 + mA._m13 * mB._m32;
        matrix._m13 = mA._m11 * mB._m13 + mA._m12 * mB._m23 + mA._m13 * mB._m33;
        matrix._m21 = mA._m21 * mB._m11 + mA._m22 * mB._m21 + mA._m23 * mB._m31;
        matrix._m22 = mA._m21 * mB._m12 + mA._m22 * mB._m22 + mA._m23 * mB._m32;
        matrix._m23 = mA._m21 * mB._m13 + mA._m22 * mB._m23 + mA._m23 * mB._m33;
        matrix._m31 = mA._m31 * mB._m11 + mA._m32 * mB._m21 + mA._m33 * mB._m31;
        matrix._m32 = mA._m31 * mB._m12 + mA._m32 * mB._m22 + mA._m33 * mB._m32;
        matrix._m33 = mA._m31 * mB._m13 + mA._m32 * mB._m23 + mA._m33 * mB._m33;
        matrix._m41 = mA._m41 * mB._m11 + mA._m42 * mB._m21 + mA._m43 * mA._m31 + mB._m41;
        matrix._m42 = mA._m41 * mB._m12 + mA._m42 * mB._m22 + mA._m43 * mB._m32 + mB._m42;
        matrix._m43 = mA._m41 * mB._m13 + mA._m42 * mB._m23 + mA._m43 * mB._m33 + mB._m43;
        return matrix;
    }

    public static Vector operator *(Vector v, Matrix m)
    {
        return new Vector(v.X * m._m11 + v.Y * m._m21 + v.Z * m._m31 + m._m41, v.X * m._m12 + v.Y * m._m22 + v.Z * m._m32 + m._m42, v.X * m._m13 + v.Y * m._m23 + v.Z * m._m33 + m._m43);
    }

    public void SetTranslation(Vector translation)
    {
        _m41 = translation.X;
        _m42 = translation.Y;
        _m43 = translation.Z;
    }

    public Vector GetTranslation()
    {
        return new Vector(_m41, _m42, _m43);
    }

    public void SetScale(Vector scale)
    {
        _m11 = scale.X;
        _m22 = scale.Y;
        _m33 = scale.Z;
    }

    public Vector GetScale()
    {
        Vector result = default(Vector);
        result.X = new Vector(_m11, _m12, _m13).Length();
        result.Y = new Vector(_m21, _m22, _m23).Length();
        result.Z = new Vector(_m31, _m32, _m33).Length();
        return result;
    }

    public void SetRotate(Vector axis, double angle)
    {
        double num = Math.Sin(angle);
        double num2 = Math.Cos(angle);
        double num3 = 1.0 - num2;
        double num4 = num3 * axis.X;
        double num5 = num3 * axis.Y;
        double num6 = num3 * axis.Z;
        _m11 = num4 * axis.X + num2;
        _m12 = num4 * axis.Y + axis.Z * num;
        _m13 = num4 * axis.Z - axis.Y * num;
        _m21 = num5 * axis.X - axis.Z * num;
        _m22 = num5 * axis.Y + num2;
        _m23 = num5 * axis.Z + axis.X * num;
        _m31 = num6 * axis.X + axis.Y * num;
        _m32 = num6 * axis.Y - axis.X * num;
        _m33 = num6 * axis.Z + num2;
    }

    public double Determinate()
    {
        return _m11 * (_m22 * _m33 - _m23 * _m32) + _m12 * (_m23 * _m31 - _m21 * _m33) + _m13 * (_m21 * _m32 - _m22 * _m31);
    }

    public Matrix Inverse()
    {
        double num = Determinate();
        double num2 = 1.0 / num;
        Matrix matrix = new Matrix();
        matrix._m11 = (_m22 * _m33 - _m23 * _m32) * num2;
        matrix._m12 = (_m13 * _m32 - _m12 * _m33) * num2;
        matrix._m13 = (_m12 * _m23 - _m13 * _m22) * num2;
        matrix._m21 = (_m23 * _m31 - _m21 * _m33) * num2;
        matrix._m22 = (_m11 * _m33 - _m13 * _m31) * num2;
        matrix._m23 = (_m13 * _m21 - _m11 * _m23) * num2;
        matrix._m31 = (_m21 * _m32 - _m22 * _m31) * num2;
        matrix._m32 = (_m12 * _m31 - _m11 * _m32) * num2;
        matrix._m33 = (_m11 * _m22 - _m12 * _m21) * num2;
        matrix._m41 = 0.0 - (_m41 * matrix._m11 + _m42 * matrix._m21 + _m43 * matrix._m31);
        matrix._m42 = 0.0 - (_m41 * matrix._m12 + _m42 * matrix._m22 + _m43 * matrix._m32);
        matrix._m43 = 0.0 - (_m41 * matrix._m13 + _m42 * matrix._m23 + _m43 * matrix._m33);
        return matrix;
    }
}
