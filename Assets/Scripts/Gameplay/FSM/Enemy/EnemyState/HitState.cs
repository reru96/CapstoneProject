using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HitState : EnemyBaseState
{
    private float stunDuration = 1f;
    private float timer;

    private Color originalColor;
    private Renderer[] renderers;
    private ParticleSystem hitEffect;

    private Vector3 knockbackDir;
    private float knockbackForce = 2f; 
    private float knockbackDuration = 0.2f;

    public HitState(EnemyStateMachine enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.agent.isStopped = true;
        enemy.anim.Play("Hit");

        timer = stunDuration;

        if (enemy.targetPlayer != null)
            knockbackDir = (enemy.transform.position - enemy.targetPlayer.position).normalized;
        else
            knockbackDir = -enemy.transform.forward;

        enemy.StartCoroutine(DoKnockback(knockbackDir));

        renderers = enemy.GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
            foreach (var r in renderers)
                r.material.color = Color.red;
        }

        if (enemy.hitEffectPrefab != null)
        {
        
            Vector3 dirFromPlayer = (enemy.transform.position - enemy.targetPlayer.position).normalized;

        
            Quaternion rotation = Quaternion.LookRotation(dirFromPlayer, Vector3.up);

            hitEffect = GameObject.Instantiate(
                enemy.hitEffectPrefab,
                enemy.transform.position + Vector3.up * 1.2f,
                rotation
            );

            GameObject.Destroy(hitEffect.gameObject, 0.5f);
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

  
    private System.Collections.IEnumerator DoKnockback(Vector3 dir)
    {
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = startPos + dir * knockbackForce;

        if (NavMesh.SamplePosition(endPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            endPos = hit.position;

        float t = 0f;
        while (t < knockbackDuration)
        {
            t += Time.deltaTime;
            float progress = t / knockbackDuration;
            enemy.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, progress));
            yield return null;
        }
    }
}
