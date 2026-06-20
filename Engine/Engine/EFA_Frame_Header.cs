using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Engine;

public struct EFA_Frame_Header
{
    public uint Unknown1;

    public uint Offset;

    public uint Size;

    public uint RawSize;

    public uint Unknown2;

    public uint Unknown3;

    public uint Width;

    public uint Unknown4;

    public uint ByteCount;

    public uint Unknown5;

    public ushort OriginX;

    public ushort OriginY;

    public uint OriginFlags;

    public ushort Pad1X;

    public ushort Pad1Y;

    public uint Pad1Flags;

    public ushort Pad2X;

    public ushort Pad2Y;

    public uint Pad2Flags;

    public static EFA_Frame_Header FromBinaryReaderBlock(BinaryReader br)
    {
        GCHandle gCHandle = GCHandle.Alloc(br.ReadBytes(Marshal.SizeOf(typeof(EFA_Frame_Header))), GCHandleType.Pinned);
        EFA_Frame_Header result = (EFA_Frame_Header)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(EFA_Frame_Header));
        gCHandle.Free();
        return result;
    }

    public void Dump()
    {
        Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}\n{12}\n{13}\n{14}\n{15}\n{16}\n{17}\n{18}\n", Unknown1, Offset, Size, RawSize, Unknown2, Unknown3, Width, Unknown4, ByteCount, Unknown5, OriginX, OriginY, OriginFlags, Pad1X, Pad1Y, Pad1Flags, Pad2X, Pad2Y, Pad2Flags);
    }
}
