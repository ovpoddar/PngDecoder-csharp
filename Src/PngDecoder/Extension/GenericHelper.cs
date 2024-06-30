using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PngDecoder.Extension;
internal static class GenericHelper
{
    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int memcmp(IntPtr a1, IntPtr a2, uint count);

    internal static unsafe bool Equal(byte[] data1, byte[] data2)
    {
        if (data1 is null || data2 is null || data1.Length != data2.Length)
            return false;

        fixed (byte* p1 = data1, p2 = data2)
            return memcmp((IntPtr)p1, (IntPtr)p2, (uint)data1.Length * sizeof(byte)) == 0;
    }


    internal static unsafe bool Equal(ReadOnlySpan<byte> data1, ReadOnlySpan<byte> data2)
    {
        if (data1.Length != data2.Length)
            return false;

        fixed (byte* p1 = data1, p2 = data2)
            return memcmp((IntPtr)p1, (IntPtr)p2, (uint)data1.Length * sizeof(byte)) == 0;
    }



    internal static byte[] Tobytes(this ValueType @struct)
    {
        var result = new byte[Marshal.SizeOf(@struct)];
        Unsafe.As<byte, ValueType>(ref result[0]) = @struct;
        return result;
    }

    internal static T ToStruct<T>(this byte[] @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);
    internal static T ToStruct<T>(this Span<byte> @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);

    internal static T FromStream<T>(this Stream stream,
        long skip = 0,
        SeekOrigin origin = SeekOrigin.Current) where T : struct
    {
        var size = Marshal.SizeOf(typeof(T));
        Span<byte> resultAsByte = stackalloc byte[size];
        stream.Seek(skip, origin);

        var read = stream.Read(resultAsByte);
        Debug.Assert(read == size);

        return resultAsByte.ToStruct<T>();
    }

    internal static byte ReadByteNotMove(this Stream stream, long index)
    {
        var current = stream.Position;

        Span<byte> result = stackalloc byte[1];
        stream.Seek(index, SeekOrigin.Current);
        stream.Read(result);
        stream.Seek(current, SeekOrigin.Begin);
        return result[0];
    }
}
