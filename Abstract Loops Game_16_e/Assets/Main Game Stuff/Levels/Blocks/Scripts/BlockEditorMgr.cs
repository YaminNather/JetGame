#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways][AddComponentMenu("Block Stuff/BlockEditorMgr")][DisallowMultipleComponent]
public class BlockEditorMgr : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform m_RotationObjTrans;
    public Transform RotationObjTrans { get => m_RotationObjTrans; }
    #endregion

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (transform.position.x != 0f || transform.position.y != 0f)
        {
            transform.position = new Vector3(0f, 0f, transform.position.z);
            Undo.RecordObject(transform, $"Reset Position of block {transform.name } to zero in X and Y axis");
        }
    }
}

[CustomEditor(typeof(BlockEditorMgr)), CanEditMultipleObjects]
public class BlockEditorMgrEditor : Editor
{
    private new BlockEditorMgr target;

    public override VisualElement CreateInspectorGUI()
    {
        target = base.target as BlockEditorMgr;

        VisualElement r = new VisualElement();

        //Drawing default Inspector.
        r.Add(new IMGUIContainer(() => DrawDefaultInspector()));


        //Drawing Transform Component of the rotating part.
        if (serializedObject.isEditingMultipleObjects == false && target.RotationObjTrans != null)
        {
            Editor transformEditor = Editor.CreateEditor(target.RotationObjTrans);
            Foldout foldout = new Foldout() {text = "Rotating Objects Transform"};
            foldout.Add(new IMGUIContainer(() =>
            {
                transformEditor.OnInspectorGUI();
            }));
            r.Add(foldout);
        }

        //Getting all Rotation Components from children of target/targets.
        SerializedObject sObj = RotationComponentsSObjsGet_F();
        if (sObj != null)
        {
            //Drawing a field to edit their RotationDir.
            PropertyField rotationDirField = new PropertyField();
            rotationDirField.BindProperty(sObj.FindProperty("m_RotationDir"));
            r.Add(rotationDirField);

            //Drawing a field to edit their RotationSpeed.
            PropertyField rotationSpeedField = new PropertyField();
            rotationSpeedField.BindProperty(sObj.FindProperty("m_RotationSpeed"));
            r.Add(rotationSpeedField);

            //A Button to set all Prefab of the same difficulty level to the same rotation speed.
            if (serializedObject.isEditingMultipleObjects == false)
            {
                Button button0 = new Button(SameDifficultyPrefabsChangeSpeed_F)
                {
                    text = "Change all Prefab's speed"
                };
                r.Add(button0);
            }
        }

        return r;
    }

    private void SameDifficultyPrefabsChangeSpeed_F()
    {
        //Finding the difficulty of the currently selected object.
        string difficultyName = "";
        if (target.transform.name.ToLower().Contains("easy")) difficultyName = "easy";
        else if (target.transform.name.ToLower().Contains("normal")) difficultyName = "normal";
        else if (target.transform.name.ToLower().Contains("hard")) difficultyName = "hard";

        //Finding all the prefabs with the same difficulty.
        string[] paths = AssetDatabase.GetAllAssetPaths()
            .Where(x => x.EndsWith(".prefab") && x.StartsWith("Assets/Main Game Stuff/Levels/Blocks/Types/"))
            .Where(x => Path.GetFileNameWithoutExtension(x).ToLower().Contains(difficultyName)).ToArray();

        string msg = $"All paths of difficulty {difficultyName}:";
        foreach (string path in paths) msg += "\n" + path;
        Debug.Log(msg);

        //Setting prefabs rotation speed to the currently selected one.
        float speed = 0.0f;
        using (SerializedObject sObj = RotationComponentsSObjsGet_F()) speed = sObj.FindProperty("m_RotationSpeed").floatValue;
            foreach (string path in paths)
            {
                using (PrefabUtility.EditPrefabContentsScope scope = new PrefabUtility.EditPrefabContentsScope(path))
                {
                    using (SerializedObject sObj = new SerializedObject(scope.prefabContentsRoot.GetComponentInChildren<BlockComponentRotationBase>()))
                    {
                        sObj.FindProperty("m_RotationSpeed").floatValue = speed;
                        sObj.ApplyModifiedProperties();
                    }
                }
            }
    }

    private SerializedObject RotationComponentsSObjsGet_F()
    {
        BlockEditorMgr target = this.target as BlockEditorMgr;

        SerializedObject sObj = null;
        if (serializedObject.isEditingMultipleObjects == false)
        {
            if (target.GetComponentInChildren<BlockComponentRotationBase>())
                sObj = new SerializedObject(target.GetComponentInChildren<BlockComponentRotationBase>());
        }
        else
        {
            List<Object> objs = new List<Object>();
            for (int i = 0; i < targets.Length; i++)
            {
                BlockEditorMgr blockEditorMgr = targets[i] as BlockEditorMgr;
                if (blockEditorMgr.GetComponentInChildren<BlockComponentRotationBase>())
                    objs.Add(blockEditorMgr.GetComponentInChildren<BlockComponentRotationBase>());
            }

            if (objs.Count != 0)
                sObj = new SerializedObject(objs.ToArray());
        }

        return sObj;
    }
}
#endif