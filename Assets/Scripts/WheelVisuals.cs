using UnityEngine;

public class WheelVisuals : MonoBehaviour
{
    private Rigidbody rb;
    private ParkingFSM_Rownolegle fsm;

    private Transform wheelFL, wheelFR, wheelRL, wheelRR;
    private Transform wheelFL_visual, wheelFR_visual, wheelRL_visual, wheelRR_visual;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        fsm = GetComponent<ParkingFSM_Rownolegle>();

        if (fsm == null)
            Debug.LogError("ParkingFSM_Rownolegle nie znaleziony na obiekcie!");

        // Tworzenie kół
        CreateWheel("Wheel_FL", new Vector3(-0.5f, -0.15f, 0.4f), out wheelFL, out wheelFL_visual);
        CreateWheel("Wheel_FR", new Vector3(0.5f, -0.15f, 0.4f), out wheelFR, out wheelFR_visual);
        CreateWheel("Wheel_RL", new Vector3(-0.5f, -0.15f, -0.4f), out wheelRL, out wheelRL_visual);
        CreateWheel("Wheel_RR", new Vector3(0.5f, -0.15f, -0.4f), out wheelRR, out wheelRR_visual);
    }

    void CreateWheel(string name, Vector3 localPos, out Transform pivot, out Transform visual)
    {
        // Pivot do skrętu (tylko dla przednich kół)
        GameObject pivotObj = new GameObject(name + "_pivot");
        pivotObj.transform.SetParent(transform);
        pivotObj.transform.localPosition = localPos;
        pivotObj.transform.localRotation = Quaternion.identity;
        pivot = pivotObj.transform;

        // Wizualne koło (cylinder)
        GameObject wheelObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelObj.name = name;
        wheelObj.transform.SetParent(pivotObj.transform);
        wheelObj.transform.localPosition = Vector3.zero;
        wheelObj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        wheelObj.transform.localScale = new Vector3(0.9f, 0.18f, 0.9f);
        visual = wheelObj.transform;

        // Usuwamy kolider koła
        Destroy(wheelObj.GetComponent<Collider>());

        // Materiał
        Renderer r = wheelObj.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            r.material.color = new Color(0.1f, 0.1f, 0.1f);
        }
    }

    void Update()
    {
        // Obliczenie prędkości obrotu koła
        float speed = rb.linearVelocity.magnitude;
        float direction = Vector3.Dot(rb.linearVelocity, transform.forward) >= 0 ? 1f : -1f;
        float rotAngle = speed * Time.deltaTime * 180f / (Mathf.PI * 0.35f);

        // Tylne koła - tylko toczenie (bez skrętu)
        if (wheelRL_visual != null)
            wheelRL_visual.Rotate(0, rotAngle * direction, 0, Space.Self);
        if (wheelRR_visual != null)
            wheelRR_visual.Rotate(0, rotAngle * direction, 0, Space.Self);

        // Przednie koła - skręt + toczenie
        float steer = GetSteerAngle();

        if (wheelFL != null)
            wheelFL.localEulerAngles = new Vector3(0, steer, 0);
        if (wheelFR != null)
            wheelFR.localEulerAngles = new Vector3(0, steer, 0);

        if (wheelFL_visual != null)
            wheelFL_visual.Rotate(0, rotAngle * direction, 0, Space.Self);
        if (wheelFR_visual != null)
            wheelFR_visual.Rotate(0, rotAngle * direction, 0, Space.Self);
    }

    float GetSteerAngle()
    {
        if (fsm == null) return 0f;

        // Kąty skrętu dla różnych stanów parkowania
        switch (fsm.currentState)
        {
            // Parkowanie równoległe - prawa strona (faza 1 - skręt w prawo)
            case ParkingFSM_Rownolegle.State.ParaRight1:
                return -35f;

            // Parkowanie równoległe - prawa strona (faza 2 - wyrównanie)
            case ParkingFSM_Rownolegle.State.ParaRight2:
                return 25f;

            // Parkowanie równoległe - lewa strona (faza 1 - skręt w lewo)
            case ParkingFSM_Rownolegle.State.ParaLeft1:
                return 35f;

            // Parkowanie równoległe - lewa strona (faza 2 - wyrównanie)
            case ParkingFSM_Rownolegle.State.ParaLeft2:
                return -25f;

            // Pozostałe stany - bez skrętu
            default:
                return 0f;
        }
    }
}