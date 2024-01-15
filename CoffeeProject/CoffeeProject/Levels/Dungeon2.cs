using BehaviorKit;
using CoffeeProject.Behaviors;
using CoffeeProject.BoxDisplay;
using CoffeeProject.Encounters;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.RoomGeneration;
using CoffeeProject.Run;
using CoffeeProject.SurfaceMapping;
using CoffeeProject.Weapons;
using MagicDustLibrary.CommonObjectTypes;
using MagicDustLibrary.CommonObjectTypes.Image;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.CommonObjectTypes.TileMap;
using MagicDustLibrary.ComponentModel;
using MagicDustLibrary.Content;
using MagicDustLibrary.Display;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoffeeProject.Levels
{
    public class Dungeon2 : GameLevel, IDungeonLevel
    {
        private LevelArgs _levelArgs;
        protected override LevelSettings GetDefaults()
        {
            var settings = new LevelSettings
            {
                CameraSettings = new CameraSettings()
            };
            return settings;
        }

        private Image Vignette { get; set; }
        private BuildConfiguration BuildConfiguration { get; set; }

        public int Level => 2;

        public Vector2 PlayerPosition { get; set; } = Vector2.Zero;

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            _levelArgs = arguments;
            Vignette = state.Using<IFactoryController>().CreateObject<Image>()
            .SetPlacement(new Placement<TintLayer>())
            .SetTexture("vignette")
            .AddToState(state);

            state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetPlacement(new Placement<GUI>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.CenterLeft)
                .SetScale(0.5f)
                .SetText(arguments.Data[0])
                .SetPos(new Vector2(103, 103))
                .SetColor(Color.Black)
                .AddToState(state);
            state.Using<IFactoryController>()
                .CreateObject<Label>()
                .SetPlacement(new Placement<GUI>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.CenterLeft)
                .SetScale(0.5f)
                .SetText(arguments.Data[0])
                .SetPos(new Vector2(100, 100))
                .SetColor(Color.White)
                .AddToState(state);

            var mapper = new EncounterMapper();
            mapper.AddEncounter(new BlazeEnemyEncounter(Level));
            mapper.AddEncounter(new ShellEnemyEncounter(Level));
            mapper.AddEncounter(new BossSpawnerEncounter(Level));
            mapper.AddEncounter(new PetalyEnemyEncounter(Level));
            mapper.AddEncounter(new HealingItemEncounter(Level));
            mapper.AddEncounter(new ShopItemEncounter(Level));
            mapper.AddEncounter("PlayerSpawnerEncounter", new PlayerSpawnerEncounter(this));

            state.Using<IDungeonController>().CreateDungeon(
                new DungeonParameters("Dungeon2", 4, 8, 8),
                new TileMapParameters("level1", 324, 0.2f, "level"),
                mapper
                );

            BuildConfiguration = JsonSerializer.Deserialize<BuildConfiguration>(arguments.Data[1]);
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
            Vignette
                .SetScale(client.Window.Size.ToVector2() / Vignette.TextureBounds.Size.ToVector2())
                .SetPos(client.Window.Size.ToVector2() / 2);
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            Vignette
                .SetScale(client.Window.Size.ToVector2() / Vignette.TextureBounds.Size.ToVector2())
                .SetPos(client.Window.Size.ToVector2() / 2);

            var builder = new CharacterBuilder();
            var obj = builder.Build(state, BuildConfiguration, client, PlayerPosition, _levelArgs);
            state.Using<IFactoryController>()
            .CreateObject<DynamicLabel>()
            .SetPlacement(new Placement<GUI>())
            .UseFont(state, "Caveat")
            .SetPivot(PivotPosition.CenterLeft)
            .SetScale(0.4f)
            .SetText(() => obj.Stats.Currency.ToString() + " оп.")
            .SetPos(new Vector2(103, 153))
            .SetColor(Color.Black)
            .AddToState(state);
            state.Using<IFactoryController>()
            .CreateObject<DynamicLabel>()
            .SetPlacement(new Placement<GUI>())
            .UseFont(state, "Caveat")
            .SetPivot(PivotPosition.CenterLeft)
            .SetScale(0.4f)
            .SetText(() => obj.Stats.Currency.ToString() + " оп.")
            .SetPos(new Vector2(100, 150))
            .SetColor(Color.White)
            .AddToState(state);
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
        }
    }
}
