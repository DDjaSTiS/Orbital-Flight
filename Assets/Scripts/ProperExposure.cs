using UnityEngine;

public class ProperExposure : MonoBehaviour
{
    [Tooltip("Intensity of the effect")] [SerializeField] float intensity = 2;
    [SerializeField] float interp = 0.1f;
    [SerializeField] GameObject sun;
    [SerializeField] float horizontalMaxAngle = 40f;
    [SerializeField] float verticalMaxAngle = 25f;
    [SerializeField] GameObject objectOnScene; // Object receiving light from sun.

    private float avrgBrightness = 0f;
    private Collider objectCollider;
    private Vector3 sunPosition;
    private Collider cameraCollider;
    private int rayRawCount = 12;
    private int rayColCount = 12;

    
    void Start()
    {
        sunPosition = sun.transform.position;
        objectCollider = objectOnScene.GetComponent<Collider>();
        cameraCollider = GetComponent<Collider>();
    }
    void FixedUpdate()
    {
        avrgBrightness = GetAverageBrightnessOfCameraView(sunPosition);
        float exposure = RenderSettings.skybox.GetFloat("_Exposure");
        float newExposure = Mathf.Clamp(0.45f - avrgBrightness * intensity, 0f, 0.45f);
        RenderSettings.skybox.SetFloat("_Exposure", Mathf.SmoothStep(exposure, newExposure, interp));
        //RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(exposure, newExposure, interp));
    }
    private float GetAverageBrightnessOfCameraView(Vector3 positionOfSun)
    {
        int rayHitsCounter = 0;
        float sumOfBrightness = 0f;
        float avrgBrightness;
        Vector3 fromSunToCamera = transform.position - positionOfSun;
        bool isWatchingTheSun = IsSunOnCamera(horizontalMaxAngle, verticalMaxAngle, fromSunToCamera);
        //Debug.DrawRay(positionOfSun, fromSunToCamera);
        Physics.Raycast(positionOfSun, fromSunToCamera, out RaycastHit hit);
        if (hit.collider == null)
            return 0f;
        bool isSunBlocked = !hit.collider.Equals(cameraCollider);
        if(isSunBlocked)
        {
            avrgBrightness = 0f;
            return avrgBrightness;
        }
        else
        {
            if (isWatchingTheSun)
            {
                avrgBrightness = 1f;
                return avrgBrightness;
            }
            else
            {
                ProcessRaycastingToObject(ref rayHitsCounter, ref sumOfBrightness);
                if(rayHitsCounter == 0)
                {
                    avrgBrightness = 1f;
                }
                else
                {
                    avrgBrightness = sumOfBrightness / rayHitsCounter;
                }
                return avrgBrightness;
            }
        }
    }
    private float GetSizeOfObject(GameObject gameObject)
    {
        var mesh = gameObject.GetComponent<MeshFilter>();
        var size = mesh.sharedMesh.bounds.size;
        Vector4 resultSize = Matrix4x4.Scale(gameObject.transform.localScale) * size;
        return Mathf.Max(resultSize.x, resultSize.y, resultSize.z);
    }
    // Getting count of rays from sun which got into camera after bouncing of an object.
    // Each ray brings brightness (from 0 to 1). In result we have overall amount of brightness from object.
    private void ProcessRaycastingToObject(ref int countHit, ref float sumBrightness)
    {
        var halfSize = GetSizeOfObject(objectOnScene) / 2;
        float offsetVectorLength = halfSize / rayRawCount;
        for (int i = rayRawCount; i >= -rayRawCount; i -= 6)
        {
            for (int j = -rayColCount; j <= rayColCount; j += 6)
            {
                RaycastHit hit;
                Vector3 direction = (objectOnScene.transform.position - sunPosition +
                    j * sun.transform.right * offsetVectorLength +
                    i * sun.transform.up * offsetVectorLength);                
                Physics.Raycast(sunPosition, direction, out hit);
                if (hit.collider != null)
                {
                    var isHit = hit.collider.Equals(objectCollider);
                    if (isHit)
                    {
                        countHit++;
                        // We need to shoot new rays from an offset point of actual hitpoint of object in order to trigger its collider.
                        Vector3 offsetPoint = hit.point - 0.05f * halfSize * (hit.point - sunPosition).normalized;
                        Physics.Raycast(offsetPoint, transform.position - offsetPoint, out RaycastHit hit2);
                        if (hit2.collider == null)
                            break;
                        isHit = hit2.collider.Equals(cameraCollider);
                        if (isHit)
                        {
                            //Debug.DrawLine(sunPosition, hit.point, Color.yellow);
                            //Debug.DrawLine(offsetPoint, transform.position, Color.red);
                            Vector3 fromHitpointToSunPosition = sunPosition - hit.point;
                            // Angle between normal of surface the ray hit and the direction of sunlight.
                            var angle = Vector3.Angle(fromHitpointToSunPosition, hit.normal);
                            // We get 1 brightness (100%) when angle is 0 degrees, 0 brightness when angle is 90.
                            sumBrightness += Mathf.Cos(angle * Mathf.Deg2Rad);
                        }
                    }
                }
            }
        }
    }
    // Getting horizontal and vertical angles between line (camera-planet) and line (camera-sun) to check if sun is in camera's rectangle.
    private bool IsSunOnCamera(float maxHorAngle, float maxVertAngle, Vector3 _fromSunToCamera)
    {
        Vector3 proj1 = Vector3.ProjectOnPlane(_fromSunToCamera, transform.up);
        Vector3 proj2 = Vector3.ProjectOnPlane(-transform.forward, transform.up);
        Vector3 proj3 = Vector3.ProjectOnPlane(_fromSunToCamera, transform.right);
        Vector3 proj4 = Vector3.ProjectOnPlane(-transform.forward, transform.right);
        float verAngle = Vector3.Angle(proj3, proj4);
        float horAngle = Vector3.Angle(proj1, proj2);
        if (horAngle < maxHorAngle && verAngle < maxVertAngle)
            return true;
        else
            return false;
    }
}
