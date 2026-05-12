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
        public const int MaxLevelNumber = 1000000000;
        public const int MaxMoney = int.MaxValue;
        public const int MaxUpgradeLevel = 100000000;
        private static readonly Dictionary<int, FrozenPlayerState> FrozenPlayers = new Dictionary<int, FrozenPlayerState>();
        private static float nextFreezeSyncTime;

        private sealed class FrozenPlayerState
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }

        public sealed class UpgradeEntry
        {
            public string Id;
            public string Label;
            public int CurrentLevel;
        }

        private sealed class UpgradeDefinition
        {
            public string Id;
            public string Label;
            public string DictionaryField;
        }

        private static readonly UpgradeDefinition[] UpgradeDefinitions =
        {
            new UpgradeDefinition { Id = "Health", Label = "Health", DictionaryField = "playerUpgradeHealth" },
            new UpgradeDefinition { Id = "Stamina", Label = "Stamina", DictionaryField = "playerUpgradeStamina" },
            new UpgradeDefinition { Id = "ExtraJump", Label = "Extra Jump", DictionaryField = "playerUpgradeExtraJump" },
            new UpgradeDefinition { Id = "MapPlayerCount", Label = "Map Player Count", DictionaryField = "playerUpgradeMapPlayerCount" },
            new UpgradeDefinition { Id = "Launch", Label = "Tumble Launch", DictionaryField = "playerUpgradeLaunch" },
            new UpgradeDefinition { Id = "TumbleClimb", Label = "Tumble Climb", DictionaryField = "playerUpgradeTumbleClimb" },
            new UpgradeDefinition { Id = "DeathHeadBattery", Label = "Death Head Battery", DictionaryField = "playerUpgradeDeathHeadBattery" },
            new UpgradeDefinition { Id = "TumbleWings", Label = "Tumble Wings", DictionaryField = "playerUpgradeTumbleWings" },
            new UpgradeDefinition { Id = "Speed", Label = "Sprint Speed", DictionaryField = "playerUpgradeSpeed" },
            new UpgradeDefinition { Id = "CrouchRest", Label = "Crouch Rest", DictionaryField = "playerUpgradeCrouchRest" },
            new UpgradeDefinition { Id = "Strength", Label = "Grab Strength", DictionaryField = "playerUpgradeStrength" },
            new UpgradeDefinition { Id = "Throw", Label = "Throw Strength", DictionaryField = "playerUpgradeThrow" },
            new UpgradeDefinition { Id = "Range", Label = "Grab Range", DictionaryField = "playerUpgradeRange" }
        };

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
            UpdateFrozenPlayers();

            if (IsDisableSpawnAIActive() && Time.time >= nextEnemyDespawnTime)
            {
                nextEnemyDespawnTime = Time.time + 0.75f;
                DespawnAllEnemies();
            }
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

        public static int GetCurrentLevelNumber()
        {
            RunManager manager = RunManager.instance;
            if (manager == null)
                return 1;

            return Mathf.Clamp(manager.levelsCompleted + 1, 1, MaxLevelNumber);
        }

        public static int GetCurrentMoney()
        {
            try
            {
                return Mathf.Clamp(SemiFunc.StatGetRunCurrency(), 0, MaxMoney);
            }
            catch
            {
                return 0;
            }
        }

        public static bool SetLevel(int levelNumber, out string message)
        {
            message = string.Empty;
            if (!CanRunSessionMutation(LocalizedMessages.Get("setLevelActionName"), out message))
                return false;

            RunManager manager = RunManager.instance;
            if (manager == null || manager.levelCurrent == null || GetBoolField(manager, "restarting") || GetBoolField(manager, "restartingDone") || GetBoolField(manager, "waitToChangeScene"))
            {
                message = LocalizedMessages.Get("mapChangeBlocked");
                return false;
            }

            if (GameDirector.instance != null && GameDirector.instance.currentState != GameDirector.gameState.Main)
            {
                message = LocalizedMessages.Get("mapChangeBlocked");
                return false;
            }

            int levelsCompleted = Mathf.Clamp(levelNumber - 1, 0, MaxLevelNumber - 1);
            manager.levelsCompleted = levelsCompleted;
            SemiFunc.StatSetRunLevel(levelsCompleted);
            SyncLevelToClients(manager, manager.levelCurrent);
            manager.RestartScene();
            SemiFunc.OnSceneSwitch(false, false);
            message = LocalizedMessages.Format("levelSet", levelNumber);
            return true;
        }

        public static bool SetMoney(int money, out string message)
        {
            message = string.Empty;
            if (!CanRunSessionMutation(LocalizedMessages.Get("setMoneyActionName"), out message))
                return false;

            money = Mathf.Clamp(money, 0, MaxMoney);
            SemiFunc.StatSetRunCurrency(money);
            SemiFunc.ShopUpdateCost();
            message = LocalizedMessages.Format("moneySet", money);
            return true;
        }

        public static bool AddMoney(int amount, out string message)
        {
            message = string.Empty;
            if (!CanRunSessionMutation(LocalizedMessages.Get("addMoneyActionName"), out message))
                return false;

            int current = GetCurrentMoney();
            if (amount < 0 || current > MaxMoney - amount)
            {
                message = LocalizedMessages.Format("moneyTotalOutOfRange", MaxMoney);
                return false;
            }

            int updated = current + amount;
            SemiFunc.StatSetRunCurrency(updated);
            SemiFunc.ShopUpdateCost();
            message = LocalizedMessages.Format("moneySet", updated);
            return true;
        }

        public static bool ToggleFreeze(PlayerAvatar avatar, out string message)
        {
            message = string.Empty;
            if (!CanRunPlayerAction(avatar, LocalizedMessages.Get("freezeActionName"), out message))
                return false;

            int key = GetPlayerKey(avatar);
            if (FrozenPlayers.ContainsKey(key))
            {
                FrozenPlayers.Remove(key);
                message = LocalizedMessages.Get("playerUnfrozen");
            }
            else
            {
                FrozenPlayers[key] = new FrozenPlayerState
                {
                    Position = avatar.transform.position,
                    Rotation = avatar.transform.rotation
                };
                message = LocalizedMessages.Get("playerFrozen");
            }

            return true;
        }

        public static bool IsFrozen(PlayerAvatar avatar)
        {
            return avatar != null && FrozenPlayers.ContainsKey(GetPlayerKey(avatar));
        }

        public static string GetPlayerSteamId(PlayerAvatar avatar)
        {
            return avatar == null ? null : SemiFunc.PlayerGetSteamID(avatar);
        }

        public static List<UpgradeEntry> GetPlayerUpgrades(PlayerAvatar avatar)
        {
            string steamId = GetPlayerSteamId(avatar);
            if (string.IsNullOrWhiteSpace(steamId) || StatsManager.instance == null)
                return new List<UpgradeEntry>();

            List<UpgradeEntry> result = new List<UpgradeEntry>();
            foreach (UpgradeDefinition definition in UpgradeDefinitions)
            {
                result.Add(new UpgradeEntry
                {
                    Id = definition.Id,
                    Label = definition.Label,
                    CurrentLevel = GetUpgradeLevel(definition, steamId)
                });
            }

            return result;
        }

        public static bool SetPlayerUpgrade(PlayerAvatar avatar, string upgradeId, int targetLevel, out string message)
        {
            message = string.Empty;
            if (!CanRunPlayerAction(avatar, LocalizedMessages.Get("upgradesActionName"), out message))
                return false;

            UpgradeDefinition definition = GetUpgradeDefinition(upgradeId);
            string steamId = GetPlayerSteamId(avatar);
            if (definition == null || string.IsNullOrWhiteSpace(steamId))
            {
                message = LocalizedMessages.Get("playerActionFailed");
                return false;
            }

            targetLevel = Mathf.Clamp(targetLevel, 0, MaxUpgradeLevel);
            int currentLevel = GetUpgradeLevel(definition, steamId);
            return ApplyPlayerUpgradeDelta(steamId, definition, targetLevel - currentLevel, targetLevel, out message);
        }

        public static bool AddPlayerUpgrade(PlayerAvatar avatar, string upgradeId, int amount, out string message)
        {
            message = string.Empty;
            if (!CanRunPlayerAction(avatar, LocalizedMessages.Get("upgradesActionName"), out message))
                return false;

            UpgradeDefinition definition = GetUpgradeDefinition(upgradeId);
            string steamId = GetPlayerSteamId(avatar);
            if (definition == null || string.IsNullOrWhiteSpace(steamId))
            {
                message = LocalizedMessages.Get("playerActionFailed");
                return false;
            }

            int currentLevel = GetUpgradeLevel(definition, steamId);
            if (amount < 0 || currentLevel > MaxUpgradeLevel - amount)
            {
                message = LocalizedMessages.Format("numberOutOfRange", LocalizedMessages.Get("upgradeValue"), 0, MaxUpgradeLevel);
                return false;
            }

            int targetLevel = currentLevel + amount;
            return ApplyPlayerUpgradeDelta(steamId, definition, amount, targetLevel, out message);
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

        private static bool CanRunSessionMutation(string functionName, out string message)
        {
            message = string.Empty;
            if (!HostOnlyGuard.IsPlayableSession())
            {
                message = LocalizedMessages.Get("playableOnly");
                return false;
            }

            return HostOnlyGuard.CanUseHostOnly(true, functionName);
        }

        private static void UpdateFrozenPlayers()
        {
            if (FrozenPlayers.Count == 0)
                return;

            if (!HostOnlyGuard.IsHostOnlyActive())
            {
                FrozenPlayers.Clear();
                return;
            }

            if (Time.unscaledTime < nextFreezeSyncTime)
                return;

            nextFreezeSyncTime = Time.unscaledTime + 0.12f;
            foreach (PlayerEntry entry in GetPlayers().ToList())
            {
                if (entry.Avatar == null)
                    continue;

                int key = GetPlayerKey(entry.Avatar);
                if (!FrozenPlayers.TryGetValue(key, out FrozenPlayerState state))
                    continue;

                Teleport(entry.Avatar, state.Position, state.Rotation);
            }
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

        private static int GetPlayerKey(PlayerAvatar avatar)
        {
            if (avatar == null)
                return 0;

            return avatar.photonView != null ? avatar.photonView.ViewID : avatar.GetInstanceID();
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

        private static UpgradeDefinition GetUpgradeDefinition(string upgradeId)
        {
            return UpgradeDefinitions.FirstOrDefault(definition => definition.Id == upgradeId);
        }

        private static int GetUpgradeLevel(UpgradeDefinition definition, string steamId)
        {
            Dictionary<string, int> dictionary = GetUpgradeDictionary(definition);
            if (dictionary == null || string.IsNullOrWhiteSpace(steamId))
                return 0;

            return dictionary.TryGetValue(steamId, out int value) ? Mathf.Clamp(value, 0, MaxUpgradeLevel) : 0;
        }

        private static Dictionary<string, int> GetUpgradeDictionary(UpgradeDefinition definition)
        {
            if (definition == null || StatsManager.instance == null)
                return null;

            FieldInfo field = typeof(StatsManager).GetField(definition.DictionaryField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field?.GetValue(StatsManager.instance) as Dictionary<string, int>;
        }

        private static bool ApplyPlayerUpgradeDelta(string steamId, UpgradeDefinition definition, int delta, int targetLevel, out string message)
        {
            message = string.Empty;
            if (delta == 0)
            {
                message = LocalizedMessages.Format("upgradeSet", definition.Label, targetLevel);
                return true;
            }

            if (PunManager.instance == null)
            {
                message = LocalizedMessages.Get("playerActionFailed");
                return false;
            }

            if (GameManager.Multiplayer())
            {
                PhotonView photonView = GetFieldValue<PhotonView>(PunManager.instance, "photonView");
                if (photonView == null)
                {
                    message = LocalizedMessages.Get("playerActionFailed");
                    return false;
                }

                photonView.RPC("TesterUpgradeCommandRPC", RpcTarget.All, steamId, definition.Id, delta);
            }
            else
            {
                ApplyUpgradeDeltaLocal(definition.Id, steamId, delta);
            }

            message = LocalizedMessages.Format("upgradeSet", definition.Label, targetLevel);
            return true;
        }

        private static void ApplyUpgradeDeltaLocal(string upgradeId, string steamId, int delta)
        {
            switch (upgradeId)
            {
                case "Health":
                    PunManager.instance.UpgradePlayerHealth(steamId, delta);
                    break;
                case "Stamina":
                    PunManager.instance.UpgradePlayerEnergy(steamId, delta);
                    break;
                case "ExtraJump":
                    PunManager.instance.UpgradePlayerExtraJump(steamId, delta);
                    break;
                case "MapPlayerCount":
                    PunManager.instance.UpgradeMapPlayerCount(steamId, delta);
                    break;
                case "Launch":
                    PunManager.instance.UpgradePlayerTumbleLaunch(steamId, delta);
                    break;
                case "TumbleClimb":
                    PunManager.instance.UpgradePlayerTumbleClimb(steamId, delta);
                    break;
                case "DeathHeadBattery":
                    PunManager.instance.UpgradeDeathHeadBattery(steamId, delta);
                    break;
                case "TumbleWings":
                    PunManager.instance.UpgradePlayerTumbleWings(steamId, delta);
                    break;
                case "Speed":
                    PunManager.instance.UpgradePlayerSprintSpeed(steamId, delta);
                    break;
                case "CrouchRest":
                    PunManager.instance.UpgradePlayerCrouchRest(steamId, delta);
                    break;
                case "Strength":
                    PunManager.instance.UpgradePlayerGrabStrength(steamId, delta);
                    break;
                case "Throw":
                    PunManager.instance.UpgradePlayerThrowStrength(steamId, delta);
                    break;
                case "Range":
                    PunManager.instance.UpgradePlayerGrabRange(steamId, delta);
                    break;
            }
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
