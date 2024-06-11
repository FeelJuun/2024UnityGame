using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    public Image fadePlane;
    public GameObject gameOverUI;
    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

    void OnGameOver(){
        StartCoroutine(Fade (Color.clear, Color.black,1));
        gameOverUI.SetActive (true);
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
        SceneManager.LoadScene("TEst"); // Application.LoadLevel은 옛 버전이라 Scenmanager로 교체하였습니다.

    }
}
