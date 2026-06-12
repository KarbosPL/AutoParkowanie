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

        // === PARKING SKOŚNY (OSTRY KĄT 75 STOPNI) ===
        
        // Prawa strona parkingu (X od 3 do 11)
        int prawaStart = ToTexX(W, 3f);
        int prawaKoniec = ToTexX(W, 11f);
        
        // Lewa strona parkingu (X od -11 do -3)
        int lewaStart = ToTexX(W, -11f);
        int lewaKoniec = ToTexX(W, -3f);
        
        // === MIEJSCA PARKINGOWE SKOŚNE ===
        // Co 5 jednostek w osi Z
        float[] miejscaZ = { -45f, -40f, -35f, -30f, -25f, -20f, -15f, -10f, -5f, 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f };
        
        // OSTY KĄT - 75 stopni (prawie prostopadły)
        float skosAngle = 75f;
        
        foreach (float z in miejscaZ)
        {
            int texY = ToTexY(H, z);
            
            // Prawa strona - skos w prawo
            DrawDiagonal(tex, texY, prawaStart, prawaKoniec, linia, 4, skosAngle);
            
            // Lewa strona - skos w lewo
            DrawDiagonal(tex, texY, lewaStart, lewaKoniec, linia, 4, -skosAngle);
        }
        
        // Linie zewnętrzne na krańcach parkingów
        DrawV(tex, ToTexX(W, 11f), 0, H, linia, 4);
        DrawV(tex, ToTexX(W, -11f), 0, H, linia, 4);
        
        // Linie poprzeczne na końcach (Z = -47 i 47)
        int koniecDolny = ToTexY(H, -47f);
        int koniecGorny = ToTexY(H, 47f);
        DrawH(tex, koniecDolny, prawaStart, prawaKoniec, linia, 4);
        DrawH(tex, koniecDolny, lewaStart, lewaKoniec, linia, 4);
        DrawH(tex, koniecGorny, prawaStart, prawaKoniec, linia, 4);
        DrawH(tex, koniecGorny, lewaStart, lewaKoniec, linia, 4);

        tex.Apply();
        parkingTexture = tex;
        ParkingTexture1.parkingTexture = tex;  
        return tex;
    }

    void DrawDiagonal(Texture2D tex, int y, int xStart, int xEnd, Color c, int thickness, float angle)
    {
        float tanAngle = Mathf.Tan(angle * Mathf.Deg2Rad);
        int width = xEnd - xStart;
        
        for (int px = xStart; px < xEnd; px++)
        {
            // Oblicz przesunięcie w Y na podstawie X
            float t = (float)(px - xStart) / width;
            int offset = Mathf.RoundToInt(t * 50f * tanAngle);
            
            int py = y - offset;
            
            for (int t2 = -thickness/2; t2 <= thickness/2; t2++)
            {
                for (int t3 = -thickness/2; t3 <= thickness/2; t3++)
                {
                    int finalX = Mathf.Clamp(px + t2, 0, tex.width - 1);
                    int finalY = Mathf.Clamp(py + t3, 0, tex.height - 1);
                    tex.SetPixel(finalX, finalY, c);
                }
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