using TankAssault.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace TankAssault.Audio
{
    /// <summary>
    /// Central audio hub: routes music/SFX through a mixer for independent volume control and
    /// exposes a pooled one-shot SFX player so weapons/explosions never allocate new AudioSources.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxOneShotPrefab;
        [SerializeField] private int sfxPoolSize = 16;
        [SerializeField] private string musicVolumeParam = "MusicVolume";
        [SerializeField] private string sfxVolumeParam = "SfxVolume";

        private AudioSource[] _sfxPool;
        private int _nextSfxIndex;

        protected override void Awake()
        {
            base.Awake();
            _sfxPool = new AudioSource[sfxPoolSize];
            for (int i = 0; i < sfxPoolSize; i++)
            {
                _sfxPool[i] = Instantiate(sfxOneShotPrefab, transform);
            }

            var save = SaveSystem.Instance.Current;
            SetMusicVolume(save.MusicVolume);
            SetSfxVolume(save.SfxVolume);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource == null || clip == null) return;
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void PlaySfx(AudioClip clip, Vector3 worldPosition, float volume = 1f)
        {
            if (clip == null) return;
            var source = _sfxPool[_nextSfxIndex];
            _nextSfxIndex = (_nextSfxIndex + 1) % _sfxPool.Length;

            source.transform.position = worldPosition;
            source.spatialBlend = 1f; // full 3D spatial audio
            source.volume = volume;
            source.PlayOneShot(clip);
        }

        public void SetMusicVolume(float linear01)
        {
            mixer?.SetFloat(musicVolumeParam, LinearToDecibel(linear01));
        }

        public void SetSfxVolume(float linear01)
        {
            mixer?.SetFloat(sfxVolumeParam, LinearToDecibel(linear01));
        }

        private static float LinearToDecibel(float linear01)
        {
            return linear01 <= 0.0001f ? -80f : Mathf.Log10(linear01) * 20f;
        }
    }
}
