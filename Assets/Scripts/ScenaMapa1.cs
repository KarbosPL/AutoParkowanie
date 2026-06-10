using UnityEngine;

public class ScenaMapa1 : MonoBehaviour
{
    void Start()
    {
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent)
        {
            agent.transform.position = new Vector3(0, 0.5f, -45f);
            agent.transform.rotation = Quaternion.Euler(0, 0, 0);
            SetColor(agent, new Color(0.9f, 0.9f, 0.1f));

            agent.GetComponent<ParkingFSM_Rownolegle>().enabled = true;
          agent.GetComponent<ParkingFSM_Prostopadle>().enabled = false;   
        }

        // Rząd prawy
        CreateCar("AutoZaparkowane_1", new Vector3(4f, 0.5f, -15f),   new Color(0.8f, 0.1f, 0.1f));
        CreateCar("AutoZaparkowane_2", new Vector3(4f, 0.5f, -25f),  new Color(0.1f, 0.3f, 0.8f));
        CreateObstacle("FalszywaLuka", new Vector3(4f, 0.5f, -35f),  new Vector3(0.8f, 1f, 0.8f), new Color(0.5f, 0.5f, 0.5f));
        CreateCar("AutoZaparkowane_3", new Vector3(4f, 0.5f, -45f),  new Color(0.1f, 0.7f, 0.2f));
        CreateCar("AutoZaparkowane_10", new Vector3(4f, 0.5f, 15f),  new Color(0.1f, 0.7f, 0.2f));

        // Rząd lewy
        CreateCar("AutoZaparkowane_4", new Vector3(-4f, 0.5f, 5f),  new Color(0.3f, 0.1f, 0.5f));
        CreateCar("AutoZaparkowane_5", new Vector3(-4f, 0.5f, -15f), new Color(0.9f, 0.4f, 0.1f));
        CreateCar("AutoZaparkowane_6", new Vector3(-4f, 0.5f, -25f), new Color(0.1f, 0.5f, 0.5f));
        CreateCar("AutoZaparkowane_7", new Vector3(-4f, 0.5f, -35f), new Color(0.6f, 0.2f, 0.2f));
        CreateCar("AutoZaparkowane_8", new Vector3(-4f, 0.5f, -45f), new Color(0.2f, 0.6f, 0.3f));
        CreateCar("AutoZaparkowane_9", new Vector3(-4f, 0.5f, 15f), new Color(0.2f, 0.6f, 0.3f));

        // Tekstura na Plane
        GameObject planeObj = GameObject.Find("Plane");
        if (planeObj != null)
            planeObj.AddComponent<ParkingTexture1>();
    }

    void CreateCar(string name, Vector3 pos, Color color)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            existing.transform.position = pos;
            SetColor(existing, color);
            return;
        }
        GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.name = name;
        car.transform.position = pos;
        car.transform.localScale = new Vector3(2f, 1f, 4.5f);
        SetColor(car, color);
    }

    void CreateObstacle(string name, Vector3 pos, Vector3 scale, Color color)
    {
        GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs.name = name;
        obs.transform.position = pos;
        obs.transform.localScale = scale;
        SetColor(obs, color);
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