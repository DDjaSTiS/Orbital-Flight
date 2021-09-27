
using UnityEngine;
using UnityEngine.VFX;

public class SunEffectController : MonoBehaviour
{
    public float Rate { get => rate; private set { } }

    [SerializeField] float rate;
    VisualEffect visualEffect;    
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        visualEffect.Play();
        visualEffect.playRate = 0.003f;
        rate = visualEffect.playRate;
    }
}
