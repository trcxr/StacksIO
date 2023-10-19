using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {
    public Transform targetPosition;
    private RectTransform pointerRect;

    private void Awake() {
        pointerRect = transform.Find("Pointer").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update() {
        Vector3 toPosition = targetPosition.position;
        Vector3 fromPosition = Camera.main.transform.position;
        fromPosition.y = 0;

        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = Vector3.Angle(fromPosition, toPosition);
        pointerRect.localEulerAngles = new Vector3(0, 0, angle);

        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition.position);

        bool isOffScreen = targetPositionScreenPoint.x <= 0 || targetPositionScreenPoint.x >= Screen.width
            || targetPositionScreenPoint.y <= 0 || targetPositionScreenPoint.y >= Screen.height;

        if (isOffScreen) {
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;

            if (cappedTargetScreenPosition.x <= 0) cappedTargetScreenPosition.x = 0f;
            if (cappedTargetScreenPosition.x >= Screen.width) cappedTargetScreenPosition.x = Screen.width;
            if (cappedTargetScreenPosition.y <= 0) cappedTargetScreenPosition.y = 0f;
            if (cappedTargetScreenPosition.y >= Screen.height) cappedTargetScreenPosition.y = Screen.height;

            Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(cappedTargetScreenPosition);
            pointerRect.position = pointerWorldPosition;

        }
    }
}
