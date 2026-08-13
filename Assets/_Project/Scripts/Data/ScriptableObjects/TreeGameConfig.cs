using UnityEngine;

[CreateAssetMenu(fileName = "TreeGameConfig", menuName = "TimberGame/Game Config")]
public class TreeGameConfig : ScriptableObject
{
    [Header("--- CẤU HÌNH VỊ TRÍ CÂY ---")]
    [Tooltip("Vị trí Pos Y của khúc gỗ đáy (tính từ ô trắng mẫu)")]
    public float basePositionY = -300f;

    [Tooltip("Chiều cao Height của 1 khúc gỗ")]
    public float chunkHeight = 213f;

    [Header("--- CẤU HÌNH CÂY ---")]
    [Tooltip("Số khúc gỗ hiển thị trên màn hình cùng lúc")]
    public int visibleChunksCount = 8;

    [Tooltip("Tỷ lệ % sinh ra cành cây (so với gỗ thẳng)")]
    [Range(0, 100)] public int branchSpawnRate = 60;

    [Header("--- CẤU HÌNH THỜI GIAN & ĐIỂM ---")]
    [Tooltip("Thời gian tối đa (giây)")]
    public float maxTime = 10f;

    [Tooltip("Thời gian cộng thêm mỗi lần chặt đúng (giây)")]
    public float timeBonusPerChop = 0.25f;

    [Tooltip("Điểm số nhận được mỗi lần chặt")]
    public int scorePerChop = 1;
}
