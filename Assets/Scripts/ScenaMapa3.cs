using UnityEngine;

public class ScenaMapa3 : MonoBehaviour
{
    void Start()
    {
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent)
        {
            agent.transform.position = new Vector3(0, 0.5f, -48.75f);
            agent.transform.rotation = Quaternion.Euler(0, 0, 0);
            SetColor(agent, new Color(0.9f, 0.9f, 0.1f));

            // Wyłącz inne FSM
            ParkingFSM_Rownolegle rownolegle = agent.GetComponent<ParkingFSM_Rownolegle>();
            if (rownolegle != null) rownolegle.enabled = false;
            
            ParkingFSM_Prostopadle prostopadle = agent.GetComponent<ParkingFSM_Prostopadle>();
            if (prostopadle != null) prostopadle.enabled = false;
            
            // Dodaj i włącz FSM skośne jeśli nie istnieje
            ParkingFSM_Skosnie skosnie = agent.GetComponent<ParkingFSM_Skosnie>();
            if (skosnie == null)
            {
                skosnie = agent.AddComponent<ParkingFSM_Skosnie>();
                Debug.Log("Dodano komponent ParkingFSM_Skosnie do AutoAgent");
            }
            skosnie.enabled = true;
        }
        else
        {
            Debug.LogError("Nie znaleziono obiektu AutoAgent!");
            return;
        }

        // Miejsca parkingowe co 7.5 jednostki od -48.75 do 48.75
        float[] miejscaZ = {-47.75f, -40.25f, -32.75f, -25.25f, -17.75f, -10.25f, -2.75f, 4.75f, 12.25f, 19.75f, 27.25f, 34.75f, 42.25f, 49.75f };
        
        Color[] kolory = new Color[]
        {
            new Color(0.8f, 0.1f, 0.1f),
            new Color(0.1f, 0.3f, 0.8f),
            new Color(0.1f, 0.7f, 0.2f),
            new Color(0.3f, 0.1f, 0.5f),
            new Color(0.9f, 0.4f, 0.1f),
            new Color(0.1f, 0.5f, 0.5f),
            new Color(0.6f, 0.2f, 0.2f),
            new Color(0.2f, 0.6f, 0.3f),
            new Color(0.5f, 0.5f, 0.1f),
            new Color(0.8f, 0.3f, 0.6f)
        };
        
        int licznikPrawo = 0;
        int licznikLewo = 0;
        
        // Najpierw wygeneruj wszystkie samochody
        for (int i = 0; i < miejscaZ.Length; i++)
        {
            // Przesunięcie o połowę szerokości miejsca (3.75f)
            float z = miejscaZ[i] - 3.75f;
            
            // Prawa strona - X = 5.6f
            Color kolorPrawo = kolory[licznikPrawo % kolory.Length];
            CreateCar($"AutoZaparkowane_Prawa_{i}", new Vector3(5.6f, 0.5f, z), kolorPrawo, 45f);
            licznikPrawo++;
            
            // Lewa strona - X = -5.5f
            Color kolorLewo = kolory[licznikLewo % kolory.Length];
            CreateCar($"AutoZaparkowane_Lewa_{i}", new Vector3(-5.5f, 0.5f, z), kolorLewo, -45f);
            licznikLewo++;
        }
        
        // === ZAMIEŃ WYBRANE SAMOCHODY NA MAŁE PRZESZKODY ===
        
        // Zamień AutoZaparkowane_Lewa_3 na przeszkodę (przy drodze, X = -3)
        GameObject lewa3 = GameObject.Find("AutoZaparkowane_Lewa_3");
        if (lewa3 != null)
        {
            Vector3 pos = lewa3.transform.position;
            Destroy(lewa3);
            // Przeszkoda przy samej drodze (alejka kończy się na -2.5)
            CreateObstacle("FalszywaLuka_Lewa_3", new Vector3(-3.0f, 0.5f, pos.z), new Vector3(0.5f, 1f, 1.125f), new Color(0.5f, 0.5f, 0.5f));
            Debug.Log("Zamieniono AutoZaparkowane_Lewa_3 na przeszkodę (X = -3)");
        }
        
        // Zamień AutoZaparkowane_Prawa_5 na przeszkodę (przy drodze, X = 3)
        GameObject prawa5 = GameObject.Find("AutoZaparkowane_Prawa_5");
        if (prawa5 != null)
        {
            Vector3 pos = prawa5.transform.position;
            Destroy(prawa5);
            // Przeszkoda przy samej drodze (alejka kończy się na 2.5)
            CreateObstacle("FalszywaLuka_Prawa_5", new Vector3(3.0f, 0.5f, pos.z), new Vector3(0.5f, 1f, 1.125f), new Color(0.5f, 0.5f, 0.5f));
            Debug.Log("Zamieniono AutoZaparkowane_Prawa_5 na przeszkodę (X = 3)");
        }
        
        // === TYLKO USUŃ AutoZaparkowane_Prawa_8 - BEZ PRZESZKODY ===
        GameObject prawa8 = GameObject.Find("AutoZaparkowane_Prawa_8");
        if (prawa8 != null)
        {
            Destroy(prawa8);
            Debug.Log("Usunięto AutoZaparkowane_Prawa_8");
        }
        
        GameObject planeObj = GameObject.Find("Plane");
        if (planeObj != null)
        {
            // Usuń starą teksturę jeśli istnieje
            ParkingTexture3 existingTexture = planeObj.GetComponent<ParkingTexture3>();
            if (existingTexture != null) Destroy(existingTexture);
            
            // Dodaj nową
            planeObj.AddComponent<ParkingTexture3>();
        }
        else
        {
            Debug.LogError("Nie znaleziono obiektu Plane!");
        }
        
        // === WŁĄCZ KAMERĘ ===
        EnableCamera();
    }
    
    void EnableCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Kamera kam = mainCam.GetComponent<Kamera>();
            if (kam != null)
            {
                kam.enabled = true;
                Debug.Log("Kamera włączona w ScenaMapa3");
            }
        }
    }

    void CreateCar(string name, Vector3 pos, Color color, float rotationY)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            existing.transform.position = pos;
            existing.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            SetColor(existing, color);
            return;
        }
        GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.name = name;
        car.transform.position = pos;
        car.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        car.transform.localScale = new Vector3(2f, 1f, 4.5f);
        SetColor(car, color);
    }

    void CreateObstacle(string name, Vector3 pos, Vector3 scale, Color color)
    {
        GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs.name = name;
        obs.transform.position = pos;
        obs.transform.rotation = Quaternion.Euler(0, 0, 0);
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