using UnityEngine;

public class TouchInteractionModel : MonoBehaviour
{
    private const float ROTATION_SPEED = 100f;
    private const float ZOOM_SPEED = 0.001f;
    private const float MOVE_THRESHOLD = 5f;
    private const float TAP_THRESHOLD = 0.2f;
    private const float MIN_SCALE = 0.01f;
    private const float MAX_SCALE = 10f;

    private float touchStartTime;
    private Vector2 touchStartPosition;
    private bool isDragging = false;
    private bool isInteracting = false;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private Vector3 initialScale;
    private float currentScale = 1f;
    private Transform arAnchor;

    void Start()
    {
        targetRotation = transform.rotation;
        targetPosition = transform.position;
        initialScale = transform.localScale;
        currentScale = initialScale.x;
        
        GameObject anchor = new GameObject($"{gameObject.name}_Anchor");
        anchor.transform.position = transform.position;
        anchor.transform.rotation = transform.rotation;
        arAnchor = anchor.transform;
        transform.parent = arAnchor;
    }

    void HandleRotation(Touch touch)
    {
        isInteracting = true;

        float rotationZ = -touch.deltaPosition.x * ROTATION_SPEED * Time.deltaTime;

        transform.Rotate(Vector3.forward, rotationZ, Space.Self);

        targetRotation = transform.rotation;
        Debug.Log("[AR_INTERACTION] Rotating model horizontally around Z-axis");
    }

    void HandleZoom(Touch touch0, Touch touch1)
    {
        isInteracting = true;

        Vector2 currentTouch0 = touch0.position;
        Vector2 currentTouch1 = touch1.position;
        Vector2 previousTouch0 = touch0.position - touch0.deltaPosition;
        Vector2 previousTouch1 = touch1.position - touch1.deltaPosition;

        float currentDistance = Vector2.Distance(currentTouch0, currentTouch1);
        float previousDistance = Vector2.Distance(previousTouch0, previousTouch1);

        float deltaDistance = currentDistance - previousDistance;
        
        float zoomFactor = deltaDistance * ZOOM_SPEED;
        currentScale = Mathf.Clamp(currentScale + zoomFactor, MIN_SCALE, MAX_SCALE);

        Vector3 newScale = Vector3.one * currentScale;
        transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * 10f);

        Debug.Log($"[AR_INTERACTION] Zooming - Current Scale: {currentScale}, Delta: {deltaDistance}");
    }

    public void UpdateFromTracking(Pose pose)
    {
        if (arAnchor != null)
        {
            arAnchor.position = pose.position;
            if (!isInteracting)
            {
                arAnchor.rotation = pose.rotation;
            }
        }
    }

    void Update()
    {
        if (Input.touchCount == 0)
        {
            isInteracting = false;
            isDragging = false;
            return;
        }

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            HandleZoom(touch0, touch1);
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartTime = Time.time;
                touchStartPosition = touch.position;
                isDragging = false;
                Debug.Log("[AR_INTERACTION] Touch began");
                break;

            case TouchPhase.Moved:
                float touchDistance = Vector2.Distance(touch.position, touchStartPosition);

                if (touchDistance > MOVE_THRESHOLD)
                {
                    isDragging = true;
                    HandleRotation(touch);
                }
                break;

            case TouchPhase.Ended:
                if (!isDragging && Time.time - touchStartTime < TAP_THRESHOLD)
                {
                    Debug.Log("[AR_INTERACTION] Tap detected");
                }
                targetRotation = transform.rotation;
                Debug.Log("[AR_INTERACTION] Touch ended");
                break;
        }
    }

    public void ResetTransform()
    {
        transform.localScale = initialScale;
        transform.rotation = Quaternion.identity;
        isInteracting = false;
        Debug.Log("[AR_INTERACTION] Transform reset");
    }
}
