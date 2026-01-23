using UnityEngine;

[CreateAssetMenu(fileName = "BasicTrapScriptableObject", menuName = "Scriptable Objects/BasicTrapScriptableObject")]
public class BasicTrapScriptableObject : ScriptableObject
{
    public int damage;
    public int health;
    public float effectstrength;
    public float effectduration;
}
