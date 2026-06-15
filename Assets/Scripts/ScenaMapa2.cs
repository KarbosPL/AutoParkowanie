using UnityEngine;

public class ScenaMapa2 : MonoBehaviour
{
    void Start()
    {
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent)
        {
            agent.transform.position = new Vector3(0, 0.5f, -45f);
            agent.transform.rotation = Quaternion.Euler(0, 0, 0);
            SetColor(agent, new Color(0.9f, 0.9f, 0.1f));

            agent.GetComponent<ParkingFSM_Rownolegle>().enabled = false;
            agent.GetComponent<ParkingFSM_Prostopadle>().enabled = true;   
        }

        // Miejsca parkingowe co 7 jednostek od -45 do 46
        float[] miejscaZ = { -46f, -39f, -32f, -25f, -18f, -11f, -4f, 3f, 10f, 17f, 24f, 31f, 38f, 45f };
        
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
            // Przesunięcie o połowę szerokości miejsca (3.5f)
            float z = miejscaZ[i] - 3.5f;
            
            // Prawa strona
            Color kolorPrawo = kolory[licznikPrawo % kolory.Length];
            CreateCar($"AutoZaparkowane_Prawa_{i}", new Vector3(5.5f, 0.5f, z), kolorPrawo, 90f);
            licznikPrawo++;
            
            // Lewa strona
            Color kolorLewo = kolory[licznikLewo % kolory.Length];
            CreateCar($"AutoZaparkowane_Lewa_{i}", new Vector3(-5.5f, 0.5f, z), kolorLewo, 90f);
            licznikLewo++;
        }
        
        // === ZAMIEŃ WYBRANE SAMOCHODY NA MAŁE PRZESZKODY (BLIŻEJ DROGI O 1.5f) ===
        
        // Usuń AutoZaparkowane_Lewa_3 (indeks 3) - przesuń bliżej drogi o 1.5f (z -5.5 na -4.0)
        GameObject lewa3 = GameObject.Find("AutoZaparkowane_Lewa_3");
        if (lewa3 != null)
        {
            Vector3 pos = lewa3.transform.position;
            Destroy(lewa3);
            CreateObstacle("FalszywaLuka_Lewa_3", new Vector3(-4.0f, 0.5f, pos.z), new Vector3(0.5f, 1f, 1.125f), new Color(0.5f, 0.5f, 0.5f));
            Debug.Log("Zamieniono AutoZaparkowane_Lewa_3 na przeszkodę (bliżej drogi o 1.5f)");
        }
        
        // Usuń AutoZaparkowane_Prawa_4 (indeks 4) - przesuń bliżej drogi o 1.5f (z 5.5 na 4.0)
        GameObject prawa4 = GameObject.Find("AutoZaparkowane_Prawa_4");
        if (prawa4 != null)
        {
            Vector3 pos = prawa4.transform.position;
            Destroy(prawa4);
            CreateObstacle("FalszywaLuka_Prawa_4", new Vector3(4.0f, 0.5f, pos.z), new Vector3(0.5f, 1f, 1.125f), new Color(0.5f, 0.5f, 0.5f));
            Debug.Log("Zamieniono AutoZaparkowane_Prawa_4 na przeszkodę (bliżej drogi o 1.5f)");
        }
        
        // === TYLKO USUŃ AutoZaparkowane_Prawa_8 - BEZ PRZESZKODY ===
        GameObject prawa8 = GameObject.Find("AutoZaparkowane_Prawa_8");
        if (prawa8 != null)
        {
            Destroy(prawa8);
            Debug.Log("Usunięto AutoZaparkowane_Prawa_8");
        }
        
        // Opcjonalnie - dodatkowa mała przeszkoda testowa
        CreateObstacle("FalszywaLuka", new Vector3(4.0f, 0.5f, -35f), new Vector3(0.5f, 1f, 1.125f), new Color(0.5f, 0.5f, 0.5f));

        GameObject planeObj = GameObject.Find("Plane");
        if (planeObj != null)
            planeObj.AddComponent<ParkingTexture2>();
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