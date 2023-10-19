using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleStack : MonoBehaviour {

    Vector3 initialTransform;

    bool isCollectable;

    private void Start() {
        initialTransform = gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Ground") {
            isCollectable = true;
        }

        if (other.tag == "Player" && other.GetType() == typeof(BoxCollider)) {
            isCollectable = false;
            GameManagerScript.instance.ShowStackIncrementText();
            GameManagerScript.instance.IncreasePlayerStackSize();
            GameManagerScript.instance.PlayPlayerStackAnimation();
            ResetCollectible();
        }
        if (other.tag == "Enemy" && other.GetType() == typeof(BoxCollider)) {
            isCollectable = false;
            GameManagerScript.instance.RemoveCollectible(other.gameObject, gameObject.transform);
            GameManagerScript.instance.IncreaseEnemyStackSize(other.gameObject);
            GameManagerScript.instance.PlayEnemyStackAnimation(other.gameObject);
            ResetCollectible();
        }
        if (other.tag == "Enemy" && other.GetType() == typeof(SphereCollider)) {
            if (isCollectable) {
                GameManagerScript.instance.AddCollectible(other.gameObject, gameObject.transform);
            }
        }
    }

    private void ResetCollectible() {
        gameObject.transform.position = initialTransform;
        this.gameObject.SetActive(false);
        GameManagerScript.instance.SpawnCollectible(this.transform);
    }

    private void OnTriggerStay(Collider other) {
        if (other.tag == "Enemy" && other.GetType() == typeof(SphereCollider)) {
            if (isCollectable) {
                GameManagerScript.instance.AddCollectible(other.gameObject, gameObject.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Enemy" && other.GetType() == typeof(SphereCollider)) {
            GameManagerScript.instance.RemoveCollectible(other.gameObject, gameObject.transform);
        }
    }
}
