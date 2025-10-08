using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

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
    private Transform playerTransform;

    

    private void Start()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

       var respawnManager = Container.Resolver.Resolve<RespawnManager>();
        if (respawnManager != null)
            respawnManager.OnPlayerSpawned += TrySetPlayer;
        else
            Debug.LogWarning("[CameraFadeWall] RespawnManager non trovato nel Container.");
    }

    private void OnDestroy()
    {
        var respawnManager = Container.Resolver.Resolve<RespawnManager>();
        if (respawnManager != null)
            respawnManager.OnPlayerSpawned -= TrySetPlayer;
    }

    private void Update()
    {
        if (playerTransform == null) return;

      
        foreach (var rend in fadedWalls)
        {
            if (rend != null && originalColors.ContainsKey(rend))
            {
                Color c = rend.material.color;
                c.a = Mathf.Lerp(c.a, originalColors[rend].a, Time.deltaTime * fadeSpeed);
                rend.material.color = c;
            }
        }
        fadedWalls.Clear();

      
        Vector3 dir = playerTransform.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray, dir.magnitude, wallMask);

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                if (!originalColors.ContainsKey(rend))
                    originalColors[rend] = rend.material.color;

                Color c = rend.material.color;
                c.a = Mathf.Lerp(c.a, transparentAlpha, Time.deltaTime * fadeSpeed);
                rend.material.color = c;

                fadedWalls.Add(rend);
            }
        }
    }

    public void TrySetPlayer(GameObject player)
    {
        if (player == null) return;

        playerTransform = player.transform;

        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerTransform;
            virtualCamera.LookAt = playerTransform;
        }
    }
}