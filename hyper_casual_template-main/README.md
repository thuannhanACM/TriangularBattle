![UnityVersion](https://img.shields.io/static/v1?label=Unity&message=2019.4.9f1&color=success)

# hyper_casual_template

ハイパーカジュアルゲームを開発するにあたってのディレクトリ構成テンプレート

## テンプレート内に導入している外部アセット

- [Confetti FX](https://assetstore.unity.com/packages/vfx/particles/confetti-fx-82497)
- [GUI Mobile Hyper-Casual](https://assetstore.unity.com/packages/2d/gui/icons/gui-mobile-hyper-casual-157435)
- [NiceVibrations](https://assetstore.unity.com/packages/tools/integration/nice-vibrations-haptic-feedback-for-mobile-gamepads-108559)

## 利用方法

### リポジトリの作成

本テンプレートリポジトリを利用してリポジトリを作成します。

[こちら](https://docs.github.com/ja/github/creating-cloning-and-archiving-repositories/creating-a-repository-from-a-template)を参考に。

### プロジェクト固有ディレクトリへリネームする

`Assets/_HyperCasualTemplate` をプロジェクト名に合わせて `_{プロジェクト名}` にリネームして利用してください。

合わせて、Unity設定の`Product Name`（File>Build Setting>Player Settings>Player>Product Name）も`{プロジェクト名}`にリネームしてください。

### Firebase設定ファイルを追加する

[新しいアプリの開発を開始する](https://github.com/shinonomekazan/ultra-casual-management/wiki/新しいアプリの開発を開始する)を作成したDEV用Firebaseの設定ファイルを、以下の場所に配置します。

- `Assets/FirebaseConfig/GoogleService-Info-dev.plist`
- `Assets/FirebaseConfig/google-services-dev.json`

また、同じファイルをコピーして、以下のファイルも作成してください。

- `Assets/FirebaseConfig/GoogleService-Info.plist`
- `Assets/FirebaseConfig/google-services.json`

この時点で、本番用のFirebaseのファイルもあれば、合わせて設置してください。

ファイルの追加はUnityで行うように注意してください。

#### firebase_urlとDATABASE_URLの追加
plistファイル内にDATABASE_URL、jsonファイル内にfirebase_urlの記載がない場合は[こちら](https://github.com/shinonomekazan/hcg-throw-out/pull/40/commits/8900d4dd8e9435b05c46ad4f2a2ac6a139b478c4)を参考にして追加してください。

値はダミーで良いですが、RemoteConfigの切り替えがうまくいっているかのチェックに使用するので、devとprodで値を変えてください。

### 各種SDKの利用

`Assets/ThirdPartySDK` の各コンポーネントを、シーンに配置して利用してください。

## ルール

### プロジェクト用ディレクトリを作成する

`_{プロジェクト名}` のプロジェクト固有ディレクトリを作成し、その配下で開発を進めていきます。

### アソートゲーム等の追加要素について

`_AppendGames` というサブディレクトリを作成し、その配下に各追加ゲーム毎にディレクトリを切って開発を進めていきます。

テンプレート内のサンプルでは `MiniGame1` と `MiniGame2` をサンプルで配置しています。

### 各素材配置場所

外部アセット(AssetStoreから購入したもの等)は `Assets/_ExternalAssets` 配下に配置します。

アセットの内容によってはフルパスでないと動かないものもあるので、その場合のみ例外です。

また、プロジェクト固有で利用する画像、モデル、Prefabに関しては `_{プロジェクト名}` 配下のディレクトリへ配置します。

例としては以下です。

| 種別 | ディレクトリ |
| --- | --- |
| テクスチャ | Assets/_{プロジェクト名}/Textures |
| マテリアル | Assets/_{プロジェクト名}/Materials |
| モデル | Assets/_{プロジェクト名}/Models |
| プレハブ | Assets/_{プロジェクト名}/Prefabs |

## 他の開発ガイドについて

このREAMDEにない他のガイドについては、[開発ガイド](./guides/README.md) を参照してください。
