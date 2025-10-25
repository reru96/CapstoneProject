using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "RPG/PlayerClass")]
public class SOPlayerClass: ScriptableObject
{
 
    public string className;
    [TextArea(3, 5)]
    public string description;

    public Sprite previewImage;
    public Color classColor = Color.white;

    public List<GameObject> poolPrefabs = new List<GameObject>();
    public int defaultPoolSize = 10;

    public GameObject prefab;
    public int requiredLevel = 1;

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
