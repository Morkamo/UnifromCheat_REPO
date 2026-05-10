using System.Collections.Generic;

namespace UnifromCheat_REPO.Utils
{
    internal static class LocalizedMessages
    {
        private static readonly Dictionary<int, Dictionary<string, string>> Messages = new()
        {
            {
                0, new Dictionary<string, string>
                {
                    { "hostOnlyUnavailable", "This function is available only to the lobby host." },
                    { "hostOnlyFunctionUnavailable", "{0} is available only to the lobby host." },
                    { "playableOnly", "This function is available only after joining a playable session." },
                    { "itemPrefabNotFound", "Item prefab not found." },
                    { "playerNotFound", "Player not found." },
                    { "objectSpawnerHostOnly", "Object Spawner is available only to the lobby host." },
                    { "spawnedItem", "Spawned {0}." },
                    { "spawnFailed", "Failed to spawn {0}." },
                    { "playerActionFailed", "Player action failed." },
                    { "mapChangeBlocked", "Map change is not available right now." },
                    { "mapChangeFailed", "Failed to change map." },
                    { "mapChangeStarted", "Changing map to {0}." }
                }
            },
            {
                1, new Dictionary<string, string>
                {
                    { "hostOnlyUnavailable", "Эта функция доступна только хосту лобби." },
                    { "hostOnlyFunctionUnavailable", "{0} доступна только хосту лобби." },
                    { "playableOnly", "Эта функция доступна только после входа в игровую сессию." },
                    { "itemPrefabNotFound", "Префаб предмета не найден." },
                    { "playerNotFound", "Игрок не найден." },
                    { "objectSpawnerHostOnly", "Object Spawner доступен только хосту лобби." },
                    { "spawnedItem", "Создан предмет: {0}." },
                    { "spawnFailed", "Не удалось создать предмет: {0}." },
                    { "playerActionFailed", "Не удалось выполнить действие с игроком." },
                    { "mapChangeBlocked", "Сейчас нельзя сменить карту." },
                    { "mapChangeFailed", "Не удалось сменить карту." },
                    { "mapChangeStarted", "Смена карты на {0}." }
                }
            },
            {
                2, new Dictionary<string, string>
                {
                    { "hostOnlyUnavailable", "Ця функція доступна лише хосту лобі." },
                    { "hostOnlyFunctionUnavailable", "{0} доступна лише хосту лобі." },
                    { "playableOnly", "Ця функція доступна лише після входу в ігрову сесію." },
                    { "itemPrefabNotFound", "Префаб предмета не знайдено." },
                    { "playerNotFound", "Гравця не знайдено." },
                    { "objectSpawnerHostOnly", "Object Spawner доступний лише хосту лобі." },
                    { "spawnedItem", "Створено предмет: {0}." },
                    { "spawnFailed", "Не вдалося створити предмет: {0}." },
                    { "playerActionFailed", "Не вдалося виконати дію з гравцем." },
                    { "mapChangeBlocked", "Зараз не можна змінити карту." },
                    { "mapChangeFailed", "Не вдалося змінити карту." },
                    { "mapChangeStarted", "Зміна карти на {0}." }
                }
            },
            {
                3, new Dictionary<string, string>
                {
                    { "hostOnlyUnavailable", "此功能仅大厅房主可用。" },
                    { "hostOnlyFunctionUnavailable", "{0} 仅大厅房主可用。" },
                    { "playableOnly", "此功能只能在进入可游玩会话后使用。" },
                    { "itemPrefabNotFound", "未找到物品预制体。" },
                    { "playerNotFound", "未找到玩家。" },
                    { "objectSpawnerHostOnly", "Object Spawner 仅大厅房主可用。" },
                    { "spawnedItem", "已生成 {0}。" },
                    { "spawnFailed", "无法生成 {0}。" },
                    { "playerActionFailed", "无法执行玩家操作。" },
                    { "mapChangeBlocked", "现在无法更改地图。" },
                    { "mapChangeFailed", "更改地图失败。" },
                    { "mapChangeStarted", "正在更改地图为 {0}。" }
                }
            }
        };

        public static string Get(string key)
        {
            if (Messages.TryGetValue(Core.lg_state, out var pack) && pack.TryGetValue(key, out string value))
                return value;

            return Messages[0].TryGetValue(key, out string fallback) ? fallback : key;
        }

        public static string Format(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }
    }
}
