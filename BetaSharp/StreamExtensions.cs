using System.Buffers.Binary;
using BetaSharp.Util;

namespace BetaSharp;

internal static class StreamExtensions
{
    extension(Stream stream)
    {
        public void WriteByte(sbyte value)
        {
            stream.WriteByte((byte)value);
        }

        public void WriteBoolean(bool value)
        {
            stream.WriteByte((byte)(value ? 1 : 0));
        }

        public void WriteShort(short value)
        {
            Span<byte> span = stackalloc byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(span, value);
            stream.Write(span);
        }

        public void WriteUShort(ushort value)
        {
            Span<byte> span = stackalloc byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(span, value);
            stream.Write(span);
        }

        public void WriteInt(int value)
        {
            Span<byte> span = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(span, value);
            stream.Write(span);
        }

        public void WriteFloat(float value)
        {
            Span<byte> span = stackalloc byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(span, value);
            stream.Write(span);
        }

        public void WriteDouble(double value)
        {
            Span<byte> span = stackalloc byte[sizeof(double)];
            BinaryPrimitives.WriteDoubleBigEndian(span, value);
            stream.Write(span);
        }

        public void WriteLong(long value)
        {
            Span<byte> span = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(span, value);
            stream.Write(span);
        }

        public void WriteString(string value)
        {
            byte[] buffer = ModifiedUtf8.GetBytes(value);

            ArgumentOutOfRangeException.ThrowIfGreaterThan(buffer.Length, short.MaxValue);

            stream.WriteUShort((ushort)buffer.Length);
            stream.Write(buffer);
        }

        public bool ReadBoolean()
        {
            return stream.ReadByte() > 0;
        }

        public short ReadShort()
        {
            Span<byte> span = stackalloc byte[sizeof(short)];
            stream.ReadExactly(span);

            return BinaryPrimitives.ReadInt16BigEndian(span);
        }

        public ushort ReadUShort()
        {
            Span<byte> span = stackalloc byte[sizeof(ushort)];
            stream.ReadExactly(span);

            return BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        public int ReadInt()
        {
            Span<byte> span = stackalloc byte[sizeof(int)];
            stream.ReadExactly(span);

            return BinaryPrimitives.ReadInt32BigEndian(span);
        }

        public float ReadFloat()
        {
            Span<byte> span = stackalloc byte[sizeof(float)];
            stream.ReadExactly(span);

            return BinaryPrimitives.ReadSingleBigEndian(span);
        }

        public double ReadDouble()
        {
            Span<byte> span = stackalloc byte[sizeof(double)];
            stream.ReadExactly(span);

            return BinaryPrimitives.ReadDoubleBigEndian(span);
        }

        public long ReadLong()
        {
            Span<byte> span = stackalloc byte[sizeof(long)];
            stream.ReadExactly(span);

            return BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public string ReadString(int maxLength = ushort.MaxValue)
        {
            ushort length = stream.ReadUShort();

            ArgumentOutOfRangeException.ThrowIfGreaterThan(length, maxLength);

            byte[] buffer = new byte[length];

            stream.ReadExactly(buffer);

            return ModifiedUtf8.GetString(buffer);
        }
    }
}
