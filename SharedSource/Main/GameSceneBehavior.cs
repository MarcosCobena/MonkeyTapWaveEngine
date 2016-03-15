using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace MonkeyTapWaveEngine
{
    class GameSceneBehavior : SceneBehavior
    {
        private GameState previousGameState, currentGameState;
        private MyScene scene;
        private TimeSpan nextMonkeyElapsedGameTime;

        private enum GameState
        {
            Start,
            Playing,
            GameOver
        }

        public GameSceneBehavior() : base()
        {
            // For the 1st time we suppose previous state was game over
            this.previousGameState = GameState.GameOver;
            this.currentGameState = GameState.Start;
        }

        protected override void ResolveDependencies()
        {
            this.scene = this.Scene as MyScene;
        }

        protected override void Update(TimeSpan gameTime)
        {
            switch (currentGameState)
            {
                case GameState.Start:
                    if (this.previousGameState == GameState.GameOver)
                    {
                        this.HideEveryMonkeyAndReactivateThem();
                        this.HideGameOver();
                        this.ShowTapToStart();
                    }

                    break;
                case GameState.Playing:
                    if (this.previousGameState == GameState.Playing)
                    {
                        this.HideTapToStart();
                    }

                    this.AddMonkeysWhileUserTapsOnPreviousOneInTime(gameTime);
                    break;
                case GameState.GameOver:
                    if (this.previousGameState == GameState.Playing)
                    {
                        this.DeactivateEveryMonkeyToAvoidWrongTaps();
                        this.ShowGameOverAndScore();
                    }

                    break;
                default:
                    break;
            }

            this.previousGameState = this.currentGameState;

            if (currentGameState == GameState.Playing)
            {
                this.CheckAnyMonkeyDiedForGameOver();
            }

            if (currentGameState == GameState.GameOver)
            {
                IfUserTapsAnywhereInScreenRestartGame();
            }
        }

        internal void MoveToPlayingState()
        {
            this.currentGameState = GameState.Playing;
        }

        private void DeactivateEveryMonkeyToAvoidWrongTaps()
        {
            this.scene.DeactivateEveryMonkeyToAvoidWrongTaps();
        }

        private void HideGameOver()
        {
            this.scene.HideGameOver();
        }

        private void IfUserTapsAnywhereInScreenRestartGame()
        {
            var touchState = WaveServices.Input.TouchPanelState;

            if (touchState.Count > 0)
            {
                this.currentGameState = GameState.Start;
            }
        }

        private void ShowGameOverAndScore()
        {
            this.scene.ShowGameOverAndScore();
        }

        private void CheckAnyMonkeyDiedForGameOver()
        {
            var anyDied = this.scene.CheckAnyMonkeyDied();

            if (anyDied)
            {
                this.currentGameState = GameState.GameOver;
            }
        }

        private void HideEveryMonkeyAndReactivateThem()
        {
            this.scene.HideEveryMonkeyAndReactivateThem();
        }

        private void AddMonkeysWhileUserTapsOnPreviousOneInTime(TimeSpan gameTime)
        {
            this.nextMonkeyElapsedGameTime += gameTime;

            if (this.nextMonkeyElapsedGameTime >= TimeSpan.FromSeconds(2))
            {
                this.scene.ShowOneMoreRandomMonkey();
                this.nextMonkeyElapsedGameTime = TimeSpan.Zero;
            }
        }

        private void HideTapToStart()
        {
            this.scene.HideTapToStart();
        }

        private void ShowTapToStart()
        {
            this.scene.ShowTapToStart();
        }
    }
}
