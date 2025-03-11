using UnityEngine;

namespace UnifromCheat_REPO
{
    public class Loader
    {
        public static GameObject LoaderObject;
        
        public static void Load()
        {
            LoaderObject = new GameObject("UnifromLoader");
            LoaderObject.AddComponent<Core>();
            
            Object.DontDestroyOnLoad(LoaderObject);
        }
    }
}