using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public GameObject m_poolObject;
    public int m_poolSize = 20;
    public List<PoolObject> m_poolItems;

    private void Start()
    {
        m_poolItems = new List<PoolObject>();

        GameObject Item;
        for (int i = 0; i < m_poolSize; i++)
        {
            Item = Instantiate(m_poolObject);

            AddItem(Item.GetComponent<PoolObject>());

            m_poolItems[i].Pool = this;
        }
    }

    public void AddItem(PoolObject item)
    {
        m_poolItems.Add(item);
        item.transform.parent = transform;
        item.gameObject.SetActive(false);
    }

    public GameObject InstantiateItem(Vector3 pos, Quaternion rotation, Transform parrent = null)
    {
        if (m_poolItems.Count <= 0)
        {
            Debug.Log("pool is empty");
            return null;
        }

        m_poolItems[0].Init(pos, rotation, parrent);

        GameObject item = m_poolItems[0].gameObject;
        m_poolItems.RemoveAt(0);
        return item;
    }
}