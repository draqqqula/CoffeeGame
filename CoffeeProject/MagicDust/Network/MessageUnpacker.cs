using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Network
{
    public class MessageUnpacker : IUnpacker
    {
        private readonly GameState _state;
        private readonly Dictionary<byte[], GameObject> _collection;

        private static ImmutableArray<Type> PackableTypes = Assembly.GetAssembly(typeof(GameState))
            .GetTypes()
            .Where(type => type.GetInterfaces().Contains(typeof(IPackable)))
            .OrderBy(it => it.GetCustomAttribute<ByteKeyAttribute>().value)
            .ToImmutableArray();

        public void Unpack(byte[] data)
        {
            var clients = _state.Services.GetService<StateClientManager>().GetAll();
            var views = _state.Services.GetService<ViewStorage>();
            var displays = GetDisplays(
                data,
                _state.ApplicationServices.GetService<IContentStorage>(),
                _collection);

            foreach (var client in clients)
            {
                var view = views.GetFor(client);
                foreach (var display in displays)
                {
                    view.Add(display);
                }
            }
        }

        public MessageUnpacker(GameState state, Dictionary<byte[], GameObject> networkCollection)
        {
            this._state = state;
            this._collection = networkCollection;
        }

        private static IEnumerable<IDisplayable> GetDisplays(byte[] bytes, IContentStorage contentStorage, Dictionary<byte[], GameObject> networkCollection)
        {
            int pointer = 0;
            while (pointer < bytes.Length)
            {
                Type type = PackableTypes[bytes[pointer]];
                int length = BinaryPrimitives.ReadInt32LittleEndian(bytes[(pointer + 1)..]);
                IDisplayable obj = null;

                if (type == typeof(FrameForm))
                {
                    obj = FrameForm.Unpack(bytes.AsSpan<byte>()[(pointer + 5)..(pointer + 5 + length)], contentStorage);
                }
                else if (type == typeof(TileMapChunk))
                {

                    obj = TileMapChunk.Unpack(bytes.AsSpan<byte>()[(pointer + 5)..(pointer + 5 + length)], networkCollection);

                }

                pointer += length + 5;

                if (obj is null)
                {
                    throw new ArgumentException("Cannot decide type of encoded object");
                }
                yield return obj;
            }
            yield break;
        }
    }
}
