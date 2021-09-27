using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterEffectController : MonoBehaviour
{
    [SerializeField] ParticleSystem upperEffect;
    [SerializeField] ParticleSystem lowerEffect;
    [SerializeField] ParticleSystem rightEffect;
    [SerializeField] ParticleSystem leftEffect;
    [SerializeField] bool inverted = false;
    [Header ("Only one of these can be true!")]
    [SerializeField] bool forVerticalAxis = true;
    [SerializeField] bool forHorizontalAxis = false;

    void Start()
    {
        FindObjectOfType<ShipController>().angularVelocityChanged += PlayThrusterEffects;
    }
    private void PlayThrusterEffects(float multiplier, string axisName) 
    {
        if (axisName == "Vertical" && forVerticalAxis)
            PlayEffectOnVerticalAxis(multiplier);
        else if (axisName == "Horizontal" && forHorizontalAxis)
            PlayEffectOnHorizontalAxis(multiplier);
        else if(axisName == "Roll")
            PlayEffectOnRollAxis(multiplier);
    }
    
    private void PlayEffectOnVerticalAxis(float multiplier)
    {
        if (Mathf.Abs(multiplier) < 0.01f)
        {
            upperEffect.Stop();
            lowerEffect.Stop();
            return;
        }
        if (multiplier > 0)
        {
            if (!inverted)
            {
                if(!lowerEffect.isPlaying)
                    lowerEffect.Play();
            }
            else
            {
                if(!upperEffect.isPlaying)
                    upperEffect.Play();
            }
        }
        else
        {
            if (!inverted)
            {
                if(!upperEffect.isPlaying)
                    upperEffect.Play();
            }                
            else
            {
                if(!lowerEffect.isPlaying)
                    lowerEffect.Play();
            }
        }
    }
    private void PlayEffectOnHorizontalAxis(float multiplier)
    {
        if (Mathf.Abs(multiplier) < 0.01f)
        {
            upperEffect.Stop();
            lowerEffect.Stop();
            return;
        }
        if (multiplier > 0)
        {
            if (!inverted)
            {
                if (!upperEffect.isPlaying)
                    upperEffect.Play();
            }
            else
            {
                if (!lowerEffect.isPlaying)
                    lowerEffect.Play();
            }
        }
        else
        {
            if (!inverted)
            {
                if (!lowerEffect.isPlaying)
                    lowerEffect.Play();
            }
            else
            {
                if (!upperEffect.isPlaying)
                    upperEffect.Play();
            }
        }
    }
    private void PlayEffectOnRollAxis(float multiplier)
    {
        if (Mathf.Abs(multiplier) < 0.01f)
        {
            leftEffect.Stop();
            rightEffect.Stop();
            return;
        }
        if (multiplier > 0)
        {
            if (!leftEffect.isPlaying)
                leftEffect.Play();
        }            
        else if(multiplier < 0)
        {
            if (!rightEffect.isPlaying)
                rightEffect.Play();
        }
    }
}
