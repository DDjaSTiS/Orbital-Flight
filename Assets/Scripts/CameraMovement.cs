using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Distance { get => distance; private set { } }
        
    [Tooltip("Default distance to object")] [SerializeField] float distance = 10.0f;
    [SerializeField] float mouseSensitivity = 0.005f;
    [SerializeField] float minDistance = 3f;
    [SerializeField] float maxDistance = 25f;
    [SerializeField] GameObject body;
    private Vector3 originalMousePosition;
    private float verticalOffset;
    private float horizontalOffset;
    private float oldVertOffset = 0;
    private float oldHorOffset = 0;

   
    private void LateUpdate()
    {
        ProcessMouseInput();
        SetNewPositionAndOrientation();
        transform.LookAt(body.transform);
    }
    private void ProcessMouseInput()
    {
        distance = GetNewDistanceToObject(distance);
        if (Input.GetKeyDown(KeyCode.Mouse0))
            originalMousePosition = Input.mousePosition;  //remembering the position of mouse when clicking
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            oldHorOffset = horizontalOffset;  //remembering the last offset when releasing mouse button
            oldVertOffset = verticalOffset;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            verticalOffset = (Input.mousePosition.y - originalMousePosition.y) * mouseSensitivity + oldVertOffset;
            horizontalOffset = (Input.mousePosition.x - originalMousePosition.x) * mouseSensitivity + oldHorOffset;
            float maxVertAngle = Mathf.PI / 2 - 0.05f;
            if (verticalOffset > maxVertAngle)
            {
                oldVertOffset = verticalOffset = maxVertAngle;
                originalMousePosition.y = Input.mousePosition.y;
            }
            else if (verticalOffset < -maxVertAngle)
            {
                oldVertOffset = verticalOffset = -maxVertAngle;
                originalMousePosition.y = Input.mousePosition.y;
            }
        }
    }
    private float GetNewDistanceToObject(float originalDistance)
    {
        float rawDistance = originalDistance - Input.mouseScrollDelta.y / 2 * originalDistance * 0.1f;
        float clampedDistance = Mathf.Clamp(rawDistance, minDistance, maxDistance);
        return clampedDistance;
    }
    private void SetNewPositionAndOrientation()
    {
        Vector3 res = NewPositionOfCamera(verticalOffset, horizontalOffset, distance);
        transform.position = res;
    }
    private Vector3 NewPositionOfCamera(float vertOffset, float horizOffset, float distToObject)
    {
        float x, z, y;
        x = distToObject * Mathf.Sin(horizOffset) * Mathf.Cos(vertOffset) + body.transform.position.x;
        z = distToObject * Mathf.Cos(horizOffset) * Mathf.Cos(vertOffset) + body.transform.position.z;
        y = -distToObject * Mathf.Sin(vertOffset) + body.transform.position.y;
        return new Vector3(x, y, z);
    }
}
