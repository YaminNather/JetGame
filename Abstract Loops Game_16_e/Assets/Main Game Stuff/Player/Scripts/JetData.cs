using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetData : MonoBehaviour
{
    [SerializeField] private string m_PlayerName;
    public string PlayerName { get => m_PlayerName; }

    [SerializeField] private GameObject m_Display_Prefab;
    public GameObject Display_Prefab { get => m_Display_Prefab; }

    [SerializeField] private int m_Cost;
    public int Cost { get => m_Cost; }
}
