using MagicDustLibrary.Display;

namespace MagicDustLibrary.Network
{
    public interface IPackable
    {
        public IEnumerable<byte> Pack(DefaultContentStorage contentStorage);

    }

    public class ByteKeyAttribute : Attribute
    {
        public byte value;

        public ByteKeyAttribute(byte value)
        {
            this.value = value;
        }
    }
}
