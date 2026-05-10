using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnifromCheat_REPO.Utils
{
    internal static class GameController
    {
        public sealed class PlayerEntry
        {
            public PlayerAvatar Avatar;
            public string Name;
            public int Health;
            public int MaxHealth;
            public bool IsDead;
        }

        public sealed class MapEntry
        {
            public Level Level;
            public string DisplayName;
        }

        private static float nextEnemyDespawnTime;
        private static float nextMapChangeTime;
        private const float AutoReviveDeathHeadSyncDelay = 0.35f;
        private const int AutoRevivePostSpawnFrames = 3;
        private static readonly HashSet<int> AutoReviveQueuedPlayers = new HashSet<int>();
        private static List<PlayerEntry> cachedPlayers = new List<PlayerEntry>();
        private static float nextPlayersRefreshTime;
        private static List<MapEntry> cachedMaps = new List<MapEntry>();
        private static RunManager cachedMapsManager;
        private static int cachedMapsSignature;

        public static bool IsDisableSpawnAIActive()
        {
            return Core.gcDisableSpawnAI && HostOnlyGuard.IsHostOnlyActive();
        }

        public static bool IsDisableAutoExtractActive()
        {
            return Core.gcDisableAutoExtract && HostOnlyGuard.IsHostOnlyActive();
        }

        public static bool IsSharedUpgradesActive()
        {
            return Core.gcSharedUpgrades && HostOnlyGuard.IsHostOnlyActive();
        }

        public static bool IsAutoReviveActive()
        {
            return Core.gcAutoRevive && HostOnlyGuard.IsHostOnlyActive();
        }

        public static void UpdateGameplay()
        {
            if (!IsDisableSpawnAIActive() || Time.time < nextEnemyDespawnTime)
                return;

            nextEnemyDespawnTime = Time.time + 0.75f;
            DespawnAllEnemies();
        }

        public static List<PlayerEntry> GetPlayers()
        {
            if (Time.unscaledTime < nextPlayersRefreshTime)
                return cachedPlayers;

            nextPlayersRefreshTime = Time.unscaledTime + 0.15f;
            List<PlayerEntry> result = new List<PlayerEntry>();
            if (!HostOnlyGuard.IsPlayableSession())
            {
                cachedPlayers = result;
                return result;
            }

            IEnumerable<PlayerAvatar> players = GameDirector.instance?.PlayerList;

            if (players == null)
            {
                cachedPlayers = result;
                return result;
            }

            foreach (PlayerAvatar avatar in players)
            {
                if (avatar == null)
                    continue;

                PlayerHealth health = avatar.playerHealth;
                result.Add(new PlayerEntry
                {
                    Avatar = avatar,
                    Name = TruncateName(GetPlayerName(avatar)),
                    Health = GetIntField(health, "health"),
                    MaxHealth = GetIntField(health, "maxHealth"),
                    IsDead = GetBoolField(avatar, "deadSet") || GetBoolField(avatar, "isDisabled")
                });
            }

            cachedPlayers = result;
            return result;
        }

        public static List<MapEntry> GetMaps()
        {
            RunManager manager = RunManager.instance;
            if (manager == null)
                return new List<MapEntry>();

            int signature = GetMapsSignature(manager);
            if (cachedMapsManager == manager && cachedMapsSignature == signature)
                return cachedMaps;

            Dictionary<string, MapEntry> maps = new Dictionary<string, MapEntry>();
            AddMaps(maps, manager.levels);
            AddMaps(maps, manager.levelShop);
            AddMaps(maps, manager.levelArena);

            cachedMaps = maps.Values.OrderBy(map => map.DisplayName).ToList();
            cachedMapsManager = manager;
            cachedMapsSignature = signature;
            return cachedMaps;
        }

        public static bool Heal(PlayerAvatar avatar, out string message)
        {
            message = string.Empty;
            if (!CanRunPlayerAction(avatar, "Game Controller", out message))
                return false;

            PlayerHealth health = avatar.playerHealth;
            if (health == null)
            {
                message = LocalizedMessages.Get("playerActionFailed");
                return false;
            }

            int amount = Mathf.Max(0, GetIntField(health, "maxHealth") - GetIntField(health, "health"));
            if (amount > 0)
                health.HealOther(amount, true);

            return true;
        }

        public static bool Revive(PlayerAvatar avatar, out string message)
        {
            message = string.Empty;
            if (!CanRunPlayerAction(avatar, "Game Controller", out message))
                return false;

            if (!GetBoolField(avatar, "deadSet") && !GetBoolField(avatar, "isDisabled"))
                return true;

            avatar.Revive(true);
            return true;
        }

        public static bool Kill(PlayerAvatar avatar, out string message)
        {
            message = string.Empty;
            if (!CanRunPlayerAction(avatar, "Game Controller", out message))
                return false;

            PlayerHealth health = avatar.playerHealth;
            if (health == null)
            {
                message = LocalizedMessages.Get("playerActionFailed");
                return false;
            }

            health.HurtOther(9999, Vector3.zero, false, -1, false);
            return true;
        }

        public static bool GoTo(PlayerAvatar target, out string message)
        {
            message = string.Empty;
            PlayerAvatar local = PlayerController.instance?.playerAvatarScript;
            if (!CanRunPlayerAction(target, "Game Controller", out message) || local == null)
                return false;

            Teleport(local, target.transform.position + target.transform.forward * -1.5f + Vector3.up * 0.2f, target.transform.rotation);
            return true;
        }

        public static bool Bring(PlayerAvatar target, out string message)
        {
            message = string.Empty;
            PlayerAvatar local = PlayerController.instance?.playerAvatarScript;
            if (!CanRunPlayerAction(target, "Game Controller", out message) || local == null)
                return false;

            Teleport(target, local.transform.position + local.transform.forward * 1.5f + Vector3.up * 0.2f, local.transform.rotation);
            return true;
        }

        public static bool ChangeMap(Level level, out string message)
        {
            message = string.Empty;
            if (level == null)
            {
                message = LocalizedMessages.Get("mapChangeFailed");
                return false;
            }

            if (!HostOnlyGuard.CanUseHostOnly(true, "Game Controller"))
                return false;

            RunManager manager = RunManager.instance;
            if (manager == null || GetBoolField(manager, "restarting") || GetBoolField(manager, "restartingDone") || GetBoolField(manager, "waitToChangeScene"))
            {
                message = LocalizedMessages.Get("mapChangeBlocked");
                return false;
            }

            if (Time.unscaledTime < nextMapChangeTime)
            {
                message = LocalizedMessages.Get("mapChangeBlocked");
                return false;
            }

            if (GameDirector.instance != null && GameDirector.instance.currentState != GameDirector.gameState.Main)
            {
                message = LocalizedMessages.Get("mapChangeBlocked");
                return false;
            }

            nextMapChangeTime = Time.unscaledTime + 5f;
            manager.levelCurrent = level;
            SyncLevelToClients(manager, level);
            SetSaveLevel(manager, level);
            manager.RestartScene();
            SemiFunc.OnSceneSwitch(false, false);
            message = LocalizedMessages.Format("mapChangeStarted", GetLevelDisplayName(level));
            return true;
        }

        public static void DespawnAllEnemies()
        {
            if (!HostOnlyGuard.IsHostOnlyActive())
                return;

            EnemyParent[] enemies = Object.FindObjectsOfType<EnemyParent>(true);
            foreach (EnemyParent enemy in enemies)
            {
                if (enemy == null)
                    continue;

                try
                {
                    enemy.Despawn();
                }
                catch
                {
                    // Some pooled enemies can be half-initialized during scene load; skip them.
                }
            }
        }

        public static void ApplySharedUpgrade(Component upgradeComponent, string upgradeTypeName)
        {
            if (!IsSharedUpgradesActive() || upgradeComponent == null || PunManager.instance == null)
                return;

            ItemToggle itemToggle = upgradeComponent.GetComponent<ItemToggle>();
            PlayerAvatar source = GetPlayerFromItemToggle(itemToggle);
            string sourceSteamId = source != null ? SemiFunc.PlayerGetSteamID(source) : null;

            foreach (PlayerEntry entry in GetPlayers())
            {
                if (entry.Avatar == null)
                    continue;

                string steamId = SemiFunc.PlayerGetSteamID(entry.Avatar);
                if (string.IsNullOrWhiteSpace(steamId) || steamId == sourceSteamId)
                    continue;

                ApplyUpgradeToSteamId(upgradeTypeName, steamId);
            }
        }

        public static void QueueAutoRevive(PlayerAvatar avatar)
        {
            if (!IsAutoReviveActive() || avatar == null || Core.Instance == null)
                return;

            PhotonView photonView = avatar.photonView;
            int key = photonView != null ? photonView.ViewID : avatar.GetInstanceID();
            if (!AutoReviveQueuedPlayers.Add(key))
                return;

            Core.Instance.StartCoroutine(AutoReviveAfterDelay(avatar, key));
        }

        private static bool CanRunPlayerAction(PlayerAvatar avatar, string functionName, out string message)
        {
            message = string.Empty;
            if (avatar == null)
            {
                message = LocalizedMessages.Get("playerNotFound");
                return false;
            }

            return HostOnlyGuard.CanUseHostOnly(true, functionName);
        }

        private static IEnumerator AutoReviveAfterDelay(PlayerAvatar avatar, int key)
        {
            yield return new WaitForSeconds(1f);

            try
            {
                if (!IsAutoReviveActive() || !IsAvatarDead(avatar))
                    yield break;

                GetTruckRevivePose(avatar, out Vector3 revivePosition, out Quaternion reviveRotation);

                float syncUntil = Time.unscaledTime + AutoReviveDeathHeadSyncDelay;
                while (Time.unscaledTime < syncUntil)
                {
                    if (!IsAutoReviveActive() || !IsAvatarDead(avatar))
                        yield break;

                    TeleportDeathHead(avatar, revivePosition, reviveRotation);
                    yield return null;
                }

                TeleportDeathHead(avatar, revivePosition, reviveRotation);
                avatar.Revive(true);

                for (int i = 0; i < AutoRevivePostSpawnFrames; i++)
                {
                    yield return null;
                    if (avatar == null)
                        yield break;

                    Teleport(avatar, revivePosition, reviveRotation);
                }
            }
            finally
            {
                AutoReviveQueuedPlayers.Remove(key);
            }
        }

        private static bool IsAvatarDead(PlayerAvatar avatar)
        {
            return avatar != null && (GetBoolField(avatar, "deadSet") || GetBoolField(avatar, "isDisabled"));
        }

        private static void GetTruckRevivePose(PlayerAvatar avatar, out Vector3 position, out Quaternion rotation)
        {
            if (TruckSafetySpawnPoint.instance != null)
            {
                position = TruckSafetySpawnPoint.instance.transform.position;
                rotation = TruckSafetySpawnPoint.instance.transform.rotation;
                return;
            }

            position = avatar != null ? avatar.transform.position : Vector3.zero;
            rotation = avatar != null ? avatar.transform.rotation : Quaternion.identity;
        }

        private static void TeleportDeathHead(PlayerAvatar avatar, Vector3 position, Quaternion rotation)
        {
            PlayerDeathHead deathHead = GetFieldValue<PlayerDeathHead>(avatar, "playerDeathHead");
            PhysGrabObject physGrabObject = deathHead != null ? GetFieldValue<PhysGrabObject>(deathHead, "physGrabObject") : null;
            SetFieldValue(deathHead, "triggeredPosition", position + Vector3.up * 0.25f);
            SetFieldValue(deathHead, "triggeredRotation", rotation);
            SetFieldValue(deathHead, "triggeredTimer", 0f);
            physGrabObject?.Teleport(position + Vector3.up * 0.25f, rotation);
        }

        private static void Teleport(PlayerAvatar avatar, Vector3 position, Quaternion rotation)
        {
            if (avatar == null)
                return;

            Vector3 forward = rotation * Vector3.forward;
            Vector3 flatForward = new Vector3(forward.x, 0f, forward.z);
            if (flatForward.sqrMagnitude > 0.001f)
                rotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);

            avatar.Spawn(position, rotation);
        }

        private static void AddMaps(Dictionary<string, MapEntry> maps, IEnumerable<Level> levels)
        {
            if (levels == null)
                return;

            RunManager manager = RunManager.instance;
            foreach (Level level in levels)
            {
                if (level == null || IsExcludedLevel(manager, level))
                    continue;

                string key = level.name;
                if (string.IsNullOrWhiteSpace(key) || maps.ContainsKey(key))
                    continue;

                maps[key] = new MapEntry
                {
                    Level = level,
                    DisplayName = GetLevelDisplayName(level)
                };
            }
        }

        private static int GetMapsSignature(RunManager manager)
        {
            unchecked
            {
                int signature = 17;
                signature = signature * 31 + GetLevelsCount(manager.levels);
                signature = signature * 31 + GetLevelsCount(manager.levelShop);
                signature = signature * 31 + GetLevelsCount(manager.levelArena);
                signature = signature * 31 + (manager.levelLobby != null ? manager.levelLobby.GetInstanceID() : 0);
                signature = signature * 31 + (manager.levelMainMenu != null ? manager.levelMainMenu.GetInstanceID() : 0);
                return signature;
            }
        }

        private static int GetLevelsCount(IEnumerable<Level> levels)
        {
            if (levels == null)
                return 0;

            int count = 0;
            foreach (Level level in levels)
            {
                if (level != null)
                    count++;
            }

            return count;
        }

        private static bool IsExcludedLevel(RunManager manager, Level level)
        {
            if (manager == null || level == null)
                return true;

            return level == manager.levelLobby ||
                   level == manager.levelLobbyMenu ||
                   level == manager.levelMainMenu ||
                   level == manager.levelSplashScreen ||
                   level == manager.levelRecording ||
                   level == manager.levelTutorial;
        }

        private static string GetLevelDisplayName(Level level)
        {
            if (level == null)
                return "Unknown";

            if (!string.IsNullOrWhiteSpace(level.NarrativeName))
                return level.NarrativeName;

            return level.name;
        }

        private static string GetPlayerName(PlayerAvatar avatar)
        {
            if (avatar == null)
                return "Unknown";

            string playerName = GetStringField(avatar, "playerName");
            return string.IsNullOrWhiteSpace(playerName) ? avatar.name : playerName;
        }

        private static string TruncateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Unknown";

            return name.Length > 30 ? name.Substring(0, 30) + "..." : name;
        }

        private static PlayerAvatar GetPlayerFromItemToggle(ItemToggle itemToggle)
        {
            int photonId = GetIntField(itemToggle, "playerTogglePhotonID");
            return photonId <= 0 ? null : SemiFunc.PlayerAvatarGetFromPhotonID(photonId);
        }

        private static void ApplyUpgradeToSteamId(string upgradeTypeName, string steamId)
        {
            switch (upgradeTypeName)
            {
                case "ItemUpgradePlayerHealth":
                    PunManager.instance.UpgradePlayerHealth(steamId);
                    break;
                case "ItemUpgradePlayerEnergy":
                    PunManager.instance.UpgradePlayerEnergy(steamId);
                    break;
                case "ItemUpgradePlayerExtraJump":
                    PunManager.instance.UpgradePlayerExtraJump(steamId);
                    break;
                case "ItemUpgradeMapPlayerCount":
                    PunManager.instance.UpgradeMapPlayerCount(steamId);
                    break;
                case "ItemUpgradePlayerTumbleLaunch":
                    PunManager.instance.UpgradePlayerTumbleLaunch(steamId);
                    break;
                case "ItemUpgradePlayerTumbleClimb":
                    PunManager.instance.UpgradePlayerTumbleClimb(steamId);
                    break;
                case "ItemUpgradeDeathHeadBattery":
                    PunManager.instance.UpgradeDeathHeadBattery(steamId);
                    break;
                case "ItemUpgradePlayerTumbleWings":
                    PunManager.instance.UpgradePlayerTumbleWings(steamId);
                    break;
                case "ItemUpgradePlayerSprintSpeed":
                    PunManager.instance.UpgradePlayerSprintSpeed(steamId);
                    break;
                case "ItemUpgradePlayerCrouchRest":
                    PunManager.instance.UpgradePlayerCrouchRest(steamId);
                    break;
                case "ItemUpgradePlayerGrabStrength":
                    PunManager.instance.UpgradePlayerGrabStrength(steamId);
                    break;
                case "ItemUpgradePlayerGrabThrow":
                    PunManager.instance.UpgradePlayerThrowStrength(steamId);
                    break;
                case "ItemUpgradePlayerGrabRange":
                    PunManager.instance.UpgradePlayerGrabRange(steamId);
                    break;
            }
        }

        private static void SyncLevelToClients(RunManager manager, Level level)
        {
            if (!GameManager.Multiplayer())
                return;

            object runManagerPUN = GetFieldValue<object>(manager, "runManagerPUN");
            PhotonView photonView = GetFieldValue<PhotonView>(runManagerPUN, "photonView");
            photonView?.RPC("UpdateLevelRPC", RpcTarget.OthersBuffered, level.name, manager.levelsCompleted, false);
        }

        private static void SetSaveLevel(RunManager manager, Level level)
        {
            int saveLevel = 0;
            if (manager.levelShop != null && manager.levelShop.Contains(level))
                saveLevel = 1;
            else if (level == manager.levelLobby)
                saveLevel = 2;

            SetFieldValue(manager, "saveLevel", saveLevel);
            SemiFunc.StatSetSaveLevel(saveLevel);
        }

        private static T GetFieldValue<T>(object instance, string fieldName) where T : class
        {
            if (instance == null)
                return null;

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field?.GetValue(instance) as T;
        }

        private static bool GetBoolField(object instance, string fieldName)
        {
            if (instance == null)
                return false;

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field != null && field.GetValue(instance) is bool value && value;
        }

        private static int GetIntField(object instance, string fieldName)
        {
            if (instance == null)
                return -1;

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field != null && field.GetValue(instance) is int value ? value : -1;
        }

        private static string GetStringField(object instance, string fieldName)
        {
            if (instance == null)
                return null;

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field?.GetValue(instance) as string;
        }

        private static void SetFieldValue(object instance, string fieldName, object value)
        {
            if (instance == null)
                return;

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field?.SetValue(instance, value);
        }
    }
}
