using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        // Podłoże - asfalt
        GameObject plane = GameObject.Find("Plane");
        if (plane) SetColor(plane, new Color(0.15f, 0.15f, 0.15f));

        // AutoAgent
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent)
        {
            agent.transform.position = new Vector3(0, 0.5f, 0);
            SetColor(agent, new Color(0.9f, 0.9f, 0.1f)); // żółty
        }

        // Zaparkowane auta
        GameObject car1 = GameObject.Find("AutoZaparkowane_1");
        GameObject car2 = GameObject.Find("AutoZaparkowane_2");
        GameObject car3 = GameObject.Find("AutoZaparkowane_3");
        GameObject car4 = GameObject.Find("AutoZaparkowane_4");
        GameObject car5 = GameObject.Find("AutoZaparkowane_5");
        GameObject car6 = GameObject.Find("AutoZaparkowane_6");

        if (car1) { car1.transform.position = new Vector3(4.1f, 0.5f, 5);  SetColor(car1, new Color(0.8f, 0.1f, 0.1f)); } // czerwony
        if (car2) { car2.transform.position = new Vector3(4, 0.5f, 15); SetColor(car2, new Color(0.1f, 0.3f, 0.8f)); } // niebieski
        if (car3) { car3.transform.position = new Vector3(4.1f, 0.5f, 32); SetColor(car3, new Color(0.1f, 0.7f, 0.2f)); } // zielony
        if (car4) { car4.transform.position = new Vector3(-4, 0.5f, 5);  SetColor(car4, new Color(0.2f, 0.1f, 0.1f)); } // 
        if (car5) { car5.transform.position = new Vector3(-4, 0.5f, 15); SetColor(car5, new Color(0.9f, 0.4f, 0.2f)); } // 
        if (car6) { car6.transform.position = new Vector3(-4, 0.5f, 32); SetColor(car6, new Color(0.9f, 0.3f, 0.3f)); } // 

        // Droga
        CreateRoad();

        // Linie parkingowe
        CreateParkingLines();
    }

    void CreateRoad()
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "Droga";
        road.transform.position = new Vector3(-1f, 0.01f, 20f);
        road.transform.localScale = new Vector3(4f, 0.02f, 60f);
        SetColor(road, new Color(0.2f, 0.2f, 0.2f));
        Destroy(road.GetComponent<Collider>());
    }

    void CreateParkingLines()
    {
        // Linie wyznaczające miejsca parkingowe
        float[] zPositions = { 0f, 7f, 12f, 20f, 27f, 35f };
        foreach (float z in zPositions)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "LiniaParking";
            line.transform.position = new Vector3(4f, 0.02f, z);
            line.transform.localScale = new Vector3(5f, 0.02f, 0.15f);
            SetColor(line, Color.white);
            Destroy(line.GetComponent<Collider>());
        }

        // Linia krawężnika
        GameObject kraweznik = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kraweznik.name = "Kraweznik";
        kraweznik.transform.position = new Vector3(1.5f, 0.1f, 20f);
        kraweznik.transform.localScale = new Vector3(0.2f, 0.2f, 60f);
        SetColor(kraweznik, new Color(0.7f, 0.7f, 0.7f));
        Destroy(kraweznik.GetComponent<Collider>());
    }

    void SetColor(GameObject obj, Color color)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            r.material.color = color;
        }
    }
}