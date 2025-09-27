using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO.Utils
{
    public class ValuableTeleporter : MonoBehaviour
    {
        public static void Teleport(int targetMode, bool oneAny, bool kinematic)
        {
            var valuables = new List<Component>();
            valuables.AddRange(ValuableDirector.instance.valuableList);
            valuables.AddRange(Object.FindObjectsOfType<SurplusValuable>(true));

            var epArray = Object.FindObjectsOfType<ExtractionPoint>(true);
            Vector3? targetPos = GetTargetPos(targetMode, epArray);
            if (targetPos == null) return;

            if (valuables.Count == 0)
            {
                FireLog("[VT] No valuables or surplus valuables found.");
                return;
            }

            if (oneAny)
            {
                var rnd = new System.Random();
                var item = valuables[rnd.Next(valuables.Count)];
                TeleportItem(item, targetPos.Value, kinematic);
            }
            else
            {
                TeleportAll(valuables, targetPos.Value, kinematic);
            }
        }

        public static void TeleportSingle(Rigidbody rb, int targetMode, bool kinematic)
        {
            if (rb == null) return;

            var epArray = Object.FindObjectsOfType<ExtractionPoint>(true);
            Vector3? targetPos = GetTargetPos(targetMode, epArray);
            if (targetPos == null) return;

            Component item = rb.GetComponentInParent<ValuableObject>();
            if (item == null)
                item = rb.GetComponentInParent<SurplusValuable>();

            if (item != null)
                TeleportItem(item, targetPos.Value, kinematic);
        }

        private static Vector3? GetTargetPos(int targetMode, ExtractionPoint[] epArray)
        {
            if (targetMode == 0)
                return PlayerController.instance.playerAvatar.transform.position;

            if (targetMode == 1)
            {
                foreach (var ep in epArray)
                {
                    if (!ep.isLocked)
                        return ep.transform.position;
                }
            }
            else if (targetMode == 2)
            {
                PhysGrabCart[] carts = Object.FindObjectsOfType<PhysGrabCart>();
                Vector3 playerPos = PlayerController.instance.playerAvatar.transform.position;

                PhysGrabCart nearest = null;
                float minDist = float.MaxValue;

                foreach (var cart in carts)
                {
                    if (cart == null) continue;
                    float dist = (playerPos - cart.gameObject.transform.position).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = cart;
                    }
                }

                if (nearest != null)
                    return nearest.gameObject.transform.position;
            }

            return null;
        }

        private static void TeleportAll(List<Component> list, Vector3 target, bool kinematic)
        {
            List<Rigidbody> affected = new List<Rigidbody>();

            foreach (var comp in list)
            {
                var rb = TeleportItem(comp, target, kinematic);
                if (rb != null && kinematic)
                    affected.Add(rb);
            }

            if (kinematic && !Core.vtm_permanentFreeze)
                _ = GradualUnfreeze(affected, Core.vtm_kinematicDisableInterval);
        }

        private static Rigidbody TeleportItem(Component item, Vector3 target, bool kinematic)
        {
            if (item == null) return null;

            var rb = item.GetComponentInChildren<Rigidbody>();
            var pv = item.GetComponent<PhotonView>();

            if (rb == null) return null;

            if (pv != null && !pv.IsMine)
                pv.RequestOwnership();

            if (kinematic)
                rb.isKinematic = true;

            Vector3 safeTarget = target + Vector3.up * Core.vtm_teleportYOffset;
            rb.position = safeTarget;
            rb.velocity = Vector3.zero;

            FireLog($"[VT] Teleported {item.name} to {safeTarget}");

            if (kinematic && !Core.vtm_permanentFreeze)
                _ = RestoreKinematic(rb, (int)Core.vtm_kinematicDisableInterval);

            return rb;
        }

        private static async Task RestoreKinematic(Rigidbody rb, int delayMs)
        {
            await Task.Delay(delayMs * 1000);
            if (rb != null)
                rb.isKinematic = false;
        }

        private static async Task GradualUnfreeze(List<Rigidbody> bodies, float interval)
        {
            foreach (var rb in bodies)
            {
                if (rb != null)
                    rb.isKinematic = false;

                await Task.Delay((int)(interval * 1000f));
            }
        }
    }
}
