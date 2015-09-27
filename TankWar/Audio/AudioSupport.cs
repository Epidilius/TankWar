using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using GraphicsSupport;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Audio
{
    static public class AudioSupport
    {
        private static Dictionary<string, SoundEffect> sAudioEffects = new Dictionary<string,SoundEffect>();

        private static SoundEffectInstance sBackgroundAudio = null;

        static public void PlayACue(string aCueName)
        {
            SoundEffect sound = FindAudioClip(aCueName);

            if(sound != null)
            {
                sound.Play(); // Once it starts, it doesn't stop
            }
        }

        static public void PlayBackgroundAudio(string aBGAudio, float aLevel)
        {
            StopBG();
            if(aBGAudio != "" || aBGAudio != null)
            {
                aLevel = MathHelper.Clamp(aLevel, 0f, 1f);
                StartBG(aBGAudio, aLevel);
            }
        }

        static private SoundEffect FindAudioClip(string aName)
        {
            SoundEffect sound = null;
            if(sAudioEffects.ContainsKey(aName))
            {
                sound = sAudioEffects[aName];
            }
            else
            {
                sound = TankWar.Game1.sContent.Load<SoundEffect>(aName);
                if(sound != null)
                    sAudioEffects.Add(aName, sound);
            }

            return sound;
        }

        static private void StartBG(string aName, float aLevel)
        {
            SoundEffect bgm = FindAudioClip(aName);
            sBackgroundAudio = bgm.CreateInstance();
            sBackgroundAudio.IsLooped = true;
            sBackgroundAudio.Volume = aLevel;
            sBackgroundAudio.Play();
        }

        static private void StopBG()
        {
            if(sBackgroundAudio != null)
            {
                sBackgroundAudio.Pause();
                sBackgroundAudio.Stop();
                sBackgroundAudio.Volume = 0f;
                sBackgroundAudio.Dispose();
            }
            sBackgroundAudio = null;
        }


    }
}
