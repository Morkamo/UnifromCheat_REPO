using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace UnifromCheat_REPO.Utils
{
    public static class NoPostProcessingManager
    {
        private const float RefreshInterval = 1f;
        private static readonly string[] KnownPostProcessingObjects =
        {
            "Post Processing Main",
            "Post Processing Overlay"
        };

        private static readonly Dictionary<PostProcessLayer, bool> layerStates = new();
        private static readonly Dictionary<PostProcessVolume, bool> volumeStates = new();
        private static readonly Dictionary<GameObject, bool> objectStates = new();
        private static readonly List<PostProcessLayer> removedLayers = new();
        private static readonly List<PostProcessVolume> removedVolumes = new();
        private static readonly List<GameObject> removedObjects = new();
        private static bool applied;
        private static float nextRefreshTime;

        public static void SetEnabled(bool enabled)
        {
            if (!enabled)
            {
                RestoreAll();
                return;
            }

            Apply(true);
        }

        public static void Update(bool enabled)
        {
            if (!enabled)
            {
                if (applied)
                    RestoreAll();
                return;
            }

            if (!applied || Time.unscaledTime >= nextRefreshTime)
                Apply(false);
        }

        public static void RestoreAll()
        {
            RestoreLayerStates();
            RestoreVolumeStates();
            RestoreObjectStates();

            applied = false;
            nextRefreshTime = 0f;
        }

        private static void Apply(bool force)
        {
            if (!force && applied && Time.unscaledTime < nextRefreshTime)
                return;

            DisablePostProcessLayers();
            DisablePostProcessVolumes();
            DisableKnownPostProcessingObjects();

            applied = true;
            nextRefreshTime = Time.unscaledTime + RefreshInterval;
        }

        private static void DisablePostProcessLayers()
        {
            PostProcessLayer[] layers = Resources.FindObjectsOfTypeAll<PostProcessLayer>();
            foreach (PostProcessLayer layer in layers)
            {
                if (!IsSceneObject(layer))
                    continue;

                if (!layerStates.ContainsKey(layer))
                    layerStates[layer] = layer.enabled;

                layer.enabled = false;
            }
        }

        private static void DisablePostProcessVolumes()
        {
            PostProcessVolume[] volumes = Resources.FindObjectsOfTypeAll<PostProcessVolume>();
            foreach (PostProcessVolume volume in volumes)
            {
                if (!IsSceneObject(volume))
                    continue;

                if (!volumeStates.ContainsKey(volume))
                    volumeStates[volume] = volume.enabled;

                volume.enabled = false;
            }
        }

        private static void DisableKnownPostProcessingObjects()
        {
            foreach (string objectName in KnownPostProcessingObjects)
            {
                GameObject obj = GameObject.Find(objectName);
                if (obj == null)
                    continue;

                if (!objectStates.ContainsKey(obj))
                    objectStates[obj] = obj.activeSelf;

                obj.SetActive(false);
            }
        }

        private static void RestoreLayerStates()
        {
            removedLayers.Clear();
            foreach (var pair in layerStates)
            {
                PostProcessLayer layer = pair.Key;
                if (layer == null)
                {
                    removedLayers.Add(layer);
                    continue;
                }

                layer.enabled = pair.Value;
            }

            foreach (PostProcessLayer layer in removedLayers)
                layerStates.Remove(layer);
        }

        private static void RestoreVolumeStates()
        {
            removedVolumes.Clear();
            foreach (var pair in volumeStates)
            {
                PostProcessVolume volume = pair.Key;
                if (volume == null)
                {
                    removedVolumes.Add(volume);
                    continue;
                }

                volume.enabled = pair.Value;
            }

            foreach (PostProcessVolume volume in removedVolumes)
                volumeStates.Remove(volume);
        }

        private static void RestoreObjectStates()
        {
            removedObjects.Clear();
            foreach (var pair in objectStates)
            {
                GameObject obj = pair.Key;
                if (obj == null)
                {
                    removedObjects.Add(obj);
                    continue;
                }

                obj.SetActive(pair.Value);
            }

            foreach (GameObject obj in removedObjects)
                objectStates.Remove(obj);
        }

        private static bool IsSceneObject(Component component)
        {
            return component != null &&
                   component.gameObject != null &&
                   component.gameObject.scene.IsValid();
        }
    }
}
