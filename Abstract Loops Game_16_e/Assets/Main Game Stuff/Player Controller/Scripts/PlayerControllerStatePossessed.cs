using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerStatePossessed : PlayerControllerState
{
    public override void OnEnter_F(JetPlayerController playerController, PlayerControllerState stateFrom)
    {
        base.OnEnter_F(playerController, stateFrom);
        
    }

    public override void OnUpdate_F()
    {
        
    }

    public override void OnExit_F(PlayerControllerState stateTo)
    {
        base.OnExit_F(stateTo);
    }
}
