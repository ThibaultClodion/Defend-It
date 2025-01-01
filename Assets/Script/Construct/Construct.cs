using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu]
public class Construct : RuleTile<Construct.Neighbor>
{
    [Header("Make Rules")]
    public bool alwaysConnect;
    public TileBase[] tilesToConnect;
    public bool checkSelf;

    [Header("Data of Construct")]
    public ConstructData data;
    //If it's a tower
    [SerializeField] public SpellData towerProjectileData;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
        public const int NotSpecified = 6;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Any: return Check_Any(tile);
            case Neighbor.Specified: return Check_Specified(tile);
            case Neighbor.Nothing: return Check_Nothing(tile);
            case Neighbor.NotSpecified: return Check_NotSpecified(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    bool Check_This(TileBase tile)
    {
        if (!alwaysConnect) return tile == this;
        else return tilesToConnect.Contains(tile) || tile == this;
    }

    bool Check_NotThis(TileBase tile)
    {
        return tile != this;
    }

    bool Check_Any(TileBase tile)
    {
        if (checkSelf) return tile != null;
        else return tile != null && tile != this;
    }
    bool Check_Specified(TileBase tile)
    {
        return tilesToConnect.Contains(tile);
    }

    bool Check_Nothing(TileBase tile)
    {
        return tile == null;
    }

    bool Check_NotSpecified(TileBase tile)
    {
        return !tilesToConnect.Contains(tile);
    }

    public void PlaceTile(Vector3Int location, Tilemap placableMap)
    {
        if (placableMap.GetTile(location) == null)
        {
            placableMap.SetTile(location, this);
        }
    }

    public void OpenDoor(Vector3Int location, Tilemap placableMap)
    {
        placableMap.SetTile(location, data.openTile);
    }

    public void CloseDoor(Vector3Int location, Tilemap placableMap)
    {
       placableMap.SetTile(location, data.closeTile);
    }
}