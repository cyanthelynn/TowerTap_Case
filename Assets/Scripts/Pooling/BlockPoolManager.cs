using UnityEngine;
using UnityEngine.Pool;

public class BlockPoolManager : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;

    private ObjectPool<Block> blockPool;

    private void Awake()
    {
        blockPool = new ObjectPool<Block>(
            createFunc: () =>
            {
                var obj = Instantiate(blockPrefab);
                obj.gameObject.SetActive(false);
                return obj;
            },
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 1000,
            maxSize: 10000
        );
    }

    public Block GetBlock() => blockPool.Get();

    public void ReleaseBlock(Block block)
    {
        block.SetDefaultRotation();
        block.SetKinematic(true);
        blockPool.Release(block);
    } 
    
    public void SetBlockPrefabMaterial(Material mat)
    {
        if (blockPrefab == null || mat == null) return;
        var renderer = blockPrefab.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = mat;
    }
}