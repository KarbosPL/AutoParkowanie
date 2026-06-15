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

        // === PARKING SKOŚNY (SZERSZE MIEJSCA O 50%) ===
        
        // Prawa strona parkingu (X od 3 do 13 - szersze)
        int prawaStart = ToTexX(W, 3f);
        int prawaKoniec = ToTexX(W, 13f);
        
        // Lewa strona parkingu (X od -13 do -3 - szersze)
        int lewaStart = ToTexX(W, -13f);
        int lewaKoniec = ToTexX(W, -3f);
        
        // === MIEJSCA PARKINGOWE SKOŚNE ===
        // Co 7.5 jednostki w osi Z (50% szersze niż 5)
        float[] miejscaZ = { -48.75f, -41.25f, -33.75f, -26.25f, -18.75f, -11.25f, -3.75f, 3.75f, 11.25f, 18.75f, 26.25f, 33.75f, 41.25f, 48.75f };
        
        // Kąt 75 stopni (bez zmian)
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
        DrawV(tex, ToTexX(W, 13f), 0, H, linia, 4);
        DrawV(tex, ToTexX(W, -13f), 0, H, linia, 4);
        
        // Linie poprzeczne na końcach (Z = -48.75 i 48.75)
        int koniecDolny = ToTexY(H, -48.75f);
        int koniecGorny = ToTexY(H, 48.75f);
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