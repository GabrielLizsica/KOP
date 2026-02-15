using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerScriptableObject", menuName = "Scriptable Objects/TowerScriptableObject")]
public class TowerScriptableObject : ScriptableObject
{
    public Dictionary<string, string> title;
    public Dictionary<string, string> description;
    public int damage;
    public int range;
    public float attackspeed;

    public void Init(SaveLoadSystem.TowerData data, int level)
    {   
        SaveLoadSystem.TowerStats stats = data.stats[$"level{level}"];
        
        title = data.title;
        description = data.description;
        damage = stats.damage;
        range = stats.range;
        attackspeed = stats.attackspeed;
    }
}
