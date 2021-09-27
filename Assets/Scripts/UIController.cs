using UnityEngine; 
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Slider thrustSlider;
    [SerializeField] Button sasButton;
    [SerializeField] Color sasTextColor;
    [SerializeField] InputField velocityField;
    [SerializeField] GameObject orbitingBody;
    [SerializeField] InputField altitudeField;
    [SerializeField] GameObject planet;
    [SerializeField] InputField apogeeField;
    [SerializeField] InputField perigeeField;
    [SerializeField] Button exitButton;
    [SerializeField] InputField controlsField;
    [SerializeField] Button controlsButton;

    private ShipController bodyMovementObject;
    private Rigidbody bodyRb;
    private bool isControlsShown = true;
    [SerializeField] float diameterOfPlanet;
    void Start()
    {
        FindObjectOfType<OrbitRenderer>().orbitUpdated += UpdateApogeeAndPerigeeFields;
        diameterOfPlanet = GetSizeOfObject(planet);
        bodyRb = orbitingBody.GetComponent<Rigidbody>();
        bodyMovementObject = FindObjectOfType<ShipController>();

        bodyMovementObject.thrustChanged += SetThrustSliderValue;
        bodyMovementObject.stabilityAssistChanged += ToggleStabilityAssist;
        
    }
    private void FixedUpdate()
    {
        UpdateVelocityField();
        UpdateAltitudeField();
    }
    
    public void Exit()
    {
        Application.Quit();
    }
    public void ToggleControlsField()
    {
        isControlsShown = !isControlsShown;
        ChangeTextOfControllsButton();
        RectTransform rt = controlsField.GetComponent<RectTransform>();
        if(isControlsShown)
            rt.sizeDelta = new Vector2(460, 400);
        else
            rt.sizeDelta = new Vector2(460, 75);
    }

    private void ChangeTextOfControllsButton()
    {
        var text = controlsButton.GetComponentInChildren<Text>();
        if (isControlsShown)
            text.text = "Hide";
        else
            text.text = "Show";
    }
    private void UpdateApogeeAndPerigeeFields(Vector3 apogee, Vector3 perigee)
    {
        if (apogee == Vector3.zero || float.IsNaN(apogee.magnitude))
            apogeeField.text = " ";
        else
        {
            var valueApogee = (apogee.magnitude - diameterOfPlanet / 2).ToString("00000000.");
            apogeeField.text = valueApogee;
        }
        var valuePerigee = (perigee.magnitude - diameterOfPlanet / 2).ToString("00000000.");
        perigeeField.text = valuePerigee;
    }

    private float GetSizeOfObject(GameObject gameObject)
    {
        var collider = gameObject.GetComponent<MeshCollider>();
        var size = collider.bounds.size;
        return size.x;
    }
    private void UpdateVelocityField()
    {
        var value = bodyRb.velocity.magnitude.ToString("0.00");
        velocityField.text = value + " m/s";
    }
    private void UpdateAltitudeField()
    {
        var value = bodyMovementObject.altitude.ToString("00000000.");
        altitudeField.text = value;
    }

    private void SetThrustSliderValue(float value)
    {
        thrustSlider.value = value;
    }
    private void ToggleStabilityAssist(bool stabilityAssistOn)
    {
        var text = sasButton.GetComponentInChildren<Text>();
        if (stabilityAssistOn)
        {
            sasButton.interactable = true;
            text.color = sasTextColor;
        }
        else
        {
            text.color = Color.gray;
            sasButton.interactable = false;
        }
    }
}
