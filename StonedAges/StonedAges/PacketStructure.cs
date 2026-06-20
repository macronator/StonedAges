using System;
using System.Text;

namespace StonedAges;

public abstract class PacketStructure
{
    private byte[] _buffer;

    public byte[] Data => _buffer;

    public PacketStructure(ushort length, ushort type)
    {
        _buffer = new byte[length];
        WriteUShort(length, 0);
        WriteUShort(type, 2);
    }

    public PacketStructure(byte[] packet)
    {
        _buffer = packet;
    }

    public byte ReadByte(int offset)
    {
        return _buffer[offset];
    }

    public bool ReadBool(int offset)
    {
        return BitConverter.ToBoolean(_buffer, offset);
    }

    public uint ReadUInt(int offset)
    {
        return BitConverter.ToUInt32(_buffer, offset);
    }

    public ushort ReadUShort(int offset)
    {
        return BitConverter.ToUInt16(_buffer, offset);
    }

    public string ReadString(int offset, int count)
    {
        return Encoding.ASCII.GetString(_buffer, offset, count);
    }

    public int ReadInt(int offset)
    {
        return BitConverter.ToInt32(_buffer, offset);
    }

    public short ReadShort(int offset)
    {
        return BitConverter.ToInt16(_buffer, offset);
    }

    public void WriteShort(short value, int offset)
    {
        byte[] array = new byte[2];
        array = BitConverter.GetBytes(value);
        Array.Copy(array, 0, _buffer, offset, 2);
    }

    public void WriteInt(int value, int offset)
    {
        byte[] array = new byte[4];
        array = BitConverter.GetBytes(value);
        Array.Copy(array, 0, _buffer, offset, 4);
    }

    public void WriteUShort(ushort value, int offset)
    {
        byte[] array = new byte[2];
        array = BitConverter.GetBytes(value);
        Array.Copy(array, 0, _buffer, offset, 2);
    }

    public void WriteUInt(uint value, int offset)
    {
        byte[] array = new byte[4];
        array = BitConverter.GetBytes(value);
        Array.Copy(array, 0, _buffer, offset, 4);
    }

    public void WriteString(string value, int offset)
    {
        byte[] array = new byte[value.Length];
        array = Encoding.ASCII.GetBytes(value);
        Array.Copy(array, 0, _buffer, offset, value.Length);
    }

    public void WriteByte(byte value, int offset)
    {
        byte[] array = new byte[1];
        array = BitConverter.GetBytes(value);
        Array.Copy(array, 0, _buffer, offset, 1);
    }

    public void WriteBool(bool value, int offset)
    {
        byte[] array = new byte[1];
        array = BitConverter.GetBytes(value);
        Array.Copy(array, 0, _buffer, offset, 1);
    }
}
