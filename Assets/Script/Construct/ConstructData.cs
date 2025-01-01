using UnityEngine;

[CreateAssetMenu]
public class ConstructData : ScriptableObject
{
    public Sprite logoSprite;
    public int health;
    public int stock;

    [Header("Type of Tile")]
    public bool isDoor;
    public bool isTower;
    public bool isWall;
    public bool isPortal;
    public bool isHeart;

    [Header("Two types of Doors")]
    public RuleTile openTile;
    public RuleTile closeTile;
}
