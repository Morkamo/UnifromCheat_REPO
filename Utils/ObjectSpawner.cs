using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnifromCheat_REPO.Utils
{
    internal static class ObjectSpawner
    {
        public enum SpawnableKind
        {
            Item,
            Valuable,
            Entity
        }

        public sealed class SpawnableEntry
        {
            public string DisplayName;
            public string ResourcePath;
            public GameObject Prefab;
            public SpawnableKind Kind;
            public EnemySetup EnemySetup;
            public PrefabRef PrefabRef;
            public bool IsCosmeticWorldObject;
            public SemiFunc.Rarity CosmeticRarity;
        }

        private static List<Item> cachedItems;
        private static List<SpawnableEntry> cachedValuables;
        private static List<SpawnableEntry> cachedEntities;
        private static bool itemsCacheBuilt;
        private static bool valuablesCacheBuilt;
        private static bool entitiesCacheBuilt;
        private static readonly HashSet<int> spawnedCosmeticWorldObjectInstanceIds = new HashSet<int>();
        private static readonly HashSet<int> spawnedCosmeticWorldObjectViewIds = new HashSet<int>();
        private static readonly Dictionary<int, SemiFunc.Rarity> spawnedCosmeticWorldObjectRarities = new Dictionary<int, SemiFunc.Rarity>();
        private const string CosmeticWorldObjectSpawnMarker = "UnifromCosmeticWorldObject";

        public static List<Item> GetSpawnableItems()
        {
            if (itemsCacheBuilt &&
                cachedItems != null &&
                (cachedItems.Count > 0 || StatsManager.instance?.itemDictionary == null || StatsManager.instance.itemDictionary.Count == 0))
                return cachedItems;

            EnsureItemDictionaryLoaded();

            IEnumerable<Item> items = StatsManager.instance?.itemDictionary != null && StatsManager.instance.itemDictionary.Count > 0
                ? StatsManager.instance.itemDictionary.Values
                : LoadItemAssets();

            cachedItems = items
                .Where(item => item != null && !item.disabled && item.physicalItem && !string.IsNullOrWhiteSpace(GetAssetName(item)))
                .OrderBy(item => GetDisplayName(item))
                .ToList();
            itemsCacheBuilt = true;

            return cachedItems;
        }

        public static List<SpawnableEntry> GetSpawnableValuables()
        {
            if (valuablesCacheBuilt && cachedValuables != null)
                return cachedValuables;

            Dictionary<string, SpawnableEntry> valuables = new Dictionary<string, SpawnableEntry>();
            Dictionary<string, SpawnableEntry> cosmeticBoxes = new Dictionary<string, SpawnableEntry>();

            AddValuableRefs(valuables, GetFieldValue<List<PrefabRef>>(ValuableDirector.instance, "tinyValuables"));
            AddValuableRefs(valuables, GetFieldValue<List<PrefabRef>>(ValuableDirector.instance, "smallValuables"));
            AddValuableRefs(valuables, GetFieldValue<List<PrefabRef>>(ValuableDirector.instance, "mediumValuables"));
            AddValuableRefs(valuables, GetFieldValue<List<PrefabRef>>(ValuableDirector.instance, "bigValuables"));
            AddValuableRefs(valuables, GetFieldValue<List<PrefabRef>>(ValuableDirector.instance, "wideValuables"));
            AddValuableRefs(valuables, GetFieldValue<List<PrefabRef>>(ValuableDirector.instance, "tallValuables"));
            AddValuableRefs(valuables, GetFieldValue<List<PrefabRef>>(ValuableDirector.instance, "veryTallValuables"));

            if (ValuableDirector.instance?.cosmeticWorldObjectSetups != null)
            {
                foreach (var setup in ValuableDirector.instance.cosmeticWorldObjectSetups)
                    AddCosmeticWorldObjectSetup(cosmeticBoxes, setup);
            }

            foreach (GameObject prefab in Resources.LoadAll<GameObject>("Valuables"))
            {
                if (prefab == null)
                    continue;

                string resourcePath = FindResourcePathForLoadedPrefab(prefab, GetValuableResourceFolders());
                if (string.IsNullOrWhiteSpace(resourcePath))
                    continue;

                CosmeticWorldObject cosmeticWorldObject = prefab.GetComponentInChildren<CosmeticWorldObject>(true);
                if (cosmeticWorldObject != null)
                    AddPrefab(cosmeticBoxes, prefab, resourcePath, SpawnableKind.Valuable, null, null, null, null, true, cosmeticWorldObject.rarity);
                else if (prefab.GetComponentInChildren<ValuableObject>(true) != null)
                    AddPrefab(valuables, prefab, resourcePath, SpawnableKind.Valuable);
            }

            cachedValuables = valuables.Values
                .OrderBy(entry => entry.DisplayName)
                .Concat(cosmeticBoxes.Values.OrderBy(entry => entry.DisplayName))
                .ToList();
            valuablesCacheBuilt = true;

            return cachedValuables;
        }

        public static List<SpawnableEntry> GetSpawnableEntities()
        {
            if (entitiesCacheBuilt && cachedEntities != null)
                return cachedEntities;

            Dictionary<string, SpawnableEntry> entries = new Dictionary<string, SpawnableEntry>();

            AddEnemySetups(entries, EnemyDirector.instance?.enemiesDifficulty1);
            AddEnemySetups(entries, EnemyDirector.instance?.enemiesDifficulty2);
            AddEnemySetups(entries, EnemyDirector.instance?.enemiesDifficulty3);
            AddEnemySetups(entries, GetFieldValue<List<EnemySetup>>(EnemyDirector.instance, "enemyList"));

            foreach (GameObject prefab in Resources.LoadAll<GameObject>("Enemies"))
            {
                if (prefab == null)
                    continue;

                string resourcePath = FindResourcePathForLoadedPrefab(prefab, GetEnemyResourceFolders());
                if (string.IsNullOrWhiteSpace(resourcePath))
                    continue;

                if (prefab.GetComponentInChildren<EnemyParent>(true) != null ||
                    prefab.GetComponentInChildren<Enemy>(true) != null)
                    AddPrefab(entries, prefab, resourcePath, SpawnableKind.Entity);
            }

            cachedEntities = entries.Values
                .OrderBy(entry => entry.DisplayName)
                .ToList();
            entitiesCacheBuilt = true;

            return cachedEntities;
        }

        public static bool SpawnItem(Item item, out string resultMessage)
        {
            resultMessage = string.Empty;
            GameObject prefab = GetPrefab(item);
            if (item == null || prefab == null)
            {
                resultMessage = LocalizedMessages.Get("itemPrefabNotFound");
                return false;
            }

            if (!CanSpawn(out resultMessage))
                return false;

            GetSpawnPose(out Vector3 spawnPosition, out Quaternion baseRotation);
            Quaternion spawnRotation = baseRotation * item.spawnRotationOffset;

            string resourcePath = GetItemResourcePath(item);
            GameObject spawned = SemiFunc.IsMultiplayer()
                ? PhotonNetwork.InstantiateRoomObject(resourcePath, spawnPosition, spawnRotation, 0)
                : Object.Instantiate(prefab, spawnPosition, spawnRotation);

            if (spawned == null)
            {
                resultMessage = LocalizedMessages.Format("spawnFailed", GetDisplayName(item));
                return false;
            }

            if (Core.Instance != null)
                Core.Instance.StartCoroutine(ChargeBatteryWhenReady(spawned));

            resultMessage = LocalizedMessages.Format("spawnedItem", GetDisplayName(item));
            return true;
        }

        public static bool SpawnEntry(SpawnableEntry entry, out string resultMessage)
        {
            resultMessage = string.Empty;
            if (entry == null || entry.Prefab == null || string.IsNullOrWhiteSpace(entry.ResourcePath))
            {
                resultMessage = LocalizedMessages.Get("itemPrefabNotFound");
                return false;
            }

            if (!CanSpawn(out resultMessage))
                return false;

            GetSpawnPose(out Vector3 spawnPosition, out Quaternion spawnRotation);

            if (entry.Kind == SpawnableKind.Entity && SpawnEntity(entry, spawnPosition, out resultMessage))
                return true;

            object[] instantiationData = entry.IsCosmeticWorldObject
                ? new object[] { CosmeticWorldObjectSpawnMarker, (int)entry.CosmeticRarity }
                : null;
            GameObject spawned = SemiFunc.IsMultiplayer()
                ? PhotonNetwork.InstantiateRoomObject(entry.ResourcePath, spawnPosition, spawnRotation, 0, instantiationData)
                : Object.Instantiate(entry.Prefab, spawnPosition, spawnRotation);

            if (spawned == null)
            {
                resultMessage = LocalizedMessages.Format("spawnFailed", entry.DisplayName);
                return false;
            }

            if (entry.Kind == SpawnableKind.Entity)
            {
                EnemyParent enemyParent = spawned.GetComponentInChildren<EnemyParent>(true);
                InitializeSpawnedEnemy(enemyParent, spawnPosition);
            }
            else if (entry.IsCosmeticWorldObject && Core.Instance != null)
            {
                SyncCosmeticWorldObjectTargetAmount();
                Core.Instance.StartCoroutine(InitializeSpawnedCosmeticWorldObjectWhenReady(spawned, entry.CosmeticRarity));
            }

            resultMessage = LocalizedMessages.Format("spawnedItem", entry.DisplayName);
            return true;
        }

        public static bool TryGetSpawnedCosmeticWorldObjectRarity(CosmeticWorldObject cosmeticWorldObject, out SemiFunc.Rarity rarity)
        {
            rarity = SemiFunc.Rarity.Common;
            if (cosmeticWorldObject == null)
                return false;

            int instanceId = cosmeticWorldObject.GetInstanceID();
            if (spawnedCosmeticWorldObjectInstanceIds.Contains(instanceId) &&
                spawnedCosmeticWorldObjectRarities.TryGetValue(instanceId, out rarity))
                return true;

            PhotonView photonView = GetCosmeticWorldObjectPhotonView(cosmeticWorldObject);
            int viewId = photonView != null ? photonView.ViewID : 0;
            if (viewId != 0 &&
                spawnedCosmeticWorldObjectViewIds.Contains(viewId) &&
                spawnedCosmeticWorldObjectRarities.TryGetValue(viewId, out rarity))
                return true;

            return false;
        }

        public static bool TryRegisterSpawnedCosmeticWorldObjectFromInstantiationData(CosmeticWorldObject cosmeticWorldObject)
        {
            if (cosmeticWorldObject == null)
                return false;

            PhotonView photonView = GetCosmeticWorldObjectPhotonView(cosmeticWorldObject);
            object[] data = photonView?.InstantiationData;
            if (data == null || data.Length < 2 || !(data[0] is string marker) || marker != CosmeticWorldObjectSpawnMarker)
                return false;

            int rarityValue;
            if (data[1] is int intValue)
                rarityValue = intValue;
            else if (data[1] is byte byteValue)
                rarityValue = byteValue;
            else
                return false;

            SemiFunc.Rarity rarity = (SemiFunc.Rarity)rarityValue;
            cosmeticWorldObject.rarity = rarity;
            RegisterSpawnedCosmeticWorldObject(cosmeticWorldObject, rarity);
            return true;
        }

        public static int GetSpawnedCosmeticWorldObjectCount()
        {
            CleanupSpawnedCosmeticWorldObjectIds();
            return SemiFunc.IsMultiplayer()
                ? spawnedCosmeticWorldObjectViewIds.Count
                : spawnedCosmeticWorldObjectInstanceIds.Count;
        }

        public static string GetDisplayName(Item item)
        {
            if (item == null)
                return "Unknown Item";

            if (!string.IsNullOrWhiteSpace(item.itemName) && item.itemName != "N/A")
                return item.itemName;

            string assetName = GetAssetName(item);
            return string.IsNullOrWhiteSpace(assetName) ? item.name : assetName;
        }

        private static bool CanSpawn(out string resultMessage)
        {
            resultMessage = string.Empty;

            if (!HostOnlyGuard.IsPlayableSession())
                return false;

            if (!HostOnlyGuard.CanUseHostOnly(false))
            {
                resultMessage = LocalizedMessages.Format("hostOnlyFunctionUnavailable", "Object Spawner");
                return false;
            }

            return true;
        }

        private static string GetAssetName(Item item)
        {
            if (item == null)
                return null;

            if (StatsManager.instance?.itemDictionary != null)
            {
                foreach (var pair in StatsManager.instance.itemDictionary)
                {
                    if (pair.Value == item)
                        return pair.Key;
                }
            }

            return item.name;
        }

        private static GameObject GetPrefab(Item item)
        {
            if (item?.prefab?.Prefab != null)
                return item.prefab.Prefab;

            string resourcePath = GetItemResourcePath(item);
            return string.IsNullOrWhiteSpace(resourcePath)
                ? null
                : Resources.Load<GameObject>(resourcePath);
        }

        private static string GetItemResourcePath(Item item)
        {
            List<string> candidates = new List<string>();

            if (item?.prefab != null)
            {
                candidates.Add(CombineResourcePath(item.prefab.ResourcePath, item.prefab.PrefabName));
                candidates.Add(item.prefab.ResourcePath);
                if (!string.IsNullOrWhiteSpace(item.prefab.PrefabName))
                    candidates.Add("Items/" + item.prefab.PrefabName);
            }

            string itemAssetName = GetStringField(item, "itemAssetName");
            if (!string.IsNullOrWhiteSpace(itemAssetName))
                candidates.Add("Items/" + itemAssetName);

            if (item?.prefab?.Prefab != null)
                candidates.Add("Items/" + item.prefab.Prefab.name);

            string assetName = GetAssetName(item);
            if (!string.IsNullOrWhiteSpace(assetName))
                candidates.Add("Items/" + assetName);

            foreach (string candidate in candidates)
            {
                string path = NormalizeResourcePath(candidate);
                if (string.IsNullOrWhiteSpace(path))
                    continue;

                if (Resources.Load<GameObject>(path) != null)
                    return path;
            }

            return NormalizeResourcePath(candidates.FirstOrDefault(path => !string.IsNullOrWhiteSpace(path)));
        }

        private static IEnumerable<Item> LoadItemAssets()
        {
            Dictionary<string, Item> result = new Dictionary<string, Item>();
            AddItemAssets(result, Resources.LoadAll<Item>("Items"));
            AddItemAssets(result, Resources.LoadAll<Item>("ScriptableObjects/Items"));
            AddItemAssets(result, Resources.LoadAll<Item>("ScriptableObjects"));
            return result.Values;
        }

        private static void EnsureItemDictionaryLoaded()
        {
            if (StatsManager.instance == null)
                return;

            if (StatsManager.instance.itemDictionary != null && StatsManager.instance.itemDictionary.Count > 0)
                return;

            MethodInfo loadItemsMethod = typeof(StatsManager).GetMethod(
                "LoadItemsFromFolder",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            loadItemsMethod?.Invoke(StatsManager.instance, null);
        }

        private static void AddItemAssets(Dictionary<string, Item> result, IEnumerable<Item> items)
        {
            if (items == null)
                return;

            foreach (Item item in items)
            {
                if (item == null)
                    continue;

                string key = GetItemResourcePath(item) ?? item.name;
                if (!result.ContainsKey(key))
                    result[key] = item;
            }
        }

        private static void AddValuableRefs(Dictionary<string, SpawnableEntry> entries, List<PrefabRef> prefabRefs)
        {
            if (prefabRefs == null)
                return;

            foreach (PrefabRef prefabRef in prefabRefs)
                AddPrefabRef(entries, prefabRef, SpawnableKind.Valuable);
        }

        private static void AddEnemySetups(Dictionary<string, SpawnableEntry> entries, List<EnemySetup> setups)
        {
            if (setups == null)
                return;

            foreach (EnemySetup setup in setups)
            {
                if (setup?.spawnObjects == null)
                    continue;

                foreach (PrefabRef prefabRef in setup.spawnObjects)
                    AddPrefabRef(entries, prefabRef, SpawnableKind.Entity, setup);
            }
        }

        private static void AddPrefabRef(Dictionary<string, SpawnableEntry> entries, PrefabRef prefabRef, SpawnableKind kind, EnemySetup enemySetup = null)
        {
            if (prefabRef == null)
                return;

            string resourcePath = NormalizeResourcePath(prefabRef.ResourcePath);
            AddPrefab(entries, prefabRef.Prefab, resourcePath, kind, enemySetup, prefabRef);
        }

        private static void AddCosmeticWorldObjectSetup(Dictionary<string, SpawnableEntry> entries, ValuableDirector.CosmeticWorldObjectSetup setup)
        {
            if (setup?.prefab == null)
                return;

            string resourcePath = NormalizeResourcePath(setup.prefab.ResourcePath);
            string keyOverride = resourcePath + ":cosmetic:" + (int)setup.rarity;
            string displayName = CleanPrefabName(setup.prefab.Prefab != null ? setup.prefab.Prefab.name : setup.prefab.PrefabName);
            displayName = string.IsNullOrWhiteSpace(displayName)
                ? FormatCosmeticRarity(setup.rarity) + " Cosmetic Box"
                : displayName + " " + FormatCosmeticRarity(setup.rarity);

            AddPrefab(entries, setup.prefab.Prefab, resourcePath, SpawnableKind.Valuable, null, setup.prefab, keyOverride, displayName, true, setup.rarity);
        }

        private static void AddPrefab(
            Dictionary<string, SpawnableEntry> entries,
            GameObject prefab,
            string resourcePath,
            SpawnableKind kind,
            EnemySetup enemySetup = null,
            PrefabRef prefabRef = null,
            string keyOverride = null,
            string displayNameOverride = null,
            bool isCosmeticWorldObject = false,
            SemiFunc.Rarity cosmeticRarity = SemiFunc.Rarity.Common)
        {
            if (prefab == null || string.IsNullOrWhiteSpace(resourcePath))
                return;

            string key = NormalizeResourcePath(keyOverride) ?? NormalizeResourcePath(resourcePath);
            if (entries.ContainsKey(key))
                return;

            entries[key] = new SpawnableEntry
            {
                DisplayName = displayNameOverride ?? CleanPrefabName(prefab.name),
                ResourcePath = NormalizeResourcePath(resourcePath),
                Prefab = prefab,
                Kind = kind,
                EnemySetup = enemySetup,
                PrefabRef = prefabRef,
                IsCosmeticWorldObject = isCosmeticWorldObject,
                CosmeticRarity = cosmeticRarity
            };
        }

        private static T GetFieldValue<T>(object instance, string fieldName) where T : class
        {
            if (instance == null)
                return null;

            FieldInfo field = instance.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            return field?.GetValue(instance) as T;
        }

        private static string GetStringField(object instance, string fieldName)
        {
            if (instance == null)
                return null;

            FieldInfo field = instance.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            return field?.GetValue(instance) as string;
        }

        private static bool SpawnEntity(SpawnableEntry entry, Vector3 spawnPosition, out string resultMessage)
        {
            resultMessage = string.Empty;
            if (entry.EnemySetup != null && LevelGenerator.Instance != null)
            {
                LevelGenerator.Instance.EnemySpawn(entry.EnemySetup, spawnPosition);
                if (Core.Instance != null)
                    Core.Instance.StartCoroutine(ForceEnemiesAtPositionWhenReady(spawnPosition));
                resultMessage = LocalizedMessages.Format("spawnedItem", entry.DisplayName);
                return true;
            }

            return false;
        }

        private static void InitializeSpawnedEnemy(EnemyParent enemyParent, Vector3 spawnPosition)
        {
            if (enemyParent == null)
                return;

            SetFieldValue(enemyParent, "SetupDone", true);
            SetFieldValue(enemyParent, "firstSpawnPointUsed", true);
            enemyParent.GetComponentInChildren<Enemy>(true)?.EnemyTeleported(spawnPosition);

            if (LevelGenerator.Instance != null)
                SetFieldValue(LevelGenerator.Instance, "EnemiesSpawnTarget", GetIntField(LevelGenerator.Instance, "EnemiesSpawnTarget") + 1);

            EnemyDirector.instance?.FirstSpawnPointAdd(enemyParent);

            if (Core.Instance != null)
                Core.Instance.StartCoroutine(ForceEnemySpawnWhenReady(enemyParent, spawnPosition));
        }

        private static IEnumerator ForceEnemySpawnWhenReady(EnemyParent enemyParent, Vector3 spawnPosition)
        {
            for (int i = 0; i < 20; i++)
            {
                if (enemyParent == null)
                    yield break;

                enemyParent.DespawnedTimerSet(0f);
                yield return null;
            }

            MethodInfo method = typeof(EnemyParent).GetMethod(
                "Spawn",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            method?.Invoke(enemyParent, null);
            if (Core.Instance != null)
                Core.Instance.StartCoroutine(PinEnemyAtSpawnPosition(enemyParent, spawnPosition));
        }

        private static IEnumerator ForceEnemiesAtPositionWhenReady(Vector3 spawnPosition)
        {
            List<EnemyParent> spawnedEnemies = new List<EnemyParent>();
            for (int i = 0; i < 25; i++)
            {
                EnemyParent[] enemies = Object.FindObjectsOfType<EnemyParent>(true);
                foreach (EnemyParent enemyParent in enemies)
                {
                    if (enemyParent == null)
                        continue;

                    float distance = Vector3.Distance(enemyParent.transform.position, spawnPosition);
                    if (distance <= 4f)
                    {
                        SetFieldValue(enemyParent, "firstSpawnPointUsed", true);
                        enemyParent.DespawnedTimerSet(0f);
                        if (!spawnedEnemies.Contains(enemyParent))
                            spawnedEnemies.Add(enemyParent);
                    }
                }

                yield return null;
            }

            MethodInfo method = typeof(EnemyParent).GetMethod(
                "Spawn",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (EnemyParent enemyParent in spawnedEnemies)
            {
                if (enemyParent == null)
                    continue;

                method?.Invoke(enemyParent, null);
                if (Core.Instance != null)
                    Core.Instance.StartCoroutine(PinEnemyAtSpawnPosition(enemyParent, spawnPosition));
            }
        }

        private static IEnumerator PinEnemyAtSpawnPosition(EnemyParent enemyParent, Vector3 spawnPosition)
        {
            for (int i = 0; i < 30; i++)
            {
                if (enemyParent == null)
                    yield break;

                SetFieldValue(enemyParent, "firstSpawnPointUsed", true);
                Enemy enemy = enemyParent.GetComponentInChildren<Enemy>(true);
                enemy?.EnemyTeleported(spawnPosition);
                yield return null;
            }
        }

        private static IEnumerator InitializeSpawnedCosmeticWorldObjectWhenReady(GameObject spawned, SemiFunc.Rarity rarity)
        {
            for (int i = 0; i < 60; i++)
            {
                if (spawned == null)
                    yield break;

                CosmeticWorldObject cosmeticWorldObject = spawned.GetComponentInChildren<CosmeticWorldObject>(true);
                if (cosmeticWorldObject != null)
                {
                    cosmeticWorldObject.rarity = rarity;
                    RegisterSpawnedCosmeticWorldObject(cosmeticWorldObject, rarity);
                }

                PhotonView photonView = spawned.GetComponent<PhotonView>() ?? spawned.GetComponentInChildren<PhotonView>(true);
                if (cosmeticWorldObject != null && (!SemiFunc.IsMultiplayer() || photonView == null || photonView.ViewID != 0))
                    yield break;

                yield return null;
            }
        }

        private static void RegisterSpawnedCosmeticWorldObject(CosmeticWorldObject cosmeticWorldObject, SemiFunc.Rarity rarity)
        {
            if (cosmeticWorldObject == null)
                return;

            int instanceId = cosmeticWorldObject.GetInstanceID();
            spawnedCosmeticWorldObjectInstanceIds.Add(instanceId);
            spawnedCosmeticWorldObjectRarities[instanceId] = rarity;

            PhotonView photonView = GetCosmeticWorldObjectPhotonView(cosmeticWorldObject);
            if (photonView != null && photonView.ViewID != 0)
            {
                spawnedCosmeticWorldObjectViewIds.Add(photonView.ViewID);
                spawnedCosmeticWorldObjectRarities[photonView.ViewID] = rarity;
            }
        }

        private static void SyncCosmeticWorldObjectTargetAmount()
        {
            ValuableDirector director = ValuableDirector.instance;
            if (director == null)
                return;

            int targetAmount = GetIntField(director, "cosmeticWorldObjectTargetAmount") + 1;
            SetFieldValue(director, "cosmeticWorldObjectTargetAmount", targetAmount);

            PhotonView photonView = GetFieldValue<PhotonView>(director, "PhotonView");
            if (!SemiFunc.IsMultiplayer() || !PhotonNetwork.IsMasterClient || photonView == null)
                return;

            photonView.RPC("CosmeticWorldObjectTargetSetRPC", RpcTarget.Others, targetAmount);
        }

        private static PhotonView GetCosmeticWorldObjectPhotonView(CosmeticWorldObject cosmeticWorldObject)
        {
            if (cosmeticWorldObject == null)
                return null;

            return cosmeticWorldObject.GetComponent<PhotonView>() ??
                   cosmeticWorldObject.GetComponentInParent<PhotonView>() ??
                   cosmeticWorldObject.GetComponentInChildren<PhotonView>(true);
        }

        private static void CleanupSpawnedCosmeticWorldObjectIds()
        {
            List<int> removeInstanceIds = null;
            foreach (int instanceId in spawnedCosmeticWorldObjectInstanceIds)
            {
                bool exists = false;
                foreach (CosmeticWorldObject cosmeticWorldObject in Object.FindObjectsOfType<CosmeticWorldObject>(true))
                {
                    if (cosmeticWorldObject != null && cosmeticWorldObject.GetInstanceID() == instanceId)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    removeInstanceIds ??= new List<int>();
                    removeInstanceIds.Add(instanceId);
                }
            }

            if (removeInstanceIds != null)
            {
                foreach (int instanceId in removeInstanceIds)
                {
                    spawnedCosmeticWorldObjectInstanceIds.Remove(instanceId);
                    spawnedCosmeticWorldObjectRarities.Remove(instanceId);
                }
            }

            List<int> removeViewIds = null;
            foreach (int viewId in spawnedCosmeticWorldObjectViewIds)
            {
                PhotonView photonView = PhotonView.Find(viewId);
                if (photonView != null)
                    continue;

                removeViewIds ??= new List<int>();
                removeViewIds.Add(viewId);
            }

            if (removeViewIds == null)
                return;

            foreach (int viewId in removeViewIds)
            {
                spawnedCosmeticWorldObjectViewIds.Remove(viewId);
                spawnedCosmeticWorldObjectRarities.Remove(viewId);
            }
        }

        private static int GetIntField(object instance, string fieldName)
        {
            if (instance == null)
                return 0;

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return field != null && field.GetValue(instance) is int value ? value : 0;
        }

        private static void SetFieldValue(object instance, string fieldName, object value)
        {
            if (instance == null)
                return;

            FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field?.SetValue(instance, value);
        }

        private static string CombineResourcePath(string resourcePath, string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
                return NormalizeResourcePath(resourcePath);

            if (string.IsNullOrWhiteSpace(resourcePath))
                return NormalizeResourcePath(prefabName);

            resourcePath = NormalizeResourcePath(resourcePath);
            prefabName = NormalizeResourcePath(prefabName);

            if (resourcePath == prefabName || resourcePath.EndsWith("/" + prefabName))
                return resourcePath;

            return resourcePath.EndsWith("/")
                ? resourcePath + prefabName
                : resourcePath + "/" + prefabName;
        }

        private static string FindResourcePathForLoadedPrefab(GameObject prefab, IEnumerable<string> folders)
        {
            if (prefab == null || folders == null)
                return null;

            foreach (string folder in folders)
            {
                string normalizedFolder = NormalizeResourcePath(folder);
                if (string.IsNullOrWhiteSpace(normalizedFolder))
                    continue;

                string candidate = NormalizeResourcePath(normalizedFolder + "/" + prefab.name);
                GameObject loaded = Resources.Load<GameObject>(candidate);
                if (loaded == prefab)
                    return candidate;
            }

            return null;
        }

        private static IEnumerable<string> GetValuableResourceFolders()
        {
            string root = NormalizeResourcePath(GetStringField(ValuableDirector.instance, "resourcePath")) ?? "Valuables";
            yield return root;

            foreach (string fieldName in new[]
                     {
                         "tinyPath",
                         "smallPath",
                         "mediumPath",
                         "bigPath",
                         "widePath",
                         "tallPath",
                         "veryTallPath"
                     })
            {
                string folder = GetStringField(ValuableDirector.instance, fieldName);
                if (!string.IsNullOrWhiteSpace(folder))
                    yield return CombineResourcePath(root, folder);
            }
        }

        private static IEnumerable<string> GetEnemyResourceFolders()
        {
            yield return "Enemies";

            foreach (EnemySetup setup in GetAllEnemySetups())
            {
                if (setup?.spawnObjects == null)
                    continue;

                foreach (PrefabRef prefabRef in setup.spawnObjects)
                {
                    string path = NormalizeResourcePath(prefabRef?.ResourcePath);
                    if (string.IsNullOrWhiteSpace(path))
                        continue;

                    int slash = path.LastIndexOf('/');
                    if (slash > 0)
                        yield return path.Substring(0, slash);
                }
            }
        }

        private static IEnumerable<EnemySetup> GetAllEnemySetups()
        {
            foreach (EnemySetup setup in EnemyDirector.instance?.enemiesDifficulty1 ?? Enumerable.Empty<EnemySetup>())
                yield return setup;

            foreach (EnemySetup setup in EnemyDirector.instance?.enemiesDifficulty2 ?? Enumerable.Empty<EnemySetup>())
                yield return setup;

            foreach (EnemySetup setup in EnemyDirector.instance?.enemiesDifficulty3 ?? Enumerable.Empty<EnemySetup>())
                yield return setup;

            foreach (EnemySetup setup in GetFieldValue<List<EnemySetup>>(EnemyDirector.instance, "enemyList") ?? Enumerable.Empty<EnemySetup>())
                yield return setup;
        }

        private static string NormalizeResourcePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            path = path.Replace('\\', '/').Trim();
            while (path.Contains("//"))
                path = path.Replace("//", "/");
            return path.Trim('/');
        }

        private static string CleanPrefabName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Unknown";

            return name
                .Replace("(Clone)", "")
                .Replace("Valuable", "")
                .Replace("Enemy -", "")
                .Replace("Enemy_", "")
                .Replace("Enemy", "")
                .Trim();
        }

        private static string FormatCosmeticRarity(SemiFunc.Rarity rarity)
        {
            return rarity == SemiFunc.Rarity.UltraRare ? "Ultra Rare" : rarity.ToString();
        }

        private static void GetSpawnPose(out Vector3 spawnPosition, out Quaternion spawnRotation)
        {
            Transform playerTransform = PlayerController.instance.playerAvatar.transform;
            Transform aimTransform = Camera.main != null ? Camera.main.transform : playerTransform;
            Vector3 forward = aimTransform.forward.sqrMagnitude > 0.001f ? aimTransform.forward.normalized : playerTransform.forward;
            spawnPosition = playerTransform.position + Vector3.up * 1f + forward * 2f;

            Vector3 flatForward = new Vector3(forward.x, 0f, forward.z);
            if (flatForward.sqrMagnitude < 0.001f)
                flatForward = playerTransform.forward;

            spawnRotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
        }

        private static IEnumerator ChargeBatteryWhenReady(GameObject spawned)
        {
            for (int i = 0; i < 40; i++)
            {
                if (spawned == null)
                    yield break;

                ItemBattery battery = spawned.GetComponent<ItemBattery>() ?? spawned.GetComponentInChildren<ItemBattery>();
                if (battery != null)
                {
                    battery.SetBatteryLife(100);
                    yield break;
                }

                yield return null;
            }
        }
    }
}
