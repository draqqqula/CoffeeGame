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
    public class TestLevel3 : GameLevel
    {
        protected override LevelSettings GetDefaults()
        {
            var settings = new LevelSettings
            {
                CameraSettings = new CameraSettings()
            };
            return settings;
        }

        private Image Vignette { get; set; }
        private Vector2 PlayerPosition { get; set; } = Vector2.Zero;
        private BuildConfiguration BuildConfiguration { get; set; }

        class PlayerSpawnerEncounter(TestLevel3 level) : Encounter
        {
            private TestLevel3 Level { get; init; } = level;
            public override void Invoke(IControllerProvider state, Vector2 position, Room room)
            {
                Level.PlayerPosition = position;
            }
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            Vignette = state.Using<IFactoryController>().CreateObject<Image>()
            .SetPlacement(new Placement<TintLayer>())
            .SetTexture("vignette")
            .AddToState(state);

            var mapper = new EncounterMapper();
            mapper.AddEncounter<BlazeEnemyEncounter>();
            mapper.AddEncounter<ShellEnemyEncounter>();
            mapper.AddEncounter<BossSpawnerEncounter>();
            mapper.AddEncounter<PetalyEnemyEncounter>();
            mapper.AddEncounter<HealingItemEncounter>();
            mapper.AddEncounter("PlayerSpawnerEncounter", new PlayerSpawnerEncounter(this));

            state.Using<IDungeonController>().CreateDungeon(
                new DungeonParameters("TestLevel", 1, 4, 4),
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
            builder.Build(state, BuildConfiguration, client, PlayerPosition);
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
        }
    }
}
