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

        // Miejsca parkingowe co 6 jednostek od -45 do 45
        float[] miejscaZ = { -45f, -39f, -33f, -27f, -21f, -15f, -9f, -3f, 3f, 9f, 15f, 21f, 27f, 33f, 39f, 45f };
        
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
        
        for (int i = 0; i < miejscaZ.Length; i++)
        {
            // Przesunięcie o połowę szerokości miejsca (3f)
            float z = miejscaZ[i] - 3f;
            
            // Prawa strona - PRZESUNIĘTE BLIŻEJ DROGI O 1 (X = 5f zamiast 6f)
            Color kolorPrawo = kolory[licznikPrawo % kolory.Length];
            CreateCar($"AutoZaparkowane_Prawa_{i}", new Vector3(5f, 0.5f, z), kolorPrawo, 90f);
            licznikPrawo++;
            
            // Lewa strona - PRZESUNIĘTE BLIŻEJ DROGI O 1 (X = -5f zamiast -6f)
            Color kolorLewo = kolory[licznikLewo % kolory.Length];
            CreateCar($"AutoZaparkowane_Lewa_{i}", new Vector3(-5f, 0.5f, z), kolorLewo, 90f);
            licznikLewo++;
        }
        
        CreateObstacle("FalszywaLuka", new Vector3(5f, 0.5f, -35f), new Vector3(0.8f, 1f, 0.8f), new Color(0.5f, 0.5f, 0.5f));

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