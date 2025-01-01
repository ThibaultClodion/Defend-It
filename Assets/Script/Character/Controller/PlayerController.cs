using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class PlayerController : MonoBehaviour
{
    private Character character;

    //Dynamic variables
    private Vector2 movementInput;
    private Vector3Int previousAim = Vector3Int.zero;

    [Header("Images and UI")]
    [SerializeField] private List<Image> spellImages = new List<Image>(Character.MAX_ABILITIES);
    [SerializeField] private Image constructImage;
    [SerializeField] private TextMeshProUGUI stockText;
    [SerializeField] private GameObject spellBar;
    [SerializeField] private GameObject constructBar;
    [SerializeField] private Sprite returnCard;
    private int UILayer;
    private bool constructState = true;

    [Header("Construction Tile")]
    [SerializeField] private Deck deck;
    private List<Construct> cards;
    [SerializeField] private Construct actualTile;
    private int tileStock;

    [Header("Money Gestion")]
    [NonSerialized] public int money;
    [SerializeField] public TextMeshProUGUI moneyText;

    [Header("Game Objects")]
    [SerializeField] private GameObject heartGO;
    [SerializeField] private GameObject launchWaveGO;

    [Header("Tilemaps and Map")] // A transférer sur mapManager
    [SerializeField] private Tilemap interactiveMap;
    [SerializeField] private Tilemap collideMap;
    [SerializeField] private Tile blueTile;
    [SerializeField] private MapManager mapManager;

    private void Awake()
    {
        //Initialize variables
        character = GetComponent<Character>();
        money = 100;
        moneyText.text = money.ToString();

        //This permit to not destroy the deck of the player
        cards = new List<Construct>();
        for(int i = 0; i < deck.cards.Count;i++)
        {
            cards.Add(deck.cards[i]);
        }

        //Define the Images display on the Construction Image
        for (int i = 0; i < Character.MAX_ABILITIES; i++)
        {
            if(character.data.spells[i] != null)
                spellImages[i].sprite = character.data.spells[i].logoSprite;
        }
    }

    private void Start()
    {
        //Get the UI Layer
        UILayer = LayerMask.NameToLayer("UI");

        //Get the Heart has first Tile
        tileStock = actualTile.data.stock;
        stockText.text = tileStock.ToString();

    }

    private void FixedUpdate()
    {
        //Make the character move
        character.Move(movementInput);
    }

    private void Update()
    {
        if (constructState)
        {
            //Allow to put Tiles
            TileUpdate();
        }

        if(character.dataUpdated)
        {
            //Change the spell images
            for (int i = 0; i < Character.MAX_ABILITIES; i++)
            {
                if (character.data.spells[i] != null)
                    spellImages[i].sprite = character.data.spells[i].logoSprite;
                else
                    spellImages[i].sprite = null;
            }
            character.dataUpdated = false;
        }
    }

    #region TilesPlacement
    private void TileUpdate()
    {
        //Find location of aiming
        Vector3Int actualAim = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int location = interactiveMap.WorldToCell(actualAim);

        //Highlight the tile
        if (actualAim != previousAim)
        {

            interactiveMap.SetTile(previousAim, null); // Remove old highlight
            interactiveMap.SetTile(actualAim, blueTile);
            previousAim = actualAim;
        }

        //Put a tile if it's possible
        if (Input.GetMouseButton(0) && !IsPointerOverUIElement() && collideMap.GetTile(location) == null)
        {
            MapGrid.Cell cell = mapManager.mapGrid.GetCell(location.x, location.y);

            if (cell.type == MapGrid.CellType.EMPTY && tileStock > 0)
            {

                //SetTile to null to destroy anything
                mapManager.SetTile(null, location);

                if (actualTile.data.isHeart)
                {
                    mapManager.PlaceHeart(heartGO, location);

                    //When heart is placed, waves can start
                    launchWaveGO.SetActive(true);


                    //Just for avoiding some bugs
                    actualTile = cards[0];
                }
                else
                {
                    //Place Tile
                    mapManager.PlaceTile(actualTile, location);
                }

                //Update Tile Stock
                tileStock--;
                stockText.text = tileStock.ToString();

                //If tile Stock is egal to 0 then the next card is return
                if(tileStock == 0)
                {
                    constructImage.sprite = returnCard;
                    stockText.alpha = 0f;
                }
            }

            //if (Input.GetMouseButtonDown(0) && cell.type != MapGrid.CellType.EMPTY)
            //{
            //    mapManager.Hit(location, 50);
            //}
        }

        //Delete a tile if it's possible
        if (Input.GetMouseButton(1) && !IsPointerOverUIElement())
        {
            MapGrid.Cell cell = mapManager.mapGrid.GetCell(location.x, location.y);

            if (cell.type != MapGrid.CellType.EMPTY)
            {
                Construct construct = mapManager.GetTile(location);
                if (construct == actualTile)
                {
                    tileStock++;
                    stockText.text = tileStock.ToString();
                    mapManager.SetTile(null, location);

                    //Il faut trouver un moyen de bien supprimer les tours (via une fonction construct.destruct sinon.
                }
            }
        }
    }

    public void SetConstrustState(bool state)
    {
        constructState = state;
        if(!state)
        {
            interactiveMap.SetTile(previousAim, null);
        }
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    #endregion

    #region Player Input
    //Get Movement Input
    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    //Cooldown a spell
    IEnumerator WaitBeforeReuse(int current)
    {
        if (character.data.spells[current] != null)
        {   
            spellImages[current].color = new Color(1f, 1f, 1f, 0.2f);
            yield return new WaitForSecondsRealtime(character.data.spells[current].cooldown);
        }
        spellImages[current].color = new Color(1f, 1f, 1f, 1f);
    }

    //Cast a spell
    public void CastAbilitie(int current)
    {
        if (!constructState)
        {
            if (character.CanLaunchSpell(current))
            {
                //Launch the spell if the cooldown allow it
                character.LaunchSpell(current, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                //Thi : I can't move this because Coroutine can only be start from a MonoBehaviour there is many other way to throw all the code in spell class
                StartCoroutine(WaitBeforeReuse(current));
            }
        }
    }

    void OnSpell1()
    {
        CastAbilitie(0);
    }
    void OnSpell2()
    {
        CastAbilitie(1);
    }
    void OnSpell3()
    {
        CastAbilitie(2);
    }
    void OnSpell4()
    {
        CastAbilitie(3);
    }

    public void OnNewCard()
    {
        GiveNewConstruct();
    }
    #endregion

    public void GiveNewConstruct()
    {
        if(cards.Count > 0 && !actualTile.data.isHeart && money > 0)
        {
            //Find a Card in the Deck
            int index = UnityEngine.Random.Range(0, cards.Count);
            Construct construct = cards[index];
            cards.RemoveAt(index);
            actualTile = construct;

            //Update the tile stock and Images
            constructImage.sprite = actualTile.data.logoSprite;
            tileStock = construct.data.stock;
            stockText.alpha = 1f;
            stockText.text = tileStock.ToString();

            //Update the money
            money--;
            moneyText.text = money.ToString();
        }
    }


    //Switch beetween construction and combat state
    public void SwitchState()
    {
        //Switch State and Images associate
        if (constructState)
        {
            SetConstrustState(false);

            constructBar.SetActive(false);
            spellBar.SetActive(true);
        }
        else
        {
            SetConstrustState(true);

            constructBar.SetActive(true);
            spellBar.SetActive(false);
        }
    }
}
