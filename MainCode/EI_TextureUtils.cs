using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;


namespace IINS.ExtendedInfo
{
	internal static class TextureUtils
	{
		// Dictionary to cache texture atlas lookups.
		private readonly static Dictionary<string, UITextureAtlas> textureCache = new Dictionary<string, UITextureAtlas>();



		/// <summary>
		/// Loads a cursor texture.
		/// </summary>
		/// <param name="cursorName">Cursor texture file name</param>
		/// <returns>New cursor</returns>
		//internal static CursorInfo LoadCursor(string cursorName)
		//{
		//	CursorInfo cursor = ScriptableObject.CreateInstance<CursorInfo>();

		//	cursor.m_texture = LoadTexture(cursorName);
		//	cursor.m_hotspot = new Vector2(5f, 0f);

		//	return cursor;
		//}


		/// <summary>
		/// Loads a four-sprite texture atlas from a given .png file.
		/// </summary>
		/// <param name="EIatlasName">Atlas name (".png" will be appended fto make the filename)</param>
		/// <returns>New texture atlas</returns>
		internal static UITextureAtlas LoadSpriteAtlas(string EIatlasName)
		{
			try
			{
				// Check if we've already cached this atlas.
				if (textureCache.ContainsKey(EIatlasName))
				{
					// Cached - return cached result.
					return textureCache[EIatlasName];
				}

				// Create new texture atlas for button.
				UITextureAtlas newAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
				newAtlas.name = EIatlasName;
				newAtlas.material = UnityEngine.Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);

				// Load texture from file.
				Texture2D newTexture = LoadTexture(EIatlasName + ".png");
				newAtlas.material.mainTexture = newTexture;

				//Setup sprites.
				string[] spriteNames = new string[] { "EIIcon1", "EIIcon2", "EIIcon3", "EIIcon4", "EIIcon5", "EIIcon6", "EIIcon7", "EIIcon8", "EIIcon9", "EIIcon10", "EIIcon11", "EIIcon12"};
				int numSprites = spriteNames.Length;
				float spriteWidth = 1f / spriteNames.Length;

				// Iterate through each sprite (counter increment is in region setup).
				for (int i = 0; i < numSprites; ++i)
				{
					UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo
					{
						name = spriteNames[i],
						texture = newTexture,
						// Sprite regions are horizontally arranged, evenly spaced.
						region = new Rect(i * spriteWidth, 0f, spriteWidth, 1f)
					};
					newAtlas.AddSprite(sprite);
				}

				// Add to cache and return.
				textureCache.Add(EIatlasName, newAtlas);
				return newAtlas;
			}
			catch
			//catch (Exception e)
            {
				//Logging.LogException(e, "exception loading texture atlas from file ", atlasName);
				return null;
            }
		}


		/// <summary>
		/// Returns the "ingame" atlas.
		/// </summary>
		//internal static UITextureAtlas InGameAtlas => GetTextureAtlas("ingame");
		internal static UITextureAtlas InGameAtlas 
		{
            get
            {
				return GetTextureAtlas("ingame");
            }
        }


		/// <summary>
		/// Returns a reference to the specified named atlas.
		/// </summary>
		/// <param name="EIatlasName">Atlas name</param>
		/// <returns>Atlas reference (null if not found)</returns>
		internal static UITextureAtlas GetTextureAtlas(string EIatlasName)
		{
			// Check if we've already cached this atlas.
			if (textureCache.ContainsKey(EIatlasName))
			{
				// Cached - return cached result.
				return textureCache[EIatlasName];
			}

			// No cache entry - get game atlases and iterate through, looking for a name match.
			UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
			for (int i = 0; i < atlases.Length; ++i)
			{
				if (atlases[i].name.Equals(EIatlasName))
				{
					// Got it - add to cache and return.
					textureCache.Add(EIatlasName, atlases[i]);
					return atlases[i];
				}
			}

			// IF we got here, we couldn't find the specified atlas.
			return null;
		}


		/// <summary>
		/// Loads a 2D texture from file.
		/// </summary>
		/// <param name="fileName">Texture file to load</param>
		/// <returns>New 2D texture</returns>
		private static Texture2D LoadTexture(string fileName)
		{
			using (Stream stream = OpenResourceFile(fileName))
			{
				// New texture.
				Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
				{
					//filterMode = FilterMode.Point,
					filterMode = FilterMode.Bilinear,
					//filterMode = FilterMode.Trilinear,
					wrapMode = TextureWrapMode.Clamp
				};

				// Read texture as byte stream from file.
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, array.Length);
				texture.LoadImage(array);
				//texture.LoadRawTextureData(array);
				texture.Apply();

				return texture;
			}
		}









		public static void ScaleTexture(Texture2D tex, int width, int height)
        {
            tex.filterMode = FilterMode.Bilinear;
            //tex.filterMode = FilterMode.Trilinear;
            var newPixels = new Color[width * height];
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    newPixels[y * width + x] = tex.GetPixelBilinear(((float)x) / width, ((float)y) / height);
                }
            }
            tex.Resize(width, height);
            tex.SetPixels(newPixels);
            tex.Apply();
        }


        public static void ScaleTexture2(Texture2D tex, int width, int height)
        {
            var newPixels = new Color[width * height];

            float ratio = ((float)width) / tex.width;
            if (tex.height * ratio > height)
            {
                ratio = ((float)height) / tex.height;
            }

            if (ratio > 1f) ratio = 1f;

            int newW = Mathf.RoundToInt(tex.width * ratio);
            int newH = Mathf.RoundToInt(tex.height * ratio);

            ScaleTexture(tex, newW, newH);
        }


        public static void CropTexture(Texture2D tex, int x, int y, int width, int height)
        {
            var newPixels = tex.GetPixels(x, y, width, height);
            tex.Resize(width, height);
            tex.SetPixels(newPixels);
            tex.Apply();
        }


        public static void RefreshAtlas(UITextureAtlas atlas)
        {
            Texture2D[] textures = new Texture2D[atlas.sprites.Count];

            int i = 0;
            foreach (UITextureAtlas.SpriteInfo sprite in atlas.sprites)
            {
                textures[i++] = sprite.texture;
            }
            atlas.AddTextures(textures);
        }

        

        public static Texture2D ConvertRenderTexture(RenderTexture renderTexture)
        {
            RenderTexture active = RenderTexture.active;
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;

            return texture2D;
        }

        public static void ResizeTexture(Texture2D texture, int width, int height)
        {
            RenderTexture active = RenderTexture.active;

            texture.filterMode = FilterMode.Bilinear;
            //texture.filterMode = FilterMode.Trilinear;
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);
            renderTexture.filterMode = FilterMode.Bilinear;
            //renderTexture.filterMode = FilterMode.Trilinear;

            RenderTexture.active = renderTexture;
            Graphics.Blit(texture, renderTexture);
            texture.Resize(width, height);
            texture.ReadPixels(new Rect(0, 0, width, width), 0, 0);
            texture.Apply();

            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        public static void CopyTexture(Texture2D texture2D, Texture2D dest)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
            Graphics.Blit(texture2D, renderTexture);

            RenderTexture active = RenderTexture.active;
            RenderTexture.active = renderTexture;
            dest.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
            dest.Apply();
            RenderTexture.active = active;

            RenderTexture.ReleaseTemporary(renderTexture);
        }




        //=========================================================================
        // Methods created by petrucio -> http:/answers.unity3d.com/questions/238922/png-transparency-has-white-borderhalo.html
        //
        // Copy the values of adjacent pixels to transparent pixels color info, to
        // remove the white border artifact when importing transparent .PNGs.
        public static void FixTransparency(Texture2D texture)
        {
            Color32[] pixels = texture.GetPixels32();
            int w = texture.width;
            int h = texture.height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int idx = y * w + x;
                    Color32 pixel = pixels[idx];
                    if (pixel.a == 0)
                    {
                        bool done = false;
                        if (!done && x > 0) done = TryAdjacent(ref pixel, pixels[idx - 1]);        // Left   pixel
                        if (!done && x < w - 1) done = TryAdjacent(ref pixel, pixels[idx + 1]);        // Right  pixel
                        if (!done && y > 0) done = TryAdjacent(ref pixel, pixels[idx - w]);        // Top    pixel
                        if (!done && y < h - 1) done = TryAdjacent(ref pixel, pixels[idx + w]);        // Bottom pixel
                        pixels[idx] = pixel;
                    }
                }
            }
    
            texture.SetPixels32(pixels);
            texture.Apply();
        }
    
        private static bool TryAdjacent(ref Color32 pixel, Color32 adjacent)
        {
            if (adjacent.a == 0) return false;
    
            pixel.r = adjacent.r;
            pixel.g = adjacent.g;
            pixel.b = adjacent.b;
            return true;
        }
        //=========================================================================

        





		/// <summary>
		/// Opens the named resource file for reading.
		/// </summary>
		/// <param name="fileName">File to open</param>
		/// <returns>Read-only file stream</returns>
		private static Stream OpenResourceFile(string fileName)
		{
			string path = Path.Combine(EIModUtils.GetAssemblyPath(), "EIIcons");
			return File.OpenRead(Path.Combine(path, fileName));
		}
	}
}
