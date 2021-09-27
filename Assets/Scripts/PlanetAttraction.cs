using UnityEngine;

public class PlanetAttraction : MonoBehaviour
{
    public float G { get => g; private set { } }

    [Tooltip("Gravitational const")] [SerializeField] float g = 0.00000000006674f * 10000;
    [SerializeField] GameObject attractor;

    private Rigidbody orbitingBodyRb;
    public void Attract(Rigidbody rBody, GameObject objToAttract)
    {
        var objRb = objToAttract.GetComponent<Rigidbody>();
        var direction = objRb.position - rBody.position;
        var distance = direction.magnitude;
        var magnitude = G * objRb.mass * rBody.mass / Mathf.Pow(distance, 2);
        rBody.AddForce(direction.normalized * magnitude);
    }

    private void Start()
    {
        orbitingBodyRb = gameObject.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Attract(orbitingBodyRb, attractor);
    }
}
