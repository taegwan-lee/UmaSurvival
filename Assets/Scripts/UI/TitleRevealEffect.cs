using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("")]
[RequireComponent(typeof(CanvasRenderer))]
public sealed class TitleRevealEffect : MaskableGraphic
{
    private const int PixelRows = 13;
    private const int PixelColumns = 24;
    private const int RingSegments = 64;

    private static readonly Color SkyLight = new Color32(230, 249, 255, 255);
    private static readonly Color SkyBlue = new Color32(169, 223, 249, 255);
    private static readonly Color Mint = new Color32(58, 219, 190, 255);
    private static readonly Color Cream = new Color32(255, 243, 192, 255);
    private static readonly Color Amber = new Color32(255, 187, 103, 255);
    private static readonly Color[] ConfettiColors = { Mint, Cream, Amber };

    private float elapsed;
    private Vector2 center;

    protected override void OnEnable()
    {
        useLegacyMeshGeneration = false;
        base.OnEnable();
    }

    internal void SetFrame(float seconds, Vector2 logoCenter)
    {
        elapsed = seconds;
        center = logoCenter;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vertices)
    {
        vertices.Clear();
        Rect bounds = GetPixelAdjustedRect();
        if (bounds.width <= 0f || bounds.height <= 0f)
        {
            return;
        }

        float shortSide = Mathf.Min(bounds.width, bounds.height);
        DrawPulse(vertices, shortSide, 0.13f, 0.65f, Amber);
        DrawPulse(vertices, shortSide, 0.24f, 0.74f, Cream);
        DrawConfetti(vertices, shortSide);
        DrawPixelWipe(vertices, bounds);
    }

    private void DrawPixelWipe(VertexHelper vertices, Rect bounds)
    {
        const float wipeEnd = 0.30f;
        if (elapsed >= wipeEnd)
        {
            return;
        }

        float travel = Mathf.InverseLerp(0f, wipeEnd, elapsed) * (PixelColumns + 9);
        float cellWidth = bounds.width / PixelColumns;
        float cellHeight = bounds.height / PixelRows;
        for (int row = 0; row < PixelRows; row++)
        {
            int delay = (row * 5 + 3) % 7;
            int column = Mathf.FloorToInt(travel - delay);
            float y = bounds.yMin + row * cellHeight;
            float edge = bounds.xMin + Mathf.Clamp(column, 0, PixelColumns) * cellWidth;
            if (column < PixelColumns)
            {
                Color bottom = Color.Lerp(SkyBlue, SkyLight, (float)row / PixelRows);
                Color top = Color.Lerp(SkyBlue, SkyLight, (row + 1f) / PixelRows);
                AddRect(vertices, edge, y, bounds.xMax - edge, cellHeight, bottom, top);
            }

            if (column >= 0 && column <= PixelColumns)
            {
                float leadingPixel = Mathf.Max(bounds.xMin, edge - cellWidth);
                Color accent = row % 3 == 0 ? Mint : row % 3 == 1 ? SkyBlue : Cream;
                accent.a = 0.8f;
                AddRect(vertices, leadingPixel, y, edge - leadingPixel, cellHeight, accent, accent);
            }
        }
    }

    private void DrawPulse(VertexHelper vertices, float shortSide, float start, float end, Color color)
    {
        float progress = Mathf.InverseLerp(start, end, elapsed);
        if (progress <= 0f || progress >= 1f)
        {
            return;
        }

        float easing = 1f - (1f - progress) * (1f - progress);
        float radius = shortSide * Mathf.Lerp(0.10f, 0.72f, easing);
        float thickness = shortSide * Mathf.Lerp(0.007f, 0.002f, progress);
        color.a = 0.67f * Mathf.Min(progress * 12f, 1f) * (1f - progress);

        int startIndex = vertices.currentVertCount;
        for (int segment = 0; segment <= RingSegments; segment++)
        {
            float angle = segment * (2f * Mathf.PI / RingSegments);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            vertices.AddVert(center + direction * (radius - thickness), color, Vector2.zero);
            vertices.AddVert(center + direction * radius, color, Vector2.zero);
            if (segment == RingSegments)
            {
                continue;
            }

            int first = startIndex + segment * 2;
            vertices.AddTriangle(first, first + 2, first + 3);
            vertices.AddTriangle(first, first + 3, first + 1);
        }
    }

    private void DrawConfetti(VertexHelper vertices, float shortSide)
    {
        float progress = Mathf.InverseLerp(0.31f, 0.83f, elapsed);
        if (progress <= 0f || progress >= 1f)
        {
            return;
        }

        for (int piece = 0; piece < 18; piece++)
        {
            float angle = 0.26f + piece * (2f * Mathf.PI / 18f);
            Vector2 direction = new Vector2(Mathf.Cos(angle) * 1.55f, Mathf.Sin(angle));
            float travel = shortSide * (0.26f + (0.12f + piece % 5 * 0.046f) * progress);
            Vector2 position = center + direction * travel + Vector2.down * (shortSide * 0.07f * progress * progress);
            float size = shortSide * (0.0065f + 0.0015f * (piece % 3));
            Color color = ConfettiColors[piece % ConfettiColors.Length];
            color.a = 0.82f * Mathf.Min(progress * 8f, 1f) * (1f - progress);
            AddRect(vertices, position.x, position.y, size, size * (piece % 2 == 0 ? 0.55f : 1f), color, color);
        }
    }

    private static void AddRect(VertexHelper vertices, float x, float y, float width, float height, Color bottom, Color top)
    {
        if (width <= 0f)
        {
            return;
        }

        int first = vertices.currentVertCount;
        vertices.AddVert(new Vector3(x, y), bottom, Vector2.zero);
        vertices.AddVert(new Vector3(x, y + height), top, Vector2.zero);
        vertices.AddVert(new Vector3(x + width, y + height), top, Vector2.zero);
        vertices.AddVert(new Vector3(x + width, y), bottom, Vector2.zero);
        vertices.AddTriangle(first, first + 1, first + 2);
        vertices.AddTriangle(first, first + 2, first + 3);
    }
}
