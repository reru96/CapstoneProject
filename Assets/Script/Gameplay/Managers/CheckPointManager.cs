using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckpointManager : Injectable<CheckpointManager>
{
    private Transform _currentCheckpoint;

    public void SaveCheckpoint(Transform checkpoint)
    {
        _currentCheckpoint = checkpoint;
        var audio = Resolve<AudioManager>();
        audio?.PlaySfx("Checkpoint_Saved");
    }

    public void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        var spawnMgr = Resolve<PlayerSpawnManager>();
        var player = spawnMgr.Player;
        if (player == null) return;

        if (NavMesh.SamplePosition(position, out var hit, 2f, NavMesh.AllAreas))
        {
            player.transform.position = hit.position;
            player.transform.rotation = rotation;
        }
    }

    public Transform GetCheckpoint() => _currentCheckpoint;
}