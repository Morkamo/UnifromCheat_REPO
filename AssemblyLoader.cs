using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnifromCheat_REPO
{
    public static class AssemblyLoader
    {
        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromResources;
        }

        private static Assembly ResolveFromResources(object sender, ResolveEventArgs args)
        {
            var requestedAssemblyName = new AssemblyName(args.Name);
            string shortName = requestedAssemblyName.Name;
            string fileName = shortName + ".dll";

            var loadedAssembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == shortName);

            if (loadedAssembly != null)
            {
                var loadedVersion = loadedAssembly.GetName().Version;
                if (loadedVersion >= requestedAssemblyName.Version)
                {
                    Debug.Log($"[AssemblyLoader] {shortName} is already loaded (version {loadedVersion}), skipping embedded load.");
                    return loadedAssembly;
                }
                else
                {
                    Debug.LogWarning($"[AssemblyLoader] Found older version of {shortName} ({loadedVersion}), loading embedded version {requestedAssemblyName.Version}.");
                }
            }

            string resourceName = Assembly.GetExecutingAssembly()
                .GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
            {
                Debug.LogError($"[AssemblyLoader] {shortName} not found among embedded resources.");
                return null;
            }

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Debug.LogError($"[AssemblyLoader] Failed to get resource stream for {shortName}.");
                    return null;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var assembly = Assembly.Load(ms.ToArray());
                    Debug.Log($"[AssemblyLoader] Loaded embedded {shortName} (version {assembly.GetName().Version}).");
                    return assembly;
                }
            }
        }
    }
}