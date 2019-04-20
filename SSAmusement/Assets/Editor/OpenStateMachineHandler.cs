using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class OpenStateMachineHandler : MonoBehaviour {

    [OnOpenAsset(1)]
    public static bool OpenIfStateMachine(int instanceID, int line) {
        StateMachine machine = EditorUtility.InstanceIDToObject(instanceID) as StateMachine;
        if (machine == null) {
            return false;
        }
        StateMachineEditorWindow.ShowWindow(machine);
        return true;
    }
}
