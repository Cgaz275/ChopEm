using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [Header("--- REFERENCES ---")]
    [SerializeField] private RectTransform treeContainer; 
    [SerializeField] private TreeChunkUI chunkPrefab;    

    [Header("--- CONFIG ---")]
    [Tooltip("Kéo file SO TreeGameConfig vào đây (hoặc để trống để tự lấy từ GameManager)")]
    [SerializeField] private TreeGameConfig config;

    private readonly List<TreeChunkUI> spawnedChunks = new List<TreeChunkUI>();

    // Property lấy Config linh hoạt: Ưu tiên Config kéo tay, nếu không có sẽ tự lấy từ GameManager
    private TreeGameConfig Config
    {
        get
        {
            if (config != null) return config;
            if (GameManager.Instance != null) return GameManager.Instance.Config;
            return null;
        }
    }

    private void Start()
    {
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
        int visibleCount = Config != null ? Config.visibleChunksCount : 8;
        float baseY = Config != null ? Config.basePositionY : -300f;
        float height = Config != null ? Config.chunkHeight : 213f;

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
        }
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

        return true;
    }

    private bool IsHitBranch(ChopSide side, ChunkType type)
    {
        if (type == ChunkType.BranchBoth) return true;
        if (side == ChopSide.Left && type == ChunkType.BranchLeft) return true;
        if (side == ChopSide.Right && type == ChunkType.BranchRight) return true;
        return false;
    }

    private void UpdateChunkPositions()
    {
        float baseY = Config != null ? Config.basePositionY : -300f;
        float height = Config != null ? Config.chunkHeight : 213f;

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

    private ChunkType GetRandomChunkType()
    {
        int spawnRate = Config != null ? Config.branchSpawnRate : 60;
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
