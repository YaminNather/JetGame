using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace SpawnLevelPrefabsTool
{
    public class ToolSettingsProvider : SettingsProvider
    {
        #region Variables
        private SerializedObject m_ToolSettings;
        const string k_SettingsFilePath = "Assets/Spawn Level Prefab Tool/Editor/Settings/Settings.asset";
        #endregion

        public ToolSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        /// <summary>
        /// Getting or creating the a Settings File that is going to be saved in our project folder, so that settings are actually saved and persistent.
        /// </summary>
        /// <returns></returns>
        internal static ToolSettings GetOrCreateSettingsFile_F()
        {
            var settingsFile = AssetDatabase.LoadAssetAtPath<ToolSettings>(k_SettingsFilePath);

            if (settingsFile == null)
            {
                settingsFile = ToolSettings.NewToolSettingsCreate_F();
                AssetDatabase.CreateAsset(settingsFile, k_SettingsFilePath);
                AssetDatabase.SaveAssets();
            }
            return settingsFile;
        }

        /// <summary>
        /// Check if Settings File in available.
        /// </summary>
        /// <returns></returns>
        private static bool IsSettingsFileAvailable_F() => File.Exists(k_SettingsFilePath);

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_ToolSettings = new SerializedObject(GetOrCreateSettingsFile_F());
            rootElement.Add(new Label("Spawn Level Prefabs Tool Settings")
            {
                style = { fontSize = 20, unityFontStyleAndWeight = FontStyle.Bold, marginTop = 5, marginBottom = 20 }
            });

            PropertyField PropertyField_0 = new PropertyField();
            PropertyField_0.BindProperty(m_ToolSettings.FindProperty("m_BlocksPath"));
            rootElement.Add(PropertyField_0);
        }

        [SettingsProvider]
        private static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (GetOrCreateSettingsFile_F() != null)
            {
                var provider = new ToolSettingsProvider("Project/SpawnLevelPrefabTool", SettingsScope.Project);
                return provider;
            }

            return null;
        }
    }

    public class ToolSettings : ScriptableObject
    {
        [SerializeField] private string m_BlocksPath;
        public string BlocksPath => m_BlocksPath;

        static internal ToolSettings NewToolSettingsCreate_F()
        {
            ToolSettings r = CreateInstance<ToolSettings>();
            r.m_BlocksPath = "Assets/Main Game Stuff/Levels/Blocks";
            return r;
        }
    }
}
