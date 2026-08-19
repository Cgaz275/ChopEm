using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [Header("--- REFERENCES ---")]
    [SerializeField] private RectTransform treeContainer;
    [SerializeField] private TreeChunkUI chunkPrefab;
    [SerializeField] private TreeChopFeedback chopFeedback;

    [Header("--- CONFIG ---")]
    [Tooltip("Kéo file SO TreeGameConfig vào đây")]
    [SerializeField] private TreeGameConfig config;

    private readonly List<TreeChunkUI> spawnedChunks = new List<TreeChunkUI>();

    private TreeGameConfig Config => config;

    private void Awake()
    {
        if (chopFeedback == null)
        {
            chopFeedback = gameObject.AddComponent<TreeChopFeedback>();
        }
    }

    private void Start()
    {
        Debug.Assert(config != null, "TreeController requires a TreeGameConfig reference.", this);
        if (config == null)
        {
            enabled = false;
        }
    }

    public void InitTree()
    {
        if (treeContainer == null || chunkPrefab == null) return;

        if (chopFeedback != null)
        {
            chopFeedback.ResetFeedback(treeContainer);
        }

        List<TreeChunkUI> existingChunks = new List<TreeChunkUI>();
        foreach (Transform child in treeContainer)
        {
            if (child.name == "ChoppedChunkFeedback") continue;

            TreeChunkUI chunk = child.GetComponent<TreeChunkUI>();
            if (chunk != null)
            {
                LeanTween.cancel(child.gameObject);
                existingChunks.Add(chunk);
            }
        }

        int visibleCount = Config.visibleChunksCount;
        for (int i = visibleCount; i < existingChunks.Count; i++)
        {
            Destroy(existingChunks[i].gameObject);
        }

        spawnedChunks.Clear();

        float baseY = Config.basePositionY;
        float height = Config.chunkHeight;

        for (int i = 0; i < visibleCount; i++)
        {
            TreeChunkUI newChunk = i < existingChunks.Count
                ? existingChunks[i]
                : Instantiate(chunkPrefab, treeContainer);

            RectTransform rect = newChunk.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(0f, baseY + i * height);
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
            }

            ChunkType type = (i < 2) ? ChunkType.Normal : GetRandomChunkType();
            newChunk.SetData(new TreeChunkData(type));
            spawnedChunks.Add(newChunk);
        }

        UpdateChopHighlight();
    }

    public bool Chop(ChopSide side)
    {
        if (spawnedChunks.Count == 0) return false;

        TreeChunkUI bottomChunk = spawnedChunks[0];

        if (IsHitBranch(side, bottomChunk.CurrentData.type))
        {
            return false; // Thua
        }

        chopFeedback.PlayChunk(bottomChunk, side);

        // Xoay vòng khúc gỗ đáy lên đỉnh
        spawnedChunks.RemoveAt(0);

        ChunkType newType = GetRandomChunkType();
        bottomChunk.SetData(new TreeChunkData(newType));

        spawnedChunks.Add(bottomChunk);

        ClearChopHighlights();

        // Cập nhật lại vị trí các khúc gỗ
        chopFeedback.AnimateChunkPositions(
            spawnedChunks,
            Config.basePositionY,
            Config.chunkHeight,
            RefreshChopHighlight
        );

        return true;
    }

    private bool IsHitBranch(ChopSide side, ChunkType type)
    {
        if (type == ChunkType.BranchBoth) return true;
        if (side == ChopSide.Left && type == ChunkType.BranchRight) return true;
        if (side == ChopSide.Right && type == ChunkType.BranchLeft) return true;
        return false;
    }

    private void UpdateChunkPositions()
    {
        float baseY = Config.basePositionY;
        float height = Config.chunkHeight;

        for (int i = 0; i < spawnedChunks.Count; i++)
        {
            RectTransform rect = spawnedChunks[i].GetComponent<RectTransform>();
            if (rect != null)
            {
                float targetY = baseY + (i * height);
                rect.anchoredPosition = new Vector2(0f, targetY);
            }
        }
    }

    public void RefreshChopHighlight()
    {
        for (int i = 0; i < spawnedChunks.Count; i++)
        {
            spawnedChunks[i].SetChopHighlight(i == 0);
        }
    }

    private void ClearChopHighlights()
    {
        for (int i = 0; i < spawnedChunks.Count; i++)
        {
            spawnedChunks[i].SetChopHighlight(false);
        }
    }

    private void UpdateChopHighlight()
    {
        RefreshChopHighlight();
    }

    private ChunkType GetRandomChunkType()
    {
        int spawnRate = Config.branchSpawnRate;
        bool shouldSpawnBranch = Random.Range(0, 100) < spawnRate;
        if (!shouldSpawnBranch) return ChunkType.Normal;

        ChunkType lastType = (spawnedChunks.Count > 0) ? spawnedChunks[spawnedChunks.Count - 1].CurrentData.type : ChunkType.Normal;

        if (lastType == ChunkType.BranchLeft)
            return (Random.value > 0.5f) ? ChunkType.BranchLeft : ChunkType.Normal;

        if (lastType == ChunkType.BranchRight)
            return (Random.value > 0.5f) ? ChunkType.BranchRight : ChunkType.Normal;

        return (Random.value > 0.5f) ? ChunkType.BranchLeft : ChunkType.BranchRight;
    }
}
