using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JetData", menuName = "Jet Stuff/JetData")]
public class JetData : ScriptableObject
{
    [SerializeField] private int m_ID;
    public int ID { get => m_ID; }
    [SerializeField] private GameObject m_JetPawnPrefab;
    public GameObject JetPawnPrefab { get => m_JetPawnPrefab; }
    [SerializeField] private int m_Cost;
    public int Cost { get => m_Cost; }
    [SerializeField] private Sprite m_Icon;
    public Sprite Icon { get => m_Icon; }

}
