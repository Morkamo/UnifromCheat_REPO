using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using TMPro;
using UnityEngine;
using static UnifromCheat_REPO.Core;

namespace UnifromCheat_REPO.WallHack
{
    public class PlayersWallHack : MonoBehaviour
    {
        private static readonly Dictionary<PlayerAvatar, TextMeshPro> trackedPlayers = new();
        private static readonly Dictionary<PlayerAvatar, GameObject> bodyOutlineRoots = new();
        private static readonly Dictionary<PlayerAvatar, GameObject> headOutlineRoots = new();
        
        private static readonly int ZTest   = Shader.PropertyToID("_ZTest");
        private static readonly int Cull    = Shader.PropertyToID("_Cull");
        private static readonly int ZWrite  = Shader.PropertyToID("_ZWrite");
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        public static void RenderPlayers()
        {
            var avatars = GameDirector.instance.PlayerList;
            if (avatars == null || avatars.Count == 0) { ClearAllForDisabled(); return; }

            GameObject plAvatar = PlayerController.instance.playerAvatar;
            PhotonView localPV;
            
            if (plAvatar != null)
                localPV = plAvatar.GetPhotonView();
            else return;
            
            var localAvatar = avatars.FirstOrDefault(av =>
            {
                var pv = av ? av.GetComponent<PhotonView>() : null;
                return pv != null && localPV != null && pv.OwnerActorNr == localPV.OwnerActorNr;
            });

            var others = avatars.Where(av => av != null && av != localAvatar).ToList();
            if (others.Count == 0) { ClearAllForDisabled(); return; }

            var mainCamera = Camera.main;
            if (mainCamera == null) return;

            foreach (var player in others)
            {
                if (player == null || player.gameObject == null) continue;
                var pv = player.GetComponent<PhotonView>();
                if (pv == null) { RemoveBodyOutline(player); RemoveHeadOutline(player); RemoveLabel(player); continue; }
                
                if (isPlayerWallHackEnabled && isShowPlayerDeadHead && player.playerDeathHead != null)
                {
                    var headHost = player.playerDeathHead.gameObject;
                    var headColor = new Color(PCDH_R, PCDH_G, PCDH_B, PCDH_A);

                    if (!headOutlineRoots.TryGetValue(player, out var hroot) || hroot == null)
                    {
                        hroot = CreateWHCopy(headHost, headColor);
                        headOutlineRoots[player] = hroot;
                    }
                    else
                    {
                        hroot.SetActive(true);
                        UpdateWHCopyColor(hroot, headColor);
                    }
                }
                else
                {
                    RemoveHeadOutline(player);
                }
                
                if (isPlayerWallHackEnabled && isShowPlayerGlow && !IsDeadOrGone(player))
                {
                    var bodyColor = new Color(PC_R, PC_G, PC_B, PC_A);

                    if (!bodyOutlineRoots.TryGetValue(player, out var root) || root == null)
                    {
                        var meshRoot = GetMeshRoot(player);
                        var newRoot = CreateWHCopy(meshRoot, bodyColor);
                        bodyOutlineRoots[player] = newRoot;
                    }
                    else
                    {
                        root.SetActive(true);
                        UpdateWHCopyColor(root, bodyColor);
                    }
                }
                else
                {
                    RemoveBodyOutline(player);
                }
                
                if (trackedPlayers.ContainsKey(player))
                    UpdateText(player, mainCamera);
                else
                    CreateTextLabel(player, mainCamera);
            }
            
            SweepMissing(others);
            if (!isPlayerWallHackEnabled) ClearPlayerLabels();
        }

        private static bool IsDeadOrGone(PlayerAvatar av)
        {
            if (av == null || av.gameObject == null) return true;
            if (!av.gameObject.activeInHierarchy) return true;

            var hp = av.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                var field = typeof(PlayerHealth).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    var v = field.GetValue(hp);
                    if (v is float f) return f <= 0f;
                    if (v is int i) return i <= 0;
                }
            }
            return false;
        }

        private static void SweepMissing(List<PlayerAvatar> currentAvatars)
        {
            var aliveOrDead = new HashSet<PlayerAvatar>(currentAvatars.Where(av => av != null));
            
            var deadBodies = bodyOutlineRoots.Keys
                .Where(av => av == null || !aliveOrDead.Contains(av) || IsDeadOrGone(av))
                .ToList();
            foreach (var av in deadBodies) RemoveBodyOutline(av);
            
            var missingHeads = headOutlineRoots.Keys
                .Where(av => av == null || !aliveOrDead.Contains(av))
                .ToList();
            foreach (var av in missingHeads) RemoveHeadOutline(av);
            
            var deadLabels = trackedPlayers.Keys
                .Where(av => av == null || !aliveOrDead.Contains(av))
                .ToList();
            foreach (var av in deadLabels) RemoveLabel(av);
        }

        private static void RemoveBodyOutline(PlayerAvatar av)
        {
            if (av == null) return;
            if (bodyOutlineRoots.TryGetValue(av, out var root) && root != null) Object.Destroy(root);
            bodyOutlineRoots.Remove(av);
        }

        private static void RemoveHeadOutline(PlayerAvatar av)
        {
            if (av == null) return;
            if (headOutlineRoots.TryGetValue(av, out var root) && root != null) Object.Destroy(root);
            headOutlineRoots.Remove(av);
        }

        private static void RemoveLabel(PlayerAvatar av)
        {
            if (av == null) return;
            if (trackedPlayers.TryGetValue(av, out var tmp) && tmp != null) Object.Destroy(tmp.gameObject);
            trackedPlayers.Remove(av);
        }

        private static void ClearAllForDisabled()
        {
            foreach (var kv in bodyOutlineRoots) if (kv.Value != null) Object.Destroy(kv.Value);
            bodyOutlineRoots.Clear();

            foreach (var kv in headOutlineRoots) if (kv.Value != null) Object.Destroy(kv.Value);
            headOutlineRoots.Clear();

            ClearPlayerLabels();
        }

        private static GameObject GetMeshRoot(PlayerAvatar av)
        {
            var visualsField = av.GetType().GetField("playerAvatarVisuals", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (visualsField != null)
            {
                var visuals = visualsField.GetValue(av);
                if (visuals != null)
                {
                    var meshParentField = visuals.GetType().GetField("meshParent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (meshParentField != null)
                    {
                        var go = meshParentField.GetValue(visuals) as GameObject;
                        if (go != null) return go;
                    }
                }
            }
            
            var smr = av.GetComponentsInChildren<SkinnedMeshRenderer>(true).FirstOrDefault();
            if (smr != null) return smr.gameObject;
            var mr = av.GetComponentsInChildren<MeshRenderer>(true).FirstOrDefault();
            if (mr != null) return mr.gameObject;
            
            return av.gameObject;
        }
        
        private static GameObject CreateWHCopy(GameObject source, Color color)
        {
            if (source == null) return null;

            var root = new GameObject(source.name + "_WHRoot_Player");
            root.hideFlags = HideFlags.DontSave;

            foreach (var mr in source.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr == null || !mr.enabled) continue;
                var mf = mr.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;

                var go = new GameObject(mf.gameObject.name + "_WH");
                go.transform.SetParent(root.transform, false);
                var follower = go.AddComponent<OutlineFollower>(); follower.Init(mf.transform);

                var newMf = go.AddComponent<MeshFilter>(); newMf.sharedMesh = mf.sharedMesh;
                var newMr = go.AddComponent<MeshRenderer>(); newMr.material = BuildWHMaterial(color);
            }

            foreach (var smr in source.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (smr == null || !smr.enabled || smr.sharedMesh == null) continue;

                var go = new GameObject(smr.gameObject.name + "_WH_Skinned");
                go.transform.SetParent(root.transform, false);
                var follower = go.AddComponent<OutlineFollower>(); follower.Init(smr.transform);

                var newSmr = go.AddComponent<SkinnedMeshRenderer>();
                newSmr.sharedMesh = smr.sharedMesh;
                newSmr.rootBone = smr.rootBone;
                newSmr.bones = smr.bones;
                newSmr.updateWhenOffscreen = true;
                newSmr.material = BuildWHMaterial(color);
            }

            return root;
        }

        private static Material BuildWHMaterial(Color c)
        {
            var mat = new Material(Shader.Find("Hidden/Internal-Colored")) { hideFlags = HideFlags.HideAndDontSave };
            mat.SetInt(ZTest, (int)UnityEngine.Rendering.CompareFunction.Always);
            mat.SetInt(Cull, (int)UnityEngine.Rendering.CullMode.Front);
            mat.SetInt(ZWrite, 0);
            mat.SetColor(ColorID, c);
            return mat;
        }

        private static void UpdateWHCopyColor(GameObject root, Color c)
        {
            if (root == null) return;
            foreach (var r in root.GetComponentsInChildren<Renderer>())
                if (r != null && r.material != null) r.material.color = c;
        }

        private class OutlineFollower : MonoBehaviour
        {
            private Transform target;
            public void Init(Transform t) => target = t;

            private void LateUpdate()
            {
                if (target == null) { Destroy(gameObject); return; }
                var trs = transform;
                trs.position = target.position;
                trs.rotation = target.rotation;
                trs.localScale = target.lossyScale + new Vector3(0.01f, 0.01f, 0.01f);
            }
        }
        
        private static void CreateTextLabel(PlayerAvatar player, Camera mainCamera)
        {
            GameObject textObject = new GameObject("Player_Label");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = "<color=red></color>";
            textMesh.fontSize = 3;
            
            if (pwh_syncTextColorWithGlow) textMesh.color = new Color(PC_R, PC_G, PC_B, PC_A);
            else textMesh.color = new Color(PTC_R, PTC_G, PTC_B, PTC_A);
            
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.enableWordWrapping = false;
            textMesh.isOverlay = true;

            textObject.transform.SetParent(player.transform, false);

            if (mainCamera != null)
            {
                textObject.transform.LookAt(mainCamera.transform);
                textObject.transform.Rotate(0, 180, 0);
            }

            trackedPlayers[player] = textMesh;
        }

        private static string GetPlayerInfo(PlayerAvatar player)
        {
            string name = player.photonView.Owner.NickName;

            string info = "";
            if (showPlayerName) info += $"\n{name}";
            if (showPlayerHealth) info += $"\n<b>{GetPlayerHealth(player)}HP</b>";
            return info;
        }

        private static string GetPlayerHealth(PlayerAvatar player)
        {
            if (player == null) return "[NULL PLAYER DATA]";

            var healthComp = player.GetComponent<PlayerHealth>();
            if (healthComp == null) return "[NO PLAYER HEALTH]";

            var type = healthComp.GetType();
            var field = type.GetField("health", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) return field.GetValue(healthComp).ToString();

            return "NULL DATA";
        }

        private static void UpdateText(PlayerAvatar player, Camera mainCamera)
        {
            if (trackedPlayers.TryGetValue(player, out var textMesh) && mainCamera != null)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, player.transform.position);
                float dynamicSize = Mathf.Clamp((0.2f + distance) - 1, 0.2f, playerTextSize);

                textMesh.text = GetPlayerInfo(player);
                textMesh.fontSize = dynamicSize;
                
                if (pwh_syncTextColorWithGlow) textMesh.color = new Color(PC_R, PC_G, PC_B, PC_A);
                else textMesh.color = new Color(PTC_R, PTC_G, PTC_B, PTC_A);

                textMesh.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }

        private static void ClearPlayerLabels()
        {
            foreach (var player in trackedPlayers.Values)
                if (player != null) Object.Destroy(player.gameObject);
            trackedPlayers.Clear();
        }
        
        public static void ClearAll()
        {
            foreach (var kv in trackedPlayers)
                if (kv.Value != null) Object.Destroy(kv.Value.gameObject);
            trackedPlayers.Clear();


            foreach (var kv in bodyOutlineRoots)
                if (kv.Value != null) Object.Destroy(kv.Value);
            bodyOutlineRoots.Clear();
        }

        private void Update()
        {
            if (Core.WH_BlockUpdates) 
                return;
            RenderPlayers();
        }
    }
}
