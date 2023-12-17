using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Network;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework.Graphics;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using MagicDustLibrary.CommonObjectTypes;
using System.Text;
using MagicDustLibrary.ComponentModel;

namespace MagicDustLibrary.Display
{
    [ByteKey(3)]
    public partial record struct DrawingParameters : IPackable
    {

        public IEnumerable<byte> Pack(IContentStorage contentStorage)
        {
            Span<byte> buffer = stackalloc byte[29];
            MemoryMarshal.Write(buffer, ref Position);
            MemoryMarshal.Write(buffer.Slice(8), ref Color);
            MemoryMarshal.Write(buffer.Slice(12), ref Rotation);
            MemoryMarshal.Write(buffer.Slice(16), ref Scale);
            MemoryMarshal.Write(buffer.Slice(24), ref Priority);
            bool mirrored = (Mirroring == SpriteEffects.None);
            MemoryMarshal.Write(buffer.Slice(28), ref mirrored);
            return buffer.ToArray();
        }

        public static DrawingParameters Unpack(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 29)
                throw new ArgumentException("Invalid byte array length for DrawingParameters");

            Vector2 position = MemoryMarshal.Read<Vector2>(bytes);
            Color color = MemoryMarshal.Read<Color>(bytes[8..]);
            float rotation = MemoryMarshal.Read<float>(bytes[12..]);
            Vector2 scale = MemoryMarshal.Read<Vector2>(bytes[16..]);
            float priority = MemoryMarshal.Read<float>(bytes[24..]);
            bool mirrored = MemoryMarshal.Read<bool>(bytes[28..]);
            SpriteEffects mirroring = mirrored ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            return new DrawingParameters(position, color, rotation, scale, mirroring, priority);
        }
    }
    [ByteKey(2)]
    public partial struct FrameForm : IDisplayable, IPackable
    {
        public IEnumerable<byte> Pack(IContentStorage contentStorage)
        {
            Span<byte> buffer = stackalloc byte[57];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, Borders.X);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[4..], Borders.Y);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[8..], Borders.Width);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[12..], Borders.Height);
            BinaryPrimitives.WriteSingleLittleEndian(buffer[16..], Anchor.X);
            BinaryPrimitives.WriteSingleLittleEndian(buffer[20..], Anchor.Y);
            BinaryPrimitives.WriteInt32LittleEndian(buffer[24..], contentStorage.GetID(Sheet));
            Arguments.Pack(contentStorage).ToArray().CopyTo(buffer.Slice(28, 29));
            return buffer.ToArray();
        }

        public static FrameForm Unpack(ReadOnlySpan<byte> bytes, IContentStorage contentStorage)
        {
            if (bytes.Length != 57)
                throw new ArgumentException("Invalid byte array length for FrameForm");

            Rectangle borders = new Rectangle(
                BinaryPrimitives.ReadInt32LittleEndian(bytes),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[4..]),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[8..]),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[12..]));
            Vector2 anchor = new Vector2(
                BinaryPrimitives.ReadSingleLittleEndian(bytes[16..]),
                BinaryPrimitives.ReadSingleLittleEndian(bytes[20..]));
            int sheetID = BinaryPrimitives.ReadInt32LittleEndian(bytes[24..]);
            DrawingParameters arguments = DrawingParameters.Unpack(bytes.Slice(28, 29).ToArray());

            return new FrameForm(borders, anchor, arguments, contentStorage.GetAsset<Texture2D>(sheetID));

        }
    }
}

namespace MagicDustLibrary.CommonObjectTypes.TileMap
{

    [ByteKey(1)]
    public partial struct TileMapChunk : IPackable, IDisplayable
    {
        public IEnumerable<byte> Pack(IContentStorage contentStorage)
        {
            List<byte> buffer = new();
            //var LinkID = Source.LinkedID;
            //buffer.AddRange(LinkID);
            buffer.AddRange(BitConverter.GetBytes(Position.X));
            buffer.AddRange(BitConverter.GetBytes(Position.Y));
            buffer.AddRange(BitConverter.GetBytes(Chunk.X));
            buffer.AddRange(BitConverter.GetBytes(Chunk.Y));
            buffer.AddRange(BitConverter.GetBytes(Chunk.Width));
            buffer.AddRange(BitConverter.GetBytes(Chunk.Height));
            buffer.AddRange(BitConverter.GetBytes(Extra.Count()));
            foreach (Point point in Extra)
            {
                buffer.AddRange(BitConverter.GetBytes(point.X));
                buffer.AddRange(BitConverter.GetBytes(point.Y));
            }
            return buffer;
        }

        public static TileMapChunk Unpack(ReadOnlySpan<byte> bytes, Dictionary<byte[], ComponentBase> networkCollection)
        {
            byte[] linkID = bytes[0..16].ToArray();

            Vector2 position = new Vector2(
                BinaryPrimitives.ReadSingleLittleEndian(bytes[16..]),
                BinaryPrimitives.ReadSingleLittleEndian(bytes[20..])
                );
            Rectangle chunk = new Rectangle(
                BinaryPrimitives.ReadInt32LittleEndian(bytes[24..]),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[28..]),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[32..]),
                BinaryPrimitives.ReadInt32LittleEndian(bytes[36..])
                );
            Point[] extra = new Point[BinaryPrimitives.ReadInt32LittleEndian(bytes[40..])];
            for (int i = 0; i < extra.Length; i++)
            {
                extra[i] = new Point(
                    BinaryPrimitives.ReadInt32LittleEndian(bytes[(44 + i * 8)..]),
                    BinaryPrimitives.ReadInt32LittleEndian(bytes[(48 + i * 8)..])
                    );
            }
            //return new TileMapChunk(networkCollection[linkID] as TileMap, chunk, position, extra);
            throw new Exception();
        }
    }

    [ByteKey(0)]
    public partial class TileMap : IPackable
    {
        public IEnumerable<byte> Pack(IContentStorage contentStorage)
        {
            throw new NotImplementedException();
        }

        private static byte[,] BuildMap(ReadOnlySpan<byte> bytes, int rows, int columns)
        {
            byte[,] map = new byte[rows, columns];
            int index = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    map[i, j] = bytes[index];
                    index++;
                }
            }

            return map;
        }

        public static TileMap Unpack(ReadOnlySpan<byte> bytes, GameState state, Layer layer)
        {
            throw new NotImplementedException();
        }

        private void Link(byte[] linkID)
        {
            throw new NotImplementedException();
        }
    }
}