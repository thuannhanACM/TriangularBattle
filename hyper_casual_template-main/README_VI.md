![UnityVersion](https://img.shields.io/static/v1?label=Unity&message=2019.4.9f1&color=success)

# hyper_casual_template

Template cấu trúc thư mục dev hyper casual game

## Các asset ngoài được đưa vào template

- [Confetti FX](https://assetstore.unity.com/packages/vfx/particles/confetti-fx-82497)
- [GUI Mobile Hyper-Casual](https://assetstore.unity.com/packages/2d/gui/icons/gui-mobile-hyper-casual-157435)
- [NiceVibrations](https://assetstore.unity.com/packages/tools/integration/nice-vibrations-haptic-feedback-for-mobile-gamepads-108559)

## Cách sử dụng

### Tạo Repository

Sử dụng repository mẫu sau để tạo repository

[Tham khảo tại đây](https://docs.github.com/ja/github/creating-cloning-and-archiving-repositories/creating-a-repository-from-a-template)

### Đổi tên theo tên project

Đổi `Assets/_HyperCasualTemplate` theo tên của project `_{Tên project}`

## Rule

### Tạo directory cho project

Tạo directory riêng cho project `_{tên project}` và triển khai công việc của project tại đó

### Khi thêm các Assort games

Tạo sub directory `_AppendGames`, chia nhỏ và triển khai các game bằng các directory riêng.

Tham khảo `MiniGame1` và `MiniGame2` được đặt trong templatetemplate

### Prefab hóa UI

Cố gắng phát triển lập trình theo hướng Prefab hóa UI giống như mẫu trong template `Assets/_HyperCasualTemplate/Prefabs/UI/UICanvas.prefab` để thiết kế có thể chỉnh sửa được UI mà không cần thay đổi scene file.

### Bố trí các hạng mục

Đặt các asset ngoài (ví dụ các asset được mua từ AssetStore) dưới `Assets/_ExternalAssets`

Tùy thuộc vào nội dung asset, nếu cần đặt full path mới chạy được thì sẽ xử lý ngoại lệ.

Ngoài ra, đặt các video, model, Prefab liên quan đến project tại các directory dưới `_{tên project}` 

Ví dụ tham khảo'

| hạng mục | directory |
| --- | --- |
| texture | Assets/_{tên project}/Textures |
| material | Assets/_{tên project}/Materials |
| model | Assets/_{tên project}/Models |
| prefab | Assets/_{tên project}/Prefabs |

## 他の開発ガイドについて

このREAMDEにない他のガイドについては、[開発ガイド](./guides/REAMDE_VI.md) を参照してください。
