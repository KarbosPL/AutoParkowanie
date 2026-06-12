using UnityEngine;

public class ParkingTexture3 : MonoBehaviour
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

        Color asfaltAlejka  = new Color(0.35f, 0.35f, 0.35f);
        Color asfaltParking = new Color(0.15f, 0.15f, 0.15f);
        Color linia         = Color.white;

        // Wypełnij całość asfaltem parkingowym
        Fill(tex, 0, 0, W, H, asfaltParking);

        // Alejka: Unity X = -2.5 do 2.5
        int aleL = ToTexX(W, -2.5f);
        int aleR = ToTexX(W, 2.5f);
        Fill(tex, aleL, 0, aleR - aleL, H, asfaltAlejka);

        // === PARKING SKOŚNY (45 stopni) ===
        
        // Prawa strona parkingu (X: 2.5 do 14)
        int prawaStart = ToTexX(W, 3.5f);
        int prawaKoniec = ToTexX(W, 14f);
        
        // Lewa strona parkingu (X: -14 do -2.5)
        int lewaStart = ToTexX(W, -14f);
        int lewaKoniec = ToTexX(W, -3.5f);
        
        // Linie zewnętrzne parkingów
        DrawV(tex, ToTexX(W, 14f), 0, H, linia, 4);
        DrawV(tex, ToTexX(W, -14f), 0, H, linia, 4);
        
        // === MIEJSCA PARKINGOWE SKOŚNE ===
        // Miejsca co 5 jednostek w osi Z
        float[] miejscaZ = { -45f, -40f, -35f, -30f, -25f, -20f, -15f, -10f, -5f, 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f };
        
        foreach (float z in miejscaZ)
        {
            int texY = ToTexY(H, z);
            
            // Rysuj linie skośne (45 stopni)
            DrawDiagonal(tex, texY, prawaStart, prawaKoniec, linia, 4, true);  // prawa strona - skos w prawo
            DrawDiagonal(tex, texY, lewaStart, lewaKoniec, linia, 4, false); // lewa strona - skos w lewo
        }
        
        // Linie poprzeczne na końcach parkingów
        float[] konceParkingu = { -47f, 47f };
        
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

    void DrawDiagonal(Texture2D tex, int y, int xStart, int xEnd, Color c, int thickness, bool rightSide)
    {
        int width = xEnd - xStart;
        for (int px = xStart; px < xEnd; px++)
        {
            // Oblicz przesunięcie dla skosu 45 stopni
            int offset = rightSide ? (px - xStart) : (xEnd - px - 1);
            int py = y - offset / 2;
            
            for (int t = -thickness/2; t <= thickness/2; t++)
            {
                int finalPy = Mathf.Clamp(py + t, 0, tex.height - 1);
                tex.SetPixel(px, finalPy, c);
            }
        }
    }

    int ToTexX(int W, float unityX) => (int)((unityX + 50f) / 100f * W);
    int ToTexY(int H, float unityZ) => (int)((unityZ + 50f) / 100f * H);

    void Fill(Texture2D tex, int x, int y, int w, int h, Color c)
    {
        for (int px = x; px < x + w && px < tex.width; px++)
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