using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public float timeLeft;
    private Text timerText;

    private void Start() {
        timerText = GetComponent<Text>();
    }

    void Update() {
        if (!GameManagerScript.instance.timeOut) {
            timeLeft -= Time.unscaledDeltaTime;

            string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
            string seconds = (timeLeft % 60).ToString("00");

            timerText.text = minutes + ":" + seconds;

            //timeLeft -= Time.unscaledDeltaTime;
            //timerText.text = timeLeft.ToString("0");
            if (timeLeft < 0f) {
                GameManagerScript.instance.timeOut = true;
            }
        }
    }
}
