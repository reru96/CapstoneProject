using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Core;
using Gameplay;
using UnityEngine;

public class CameraFadeWall : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        StartCoroutine(AssignCameraTarget());
    }

    private IEnumerator AssignCameraTarget()
    {
       
        PlayerSpawnManager playerManager = null;
        while (playerManager == null || playerManager.Player == null)
        {
            playerManager = ServiceLocator.Get<PlayerSpawnManager>();
            yield return null; 
        }

        virtualCamera.Follow = playerManager.Player.transform;
        virtualCamera.LookAt = playerManager.Player.transform;
        virtualCamera.Priority = 10; 
    }
}