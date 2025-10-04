using System.Threading.Tasks;
using UnifromCheat_REPO.Utils;
using UnityEngine;

namespace UnifromCheat_REPO
{
    public class Loader
    {
        public static GameObject LoaderObject;

        public static void Load()
        {
            AssemblyLoader.Init();
            FireboxConsole.Init();
            
            if (GameObject.Find("UnifromLoader") != null)
            {
                FireboxConsole.FireLog("Unifrom can only be one :)");
                return;
            }
            
            LoaderObject = new GameObject("UnifromLoader");
            LoaderObject.AddComponent<Core>();

            Object.DontDestroyOnLoad(LoaderObject);
        }

        /*public static async void Unload()
        {
            if (LoaderObject != null)
                UnityEngine.Object.DestroyImmediate(LoaderObject);

            FireboxConsole.FireLog("Good bye!");
            
            await Task.Delay(2000);
            
            FireboxConsole.Close();
        }*/
    }
}