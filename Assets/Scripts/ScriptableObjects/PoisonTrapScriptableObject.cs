using UnityEngine;

[CreateAssetMenu(fileName = "PoisonTrapScriptableObject", menuName = "Scriptable Objects/PoisonTrapScriptableObject")]
public class PoisonTrapScriptableObject : ScriptableObject
{
    public int damage;
    public int health;
    public float effectstrength;
}
