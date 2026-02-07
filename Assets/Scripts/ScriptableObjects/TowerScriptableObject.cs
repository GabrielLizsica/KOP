using UnityEngine;

[CreateAssetMenu(fileName = "TowerScriptableObject", menuName = "Scriptable Objects/TowerScriptableObject")]
public class TowerScriptableObject : ScriptableObject
{
    public int damage;
    public int range;
    public float attackspeed;

    public void Init(int _damage, int _range, float _attackspeed)
    {
        damage = _damage;
        range = _range;
        attackspeed = _attackspeed;
    }
}
