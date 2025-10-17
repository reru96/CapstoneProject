using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaController : MonoBehaviour
{
    public float maxStamina;
    public float currentStamina;
    public float regenerationRate; 
    public float regenDelay = 1f;        

    private float regenTimer = 0f;

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        
        if (currentStamina < maxStamina)
        {
            if (regenTimer <= 0f)
            {
                AddStamina(regenerationRate * Time.deltaTime);
            }
            else
            {
                regenTimer -= Time.deltaTime;
            }
        }
    }

    public void SetStamina(float amount)
    {
        currentStamina = Mathf.Clamp(amount, 0f, maxStamina);
    }

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public void ConsumeStaminaEvenIfInsufficient(float amount)
    {
        currentStamina -= amount;       
        regenTimer = regenDelay;        
    }

    public bool ConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            ConsumeStaminaEvenIfInsufficient(amount);
            return true;
        }
        return false;
    }

    public float GetStaminaPercentage()
    {
        return Mathf.Clamp01(currentStamina / maxStamina);
    }
}
