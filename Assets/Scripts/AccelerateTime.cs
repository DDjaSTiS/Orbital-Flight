using UnityEngine;

public class AccelerateTime : MonoBehaviour
{
    [SerializeField] float timeScaleIncrease = 0.1f;
    void Update()
    {
        if (Input.GetKey(KeyCode.KeypadPlus) && Time.timeScale < 20)
        {
            Time.timeScale += timeScaleIncrease;
            Debug.Log(Time.timeScale.ToString());
        }
        if (Input.GetKey(KeyCode.KeypadMinus) && Time.timeScale > 1.1f)
        {
            Time.timeScale -= timeScaleIncrease;
            Debug.Log(Time.timeScale.ToString());
        }
    }
}
