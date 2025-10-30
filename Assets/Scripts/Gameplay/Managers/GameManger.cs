using Core;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

namespace Gameplay
{
    public class GameManager : Injectable<GameManager>
    {
        [Header("Screen Fader")]
        [SerializeField] private CanvasGroup screenFader;
        [SerializeField] private float fadeDuration = 1f;

        public int Coins { get; private set; } = 0;

        public static event Action<int> OnCoinsChanged;
        public static event Action<string> OnSceneLoaded;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            if (FindObjectsOfType<GameManager>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            if (screenFader == null)
            {
                GameObject faderObj = new GameObject("ScreenFader");
                faderObj.transform.SetParent(transform);
                screenFader = faderObj.AddComponent<CanvasGroup>();
                var canvas = faderObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                faderObj.AddComponent<UnityEngine.UI.Image>().color = Color.black;
                screenFader.alpha = 0f;
            }

            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FadeIn();
            OnSceneLoaded?.Invoke(scene.name);
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

        public void SetCoins(int amount)
        {
            Coins = amount;
            OnCoinsChanged?.Invoke(Coins);
            Debug.Log("[GameManager] Coins resettati");
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            yield return FadeOut();
            yield return new WaitForSeconds(0.1f);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone)
                yield return null;
            FadeIn();
        }
        public void FadeIn()
        {
            if (screenFader == null) return;

            screenFader.gameObject.SetActive(true);
            screenFader.alpha = 1f;
            screenFader.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                screenFader.gameObject.SetActive(false);
            });
        }

        public IEnumerator FadeOut()
        {
            if (screenFader == null)
                yield break;

            screenFader.gameObject.SetActive(true);
            screenFader.alpha = 0f;

            yield return screenFader.DOFade(1f, fadeDuration).WaitForCompletion();
        }
        private void OnApplicationQuit()
        {
            var inventoryManager = ServiceLocator.Get<InventoryManager>();
            if (inventoryManager != null)
            {
                SaveSystem.SavePermanentInventory(inventoryManager.permanentInventory, Coins);
                Debug.Log($"[GameManager] Dati salvati con {Coins} coins.");
            }
        }

    }
}
