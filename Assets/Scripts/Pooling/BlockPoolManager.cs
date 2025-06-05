using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace TowerTap
{
    public class BlockPoolManager : MonoBehaviour
    {
        [SerializeField] private Block blockPrefab;

        private ObjectPool<Block> blockPool;
        private GameData _gameData;
        private ShopData _shopData;
        private IEventBus _eventBus;
    
        private readonly List<Block> _allBlocks = new List<Block>();

        [Inject]
        public void Construct(IEventBus eventBus, GameData gameData, ShopData shopData)
        {
            _eventBus  = eventBus;
            _gameData  = gameData;
            _shopData  = shopData;
        
            _eventBus.Subscribe<BlockSkinChangedEvent>(OnBlockSkinChanged);
        }

        private void Awake()
        {
            CreateBlockPool();
        }

        private void CreateBlockPool()
        {
            blockPool = new ObjectPool<Block>(
                createFunc: () =>
                {
                    var obj = Instantiate(blockPrefab);
                    obj.gameObject.SetActive(false);

                    _allBlocks.Add(obj);

                    return obj;
                },
                actionOnGet: obj =>
                {
                    UpdateBlockColor(obj);
                    obj.gameObject.SetActive(true);
                },
                actionOnRelease: obj => { obj.gameObject.SetActive(false); },
                actionOnDestroy: obj =>
                {
                    Destroy(obj);
                    _allBlocks.Remove(obj);
                },
                collectionCheck: false,
                defaultCapacity: 100,
                maxSize: 1000
            );
        }

        public Block GetBlock() => blockPool.Get();

        public void ReleaseBlock(Block block)
        {
            block.SetDefaultRotation();
            block.SetKinematic(true);
            blockPool.Release(block);
        }

        private void UpdateBlockColor(Block block)
        {
            int index = _gameData.selectedSkinIndex;
            if (index < 0 || index >= _shopData.shopDefinitions.Count)
                return;

            Color chosenColor = _shopData.shopDefinitions[index].color;
            var rend = block.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = chosenColor;
            }
        }
        private void OnBlockSkinChanged(BlockSkinChangedEvent evt)
        {
            int index = _gameData.selectedSkinIndex;
            if (index < 0 || index >= _shopData.shopDefinitions.Count)
                return;

            Color chosenColor = _shopData.shopDefinitions[index].color;
            foreach (var block in _allBlocks)
            {
                var rend = block.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.color = chosenColor;
                }
            }
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<BlockSkinChangedEvent>(OnBlockSkinChanged);
        }
    }
}
