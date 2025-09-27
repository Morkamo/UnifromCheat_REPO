using System.Collections.Generic;
using UnityEngine;

namespace UnifromCheat_REPO.Utils;

public static class GameObjectAnalyzer
{
    /// <summary>
    /// Анализирует объект, его родителей и детей, возвращает структуру GameObject -> список компонентов
    /// </summary>
    public static Dictionary<GameObject, List<Component>> Analyze(GameObject go, bool includeChildren = true, bool includeParents = true)
    {
        var result = new Dictionary<GameObject, List<Component>>();
        
        AddComponents(go, result);
        
        if (includeParents)
        {
            var parent = go.transform.parent;
            while (parent != null)
            {
                AddComponents(parent.gameObject, result);
                parent = parent.parent;
            }
        }
        
        if (includeChildren)
        {
            AnalyzeChildrenRecursive(go, result);
        }

        return result;
    }
    
    public static void AnalyzeObject(GameObject go, bool includeChildren = true, bool includeParents = true)
    {
        var dict = Analyze(go, includeChildren, includeParents);

        foreach (var kv in dict)
        {
            foreach (var comp in kv.Value)
            {
                if (comp == null) continue;
                FireboxConsole.FireLog($"[{kv.Key.name}] {comp.GetType()}");
            }
        }
    }

    private static void AddComponents(GameObject go, Dictionary<GameObject, List<Component>> dict)
    {
        if (!dict.ContainsKey(go))
            dict[go] = new List<Component>();

        dict[go].AddRange(go.GetComponents<Component>());
    }

    private static void AnalyzeChildrenRecursive(GameObject go, Dictionary<GameObject, List<Component>> dict)
    {
        foreach (Transform child in go.transform)
        {
            AddComponents(child.gameObject, dict);
            AnalyzeChildrenRecursive(child.gameObject, dict);
        }
    }
}