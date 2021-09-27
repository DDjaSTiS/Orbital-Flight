using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManeurVectors : MonoBehaviour
{
    public Vector3 ProgradeVector { get => prograde; }
    public Vector3 RadialInVector { get => radialIn; }
    public Vector3 NormalVector { get => normal; }

    [SerializeField] GameObject orbitingBody;

    [SerializeField] Vector3 prograde;
    [SerializeField] Vector3 radialIn;
    [SerializeField] Vector3 normal;
    private ShipController bodyMovementObject;
    private Rigidbody rbOrbitingBody;
    // Start is called before the first frame update
    void Start()
    {
        bodyMovementObject = orbitingBody.GetComponent<ShipController>();
        rbOrbitingBody = orbitingBody.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (bodyMovementObject.isShipOnGround)
            return;
        UpdateManeurVectors();
    }

    private void UpdateManeurVectors()
    {
        prograde = rbOrbitingBody.velocity.normalized;
        if(OrbitParameters.instance != null)
            normal = OrbitParameters.instance.Normal;
        radialIn = Vector3.Cross(rbOrbitingBody.velocity, normal).normalized;
    }
}
