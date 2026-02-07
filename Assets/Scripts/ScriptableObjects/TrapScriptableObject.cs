using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapScriptableObject", menuName = "Scriptable Objects/TrapScriptableObject")]
public class TrapScriptableObject : ScriptableObject
{
    public float damage;
    public int health;
    public float effectstrength;
    public float effectduration;

    public void Init(float _damage, int _health, float _effectsrength, float _effectduration)
    {
        this.damage = _damage;
        this.health = _health;
        this.effectstrength = _effectsrength;
        this.effectduration = _effectduration;
    }
}
