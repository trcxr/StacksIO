using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour {

    public bool gamePlayed;
    public static Tracker instance;
    public int sceneId;

    public int sceneCount;

    public List<Color> enemyColrors;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start() {
        gamePlayed = false;
    }
}