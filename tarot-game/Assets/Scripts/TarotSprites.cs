using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TarotSprites", menuName = Vault.other.scriptableObjectMenu + "TarotSprites", order = 0)]
public class TarotSprites : ScriptableObject
{
    [SerializeField] private List<Sprite> sprites;

    public Sprite getSprite(int card) => sprites[card - 1];
}