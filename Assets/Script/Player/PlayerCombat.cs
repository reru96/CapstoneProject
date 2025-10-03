using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    private int attackStep = 0;


    private Queue<string> actionQueue = new Queue<string>();
    private bool isPerformingAction = false;

    private void Start()
    {
        attackStep = 0;
    }
    void Update()
    {
        HandleInput();
        ProcessQueue();
        if (isPerformingAction)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

          
            if (!stateInfo.IsTag("Attack") && !stateInfo.IsTag("Ability"))
            {
                OnActionEnd();
            }
        }

    }

    void HandleInput()
    {

        if (Input.GetKeyDown(InputManager.Instance.config.attack))
        {
            actionQueue.Enqueue("Attack");
        }

 
        if (Input.GetKeyDown(InputManager.Instance.config.dodge))
        {
            actionQueue.Clear();
            CancelAttack();
            animator.SetTrigger("Dodge");
        }

  
        if (Input.GetKeyDown(InputManager.Instance.config.ability_1))
        {
            actionQueue.Enqueue("Ability1");
        }
    }

    void ProcessQueue()
    {
        if (isPerformingAction || actionQueue.Count == 0) return;

        string nextAction = actionQueue.Dequeue();

        switch (nextAction)
        {
            case "Attack":
                animator.SetTrigger(GetAttackTrigger());
                break;
            case "Ability1":
                animator.SetTrigger("Ability1");
                break;
        }

        isPerformingAction = true;
    }


    string GetAttackTrigger()
    {
        attackStep++;
        if (attackStep > 3) attackStep = 1;
        return $"Attack{attackStep}";
    }


    public void OnActionEnd()
    {
        isPerformingAction = false;
     
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsTag("Attack")) attackStep = 0;
    }
   
    void CancelAttack()
    {
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
        isPerformingAction = false;
        attackStep = 0;
    }
}
