using System;
using System.Collections.Generic;
using UnityEngine;


    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [System.Serializable]
        public class SoundAudioData
        {
            public SoundType soundType;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
        }

        [Header("--- AUDIO SOURCES ---")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("--- AUDIO CLIPS LIST ---")]
        [SerializeField] private List<SoundAudioData> soundList;

        private Dictionary<SoundType, SoundAudioData> soundDictionary;

        // PlayerPrefs Keys để lưu cài đặt Bật/Tắt âm thanh
        private const string MUTE_SFX_KEY = "TimberGame_MuteSFX";
        private const string MUTE_BGM_KEY = "TimberGame_MuteBGM";

        public bool IsSFXMuted { get; private set; }
        public bool IsBGMMuted { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitDictionary();
            LoadAudioSettings();
        }

        private void Start()
        {
            // Tự động bật Nhạc nền BGM khi bắt đầu Game
            PlayBGM(SoundType.BGM);
        }

        /// <summary>
        /// Chuyển List cấu hình trên Inspector thành Dictionary để truy xuất siêu nhanh O(1)
        /// </summary>
        private void InitDictionary()
        {
            soundDictionary = new Dictionary<SoundType, SoundAudioData>();
            foreach (var sound in soundList)
            {
                if (!soundDictionary.ContainsKey(sound.soundType))
                {
                    soundDictionary.Add(sound.soundType, sound);
                }
            }
        }

        #region --- PLAY AUDIO METHODS ---

        /// <summary>
        /// Phát hiệu ứng âm thanh SFX (Ví dụ: Chop, Lose, ButtonClick)
        /// </summary>
        public void PlaySFX(SoundType type)
        {
            if (IsSFXMuted) return;

            if (soundDictionary.TryGetValue(type, out SoundAudioData sound))
            {
                if (sound.clip != null && sfxSource != null)
                {
                    sfxSource.PlayOneShot(sound.clip, sound.volume);
                }
            }
        }

        /// <summary>
        /// Phát nhạc nền BGM (Tự động lặp lại)
        /// </summary>
        public void PlayBGM(SoundType type)
        {
            if (soundDictionary.TryGetValue(type, out SoundAudioData sound))
            {
                if (sound.clip != null && bgmSource != null)
                {
                    bgmSource.clip = sound.clip;
                    bgmSource.volume = sound.volume;
                    bgmSource.loop = true;

                    if (!IsBGMMuted)
                    {
                        bgmSource.Play();
                    }
                }
            }
        }

        #endregion

        #region --- SETTINGS & MUTE TOGGLE ---

        /// <summary>
        /// Bật/Tắt Âm thanh hiệu ứng (Gắn nút này vào Popup Settings)
        /// </summary>
        public void ToggleSFX()
        {
            IsSFXMuted = !IsSFXMuted;
            PlayerPrefs.SetInt(MUTE_SFX_KEY, IsSFXMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Bật/Tắt Nhạc nền (Gắn nút này vào Popup Settings)
        /// </summary>
        public void ToggleBGM()
        {
            IsBGMMuted = !IsBGMMuted;

            if (bgmSource != null)
            {
                bgmSource.mute = IsBGMMuted;
                if (IsBGMMuted) bgmSource.Pause();
                else if (!bgmSource.isPlaying) bgmSource.Play();
            }

            PlayerPrefs.SetInt(MUTE_BGM_KEY, IsBGMMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadAudioSettings()
        {
            IsSFXMuted = PlayerPrefs.GetInt(MUTE_SFX_KEY, 0) == 1;
            IsBGMMuted = PlayerPrefs.GetInt(MUTE_BGM_KEY, 0) == 1;

            if (bgmSource != null)
            {
                bgmSource.mute = IsBGMMuted;
            }
        }

        #endregion
    }
