using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

public class SelectScene : MonoBehaviour
{
    [MenuItem("Open Scene/Movement Test")]
    public static void OpenWalkTest()
    {
        OpenScene("movementTest");
    }

    [MenuItem("Open Scene/Training Arena")]
    public static void OpenTraining()
    {
        OpenScene("TrainingArena");
    }

    [MenuItem("Open Scene/Pitch Color Light")]
    public static void OpenPitchColor()
    {
        OpenScene("dBPitchColorLight");
    }

    [MenuItem("Open Scene/Record And Playback")]
    public static void OpenRecordAndPlayback()
    {
        OpenScene("recordAndPlayback");
    }
    static void OpenScene(string name)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
    }	
}

