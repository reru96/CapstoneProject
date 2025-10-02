using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CameraFadeWall : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public LayerMask wallMask;
    public float transparentAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private List<Renderer> fadedWalls = new List<Renderer>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
    private Transform playerTransform;

    private void OnEnable()
    {
       
        if (RespawnManager.Instance != null)
        {
            TrySetPlayer(RespawnManager.Instance.Player);
        }
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