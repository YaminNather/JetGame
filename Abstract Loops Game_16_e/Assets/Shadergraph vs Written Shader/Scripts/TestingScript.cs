using UnityEngine;

namespace Assets.Shadergraph_vs_Written_Shader.Scripts
{
    public class TestingScript : MonoBehaviour
    {
        [SerializeField] private GameObject m_TestPrefab;
        [SerializeField] private int m_SpawnCount;

        [Header("Grid Settings")]
        [SerializeField] private int[] m_GridSize;
        [SerializeField] private float m_Gap;

        [Header("Material Settings")]
        [SerializeField] private TestTypeEN m_TestType;
        [SerializeField] private Material m_ShadergraphMaterial;
        [SerializeField] private Material m_WrittenMaterial;

        private void Start()
        {
            bool toBreak = false;
            for (int i = 0, spawnedCount = 0; i < m_GridSize[0]; i++)
            {
                for (int j = 0; j < m_GridSize[1]; j++, spawnedCount++)
                {
                    if (spawnedCount >= m_SpawnCount)
                    {
                        toBreak = true;
                        break;
                    }

                    GameObject SpawnedGObj = Instantiate(m_TestPrefab, new Vector3(j * m_Gap, 0.0f, i * m_Gap), Quaternion.identity);

                    SpawnedGObj.GetComponentInChildren<MeshRenderer>().sharedMaterial = m_TestType == TestTypeEN.Shadergraph ? 
                        m_ShadergraphMaterial : m_WrittenMaterial;
                }

                if (toBreak == true) break;
            }
        }
        
        private enum TestTypeEN { Shadergraph, Written }
    }
}
