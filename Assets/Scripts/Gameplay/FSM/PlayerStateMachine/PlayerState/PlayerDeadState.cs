using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeadState : PlayerBaseState
{

    private readonly float deathAnimDuration = 3f;

    public PlayerDeadState(PlayerStateMachine player) : base(player)
    {

    }

    public override void Enter()
    {

        player.isInvincible = true;
        player.isMoving = false;
        player.isAttacking = false;
        player.p_stats.ResetStats();

        if (player.agent != null)
            player.agent.isStopped = true;

        if (player.animator != null)
            player.animator.Play("Die");

        player.StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathAnimDuration);

        SceneManager.LoadScene("Level1");
    }

    public override void Exit()
    {
        player.isInvincible = false;
    }
}
