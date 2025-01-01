using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

[CreateAssetMenu]
public class LevelWaves : ScriptableObject
{
    [Header("Map Settings")]
    public List<GameObject> spawnsPosition;
    public List<GameObject> ennemies;

    [System.Serializable]
    public class Wave
    {
        [Header("Difficulty settings")]
        public int difficultyScore;
        public int time;

        [Header("Wave spawns and ennemies")]

        [Tooltip("Which spawn are use from Spawns Position for this wave")]
        public List<int> spawns;

        [Tooltip("Which ennemies are use from ennemies and with what spawn pourcentage")]
        public List<int> ennemies;
        public List<int> ennemiePourcentage;

        public int[] ConvertPourcentage()
        {
            int[] spawnsPourcentage = new int[100];
            for(int i = 0; i < ennemiePourcentage.Count; i++)
            {
                for(int j = 0; j < ennemiePourcentage[i]; j++)
                {
                    spawnsPourcentage[j] = i;
                }
            }

            return spawnsPourcentage;
        }

        public float HowManyEnnemyEstimation(List<GameObject> ennemies)
        {
            float averageScore = 0;
            for(int i = 0; i < ennemies.Count; i++) 
            {
                averageScore += ennemies[i].GetComponent<Character>().data.stats.difficultyScore * (ennemiePourcentage[i]/100);
            }

            return ((float)difficultyScore) / averageScore;
        }

        public Vector3 GetAPosition(List<GameObject> spawnsPosition)
        {
            return spawnsPosition[spawns[Random.Range(0,spawns.Count)]].transform.position;
        }

        public GameObject GetAnEnnemy(List<GameObject> ennemies, int[] spawnPourcentage, int actualScore, int maxScore)
        {
            //Spawn randomly (with pourcentage) a ennemy
            if(actualScore >= maxScore)
            {
                return ennemies[spawnPourcentage[Random.Range(0, 100)]];
            }
            //Spawn an ennemy with a score that is not greater then actualScore to balance the wave
            else
            {
                while(true)
                {
                    GameObject ennemy = ennemies[spawnPourcentage[Random.Range(0, 100)]];
                    if(ennemy.GetComponent<Character>().data.stats.difficultyScore <= actualScore)
                    {
                        return ennemy;
                    }
                }
            }
        }

        public int[] MinMaxScore(List<GameObject> ennemies)
        {
            int[] MinMax = {int.MaxValue, int.MinValue};
            for(int i = 0; i < ennemies.Count; ++i) 
            {
                if (MinMax[0] > ennemies[i].GetComponent<Character>().data.stats.difficultyScore)
                {
                    MinMax[0] = ennemies[i].GetComponent<Character>().data.stats.difficultyScore;
                }
                if (MinMax[1] < ennemies[i].GetComponent<Character>().data.stats.difficultyScore)
                {
                    MinMax[1] = ennemies[i].GetComponent<Character>().data.stats.difficultyScore;
                }
            }
            return MinMax;
        }

    }

    [Header("Waves of the map")]
    [SerializeField] public Wave[] waves;

    /*Old Code for spawn
     * public enum SPAWN
    {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        AROUND
    }

    [System.Serializable]
    public class Squad
    {
        public SPAWN spawn;
        public int number;
        public IAController ennemy;

        public List<Vector3Int> GetSpawnPosition(Vector3Int tilemapSize)
        {
            if(spawn == SPAWN.AROUND)
            {
                return CircleSpawn(tilemapSize);
            }

            List<Vector3Int> result = new List<Vector3Int>();
            if (spawn == SPAWN.BOTTOM)
                result = LineSpawn(0, -tilemapSize.y / 2);
            else if(spawn == SPAWN.TOP)
                result = LineSpawn(0, tilemapSize.y / 2);
            else if (spawn == SPAWN.LEFT)
                result = LineSpawn(-tilemapSize.x / 2, 0);
            else if (spawn == SPAWN.RIGHT)
                result = LineSpawn(tilemapSize.x / 2, 0);

            return result;
        }

        private List<Vector3Int> LineSpawn(int x, int y)
        {
            int width = number / 5;
            int ratioDecalage = 1;

            List<Vector3Int> positions = new List<Vector3Int>(number);
            int decalageHeight = 0;
            for (int i = 0; i < number; i++)
            {
                if (i % width == 0)
                    decalageHeight += ratioDecalage;

                if(x == 0)
                    positions.Add(new Vector3Int(i % width, y + decalageHeight, 0)); // bas et haut de la map
                if(y == 0)
                    positions.Add(new Vector3Int(x + decalageHeight, i % width, 0)); // droite et gauche
            }
            return positions;
        }

        private List<Vector3Int> CircleSpawn(Vector3Int size)
        {
            return new List<Vector3Int>(number);
        }
    }

    [System.Serializable]
    public class Wave
    {
        public List<Squad> squad;
        
        public int totalEnnemies()
        {
            int result = 0; 
            squad.ForEach(x => result += x.number);
            return result;
        }
    }*/ 
}
