using UnityEngine;
using System;

//Model lưu trữ dữ liệu 1 khúc gỗ
// Không sử dụng MonoBehaviour để tối ưu dung lượng RAM

[Serializable]
public class TreeChunkData {
    [Header("Thông tin cơ bản")]
    [Tooltip("Loại khúc gỗ (Normal, BranchLeft, BranchRight, BranchRoot,...)")]
    public ChunkType type;

    [Header("Dành cho màn hình hướng dẫn")]
    [Tooltip("Chữ hướng dẫn hiển thị với khúc gỗ (VD: TAP LEFT, TAP RIGHT,...)")]
    public string instructionText;

    //constructor 1 : Mặc định
    public TreeChunkData ()
    {
        this.type = ChunkType.Normal;
        this.instructionText = string.Empty;
    }

    //contructor 2: Với tham số 
    public TreeChunkData (ChunkType type, string instructionText="") // ="" là để bỏ required, có thể truyền ChunkType và string hoặc chỉ Chunktype
    {
        this.type = type;
        this.instructionText = instructionText;
    }


}