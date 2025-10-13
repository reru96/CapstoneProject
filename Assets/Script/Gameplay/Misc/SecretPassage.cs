using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.AI.Navigation;

public class SecretPassage : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float radius = 1.2f;
    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private string promptMessage = "Press E";

    [Header("Objects to Toggle")]
    [SerializeField] private GameObject[] enableOnPress;
    [SerializeField] private GameObject[] disableOnPress;

    [Header("NavMesh Surfaces to Rebuild")]
    [SerializeField] private NavMeshSurface[] surfacesToRebake;

    private Transform player;
    private bool inRange;

    private void OnEnable()
    {
      
        var respawnManager = CoreSystem.Instance.Container.Resolve<PlayerSpawnManager>();
        if (respawnManager != null)
        {
         
            respawnManager.OnPlayerSpawned += SetPlayer;

       
            if (respawnManager.Player != null)
                player = respawnManager.Player.transform;
        }

        if (promptText)
            promptText.enabled = false;
    }

    private void OnDestroy()
    {
        var respawnManager = CoreSystem.Instance.Container.Resolve<PlayerSpawnManager>();
        if (respawnManager != null)
            respawnManager.OnPlayerSpawned -= SetPlayer;
    }

    private void Update()
    {
        if (player == null) return; 

        inRange = Vector3.SqrMagnitude(player.position - transform.position) <= radius * radius;

        if (promptText)
        {
            promptText.text = promptMessage;
            promptText.enabled = inRange;
        }

        if (inRange && Input.GetKeyDown(key))
            Activate();
    }

    private void SetPlayer(GameObject playerObj)
    {
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Activate()
    {
     
        foreach (var go in enableOnPress)
        {
            if (!go) continue;
            var obstacle = go.GetComponent<NavMeshObstacle>();
            if (obstacle) obstacle.enabled = false;
            go.SetActive(true);
        }

        foreach (var go in disableOnPress)
        {
            if (!go) continue;
            var obstacle = go.GetComponent<NavMeshObstacle>();
            if (obstacle) obstacle.enabled = false;
            go.SetActive(false);
        }

        foreach (var surface in surfacesToRebake)
            surface?.BuildNavMesh();

        if (promptText) promptText.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
