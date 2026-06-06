using UnityEngine;

public class WheelVisuals : MonoBehaviour
{
    private Rigidbody rb;
    private ParkingFSM2 fsm;

    private Transform wheelFL, wheelFR, wheelRL, wheelRR;

    void Start()
    {
        rb  = GetComponent<Rigidbody>();
        fsm = GetComponent<ParkingFSM2>();

        // Pozycje kół względem środka auta (auto ma scale X=2, Z=4.5)
        wheelFL = CreateWheel("Wheel_FL", new Vector3(-0.5f, -0.15f,  0.4f));
        wheelFR = CreateWheel("Wheel_FR", new Vector3( 0.5f, -0.15f,  0.4f));
        wheelRL = CreateWheel("Wheel_RL", new Vector3(-0.5f, -0.15f, -0.4f));
        wheelRR = CreateWheel("Wheel_RR", new Vector3( 0.5f, -0.15f, -0.4f));
    }

    Transform CreateWheel(string name, Vector3 localPos)
    {
        // Pusty obiekt jako pivot skrętu
        GameObject pivot = new GameObject(name + "_pivot");
        pivot.transform.SetParent(transform);
        pivot.transform.localPosition = localPos;
        pivot.transform.localRotation = Quaternion.identity;

        // Wizualny cylinder jako dziecko pivota
        GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheel.name = name;
        wheel.transform.SetParent(pivot.transform);
        wheel.transform.localPosition = Vector3.zero;
        // Koło leży poziomo - cylinder obrócony 90° wokół Z
        wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        // Płaski krążek: szerokość 0.25, promień 0.35
        wheel.transform.localScale = new Vector3(0.9f, 0.18f, 0.9f);

        Destroy(wheel.GetComponent<Collider>());

        Renderer r = wheel.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            r.material.color = new Color(0.1f, 0.1f, 0.1f);
        }

        return pivot.transform;
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        float direction = Vector3.Dot(rb.linearVelocity, transform.forward) >= 0 ? 1f : -1f;
        float rotAngle = speed * Time.deltaTime * 180f / (Mathf.PI * 0.35f);

        // Toczenie tylko tylnych kół
        wheelRL.GetChild(0).Rotate(0, rotAngle * direction, 0, Space.Self);
        wheelRR.GetChild(0).Rotate(0, rotAngle * direction, 0, Space.Self);

        // Skręt i toczenie przednich kół
        float steer = GetSteerAngle();
        wheelFL.localEulerAngles = new Vector3(0, steer, 0);
        wheelFR.localEulerAngles = new Vector3(0, steer, 0);
        wheelFL.GetChild(0).Rotate(0, rotAngle * direction, 0, Space.Self);
        wheelFR.GetChild(0).Rotate(0, rotAngle * direction, 0, Space.Self);
    }

    float GetSteerAngle()
    {
        if (fsm == null) return 0f;

        switch (fsm.currentState)
        {
            case ParkingFSM2.State.PerpRight:
            case ParkingFSM2.State.ParaRight1:
                return -30f;
            case ParkingFSM2.State.PerpLeft:
            case ParkingFSM2.State.ParaLeft1:
                return 30f;
            case ParkingFSM2.State.ParaRight2:
                return 20f;
            case ParkingFSM2.State.ParaLeft2:
                return -20f;
            default:
                return 0f;
        }
    }
}