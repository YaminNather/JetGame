using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode][SaveDuringPlay][AddComponentMenu("")]
public class LockCameraAxisCME : CinemachineExtension
{    
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if(stage == CinemachineCore.Stage.Body)
        {
            state.RawPosition = state.RawPosition.With(x:0f, y:0f);
            state.RawOrientation = Quaternion.Euler(state.RawOrientation.eulerAngles.With(x:0f));
        }
    }
}
