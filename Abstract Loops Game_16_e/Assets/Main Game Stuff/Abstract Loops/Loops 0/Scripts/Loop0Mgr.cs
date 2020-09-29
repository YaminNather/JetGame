using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop0Mgr : LoopMgrBase
{
    #region Variables
    [SerializeField] private Material Skybox_Mat;

    private Transform Loop;
    private float Hue;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        Loop = transform.GetChild(0);
                
    }

    public override void OnSpawn_F()
    {
        Hue = Random.Range(0f, 1f);
        RenderSettings.skybox = Skybox_Mat;
    }

    private void Update()
    {
        Hue += 0.1f * Time.deltaTime;
        if (Hue >= 1f) Hue = 0f;

        //Skybox_Mat.SetColor("_BaseColor", Color.HSVToRGB(Hue, 1f, 0.2f));
        //Loop_Mat.SetColor("_BaseColor", Color.HSVToRGB(Hue, 1f, 1f));
        MainGameReferences.s_Instance.colorMgr.Hue0 = Hue;
        Loop.Rotate(0f, 0f, 90f * Time.deltaTime);
    }
}
