using UnityEngine;

public class ParkingTexture2 : MonoBehaviour
{
    void Start()
    {
        Texture2D tex = Generate();
        Renderer r = GetComponent<Renderer>();
        if (r == null) return;
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.mainTexture = tex;
        r.material = mat;
    }

    public static Texture2D parkingTexture;
    
    Texture2D Generate()
    {
        int W = 2048, H = 2048;
        Texture2D tex = new Texture2D(W, H);

        Color asfaltAlejka  = new Color(0.35f, 0.35f, 0.35f);  // Bardziej szarawy
        Color asfaltParking = new Color(0.15f, 0.15f, 0.15f);
        Color linia         = Color.white;

        // Wypełnij całość asfaltem parkingowym
        Fill(tex, 0, 0, W, H, asfaltParking);

        // Alejka: Unity X = -2.5 do 2.5
        int aleL = ToTexX(W, -2.5f);
        int aleR = ToTexX(W, 2.5f);
        Fill(tex, aleL, 0, aleR - aleL, H, asfaltAlejka);

        // === PARKING PROSTOPADŁY ===
        
        // Prawa strona parkingu (X: 2.5 do 12)
        int prawaStart = ToTexX(W, 2.5f);
        int prawaKoniec = ToTexX(W, 12f);
        
        // Lewa strona parkingu (X: -12 do -2.5)
        int lewaStart = ToTexX(W, -12f);
        int lewaKoniec = ToTexX(W, -2.5f);
        
        // Linie zewnętrzne parkingów (tylko zewnętrzne krawędzie)
        DrawV(tex, ToTexX(W, 12f), 0, H, linia, 4);
        DrawV(tex, ToTexX(W, -12f), 0, H, linia, 4);
        
        // === MIEJSCA PARKINGOWE ===
        // Miejsca co 4 jednostki w osi Z (szerokość miejsca = 4)
        float[] miejscaZ = { -44f, -40f, -36f, -32f, -28f, -24f, -20f, -16f, -12f, -8f, -4f, 0f, 4f, 8f, 12f, 16f, 20f, 24f, 28f, 32f, 36f, 40f, 44f };
        
        // Szerokość miejsca parkingowego = 4
        float miejsceSzerokosc = 4f;
        
        foreach (float z in miejscaZ)
        {
            int texY = ToTexY(H, z);
            
            // Prawa strona - miejsca parkingowe
            DrawH(tex, texY, prawaStart, prawaKoniec, linia, 4);
            
            // Lewa strona - miejsca parkingowe
            DrawH(tex, texY, lewaStart, lewaKoniec, linia, 4);
        }
        
        // Linie poprzeczne na końcach parkingów
        float[] konceParkingu = { -46f, 46f };
        
        foreach (float z in konceParkingu)
        {
            int texY = ToTexY(H, z);
            DrawH(tex, texY, prawaStart, prawaKoniec, linia, 4);
            DrawH(tex, texY, lewaStart, lewaKoniec, linia, 4);
        }

        tex.Apply();
        parkingTexture = tex;
        return tex;
    }

    int ToTexX(int W, float unityX) => (int)((unityX + 50f) / 100f * W);
    int ToTexY(int H, float unityZ) => (int)((unityZ + 50f) / 100f * H);

    void Fill(Texture2D tex, int x, int y, int w, int h, Color c)
    {
        for (int px = x; px < x + w && px < tex.width;  px++)
        for (int py = y; py < y + h && py < tex.height; py++)
            tex.SetPixel(px, py, c);
    }

    void DrawV(Texture2D tex, int x, int yStart, int yEnd, Color c, int thickness)
    {
        for (int t = -thickness/2; t <= thickness/2; t++)
        {
            int px = Mathf.Clamp(x + t, 0, tex.width - 1);
            for (int py = yStart; py < yEnd; py++)
                tex.SetPixel(px, py, c);
        }
    }

    void DrawH(Texture2D tex, int y, int xStart, int xEnd, Color c, int thickness)
    {
        for (int t = -thickness/2; t <= thickness/2; t++)
        {
            int py = Mathf.Clamp(y + t, 0, tex.height - 1);
            for (int px = xStart; px < xEnd; px++)
                tex.SetPixel(px, py, c);
        }
    }
}