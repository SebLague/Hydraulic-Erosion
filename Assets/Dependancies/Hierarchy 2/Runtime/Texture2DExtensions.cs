using System;
using UnityEngine;

namespace Hierarchy2
{
    public static class Texture2DExtensions
    {
        public static string PNGImageEncodeBase64(this Texture2D texture2D)
        {
            var bytes = texture2D.EncodeToPNG();
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public static Texture2D PNGImageDecodeBase64(this string base64)
        {
            return Convert.FromBase64String(base64).PNGImageDecode();
        }

        public static Texture2D PNGImageDecode(this byte[] bytes)
        {
            Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            texture2D.hideFlags = HideFlags.HideAndDontSave;
#if UNITY_EDITOR
            texture2D.alphaIsTransparency = true;
#endif
            texture2D.LoadImage(bytes);
            texture2D.Apply();
            return texture2D;
        }
    }
}