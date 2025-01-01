using System.Collections;
using UnityEngine;

public enum ModdingTool
{
    PEN,
    ERASE,
    FILL
}

public enum ModdingType
{
    CHARACTER,
    SPELL,
    TOWER
}

public class ModdingManager : MonoBehaviour
{
    [SerializeField] private Camera cameraMain;
    [SerializeField] private PixelGrid grid;
    // Ces valeurs seront modifiables via UI après
    [SerializeField] Color32 updateColor;
    [SerializeField] private int toolSize = 1;
    [SerializeField] private CharacterData characterData;

    private int zoomFactor = 5;
    private int zoomMax = 100;
    private int zoomMin = 10;
    private bool canMoveCamera = false;

    private ModdingTool currentTool; 
    //private ModdingType currentType;

    void Start()
    {
        currentTool = ModdingTool.PEN;
        //currentType = ModdingType.CHARACTER;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3Int actualAim = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(currentTool == ModdingTool.FILL) 
            {
                Vector3Int position = grid.WorldToGridPosition(actualAim);
                grid.FillPixels(position, updateColor);
            }
            else
            {
                for (int x = 0; x < toolSize; x++)
                {
                    for (int y = 0; y < toolSize; y++)
                    {
                        Vector3Int position = grid.WorldToGridPosition(actualAim);
                        position.x += x;
                        position.y += y;
                        if (currentTool == ModdingTool.PEN)
                        {
                            grid.SetPixel(position, updateColor);
                        }
                        else if (currentTool == ModdingTool.ERASE)
                        {
                            grid.RemovePixel(position);
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            canMoveCamera = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            canMoveCamera = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && cameraMain.orthographicSize > zoomMin) // forward
        {
            cameraMain.orthographicSize -= zoomFactor;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && cameraMain.orthographicSize < zoomMax) // backwards
        {
            cameraMain.orthographicSize += zoomFactor;
        }

        if (canMoveCamera)
        {
            //TODO
        }
    } 

    public void UseModdingTool(int moddingTool)
    {
        currentTool = (ModdingTool)moddingTool;
    }

    private void SpellDataToJson()
    {

    }

    private void JsonToSpellData()
    {

    }
}