using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "RPG/PlayerClass")]
public class SOPlayerClass: ScriptableObject
{
    [Header("Basic Info")]
    public string className;
    [TextArea(3, 5)]
    public string description;

    [Header("Visuals")]
    public Sprite previewImage;
    public Color classColor = Color.white;

    [Header("Gameplay")]
    public GameObject prefab;
    public int requiredLevel = 1;

    [Header("Stats")]
    public float health = 100f;
    public float mana = 100f;
    public float stamina = 100f;
    public float speed = 5f;
    public float damage = 10f;
    public float defense = 10f;
    public float strenght = 10f;
    public float intelligence = 10f;
    public float dexterity = 10f;


    [Header("Abilities")]
    public string[] abilityNames;
    public Sprite[] abilityIcons;

}
