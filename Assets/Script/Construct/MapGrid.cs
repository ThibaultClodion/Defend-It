using UnityEngine;

public class MapGrid
{
    private Cell[,] grid;
    private int width;
    private int height;
    
    public enum CellType
    {
        EMPTY,
        WALL,
        CLOSEDOOR,
        OPENDOOR,
        TOWER,
        HEART
    }

    public struct Cell
    {
        public CellType type;
        public int currentHealth;
    }
    

    public MapGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new Cell[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                grid[x, y].type = CellType.EMPTY;
                grid[x, y].currentHealth = 0;
            }
        }
    }

    public void SetCell(int x, int y, Cell cell)
    {
        Vector2Int position = WorldToGrid(x, y);
        grid[position.x, position.y] = cell;
    }

    public void SetCellType(int x, int y, CellType cell)
    {
        Vector2Int position = WorldToGrid(x, y);
        grid[position.x, position.y].type = cell;
    }

    public bool HitCell(int x, int y, int damage)
    {
        Vector2Int position = WorldToGrid(x, y);
        grid[position.x, position.y].currentHealth -= damage;
        bool isDestroyed = grid[position.x, position.y].currentHealth <= 0;
        // enlever si on veut respawn les barrière
        if (isDestroyed)
            grid[position.x, position.y].type = CellType.EMPTY;
        return isDestroyed;
    }

    public Cell GetCell(int x, int y) 
    {
        Vector2Int position = WorldToGrid(x, y);
        return grid[position.x, position.y];
    }

    private Vector2Int WorldToGrid(int x, int y)
    {
        return new Vector2Int(x + width / 2, y + height / 2);
    }

    public static CellType ConstructDataToCellType(ConstructData data)
    {
        if (data.isDoor)
        {
            return CellType.CLOSEDOOR;
        }
        else if (data.isTower)
        {
            return CellType.TOWER;
        }
        else if (data.isHeart)
        {
            return CellType.HEART;
        }
        return CellType.WALL;
    }

    public static Cell ConstructDataToCell(ConstructData data)
    {
        Cell cell = new Cell();
        cell.type = ConstructDataToCellType(data);
        cell.currentHealth = data.health;
        return cell;
    }

    //TODO
    public int LayersAroundTarget(Vector2Int target)
    {
        return 0;
    }
}

