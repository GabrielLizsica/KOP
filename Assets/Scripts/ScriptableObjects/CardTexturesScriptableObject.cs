using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTexturesScriptableObject", menuName = "Scriptable Objects/CardTexturesScriptableObject")]
public class CardTexturesScriptableObject : ScriptableObject
{
    public Dictionary<MainGameLogic.CardTypes, Texture2D> textures;
}
