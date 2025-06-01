using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TowerStackManager : MonoBehaviour, IStartable
{
    [Inject] private GameParameters parameters;
    [Inject] private BlockPoolManager poolManager;
    [Inject] private ParticleManager _particleManager;
    [Inject] private CameraController _cameraController;

    private readonly Stack<Block> stackedBlocks = new Stack<Block>();
    private Block currentMovingBlock;
    private int layerCount;
    private bool isGameOver;
    private Vector3 moveDirection;
    private float currentRange;

    public void Start()
    {
        Vector3 basePos = Vector3.zero;
        var baseBlock = poolManager.GetBlock();
        baseBlock.transform.SetParent(transform, false);
        baseBlock.transform.localPosition = basePos;
        baseBlock.transform.localScale = new Vector3(1f, parameters.blockHeight, 1f);
        stackedBlocks.Push(baseBlock);
        layerCount = 1;
        SpawnNextMovingBlock();
    }

    private void Update()
    {
        if (isGameOver) return;

        if (currentMovingBlock != null)
        {
            MoveBlock();
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryTrimBlock();
        }
    }

    private void MoveBlock()
    {
        var pos = currentMovingBlock.transform.localPosition;

        if (IsMovingOnZ())
        {
            pos.z = MoveAxis(pos.z, ref moveDirection.z, parameters.moveSpeed, parameters.movementRange);
        }
        else
        {
            pos.x = MoveAxis(pos.x, ref moveDirection.x, parameters.moveSpeed, parameters.movementRange);
        }

        currentMovingBlock.transform.localPosition = pos;
    }

    private bool IsMovingOnZ() => moveDirection.z != 0f;

    private float MoveAxis(float current, ref float direction, float speed, float range)
    {
        float next = current + direction * speed * Time.deltaTime;
        if (next > range)
        {
            direction = -1f;
            return range;
        }
        if (next < -range)
        {
            direction = 1f;
            return -range;
        }
        return next;
    }


    private void SpawnNextMovingBlock()
    {
        var lastBlock = stackedBlocks.Peek();
        Vector3 spawnScale = default;
        Vector3 spawnPos = default;

        CalculateSpawnScale(ref lastBlock, ref spawnScale);
        DetermineSpawnParameters(ref lastBlock, ref spawnPos, ref moveDirection);

        currentMovingBlock = poolManager.GetBlock();
        currentMovingBlock.transform.SetParent(transform, false);
        currentMovingBlock.transform.localPosition = spawnPos;
        currentMovingBlock.transform.localScale = spawnScale;
    }

    private void CalculateSpawnScale(ref Block lastBlock, ref Vector3 spawnScale)
    {
        float xScale = lastBlock.transform.localScale.x;
        float zScale = lastBlock.transform.localScale.z;
        spawnScale.Set(xScale, parameters.blockHeight, zScale);
    }

    private void DetermineSpawnParameters(ref Block lastBlock, ref Vector3 spawnPos, ref Vector3 direction)
    {
        bool moveOnZ = layerCount % 2 == 1;
        float spawnY = layerCount * parameters.blockHeight;
        float range = parameters.movementRange;

        if (moveOnZ)
        {
            float xPos = lastBlock.transform.localPosition.x;
            spawnPos.Set(xPos, spawnY, -range);
            direction = Vector3.forward;
        }
        else
        {
            float zPos = lastBlock.transform.localPosition.z;
            spawnPos.Set(-range, spawnY, zPos);
            direction = Vector3.right;
        }
    }


   private void TryTrimBlock()
{
    if (currentMovingBlock == null) return;

    var lastBlock = stackedBlocks.Peek();
    var movingTransform = currentMovingBlock;
    bool moveOnZ = layerCount % 2 == 1;

    if (!TryProcessAxis(moveOnZ, lastBlock, movingTransform))
    {
        isGameOver = true;
        currentMovingBlock.GetComponent<Rigidbody>().isKinematic = false;
        Debug.Log("GAME OVER");
        return;
    }

    stackedBlocks.Push(movingTransform);
    layerCount++;
    currentMovingBlock = null;
    SpawnNextMovingBlock();
}

   private bool TryProcessAxis(bool onZ, Block last, Block moving)
   {
       float origSize, lastCenter, lastHalf, movingCenter, movingHalf, lastFullSize;
       if (onZ)
       {
           origSize = moving.transform.localScale.z;
           lastCenter = last.transform.localPosition.z;
           lastHalf = last.transform.localScale.z * 0.5f;
           movingCenter = moving.transform.localPosition.z;
           movingHalf = origSize * 0.5f;
           lastFullSize = last.transform.localScale.z;
       }
       else
       {
           origSize = moving.transform.localScale.x;
           lastCenter = last.transform.localPosition.x;
           lastHalf = last.transform.localScale.x * 0.5f;
           movingCenter = moving.transform.localPosition.x;
           movingHalf = origSize * 0.5f;
           lastFullSize = last.transform.localScale.x;
       }

       var (overlapMin, overlapMax, overlap) = CalculateOverlap(lastCenter, lastHalf, movingCenter, movingHalf);
       if (overlap <= parameters.minOverlapThreshold) return false;

       if (Mathf.Abs(overlap - lastFullSize) < parameters.perfectPlacementThreshold)
       {
           Debug.Log("Perfect placement");
           last.PlayPerfectEffect();
           SnapToLast(onZ, last, moving);
           return true;
       }

       PlaceSurvivingPiece(onZ, last, moving, overlapMin, overlapMax);
       DropRemovedPiece(onZ, last, moving, movingCenter, origSize, overlapMin, overlapMax);

       return true;
   }
   private void SnapToLast(bool onZ, Block last, Block moving)
   {
       Vector3 pos = moving.transform.localPosition;
       Vector3 scale = moving.transform.localScale;

       if (onZ)
       {
           pos.x = last.transform.localPosition.x;
           pos.z = last.transform.localPosition.z;
           scale.x = last.transform.localScale.x;
           scale.z = last.transform.localScale.z;
       }
       else
       {
           pos.x = last.transform.localPosition.x;
           pos.z = last.transform.localPosition.z;
           scale.x = last.transform.localScale.x;
           scale.z = last.transform.localScale.z;
       }

       scale.y = parameters.blockHeight;
       moving.transform.localPosition = pos;
       moving.transform.localScale = scale;
   }


private (float overlapMin, float overlapMax, float overlap) CalculateOverlap(
    float lastCenter, float lastHalf, float movingCenter, float movingHalf)
{
    float lastMin = lastCenter - lastHalf;
    float lastMax = lastCenter + lastHalf;
    float movingMin = movingCenter - movingHalf;
    float movingMax = movingCenter + movingHalf;

    float overlapMin = Mathf.Max(lastMin, movingMin);
    float overlapMax = Mathf.Min(lastMax, movingMax);
    float overlap = overlapMax - overlapMin;
    return (overlapMin, overlapMax, overlap);
}

private void PlaceSurvivingPiece(bool onZ, Block last, Block moving, float overlapMin, float overlapMax)
{
    float newSize = overlapMax - overlapMin;
    float newCenter = (overlapMin + overlapMax) * 0.5f;

    if (onZ)
    {
        float xPos = last.transform.localPosition.x;
        moving.transform.localPosition = new Vector3(xPos, moving.transform.localPosition.y, newCenter);
        moving.transform.localScale = new Vector3(last.transform.localScale.x, parameters.blockHeight, newSize);
    }
    else
    {
        float zPos = last.transform.localPosition.z;
        moving.transform.localPosition = new Vector3(newCenter, moving.transform.localPosition.y, zPos);
        moving.transform.localScale = new Vector3(newSize, parameters.blockHeight, last.transform.localScale.z);
    }
    
    _cameraController.SetCameraHeight(last);
}

private void DropRemovedPiece(
    bool onZ, Block last, Block moving,
    float movingCenter, float origSize,
    float overlapMin, float overlapMax)
{
    float removedSize = origSize - (overlapMax - overlapMin);
    if (removedSize <= 0f) return;

    float dropCenter;
    if (onZ)
    {
        float movingHalf = origSize * 0.5f;
        float movingMin = movingCenter - movingHalf;
        float movingMax = movingCenter + movingHalf;

        bool dropOnMinSide = movingCenter < last.transform.localPosition.z;
        float dropMin = dropOnMinSide ? movingMin : overlapMax;
        float dropMax = dropOnMinSide ? overlapMin : movingMax;
        dropCenter = (dropMin + dropMax) * 0.5f;

        var dropped = poolManager.GetBlock();
        dropped.transform.SetParent(transform, false);
        dropped.transform.localScale = new Vector3(last.transform.localScale.x, parameters.blockHeight, removedSize);
        dropped.transform.localPosition = new Vector3(last.transform.localPosition.x, moving.transform.localPosition.y, dropCenter);
        dropped.SetKinematic(false);
    }
    else
    {
        float movingHalf = origSize * 0.5f;
        float movingMin = movingCenter - movingHalf;
        float movingMax = movingCenter + movingHalf;

        bool dropOnMinSide = movingCenter < last.transform.localPosition.x;
        float dropMin = dropOnMinSide ? movingMin : overlapMax;
        float dropMax = dropOnMinSide ? overlapMin : movingMax;
        dropCenter = (dropMin + dropMax) * 0.5f;

        var dropped = poolManager.GetBlock();
        dropped.transform.SetParent(transform, false);
        dropped.transform.localScale = new Vector3(removedSize, parameters.blockHeight, last.transform.localScale.z);
        dropped.transform.localPosition = new Vector3(dropCenter, moving.transform.localPosition.y, last.transform.localPosition.z);
        dropped.SetKinematic(false);
    }
}

}
