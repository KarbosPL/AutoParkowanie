using UnityEngine;

public class ScenaMapa3 : MonoBehaviour  // ← było ScenaMapa1
{
    float[] slotZ = { 2f, 5f, 8f, 11f, 14f, 17f, 20f, 23f, 26f };

    void Start()
    {
        GameObject plane = GameObject.Find("Plane");
        if (plane) SetColor(plane, new Color(0.15f, 0.15f, 0.15f));

        GameObject agent = GameObject.Find("AutoAgent");
        if (agent)
        {
            agent.transform.position = new Vector3(0, 0.5f, -3f);
            agent.transform.rotation = Quaternion.Euler(0, 0, 0);
            SetColor(agent, new Color(0.9f, 0.9f, 0.1f));
        }

        SpawnCars();
        CreateAsfalt();
        CreateParkingLines();
    }

    void SpawnCars()
    {
        CreateCar("AutoP_0", new Vector3(6f, 0.5f, slotZ[0]), 90f, new Color(0.8f, 0.1f, 0.1f));
        CreateCar("AutoP_1", new Vector3(6f, 0.5f, slotZ[1]), 90f, new Color(0.1f, 0.3f, 0.8f));
        CreateCar("AutoP_2", new Vector3(6f, 0.5f, slotZ[2]), 90f, new Color(0.6f, 0.2f, 0.6f));
        // Miejsce 3 - PUSTE
        CreateObstacle("FalszywaLuka", new Vector3(6f, 0.5f, slotZ[4]),
            new Vector3(0.8f, 1f, 0.8f), new Color(0.5f, 0.5f, 0.5f));
        CreateCar("AutoP_5", new Vector3(6f, 0.5f, slotZ[5]), 90f, new Color(0.1f, 0.7f, 0.2f));
        CreateCar("AutoP_6", new Vector3(6f, 0.5f, slotZ[6]), 90f, new Color(0.9f, 0.5f, 0.1f));
        CreateCar("AutoP_7", new Vector3(6f, 0.5f, slotZ[7]), 90f, new Color(0.2f, 0.5f, 0.8f));
        CreateCar("AutoP_8", new Vector3(6f, 0.5f, slotZ[8]), 90f, new Color(0.7f, 0.1f, 0.3f));

        CreateCar("AutoL_0", new Vector3(-6f, 0.5f, slotZ[0]), -90f, new Color(0.3f, 0.1f, 0.5f));
        CreateCar("AutoL_1", new Vector3(-6f, 0.5f, slotZ[1]), -90f, new Color(0.9f, 0.4f, 0.1f));
        // Miejsce 2 - PUSTE
        CreateCar("AutoL_3", new Vector3(-6f, 0.5f, slotZ[3]), -90f, new Color(0.1f, 0.5f, 0.5f));
        CreateCar("AutoL_4", new Vector3(-6f, 0.5f, slotZ[4]), -90f, new Color(0.6f, 0.2f, 0.2f));
        CreateCar("AutoL_5", new Vector3(-6f, 0.5f, slotZ[5]), -90f, new Color(0.2f, 0.6f, 0.3f));
        CreateCar("AutoL_6", new Vector3(-6f, 0.5f, slotZ[6]), -90f, new Color(0.8f, 0.7f, 0.1f));
        CreateCar("AutoL_7", new Vector3(-6f, 0.5f, slotZ[7]), -90f, new Color(0.4f, 0.1f, 0.7f));
        CreateCar("AutoL_8", new Vector3(-6f, 0.5f, slotZ[8]), -90f, new Color(0.1f, 0.4f, 0.2f));
    }

    void CreateCar(string name, Vector3 pos, float rotY, Color color)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            existing.transform.position = pos;
            existing.transform.rotation = Quaternion.Euler(0, rotY, 0);
            SetColor(existing, color);
            return;
        }

        GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.name = name;
        car.transform.position = pos;
        car.transform.localScale = new Vector3(2f, 1f, 4.5f);
        car.transform.rotation = Quaternion.Euler(0, rotY, 0);
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

    void CreateAsfalt()
    {
        float length = 32f;
        float centerZ = 14f;

        GameObject alejka = GameObject.CreatePrimitive(PrimitiveType.Cube);
        alejka.name = "Alejka";
        alejka.transform.position = new Vector3(0f, 0.01f, centerZ);
        alejka.transform.localScale = new Vector3(5f, 0.02f, length);
        SetColor(alejka, new Color(0.2f, 0.2f, 0.2f));
        Destroy(alejka.GetComponent<Collider>());

        GameObject parkP = GameObject.CreatePrimitive(PrimitiveType.Cube);
        parkP.name = "ParkingPrawy";
        parkP.transform.position = new Vector3(5.75f, 0.01f, centerZ);
        parkP.transform.localScale = new Vector3(6f, 0.02f, length);
        SetColor(parkP, new Color(0.18f, 0.18f, 0.18f));
        Destroy(parkP.GetComponent<Collider>());

        GameObject parkL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        parkL.name = "ParkingLewy";
        parkL.transform.position = new Vector3(-5.75f, 0.01f, centerZ);
        parkL.transform.localScale = new Vector3(6f, 0.02f, length);
        SetColor(parkL, new Color(0.18f, 0.18f, 0.18f));
        Destroy(parkL.GetComponent<Collider>());
    }

    void CreateParkingLines()
    {
        float length = 32f;
        float centerZ = 14f;

        GameObject kP = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kP.name = "KraweznikP";
        kP.transform.position = new Vector3(2.5f, 0.1f, centerZ);
        kP.transform.localScale = new Vector3(0.2f, 0.15f, length);
        SetColor(kP, new Color(0.7f, 0.7f, 0.7f));
        Destroy(kP.GetComponent<Collider>());

        GameObject kL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kL.name = "KraweznikL";
        kL.transform.position = new Vector3(-2.5f, 0.1f, centerZ);
        kL.transform.localScale = new Vector3(0.2f, 0.15f, length);
        SetColor(kL, new Color(0.7f, 0.7f, 0.7f));
        Destroy(kL.GetComponent<Collider>());

        float[] lineZ = { 0.5f, 3.5f, 6.5f, 9.5f, 12.5f, 15.5f, 18.5f, 21.5f, 24.5f, 27.5f };

        foreach (float z in lineZ)
        {
            GameObject lP = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lP.name = "LinP_" + z;
            lP.transform.position = new Vector3(5.75f, 0.02f, z);
            lP.transform.localScale = new Vector3(6f, 0.02f, 0.1f);
            SetColor(lP, Color.white);
            Destroy(lP.GetComponent<Collider>());

            GameObject lL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lL.name = "LinL_" + z;
            lL.transform.position = new Vector3(-5.75f, 0.02f, z);
            lL.transform.localScale = new Vector3(6f, 0.02f, 0.1f);
            SetColor(lL, Color.white);
            Destroy(lL.GetComponent<Collider>());
        }
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