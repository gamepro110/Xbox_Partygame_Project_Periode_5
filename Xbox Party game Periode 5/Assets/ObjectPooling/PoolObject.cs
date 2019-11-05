using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    [HideInInspector] public ObjectPooling m_pool;

    public ObjectPooling Pool
    {
        set => m_pool = value;
    }

    protected abstract void ResetItem();

    public virtual void Init(Vector3 pos, Quaternion rotation, Transform parent = null)
    {
        ResetItem();
        gameObject.SetActive(true);
        transform.position = pos;
        transform.rotation = rotation;
        transform.parent = parent;
    }

    public void ReturnToPool()
    {
        m_pool.AddItem(this);
    }
}