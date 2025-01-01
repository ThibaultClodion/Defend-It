using TMPro;
using UnityEngine;

public class News : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;
    private string[] news = new string[10];
    
    void Start()
    {
        //Set the news
        for(int i = 0; i < news.Length; i++) 
        {
            news[i] = "News : " + i;
        }

        text.text = news[Random.Range(0, news.Length)];
    }

    //Display a new news when enable
    private void OnEnable()
    {
        text.text = news[Random.Range(0, news.Length)];
    }
}
