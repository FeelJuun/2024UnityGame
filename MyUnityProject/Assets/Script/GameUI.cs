using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{

    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public TextMeshProUGUI newWaveTitle;
    public TextMeshProUGUI newWaveEnemyCount;
    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI gameOverScoreUI;
    public RectTransform healthBar;

    Spawner spawner;
    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    void Awake(){
        spawner = FindObjectOfType<Spawner> ();
        spawner.OnNewWave += OnNewWave;
    }

    void Update(){
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if (player != null){
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3 (healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber){
        string[] numbers = {"One", "Two", "Three", "Four", "Five"};
        newWaveTitle.text = "- Wave " + numbers[waveNumber-1] + "-";
        string enemyCountString = ((spawner.waves [waveNumber-1].infinite) ? "Infinite":spawner.waves [waveNumber-1].enemyCount +"");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    void OnGameOver(){
        Cursor.visible = true;
        StartCoroutine(Fade (Color.clear, new Color(0,0,0,.95f),1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive (true);
    }

    IEnumerator AnimateNewWaveBanner() {

        float delayTime = 1f;
        float speed = 3f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1/speed + delayTime;

        while (animatePercent >= 0){
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1){
                animatePercent = 1;
                if ( Time.time > endDelayTime){
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-385, 45, animatePercent);
            yield return null;
        }

    }

    IEnumerator Fade(Color from, Color to, float time){
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1){
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from,to,percent);
            yield return null;
        }
    }
    
    // 게임다시시작 버튼
    public void StartNewGame() {
        SceneManager.LoadScene("Game"); // Application.LoadLevel은 옛 버전이라 Scenmanager로 교체하였습니다.

    }

    public void ReturnToMainMenu(){
        SceneManager.LoadScene("Menu");

    }
}
