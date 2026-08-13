using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class TreeChunkUI : MonoBehaviour
{
    [Header("--- UI REFERENCES ---")]
    [SerializeField] private Image chunkImage;
    [SerializeField] private TextMeshProUGUI instructionText; // Text hiển thị TAP LEFT/TAP RIGHT nếu ở màn HowToPlay

    [Header("--- SPRITES CONFIG ---")]
    [SerializeField] private Sprite normalSprite;      // Branch
    [SerializeField] private Sprite branchLeftSprite;  // Branch_cut Left
    [SerializeField] private Sprite branchRightSprite; // Branch_cut Right
    [SerializeField] private Sprite branchBothSprite;  // Branch_cut Both
    [SerializeField] private Sprite rootSprite;        // Branch_root

    // Lưu trữ thông tin Data hiện tại của khúc gỗ
    public TreeChunkData CurrentData { get; private set; }

    /// <summary>
    /// Cập nhật hình ảnh và dữ liệu cho khúc gỗ
    /// </summary>
    public void SetData(TreeChunkData data)
    {
        if (data == null) return;
        CurrentData = data;

        // 1. Đổi Sprite theo ChunkType
        if (chunkImage != null)
        {
            switch (data.type)
            {
                case ChunkType.BranchLeft:
                    chunkImage.sprite = branchLeftSprite;
                    break;
                case ChunkType.BranchRight:
                    chunkImage.sprite = branchRightSprite;
                    break;
                case ChunkType.BranchBoth:
                    chunkImage.sprite = branchBothSprite;
                    break;
                case ChunkType.Root:
                    chunkImage.sprite = rootSprite;
                    break;
                case ChunkType.Normal:
                default:
                    chunkImage.sprite = normalSprite;
                    break;
            }
        }

        // 2. Cập nhật Text hướng dẫn (nếu có)
        if (instructionText != null)
        {
            bool hasText = !string.IsNullOrEmpty(data.instructionText);
            instructionText.gameObject.SetActive(hasText);
            if (hasText)
            {
                instructionText.text = data.instructionText;
            }
        }
    }
}
