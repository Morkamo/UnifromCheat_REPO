using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnifromCheat_REPO.Utils;

public static class ResourceLoader
{
    public static Texture2D LoadTexture(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                FireboxConsole.FireLog($"[ResourceLoader] Resource not found: {resourceName}");
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(buffer);
            tex.filterMode = FilterMode.Point;
            return tex;
        }
    }
}