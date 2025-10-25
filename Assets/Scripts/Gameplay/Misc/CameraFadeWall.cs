using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Core;
using Gameplay;
using UnityEngine;

public class CameraFadeWall : MonoBehaviour
{

    [Header("Cinemachine")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("Fade Settings")]
    public LayerMask wallMask;
    public float transparentAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private List<Renderer> fadedWalls = new List<Renderer>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private void Start()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        var playerSpawnManager = ServiceLocator.Get<PlayerSpawnManager>();
      
        if (playerSpawnManager != null)
        {
            playerSpawnManager.OnPlayerSpawned += OnPlayerSpawned;
        }
    }

    private void OnPlayerSpawned(GameObject player)
    {
        if (player == null) return;

        var playerSpawnManager = ServiceLocator.Get<PlayerSpawnManager>();
        player = playerSpawnManager.Player;

        if (virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
            virtualCamera.gameObject.SetActive(true);

            Debug.Log("[CameraFadeWall] Player assegnato alla VirtualCamera.");
        }
        else
        {
            Debug.LogWarning("[CameraFadeWall] Nessuna VirtualCamera trovata!");
        }
    }

    private void OnDestroy()
    {
        var playerSpawnManager = ServiceLocator.Get<PlayerSpawnManager>();
        if (playerSpawnManager != null)
            playerSpawnManager.OnPlayerSpawned -= OnPlayerSpawned;
    }


    //private void Update()
    //{
    //    FadeWalls();
    //}

    //private void FadeWalls()
    //{
    //    List<Renderer> hitRenderers = new List<Renderer>();

    //    var playerSpawnManager = ServiceLocator.Get<PlayerSpawnManager>();
    //    Transform playerTransform = playerSpawnManager.Player.transform;
    //    Vector3 dir = playerTransform.position - transform.position;
    //    Ray ray = new Ray(transform.position, dir);
    //    RaycastHit[] hits = Physics.RaycastAll(ray, dir.magnitude, wallMask);

    //    foreach (var hit in hits)
    //    {
    //        Renderer rend = hit.collider.GetComponent<Renderer>();
    //        if (rend == null) continue;

    //        hitRenderers.Add(rend);

    //        if (!originalColors.ContainsKey(rend))
    //            originalColors[rend] = rend.material.color;

    //        Color targetColor = originalColors[rend];
    //        targetColor.a = transparentAlpha;

    //        rend.material.color = Color.Lerp(rend.material.color, targetColor, Time.deltaTime * fadeSpeed);

    //        if (!fadedWalls.Contains(rend))
    //            fadedWalls.Add(rend);
    //    }

   
    //    for (int i = fadedWalls.Count - 1; i >= 0; i--)
    //    {
    //        Renderer rend = fadedWalls[i];
    //        if (!hitRenderers.Contains(rend))
    //        {
    //            if (originalColors.ContainsKey(rend))
    //            {
    //                Color targetColor = originalColors[rend];
    //                rend.material.color = Color.Lerp(rend.material.color, targetColor, Time.deltaTime * fadeSpeed);

              
    //                if (Mathf.Abs(rend.material.color.a - targetColor.a) < 0.01f)
    //                {
    //                    rend.material.color = targetColor;
    //                    fadedWalls.RemoveAt(i);
    //                    originalColors.Remove(rend);
    //                }
    //            }
    //        }
    //    }
    //}

}