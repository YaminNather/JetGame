using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class ScoreAdderComponent : MonoBehaviour
{    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHitbox ph) == false) return;
        
        if (MainGameReferences.INSTANCE != null)
        {
            MainGameReferences.INSTANCE.scoreMgr.ScoreAdd_F(1);
            ph.player.AudioPlay_F(JetAudioMgr.AudioClipsEN.ZoomingPastBlock);
        }
    }    
}

#if UNITY_EDITOR
public partial class ScoreAdderComponent : MonoBehaviour
{
    [MenuItem("GameObject/Custom/Score Adder", priority = 10)]
    private static void AddToGameObjectMenu_F(MenuCommand menuCommand)
    {
        GameObject createdGObj = new GameObject("Score Adder", new System.Type[] { typeof(ScoreAdderComponent), typeof(BoxCollider)} );
        createdGObj.GetComponent<BoxCollider>().isTrigger = true;
        GameObjectUtility.SetParentAndAlign(createdGObj, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(createdGObj, "Created a Score Adder");
        Selection.activeGameObject = createdGObj;
    }
}
#endif
