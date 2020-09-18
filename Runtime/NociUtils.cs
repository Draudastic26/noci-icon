using UnityEditor;
using UnityEngine;

namespace drstc.noci
{    
    public static class NociUtils
    {
        public static void SaveTextureAsPNG(Texture2D texture, string path)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            file.Directory.Create();
            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(path);
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(uniqueFileName, bytes);
            Debug.Log(bytes.Length + " bytes were saved as: " + uniqueFileName);
        }
    }
}