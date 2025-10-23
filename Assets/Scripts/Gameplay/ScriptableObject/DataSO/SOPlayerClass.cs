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

    public float baseStrength;
    public float baseDexterity;
    public float baseIntelligence;
    public float baseFaith;
    public float baseArcane;
  

    [Header("Abilities")]
    public string[] abilityNames;
    public Sprite[] abilityIcons;

}
