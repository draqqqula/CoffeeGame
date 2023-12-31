﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace MagicDustLibrary.Display
{
    public interface IContentStorage
    {
        public T GetAsset<T>(string name) where T : class;
        public T GetAsset<T>(int id) where T : class;
        public void AddAsset(object asset, string name);
        public string GetName(object asset);
        public int GetID(object asset);
    }
    public class DefaultContentStorage : IContentStorage
    {
        private readonly ContentManager ContentProvider;
        private readonly GraphicsDevice Device;
        private readonly Dictionary<string, object> Data = new Dictionary<string, object>();
        private readonly Dictionary<object, string> ReverseData = new Dictionary<object, string>();
        private readonly string[] IDExchanger;
        private readonly Dictionary<string, int> ReverseIDExchanger;
        private readonly Dictionary<string, List<string>> FileExtensions;

        public T GetAsset<T>(string name) where T : class
        {
            if (Data.ContainsKey(name))
            {
                return Data[name] as T;
            }
            else
            {
                T asset;
                try
                {
                    asset = ContentProvider.Load<T>(name);
                }
                catch
                {
                    return null;
                }
                Data.Add(name, asset);
                ReverseData.Add(asset, name);
                return asset;
            }
        }

        public void AddAsset(object asset, string name)
        {
            Data.Add(name, asset);
        }

        public T GetAsset<T>(int id) where T : class
        {
            return GetAsset<T>(IDExchanger[id]);
        }

        public string GetName(object asset)
        {
            return ReverseData[asset];
        }

        public int GetID(object asset)
        {
            return ReverseIDExchanger[GetName(asset)];
        }

        public void PreLoadAssets(params string[] names)
        {
            foreach (var name in names)
            {
                Data[name] = ContentProvider.Load<Texture2D>(name);
            }
        }

        public DefaultContentStorage(GraphicsDevice device, ContentManager content)
        {
            ContentProvider = content;
            Device = device;
            var files = Directory.CreateDirectory(content.RootDirectory).GetFiles();
            IDExchanger = files
                .Select(it => Path.GetFileNameWithoutExtension(it.Name))
                .OrderBy(it => it).Distinct().ToArray();
            ReverseIDExchanger = Enumerable.Range(0, IDExchanger.Length).ToDictionary(it => IDExchanger[it]);
            FileExtensions = CreateFileExtensions(files);
        }

        private Dictionary<string, List<string>> CreateFileExtensions(FileInfo[] files)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var file in files)
            {
                var withExtension = Path.GetFileName(file.Name);
                var noExtension = Path.GetFileNameWithoutExtension(file.Name);
                if (result.ContainsKey(noExtension))
                {
                    result[noExtension].Add(withExtension);
                }
                else
                {
                    result.Add(noExtension, new List<string> { withExtension });
                }
            }
            return result;
        }

        public Texture2D GenerateMissingTexture(Color goodColor, Color badColor, int size)
        {
            Color[] colors = new Color[size * size];
            for (int i = 0; i < size * size; i++)
            {
                if (i % 2 == 0) colors[i] = goodColor;
                else colors[i] = badColor;
            }
            Texture2D texture = new Texture2D(Device, size, size);
            texture.SetData(colors);
            return texture;
        }

        private static Color GenerateRandomColor()
        {
            Random random = new Random();
            Vector4 vector = new(random.NextSingle(), random.NextSingle(), random.NextSingle(), 1.0f);
            return Color.FromNonPremultiplied(vector);
        }
    }
}
