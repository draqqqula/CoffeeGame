using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Display;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Network;
using MagicDustLibrary.Organization.BaseServices;
using MagicDustLibrary.Organization.StateManagement;
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
        private readonly StateClientManager _clientManager;
        private readonly IContentStorage _contentStorage;
        private readonly StateLayerManager _layerManager;
        private readonly CameraStorage _cameraStorage;
        private readonly ViewStorage _viewStorage;
        public StateConnectionHandleManager(StateClientManager clientManager,
            IContentStorage contentStorage, StateLayerManager layerManager,
            CameraStorage cameraStorage, ViewStorage viewStorage)
        {
            _clientManager = clientManager;
            _contentStorage = contentStorage;
            _layerManager = layerManager;
            _cameraStorage = cameraStorage;
            _viewStorage = viewStorage;
        }

        public void SendPictures()
        {
            var clients = _clientManager.GetAll().Where(it => it.IsRemote);
            foreach (var client in clients)
            {
                var data = GetPack(client);
                client.SendData(data.ToArray());
            }
        }

        public IEnumerable<byte> GetInitialPack()
        {
            List<byte> buffer = new();
            var tileMaps = _layerManager.GetAll().SelectMany(it => it).Where(it => it is TileMap).Select(it => it as TileMap);
            int c = 0;
            foreach (var map in tileMaps)
            {
                //map.Link(BitConverter.GetBytes(c).Concat(BitConverter.GetBytes(c)).Concat(BitConverter.GetBytes(c)).Concat(BitConverter.GetBytes(c)).ToArray());
                IEnumerable<byte> mapBytes = map.Pack(_contentStorage);
                buffer.AddRange(BitConverter.GetBytes(mapBytes.Count()));
                buffer.AddRange(mapBytes);
                c++;
            }
            return buffer;
        }

        public IEnumerable<byte> GetPack(GameClient client)
        {
            List<byte> buffer = new();
            var camera = _cameraStorage.GetFor(client);
            foreach (var layer in _layerManager.GetAll())
            {
                var view = _viewStorage.GetFor(client);

                foreach (var drawable in view.GetAndClear())
                {
                    if (drawable is IPackable packable)
                    {
                        var pack = packable.Pack(_contentStorage);
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
    }
}
