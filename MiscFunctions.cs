using HarmonyLib;
using UnifromCheat_REPO.Utils;
using Unity.VisualScripting;
using UnityEngine;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO
{
    public class MiscFunctions : MonoBehaviour
    {
        public static MiscFunctions Instance;

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

            Camera.onPreRender += Fullbright;
            Camera.onPreRender += ChangeFOV;
            GrabWatcher.OnGrabbedObject += OnGrabbedObject;
        }

        private void OnDisable()
        {
            Camera.onPreRender -= Fullbright;
            Camera.onPreRender -= ChangeFOV;
            GrabWatcher.OnGrabbedObject -= OnGrabbedObject;
        }

        private void Fullbright(Camera cam)
        {
            if (Core.isFullbrightEnabled)
                FullbrightManager.ApplyFullbright();
        }

        private void ChangeFOV(Camera cam)
        {
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