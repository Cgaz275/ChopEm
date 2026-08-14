using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [Header("--- REFERENCES ---")]
    [SerializeField] private RectTransform treeContainer; 
    [SerializeField] private TreeChunkUI chunkPrefab;    

    [Header("--- CONFIG ---")]
    [Tooltip("Kéo file SO TreeGameConfig vào đây")]
    [SerializeField] private TreeGameConfig config;

    private readonly List<TreeChunkUI> spawnedChunks = new List<TreeChunkUI>();

    private TreeGameConfig Config => config;

    private void Start()
    {
        Debug.Assert(config != null, "TreeController requires a TreeGameConfig reference.", this);
        if (config == null)
        {
            enabled = false;
            return;
        }

        InitTree();
    }

    public void InitTree()
    {
        foreach (var chunk in spawnedChunks)
        {
            if (chunk != null) Destroy(chunk.gameObject);
        }
        spawnedChunks.Clear();

        // Đọc cấu hình từ ScriptableObject
        int visibleCount = Config.visibleChunksCount;
        float baseY = Config.basePositionY;
        float height = Config.chunkHeight;

        for (int i = 0; i < visibleCount; i++)
        {
            TreeChunkUI newChunk = Instantiate(chunkPrefab, treeContainer);
            
            RectTransform rect = newChunk.GetComponent<RectTransform>();
            if (rect != null)
            {
                float targetY = baseY + (i * height);
                rect.anchoredPosition = new Vector2(0f, targetY);
            }

            ChunkType type = (i < 2) ? ChunkType.Normal : GetRandomChunkType();
            newChunk.SetData(new TreeChunkData(type));

            spawnedChunks.Add(newChunk);
            newChunk.SetChopHighlight(i == 0);
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

        // Xoay vòng khúc gỗ đáy lên đỉnh
        spawnedChunks.RemoveAt(0);

        ChunkType newType = GetRandomChunkType();
        bottomChunk.SetData(new TreeChunkData(newType));

        spawnedChunks.Add(bottomChunk);

        // Cập nhật lại vị trí các khúc gỗ
        UpdateChunkPositions();
        UpdateChopHighlight();

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
