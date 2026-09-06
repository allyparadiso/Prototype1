using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheel;
    public WheelCollider wheelCollider;

    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    private void Update()
    {
        wheelCollider.GetWorldPose(out position, out rotation);
        wheel.transform.position = position;
        wheel.transform.rotation = rotation;
    }
}
