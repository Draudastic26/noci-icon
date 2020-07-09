using System.IO;
using UnityEngine;

namespace drstc.nociincon
{    
    public static class NociUtils
    {
        public const string EXTENSION_PNG = ".png";

        public static void SaveTextureAsPNG(Texture2D texture, string path, string fileName)
        {
            fileName += EXTENSION_PNG;
            var filePath = Path.Combine(path, fileName);
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(file.FullName, bytes);
            Debug.Log(bytes.Length + "bytes were saved as: " + file.FullName);
        }
    }
}