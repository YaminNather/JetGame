using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Hierarchy2
{
    [CustomEditor(typeof(HierarchyFolder)), CanEditMultipleObjects]
    internal class HierarchyFolderEditor : Editor
    {
        private void OnEnable()
        {

        }

        public override VisualElement CreateInspectorGUI()
        {
            var script = target as HierarchyFolder;

            var root = new VisualElement();

            IMGUIContainer imguiContainer = new IMGUIContainer(() =>
            {
                script.flattenMode = (HierarchyFolder.FlattenMode) EditorGUILayout.EnumPopup("Flatten Mode", script.flattenMode);
                if (script.flattenMode != HierarchyFolder.FlattenMode.None)
                {
                    script.flattenSpace = (HierarchyFolder.FlattenSpace) EditorGUILayout.EnumPopup("Flatten Space", script.flattenSpace);
                    script.destroyAfterFlatten = EditorGUILayout.Toggle("Destroy After Flatten", script.destroyAfterFlatten);
                }
            });

            //root.Add(imguiContainer);
            
            PropertyField flattenModePropertyField = new PropertyField();
            flattenModePropertyField.BindProperty(serializedObject.FindProperty("flattenMode"));
            root.Add(flattenModePropertyField);

            VisualElement flattenOptionSectionVE = new VisualElement();
            
            PropertyField propertyField0 = new PropertyField();
            propertyField0.BindProperty(serializedObject.FindProperty("flattenSpace"));
            flattenOptionSectionVE.Add(propertyField0);
            
            propertyField0 = new PropertyField();
            propertyField0.BindProperty(serializedObject.FindProperty("destroyAfterFlatten"));
            flattenOptionSectionVE.Add(propertyField0);

            flattenOptionSectionVE.visible = !serializedObject.FindProperty("flattenMode").hasMultipleDifferentValues 
                                             && ((HierarchyFolder.FlattenMode)serializedObject.FindProperty("flattenMode").enumValueIndex != HierarchyFolder.FlattenMode.None);
            root.Add(flattenOptionSectionVE);

            flattenModePropertyField.RegisterValueChangeCallback(x =>
            {
                flattenOptionSectionVE.visible = (HierarchyFolder.FlattenMode)x.changedProperty.enumValueIndex != HierarchyFolder.FlattenMode.None;
            });

            return root;
        }

        [MenuItem("GameObject/Hierarchy Folder", priority = 0)]
        static void CreateInstance() => new GameObject("Folder", new Type[1] {typeof(HierarchyFolder)});
    }
}