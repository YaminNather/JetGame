using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Collider)), DisallowMultipleComponent]
public partial class ScoreAdder : MonoBehaviour
{   
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            if (MainGameReferences.s_Instance != null) MainGameReferences.s_Instance.scoreMgr.ScoreAdd_F(1);
        }
    }
}

#if UNITY_EDITOR
public partial class ScoreAdder : MonoBehaviour
{
    private void OnValidate()
    {
        if (Application.isPlaying == true)
            return;
        if (GetComponent<Collider>().isTrigger == false)
            GetComponent<Collider>().isTrigger = true;
    }

    [MenuItem("GameObject/Custom/Score Adder", priority = 10)]
    public static void ScoreAdderAddFromMenu_F(MenuCommand menuCommand)
    {
        GameObject createdGObj = new GameObject("Score Adder", new System.Type[] { typeof(BoxCollider), typeof(ScoreAdder) });
        createdGObj.GetComponent<BoxCollider>().isTrigger = true;
        GameObjectUtility.SetParentAndAlign(createdGObj, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(createdGObj, "Created a Score Adder");
        Selection.activeGameObject = createdGObj;
    }
}
#endif
