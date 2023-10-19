using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClampStackSize : MonoBehaviour {
    public GameObject clampTextPrefab;
    public GameObject clampText;

    public float topDistance;
    public float scaleDivisor;
    public float scaleSpeed;

    private Text stackSizeText;
    private Text nameText;
    private GameObject topStack;

    private void Start() {
        clampText = Instantiate(clampTextPrefab, GameObject.Find("Canvas").transform);
        stackSizeText = clampText.transform.Find("StackSize").gameObject.GetComponent<Text>();
        nameText = clampText.transform.Find("Name").gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManagerScript.instance.playerIsDead || GameManagerScript.instance.endGamePanel.activeInHierarchy) {
            return;
        }

        // Get the Min and Max Allowed position
        float minX = clampText.GetComponent<RectTransform>().rect.width / 2;
        float maxX = Screen.width - minX;

        float minY = clampText.GetComponent<RectTransform>().rect.height / 2;
        float maxY = Screen.height - minY;

        // Get the position of the target.
        topStack = transform.GetChild(transform.childCount - 1).gameObject;
        Vector3 textPosition = Camera.main.WorldToScreenPoint(topStack.transform.position + new Vector3(0.0f, topDistance, 0.0f));

        if (textPosition.z < 0) {
            textPosition.y = -textPosition.y;
            textPosition.x = -textPosition.x;
        }

        // Get the current position before clamping the UI to the screen.
        Vector3 newPosition = new Vector3(textPosition.x, textPosition.y, textPosition.z);

        // Clamp the UI to the screen.
        textPosition.x = Mathf.Clamp(textPosition.x, minX, maxX);
        textPosition.y = Mathf.Clamp(textPosition.y, minY, maxY);
        textPosition.z = 0f;
        clampText.transform.position = textPosition;

        // Apply Rotation
        clampText.transform.up = -((newPosition - textPosition).normalized);

        // Reset Rotation when the target is on the screen.
        Vector3 objectPosition = Camera.main.WorldToViewportPoint(topStack.transform.position);
        if (objectPosition.x > 0 && objectPosition.x < 1 && objectPosition.y > 0 && objectPosition.y < 1 && objectPosition.z > 0) {
            clampText.transform.up = Vector3.zero;
        }

        /* Below Code is used to handle resizing of UI Pointer based on distance from the Player */

        Vector3 distanceFromPlayer = transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
        //Debug.Log(gameObject.name + " distanceFromPLayer : " + distanceFromPlayer + " Magnitude : " + distanceFromPlayer.magnitude);

        if (distanceFromPlayer.magnitude > 5f) {
            Vector3 newScale = (Vector3.one * scaleDivisor) / distanceFromPlayer.magnitude;
            newScale.x = newScale.y = newScale.z = Mathf.Clamp(newScale.x, 0.0f, 1.0f);
            clampText.transform.localScale = Vector3.Lerp(clampText.transform.localScale, newScale, scaleSpeed * Time.deltaTime);
        } else {
            clampText.transform.localScale = Vector3.Lerp(clampText.transform.localScale, Vector3.one, scaleSpeed * Time.deltaTime);
        }

        if (gameObject.tag == "Enemy") {
            stackSizeText.text = gameObject.GetComponent<EnemyController>().stackSize.ToString();
            nameText.text = gameObject.GetComponent<EnemyController>().myName;
        } else if (gameObject.tag == "Player") {
            stackSizeText.text = gameObject.GetComponent<PlayerController>().stackSize.ToString();
            nameText.text = gameObject.GetComponent<PlayerController>().myName;
        }
    }
}
