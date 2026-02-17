using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellScriptableObject", menuName = "Scriptable Objects/SpellScriptableObject")]
public class SpellScriptableObject : ScriptableObject
{
    public Dictionary<string, string> title;
    public Dictionary<string, string> description;
    public Dictionary<string, int> costs;
    public float effectstrength;
    public float effectduration;

    public void Init(SaveLoadSystem.SpellData data, int level)
    {
        SaveLoadSystem.SpellStats stats = data.stats[$"level{level}"];

        this.title = data.title;
        this.description = data.description;
        this.costs = data.costs;
        this.effectstrength = stats.effectstrength;
        this.effectduration = stats.effectduration;
    }
}
