﻿namespace GleyAllPlatformsSave
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;


    public class SaveSettingsWindow : EditorWindow
    {
        private const string FOLDER_NAME = "AllPlatformsSave";
        private const string PARENT_FOLDER = "GleyPlugins";
        private static string rootFolder;

        SaveSettings saveSettings;
        List<SupportedBuildTargetGroup> buildTargetGroup;
        List<SupportedSaveMethods> selectedSaveMethod;
        string message;
        // Get existing open window or if none, make a new one:
        [MenuItem("Window/Gley/All Platforms Save", false, 10)]
        static void Init()
        {
            if (!LoadRootFolder())
                return;

            string path = $"{rootFolder}/Scripts/Version.txt";
            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<Gley.Common.AssetVersion>(reader.ReadToEnd()).longVersion;

            SaveSettingsWindow window = (SaveSettingsWindow)GetWindow(typeof(SaveSettingsWindow));
            window.titleContent = new GUIContent("Save - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }

        static bool LoadRootFolder()
        {
            rootFolder = Gley.Common.EditorUtilities.FindFolder(FOLDER_NAME, PARENT_FOLDER);
            if (rootFolder == null)
            {
                Debug.LogError($"Folder Not Found:'{PARENT_FOLDER}/{FOLDER_NAME}'");
                return false;
            }
            return true;
        }


        private void OnEnable()
        {
            if (!LoadRootFolder())
                return;

            saveSettings = Resources.Load<SaveSettings>("SaveSettingsData");
            if (saveSettings == null)
            {
                CreateAdSettings();
                saveSettings = Resources.Load<SaveSettings>("SaveSettingsData");
            }
            selectedSaveMethod = saveSettings.saveMethod;
            buildTargetGroup = saveSettings.buildTargetGroup;
        }

        public static void CreateAdSettings()
        {
            SaveSettings asset = ScriptableObject.CreateInstance<SaveSettings>();
            Gley.Common.EditorUtilities.CreateFolder($"{rootFolder}/Resources");

            AssetDatabase.CreateAsset(asset, $"{rootFolder}/Resources/SaveSettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        void OnGUI()
        {
            GUILayout.Label("Configure your save plugin from here: ", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            for (int i = 0; i < buildTargetGroup.Count; i++)
            {
                buildTargetGroup[i] = (SupportedBuildTargetGroup)EditorGUILayout.EnumPopup("Select your build target:", buildTargetGroup[i]);
                selectedSaveMethod[i] = (SupportedSaveMethods)EditorGUILayout.EnumPopup("Select save method:", selectedSaveMethod[i]);
                if (GUILayout.Button("Remove Build Target"))
                {
                    buildTargetGroup.RemoveAt(i);
                    selectedSaveMethod.RemoveAt(i);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            if (GUILayout.Button("Add Build Target"))
            {
                buildTargetGroup.Add(SupportedBuildTargetGroup.Android);
                selectedSaveMethod.Add(SupportedSaveMethods.JSONSerializationFileSave);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Save"))
            {
                for (int i = 0; i < buildTargetGroup.Count - 1; i++)
                {
                    for (int j = i + 1; j < buildTargetGroup.Count; j++)
                    {
                        if (buildTargetGroup[i] == buildTargetGroup[j])
                        {
                            message = "Platform " + buildTargetGroup[i] + " exists multiple times. \nRemove duplicate entries and save again";
                            return;
                        }
                    }
                }

                var buildTargetGroups = Enum.GetValues(typeof(SupportedBuildTargetGroup));
                var saveMethods = Enum.GetValues(typeof(SupportedSaveMethods));
                for (int i = 0; i < buildTargetGroups.Length; i++)
                {
                    for (int j = 0; j < saveMethods.Length; j++)
                    {
                        try
                        {
                            AddPreprocessorDirective(saveMethods.GetValue(j).ToString(), true, (BuildTargetGroup)buildTargetGroups.GetValue(i));
                        }
                        catch { }
                    }
                }

                for (int i = 0; i < buildTargetGroup.Count; i++)
                {
                    AddPreprocessorDirective(selectedSaveMethod[i].ToString(), false, (BuildTargetGroup)buildTargetGroup[i]);
                }

                SaveSettings();
                message = "Settings applied.";
            }
            GUILayout.Label(message, EditorStyles.boldLabel);

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Test Scene"))
            {
                EditorSceneManager.OpenScene($"{rootFolder}/Example/Scenes/SimpleSaveExample.unity");
            }
        }

        private void SaveSettings()
        {
            saveSettings.saveMethod = selectedSaveMethod;
            saveSettings.buildTargetGroup = buildTargetGroup;
            EditorUtility.SetDirty(saveSettings);
        }

        void AddPreprocessorDirective(string directive, bool remove, BuildTargetGroup target)
        {
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (remove)
            {
                if (textToWrite.Contains(directive))
                {
                    textToWrite = textToWrite.Replace(directive, "");
                }
            }
            else
            {
                Debug.Log(directive + " is set for " + target);
                if (!textToWrite.Contains(directive))
                {
                    if (textToWrite == "")
                    {
                        textToWrite += directive;
                    }
                    else
                    {
                        textToWrite += "," + directive;
                    }
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, textToWrite);
        }
    }
}
