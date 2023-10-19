using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float defaultOffset = 4.0f;
    public float followSpeed = 5f;
    public Transform targetTransform;

    private Vector3 tempVec3 = new Vector3();
    private Transform bottomStack;
    private Transform topStack;
    private float offsetMultiplier = 1.5f;

    private void LateUpdate() {
        if (!GameManagerScript.instance.playerIsDead) {
            bottomStack = targetTransform.Find("PlayerStack");
            topStack = targetTransform.GetChild(targetTransform.childCount - 1);
            float offset = defaultOffset + (offsetMultiplier * (topStack.position.y - bottomStack.position.y));

            tempVec3.x = Mathf.Lerp(tempVec3.x, targetTransform.position.x, followSpeed * Time.deltaTime);
            tempVec3.y = Mathf.Lerp(tempVec3.y, targetTransform.position.y + offset, followSpeed * Time.deltaTime);
            tempVec3.z = Mathf.Lerp(tempVec3.z, targetTransform.position.z - (offset / 1.3f), followSpeed * Time.deltaTime);

            this.transform.position = tempVec3;
        }
    }
}