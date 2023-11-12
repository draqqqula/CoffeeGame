using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MagicDustLibrary.Network
{
    public class StateUnpacker : IUnpacker
    {
        private readonly GameState _state;
        private readonly Dictionary<byte[], GameObject> _collection;
        private readonly Layer _unpackLayer;
        public void Unpack(byte[] data)
        {
            var tilemaps = UnpackTileMaps(data, _unpackLayer);
            foreach (var tilemap in tilemaps)
            {
                //_collection.Add(tilemap.LinkedID, tilemap);
            }
        }

        public IEnumerable<TileMap> UnpackTileMaps(byte[] bytes, Layer layer)
        {
            int pointer = 0;
            while (pointer < bytes.Length)
            {
                int length = BinaryPrimitives.ReadInt32LittleEndian(bytes[pointer..]);
                var obj = TileMap.Unpack(bytes[(pointer + 4)..(pointer + 4 + length)], _state, _unpackLayer);
                pointer += length + 4;
                yield return obj;
            }
        }

        public StateUnpacker(GameState state, Dictionary<byte[], GameObject> networkCollection, Layer unpackLayer)
        {
            this._state = state;
            this._collection = networkCollection;
            _unpackLayer = unpackLayer;
        }
    }
}
