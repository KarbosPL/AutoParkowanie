using UnityEngine;

public class ScenaMapa1 : MonoBehaviour
{
    void Start()
    {
        // Podłoże
        GameObject plane = GameObject.Find("Plane");
        if (plane) SetColor(plane, new Color(0.15f, 0.15f, 0.15f));

        // AutoAgent startuje w alejce między rzędami
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent)
        {
            agent.transform.position = new Vector3(0, 0.5f, -3f);
            agent.transform.rotation = Quaternion.Euler(0, 0, 0);
            SetColor(agent, new Color(0.9f, 0.9f, 0.1f));
        }

        // ── RZĄd PRAWY (miejsca prostopadłe) ──────────────────────────
        // Miejsce 1 - zajęte
        CreateCar("AutoZaparkowane_1", new Vector3(7f, 0.5f, 3f),
            new Color(0.8f, 0.1f, 0.1f));

        // Miejsce 2 - zajęte
        CreateCar("AutoZaparkowane_2", new Vector3(7f, 0.5f, 8.5f),
            new Color(0.1f, 0.3f, 0.8f));

        // Miejsce 3 - FAŁSZYWA LUKA (wózek sklepowy / auto wystające)
        // Mały obiekt który blokuje miejsce ale jest wąski
        CreateObstacle("FalszywaLuka", new Vector3(7f, 0.5f, 14f),
            new Vector3(0.8f, 1f, 0.8f), new Color(0.5f, 0.5f, 0.5f));

        // Miejsce 4 - PUSTE (tu ma zaparkować agent)
        // Brak obiektu - wolna przestrzeń

        // Miejsce 5 - zajęte
        CreateCar("AutoZaparkowane_3", new Vector3(7f, 0.5f, 22f),
            new Color(0.1f, 0.7f, 0.2f));

        // ── RZĄd LEWY (przeciwny rząd - tworzy alejkę) ───────────────
        CreateCar("AutoZaparkowane_4", new Vector3(-7f, 0.5f, 3f),
            new Color(0.3f, 0.1f, 0.5f));
        CreateCar("AutoZaparkowane_5", new Vector3(-7f, 0.5f, 8.5f),
            new Color(0.9f, 0.4f, 0.1f));
        CreateCar("AutoZaparkowane_6", new Vector3(-7f, 0.5f, 14f),
            new Color(0.1f, 0.5f, 0.5f));
        CreateCar("AutoZaparkowane_7", new Vector3(-7f, 0.5f, 19.5f),
            new Color(0.6f, 0.2f, 0.2f));
        CreateCar("AutoZaparkowane_8", new Vector3(-7f, 0.5f, 25f),
            new Color(0.2f, 0.6f, 0.3f));

        // ── NAWIERZCHNIA ──────────────────────────────────────────────
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
        // Alejka między rzędami
        GameObject alejka = GameObject.CreatePrimitive(PrimitiveType.Cube);
        alejka.name = "Alejka";
        alejka.transform.position = new Vector3(0f, 0.01f, 15f);
        alejka.transform.localScale = new Vector3(10f, 0.02f, 40f);
        SetColor(alejka, new Color(0.2f, 0.2f, 0.2f));
        Destroy(alejka.GetComponent<Collider>());

        // Powierzchnia miejsc parkingowych prawe
        GameObject parking_prawy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        parking_prawy.name = "ParkingPrawy";
        parking_prawy.transform.position = new Vector3(9.5f, 0.01f, 15f);
        parking_prawy.transform.localScale = new Vector3(7f, 0.02f, 40f);
        SetColor(parking_prawy, new Color(0.18f, 0.18f, 0.18f));
        Destroy(parking_prawy.GetComponent<Collider>());

        // Powierzchnia miejsc parkingowych lewe
        GameObject parking_lewy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        parking_lewy.name = "ParkingLewy";
        parking_lewy.transform.position = new Vector3(-9.5f, 0.01f, 15f);
        parking_lewy.transform.localScale = new Vector3(7f, 0.02f, 40f);
        SetColor(parking_lewy, new Color(0.18f, 0.18f, 0.18f));
        Destroy(parking_lewy.GetComponent<Collider>());
    }

    void CreateParkingLines()
    {
        // Linie prawego rzędu
        float[] zLines = { 0.5f, 6f, 11.5f, 17f, 22.5f, 28f };
        foreach (float z in zLines)
        {
            // Prawa strona
            GameObject lP = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lP.name = "LiniaP";
            lP.transform.position = new Vector3(8f, 0.02f, z);
            lP.transform.localScale = new Vector3(6f, 0.02f, 0.12f);
            SetColor(lP, Color.white);
            Destroy(lP.GetComponent<Collider>());

            // Lewa strona
            GameObject lL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lL.name = "LiniaL";
            lL.transform.position = new Vector3(-8f, 0.02f, z);
            lL.transform.localScale = new Vector3(6f, 0.02f, 0.12f);
            SetColor(lL, Color.white);
            Destroy(lL.GetComponent<Collider>());
        }

        // Krawężniki
        GameObject kP = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kP.name = "KraweznikP";
        kP.transform.position = new Vector3(5f, 0.1f, 15f);
        kP.transform.localScale = new Vector3(0.2f, 0.15f, 40f);
        SetColor(kP, new Color(0.7f, 0.7f, 0.7f));
        Destroy(kP.GetComponent<Collider>());

        GameObject kL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kL.name = "KraweznikL";
        kL.transform.position = new Vector3(-5f, 0.1f, 15f);
        kL.transform.localScale = new Vector3(0.2f, 0.15f, 40f);
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