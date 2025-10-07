using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : EnemyBaseState
{
    private float stunDuration = 0.8f;
    private float timer;

    private Color originalColor;
    private Renderer[] renderers;
    private ParticleSystem hitEffect;

    public HitState(EnemyStateMachine enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.agent.isStopped = true;
        enemy.anim.Play("Hit");
        timer = stunDuration;

      
        renderers = enemy.GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
            foreach (var r in renderers)
                r.material.color = Color.red;
        }

       
        if (enemy.hitEffectPrefab != null)
        {
            hitEffect = GameObject.Instantiate(
                enemy.hitEffectPrefab,
                enemy.transform.position + Vector3.up * 1.2f,
                Quaternion.identity
            );

            GameObject.Destroy(hitEffect.gameObject, 2f);
        }
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;

        if (enemy.canSeePlayerNow)
            enemy.lastSeenPosition = enemy.targetPlayer.position;

        if (timer <= 0f)
        {
            if (enemy.canSeePlayerNow)
                enemy.SwitchState(new ChasingState(enemy));
            else
                enemy.SwitchState(new PatrollingState(enemy));
        }
    }

    public override void Exit()
    {
       
        if (renderers != null)
        {
            foreach (var r in renderers)
                r.material.color = originalColor;
        }

        enemy.agent.isStopped = false;
    }
}
