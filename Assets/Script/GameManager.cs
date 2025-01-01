using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;
using static LevelWaves;

public class GameManager : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private MapManager mapManager;
    [SerializeField] private Tilemap tilemap;

    [Header("Ennemy and Player")]
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject ennemiesGO;
    [SerializeField] private LevelWaves level;

    [Header("UI")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject launchWaveGO;
    [SerializeField] private GameObject commonUI;
    [SerializeField] private GameObject computerUI;
    [SerializeField] private GameObject phoneUI;
    [SerializeField] private GameObject[] cards;
    //Par la suite il faudra randomiser les constructions avec le deck
    [SerializeField] private Construct[] constructions;

    [SerializeField] private Button[] UIMobileButtonToDesactivate;

    //Game Variables
    private int actualWave = 0;
    private int actualScore = 0;
    private bool waveClean = false;


    void Start()
    {
        //The user is on mobile
        if (Application.isMobilePlatform)
        {
            phoneUI.SetActive(true);
            computerUI.SetActive(false);
        }
        //The user is not on mobile
        else
        {
            phoneUI.SetActive(false);
            computerUI.SetActive(true);

            //Desactivate Some Mobile Buttons
            for(int i = 0; i < UIMobileButtonToDesactivate.Length; i++)
            {
                UIMobileButtonToDesactivate[i].enabled = false;
            }
        }

        //if the player is on Mobile or Use Controller
        if (Application.isMobilePlatform || Application.isConsolePlatform)
        {
            eventSystem.firstSelectedGameObject = cards[0];
        }
    }

    
    void Update()
    {
        if (ennemiesGO.GetComponentsInChildren<Transform>().Count() <= 1 && waveClean)
        {
            //Put waveClean to false to not enter this function another time
            waveClean = false;
            EndOfWave();
        }

        /*if(Input.GetKeyDown("space"))
        { // capture screen shot on left mouse button down

            string folderPath = "Assets/Screenshots/"; // the path of your project folder

            if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
                System.IO.Directory.CreateDirectory(folderPath);  // it will get created

            var screenshotName =
                                    "Screenshot_" +
                                    System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                    ".png"; // put youre favorite data format here
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 2); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
            Debug.Log(folderPath + screenshotName); // You get instant feedback in the console
        }*/
    }

    #region Events UI
    //Launch the actual wave
    public void LaunchWave()
    {
        if(actualWave < level.waves.Length)
        {
            Wave wave = level.waves[actualWave];
            actualScore = wave.difficultyScore;

            //Get the conversion for pourcentage spawn
            int[] spawnPourcentage = wave.ConvertPourcentage();

            StartCoroutine(SpawnMonsters(wave, spawnPourcentage));

            launchWaveGO.SetActive(false);
            player.SwitchState();

            waveClean = false;
            actualWave++;
        }
    }

    //Spawn all monsters from a wave
    IEnumerator SpawnMonsters(Wave wave, int[] spawnPourcentage)
    {
        float ennemiesNumberEstimation = wave.HowManyEnnemyEstimation(level.ennemies);
        int[] minMaxScore = wave.MinMaxScore(level.ennemies);

        while (actualScore > minMaxScore[0])
        {
            GameObject ennemy = wave.GetAnEnnemy(level.ennemies, spawnPourcentage, actualScore, minMaxScore[1]);
            Vector3 position = wave.GetAPosition(level.spawnsPosition);

            GameObject generatedEnnemy = Instantiate(ennemy, position, new Quaternion());
            generatedEnnemy.transform.parent = ennemiesGO.transform;

            actualScore -= ennemy.GetComponent<Character>().data.stats.difficultyScore;

            //Avoid the fact that ennemies spawn on each other
            yield return new WaitForSeconds(UnityEngine.Random.Range(wave.time/(ennemiesNumberEstimation*3), wave.time / ennemiesNumberEstimation));
        }

        waveClean = true;
    }
    #endregion

    //Makes changement at the end of a wave
    private void EndOfWave()
    {
        Debug.Log("The wave is finish");
        launchWaveGO.SetActive(true);

        //Player Changes
        player.SwitchState();
        player.GiveNewConstruct();
        player.money++;
        player.moneyText.text = player.money.ToString();
    }
}
