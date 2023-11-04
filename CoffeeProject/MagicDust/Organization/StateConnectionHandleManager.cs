using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Organization
{
    public class StateConnectionHandleManager : ClientRelatedActions
    {
        private readonly GameState _state;

        public void SendPictures()
        {
            var clients = _state.StateServices.GetService<StateClientManager>().GetAll().Where(it => it.IsRemote);
            foreach (var client in clients)
            {
                var data = GetPack(client);
                client.SendData(data.ToArray());
            }
        }

        public IEnumerable<byte> GetInitialPack()
        {
            List<byte> buffer = new();
            var tileMaps = _state.StateServices.GetService<StateLayerManager>().GetAll().SelectMany(it => it).Where(it => it is TileMap).Select(it => it as TileMap);
            int c = 0;
            foreach (var map in tileMaps)
            {
                map.Link(BitConverter.GetBytes(c).Concat(BitConverter.GetBytes(c)).Concat(BitConverter.GetBytes(c)).Concat(BitConverter.GetBytes(c)).ToArray());
                IEnumerable<byte> mapBytes = map.Pack(_state.ApplicationServices.GetService<IContentStorage>());
                buffer.AddRange(BitConverter.GetBytes(mapBytes.Count()));
                buffer.AddRange(mapBytes);
                c++;
            }
            return buffer;
        }

        public IEnumerable<byte> GetPack(GameClient client)
        {
            List<byte> buffer = new();
            var camera = _state.StateServices.GetService<CameraStorage>().GetFor(client);
            foreach (var layer in _state.StateServices.GetService<StateLayerManager>().GetAll())
            {
                var view = _state.StateServices.GetService<ViewStorage>().GetFor(client);

                foreach (var drawable in view.GetAndClear())
                {
                    if (drawable is IPackable packable)
                    {
                        var pack = packable.Pack(_state.ContentStorage);
                        buffer.Add(packable.GetType().GetCustomAttribute<ByteKeyAttribute>().value);
                        buffer.AddRange(BitConverter.GetBytes(pack.Count()));
                        buffer.AddRange(pack);
                    }
                }
            }

            return buffer.ToArray();
        }

        protected override void AddClient(GameClient client)
        {
            if (client.IsRemote)
            {
                client.SendData(GetInitialPack().ToArray());
            }
        }

        protected override void RemoveClient(GameClient client)
        {
        }

        protected override void UpdateClient(GameClient client)
        {
        }

        public StateConnectionHandleManager(GameState state)
        {
            _state = state;
        }
    }
}
