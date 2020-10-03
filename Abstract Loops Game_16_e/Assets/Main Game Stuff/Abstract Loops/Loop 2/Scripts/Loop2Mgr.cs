using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop2Mgr : LoopMgrBase
{
    #region Variables
    [SerializeField] private Loop2PartMgr Part0;
    [SerializeField] private Loop2PartMgr Part1;
    #endregion

    protected override void Reset_F()
    {
        Part0.Reset_F();
        Part1.Reset_F();
    }
}
