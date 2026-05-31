using UnityEngine;

public class ScenaMapa1 : MonoBehaviour
{
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

        // ── RZĄd PRAWY — większe odstępy (co ~8f zamiast ~5.5f) ──────
        CreateCar("AutoZaparkowane_1", new Vector3(4f, 0.5f, 2f),
            new Color(0.8f, 0.1f, 0.1f));

        CreateCar("AutoZaparkowane_2", new Vector3(4f, 0.5f, 10f),
            new Color(0.1f, 0.3f, 0.8f));

        CreateObstacle("FalszywaLuka", new Vector3(4f, 0.5f, 18f),
            new Vector3(0.8f, 1f, 0.8f), new Color(0.5f, 0.5f, 0.5f));

        // Miejsce 4 - PUSTE (tu parkuje agent) @ z=26f

        CreateCar("AutoZaparkowane_3", new Vector3(4f, 0.5f, 34f),
            new Color(0.1f, 0.7f, 0.2f));

        // ── RZĄd LEWY ─────────────────────────────────────────────────
        CreateCar("AutoZaparkowane_4", new Vector3(-4f, 0.5f, 2f),
            new Color(0.3f, 0.1f, 0.5f));
        CreateCar("AutoZaparkowane_5", new Vector3(-4f, 0.5f, 10f),
            new Color(0.9f, 0.4f, 0.1f));
        CreateCar("AutoZaparkowane_6", new Vector3(-4f, 0.5f, 18f),
            new Color(0.1f, 0.5f, 0.5f));
        CreateCar("AutoZaparkowane_7", new Vector3(-4f, 0.5f, 26f),
            new Color(0.6f, 0.2f, 0.2f));
        CreateCar("AutoZaparkowane_8", new Vector3(-4f, 0.5f, 34f),
            new Color(0.2f, 0.6f, 0.3f));

        CreateAsfalt();
        CreateParkingLines();
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

    void CreateAsfalt()
    {
        GameObject alejka = GameObject.CreatePrimitive(PrimitiveType.Cube);
        alejka.name = "Alejka";
        alejka.transform.position = new Vector3(0f, 0.01f, 20f);
        alejka.transform.localScale = new Vector3(5f, 0.02f, 50f);
        SetColor(alejka, new Color(0.2f, 0.2f, 0.2f));
        Destroy(alejka.GetComponent<Collider>());

        GameObject parking_prawy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        parking_prawy.name = "ParkingPrawy";
        parking_prawy.transform.position = new Vector3(6.5f, 0.01f, 20f);
        parking_prawy.transform.localScale = new Vector3(7f, 0.02f, 50f);
        SetColor(parking_prawy, new Color(0.18f, 0.18f, 0.18f));
        Destroy(parking_prawy.GetComponent<Collider>());

        GameObject parking_lewy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        parking_lewy.name = "ParkingLewy";
        parking_lewy.transform.position = new Vector3(-6.5f, 0.01f, 20f);
        parking_lewy.transform.localScale = new Vector3(7f, 0.02f, 50f);
        SetColor(parking_lewy, new Color(0.18f, 0.18f, 0.18f));
        Destroy(parking_lewy.GetComponent<Collider>());
    }

    void CreateParkingLines()
    {
        // Linie co 8 jednostek, dopasowane do nowych pozycji aut
        // Każda linia między miejscami: -2, 6, 14, 22, 30, 38
        float[] zLines = { -2f, 6f, 14f, 22f, 30f, 38f };

        foreach (float z in zLines)
        {
            // Prawa strona
            GameObject lP = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lP.name = "LiniaP_" + z;
            lP.transform.position = new Vector3(5.5f, 0.02f, z);
            lP.transform.localScale = new Vector3(6f, 0.02f, 0.12f);
            SetColor(lP, Color.white);
            Destroy(lP.GetComponent<Collider>());

            // Lewa strona
            GameObject lL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lL.name = "LiniaL_" + z;
            lL.transform.position = new Vector3(-5.5f, 0.02f, z);
            lL.transform.localScale = new Vector3(6f, 0.02f, 0.12f);
            SetColor(lL, Color.white);
            Destroy(lL.GetComponent<Collider>());
        }

        // Krawężniki
        GameObject kP = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kP.name = "KraweznikP";
        kP.transform.position = new Vector3(2.5f, 0.1f, 20f);
        kP.transform.localScale = new Vector3(0.2f, 0.15f, 50f);
        SetColor(kP, new Color(0.7f, 0.7f, 0.7f));
        Destroy(kP.GetComponent<Collider>());

        GameObject kL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kL.name = "KraweznikL";
        kL.transform.position = new Vector3(-2.5f, 0.1f, 20f);
        kL.transform.localScale = new Vector3(0.2f, 0.15f, 50f);
        SetColor(kL, new Color(0.7f, 0.7f, 0.7f));
        Destroy(kL.GetComponent<Collider>());
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