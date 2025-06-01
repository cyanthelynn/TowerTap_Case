using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TowerStackManager : MonoBehaviour, IStartable
{
    [Inject] private GameParameters parameters;
    [Inject] private BlockPoolManager poolManager;

    private readonly Stack<Transform> stackedBlocks = new Stack<Transform>();
    private GameObject currentMovingBlock;
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
        stackedBlocks.Push(baseBlock.transform);
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

    private void CalculateSpawnScale(ref Transform lastBlock, ref Vector3 spawnScale)
    {
        float xScale = lastBlock.localScale.x;
        float zScale = lastBlock.localScale.z;
        spawnScale.Set(xScale, parameters.blockHeight, zScale);
    }

    private void DetermineSpawnParameters(ref Transform lastBlock, ref Vector3 spawnPos, ref Vector3 direction)
    {
        bool moveOnZ = layerCount % 2 == 1;
        float spawnY = layerCount * parameters.blockHeight;
        float range = parameters.movementRange;

        if (moveOnZ)
        {
            float xPos = lastBlock.localPosition.x;
            spawnPos.Set(xPos, spawnY, -range);
            direction = Vector3.forward;
        }
        else
        {
            float zPos = lastBlock.localPosition.z;
            spawnPos.Set(-range, spawnY, zPos);
            direction = Vector3.right;
        }
    }


   private void TryTrimBlock()
{
    if (currentMovingBlock == null) return;

    var lastBlock = stackedBlocks.Peek();
    var movingTransform = currentMovingBlock.transform;
    bool moveOnZ = layerCount % 2 == 1;

    if (!TryProcessAxis(moveOnZ, lastBlock, movingTransform))
    {
        isGameOver = true;
        Debug.Log("GAME OVERR");
        return;
    }

    stackedBlocks.Push(movingTransform);
    layerCount++;
    currentMovingBlock = null;
    SpawnNextMovingBlock();
}

private bool TryProcessAxis(bool onZ, Transform last, Transform moving)
{
    float origSize, lastCenter, lastHalf, movingCenter, movingHalf;
    if (onZ)
    {
        origSize = moving.localScale.z;
        lastCenter = last.localPosition.z;
        lastHalf = last.localScale.z * 0.5f;
        movingCenter = moving.localPosition.z;
        movingHalf = origSize * 0.5f;
    }
    else
    {
        origSize = moving.localScale.x;
        lastCenter = last.localPosition.x;
        lastHalf = last.localScale.x * 0.5f;
        movingCenter = moving.localPosition.x;
        movingHalf = origSize * 0.5f;
    }

    var (overlapMin, overlapMax, overlap) = CalculateOverlap(lastCenter, lastHalf, movingCenter, movingHalf);
    if (overlap <= parameters.minOverlapThreshold) return false;

    PlaceSurvivingPiece(onZ, last, moving, overlapMin, overlapMax);
    DropRemovedPiece(onZ, last, moving, movingCenter, origSize, overlapMin, overlapMax);

    return true;
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

private void PlaceSurvivingPiece(bool onZ, Transform last, Transform moving, float overlapMin, float overlapMax)
{
    float newSize = overlapMax - overlapMin;
    float newCenter = (overlapMin + overlapMax) * 0.5f;

    if (onZ)
    {
        float xPos = last.localPosition.x;
        moving.localPosition = new Vector3(xPos, moving.localPosition.y, newCenter);
        moving.localScale = new Vector3(last.localScale.x, parameters.blockHeight, newSize);
    }
    else
    {
        float zPos = last.localPosition.z;
        moving.localPosition = new Vector3(newCenter, moving.localPosition.y, zPos);
        moving.localScale = new Vector3(newSize, parameters.blockHeight, last.localScale.z);
    }
}

private void DropRemovedPiece(
    bool onZ, Transform last, Transform moving,
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

        bool dropOnMinSide = movingCenter < last.localPosition.z;
        float dropMin = dropOnMinSide ? movingMin : overlapMax;
        float dropMax = dropOnMinSide ? overlapMin : movingMax;
        dropCenter = (dropMin + dropMax) * 0.5f;

        var dropped = poolManager.GetBlock();
        dropped.transform.SetParent(transform, false);
        dropped.transform.localScale = new Vector3(last.localScale.x, parameters.blockHeight, removedSize);
        dropped.transform.localPosition = new Vector3(last.localPosition.x, moving.localPosition.y, dropCenter);
        var rb = dropped.GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
    else
    {
        float movingHalf = origSize * 0.5f;
        float movingMin = movingCenter - movingHalf;
        float movingMax = movingCenter + movingHalf;

        bool dropOnMinSide = movingCenter < last.localPosition.x;
        float dropMin = dropOnMinSide ? movingMin : overlapMax;
        float dropMax = dropOnMinSide ? overlapMin : movingMax;
        dropCenter = (dropMin + dropMax) * 0.5f;

        var dropped = poolManager.GetBlock();
        dropped.transform.SetParent(transform, false);
        dropped.transform.localScale = new Vector3(removedSize, parameters.blockHeight, last.localScale.z);
        dropped.transform.localPosition = new Vector3(dropCenter, moving.localPosition.y, last.localPosition.z);
        var rb = dropped.GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}

}
