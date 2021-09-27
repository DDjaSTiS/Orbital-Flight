using System;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRenderer : MonoBehaviour
{
    // TODO: Clean up code.
    public event Action<Vector3, Vector3> orbitUpdated;

    private Vector3 apogee;
    private Vector3 perigee;
    [SerializeField] GameObject ship;
    [SerializeField] Canvas apogeeCanvas;
    [SerializeField] Canvas perigeeCanvas;
    [SerializeField] Canvas shipPositionCanvas;
    [SerializeField] Camera _camera;
    [SerializeField] float uiScale = 0.00025f;
    private LineRenderer lineRendererComponent;
    void Start()
    {
        FindObjectOfType<OrbitParameters>().parametersChanged += DrawOrbit;
        lineRendererComponent = GetComponent<LineRenderer>();
    }
    private void LateUpdate()
    {
        shipPositionCanvas.transform.position = ship.transform.position;
        UpdateCanvasScaleAndOrientation();
    }
    private void DrawOrbit(OrbitParameters instance)
    {
        if(instance.E < Mathf.Epsilon)
            return;
        if (instance.E > 1)
            DrawHyperbola(180, instance);
        else
            DrawEllipse(360, instance);
        SetPositionsOfCanvases();
        if (orbitUpdated != null)
            orbitUpdated(apogee, perigee);
    }

    private void DrawEllipse(int numOfPoints, OrbitParameters parameters)
    {
        if (parameters == null)
            return;
        Vector3[] points = new Vector3[numOfPoints + 1];
        lineRendererComponent.positionCount = points.Length;
        float[] distances = new float[numOfPoints + 1];
        float k = 0;
        for (int i = 0; i <= numOfPoints; i++)
        {
            float u;
            distances[i] = parameters.A * (1 - parameters.E * (parameters.E + Mathf.Cos(k)) / (1 + parameters.E * Mathf.Cos(k)));
            u = parameters.W + k;
            float alpha = Mathf.Cos(parameters.Longitude) * Mathf.Cos(u) - Mathf.Sin(parameters.Longitude) * Mathf.Sin(u) * Mathf.Cos(parameters.I);
            float beta = Mathf.Sin(parameters.Longitude) * Mathf.Cos(u) + Mathf.Cos(parameters.Longitude) * Mathf.Sin(u) * Mathf.Cos(parameters.I);
            float gamma = Mathf.Sin(u) * Mathf.Sin(parameters.I);
            float x = distances[i] * alpha;
            float y = distances[i] * gamma;
            float z = distances[i] * beta;
            points[i] = new Vector3(x, y, z);
            if (i == 0)
                perigee = points[i];
            if (i == numOfPoints / 2)
                apogee = points[i];
            k += Mathf.PI * 2 / numOfPoints;
        }
        lineRendererComponent.SetPositions(points);        
    }
    private void DrawHyperbola(int numOfPoints, OrbitParameters parameters)
    {
        Vector3[] points = new Vector3[numOfPoints + 1];
        float[] distances = new float[numOfPoints + 1];
        float k = 0;
        for (int i = 0; i <= numOfPoints; i++)
        {
            float u;
            distances[i] = parameters.A * (1 - parameters.E * (parameters.E + Mathf.Cos(k)) / (1 + parameters.E * Mathf.Cos(k)));
            u = parameters.W + k;
            float alpha = Mathf.Cos(parameters.Longitude) * Mathf.Cos(u) - Mathf.Sin(parameters.Longitude) * Mathf.Sin(u) * Mathf.Cos(parameters.I);
            float beta = Mathf.Sin(parameters.Longitude) * Mathf.Cos(u) + Mathf.Cos(parameters.Longitude) * Mathf.Sin(u) * Mathf.Cos(parameters.I);
            float gamma = Mathf.Sin(u) * Mathf.Sin(parameters.I);
            float x = distances[i] * alpha;
            float y = distances[i] * gamma;
            float z = distances[i] * beta;
            points[i] = new Vector3(x, y, z);
            if (i == 0)
                perigee = points[i];
            k += Mathf.PI * 2 / numOfPoints;
        }
        var newPoints = TransformArrayForHyperbolaDrawing(points);
        lineRendererComponent.positionCount = newPoints.Length;
        lineRendererComponent.SetPositions(newPoints);
        apogee = Vector3.zero;
    }    
    private void SetPositionsOfCanvases()
    {
        if (apogee != Vector3.zero && !float.IsNaN(apogee.x))
            apogeeCanvas.transform.position = apogee;
        else
            apogeeCanvas.transform.position = Vector3.zero;
        if (perigee != Vector3.zero)
            perigeeCanvas.transform.position = perigee;
    }
    private Vector3[] TransformArrayForHyperbolaDrawing(Vector3[] points) // To get rid of the one of hyperbolas branches.
    {
        var maxMagnitudeFirstHalf = 0f;
        var indexOfMaxFirst = 0;
        var maxMagnitudeSecHalf = 0f;
        var indexOfMaxSec = 0;
        for (int i = 0, j = points.Length - 1; i < points.Length / 2; i++, j--)
        {
            var firstMagnitude = points[i].magnitude;
            if (firstMagnitude > maxMagnitudeFirstHalf)
            {
                maxMagnitudeFirstHalf = firstMagnitude;
                indexOfMaxFirst = i;
            }
            var secMagnitude = points[j].magnitude;
            if (secMagnitude > maxMagnitudeSecHalf)
            {
                maxMagnitudeSecHalf = secMagnitude;
                indexOfMaxSec = j;
            }
        }
        var transformedArray = new List<Vector3>();
        for(int j = indexOfMaxSec + 1; j < points.Length; j++)
        {
            transformedArray.Add(points[j]);
        }
        for(int i = 0; i <= indexOfMaxFirst - 1; i++)
        {
            transformedArray.Add(points[i]);
        }
        var result = transformedArray.ToArray();
        return result;
    }
   
    private void UpdateCanvasScaleAndOrientation()
    {
        SetUICanvasScaleAndOrientation(apogee, apogeeCanvas, uiScale);
        SetUICanvasScaleAndOrientation(perigee, perigeeCanvas, uiScale);
        SetUICanvasScaleAndOrientation(ship.transform.position, shipPositionCanvas, uiScale);
    }

    private void SetUICanvasScaleAndOrientation(Vector3 pointOnOrbit, Canvas uiCanvas, float scalingFactor)
    {
        float distancePointToCamera = (_camera.transform.position - pointOnOrbit).magnitude;
        uiCanvas.transform.LookAt(_camera.transform, _camera.transform.up);
        var newCanvasScale = Vector3.one * distancePointToCamera * scalingFactor;
        if(!float.IsNaN(newCanvasScale.x))
            uiCanvas.transform.localScale = newCanvasScale;
    }
}
