# Hướng dẫn về debug

## Sơ lược

Giờ đây bạn đã có thể xử lý `Firebase RemoteConfig` và `PlayerPrefs`một cách minh bạch.
Trường hợp muốn add thêm hạng mục Config thì chỉ cần thêm 1 dòng cho mỗi hạng mục, thêm vào phía `RemoteConfig` với cùng 1 tên thì hạng mục cũng sẽ được thêm vào Debug UI.

## Áp dụng

- Hãy bố trí DebugButton(Assets/_HyperCasualTemplate/Prefabs/UI/DebugButton.prefab) ở vị trí thích hợp của scene từ Start trở đi. 
- Khi khai báo NO_UIDEBUG thì sẽ chuyển thành chế độ không hiển thị, nên khi release cũng sẽ không được hiển thị.

## Cách gọi DebugUI

- Nhấn DebugButton hoặc tap 3 điểm trên máy (iPhone/Android)
- Chức năng tap 3 điểm cũng bị vô hiệu hóa bằng khai báo NO_UIDEBUG

## Cách sử dụng DebugUI

- Khi mở ra thì thành OPTIONS nhưng ngoài ra có thể sử dụng SYSTEM, CONSOLE, PROFILER.
- Chi tiết xin xem tài liệu chính thức của SRDebugger  (https://stompyrobot.uk/tools/srdebugger/) 

### Về hạng mục OPTIONS

- configMode(Remote/Local)
    - Chuyển đổi qua lại để chọn sử dụng `RemoteConfig`hoặc Local(`PlayerPrefs`)
	- Khi chuyển đổi qua lại thì Debug menu sẽ tự động đóng để cập nhật nội dung hiển thị.
- CaptureMode(Remote/Local)
    - Ẩn tree của GameObject cái mà DebugCaptureMode component đang được AddComponent
- Fetch Remote Data
    - Lấy lại thông tin `RemoteConfig`. Debug menu sẽ tự động đóng để cập nhật nội dung hiển thị.
- Copy Remote Data
    - Copy nội dung thiết lập `RemoteConfig` vào clipboard.
- InstanceId / CopyInstanceId
    - Hiển thị IstanceId của Firebase, copy vào clipboard.
- Hạng mục đã thêm vào từng title
    - Trường hợp configMode là Local thì sẽ được ưu tiên sử dụng so với giá trị `RemoteConfig`. Mọi thay đổi sẽ được ghi vào `PlayerPrefs` ngay lập tức.


## Phương pháp định nghĩa Config

### Tập tin C# định nghĩa Config

- Assets/Libs/Config/Config.cs

### Phương pháp mô tả

- Example

        [DefaultValue(15000, 200)]AdInterval,

- `DefaultValue(15000, 200)` 
    - `15000`
	    -  Là giá trị mặc định. có thể sử dụng `bool`, `int`, `float`, `string`, kiểu (Type) của giá trị mặc định được sử dụng nguyên như vậy ở `PlayerPrefs`và Debug UI.
	- `200`
	    -  Ưu tiên hiển thị (SortPriority) ở DebugUI menu. Hãy chỉ định từ 100 trở lên.
- `AdInterval`
    - Là tên hạng mục. Hãy làm giống tên parameter của `RemoteConfig`.

## Phương pháp lấy giá trị Config

- Example

        var RConfigAd = ConfigData.GetValue<int>(Config.AdInterval);

    - Hãy gán kiểu (Type) cùng loại với giá trị mặc định cho `GetValue<T>` để lấy.

