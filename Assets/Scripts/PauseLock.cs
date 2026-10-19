using System.Collections.Generic;
using UnityEngine;

public static class PauseLock
{
    private static readonly HashSet<string> locks = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        locks.Clear();
    }

    public static void Set(string key, bool active)
    {
        if (active == locks.Contains(key))
            return;

        if (active)
            locks.Add(key);
        else
            locks.Remove(key);

        Time.timeScale = locks.Count > 0 ? 0f : 1f;
    }

    public static bool AnyLocked
    {
        get { return locks.Count > 0; }
    }

    public static bool IsLocked(string key)
    {
        return locks.Contains(key);
    }

    public static void Clear()
    {
        locks.Clear();
        Time.timeScale = 1f;
    }
}
