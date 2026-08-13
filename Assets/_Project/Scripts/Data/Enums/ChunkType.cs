// Các loại khối gỗ
public enum ChunkType
{
    Normal,      // Khúc gỗ thẳng (Branch)
    BranchLeft,  // Khúc gỗ cành/vết cắt bên Trái
    BranchRight, // Khúc gỗ cành/vết cắt bên Phải
    BranchBoth,  // Khúc gỗ vết cắt cả 2 bên
    Root         // Gốc cây
}