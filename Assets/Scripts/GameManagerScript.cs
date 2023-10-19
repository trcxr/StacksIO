using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {
    private GameObject player;
    private GameObject playerStack;
    private PlayerController playerControllerScript;
    private bool changeScene = false;
    private List<Color> enemyColors;
    private List<string> enemyNames;

    public GameObject stackPrefab;
    public GameObject startGamePanel;
    public GameObject endGamePanel;
    public GameObject endGameText;
    public GameObject loadingText;
    public GameObject timerPanel;
    public GameObject stackIncrementText;
    public GameObject killText;
    public List<GameObject> enemyList;
    public List<GameObject> collectibles;

    public static GameManagerScript instance;
    public bool playerIsDead = false;
    public bool spawnCollectibles = false;
    public bool canMove = false;
    public bool timeOut = false;

    [TextArea(0, 10)]
    public string winText;
    [TextArea(0, 10)]
    public string loseText;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        Time.timeScale = 0.0f;
        player = GameObject.FindGameObjectWithTag("Player");
        playerStack = player.transform.Find("PlayerStack").gameObject;
        playerControllerScript = player.GetComponent<PlayerController>();
        player.SetActive(false);
        enemyColors = Tracker.instance.enemyColrors;

        if (Tracker.instance.gamePlayed) {
            startGamePanel.SetActive(false);
        } else {
            startGamePanel.SetActive(true);
        }

        if (!startGamePanel.activeInHierarchy) {
            StartGame();
        }

        enemyNames = Enum.GetValues(typeof(NameEnum))
                                        .Cast<NameEnum>()
                                        .Select(v => v.ToString())
                                        .ToList();
    }

    private void Update() {
        if (spawnCollectibles) {
            for (int i = 0; i < collectibles.Count; i++) {
                collectibles[i].SetActive(true);
            }
            spawnCollectibles = false;
        }

        if (collectibles.Count == 0) {
            spawnCollectibles = true;
        }

        if (enemyList.Count == 0) {
            endGamePanel.SetActive(true);
            endGameText.GetComponent<Text>().text = winText;
            canMove = false;
            timerPanel.SetActive(false);
            Destroy(player.GetComponent<ClampStackSize>().clampText.gameObject);
            Tracker.instance.gamePlayed = true;
            changeScene = true;
        }

        if (playerIsDead) {
            endGamePanel.SetActive(true);
            endGameText.GetComponent<Text>().text = loseText;
            canMove = false;
            EnemyState(false);
            DestroyClampText();
            timerPanel.SetActive(false);
            Tracker.instance.gamePlayed = true;
        }

        if (timeOut) {
            endGamePanel.SetActive(true);
            if (PlayerWon()) {
                endGameText.GetComponent<Text>().text = winText;
                changeScene = true;
            } else {
                endGameText.GetComponent<Text>().text = loseText;
            }
            canMove = false;
            EnemyState(false);
            DestroyClampText();
            Destroy(player);
            Tracker.instance.gamePlayed = true;
        }
    }

    public void RestartScene() {
        int sceneId = int.Parse(SceneManager.GetActiveScene().name.Trim().Substring(SceneManager.GetActiveScene().name.Length - 1));
        if (changeScene) {
            sceneId = UnityEngine.Random.Range(0, Tracker.instance.sceneCount);
            if (sceneId == Tracker.instance.sceneId) {
                if (sceneId > 0) {
                    sceneId = 0;
                } else if (sceneId < Tracker.instance.sceneCount - 1) {
                    sceneId = Tracker.instance.sceneCount - 1;
                }
            }
        }
        Tracker.instance.sceneId = sceneId;
        SceneManager.LoadScene("Game" + sceneId);
    }

    public void IncreasePlayerStackSize() {
        int stackSize = ++(playerControllerScript.stackSize);
        SpawnPlayerStack(stackSize);
    }

    public void IncreaseEnemyStackSize(GameObject enemy) {
        int stackSize = ++(enemy.GetComponent<EnemyController>().stackSize);
        SpawnEnemyStack(enemy, stackSize);
    }

    public void PlayPlayerStackAnimation() {
        int stackSize = playerControllerScript.stackSize;
        StartCoroutine(AnimateStackRoutine(player, stackSize));
    }

    public void PlayEnemyStackAnimation(GameObject enemy) {
        int stackSize = enemy.GetComponent<EnemyController>().stackSize;
        StartCoroutine(AnimateStackRoutine(enemy, stackSize));
    }

    IEnumerator AnimateStackRoutine(GameObject obj, int stackSize) {
        for (int i = 0; i < stackSize; i++) {
            if (obj == null || !obj.activeInHierarchy) {
                yield return null;
            } else {
                obj.transform.GetChild(i + 1).gameObject.GetComponent<Animator>().Play("ScaleAnimate", 0, 0f);
                yield return new WaitForSeconds(.05f);
            }
        }
    }

    public void AddCollectible(GameObject enemy, Transform collectibleTransform) {
        enemy.GetComponent<EnemyController>().AddCollectible(collectibleTransform);
    }

    public void RemoveCollectible(GameObject enemy, Transform collectibleTransform) {
        enemy.GetComponent<EnemyController>().RemoveCollectible(collectibleTransform);
    }

    public void ShowStackIncrementText(int i = 1) {
        stackIncrementText.SetActive(false);
        stackIncrementText.GetComponent<Text>().text = "+" + i;

        StartCoroutine(ShowStackIncrementTextRoutine());
    }

    IEnumerator ShowStackIncrementTextRoutine() {
        stackIncrementText.SetActive(true);
        yield return new WaitForSeconds(1f);
        stackIncrementText.SetActive(false);
    }

    public void ShowEnemyKillText() {
        killText.SetActive(false);
        killText.GetComponent<Text>().text = "Kill!";

        StartCoroutine(ShowEnemyKillTextRoutine());
    }

    IEnumerator ShowEnemyKillTextRoutine() {
        killText.SetActive(true);
        yield return new WaitForSeconds(1f);
        killText.SetActive(false);
    }

    public void SpawnCollectible(Transform transform) {
        transform.position = new Vector3(transform.position.x + UnityEngine.Random.Range(-2, 2), transform.position.y, transform.position.z + UnityEngine.Random.Range(-2, 2));
        StartCoroutine(SpawnCollectibleRoutine(transform));
    }

    IEnumerator SpawnCollectibleRoutine(Transform transform) {
        yield return new WaitForSeconds(3f);
        transform.gameObject.SetActive(true);
    }

    private void SpawnPlayerStack(int stackSize) {
        GameObject spawnStack = Instantiate(stackPrefab, player.transform);
        spawnStack.transform.localPosition = new Vector3(0.0f, ((stackSize - 1) * 0.15f), 0.0f);
        spawnStack.GetComponent<MeshRenderer>().material = playerStack.GetComponent<MeshRenderer>().material;
    }

    private void SpawnEnemyStack(GameObject enemy, int stackSize) {
        GameObject spawnStack = Instantiate(stackPrefab, enemy.transform);
        spawnStack.transform.localPosition = new Vector3(0.0f, ((stackSize - 1) * 0.15f), 0.0f);
        spawnStack.GetComponent<MeshRenderer>().material = enemy.transform.Find("EnemyStack").gameObject.GetComponent<MeshRenderer>().material;
    }

    private bool PlayerWon() {
        int playerStackSize = playerControllerScript.stackSize;

        for (int i = 0; i < enemyList.Count; i++) {
            if (playerStackSize < enemyList[i].GetComponent<EnemyController>().stackSize) {
                return false;
            }
        }
        return true;
    }

    private void StartGame() {
        Time.timeScale = 1.0f;
        spawnCollectibles = true;
        startGamePanel.SetActive(false);
        endGamePanel.SetActive(false);
        StartCoroutine(SetupGame());
    }

    IEnumerator SetupGame() {
        loadingText.GetComponent<Text>().text = "Spawning Stacks...";
        loadingText.SetActive(true);
        yield return new WaitForSeconds(2f);

        loadingText.GetComponent<Text>().text = "Spawning Enemies...";
        SetupEnemies();
        EnemyState(true);
        player.SetActive(true);
        yield return new WaitForSeconds(1f);

        loadingText.GetComponent<Text>().text = "GO...";
        yield return new WaitForSeconds(1f);
        loadingText.SetActive(false);
        canMove = true;
        timerPanel.SetActive(true);
    }

    private void Shuffle<T>(IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    private void EnemyState(bool active) {
        for (int i = 0; i < enemyList.Count; i++) {
            enemyList[i].SetActive(active);
        }
    }

    private void SetupEnemies() {
        Shuffle<Color>(enemyColors);
        Shuffle<string>(enemyNames);
        for (int i = 0; i < enemyList.Count; i++) {
            Renderer rend = enemyList[i].transform.Find("EnemyStack").gameObject.GetComponent<Renderer>();
            rend.material = new Material(rend.material);
            rend.material.color = enemyColors[i];
            enemyList[i].GetComponent<EnemyController>().myName = enemyNames[i];
        }
    }

    private void DestroyClampText() {
        for (int i = 0; i < enemyList.Count; i++) {
            Destroy(enemyList[i].GetComponent<ClampStackSize>().clampText);
        }
    }
}
