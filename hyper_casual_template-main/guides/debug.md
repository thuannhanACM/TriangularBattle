# デバッグについてのガイド

## 概要

`Firebase RemoteConfig` と `PlayerPrefs` を透過的に扱えるようになりました。
Config項目を追加したくなった場合、１項目につき１行追加し`RemoteConfig` 側に同名で追加するだけで、デバッグUIに項目も追加されて扱えるようになります。
また、例えば`Next Level`など、任意のActionボタンを追加することも可能です。

## 導入

- Start以降のシーンの適切な場所に DebugButton(Assets/_HyperCasualTemplate/Prefabs/UI/DebugButton.prefab) を配置してください。
- このボタンはUnityエディタでの実行時のみ表示され、iOS/Androidでは表示されません。
    - この機能はDebugUISafetyコンポーネントで実現しています。他にリリース時に隠したいGameObjectがあれば、AddComponentしてください。

## デバッグUIの呼び出し方

- DebugButtonを押すか、実機(iPhone/Android)上で３点タップしてください。
- ３点タップ機能は、 NO_UIDEBUG 定義で無効化されます。

## デバッグUIの使い方

- 開くと OPTIONS になっていますが、他に SYSTEM, CONSOLE, PROFILER が利用可能です。
- 詳細は SRDebugger のオフィシャルドキュメント (https://stompyrobot.uk/tools/srdebugger/) を参照してください。

### OPTIONSの項目について

- configMode(Remote/Local)
    - `RemoteConfig`とLocal(`PlayerPrefs`)のどちらを利用するかを切り替えます。
	- 切り替えると、表示内容更新のために自動的にデバッグメニューが閉じます。
- CaptureMode
    - DebugCaptureModeコンポーネントがAddComponentされているGameObjectのツリーを非表示にします。
- Fetch Remote Data
    - `RemoteConfig`の情報を再取得します。表示内容更新のために自動的にデバッグメニューが閉じます。
- Copy Remote Data
    - クリップボードに `RemoteConfig`の設定内容をコピーします。
- InstanceId / CopyInstanceId
    - FirebaseのInstanceIdを表示、クリップボードにコピーします。
- タイトルごとに追加した項目
    - configModeがLocalだった場合、`RemoteConfig`の値より優先して利用されます。変更すると即座に`PlayerPrefs`に記録されます。


## Configの定義方法

### Config定義C#ファイル

- Assets/Libs/Config/Config.cs

### 記述方法

- Example

        [DefaultValue(15000, 200, "AdInt")]AdInterval,

- `DefaultValue(15000, 200)` 
    - `15000`
	    -  デフォルト値です。 `bool`, `int`, `float`, `string` を利用でき、デフォルト値の型(Type)がそのまま `PlayerPrefs` や デバッグUI で利用されます。
	- `200`
	    -  デバッグUIメニュー上での 表示優先(SortPriority)です。100以上を指定してください。
	- "AdInt"
	    -  デバッグUIメニュー上での 表示名です。省略すると項目名になります。項目名が長すぎる場合などにご利用ください。
- `AdInterval`
    - 項目名。`RemoteConfig` のパラメータ名と同一にしてください。

## Config値の取得方法

- Example

        var RConfigAd = ConfigData.GetValue<int>(Config.AdInterval);

    - デフォルト値と同一の型(Type)を `GetValue<T>` に与えて取得してください。

## 値が変わった時に何か実行する

- Example
```
public static void AdInterval(object newValue)
{
    Debug.Log($"AdInterval changed => {newValue}");
}
```
    - class ConfigAction に、Configと同名で、Action<object>となるメソッドを記述してください。


## 任意のActionボタンの定義方法

### Config定義C#ファイル

- Assets/Libs/Config/SROptions.Action.cs
    - ここに記述したボタンは、Pin止め扱いになってゲーム画面右上に表示されます
    - デバッグメニューを開き、Pinアイコンで操作してActionカテゴリのPinを解除すれば、ゲーム画面から消すこともできます

### 記述方法

- 基本的には、オフィシャルドキュメントを参照してください
    - https://www.stompyrobot.uk/tools/srdebugger/documentation/#options_tab

- Example

    [Sort(0)]
    [Category(ActionCategory)]
    public void Restart()
    {
        // Implementation
    }

- [Sort(0)]
    - Actionグループ内での表示順です
