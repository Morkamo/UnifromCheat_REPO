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
                    { "mapChangeStarted", "Changing map to {0}." },
                    { "setLevelActionName", "Set Level" },
                    { "setMoneyActionName", "Set Money" },
                    { "addMoneyActionName", "Add Money" },
                    { "freezeActionName", "Freeze" },
                    { "upgradesActionName", "Upgrades" },
                    { "levelValue", "level" },
                    { "moneyValue", "money" },
                    { "upgradeValue", "upgrade level" },
                    { "enterNumber", "Enter a {0} number." },
                    { "invalidNumber", "Invalid {0} number." },
                    { "numberOutOfRange", "{0} is out of range. Allowed: {1}-{2}." },
                    { "moneyTotalOutOfRange", "Money total is out of range. Allowed: 0-{0}." },
                    { "setLevelConfirmTitle", "Set level?" },
                    { "setLevelConfirmBody", "Set current level to {0} and restart current map?" },
                    { "setMoneyConfirmTitle", "Set money?" },
                    { "setMoneyConfirmBody", "Set session money to {0}?" },
                    { "addMoneyConfirmTitle", "Add money?" },
                    { "addMoneyConfirmBody", "Add {0} money? New total: {1}." },
                    { "levelSet", "Level set to {0}." },
                    { "levelSetFailed", "Failed to set level." },
                    { "moneySet", "Money set to {0}." },
                    { "moneyUpdated", "Money updated to {0}." },
                    { "moneyUpdateFailed", "Failed to update money." },
                    { "playerFrozen", "Player frozen." },
                    { "playerUnfrozen", "Player unfrozen." },
                    { "setUpgradeConfirmTitle", "Set upgrade?" },
                    { "setUpgradeConfirmBody", "Set {0} upgrade level to {1}?" },
                    { "addUpgradeConfirmTitle", "Add upgrade?" },
                    { "addUpgradeConfirmBody", "Add {0} to {1}? New level: {2}." },
                    { "upgradeSet", "{0} upgrade level is now {1}." }
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
                    { "mapChangeStarted", "Смена карты на {0}." },
                    { "setLevelActionName", "Set Level" },
                    { "setMoneyActionName", "Set Money" },
                    { "addMoneyActionName", "Add Money" },
                    { "freezeActionName", "Freeze" },
                    { "upgradesActionName", "Upgrades" },
                    { "levelValue", "уровня" },
                    { "moneyValue", "денег" },
                    { "upgradeValue", "уровня улучшения" },
                    { "enterNumber", "Введите число для {0}." },
                    { "invalidNumber", "Некорректное число для {0}." },
                    { "numberOutOfRange", "{0} вне диапазона. Допустимо: {1}-{2}." },
                    { "moneyTotalOutOfRange", "Итоговая сумма денег вне диапазона. Допустимо: 0-{0}." },
                    { "setLevelConfirmTitle", "Установить уровень?" },
                    { "setLevelConfirmBody", "Установить текущий уровень на {0} и перезапустить карту?" },
                    { "setMoneyConfirmTitle", "Установить деньги?" },
                    { "setMoneyConfirmBody", "Установить деньги сессии на {0}?" },
                    { "addMoneyConfirmTitle", "Добавить деньги?" },
                    { "addMoneyConfirmBody", "Добавить {0} денег? Новый итог: {1}." },
                    { "levelSet", "Уровень установлен на {0}." },
                    { "levelSetFailed", "Не удалось установить уровень." },
                    { "moneySet", "Деньги установлены на {0}." },
                    { "moneyUpdated", "Деньги обновлены до {0}." },
                    { "moneyUpdateFailed", "Не удалось обновить деньги." },
                    { "playerFrozen", "Игрок заморожен." },
                    { "playerUnfrozen", "Игрок разморожен." },
                    { "setUpgradeConfirmTitle", "Установить улучшение?" },
                    { "setUpgradeConfirmBody", "Установить уровень улучшения {0} на {1}?" },
                    { "addUpgradeConfirmTitle", "Добавить улучшение?" },
                    { "addUpgradeConfirmBody", "Добавить {0} к {1}? Новый уровень: {2}." },
                    { "upgradeSet", "Уровень улучшения {0}: {1}." }
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
                    { "mapChangeStarted", "Зміна карти на {0}." },
                    { "setLevelActionName", "Set Level" },
                    { "setMoneyActionName", "Set Money" },
                    { "addMoneyActionName", "Add Money" },
                    { "freezeActionName", "Freeze" },
                    { "upgradesActionName", "Upgrades" },
                    { "levelValue", "рівня" },
                    { "moneyValue", "грошей" },
                    { "upgradeValue", "рівня покращення" },
                    { "enterNumber", "Введіть число для {0}." },
                    { "invalidNumber", "Некоректне число для {0}." },
                    { "numberOutOfRange", "{0} поза діапазоном. Дозволено: {1}-{2}." },
                    { "moneyTotalOutOfRange", "Підсумкова сума грошей поза діапазоном. Дозволено: 0-{0}." },
                    { "setLevelConfirmTitle", "Встановити рівень?" },
                    { "setLevelConfirmBody", "Встановити поточний рівень на {0} і перезапустити карту?" },
                    { "setMoneyConfirmTitle", "Встановити гроші?" },
                    { "setMoneyConfirmBody", "Встановити гроші сесії на {0}?" },
                    { "addMoneyConfirmTitle", "Додати гроші?" },
                    { "addMoneyConfirmBody", "Додати {0} грошей? Новий підсумок: {1}." },
                    { "levelSet", "Рівень встановлено на {0}." },
                    { "levelSetFailed", "Не вдалося встановити рівень." },
                    { "moneySet", "Гроші встановлено на {0}." },
                    { "moneyUpdated", "Гроші оновлено до {0}." },
                    { "moneyUpdateFailed", "Не вдалося оновити гроші." },
                    { "playerFrozen", "Гравця заморожено." },
                    { "playerUnfrozen", "Гравця розморожено." },
                    { "setUpgradeConfirmTitle", "Встановити покращення?" },
                    { "setUpgradeConfirmBody", "Встановити рівень покращення {0} на {1}?" },
                    { "addUpgradeConfirmTitle", "Додати покращення?" },
                    { "addUpgradeConfirmBody", "Додати {0} до {1}? Новий рівень: {2}." },
                    { "upgradeSet", "Рівень покращення {0}: {1}." }
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
                    { "mapChangeStarted", "正在更改地图为 {0}。" },
                    { "setLevelActionName", "Set Level" },
                    { "setMoneyActionName", "Set Money" },
                    { "addMoneyActionName", "Add Money" },
                    { "freezeActionName", "Freeze" },
                    { "upgradesActionName", "Upgrades" },
                    { "levelValue", "等级" },
                    { "moneyValue", "金钱" },
                    { "upgradeValue", "升级等级" },
                    { "enterNumber", "请输入{0}数字。" },
                    { "invalidNumber", "{0}数字无效。" },
                    { "numberOutOfRange", "{0}超出范围。允许：{1}-{2}。" },
                    { "moneyTotalOutOfRange", "金钱总额超出范围。允许：0-{0}。" },
                    { "setLevelConfirmTitle", "设置等级？" },
                    { "setLevelConfirmBody", "将当前等级设置为 {0} 并重启当前地图？" },
                    { "setMoneyConfirmTitle", "设置金钱？" },
                    { "setMoneyConfirmBody", "将会话金钱设置为 {0}？" },
                    { "addMoneyConfirmTitle", "添加金钱？" },
                    { "addMoneyConfirmBody", "添加 {0} 金钱？新的总额：{1}。" },
                    { "levelSet", "等级已设置为 {0}。" },
                    { "levelSetFailed", "设置等级失败。" },
                    { "moneySet", "金钱已设置为 {0}。" },
                    { "moneyUpdated", "金钱已更新为 {0}。" },
                    { "moneyUpdateFailed", "更新金钱失败。" },
                    { "playerFrozen", "玩家已冻结。" },
                    { "playerUnfrozen", "玩家已解冻。" },
                    { "setUpgradeConfirmTitle", "设置升级？" },
                    { "setUpgradeConfirmBody", "将 {0} 升级等级设置为 {1}？" },
                    { "addUpgradeConfirmTitle", "添加升级？" },
                    { "addUpgradeConfirmBody", "给 {1} 添加 {0}？新等级：{2}。" },
                    { "upgradeSet", "{0} 升级等级现在是 {1}。" }
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
