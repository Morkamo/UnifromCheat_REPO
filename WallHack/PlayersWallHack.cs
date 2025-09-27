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
        private static Dictionary<PlayerAvatar, TextMeshPro> trackedPlayers = new Dictionary<PlayerAvatar, TextMeshPro>();
        private static Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

        public static Material GlowMaterial;
        private static readonly int ZTest = Shader.PropertyToID("_ZTest");

        public static void RenderPlayers()
        {
            if (GameDirector.instance.PlayerList != null)
            {
                var playerList = GameDirector.instance.PlayerList;
                if (playerList == null || playerList.Count == 0) return;

                var localPlayer = playerList
                    .FirstOrDefault(pl =>
                        pl != null &&
                        pl.photonView != null &&
                        PlayerController.instance != null &&
                        PlayerController.instance.playerAvatar != null &&
                        PlayerController.instance.playerAvatar.GetPhotonView() != null &&
                        pl.photonView.OwnerActorNr == PlayerController.instance.playerAvatar.GetPhotonView().OwnerActorNr);

                var playerParrents = playerList.Where(pl => pl != null && pl != localPlayer).ToList();
                if (playerParrents.Count == 0) return;

                var mainCamera = Camera.main;
                if (mainCamera == null) return;

                if (GlowMaterial == null)
                {
                    GlowMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                    GlowMaterial.color = new Color(PC_R, PC_G, PC_B, PC_A);
                    GlowMaterial.SetInt(ZTest, 0);
                }

                foreach (var playerParent in playerParrents)
                {
                    if (playerParent == null) continue;

                    var player = playerParent.GetComponentInChildren<PlayerAvatar>();
                    if (player == null || player.gameObject == null || player.photonView == null)
                        continue;

                    // Голова
                    if (player.playerDeathHead != null && player.playerDeathHead.headRenderer != null)
                    {
                        Renderer headRend = player.playerDeathHead.headRenderer;
                        if (headRend != null)
                        {
                            originalMaterials.TryAdd(headRend, headRend.materials);

                            if (isPlayerWallHackEnabled && isShowPlayerDeadHead)
                            {
                                Material[] mats = new Material[headRend.materials.Length];
                                for (int i = 0; i < mats.Length; i++)
                                    mats[i] = GlowMaterial;

                                headRend.materials = mats;
                                headRend.material.color = new Color(PCDH_R, PCDH_G, PCDH_B, PCDH_A);
                            }
                            else if (originalMaterials.TryGetValue(headRend, out var matsOriginal))
                            {
                                headRend.materials = matsOriginal;
                                ClearPlayerLabels();
                            }
                        }
                    }

                    // Тело
                    if (playerParent.playerAvatarVisuals != null && playerParent.playerAvatarVisuals.meshParent != null)
                    {
                        var renderers = playerParent.playerAvatarVisuals.meshParent.GetComponentsInChildren<Renderer>();
                        foreach (var r in renderers)
                        {
                            if (r == null || r.GetComponent<TextMeshPro>() != null)
                                continue;

                            originalMaterials.TryAdd(r, r.materials);

                            if (isPlayerWallHackEnabled && isShowPlayerGlow)
                            {
                                Material[] mats = new Material[r.materials.Length];
                                for (int i = 0; i < mats.Length; i++)
                                    mats[i] = GlowMaterial;

                                r.materials = mats;
                                r.material.color = new Color(PC_R, PC_G, PC_B, PC_A);
                            }
                            else if (originalMaterials.TryGetValue(r, out var material))
                            {
                                r.materials = material;
                            }

                            if (!isPlayerWallHackEnabled)
                                ClearPlayerLabels();
                        }
                    }

                    if (trackedPlayers.ContainsKey(player))
                        UpdateText(player, mainCamera);
                    else
                        CreateTextLabel(player, mainCamera);
                }
            }
        }


        private static void CreateTextLabel(PlayerAvatar player, Camera mainCamera)
        {
            GameObject textObject = new GameObject("Player_Label");
            TextMeshPro textMesh = textObject.AddComponent<TextMeshPro>();

            textMesh.text = "<color=red></color>";
            textMesh.fontSize = 3;
            textMesh.color = new Color(PC_R, PC_G, PC_B, PC_A);
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

            if (showPlayerName)
                info += $"\n{name}";

            if (showPlayerHealth)
            {
                info += $"\n<b>{GetPlayerHealth(player)}HP</b>";
            }

            return info;
        }

        private static string GetPlayerHealth(PlayerAvatar player)
        {
            if (player == null)
                return "NULL PLAYER DATA";

            var healthComp = player.GetComponent<PlayerHealth>();
            if (healthComp == null)
                return "NO PLAYER HEALTH";

            var type = healthComp.GetType();
            var field = type.GetField("health", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
                return field.GetValue(healthComp).ToString();

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
                textMesh.color = new Color(PC_R, PC_G, PC_B, PC_A);

                textMesh.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            }
        }

        private static void ClearPlayerLabels()
        {
            foreach (var player in trackedPlayers.Values)
            {
                if (player != null)
                    Destroy(player.gameObject);
            }

            trackedPlayers.Clear();
        }

        private void Update()
        {
            RenderPlayers();
        }
    }
}
