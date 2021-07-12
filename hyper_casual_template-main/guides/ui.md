# UI実装についてのガイド

## UIはなるべくPrefab化する

テンプレート内のサンプルの `Assets/_HyperCasualTemplate/Prefabs/UI/UICanvas.prefab` のようにUI要素はなるべくPrefab化して開発を進めます。

こうすることにより、シーンファイルを更新せずにデザイナーがUI更新を行えるようにする為です。

## ノッチ(SafeArea)に対応するUIを作成する手順

1. `UICanvas.prefab` を複製(Duplicate)する
2. 複製した方のprefabの名前を変える
3. SafeAreaPlateのChild要素として、UIを配置する
4. 配置する要素はHeader/Footerを参考に、画面の端を基準にすること

## Device Simulatorを利用して確認する

Device Simulatorを使うと、ノッチを避けてUIが配置できているか、などを確認することができます。

1. Gameタブの左上の `Game ▼` というドロップダウンで、 `Simulator` を選んでシミュレーターモードにする
2. `Simulator ▼` の右側のデバイス名のドロップダウンで、例えば `Apple iPhone 11` を選ぶ
3. SafeAreaPlate コンポーネントが解像度変更を検知して自動的に調整されるはずだが、反応しないようなら SafeAreaPlate の `Adjust` ボタンを押す
4. 問題なさそうなら、`iPhone 6` などノッチのないデバイスでも確認する
5. 一通り確認できたら、 `Simulator ▼` ドロップダウンを Game に戻す (Simulatorモードは負荷が高く、描画が乱れるケースがあるため)
