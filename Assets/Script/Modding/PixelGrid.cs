using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PixelGrid : MonoBehaviour
{
    
    [SerializeField] private Tile pixelTile;
    [SerializeField] private Tilemap pixelMap;
    
    private static int width = 64;
    private static int height = 64;
    private Color[] colors = new Color[width * height];
    private Color defautColor = Color.white;

    void Start()
    {
        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < height; x++)
            {
                Vector3Int p = new Vector3Int(x, y, 0);
                pixelMap.SetTile(p, pixelTile);
                colors[x * width + y] = defautColor;
            }
        }
    }

    void Update()
    {
       
    }

    public void Save()
    {
        Texture2D texture = TextureFromColorGrid(colors, width, height);
        SaveTextureAsPNG(texture, "Modding/test.png");
    }

    public static Texture2D TextureFromColorGrid(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public bool PositionIsValid(Vector3Int position)
    {
        if (position.x >= width || position.x < 0
            || position.y >= height || position.y < 0)
        {
            return false;
        }
        return true;
    }

    public void SetPixel(Vector3Int position, Color color)
    {
        if(!PositionIsValid(position))
        {
            return;
        }

        Tile pixelTileCopy = Tile.CreateInstance<Tile>();
        pixelTileCopy.sprite = pixelTile.sprite;
        pixelTileCopy.color = color;
        colors[position.x * width + position.y] = color;
        pixelMap.SetTile(position, pixelTileCopy);
    }

    //Le faire tourner sur un thread / return WaitForSecond pour temps réel / optimiser l'algo (récursif?)
    public void FillPixels(Vector3Int position, Color newColor)
    {
        if (!PositionIsValid(position))
        {
            return;
        }
        List<Vector3Int> pixelsToSearch = new List<Vector3Int> { position };
        Color colorToSearch = GetPixelColor(position);

        if(colorToSearch == newColor) 
        {
            return;
        }

        while(pixelsToSearch.Count > 0)
        {
            List<Vector3Int> filteredPixels = GetPixelsPositionAround(pixelsToSearch[0]).Where(pixel =>
            PositionIsValid(pixel) && !pixelsToSearch.Contains(pixel) && GetPixelColor(pixel) == colorToSearch
            ).ToList();
            pixelsToSearch.AddRange(filteredPixels);
            SetPixel(pixelsToSearch[0], newColor);
            pixelsToSearch.RemoveAt(0);
        }
    }

    public void RemovePixel(Vector3Int position)
    {
        SetPixel(position, defautColor);
    }

    public Color GetPixelColor(Vector3Int position)
    {
        return colors[position.x * width + position.y];
    }

    public Vector3Int WorldToGridPosition(Vector3Int position)
    {
        return pixelMap.WorldToCell(position);
    }

    private static List<Vector3Int> GetPixelsPositionAround(Vector3Int position)
    {
        return new List<Vector3Int>{ 
            position + new Vector3Int(1, 0),
            position + new Vector3Int(-1, 0),
            position + new Vector3Int(0, 1),
            position + new Vector3Int(0, -1)
        };
    }

    //public static Texture2D TextureFromHeightMap(float[,] heightMap)
    //{
    //    int width = heightMap.GetLength(0);
    //    int height = heightMap.GetLength(1);

    //    Color[] colourMap = new Color[width * height];
    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
    //        }
    //    }

    //    return TextureFromColorGrid(colourMap, width, height);
    //}

    public static void SaveTextureAsPNG(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
    }
}
