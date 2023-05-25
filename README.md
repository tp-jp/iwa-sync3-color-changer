# iwaSync3ColorChanger

Hoshino Labs.様が作成した iwaSync3 のカラーを一括で変更できる便利ツールです。
ワールド作成時などにご利用ください。

## 導入方法

VCCをインストール済みの場合、以下の**どちらか一つ**の手順を行うことでインポートできます。

- [https://tp-jp.github.io/vpm-repos/](https://tp-jp.github.io/vpm-repos/) へアクセスし、「Add to VCC」をクリック

- VCCのウィンドウで `Setting - Packages - Add Repository` の順に開き、 `https://tp-jp.github.io/vpm-repos/index.json` を追加

[VPM CLI](https://vcc.docs.vrchat.com/vpm/cli/) を使用してインストールする場合、コマンドラインを開き以下のコマンドを入力してください。

```
vpm add repo https://tp-jp.github.io/vpm-repos/index.json
```

VCCから任意のプロジェクトを選択し、「Manage Project」から「Manage Packages」を開きます。
一覧の中から `IwaSync3ColorChanger` の右にある「＋」ボタンをクリックするか「Installed Vection」から任意のバージョンを選択することで、プロジェクトにインポートします。
![image](https://github.com/tp-jp/iwa-sync3-color-changer/assets/130125691/957ef247-86ac-4ebc-9d29-68cd9988f80d)

リポジトリを使わずに導入したい場合は [releases](https://github.com/tp-jp/LightProbeGenerator/releases) から unitypackage をダウンロードして、プロジェクトにインポートしてください。

## 使い方

1. ツールバーから `TpLab > IwaSync3ColorChanger` を選択します。  
![image](https://github.com/tp-jp/iwa-sync3-color-changer/assets/130125691/f716888a-5dbc-496f-8f6f-b19c03431445)

2. 表示されたウィンドウの設定を行い、カラーの変更を行います。  
    - 対象のiwaSync3  
変更したい iwaSync3 を指定します。

    - 変更後の色  
変更後のカラーを指定します。
      - メインカラー
      - バックグラウンドカラー
      - ボーダーカラー

    - Apply  
iwaSync3のカラーを変更します。

    - Revert  
iwaSync3のカラーを元に戻します。

## 更新履歴

[CHANGELOG](CHANGELOG.md)
