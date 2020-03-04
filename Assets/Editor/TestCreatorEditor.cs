using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestCreator))]
public class TestCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TestCreator testCreator = (TestCreator)target;
        
        if (GUILayout.Button("Generate Test"))
        {
            testCreator.CreateBuildings();
        }
    }
}
