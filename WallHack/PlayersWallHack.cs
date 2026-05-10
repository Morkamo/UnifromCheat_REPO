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
        private static readonly HashSet<PlayerAvatar> s_processedThisFrame = new();
        private static readonly List<PlayerAvatar> s_tmpToRemove = new();
        private static readonly FieldInfo PlayerDeathHeadField = typeof(PlayerAvatar).GetField(
            "playerDeathHead",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        private static readonly FieldInfo DeathHeadTriggeredField = typeof(PlayerDeathHead).GetField(
            "triggered",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        
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

            s_processedThisFrame.Clear();

            foreach (var player in others)
            {
                if (player == null || player.gameObject == null) continue;
                var pv = player.GetComponent<PhotonView>();
                if (pv == null) { RemoveBodyOutline(player); RemoveHeadOutline(player); RemoveLabel(player); continue; }

                if (!TryGetPlayerVisualBounds(player, out var visualBounds))
                {
                    RemoveBodyOutline(player);
                    RemoveLabel(player);
                    continue;
                }

                s_processedThisFrame.Add(player);

                PlayerDeathHead deathHead = GetPlayerDeathHead(player);
                bool deathHeadTriggered = IsTriggeredDeathHead(deathHead);

                if (isPlayerWallHackEnabled && isShowPlayerDeadHead && deathHeadTriggered)
                {
                    var headHost = deathHead.gameObject;
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
                
                if (isPlayerWallHackEnabled)
                {
                    if (trackedPlayers.ContainsKey(player))
                    {
                        if (trackedPlayers[player] != null)
                            trackedPlayers[player].gameObject.SetActive(true);
                        UpdateText(player, mainCamera, visualBounds);
                    }
                    else
                    {
                        CreateTextLabel(player, mainCamera, visualBounds);
                    }
                }
            }
            
            if (!isPlayerWallHackEnabled)
                ClearPlayerLabels();

            SweepMissing();
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

        private static void SweepMissing()
        {
            s_tmpToRemove.Clear();
            foreach (var kv in bodyOutlineRoots)
                if (kv.Key == null || !s_processedThisFrame.Contains(kv.Key) || IsDeadOrGone(kv.Key))
                    s_tmpToRemove.Add(kv.Key);
            foreach (var av in s_tmpToRemove) RemoveBodyOutline(av);

            s_tmpToRemove.Clear();
            foreach (var kv in headOutlineRoots)
                if (kv.Key == null || !s_processedThisFrame.Contains(kv.Key))
                    s_tmpToRemove.Add(kv.Key);
            foreach (var av in s_tmpToRemove) RemoveHeadOutline(av);

            s_tmpToRemove.Clear();
            foreach (var kv in trackedPlayers)
                if (kv.Key == null || !s_processedThisFrame.Contains(kv.Key))
                    s_tmpToRemove.Add(kv.Key);
            foreach (var av in s_tmpToRemove) RemoveLabel(av);

            s_processedThisFrame.Clear();
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

        private static bool TryGetPlayerVisualBounds(PlayerAvatar player, out Bounds bounds)
        {
            bounds = default;

            GameObject visualRoot = GetMeshRoot(player);
            if (visualRoot == null || !visualRoot.activeInHierarchy)
                return false;

            bool found = false;
            foreach (var renderer in visualRoot.GetComponentsInChildren<Renderer>(true))
            {
                if (!ShouldUsePlayerRenderer(renderer))
                    continue;

                if (!renderer.gameObject.activeInHierarchy || renderer.bounds.size.sqrMagnitude <= 0.0001f)
                    continue;

                if (!found)
                {
                    bounds = renderer.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return found && bounds.size.sqrMagnitude > 0.0001f;
        }

        private static bool ShouldUsePlayerRenderer(Renderer renderer)
        {
            if (renderer == null || !renderer.enabled)
                return false;

            if (renderer.GetComponent<TextMeshPro>() != null)
                return false;

            return true;
        }

        private static PlayerDeathHead GetPlayerDeathHead(PlayerAvatar player)
        {
            if (player == null) return null;

            var deathHead = PlayerDeathHeadField?.GetValue(player) as PlayerDeathHead;
            if (deathHead != null) return deathHead;

            if (player.playerCosmetics != null && player.playerCosmetics.deathHead != null)
                return player.playerCosmetics.deathHead;

            var cosmetics = player.GetComponentInChildren<PlayerCosmetics>(true);
            return cosmetics != null ? cosmetics.deathHead : null;
        }

        private static bool IsTriggeredDeathHead(PlayerDeathHead deathHead)
        {
            if (deathHead == null || deathHead.gameObject == null) return false;

            var triggeredValue = DeathHeadTriggeredField?.GetValue(deathHead);
            if (triggeredValue is bool triggered)
                return triggered;

            return deathHead.gameObject.activeInHierarchy;
        }
        
        private static GameObject CreateWHCopy(GameObject source, Color color)
        {
            if (source == null) return null;

            var root = new GameObject(source.name + "_WHRoot_Player");
            root.hideFlags = HideFlags.DontSave;

            foreach (var mr in source.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr == null) continue;
                var mf = mr.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;

                var go = new GameObject(mf.gameObject.name + "_WH");
                go.transform.SetParent(root.transform, false);
                var follower = go.AddComponent<OutlineFollower>(); follower.Init(mf.transform);

                var newMf = go.AddComponent<MeshFilter>(); newMf.sharedMesh = mf.sharedMesh;
                var newMr = go.AddComponent<MeshRenderer>();
                WallHackRenderUtils.AssignOverlayMaterial(newMr, BuildWHMaterial(color));
                WallHackRenderUtils.ConfigureOverlayRenderer(newMr);
            }

            foreach (var smr in source.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (smr == null || smr.sharedMesh == null) continue;

                var go = new GameObject(smr.gameObject.name + "_WH_Skinned");
                go.transform.SetParent(root.transform, false);
                var follower = go.AddComponent<OutlineFollower>(); follower.Init(smr.transform);

                var newSmr = go.AddComponent<SkinnedMeshRenderer>();
                newSmr.sharedMesh = smr.sharedMesh;
                newSmr.rootBone = smr.rootBone;
                newSmr.bones = smr.bones;
                newSmr.updateWhenOffscreen = true;
                WallHackRenderUtils.AssignOverlayMaterial(newSmr, BuildWHMaterial(color));
                WallHackRenderUtils.ConfigureOverlayRenderer(newSmr);
            }

            return root;
        }

        private static Material BuildWHMaterial(Color c)
        {
            return WallHackRenderUtils.CreateOverlayMaterial(c);
        }

        private static void UpdateWHCopyColor(GameObject root, Color c)
        {
            if (root == null) return;
            foreach (var r in root.GetComponentsInChildren<Renderer>())
                WallHackRenderUtils.SetRendererColor(r, c);
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
        
        private static void CreateTextLabel(PlayerAvatar player, Camera mainCamera, Bounds visualBounds)
        {
            GameObject textObject = new GameObject("Player_Label");
            textObject.hideFlags = HideFlags.DontSave;
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = "<color=red></color>";
            textMesh.fontSize = 3;
            
            if (pwh_syncTextColorWithGlow) textMesh.color = new Color(PC_R, PC_G, PC_B, PC_A);
            else textMesh.color = new Color(PTC_R, PTC_G, PTC_B, PTC_A);
            
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.enableWordWrapping = false;
            textMesh.isOverlay = true;
            WallHackRenderUtils.ConfigureOverlayText(textMesh);

            if (mainCamera != null)
            {
                UpdateTextTransform(textMesh, visualBounds, mainCamera);
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

        private static void UpdateText(PlayerAvatar player, Camera mainCamera, Bounds visualBounds)
        {
            if (trackedPlayers.TryGetValue(player, out var textMesh) && mainCamera != null)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, visualBounds.center);
                float dynamicSize = Mathf.Clamp((0.2f + distance) - 1, 0.2f, playerTextSize);

                textMesh.text = GetPlayerInfo(player);
                textMesh.fontSize = dynamicSize;
                
                Color textColor = pwh_syncTextColorWithGlow
                    ? new Color(PC_R, PC_G, PC_B, PC_A)
                    : new Color(PTC_R, PTC_G, PTC_B, PTC_A);
                WallHackRenderUtils.SetTextColor(textMesh, textColor);

                UpdateTextTransform(textMesh, visualBounds, mainCamera);
            }
        }

        private static void UpdateTextTransform(TextMeshPro textMesh, Bounds visualBounds, Camera mainCamera)
        {
            Vector3 center = visualBounds.center;
            float yOffset = Mathf.Max(visualBounds.size.y, 1f);

            textMesh.transform.position = center + Vector3.up * (yOffset + 0.5f);
            WallHackRenderUtils.FaceTextToCamera(textMesh, mainCamera);
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

            foreach (var kv in headOutlineRoots)
                if (kv.Value != null) Object.Destroy(kv.Value);
            headOutlineRoots.Clear();
        }

        private void Update()
        {
            if (Core.WH_BlockUpdates) 
                return;
            RenderPlayers();
        }
    }
}
