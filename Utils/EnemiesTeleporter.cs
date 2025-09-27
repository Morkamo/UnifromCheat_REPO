using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using static UnifromCheat_REPO.Utils.FireboxConsole;

namespace UnifromCheat_REPO.Utils
{
    public class EnemiesTeleporter : MonoBehaviour
    {
        public static void Teleport(int targetMode, bool oneAny, bool kinematic)
        {
            var enemies = EnemyDirector.instance.enemiesSpawned;
            if (enemies == null || enemies.Count == 0)
            {
                FireLog("[ET] No enemies found.");
                return;
            }

            var epArray = Object.FindObjectsOfType<ExtractionPoint>(true);
            Vector3? targetPos = GetTargetPos(targetMode, epArray);
            if (targetPos == null) return;

            if (oneAny)
            {
                var rnd = new System.Random();
                var enemy = enemies[rnd.Next(enemies.Count)];
                TeleportItem(enemy, targetPos.Value, kinematic);
            }
            else
            {
                TeleportAll(enemies, targetPos.Value, kinematic);
            }
        }

        public static void TeleportSingle(Rigidbody rb, int targetMode, bool kinematic)
        {
            if (rb == null) return;

            var epArray = Object.FindObjectsOfType<ExtractionPoint>(true);
            Vector3? targetPos = GetTargetPos(targetMode, epArray);
            if (targetPos == null) return;

            var enemy = rb.GetComponentInParent<EnemyParent>();
            if (enemy != null)
                TeleportItem(enemy, targetPos.Value, kinematic);
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
                return new Vector3(-1000, -1000, -1000);
            }

            return null;
        }

        private static void TeleportAll(List<EnemyParent> list, Vector3 target, bool kinematic)
        {
            List<Rigidbody> affected = new List<Rigidbody>();

            foreach (var enemy in list)
            {
                var rb = TeleportItem(enemy, target, kinematic);
                if (rb != null && kinematic)
                    affected.Add(rb);
            }

            if (kinematic && !Core.em_permanentFreeze)
                _ = GradualUnfreeze(affected, Core.vtm_kinematicDisableInterval);
        }

        private static Rigidbody TeleportItem(EnemyParent enemy, Vector3 target, bool kinematic)
        {
            if (enemy == null) return null;

            var rb = enemy.GetComponentInChildren<Rigidbody>();
            var pv = enemy.GetComponent<PhotonView>();

            if (rb == null) return null;

            if (pv != null && !pv.IsMine)
                pv.RequestOwnership();

            if (kinematic)
                rb.isKinematic = true;

            Vector3 safeTarget = target + Vector3.up * Core.em_teleportYOffset;
            rb.position = safeTarget;
            rb.velocity = Vector3.zero;

            FireLog($"[ET] Teleported {enemy.name} to {safeTarget}");

            if (kinematic && !Core.em_permanentFreeze)
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
