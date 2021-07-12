# Hướng dẫn triển khai UI

## Nên cố gắng chuyển đổi UI thành Prefab

Nên cố gắng chuyển đổi UI element thành Prefab để phát triển, giống như `Assets/_HyperCasualTemplate/Prefabs/UI/UICanvas.prefab`của sample trong template.

Điều này giúp designer có thể cập nhật UI mà không cần cập nhật scene file.

## Trình tự tạo UI hỗ trợ notch (SafeArea)

1. Sao chép (Duplicate) `UICanvas.prefab`
2. Đổi tên prefab cho tệp đã sao chép
3. Bố trí UI như một Child element của SafeAreaPlate
4. Element được bố trí sẽ tham chiếu Header/Footer lấy cạnh màn hình làm chuẩn.

## Sử dụng Device Simulator để xác nhận

Sử dụng Device Simulator để xác nhận xem có thể bố trí UI tránh notch được không, etc. 

1. Ở menu thả xuống (dropdown)`Game ▼`nằm phía trên bên trái tab Game, chọn  `Simulator` để vào Simulator mode
2. Ở menu thả xuống tên thiết bị nằm phía bên phải `Simulator ▼`, chọn chẳng hạn như `Apple iPhone 11` 
3. SafeAreaPlate component sẽ phát hiện thay đổi độ phân giải và tự động điều chỉnh, nếu không thì nhấn nút `Adjust` của SafeAreaPlate.
4. Nếu không vấn đề gì thì xác nhận cả trên thiết bị không notch như `iPhone 6`
5. Xác nhận xong hết thì về lại chế độ Game tại menu thả xuống  `Simulator ▼` (Lý do là simulator mode khá nặng nên có trường hợp hình ảnh bị xáo trộn)
