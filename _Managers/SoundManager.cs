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
        private Dictionary<string, bool> soundInstancesPauseFactors;
        private Dictionary<string, float> soundBaseVolumes;
        private List<SoundEffectInstance> activeSoundInstances = new List<SoundEffectInstance>();

        private Song currentSong;
        private string currentSongName;

        private float globalSoundEffectVolume = 1f;


        public SoundManager()
        {
            soundEffects = new Dictionary<string, SoundEffect>();
            soundInstances = new Dictionary<string, SoundEffectInstance>();
            soundInstancesPauseFactors = new Dictionary<string, bool> ();
            soundBaseVolumes = new Dictionary<string, float>();
        }

        public void LoadSound(string soundName, SoundEffect soundEffect, float baseVolume = 1f)
        {
            if (!soundEffects.ContainsKey(soundName))
            {
                soundEffects.Add(soundName, soundEffect);
                var instance = soundEffect.CreateInstance();
                instance.Volume = baseVolume * globalSoundEffectVolume;
                soundInstances.Add(soundName, instance);
                soundInstancesPauseFactors.Add(soundName, false);
                soundBaseVolumes.Add(soundName, MathHelper.Clamp(baseVolume, 0f, 1f));
            }
        }

        public void PlaySound(string soundName, bool loop = false)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                var instance = soundInstances[soundName];
                instance.IsLooped = loop;
                instance.Volume = soundBaseVolumes[soundName] * globalSoundEffectVolume;
                soundInstancesPauseFactors[soundName] = true;
                instance.Play();
            }
        }

        public void PlaySoundCanStack(string soundName, bool loop = false)
        {
            activeSoundInstances.RemoveAll(instance => instance.State == SoundState.Stopped);

            const int maxActiveSounds = 15;

            if (activeSoundInstances.Count < maxActiveSounds)
            {
                if (soundInstances.ContainsKey(soundName))
                {
                    var soundEffect = soundEffects[soundName];
                    var instance = soundEffect.CreateInstance();
                    instance.IsLooped = loop;
                    instance.Volume = soundBaseVolumes[soundName] * globalSoundEffectVolume;
                    instance.Play();
                    activeSoundInstances.Add(instance);
                }
            }
            else
            {
                ReplaceOldestSound(soundName, loop);
            }

            activeSoundInstances.RemoveAll(instance => instance.State == SoundState.Stopped);
        }

        private void ReplaceOldestSound(string soundName, bool loop)
        {
            var oldestInstance = activeSoundInstances[0];
            oldestInstance.Stop();
            activeSoundInstances.RemoveAt(0);

            if (soundInstances.ContainsKey(soundName))
            {
                var soundEffect = soundEffects[soundName];
                var newInstance = soundEffect.CreateInstance();
                newInstance.IsLooped = loop;
                newInstance.Volume = soundBaseVolumes[soundName] * globalSoundEffectVolume;
                newInstance.Play();
                activeSoundInstances.Add(newInstance);
            }
        }

        public void PauseSound(string soundName)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                soundInstances[soundName].Pause();
            }
        }

        public void PauseAllSound()
        {
            foreach (var sound in soundInstances.Values)
            {
                sound.Pause();
            }
            foreach (var sound in activeSoundInstances)
            {
                sound.Pause();
            }
        }
        public void PauseAllSoundExcept(string name)
        {
            foreach (var sound in soundInstances.Keys)
            {
                if (!sound.Equals(name))
                {
                    soundInstances[sound].Pause();
                }
            }
            foreach (var sound in activeSoundInstances)
            {
                sound.Pause();
            }
        }
        public void ResumeSound(string soundName)
        {
            if (soundInstances.ContainsKey(soundName))
            {
                soundInstances[soundName].Resume();
            }
        }
        public void ResumeAllActiveInstanceSound()
        {
            foreach (var sound in activeSoundInstances)
            {
                sound.Resume();
            }
        }

        public void ResumeAllSound()
        {
            foreach (var sound in soundInstances)
            {
                if (soundInstancesPauseFactors[sound.Key] == true)
                {
                    sound.Value.Resume();
                }
            }
        }

        public void SoundReset()
        {
            foreach (var sound in soundInstances.Values)
            {
                sound.Stop();
            }
            foreach (var condition in soundInstancesPauseFactors)
            {
                soundInstancesPauseFactors[condition.Key] = false;
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
                soundBaseVolumes[soundName] = MathHelper.Clamp(volume, 0f, 1f);
                soundInstances[soundName].Volume = soundBaseVolumes[soundName] * globalSoundEffectVolume;
            }
        }

        public void StopAllSounds()
        {
            foreach (var sound in soundInstances.Values)
            {
                sound.Stop();
            }
            foreach (var sound in activeSoundInstances)
            {
                sound.Stop();
            }
        }

        public void PlaySong(string songName, Song song, bool loop = true)
        {
            if (currentSong != null && MediaPlayer.State == MediaState.Playing && currentSongName == songName)
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
        public void SetGlobalSoundEffectVolume(float volume)
        {
            globalSoundEffectVolume = MathHelper.Clamp(volume, 0f, 1f);

            foreach (var soundName in soundInstances.Keys)
            {
                var instance = soundInstances[soundName];
                instance.Volume = soundBaseVolumes[soundName] * globalSoundEffectVolume;
            }

            foreach (var instance in activeSoundInstances)
            {
                instance.Volume = MathHelper.Clamp(instance.Volume, 0f, 1f) * globalSoundEffectVolume;
            }
        }
    }
}
