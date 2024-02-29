using System.Runtime.CompilerServices;

namespace HexTools;

public static class Hex
{
    public static void Write(byte src, char[] dst) => Write(src, dst, 0);

    public static void Write(byte src, char[] dst, int dstIndex)
        => Write(new byte[] { src }, 0, dst, dstIndex, 1);

    public static void Write(byte[] src, int srcIndex, char[] dst, int dstIndex, int count)
    {
        VerifyOffsetAndCount(src, srcIndex, count, offsetName: nameof(srcIndex));
        VerifyOffsetAndCount(dst, dstIndex, count * 2, offsetName: nameof(dstIndex));
        Write(src.AsSpan(srcIndex, count), dst.AsSpan(dstIndex, count));
    }

    public static void Write(ReadOnlySpan<byte> src, Span<char> dst)
    {
        if (src.IsEmpty) return;

        if (dst.Length < src.Length * 2)
            throw new ArgumentException("dst is too small", nameof(dst));

        for (int i = 0; i < src.Length; i++)
        {
            var b = src[i];
            dst[i * 2] = Nibble(b >> 4);
            dst[i * 2 + 1] = Nibble(b);
        }
    }

    /// <summary> <see cref="WriteHex(System.IO.TextWriter,byte)"/> is an extension method for
    /// <see cref="TextWriter"/> to write a single byte</summary>
    public static void WriteHex(this TextWriter tw, byte b)
    {
        tw.Write(new[] { Nibble(b >> 4), Nibble(b) });
    }

    /// <summary> <see cref="WriteHex(System.IO.TextWriter,byte)"/> is an extension method for
    /// <see cref="TextWriter"/> to write a single byte</summary>
    public static void WriteHex(this TextWriter tw, byte[] bytes, int offset, int count)
    {
        VerifyOffsetAndCount(bytes, offset, count);
        var bb = bytes.AsSpan(offset, count);
        var cc = new char[bb.Length * 2].AsSpan();
        Write(bb, cc);
        tw.Write(cc);
    }

    public static void WriteHex(this TextWriter tw, ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty) return;
        var chars = new char[bytes.Length * 2].AsSpan();
        Write(bytes, chars);
        tw.Write(chars);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char Nibble(int nibble)
    {
        var n = nibble & 0x0F;
        return (char)(n > 9 ? n + 0x037 : n + 0x30);
    }


    private static void VerifyOffsetAndCount<T>(T[] array, int offset, int count, string? offsetName = null, string? countName = null)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (offset < 0) throw new ArgumentOutOfRangeException(offsetName ?? nameof(offset));
        if (offset >= array.Length) throw new ArgumentOutOfRangeException(offsetName ?? nameof(offset));
        if (count < 0) throw new ArgumentOutOfRangeException(countName ?? nameof(count));
        if (offset + count >= array.Length) throw new ArgumentOutOfRangeException(countName ?? nameof(count));
    }
}
 