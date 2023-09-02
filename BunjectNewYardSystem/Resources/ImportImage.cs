using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bunject.NewYardSystem.Resources
{
  internal class ImportImage
  {
    public static Sprite ImportSprite(string name, string path, Vector2 pivot, float pixelsPerUnit)
    {
      var texture = ImportTexture(name, path);
      return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
    }
    
    public static Texture2D ImportTexture(string name, string path)
    {
      var bytes = File.ReadAllBytes(path);
      var newTexture = new Texture2D(2, 2);
      newTexture.LoadImage(bytes);
      newTexture.name = name;
      newTexture.Apply();
      return newTexture;
    }
  }
}
