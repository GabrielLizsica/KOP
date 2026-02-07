using UnityEngine;

[CreateAssetMenu(fileName = "SpellScriptableObject", menuName = "Scriptable Objects/SpellScriptableObject")]
public class SpellScriptableObject : ScriptableObject
{
    public float effectstrength;
    public float effectduration;

    public void Init(float _effectsrength, float _effectduration)
    {
        this.effectstrength = _effectsrength;
        this.effectduration = _effectduration;
    }
}
