using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfleScriptableObject", menuName = "Scriptable Objects/PlayerProfleScriptableObject")]
public class PlayerProfleScriptableObject : ScriptableObject
{
    public int gold;
    public Dictionary<MainGameLogic.CardTypes, SaveLoadSystem.Card> cards;
}
