using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace SpawnLevelPrefabsTool
{
    [System.Serializable]
    public class BlockSpawnEditorWindow : EditorWindow
    {
        #region Variables
        static private BlockSpawnEditorWindow s_Window;

        private Transform m_Context;
        private SerializedObject m_SO;
        private string m_BlocksPath;
        private GameObject m_SelectedPrefab;
        private LinkedList<GameObject> m_Ghosts;
        private LinkedList<Transform> m_GhostsRotationObjsTranss;

        private IntegerField m_SpawnCountIntegerField;

        [SerializeField] private bool m_DistUseContext;
        [SerializeField] private float m_InitialDist;
        [SerializeField] private float m_RelativeDist = 0;

        [SerializeField] private bool m_RotUseContext;
        [SerializeField] private float m_InitialRot;
        [SerializeField] private float m_RelativeRot;
        #endregion

        [MenuItem("Spawn Level Prefab Tool/WIP Window For Testing")]
        private static void OpenWindow_F()
        {
            s_Window?.Close();
            s_Window = CreateWindow<BlockSpawnEditorWindow>("Spawn Level Prefab Tool");
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable called");

            if (PrefabStageUtility.GetCurrentPrefabStage() == null) return;
            Debug.Log($"Open Prefab = {PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.transform.name}");

            m_BlocksPath = ToolSettingsProvider.GetOrCreateSettingsFile_F().BlocksPath;
            m_RelativeDist = 10;
            m_RelativeRot = 45;
            m_Ghosts = new LinkedList<GameObject>();
            m_GhostsRotationObjsTranss = new LinkedList<Transform>();
            
            //Make Current Selection as context.
            if (Selection.activeTransform) m_Context = Selection.activeTransform;
            
            //Create the UI.
            //s_Window.TestUICreate_F();
            UICreate_F();
            //
        }

        private void TestUICreate_F()
        {
            rootVisualElement.Add(new Label($"m_BlocksPath = {m_BlocksPath}"));
            
            string[] paths = AssetDatabase.GetAllAssetPaths().Where(x => x.StartsWith(m_BlocksPath)).Where(x => x.EndsWith(".prefab")).Where(x => DirectoryPartGet_F(x, 2) == "Double Sided Rotating Fans").ToArray();
                        
            foreach (string path in paths) rootVisualElement.Add(new Label()
            {
                text = $"{DirectoryPartGet_F(path, 2)} - {path}",
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            });
        }

        private void UICreate_F()
        {
            m_SO = new SerializedObject(this);

            //Instantiating the UXML file and adding stylesheet.
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Spawn Level Prefab Tool/Editor/Editor Window/UXML/ToolEditorWindow_UXML.uxml");
            StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Spawn Level Prefab Tool/Editor/Editor Window/UXML/ToolEditorWindow_USS.uss");
            rootVisualElement.Add(uxml.Instantiate());
            rootVisualElement.styleSheets.Add(uss);

            //Creating the Section where you select blocks.
            BlockTypeSectionsCreate_F();

            //Setting up SpawnCount IntegerField.
            m_SpawnCountIntegerField = rootVisualElement.Q<IntegerField>("SpawnCount_IntegerField");
            m_SpawnCountIntegerField.SetValueWithoutNotify(1);
            m_SpawnCountIntegerField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue <= 0) m_SpawnCountIntegerField.SetValueWithoutNotify(1);                
                if(m_SelectedPrefab != null) GhostsCreate_F();
            });

            //Setting up InitialDist FloatField.
            FloatField initialDistFloatField = rootVisualElement.Q<FloatField>("InitialDist_FloatField");
            UIToolkitUtilities.DestroyableUITextValueFieldConvertTo_F<FloatField, float>(initialDistFloatField, this, () => m_InitialDist, val => m_InitialDist = val);
            
            //Setting up RelativeDist FloatField.
            FloatField relativeDistFloatField = rootVisualElement.Q<FloatField>("RelativeDist_FloatField");
            UIToolkitUtilities.DestroyableUITextValueFieldConvertTo_F<FloatField, float>(relativeDistFloatField, this, () => m_RelativeDist, val => m_RelativeDist = val);

            //Setting up DistUseContext FloatField.
            Toggle distUseContext_Toggle = rootVisualElement.Q<Toggle>("DistUseContext_Toggle");
            UIToolkitUtilities.DestroyableUIToggleConvertTo_F(distUseContext_Toggle, this, () => m_DistUseContext, val => m_DistUseContext = val);

            //Setting up InitialRot FloatField.
            FloatField initialRotFloatField = rootVisualElement.Q<FloatField>("InitialRotation_FloatField");
            UIToolkitUtilities.DestroyableUITextValueFieldConvertTo_F<FloatField, float>(initialRotFloatField, this, () => m_InitialRot, val => m_InitialRot = val);

            //Setting up RelativeRot FloatField.
            FloatField relativeRotFloatField = rootVisualElement.Q<FloatField>("RelativeRotation_FloatField");
            UIToolkitUtilities.DestroyableUITextValueFieldConvertTo_F<FloatField, float>(relativeRotFloatField, this, () => m_RelativeRot, val => m_RelativeRot = val);

            //Setting up DistUseContext FloatField.
            Toggle rotUseContext_Toggle = rootVisualElement.Q<Toggle>("RotationUseContext_Toggle");
            UIToolkitUtilities.DestroyableUIToggleConvertTo_F(rotUseContext_Toggle, this, () => m_RotUseContext, val => m_RotUseContext = val);

            //Making the ghost update every millisecond.
            rootVisualElement.schedule.Execute(GhostsTranssRefresh_F).Every(1);
            
            //Setting up Spawn Button. 
            Button Spawn_Btn = rootVisualElement.Q<Button>("Spawn_Btn");
            Spawn_Btn.clicked += SpawnBtnOnClick_BEF;
        }

        private void BlockTypeSectionsCreate_F()
        {          
            VisualElement blocksSectionVE = rootVisualElement.Q("BlockTypesSection_VE");
            
            //Finding all the prefab paths for prefabs under the blocks folder.
            string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(x => x.StartsWith(m_BlocksPath) && x.EndsWith(".prefab"))
                .Where(x => DirectoryPartGet_F(x).Contains("Base") == false).ToArray();
            
            //Creating buttons for each prefab and assigning the buttons to the corresponding foldout.
            Dictionary<string, Foldout> foldoutsDict = new Dictionary<string, Foldout>();
            foreach(string path in prefabPaths)
            {
                //Creating button.
                Button button_0 = new Button();
                button_0.text = DirectoryPartGet_F(path);
                button_0.userData = path;
                button_0.clicked += () => SelectedPrefabSet_F(button_0.userData as string);

                //Creating new foldout if foldout doesnt exist for that type.
                string type = PrefabTypeGet_F(path);
                if (!foldoutsDict.ContainsKey(type))                
                {
                    Foldout foldout_0 = new Foldout()
                    {
                        name = type + "sSection_Foldout",
                        text = type.ToUpper(),
                        value = false
                    };
                    foldout_0.AddToClassList("BlockTypeSection");
                    blocksSectionVE.Add(foldout_0);
                    foldoutsDict.Add(type, foldout_0);
                }                

                //Making button child of foldout.
                foldoutsDict[type].Add(button_0);
            }            
        }
        
        private string PrefabTypeGet_F(string path)
        {
            return path.Replace($"{m_BlocksPath}/", "").Replace($"/{DirectoryPartGet_F(path)}", "").Replace("/Prefabs", "").Replace("/", " ");
        }

        private void SelectedPrefabSet_F(string path)
        {
            //First destroy old ghosts.
            GhostsDestroy_F();

            //Loading in the SelectedPrefab and creating the ghosts if not null.
            m_SelectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GhostsCreate_F();
        }

        private void GhostsCreate_F()
        {
            //First destroy all existing ghosts before making new ones.
            GhostsDestroy_F();

            //Spawn SpawnCountFloatField.value amount of ghosts.
            for (int i = 0; i < m_SpawnCountIntegerField.value; i++) 
            {
                m_Ghosts.AddLast(PrefabUtility.InstantiatePrefab(m_SelectedPrefab, PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.transform) as GameObject); 
                m_Ghosts.Last.Value.transform.position = Vector3.zero;
            }

            //Check if the selected prefab has blockEditorMgrs with RotationObjTrans, if it does store it.
            if (m_SelectedPrefab.TryGetComponent(out BlockEditorMgr blockEditorMgr) && blockEditorMgr.RotationObjTrans != null)
            {
                foreach(GameObject ghost in m_Ghosts) m_GhostsRotationObjsTranss.AddLast(ghost.GetComponent<BlockEditorMgr>().RotationObjTrans);                
            }
            Debug.Log($"<color=cyan>m_GhostsRotationObjsTranss.Count = {m_GhostsRotationObjsTranss.Count}</color>");
            
            //Remove All components from ghost except for transforms, meshRenderers and meshFilters.
            foreach(GameObject ghost in m_Ghosts) GhostComponentsRemoveAll_F(ghost);

            ////Refresh the transforms of the ghosts.
            //GhostsTranssRefresh_F();
        }

        private void GhostComponentsRemoveAll_F(GameObject ghost)
        {
            foreach (Component c in ghost.GetComponentsInChildren<Component>())
            {
                Type type = c.GetType();
                if (type == typeof(Transform) || type == typeof(MeshFilter) || type == typeof(MeshRenderer))
                    continue;

                //c.ComponentsRequiringThisRemove_F();
                DestroyImmediate(c);
            }
        }

        private void GhostsTranssRefresh_F()
        {
            int ghostsCount = m_Ghosts.Count;
            if (ghostsCount == 0)
                return;

            //Setting position.
            m_Ghosts.First.Value.transform.position = new Vector3(0f, 0f, (m_DistUseContext == true && m_Context != null) ? m_Context.position.z + m_InitialDist : m_InitialDist);            
            for(LinkedListNode<GameObject> node = m_Ghosts.First.Next; node != null; node = node.Next)
                node.Value.transform.position = node.Previous.Value.transform.position + new Vector3(0f, 0f, m_RelativeDist);
            
            //Setting rotation.
            if (m_GhostsRotationObjsTranss.Count != 0)
            {
                m_GhostsRotationObjsTranss.First.Value.eulerAngles = new Vector3(0f, 0f, 
                    (m_RotUseContext && m_Context != null 
                    && m_Context.TryGetComponent(out BlockEditorMgr blockEditorMgr)
                    && blockEditorMgr.RotationObjTrans != null) ? blockEditorMgr.RotationObjTrans.eulerAngles.z + m_InitialRot : m_InitialRot);

                for(LinkedListNode<Transform> node = m_GhostsRotationObjsTranss.First.Next; node != null; node = node.Next)
                    node.Value.eulerAngles = node.Previous.Value.eulerAngles + new Vector3(0f, 0f, m_RelativeRot);
            }
        }

        private void GhostsDestroy_F()
        {
            if (m_Ghosts.Count != 0)
                foreach (GameObject ghost in m_Ghosts) DestroyImmediate(ghost);
            m_Ghosts.Clear();
            m_GhostsRotationObjsTranss.Clear();
        }

        private void SpawnBtnOnClick_BEF()
        {
            if (m_SelectedPrefab == null || m_Ghosts.Count == 0)
            {
                Debug.LogError("No Object Selected");
                return;
            }
            ActualGObjsInstantiate_F();
            Close();
        }

        private void ActualGObjsInstantiate_F()
        {
            //Instantiate the prefab, set the position and rotation and register it in Undo.
            GameObject spawnedGObj = null;
            for (int i = 0; i < m_SpawnCountIntegerField.value; i++)
            {
                spawnedGObj = PrefabUtility.InstantiatePrefab(m_SelectedPrefab, PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.transform) as GameObject;
                spawnedGObj.transform.position = m_Ghosts.ElementAt(i).transform.position;
                if (m_GhostsRotationObjsTranss.Count != 0)
                {
                    spawnedGObj.GetComponent<BlockEditorMgr>().RotationObjTrans.rotation = m_GhostsRotationObjsTranss.ElementAt(i).transform.rotation;
                }                
                Undo.RegisterCreatedObjectUndo(spawnedGObj, "Created a new Block");
            }
            
            //Set the last spawned object as the selected object.
            Selection.activeTransform = spawnedGObj.transform;
        }

        //private string[] PrefabsPathsOfTypeGet_F(string type)
        //{
        //    string path = $"{m_BlocksPath}/{type}";
        //    if (Directory.Exists(path) == false)
        //    {
        //        Debug.LogError($"Directory {path} does not exist!!");
        //        return null;
        //    }

        //    string[] filePaths = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        //    if (filePaths.Length == 0)
        //    {
        //        Debug.LogError($"filePaths.Length == 0");
        //        return null;
        //    }

        //    for (int i = 0; i < filePaths.Length; i++)
        //    {
        //        filePaths[i] = filePaths[i].Replace(Application.dataPath, "");
        //        filePaths[i] = $"Assets{filePaths[i]}";
        //        filePaths[i] = filePaths[i].Replace(@"\", "/");
        //    }

        //    string str_0 = $"<color=cyan>Prefabs found:";
        //    foreach (string filePath in filePaths) str_0 += "\n" + filePath;
        //    str_0 += "\n</color>";
        //    Debug.Log(str_0);            

        //    return filePaths;
        //}

        private string[] PrefabPathsOfTypeGet_F(string type)
        {
            return AssetDatabase.GetAllAssetPaths().Where(x => x.StartsWith(m_BlocksPath) && x.EndsWith(".prefab"))
                .Where(x => DirectoryPartGet_F(x, 2).Equals(type, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        private string DirectoryPartGet_F(string path, int upCount = 0)
        {
            string r = "";
            string[] strs_0 = path.Split('/');
            r = strs_0[strs_0.Length - (1 + upCount)];
            //Debug.Log($"Directory Part = {r} calculated from path {path} with upCount = {upCount}");
            return r;
        }

        private void OnDestroy()
        {
            Debug.Log("On Destroy Called");
        }       

        private void OnDisable()
        {
            Debug.Log("OnDisable called");

            Reset_F();

            Undo.ClearUndo(this);
        }

        private void Reset_F()
        {            
            GhostsDestroy_F();
            m_SelectedPrefab = null;
            m_RelativeDist = 10f;
            m_RelativeRot = 45f;
        }
    }

    public class UIToolkitUtilities
    {
        public static T DestroyableUITextValueFieldCreate<T, G>(UnityEngine.Object editorWindow, System.Func<G> getter, System.Action<G> setter, string label, int updateTime = 1) where T : TextValueField<G>
        {
            T r;
            if (typeof(T) == typeof(FloatField)) r = (dynamic)new FloatField();
            else if (typeof(T) == typeof(DoubleField)) r = (dynamic)new DoubleField();
            else if (typeof(T) == typeof(IntegerField)) r = (dynamic)new IntegerField();
            else if (typeof(T) == typeof(LongField)) r = (dynamic)new LongField();
            else throw new Exception("Wrong Type here!!!");

            r.label = label;
            DestroyableUITextValueFieldConvertTo_F<T, G>(r, editorWindow, getter, setter);

            return r;
        }

        public static T DestroyableUITextValueFieldConvertTo_F<T, G>(T textValueField, UnityEngine.Object editorWindow, System.Func<G> getter, System.Action<G> setter, int updateTime = 1) where T : TextValueField<G>
        {
            textValueField.schedule.Execute(() => textValueField.value = getter.Invoke()).Every(updateTime);
            textValueField.RegisterValueChangedCallback(x =>
            {
                Undo.RegisterCompleteObjectUndo(editorWindow, $"TextValueField {textValueField.label} value changed");
                setter(x.newValue);
            });

            return textValueField;
        }

        public static Toggle DestroyableUIToggleConvertTo_F(Toggle toggle, UnityEngine.Object undoObject, System.Func<bool> getter, System.Action<bool> setter, int updateTime = 1)
        {
            toggle.schedule.Execute(() => toggle.value = getter.Invoke()).Every(updateTime);
            toggle.RegisterValueChangedCallback(x =>
            {
                Undo.RegisterCompleteObjectUndo(undoObject, $"Toggle {toggle.label} value changed");
                setter(x.newValue);
            });

            return toggle;
        }
    }
}