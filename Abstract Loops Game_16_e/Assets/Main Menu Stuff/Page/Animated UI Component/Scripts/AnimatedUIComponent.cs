using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimatedUIComponent : OnSceneLoadedMonoBehaviour
{
    public int m_Order;

    public abstract void EntryInitialize_F();
    public abstract void Enter_F(float time);

    public abstract void ExitInitialize_F();
    public abstract void Exit_F(float time);   
}
