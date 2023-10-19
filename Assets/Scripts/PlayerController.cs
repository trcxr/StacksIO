using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5;
    public int stackSize = 1;
    public float angleThreshold;
    public float maxDeviation;
    public GameObject pointerGameObject;

    [HideInInspector]
    public string myName;

    private void Start() {
        myName = "You";
    }

    //Update is called once per frame
    void Update() {
        if (GameManagerScript.instance.canMove) {
            float horizontal = SimpleInput.GetAxis("Horizontal");
            float vertical = SimpleInput.GetAxis("Vertical");

            horizontal = Mathf.Clamp(horizontal, -1, 1);
            vertical = Mathf.Clamp(vertical, -1, 1);
            Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
            if (movement != Vector3.zero) {
                pointerGameObject.transform.rotation = Quaternion.Slerp(pointerGameObject.transform.rotation, Quaternion.LookRotation(movement), 0.15F);
            }
            transform.position += movement * speed * Time.deltaTime;

            Vector3 tempPosition = transform.position;
            tempPosition.x = Mathf.Clamp(tempPosition.x, -maxDeviation, maxDeviation);
            tempPosition.z = Mathf.Clamp(tempPosition.z, -maxDeviation, maxDeviation);
            transform.position = tempPosition;
        }
    }

    public void OnDestroy() {
        GameManagerScript.instance.playerIsDead = true;
        Destroy(this.gameObject.GetComponent<ClampStackSize>().clampText);
    }
}