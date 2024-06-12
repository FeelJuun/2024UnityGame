using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneName;

    void Awake() {    // 본래는 OnLevelWasLoaded가 사용되었지만 Unity 버전에서 곧 사용이 종료된다고 하여 
        SceneManager.sceneLoaded += OnSceneLoaded; // OnSceneLoaded 로 바꿔보았습니다.
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 2
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {  // 이 부분
        string newSceneName = scene.name;
        if (newSceneName != sceneName) {
            sceneName = newSceneName;
            Invoke("PlayMusic", 0.2f);
        }
    }

    void PlayMusic(){
        AudioClip clipToPlay = null;

        if (sceneName == "Menu"){
            clipToPlay = menuTheme;
        }
        else if (sceneName == "Game"){
            clipToPlay = mainTheme;
        }

        if (clipToPlay != null){
            AudioManager.instance.PlayMusic (clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }
}
