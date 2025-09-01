using System.Collections.Generic;
using UnityEngine;

public class DogScrollItemPool
{
    private readonly Queue<DogScrollItem> _pool = new Queue<DogScrollItem>();
    private readonly List<DogScrollItem> _activeItems = new List<DogScrollItem>();
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    public DogScrollItemPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public DogScrollItem Get()
    {
        DogScrollItem item;
        
        if (_pool.Count > 0)
        {
            item = _pool.Dequeue();
            item.gameObject.SetActive(true);
        }
        else
        {
            var itemGO = Object.Instantiate(_prefab, _parent);
            item = itemGO.GetComponent<DogScrollItem>();
        }
        
        _activeItems.Add(item);
        return item;
    }

    public void Return(DogScrollItem item)
    {
        if (item == null) return;
        
        _activeItems.Remove(item);
        item.gameObject.SetActive(false);
        item.HideLoader();
        _pool.Enqueue(item);
    }

    public void ReturnAll()
    {
        for (int i = _activeItems.Count - 1; i >= 0; i--)
        {
            Return(_activeItems[i]);
        }
    }

    public List<DogScrollItem> GetActiveItems()
    {
        return new List<DogScrollItem>(_activeItems);
    }

    public void Clear()
    {
        ReturnAll();
        
        while (_pool.Count > 0)
        {
            var item = _pool.Dequeue();
            if (item != null && item.gameObject != null)
            {
                Object.Destroy(item.gameObject);
            }
        }
    }
}
