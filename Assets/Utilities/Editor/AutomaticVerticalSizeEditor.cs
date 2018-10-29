using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// This script must be located in a folder named Editor in order that it is being loaded by Unity
[CustomEditor(typeof(AutomaticVerticalSize))]
public class AutomaticVerticalSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("AutoSizeLayout"))
        {
            // target is the instance of AutomaticVerticalSize object 
            // that is why we also need to cast down cast it before we can access the AdjustSize function
            ((AutomaticVerticalSize)target).AdjustSize();
        }
    }
}
