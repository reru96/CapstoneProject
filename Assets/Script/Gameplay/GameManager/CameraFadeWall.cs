using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    private Transform playerTransform;

    private void Start()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        StartCoroutine(FindPlayerRoutine());
    }

    private IEnumerator FindPlayerRoutine()
    {
        GameObject player = null;

        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                TrySetPlayer(player);
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void TrySetPlayer(GameObject player)
    {
        if (player == null) return;

        playerTransform = player.transform;

        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerTransform;
            virtualCamera.LookAt = playerTransform;
            virtualCamera.gameObject.SetActive(true);

            Debug.Log("[CameraFadeWall] Player assegnato alla VirtualCamera.");
        }
        else
        {
            Debug.LogWarning("[CameraFadeWall] Nessuna VirtualCamera trovata!");
        }
    }

    //private void Update()
    //{
    //    if (playerTransform == null) return;


    //    foreach (var rend in fadedWalls)
    //    {
    //        if (rend != null && originalColors.ContainsKey(rend))
    //        {
    //            Color c = rend.material.color;
    //            c.a = Mathf.Lerp(c.a, originalColors[rend].a, Time.deltaTime * fadeSpeed);
    //            rend.material.color = c;
    //        }
    //    }
    //    fadedWalls.Clear();


    //    Vector3 dir = playerTransform.position - transform.position;
    //    Ray ray = new Ray(transform.position, dir);
    //    RaycastHit[] hits = Physics.RaycastAll(ray, dir.magnitude, wallMask);

    //    foreach (var hit in hits)
    //    {
    //        Renderer rend = hit.collider.GetComponent<Renderer>();
    //        if (rend != null)
    //        {
    //            if (!originalColors.ContainsKey(rend))
    //                originalColors[rend] = rend.material.color;

    //            Color c = rend.material.color;
    //            c.a = Mathf.Lerp(c.a, transparentAlpha, Time.deltaTime * fadeSpeed);
    //            rend.material.color = c;

    //            fadedWalls.Add(rend);
    //        }
    //    }
    //}

}