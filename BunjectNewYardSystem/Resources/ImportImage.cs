using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiling.Visuals;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Bunject.NewYardSystem.Resources
{
  internal static class ImportImage
  {
    public static Sprite ImportSprite(string name, string path, Vector2 pivot, float pixelsPerUnit)
    {
      var texture = ImportTexture(name, path);
      return ImportSprite(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit, name);
    }

    public static Texture2D ImportTexture(string name, string path)
    {
      var bytes = File.ReadAllBytes(path);
      var newTexture = new Texture2D(2, 2);
			newTexture.LoadImage(bytes);
      // do not remove this
      newTexture.filterMode = FilterMode.Point;
      newTexture.name = name;
      newTexture.Apply();
      return newTexture;
    }

    public static Sprite ImportSprite(this Texture2D texture, Vector2Int position, Vector2Int size, Vector2 pivot, float pixelsPerUnit, string name)
    {
      return texture.ImportSprite(new Rect(position, size), pivot, pixelsPerUnit, name);
    }
    public static Sprite ImportSprite(this Texture2D texture, Rect spriteBound, Vector2 pivot, float pixelsPerUnit, string name)
    {
      var sprite = Sprite.Create(texture, spriteBound, pivot, pixelsPerUnit);
      sprite.name = name;
      return sprite;
    }

    public static TileBase ImportTile(this Texture2D texture, Vector2Int position, Vector2Int size, Vector2 pivot, int pixelsPerUnit,
      int frames = 1, Vector2Int offset = default, (float min, float max) speed = default, string name = "")
    {
      var tile = frames == 1 ? ScriptableObject.CreateInstance<Tile>() : ScriptableObject.CreateInstance<AnimatedTile>() as TileBase;
      if (tile is Tile @static)
      {
        @static.sprite = texture.ImportSprite(new Rect(position, size), pivot, pixelsPerUnit, name);
      }
      else if (tile is AnimatedTile animated)
      {
        animated.m_MinSpeed = speed.min;
        animated.m_MaxSpeed = speed.max;
        animated.m_AnimatedSprites = new Sprite[frames];
        for (int f = 0; f < frames; f++)
        {
          animated.m_AnimatedSprites[f] = texture.ImportSprite(new Rect(position + offset * f, size), pivot, pixelsPerUnit, $"{name}_{f}");
        }
      }
      tile.name = name;
      return tile;
    }
  }
}
