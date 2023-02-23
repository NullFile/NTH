using UnityEngine;
using UnityEditor;

public class SwitchManagerEditor : EditorWindow
{
    private static string[] names;
    private static bool[] booleans;

    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("Altair/SwitchManager")]
    static void Init()
    {
        SwitchManagerEditor window = (SwitchManagerEditor)EditorWindow.GetWindow(typeof(SwitchManagerEditor));

        int length = SwitchManager.switchName.Length;
        names = new string[length];
        booleans = new bool[length];

        for (int i = 0; i < length; i++)
        {
            names[i] = SwitchManager.switchName[i];
            booleans[i] = SwitchManager.globalSwitch[SwitchManager.switchName[i]];
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        int length = SwitchManager.switchName.Length;
        for (int i = 0; i < length; i++)
        {
            int temp = i;
            booleans[temp] = EditorGUILayout.Toggle(names[i], booleans[temp]);
            if ((i + 1) % 20 == 0) if (GUILayout.Button("Ok")) for (int j = 0; j < length; j++) SwitchManager.globalSwitch[SwitchManager.switchName[j]] = booleans[j];
        }

        EditorGUILayout.EndScrollView();
    }

}