using UnityEngine;

public class poolding : PoolObject
{
    public Block m_block = null;

    private void Start()
    {
        m_block = GetComponent<Block>();
    }

    public override void ResetItem()
    {
        m_block.SetTimer = Random.Range(4, 6);
    }
}