using System;
using UnityEngine;

public class OrbitParameters : MonoBehaviour
{
    public static OrbitParameters instance;
    public float I { get => i; private set { } }
    public float Longitude { get => longitude; private set { } }
    public float A { get => a; private set { } }
    public float E { get => e; private set { } }
    public float W { get => w; private set { } }
    public Vector3 Normal { get => normal; private set { } }

    public event Action<OrbitParameters> parametersChanged;
    
    [Tooltip("Inclination of orbit: 0 <= i <= Pi")] [SerializeField] float i;
    [Tooltip("Longitude of ascending node: 0 <= longitude <= 2*Pi")] [SerializeField] float longitude;
    [Tooltip("Big half axis of orbit")] [SerializeField] float a;
    [Tooltip("Eccentricity of orbit")] [SerializeField] float e;
    [Tooltip("Pericenter argument: 0 <= w < 2*Pi")] [SerializeField] float w;
    [Tooltip("Normal vector")] [SerializeField] Vector3 normal;
    [SerializeField] float h;
    [SerializeField] GameObject planetAttractor;
    [SerializeField] GameObject orbitingBody;

    private Rigidbody orbitingBodyRb;
    private Rigidbody planetRb;
    private float G;
    private void Start()
    {
        instance = this;
        FindObjectOfType<ShipController>().velocityChanged += CalculateOrbitParameters;
        orbitingBodyRb = orbitingBody.GetComponent<Rigidbody>();
        planetRb = planetAttractor.GetComponent<Rigidbody>();
        G = GetGravitationalConst();
        Invoke("CalculateOrbitParameters", 0.2f);
    }

    private float GetGravitationalConst()
    {
        PlanetAttraction planetAttractionComponent = FindObjectOfType<PlanetAttraction>();
        return planetAttractionComponent.G;
    }
    private float CalculateLongitudeOfAscendingNode(float c1, float c2, float c, float i)
    {
        float longitude;
        if (i < 0.0000001f)
            longitude = -Mathf.PI / 2;
        else if(Mathf.Abs(i - Mathf.PI) < 0.0000001f)
            longitude = Mathf.PI/2;
        else
        {
            float cosSigma = -c2 / c / Mathf.Sin(i);
            if (cosSigma < -1)
                cosSigma = -1;
            else if (cosSigma > 1)
                cosSigma = 1;

            float sinSigma = c1 / c / Mathf.Sin(i);
            if (sinSigma < -1)
                sinSigma = -1;
            else if(sinSigma > 1)
                sinSigma = 1;

            if (sinSigma >= 0)
                longitude = Mathf.Acos(cosSigma);
            else
            {
                if (cosSigma < 0)
                    longitude = Mathf.PI - Mathf.Asin(sinSigma);
                else
                    longitude = 2 * Mathf.PI + Mathf.Asin(sinSigma);
            }
        }
        return longitude;
    }

    private float CalculateOmega(float f1, float f2, float f3,float f, float i, float sigma)
    {
        float omega;
        float cosOmega = (f1 * Mathf.Cos(sigma) + f2 * Mathf.Sin(sigma)) / f;
        if (i < 0.0000001f || Mathf.Abs(i - Mathf.PI) < 0.0000001f)
        {
            if (f1 > 0)
                omega = Mathf.Acos(cosOmega);
            else
                omega= 2 * Mathf.PI - Mathf.Acos(cosOmega);
        }
        else
        {
            float sinOmega = f3 / f / Mathf.Sin(i);
            if (sinOmega < -1)
                sinOmega = -1;
            else if (sinOmega > 1)
                sinOmega = 1;

            if (sinOmega >= 0)
                omega = Mathf.Acos(cosOmega);
            else
            {
                if (cosOmega < 0)
                    omega = Mathf.PI - Mathf.Asin(sinOmega);
                else
                    omega = 2 * Mathf.PI + Mathf.Asin(sinOmega);
            }
        }
        return omega;
    }

    private void CalculateOrbitParameters()
    {
        Vector3 bodyPos = orbitingBodyRb.position;
        Vector3 bodyVel = orbitingBodyRb.velocity;
        float c1 = bodyPos.z * bodyVel.y - bodyVel.z * bodyPos.y;
        float c2 = bodyVel.x * bodyPos.y - bodyPos.x * bodyVel.y;
        float c3 = bodyPos.x * bodyVel.z - bodyVel.x * bodyPos.z;       

        float c = Mathf.Sqrt(Mathf.Pow(c1, 2) + Mathf.Pow(c2, 2) + Mathf.Pow(c3, 2));
        float p = Mathf.Pow(c, 2) / planetRb.mass / G;
        float r = bodyPos.magnitude;
      
        i = Mathf.Acos(c3 / c);
       
        longitude = CalculateLongitudeOfAscendingNode(c1, c2, c, i);
        h = Mathf.Pow(bodyVel.magnitude, 2) - 2 * G * planetRb.mass / bodyPos.magnitude;
        
        a = -G * planetRb.mass / h;
        float D = bodyPos.x * bodyVel.x + bodyPos.y * bodyVel.y + bodyPos.z * bodyVel.z;
        float D1 = G * planetRb.mass / bodyPos.magnitude + h;

        float f1 = D1 * bodyPos.x - D * bodyVel.x;
        float f2 = D1 * bodyPos.z - D * bodyVel.z;
        float f3 = D1 * bodyPos.y - D * bodyVel.y;
        float f = Mathf.Sqrt(Mathf.Pow(f1, 2) + Mathf.Pow(f2, 2) + Mathf.Pow(f3, 2));
       
        e = f / G / planetRb.mass;

        float cosTheta = (p - r) / e / r;
        float sinTheta = D / e / r * Mathf.Sqrt(p/ G/ planetRb.mass);
        float theta1 = Mathf.Asin(sinTheta) * Mathf.Rad2Deg;
        float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

        w = CalculateOmega(f1, f2, f3, f, i, longitude);

        normal = new Vector3(c1, c3, c2).normalized;
        if (parametersChanged != null)
            parametersChanged(instance);
    }
}
