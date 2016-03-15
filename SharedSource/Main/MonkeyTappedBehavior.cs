using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace MonkeyTapWaveEngine
{
    [DataContract]
    public class MonkeyTappedBehavior : Behavior
    {
        private TimeSpan countDown;
        [RequiredComponent]
        private TouchGestures gestures;
        private SoundInfo hitSound;

        protected override void Initialize()
        {
            base.Initialize();

            this.hitSound = new SoundInfo(WaveContent.Assets.hit_wav);
            var soundBank = new SoundBank();
            soundBank.Add(this.hitSound);
            WaveServices.SoundPlayer.RegisterSoundBank(soundBank);

            gestures.TouchTap += Gestures_TouchTap;

            RestartCountdown();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (!this.Owner.IsVisible)
            {
                return;
            }

            this.countDown -= gameTime;

            if (this.countDown <= TimeSpan.Zero)
            {
                // Deactivating a monkey means he dies, so game over
                this.Owner.IsActive = false;
                this.RestartCountdown();
            }
        }

        private void RestartCountdown()
        {
            this.countDown = TimeSpan.FromSeconds(5);
        }

        private void Gestures_TouchTap(object sender, GestureEventArgs e)
        {
            this.Owner.IsVisible = false;
            (this.Owner.Scene as MyScene).Score++;

            WaveServices.SoundPlayer.Play(this.hitSound);
        }
    }
}
