using System.Numerics;

namespace GMToolKit.Inspector
{
    [Drawer(typeof(int))]
    internal class IntDrawer : IntegerDrawer<int>
    {
        protected override int MinValue => int.MinValue;
        protected override int MaxValue => int.MaxValue;
        protected override BigInteger Convertor(int v) => v;
        protected override int Convertor(BigInteger v) => (int)v;
        protected override bool Equals(int left, int right) => left == right;
    }

    [Drawer(typeof(uint))]
    internal class UIntDrawer : IntegerDrawer<uint>
    {
        protected override uint MinValue => uint.MinValue;
        protected override uint MaxValue => uint.MaxValue;
        protected override BigInteger Convertor(uint v) => v;
        protected override uint Convertor(BigInteger v) => (uint)v;
        protected override bool Equals(uint left, uint right) => left == right;
    }

    [Drawer(typeof(long))]
    internal class LongDrawer : IntegerDrawer<long>
    {
        protected override long MinValue => long.MinValue;
        protected override long MaxValue => long.MaxValue;
        protected override BigInteger Convertor(long v) => v;
        protected override long Convertor(BigInteger v) => (long)v;
        protected override bool Equals(long left, long right) => left == right;
    }

    [Drawer(typeof(ulong))]
    internal class ULongDrawer : IntegerDrawer<ulong>
    {
        protected override ulong MinValue => ulong.MinValue;
        protected override ulong MaxValue => ulong.MaxValue;
        protected override BigInteger Convertor(ulong v) => v;
        protected override ulong Convertor(BigInteger v) => (ulong)v;
        protected override bool Equals(ulong left, ulong right) => left == right;
    }

    [Drawer(typeof(byte))]
    internal class ByteDrawer : IntegerDrawer<byte>
    {
        protected override byte MinValue => byte.MinValue;
        protected override byte MaxValue => byte.MaxValue;
        protected override BigInteger Convertor(byte v) => v;
        protected override byte Convertor(BigInteger v) => (byte)v;
        protected override bool Equals(byte left, byte right) => left == right;
    }

    [Drawer(typeof(sbyte))]
    internal class SByteDrawer : IntegerDrawer<sbyte>
    {
        protected override sbyte MinValue => sbyte.MinValue;
        protected override sbyte MaxValue => sbyte.MaxValue;
        protected override BigInteger Convertor(sbyte v) => v;
        protected override sbyte Convertor(BigInteger v) => (sbyte)v;
        protected override bool Equals(sbyte left, sbyte right) => left == right;
    }

    [Drawer(typeof(short))]
    internal class ShortDrawer : IntegerDrawer<short>
    {
        protected override short MinValue => short.MinValue;
        protected override short MaxValue => short.MaxValue;
        protected override BigInteger Convertor(short v) => v;
        protected override short Convertor(BigInteger v) => (short)v;
        protected override bool Equals(short left, short right) => left == right;
    }
}