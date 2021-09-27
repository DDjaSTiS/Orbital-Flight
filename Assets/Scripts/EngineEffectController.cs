using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineEffectController : MonoBehaviour
{
    [SerializeField] float sizeMinValue;
    [SerializeField] float sizeMaxValue;
    [SerializeField] float speedMinValue;
    [SerializeField] float speedMaxValue;
    private ParticleSystem particle;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        FindObjectOfType<ShipController>().thrustChanged += PlayEffect;
    }
    private void ResizeEffect(float levelOfThrust)
    {
        var mainSettings = particle.main;
        mainSettings.startSize = sizeMaxValue * levelOfThrust + sizeMinValue;
        mainSettings.startSpeed = speedMaxValue * levelOfThrust + speedMinValue;
    }
    
    private void PlayEffect(float levelOfThrust)
    {
        ResizeEffect(levelOfThrust);
        if (!particle.isPlaying && levelOfThrust > Mathf.Epsilon)
            particle.Play();
        else if (particle.isPlaying && levelOfThrust < Mathf.Epsilon)
            particle.Stop();
    }
}
