using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavballController : MonoBehaviour
{
    [SerializeField] GameObject ship;

    [SerializeField] Canvas progradeCanvas;
    [SerializeField] Canvas retrogradeCanvas;
    [SerializeField] Canvas radialInCanvas;
    [SerializeField] Canvas normalCanvas;
    [SerializeField] Canvas antiNormalCanvas;
    [SerializeField] Canvas radialOutCanvas;
    private ManeurVectors maneurVectors;
    [SerializeField] float diameterOfNavball;
    // Start is called before the first frame update
    void Start()
    {
        maneurVectors = FindObjectOfType<ManeurVectors>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Prograde and retrograde icons.
        Physics.Raycast(transform.position + maneurVectors.ProgradeVector * 50f, -maneurVectors.ProgradeVector * 50f, out RaycastHit hitPrograde);
        progradeCanvas.transform.position = hitPrograde.point;
        progradeCanvas.transform.LookAt(transform);
        var retrogradePos = progradeCanvas.transform.position - maneurVectors.ProgradeVector * diameterOfNavball;
        retrogradeCanvas.transform.position = retrogradePos;
        retrogradeCanvas.transform.LookAt(transform);

        //Radial In and Out icons.
        Physics.Raycast(transform.position + maneurVectors.RadialInVector * 50f, -maneurVectors.RadialInVector * 50f, out RaycastHit hitRadialIn);
        radialInCanvas.transform.position = hitRadialIn.point;
        radialInCanvas.transform.LookAt(transform);
        var radialOutPos = radialInCanvas.transform.position - maneurVectors.RadialInVector * diameterOfNavball;
        radialOutCanvas.transform.position = radialOutPos;
        radialOutCanvas.transform.LookAt(transform);

        //Normal and Anti-Normal icons.
        Physics.Raycast(transform.position + maneurVectors.NormalVector * 50f, -maneurVectors.NormalVector * 50f, out RaycastHit hitNormal);
        normalCanvas.transform.position = hitNormal.point;
        normalCanvas.transform.LookAt(transform);
        var antiNormalPos = normalCanvas.transform.position - maneurVectors.NormalVector * diameterOfNavball;
        antiNormalCanvas.transform.position = antiNormalPos;
        antiNormalCanvas.transform.LookAt(transform);

        transform.rotation = ship.transform.rotation;
    }
}
