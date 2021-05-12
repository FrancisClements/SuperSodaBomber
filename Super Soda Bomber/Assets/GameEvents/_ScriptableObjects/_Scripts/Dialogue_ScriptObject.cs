using UnityEngine;
using UnityEditor;
using System;

/*
Dialogue_ScriptObject
    A scriptable object that holds the initial data for the dialogue:
        - Character Name
        - Character Image
        - Character's Dialogue
*/
[CreateAssetMenu(fileName = "New Dialogue Data", menuName = "ScriptableObjects/Dialogue")]
public class Dialogue_ScriptObject : ScriptableObject
{
    public string characterName;
    public Sprite characterImage;
    public string dialogue;

    #if UNITY_EDITOR
    void OnEnable() => EditorUtility.SetDirty(this);
    #endif
}

#if UNITY_EDITOR
//Custom Editor for the Dialogue
[CustomEditor(typeof(Dialogue_ScriptObject))]
public class Dialogue_ScriptObject_Editor: Editor{

    private string lineWarning = "The {0} might not fit at the dialogue box. Try splitting it by segments, shorten it or create another Dialogue Data.";

    public override void OnInspectorGUI()
    {
        var s = target as Dialogue_ScriptObject;

        //Character Info
        EditorGUILayout.LabelField("Character Info", EditorStyles.boldLabel);
        s.characterName = EditorGUILayout.TextField("Name", s.characterName);

        //warns the programmer if the name exceeds 20 characters
        if (s.characterName.Length >= 20){
            EditorGUILayout.HelpBox(String.Format(lineWarning, "character name"), MessageType.Warning);
        }

        s.characterImage = (Sprite)EditorGUILayout.ObjectField("Image", s.characterImage, typeof(Sprite), false);
        
        //Dialogue
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dialogue", EditorStyles.label);
        s.dialogue = EditorGUILayout.TextArea(s.dialogue, GUILayout.MinHeight(80));

        //warns the programmer if the dialogue exceeds 20 characters
        if (s.dialogue.Length >= 75){
            EditorGUILayout.HelpBox(String.Format(lineWarning, "dialogue"), MessageType.Warning);
        }
    }
}
#endif