using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoop0Mgr : MonoBehaviour
{
    [SerializeField] private float m_RotateSpeedZ;

    private void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, m_RotateSpeedZ * Time.deltaTime));
    }
}
