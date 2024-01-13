using CoffeeProject.Behaviors;
using CoffeeProject.GameObjects;
using CoffeeProject.Layers;
using CoffeeProject.Run;
using CoffeeProject.SurfaceMapping;
using MagicDustLibrary.CommonObjectTypes.TextDisplays;
using MagicDustLibrary.Content;
using MagicDustLibrary.Extensions;
using MagicDustLibrary.Factorys;
using MagicDustLibrary.Logic;
using MagicDustLibrary.Logic.Controllers;
using MagicDustLibrary.Organization;
using MagicDustLibrary.Organization.DefualtImplementations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BehaviorKit;
using MagicDustLibrary.ComponentModel;
using System.IO;
using MagicDustLibrary.CommonObjectTypes.Image;
using CoffeeProject.Weapons;
using System.Text.Json;

namespace CoffeeProject.Levels
{
    public class NameScreen : GameLevel
    {
        class RunLoader
        {
            public string[] FirstPart { get; set; }
            public string[] SecondPart { get; set; }
            public string[] ThirdPart { get; set; }
            public RunLoader(
                [FromStorage("Saves", "*")]ExpandoObject obj
                )
            {
                GetParts(obj);
            }

            public void GetParts(dynamic json)
            {
                List<object> first = json.FirstPart;
                List<object> second = json.SecondPart;
                List<object> third = json.ThirdPart;

                FirstPart = first.Select(it => it.ToString()).ToArray();
                SecondPart = second.Select(it => it.ToString()).ToArray();
                ThirdPart = third.Select(it => it.ToString()).ToArray();
            }
        }

        private Image Tint { get; set; }
        private CameraAnchor Anchor { get; set; }
        private GameClient MainClient { get; set; }
        private RunInfo RunInfo { get; set; } = new ();
        private string NewName { get; set; } = "";
        private int NameSelectionStep { get; set; } = 0;
        private int SelectionIndex { get; set; } = 0;
        private List<string[]> Batches { get; set; } = [];
        private DynamicLabel NameLabel { get; set; }
        private DynamicLabel PartLabel { get; set; }
        private string CurrentMessage { get; set; } = "Как меня зовут?";
        private DynamicLabel MessageLabel { get; set; }
        private DynamicLabel DescriptionLabel {  get; set; }
        private DynamicLabel DescriptionContinuation { get; set; }
        private bool OnTransition { get; set; } = false;
        public BuildConfiguration Build = new BuildConfiguration(null, null, null);
        protected override LevelSettings GetDefaults()
        {
            var settings = new LevelSettings();
            settings.AddEntry("RunInfo", RunInfo);
            return settings;
        }

        protected override void Initialize(IControllerProvider state, LevelArgs arguments)
        {
            for (int i = 0; i < 100; i++)
            {
                CreateSoulRandomly(state);
            }

            var savedData = state.Using<IFactoryController>()
                .CreateAsset<RunLoader>("run");
            Batches.Add(savedData.FirstPart);
            Batches.Add(savedData.SecondPart);
            Batches.Add(savedData.ThirdPart);

            Tint = state.Using<IFactoryController>()
                .CreateObject<Image>()
                .SetMonoColor(Color.White)
                .SetOpacity(0)
                .SetPlacement(new Placement<TintLayer>())
                .SetScale(new Vector2(1920, 1080))
                .AddToState(state);

            NameLabel = state.Using<IFactoryController>()
                .CreateObject<DynamicLabel>()
                .SetPlacement(new Placement<GUI>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.Center)
                .SetText(() => $"{NewName}")
                .SetPos(new Vector2(WindowWidth/2, WindowHeight/2))
                .AddComponent(new VisualShake())
                .AddToState(state);

            DescriptionLabel = state.Using<IFactoryController>()
                .CreateObject<DynamicLabel>()
                .SetPlacement(new Placement<GUI>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.Center)
                .SetText(() => $"{Build.GetDescription()}")
                .SetScale(0.7f)
                .SetPos(new Vector2(WindowWidth / 2, WindowHeight / 2 + 200))
                .AddToState(state);

            PartLabel = state.Using<IFactoryController>()
                .CreateObject<DynamicLabel>()
                .SetPlacement(new Placement<GUI>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.CenterLeft)
                .SetText(() => $"{(HidePartLabel? "" : Batches[NameSelectionStep][SelectionIndex])}")
                .SetPos(new Vector2(WindowWidth / 2, WindowHeight / 2 + 100))
                .SetColor(Color.Gray)
                .AddToState(state);

            DescriptionContinuation = state.Using<IFactoryController>()
                .CreateObject<DynamicLabel>()
                .SetPlacement(new Placement<GUI>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.Center)
                .SetText(() =>
                {
                    if (NameSelectionStep == 0)
                    {
                        return "-" + BuildHelper.GetDescription(BuildHelper.FromNamePart<ChosenWeapon>(Batches[NameSelectionStep][SelectionIndex]));
                    }
                    else if (NameSelectionStep == 1)
                    {
                        return "-" + BuildHelper.GetDescription(BuildHelper.FromNamePart<ChosenElement>(Batches[NameSelectionStep][SelectionIndex]));
                    }
                    else if (NameSelectionStep == 2)
                    {
                        return "-" + BuildHelper.GetDescription(BuildHelper.FromNamePart<ChosenAbility>(Batches[NameSelectionStep][SelectionIndex]));
                    }
                    else
                    {
                        return string.Empty;
                    }
                })
                .SetScale(0.7f)
                .SetPos(new Vector2(WindowWidth / 2, WindowHeight / 2 + 380))
                .SetColor(Color.Gray)
                .AddToState(state);

            MessageLabel = state.Using<IFactoryController>()
                .CreateObject<DynamicLabel>()
                .SetPlacement(new Placement<GUI>())
                .UseFont(state, "Caveat")
                .SetPivot(PivotPosition.Center)
                .SetText(() => $"{CurrentMessage}")
                .SetPos(new Vector2(WindowWidth / 2, WindowHeight / 2 - 200))
                .AddToState(state);
        }

        protected override void OnClientUpdate(IControllerProvider state, GameClient client)
        {
            Tint.SetScale(client.Window.Size.ToVector2());
        }

        protected override void OnConnect(IControllerProvider state, GameClient client)
        {
            state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Music", "Star"), "main").Play();
            WindowWidth = client.Window.Width;
            WindowHeight = client.Window.Height;
            var physics = new Physics(new SurfaceMap([], 0, 16));
            var cameraAnchor = state.Using<IFactoryController>()
                .CreateObject<CameraAnchor>()
                .SetPos(new Vector2(WindowWidth/2, WindowHeight/2))
                .AddComponent(physics)
                .AddToState(state);
            state.Using<IClientController>().AttachCamera(client, cameraAnchor);
            Anchor = cameraAnchor;
            MainClient = client;
        }

        protected override void OnDisconnect(IControllerProvider state, GameClient client)
        {
            state.Using<ISoundController>().GetSoundInstance("main").Stop();
        }

        protected override void Update(IControllerProvider state, TimeSpan deltaTime)
        {
            if (OnTransition)
            {
                Tint.Opacity += 0.3f * Convert.ToSingle(deltaTime.TotalSeconds);
                if (Tint.Opacity >= 1)
                {
                    state.Using<ILevelController>().ShutCurrent(false);
                    state.Using<ILevelController>().LaunchLevel("test", new LevelArgs(NewName, JsonSerializer.Serialize(Build)), false);
                }
                return;
            }

            var physics = Anchor.GetComponents<Physics>().First();

            if (physics.ActiveVectors.ContainsKey("move"))
            {
                HidePartLabel = true;
                return;
            }

            HidePartLabel = false;

            if (MainClient.Controls.OnPress(Control.jump))
            {
                if (NameSelectionStep >= Batches.Count)
                {
                    OnTransition = true;
                    RunInfo.CharacterName = NewName;
                }
                else if (NameSelectionStep >= Batches.Count - 1)
                {
                    NewName = Regex.Replace($"{NewName}{Batches[NameSelectionStep][SelectionIndex]}", "-", "");
                    PartLabel.Dispose();
                    Build[NameSelectionStep] = Batches[NameSelectionStep][SelectionIndex];
                    NameSelectionStep += 1;
                    physics.AddVector("move", new MovementVector(new Vector2(0, -7), -3, TimeSpan.FromSeconds(3), true));
                    CurrentMessage = "Меня точно так зовут?";
                    state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "crystal_bell2"), "sound").Play();
                    NameLabel.InvokeEach<VisualShake>(it => it.Start(6, TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.08), 18, 0.3f));
                }
                else
                {
                    NewName = $"{NewName}{Batches[NameSelectionStep][SelectionIndex]}-";
                    Build[NameSelectionStep] = Batches[NameSelectionStep][SelectionIndex];
                    NameSelectionStep += 1;
                    SelectionIndex = 0;
                    physics.AddVector("move", new MovementVector(new Vector2(0, -7), -3, TimeSpan.FromSeconds(3), true));
                    state.Using<ISoundController>().CreateSoundInstance(Path.Combine("Sound", "crystal_bell3"), "sound").Play();
                    NameLabel.InvokeEach<VisualShake>(it => it.Start(6, TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.08), 18, 0.3f));
                }
            }

            if (NameSelectionStep >= Batches.Count)
            {
                return;
            }

            if (MainClient.Controls.OnPress(Control.lookUp))
            {
                SelectionIndex = Math.Clamp(SelectionIndex - 1, 0, Batches[NameSelectionStep].Length - 1);
            }
            if (MainClient.Controls.OnPress(Control.lookDown))
            {
                SelectionIndex = Math.Clamp(SelectionIndex + 1, 0, Batches[NameSelectionStep].Length - 1);
            }

            PartLabel.SetPos(new Vector2(NameLabel.GetLayout().Right, NameLabel.Position.Y));
        }

        private void CreateSoul(IControllerProvider state, float scale, string animation, IPlacement placement, Vector2 position, float opacity,
            float sineSpeedFactor, float sineScaleFactor)
        {
            var sine = new SizeSine<Soul>(sineScaleFactor, sineSpeedFactor);
            var soul = state.Using<IFactoryController>()
                .CreateObject<Soul>()
                .SetPlacement(placement)
                .SetPos(position)
                .SetScale(scale)
                .SetOpacity(opacity)
                .AddComponent(sine)
                .AddToState(state);

            soul.Animator.SetAnimation(animation, 0);
        }

        private bool HidePartLabel = false;
        private float WindowWidth = 1920;
        private float WindowHeight = 1080;
        private const int AnimationCount = 4;
        private const float MinScale = 0.001f;
        private const float MaxScale = 0.02f;
        private const float MinDistance = 550f;
        private const float MaxDistance = 700f;
        private const float DistanceFactor = 0.5f;
        private const float PlacementScaleFactor = 6f;
        private const float MinPlacementScale = 0.1f;
        private const float MinSineSpeed = 1f;
        private const float MaxSineSpeed = 2f;
        private const float SineSpeedFactor = 1f;
        private const float MinSineScale = 1;
        private const float MaxSineScale = 1.4f;
        private const float SineScaleFactor = 1f;
        private void CreateSoulRandomly(IControllerProvider state)
        {
            var placements = new IPlacement[] {
                new Placement<OneAndHalfParalaxLayer>(),
                new Placement<OneParalaxLayer>(),
                new Placement<HalfParalaxLayer>(),
                new Placement<ThirdParalaxLayer>(),
                new Placement<QuarterParalaxLayer>(),
                new Placement<ZeroParalaxLayer>()
            };
            var random = new RandomEx();
            var animation = $"star{random.Next(1, 1 + AnimationCount)}";
            var distance = random.NextSingle(MinDistance, MaxDistance, DistanceFactor);
            var position = MathEx.AngleToVector(random.NextSingle() * float.Pi * 2) * distance + new Vector2(WindowHeight/2, WindowHeight/2);
            var placementIndex = random.Next(placements.Length - 1);
            var placement = placements[placementIndex];

            var placementScale = MinPlacementScale + ((1 - (Convert.ToSingle(placementIndex) / (placements.Length - 1))) * (1 - MinPlacementScale));

            var opacity = placementScale;
            var scale = (MinScale + random.NextSingle() * (MaxScale - MinScale)) * placementScale * PlacementScaleFactor;

            var sineSpeed = random.NextSingle(MinSineSpeed, MaxSineSpeed, SineSpeedFactor);
            var sineAmp = random.NextSingle(MinSineScale, MaxSineScale, SineScaleFactor);

            CreateSoul(state, scale, animation, placement, position, opacity, sineSpeed, sineAmp);
        }
    }
}
