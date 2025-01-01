using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public MapGrid mapGrid;
    [SerializeField] private GameObject towerGO;
    [SerializeField] private GameObject towerParentGO;
    [SerializeField] private Tilemap placableMap;

    void Start()
    {
        //TODO Modifier la taille au lancement de la map
        mapGrid = new MapGrid(50, 50);
    }


    void Update()
    {

    }

    public void PlaceTile(Construct actualTile, Vector3Int location)
    {
        MapGrid.Cell cell = MapGrid.ConstructDataToCell(actualTile.data);

        if (placableMap == null)
        {
            cell.type = MapGrid.CellType.EMPTY;
            cell.currentHealth = 0;
        }
        mapGrid.SetCell(location.x, location.y, cell);

        if (actualTile.data.isTower)
        {
            GameObject newTower = Instantiate(towerGO, location, new Quaternion(0, 0, 0, 0));
            newTower.transform.parent = towerParentGO.transform;
            
            if (actualTile.towerProjectileData.getType() == SpellType.PROJECTILE)
                newTower.GetComponent<Tower>().projectileData = (SpellProjectileData) actualTile.towerProjectileData;
        }
       
        actualTile.PlaceTile(location, placableMap);
    }

    public void PlaceHeart(GameObject heart, Vector3Int location) 
    {
        heart.transform.position = new Vector3(location.x + 0.5f, location.y + 0.5f, 0f);
        heart.SetActive(true);
    }

    public void SetTile(TileBase tile, Vector3Int location)
    {
        if (tile == null)
        {
            mapGrid.SetCell(location.x, location.y, 
                new MapGrid.Cell { type = MapGrid.CellType.EMPTY, currentHealth = 0 });
        }
        placableMap.SetTile(location, tile);
    }

    public void Hit(Vector3Int location, int damage)
    {
        bool isDestroyed = mapGrid.HitCell(location.x, location.y, damage);
        MapGrid.Cell cell = mapGrid.GetCell(location.x, location.y);
        Debug.Log(cell.currentHealth);
        if(isDestroyed)
        {
            //SetTile()
            placableMap.SetTile(location, null);
        }
    }

    public Construct GetTile(Vector3Int location)
    {
        return (Construct)placableMap.GetTile(location);
    }

    public void ResetMap()
    {
        
    }
}
