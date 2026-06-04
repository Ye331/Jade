using System.Collections.Generic;
using UnityEngine;

namespace Jade.Audio
{
    public static class GameAudio2D
    {
        private const string BgmResourcePath = "Audio/BGM/game_bgm2";
        public const string JumpResourcePath = "Audio/SFX/jump";
        public const string LandResourcePath = "Audio/SFX/landed";
        public const string CoinPickResourcePath = "Audio/SFX/pick_coins";
        public const string SkillPickResourcePath = "Audio/SFX/pick_skills";
        public const string DashResourcePath = "Audio/SFX/rush";
        public const string FootstepResourcePath = "Audio/SFX/walk8";

        private static readonly Dictionary<string, AudioClip> ClipCache = new Dictionary<string, AudioClip>();
        private static AudioSource bgmSource;
        private static AudioSource sfxSource;
        private static bool isBgmPaused;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            PreloadSfx();
            StartBgm();
        }

        private static void PreloadSfx()
        {
            LoadClip(JumpResourcePath);
            LoadClip(LandResourcePath);
            LoadClip(CoinPickResourcePath);
            LoadClip(SkillPickResourcePath);
            LoadClip(DashResourcePath);
            LoadClip(FootstepResourcePath);
            EnsureSfxSource();
        }

        private static void StartBgm()
        {
            if (bgmSource != null)
            {
                return;
            }

            AudioClip clip = Resources.Load<AudioClip>(BgmResourcePath);
            if (clip == null)
            {
                return;
            }

            GameObject go = new GameObject("GameplayBgm");
            Object.DontDestroyOnLoad(go);
            bgmSource = go.AddComponent<AudioSource>();
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.spatialBlend = 0f;
            bgmSource.volume = 0.35f;
            if (isBgmPaused)
            {
                bgmSource.Pause();
            }
            else
            {
                bgmSource.Play();
            }
        }

        public static void PauseBgm()
        {
            isBgmPaused = true;
            StartBgm();

            if (bgmSource != null && bgmSource.isPlaying)
            {
                bgmSource.Pause();
            }
        }

        public static void ResumeBgm()
        {
            isBgmPaused = false;
            StartBgm();

            if (bgmSource == null || bgmSource.isPlaying)
            {
                return;
            }

            bgmSource.UnPause();
            if (!bgmSource.isPlaying)
            {
                bgmSource.Play();
            }
        }

        public static AudioClip LoadClip(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return null;
            }

            if (ClipCache.TryGetValue(resourcePath, out AudioClip cachedClip))
            {
                return cachedClip;
            }

            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip != null)
            {
                if (clip.loadState == AudioDataLoadState.Unloaded)
                {
                    clip.LoadAudioData();
                }

                ClipCache[resourcePath] = clip;
            }

            return clip;
        }

        public static void PlayOneShot2D(string resourcePath, float volume)
        {
            AudioClip clip = LoadClip(resourcePath);
            if (clip == null)
            {
                return;
            }

            PlayOneShot2D(clip, volume);
        }

        public static void PlayOneShot2D(AudioClip clip, float volume)
        {
            if (clip == null || volume <= 0f)
            {
                return;
            }

            EnsureSfxSource().PlayOneShot(clip, volume);
        }

        private static AudioSource EnsureSfxSource()
        {
            if (sfxSource != null)
            {
                return sfxSource;
            }

            GameObject go = new GameObject("GameplaySfx");
            Object.DontDestroyOnLoad(go);
            sfxSource = go.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
            sfxSource.dopplerLevel = 0f;
            return sfxSource;
        }
    }
}
