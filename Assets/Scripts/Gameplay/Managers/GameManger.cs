using Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Gameplay
{
    public class GameManager : Injectable<GameManager>
    {
        [Header("Screen Fader")]
        [SerializeField] private CanvasGroup screenFader;
        [SerializeField] private float fadeDuration = 1f;

        public int Coins { get; private set; } = 0;
        public static event Action<int> OnCoinsChanged;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip defaultClip;
        [SerializeField, Range(0f, 1f)] private float targetVolume = 1f;

        [SerializeField] private SceneAudioPair[] sceneAudioPairs;
        private Dictionary<string, AudioClip> _sceneAudioDict;

        [Serializable]
        private struct SceneAudioPair
        {
            public string sceneName;
            public AudioClip clip;
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            _sceneAudioDict = new Dictionary<string, AudioClip>();
            foreach (var pair in sceneAudioPairs)
            {
                if (!_sceneAudioDict.ContainsKey(pair.sceneName))
                    _sceneAudioDict.Add(pair.sceneName, pair.clip);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PlaySceneAudio(scene.name);
            FadeIn();
        }

        public void FadeIn()
        {
            if (screenFader == null) return;
            screenFader.alpha = 1f;
            screenFader.gameObject.SetActive(true);

            screenFader.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                screenFader.gameObject.SetActive(false);
            });
        }

        public void FadeOut(Action onComplete = null)
        {
            if (screenFader == null) return;
            screenFader.alpha = 0f;
            screenFader.gameObject.SetActive(true);

            screenFader.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void LoadScene(string sceneName)
        {
            FadeOut(() => SceneManager.LoadScene(sceneName));
        }
      
        private void PlaySceneAudio(string sceneName)
        {
            if (audioSource == null) return;

            AudioClip clipToPlay = defaultClip;
            if (_sceneAudioDict.TryGetValue(sceneName, out var clip) && clip != null)
                clipToPlay = clip;

            audioSource.clip = clipToPlay;
            audioSource.volume = 0f;
            audioSource.Play();

            audioSource.DOFade(targetVolume, fadeDuration);
        }

        public void PlayAudio(string clipName)
        {
            if (audioSource == null) return;
            audioSource.DOFade(0f, 0f); 
            audioSource.Play();
        }

        public void StopAudio()
        {
            if (audioSource == null) return;
            audioSource.DOFade(0f, fadeDuration).OnComplete(() => audioSource.Stop());
        }
      
        public void AddCoins(int amount)
        {
            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
            Debug.Log($"[GameManager] Coins: {Coins}");
        }

        public bool SpendCoins(int amount)
        {
            if (Coins < amount)
            {
                Debug.LogWarning("[GameManager] Non abbastanza coins!");
                return false;
            }
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }

        public void ResetCoins()
        {
            Coins = 0;
            OnCoinsChanged?.Invoke(Coins);
            Debug.Log("[GameManager] Coins resettati");
        }
       
    }
}
