using UnityEngine;

public static class PlaceholderSprites
{
    private static Sprite square;
    private static Sprite circle;

    public static Sprite Square
    {
        get
        {
            if (square == null)
            {
                Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                Color[] pixels = new Color[16 * 16];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = Color.white;
                }

                texture.SetPixels(pixels);
                texture.Apply();
                texture.filterMode = FilterMode.Point;
                texture.hideFlags = HideFlags.HideAndDontSave;
                square = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
                square.hideFlags = HideFlags.HideAndDontSave;
            }

            return square;
        }
    }

    public static Sprite Circle
    {
        get
        {
            if (circle == null)
            {
                const int size = 32;
                const float radius = size * 0.45f;
                Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
                Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                        texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
                    }
                }

                texture.Apply();
                texture.filterMode = FilterMode.Point;
                texture.hideFlags = HideFlags.HideAndDontSave;
                circle = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
                circle.hideFlags = HideFlags.HideAndDontSave;
            }

            return circle;
        }
    }
}
