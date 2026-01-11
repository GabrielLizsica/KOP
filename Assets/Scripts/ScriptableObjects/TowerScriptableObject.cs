using UnityEngine;

[CreateAssetMenu(fileName = "TowerScriptableObject", menuName = "Scriptable Objects/TowerScriptableObject")]
public class TowerScriptableObject : ScriptableObject
{
    public int damage;
    public int range;
    public float attackspeed;
}
