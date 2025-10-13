using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaController : MonoBehaviour
{
    public float MaxMana;
    public float currentMana;
    public float regenerationRate;

    private void Start()
    {
        currentMana = MaxMana;
    }

    private void SetMana(float mana) => currentMana = UnityEngine.Mathf.Clamp(mana, 0, MaxMana);

    private void AddMana(float amount) => SetMana(currentMana + amount);

    public bool ConsumeMana(float amount)
    {
        if (currentMana >= amount)
        {
            AddMana(-amount);
            return true;
        }
        return false;
    }

    private void Update()
    {
   
        AddMana(regenerationRate * Time.deltaTime);
    }
}
