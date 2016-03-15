#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace MonkeyTapWaveEngine
{
    public class MyScene : Scene
    {
        private Button startButton;
        private GameSceneBehavior gameSceneBehavior;
        private IEnumerable<Entity> monkeys;
        private StackPanel gameOverPanel;
        private TextBlock scoreTextBlock;
        private MusicInfo music;

        public int Score { get; internal set; }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MyScene);
            this.gameSceneBehavior = new GameSceneBehavior();
            this.AddSceneBehavior(this.gameSceneBehavior, SceneBehavior.Order.PreUpdate);

            this.monkeys = this.EntityManager.FindAllByTag("monkey")
                .Cast<Entity>();

            this.CreateTapToStart();
            this.CreateGameOverAndScore();

            this.music = new MusicInfo(WaveContent.Assets.title_mp3);
        }

        protected override void Start()
        {
            base.Start();

            WaveServices.MusicPlayer.Play(this.music);

            this.Score = 0;
        }

        internal void HideTapToStart()
        {
            this.startButton.IsVisible = false;
        }

        internal void ShowTapToStart()
        {
            this.startButton.IsVisible = true;
        }

        internal void HideEveryMonkeyAndReactivateThem()
        {
            foreach (var monkey in this.monkeys)
            {
                monkey.IsVisible = false;
                monkey.IsActive = true;
            }
        }

        internal void ShowOneMoreRandomMonkey()
        {
            var hiddenMonkeys = this.monkeys.Where(monkey => !monkey.IsVisible);
            var randomIndex = WaveServices.Random.Next(hiddenMonkeys.Count());
            hiddenMonkeys.ElementAt(randomIndex).IsVisible = true;
        }

        internal bool CheckAnyMonkeyDied()
        {
            return this.monkeys.Any(monkey => !monkey.IsActive);
        }

        internal void ShowGameOverAndScore()
        {
            this.scoreTextBlock.Text = string.Format("Score: {0}", this.Score);
            this.gameOverPanel.IsVisible = true;
        }

        internal void HideGameOver()
        {
            this.gameOverPanel.IsVisible = false;
            this.Score = 0;
        }

        internal void DeactivateEveryMonkeyToAvoidWrongTaps()
        {
            foreach (var monkey in monkeys)
            {
                monkey.IsActive = false;
            }
        }

        private void CreateGameOverAndScore()
        {
            this.gameOverPanel = new StackPanel()
            {
                Margin = new Thickness(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            var gameOverTextBlock = new TextBlock()
            {
                Text = "Game Over",
                Foreground = Color.Red
            };
            this.gameOverPanel.Add(gameOverTextBlock);

            this.scoreTextBlock = new TextBlock()
            {
                Text = "Score: 0",
                Foreground = Color.White
            };
            this.gameOverPanel.Add(this.scoreTextBlock);

            this.HideGameOver();

            this.EntityManager.Add(this.gameOverPanel);
        }

        private void CreateTapToStart()
        {
            this.startButton = new Button()
            {
                Text = "Tap to Start",
                IsBorder = false,
                Margin = new Thickness(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsVisible = false
            };
            this.startButton.Click += (sender, args) => this.gameSceneBehavior.MoveToPlayingState();
            this.EntityManager.Add(this.startButton);
        }
    }
}
