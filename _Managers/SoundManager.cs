using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Advencursor._Managers
{
    public class SoundManager
    {
        private Dictionary<string, SoundEffect> soundEffects;
        private Dictionary<string, SoundEffectInstance> soundInstances;
        private List<SoundEffectInstance> activeSoundInstances = new List<SoundEffectInstance>();

        private Song currentSong;
        private string currentSongName;

        public SoundManager()
        {
            soundEffects = new Dictionary<string, SoundEffect>();
            soundInstances = new Dictionary<string, SoundEffectInstance>();
        }

        public void LoadSound(string soundName, SoundEffect soundEffect)
        {
            if (!soundEffects.ContainsKey(soundName))
            {
                soundEffects.Add(soundName, soundEffect);
                soundInstances.Add(soundName, soundEffect.CreateInstance());
            }
        }

        public void PlaySound(string soundName, bool loop = false)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                var instance = soundInstances[soundName];
                instance.IsLooped = loop;
                instance.Play();
            }
        }

        public void PlaySoundCanStack(string soundName, bool loop = false)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                var soundEffect = soundEffects[soundName];
                var instance = soundEffect.CreateInstance();
                instance.IsLooped = loop;
                instance.Play();
                activeSoundInstances.Add(instance);
            }
            activeSoundInstances.RemoveAll(instance => instance.State == SoundState.Stopped);
        }

        public void PauseSound(string soundName)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                soundInstances[soundName].Pause();
            }
        }

        public void ResumeSound(string soundName)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                soundInstances[soundName].Resume();
            }
        }

        public void StopSound(string soundName)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                soundInstances[soundName].Stop();
            }
        }

        public void SetVolume(string soundName, float volume)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                soundInstances[soundName].Volume = MathHelper.Clamp(volume, 0f, 1f);
            }
        }

        public void StopAllSounds()
        {
            foreach (var instance in soundInstances.Values)
            {
                instance.Stop();
            }
        }
        
        public void PlaySong(string songName, Song song, bool loop = true)
        {
            if (currentSong != null && currentSongName == songName)
            {
                return;
            }

            StopCurrentSong();

            currentSong = song;
            currentSongName = songName;

            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(song);
        }

        public void StopCurrentSong()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
                currentSong = null;
                currentSongName = null;
            }
        }

        public void SetSongVolume(float volume)
        {
            MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);
        }
    }
}
