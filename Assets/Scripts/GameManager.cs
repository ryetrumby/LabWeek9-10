using CarnivalShooter2D.Observer;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float maxTime;
    float timer;

    ScoreManager scoreManager;
    TMP_Text timerText;
    TMP_Text scoreText;

    private void Awake()
    {
        timer = maxTime;

        scoreManager = GetComponent<ScoreManager>();

        timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();
        timerText.text = timer.ToString();

        scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;


        timerText.text = timer.ToString("F2");
        scoreText.text = scoreManager.score.ToString();
    }

    

}
