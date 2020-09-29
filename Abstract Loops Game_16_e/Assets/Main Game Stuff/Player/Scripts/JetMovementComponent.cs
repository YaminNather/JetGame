using Ludiq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class JetMovementComponent : MonoBehaviour
{
    #region Variables
    [Header("Input")]
    private Vector3 m_InputVector;

    [Header("Movement Info")]    
    [SerializeField]  private float m_Accel = 90f;
    public float Accel { get => m_Accel; }
    [SerializeField]  private float m_Deccel = 20f;
    public float Deccel { get => m_Deccel; }
    [SerializeField]  private Vector3 m_VelocityMax = new Vector3(10, 10, 30f);
    public Vector3 VelocityMax { get => m_VelocityMax; }
    [SerializeField]  private float m_MovementRadius = 3f;
    public float MovementRadius { get => m_MovementRadius; }
    private Vector3 m_Velocity;
    public Vector3 Velocity { get => m_Velocity; }

    [Header("Rotation Stuff")]
     [SerializeField] private float m_RotationMaxX = 20f;
    public float RotationMaxX { get => m_RotationMaxX; }
    [SerializeField] private float m_RotationMaxZ = 45f;
    public float RotationMaxZ { get => m_RotationMaxZ; }
    #endregion

    private void LateUpdate()
    {
        InputVectorClamp_F();
        AccelApply_F();
        VelocityApply_F();
        RotationApply_F();

        m_InputVector = Vector3.zero;
    }

    /// <summary>
    /// Apply Velocity to the object.
    /// </summary>
    private void VelocityApply_F()
    {
        if (m_Velocity != Vector3.zero)
        {
            transform.position += m_Velocity * Time.deltaTime;
            Vector3 v_00 = transform.position.With(z: 0f);
            ClampRadius_F(v_00);
        }
    }

    /// <summary>
    /// Ensure player doesnt go outside the loop radius.
    /// </summary>
    /// <param name="v_00"></param>
    private void ClampRadius_F(Vector3 v_00)
    {
        if (v_00.magnitude > MovementRadius)
            transform.position = (v_00.normalized * MovementRadius).With(z: transform.position.z);
    }

    /// <summary>
    /// Rotates the plane based on current and max velocity.
    /// </summary>
    private void RotationApply_F()
    {
        transform.rotation = Quaternion.Euler
            (
                Mathf.Lerp(0f, -Mathf.Sign(m_Velocity.y) * RotationMaxX, ( VelocityMax.y - (VelocityMax.y - Mathf.Abs(m_Velocity.y)) ) / VelocityMax.y), 
                0f,
                Mathf.Lerp(0f, -Mathf.Sign(m_Velocity.x) * RotationMaxZ, ( VelocityMax.x - (VelocityMax.x - Mathf.Abs(m_Velocity.x)) ) / VelocityMax.x)
            );;
    }

    /// <summary>
    /// Applies Acceleration to Velocity.
    /// </summary>
    private void AccelApply_F()
    {        
        m_Velocity = new Vector3
            (
                VelOnAccelCalc_F(m_InputVector.x, m_Velocity.x, VelocityMax.x),
                VelOnAccelCalc_F(m_InputVector.y, m_Velocity.y, VelocityMax.y),
                VelOnAccelCalc_F(m_InputVector.z, m_Velocity.z, VelocityMax.z)
            );

        float VelOnAccelCalc_F(float input, float vel, float velMax)
        {
            float r = 0f;
            if (input != 0f)
            {
                r = vel + (Accel * input * Time.deltaTime);
                r = Mathf.Clamp(r, -velMax, velMax);
            }
            else if(vel != 0f)
            {
                r = vel + -Mathf.Sign(vel) * Deccel * Time.deltaTime;

                if (Mathf.Sign(r) != Mathf.Sign(vel)) r = 0f;
            }
            
            return r;
        }
    }

    /// <summary>
    /// Adds up Input and clamps it to one, so input can be handled in multiple points in code.
    /// </summary>
    /// <param name="v"></param>
    public void InputVectorAdd_F(Vector3 v) => m_InputVector += v;

    /// <summary>
    /// Clamps the accumulated Input to 1f.
    /// </summary>
    private void InputVectorClamp_F()
    {
        m_InputVector = new Vector3
            (
                Mathf.Clamp(m_InputVector.x, -1f, 1f),
                Mathf.Clamp(m_InputVector.y, -1f, 1f),
                Mathf.Clamp(m_InputVector.z, -1f, 1f)
            );
    }

    /// <summary>
    /// Sets Velocity to zero to completely stop the object from completely moving, instead of waiting for the object to gradually decellerate to zero.
    /// </summary>
    public void VelocitySetToZero_F()
    {
        m_Velocity = Vector3.zero;
    }
}
