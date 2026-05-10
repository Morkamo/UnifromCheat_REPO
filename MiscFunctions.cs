using UnifromCheat_REPO.Utils;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnifromCheat_REPO.WallHack;
using UnityEngine;

namespace UnifromCheat_REPO
{
    public class MiscFunctions : MonoBehaviour
    {
        public static MiscFunctions Instance;
        private static readonly Dictionary<Camera, float> defaultCameraFovs = new();

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            _ = typeof(FullbrightManager);
            this.AddComponent<RGBPlayer>();
            this.AddComponent<ValuableTeleporter>();
            this.AddComponent<EnemiesTeleporter>();
            this.AddComponent<GrabWatcher>();
            /*this.AddComponent<NicknameAnimator>();*/

            Camera.onPreRender += Fullbright;
            Camera.onPreRender += ChangeFOV;
            Camera.onPreCull += WallHackRenderUtils.ApplyCameraOverrides;
            GrabWatcher.OnGrabbedObject += OnGrabbedObject;
        }

        private void OnDisable()
        {
            Camera.onPreRender -= Fullbright;
            Camera.onPreRender -= ChangeFOV;
            Camera.onPreCull -= WallHackRenderUtils.ApplyCameraOverrides;
            GrabWatcher.OnGrabbedObject -= OnGrabbedObject;
        }

        private void Fullbright(Camera cam)
        {
            if (Core.isFullbrightEnabled)
                FullbrightManager.ApplyFullbright();
        }

        private void ChangeFOV(Camera cam)
        {
            Core.ApplyRenderDistance(cam);

            if (Core.isHideMeActive || !Core.isCustomFovEnabled)
            {
                RestoreCameraFov(cam);
                return;
            }

            if (!defaultCameraFovs.ContainsKey(cam))
                defaultCameraFovs[cam] = cam.fieldOfView;

            if (Core.isCustomFovEnabled)
            {
                cam.fieldOfView = Core.fovValue;
                
                float extraFov = Mathf.Clamp(Core.fovValue + 40f, 1f, 179f);
                Matrix4x4 proj = Matrix4x4.Perspective(
                    extraFov,
                    cam.aspect,
                    cam.nearClipPlane,
                    cam.farClipPlane
                );

                cam.cullingMatrix = proj * cam.worldToCameraMatrix;
            }
            else
            {
                RestoreCameraFov(cam);
            }
        }

        public static void RestoreAllCameraFov()
        {
            int cameraCount = Camera.allCamerasCount;
            if (cameraCount <= 0)
            {
                RestoreCameraFov(Camera.main);
                return;
            }

            var cameras = new Camera[cameraCount];
            int count = Camera.GetAllCameras(cameras);
            for (int i = 0; i < count; i++)
                RestoreCameraFov(cameras[i]);
        }

        private static void RestoreCameraFov(Camera cam)
        {
            if (cam == null)
                return;

            if (defaultCameraFovs.TryGetValue(cam, out float defaultFov))
                cam.fieldOfView = defaultFov;

            cam.ResetCullingMatrix();
        }

        public static void ApplyConfiguredFlashlightSettings()
        {
            var flashlight = GetPlayerFlashlight();
            if (flashlight == null)
                return;

            flashlight.shadows = Core.isFlashlightShadowsEnabled ? LightShadows.Hard : LightShadows.None;
            flashlight.spotAngle = Core.flashlightSpotAngle;
            flashlight.range = Core.flashlightRange;
            flashlight.color = new Color(Core.FLC_R, Core.FLC_G, Core.FLC_B);
        }

        public static void RestoreDefaultFlashlightSettings()
        {
            var flashlight = GetPlayerFlashlight();
            if (flashlight == null)
                return;

            flashlight.shadows = LightShadows.Hard;
            flashlight.spotAngle = 60f;
            flashlight.range = 25f;
            flashlight.color = new Color(1f, 0.674f, 0.382f, 1f);
        }

        private static Light GetPlayerFlashlight()
        {
            try
            {
                if (PlayerController.instance == null ||
                    PlayerController.instance.playerAvatarScript == null ||
                    PlayerController.instance.playerAvatarScript.flashlightController == null)
                    return null;

                return PlayerController.instance.playerAvatarScript.flashlightController.spotlight;
            }
            catch
            {
                return null;
            }
        }
        
        public void TeleportValuables(int targetMode, bool oneAny, bool kinematic)
        {
            ValuableTeleporter.Teleport(targetMode, oneAny, kinematic);
        }
        
        public void TeleportEnemies(int targetMode, bool oneAny, bool kinematic)
        {
            EnemiesTeleporter.Teleport(targetMode, oneAny, kinematic);
        }

        public void OnGrabbedObject(Rigidbody rb)
        {
            if (rb == null)
                return;

            if (!HostOnlyGuard.IsHostOnlyActive())
                return;
            
            if (Core.valuablesTeleporter)
            {
                if (rb.isKinematic && Core.vtm_disableKinematicOnTouch)
                    rb.isKinematic = false;
                
                if (Core.vtm_teleportOnTouch)
                    ValuableTeleporter.TeleportSingle(rb, Core.vt_state, Core.vtm_kinematic);
            }
            
            if (Core.enemiesTeleporter && Core.em_teleportOnTouch)
                EnemiesTeleporter.TeleportSingle(rb, Core.em_state, Core.em_kinematic);
        }
    }
}
