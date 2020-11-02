using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{   
    Dictionary<int, Pool> poolList = new Dictionary<int, Pool>();

    public Pool Populate(PoolType id, GameObject prefab, int amount)
    {
        var obj = poolList[(int) id].Populate(prefab, amount);
        return obj;
    }

    public Pool AddPool(PoolType id, bool reparent = true)
    {
        Pool pool;
        
        if(poolList.TryGetValue((int)id, out pool) == false)
        {
            pool = new Pool();
            poolList.Add((int)id, pool);

            if (reparent)
            {
                var poolsGO = GameObject.Find("[POOLS]") ?? new GameObject("[POOLS]");
                var poolGO = new GameObject("Pool:" + id);
                poolGO.transform.SetParent(poolsGO.transform);
                pool.SetParent(poolGO.transform);
            }
        }

        return pool;
    }

    public GameObject Spawn(PoolType id, GameObject prefab, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        return poolList[(int) id].Spawn(prefab, position, rotation, parent);
        
    }
    
    public T Spawn<T>(PoolType id, GameObject prefab, Vector3 position = default(Vector3),
        Quaternion rotation = default(Quaternion),
        Transform parent = null) where T : class
    {
        var val = poolList[(int) id].Spawn(prefab, position, rotation, parent);
        return val.GetComponent<T>();
    }

    public void Despawn(PoolType id, GameObject obj)
    {
        poolList[(int)id].Despawn(obj);
    }
}
