# ChopEm

ChopEm là game chặt cây 2D mobile theo dạng endless game có nhịp độ nhanh, được xây dựng bằng Unity. Người chơi cần chặt cây theo hướng an toàn để ghi điểm, game được xây dựng theo lối chơi của Timberman Game nhưng được định dạng lại để triển khai nhanh với toàn bộ asset và code được xây dựng lại cũng như cắt giảm hiển thị nhân vật. 

## Tổng quan gameplay

- Người chơi chạm hoặc nhấn vào vùng bên trái/phải màn hình để chặt cây.
- Mỗi khúc cây có thể là khúc bình thường hoặc chứa nhánh ở một phía.
- Chặt vào phía có nhánh sẽ kết thúc lượt chơi.
- Chặt đúng sẽ:
  - Tăng điểm.
  - Cộng thêm thời gian.
  - Đưa khúc cây vừa chặt lên vị trí mới.
  - Phát hiệu ứng bay, xoay và mờ dần.
  - Phát âm thanh chặt cây.
- Khi thời gian về 0 hoặc người chơi chặt sai, game chuyển sang màn hình Game Over.
- Điểm cao nhất được lưu cục bộ bằng `PlayerPrefs`.

## Hình minh họa

### Tổng quan giao diện

![Tổng quan giao diện ChopEm](IMG/GeneralView.jpg)

### Các màn hình trong game

| Màn hình | Hình minh họa |
|---|---|
| Home | ![Màn hình Home](IMG/Home.png) |
| Gameplay | ![Màn hình Gameplay](IMG/Gameplay.png) |
| Game Over | ![Màn hình Game Over](IMG/GameOver.png) |
| Pause | ![Màn hình Pause](IMG/Pause%20game.png) |
| Settings | ![Màn hình Settings](IMG/Setting.png) |
| How To Play | ![Màn hình How To Play](IMG/How%20to%20play.png) |

## Công nghệ sử dụng

- Unity `6000.5.6f1`
- Universal Render Pipeline (URP)
- Unity UI và TextMesh Pro
- Unity Input System
- LeanTween cho hiệu ứng chuyển động UI
- Cấu hình gameplay bằng ScriptableObject

## Cấu trúc dự án

```text
Assets/
├── _Project/
│   ├── Art/                         # Hình ảnh và tài nguyên nghệ thuật
│   ├── Effects/                     # Âm thanh và hiệu ứng
│   ├── Prefabs/                     # Prefab dùng trong gameplay và UI
│   ├── Scenes/
│   │   └── MainScene.unity          # Scene chính của game
│   └── Scripts/
│       ├── Core/                    # Quản lý game, âm thanh và object pool
│       ├── Data/                    # Enum, model và cấu hình gameplay
│       ├── Gameplay/                 # Input, cây và logic chặt cây
│       ├── UI/                       # HUD, popup và quản lý trạng thái UI
│       └── Utilities/                # Tiện ích dùng chung
├── Settings/                         # URP và Input System settings
├── TextMesh Pro/                     # Tài nguyên TextMesh Pro
└── LeanTween/                        # Thư viện tween animation
```

## Kiến trúc mã nguồn

### Core

- `GameManager.cs`: Điều phối vòng đời trận đấu, state, điểm số, high score và timer.
- `AudioManager.cs`: Phát BGM/SFX và quản lý trạng thái mute.
- `ObjectPooler.cs`: Hệ thống object pooling tổng quát, hiện chưa được cấu hình pool trong scene.

### Data

- `ChopSide`, `ChunkType`, `GameState`, `SoundType`: Các enum dùng chung.
- `TreeChunkData.cs`: Model dữ liệu của từng khúc cây.
- `TreeGameConfig.cs`: ScriptableObject chứa các thông số như thời gian, tốc độ sinh nhánh, chiều cao khúc cây và điểm số.

Cấu hình gameplay hiện tại nằm tại `Assets/TreeGameConfig.asset`.

### Gameplay

- `GameplayInput.cs`: Nhận thao tác chặt trái/phải từ UI.
- `TreeController.cs`: Khởi tạo cây, sinh nhánh, kiểm tra hướng chặt và luân chuyển các khúc cây.
- `TreeChunkUI.cs`: Hiển thị sprite và trạng thái highlight của từng khúc cây.
- `TreeChopFeedback.cs`: Chạy hiệu ứng khúc cây bay, xoay và fade out sau khi chặt.

### UI

- `UIStateController.cs`: Bật/tắt Home, Gameplay, How To Play, Pause, Game Over và Settings theo `GameState`.
- `GameplayHUD.cs`: Hiển thị điểm, high score và thanh thời gian.
- `Popup.cs`: Placeholder dành cho logic popup trong tương lai.

## Luồng xử lý chính

```text
UI Button
    -> GameplayInput.ChopLeft/ChopRight()
    -> TreeController.Chop()
        -> Chặt an toàn:
            -> TreeChopFeedback chạy hiệu ứng
            -> cập nhật danh sách khúc cây
            -> GameManager.AddScore()
            -> GameplayHUD cập nhật điểm/thời gian
            -> AudioManager phát âm thanh Chop
        -> Chặt trúng nhánh:
            -> GameManager.TriggerGameOver()
            -> UIStateController hiển thị Game Over
            -> AudioManager phát âm thanh Lose
```

`GameManager` sử dụng event để thông báo thay đổi state, điểm số, high score và thời gian cho các thành phần UI.

## Cấu hình gameplay

Các thông số chính được quản lý trong `Assets/TreeGameConfig.asset`:

| Thông số | Giá trị hiện tại | Ý nghĩa |
|---|---:|---|
| `basePositionY` | `-679` | Vị trí dọc bắt đầu của cây |
| `chunkHeight` | `281` | Khoảng cách giữa các khúc cây |
| `visibleChunksCount` | `6` | Số khúc cây hiển thị |
| `branchSpawnRate` | `60` | Tỷ lệ sinh nhánh |
| `maxTime` | `2` | Thời gian tối đa |
| `timeBonusPerChop` | `0.25` | Thời gian cộng thêm sau mỗi lần chặt đúng |
| `scorePerChop` | `1` | Điểm nhận được sau mỗi lần chặt đúng |

Giá trị trong asset được serialize trực tiếp và có thể khác giá trị mặc định trong class `TreeGameConfig.cs`.

## Cách mở project

1. Cài Unity `6000.5.6f1`.
2. Mở thư mục gốc của project bằng Unity Hub.
3. Mở scene `Assets/_Project/Scenes/MainScene.unity`.
4. Nhấn Play để chạy game trong Unity Editor.

Scene chính đã chứa các thành phần runtime cần thiết gồm `GameManager`, `AudioManager`, `TreeController`, UI state controller, HUD và các popup.

## Lưu ý phát triển

- `GameManager` và `TreeController` cùng tham chiếu tới `TreeGameConfig.asset`; khi thay đổi cấu hình cần bảo đảm cả hai vẫn dùng cùng một asset.
- `BranchBoth` và `Root` đã có enum và sprite nhưng hiện chưa được sinh bởi logic random của `TreeController`.
- `ObjectPooler` đã có mã nguồn nhưng scene hiện chưa khai báo pool.
- Các file `Constants.cs`, `MonoSingleton.cs` và `Popup.cs` hiện đang là placeholder.
- Khi chỉnh sửa `MainScene.unity`, cần cẩn thận với các serialized reference vì scene đang chứa phần lớn UI và gameplay của project.
