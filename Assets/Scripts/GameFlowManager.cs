using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] float startingVelocity;
    [SerializeField] GameObject orbitingBody;
    [SerializeField] GameObject orbitingBodyCamera;
    [SerializeField] GameObject mapCamera;
    [SerializeField] GameObject orbitRenderer;

    private LineRenderer lr;
    private CameraMovement mapCamMov;    
    private Rigidbody orbitingRigidBody;
    void Start()
    {
        Application.targetFrameRate = 144;
        orbitingRigidBody = orbitingBody.GetComponent<Rigidbody>();
        mapCamMov = mapCamera.GetComponent<CameraMovement>();        
        lr = orbitRenderer.GetComponent<LineRenderer>();
        orbitingBodyCamera.SetActive(true);
        mapCamera.SetActive(false);
        orbitingRigidBody.AddForce(orbitingBody.transform.up * startingVelocity, ForceMode.VelocityChange);        
    }
    void Update()
    {
        lr.startWidth = lr.endWidth = mapCamMov.Distance / 500;
        if(Input.GetKeyDown(KeyCode.M))
        {
            if (orbitingBodyCamera.activeInHierarchy)
            {
                orbitingBodyCamera.SetActive(false);
                mapCamera.SetActive(true);
            }
            else
            {
                orbitingBodyCamera.SetActive(true);
                mapCamera.SetActive(false);
            }
        }
    }
}
