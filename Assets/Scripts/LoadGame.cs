using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        yield return new WaitForSeconds(1f);

        int sceneCount = SceneManager.sceneCountInBuildSettings - 1;
        Debug.Log("Scene Count : " + sceneCount);

        Tracker.instance.sceneId = Random.Range(0, sceneCount);
        Tracker.instance.sceneCount = sceneCount;
        Debug.Log("Loading Game" + Tracker.instance.sceneId);
        SceneManager.LoadScene("Game" + Tracker.instance.sceneId);
    }
}
