using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private Transform parentPool;
    private Stack<GameObject> poolObjectsStack = new Stack<GameObject>();
    private int multiplyCountPool = 5;
    
    public Pool Populate(GameObject prefab, int amount, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        for (int i = 0; i < amount; i++)
        {
            var go = Instantiate(prefab, position, rotation, parentPool).transform;
            if (parent == null) go.position = position;
            else go.localPosition = position;

            go.gameObject.SetActive(false);
            poolObjectsStack.Push(go.gameObject);
        }
        
        return this;
    }

    public void SetParent(Transform parent)
    {
        parentPool = parent;
    }
    
    public GameObject Spawn(GameObject prefab, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        if (poolObjectsStack.Count == 0)
            Populate(prefab, multiplyCountPool, position, rotation, parent);
        
        var go = poolObjectsStack.Pop().transform;
        go.SetParent(parent);
        go.rotation = rotation;
        go.gameObject.SetActive(true);
        if (parent == null) go.position = position;
        else go.localPosition = position;

        var poolable = go.GetComponent<IPoolable>();
        if(poolable != null) poolable.OnSpawn();

        return go.gameObject;
    }

    public void Despawn(GameObject go)
    {
        go.SetActive(false);

        var poolable = go.GetComponent<IPoolable>();
        if(poolable != null) poolable.OnDespawn();
        if(parentPool != null) go.transform.SetParent(parentPool);
        poolObjectsStack.Push(go);
    }
}
