using UnityEngine;
using UnityEngine.Pool;

public class BlockPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    private ObjectPool<GameObject> blockPool;

    private void Awake()
    {
        blockPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var obj = Instantiate(blockPrefab);
                obj.SetActive(false);
                return obj;
            },
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 100,
            maxSize: 10000
        );
    }

    public GameObject GetBlock()
    {
        return blockPool.Get();
    }

    public void ReleaseBlock(GameObject block)
    {
        blockPool.Release(block);
    }
}