using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float speed = 5;
    public int stackSize = 1;
    public GameObject pointerGameObject;
    public Color prevStackColor;

    [HideInInspector]
    public string myName;

    [SerializeField]
    private List<Transform> collectibleStackList;

    private float rotationSpeed = 6;
    private float horizontal = 0.0f;
    private float vertical = 1.0f;

    // Start is called before the first frame update
    void Start() {
        collectibleStackList = new List<Transform>();
        int random = UnityEngine.Random.Range(0, Enum.GetNames(typeof(NameEnum)).Length);
        //myName = (Enum.GetValues(typeof(NameEnum))).GetValue(random).ToString();
    }

    // Update is called once per frame
    void Update() {
        if (GameManagerScript.instance.canMove) {
            Quaternion quaternion;
            Transform bestTarget = null;

            bestTarget = GetClosestCollectible();

            if (bestTarget != null) {
                transform.position = Vector3.MoveTowards(transform.position, bestTarget.position, speed * Time.deltaTime);
                Vector3 _direction = (bestTarget.position - transform.position).normalized;
                quaternion = Quaternion.LookRotation(_direction);
            } else {
                // TODO: Make it a little intelligent instead of just going up.
                Vector3 movement = new Vector3(horizontal, 0.0f, vertical);
                transform.position += movement * speed * Time.deltaTime;
                quaternion = Quaternion.LookRotation(movement);
            }

            pointerGameObject.transform.rotation = Quaternion.Slerp(pointerGameObject.transform.rotation, quaternion, rotationSpeed * Time.deltaTime);
        }
    }

    public void AddCollectible(Transform collectibleTransform) {
        if (!collectibleStackList.Contains(collectibleTransform)) {
            collectibleStackList.Add(collectibleTransform);
        }
    }

    public void RemoveCollectible(Transform collectibleTransform) {
        collectibleStackList.Remove(collectibleTransform);
    }

    private Transform GetClosestCollectible() {
        List<Transform> removeList = new List<Transform>();

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in collectibleStackList) {
            if (potentialTarget == null || !potentialTarget.gameObject.activeInHierarchy) {
                removeList.Add(potentialTarget);
            }

            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        collectibleStackList.RemoveAll(c => removeList.Exists(n => n == c));
        return bestTarget;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && other.GetType() == typeof(BoxCollider)) {
            vertical = -vertical;
        }

        if (other.tag == "Props" && other.GetType() == typeof(CapsuleCollider)) {
            horizontal = Vector3.zero.x - transform.position.normalized.x;
            vertical = Vector3.zero.z - transform.position.normalized.z;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && other.GetType() == typeof(BoxCollider)) {
            horizontal = -horizontal;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Enemy" && collision.collider.GetType() == typeof(BoxCollider)) {

            int enemyStackSize = collision.collider.gameObject.GetComponent<EnemyController>().stackSize;
            if (enemyStackSize > stackSize) {
                GameManagerScript.instance.enemyList.Remove(this.gameObject);
                Destroy(this.gameObject);
            } else {
                for (int i = 0; i < enemyStackSize; i++) {
                    GameManagerScript.instance.IncreaseEnemyStackSize(this.gameObject);
                }
                Destroy(collision.collider.gameObject);
                GameManagerScript.instance.PlayEnemyStackAnimation(this.gameObject);
            }
        }

        if (collision.collider.tag == "Player" && collision.collider.GetType() == typeof(BoxCollider)) {

            int playerStackSize = collision.collider.gameObject.GetComponent<PlayerController>().stackSize;
            if (playerStackSize >= stackSize) {
                for (int i = 0; i < stackSize; i++) {
                    GameManagerScript.instance.IncreasePlayerStackSize();
                }
                GameManagerScript.instance.ShowStackIncrementText(stackSize);
                GameManagerScript.instance.ShowEnemyKillText();
                GameManagerScript.instance.PlayPlayerStackAnimation();
                GameManagerScript.instance.enemyList.Remove(this.gameObject);
                Destroy(this.gameObject);
            } else {
                for (int i = 0; i < playerStackSize; i++) {
                    GameManagerScript.instance.IncreaseEnemyStackSize(this.gameObject);
                }
                Destroy(collision.collider.gameObject);
                GameManagerScript.instance.PlayEnemyStackAnimation(this.gameObject);
            }
        }
    }

    public void OnDestroy() {
        Destroy(this.gameObject.GetComponent<ClampStackSize>().clampText);
    }
}
