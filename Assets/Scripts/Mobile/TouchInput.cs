using UnityEngine;

public static class TouchInput
{
    public static Vector2 Move;

    private static readonly int Count = System.Enum.GetValues(typeof(GameAction)).Length;
    private static readonly bool[] flag = new bool[Count];
    private static readonly int[] setFrame = new int[Count];
    private static readonly int[] readFrame = new int[Count];

    static TouchInput()
    {
        for (int i = 0; i < Count; i++)
            readFrame[i] = -1;
    }

    public static void Press(GameAction action)
    {
        int i = (int)action;
        flag[i] = true;
        setFrame[i] = Time.frameCount;
        readFrame[i] = -1;
        GameInput.SetLastUsed(InputKind.Touch);
    }

    public static bool Pressed(GameAction action)
    {
        int i = (int)action;
        if (!flag[i])
            return false;

        if (readFrame[i] == -1)
            readFrame[i] = Time.frameCount;

        return readFrame[i] == Time.frameCount;
    }

    internal static void EndFrame()
    {
        for (int i = 0; i < Count; i++)
        {
            if (!flag[i])
                continue;

            bool consumed = readFrame[i] != -1;
            bool stale = Time.frameCount - setFrame[i] > 2; 
            if (consumed || stale)
            {
                flag[i] = false;
                readFrame[i] = -1;
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateRunner()
    {
        GameObject go = new GameObject("TouchInputRunner");
        go.hideFlags = HideFlags.HideInHierarchy;
        Object.DontDestroyOnLoad(go);
        go.AddComponent<TouchInputRunner>();
    }

    private class TouchInputRunner : MonoBehaviour
    {
        private void LateUpdate() => TouchInput.EndFrame();
    }
}