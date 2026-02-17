using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapScriptableObject", menuName = "Scriptable Objects/TrapScriptableObject")]
public class TrapScriptableObject : ScriptableObject
{
    public Dictionary<string, string> title;
    public Dictionary<string, string> description;
    public Dictionary<string, int> costs;
    public float damage;
    public int health;
    public float effectstrength;
    public float effectduration;

    public void Init(SaveLoadSystem.TrapData data, int level)
    {
        SaveLoadSystem.TrapStats stats = data.stats[$"level{level}"];
        this.title = data.title;
        this.description = data.description;
        this.costs = data.costs;
        this.damage = stats.damage;
        this.health = stats.health;
        this.effectstrength = stats.effectstrength;
        this.effectduration = stats.effectduration;
    }
}
