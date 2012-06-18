## 概要

![08式机上撮影機](http://cdn-ak.f.st-hatena.com/images/fotolife/d/daruyanagi/20120311/20120311165515.png)

**「08式机上撮影機（Type08ScreenCapture）」**は、Windows 8の［Windows］＋［PrintScreen］機能をWindows 7で実現します。ホットキーを押すと、デスクトップのスクリーンショットを撮影して、ピクチャーフォルダへ自動保存することができます。

*   [Windows 8 は［Windows］＋［PrintScreen］キーでデスクトップのスクリーンショットを“ピクチャー”フォルダに保存できる - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/05/220912)

### 注意事項

*   [[Notice]] .NET Framework 4 Client Profile が必要です。インストール時にセットアップされます。
*   [[important]] 一部ブラウザーがインストーラーを不正なファイルとして検出します。別に怪しい挙動を仕込んではいませんが、気になる方はダウンロードを控えていただけますようお願いいたします。 - [オレの作ったアプリが不正なファイル呼ばわりされる件について - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/03/07/221611)

## ToDo または今後の実装予定

とくになし。要望があれば[[Contact]]まで。

## 変更履歴

* 1.6.0（12/06/19）
    * [修正] スクリーンショットエフェクトの復活（[デスクトップ全体を一時的に暗転させたい (2) - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/06/19/022455)）
    * [修正] サウンドの存否チェックを追加
* 1.5.0（12/06/16）
    * 保存フォルダの変更（[Windows 8 RP のスクリーンショット機能 - だるろぐ]9http://daruyanagi.hatenablog.com/entry/2012/06/06/200220)）
    * 保存フォルダの存在チェック
    * 撮影音の変更
    * [後退] スクリーンショットエフェクトが動かない
* 1.4.0（2012/05/27）
    * バージョンの変更ミス
* 1.3.0（2012/05/27）
    * バルーン通知
    * サウンド通知
    * マウスカーソルを含めたキャプチャー
    * [修正] スクリーンショットエフェクト（[デスクトップ全体を一時的に暗転させたい - だるろぐ](http://daruyanagi.hatenablog.com/entry/2012/05/27/155731)）
    * [変更] 保存形式をPNGに
    * 設定の保存

* 1.2.0（2012/05/23）
    * [削除] 通知のON/OFF機能追加
    * [削除] マウスカーソルを含めたキャプチャー
    * スクリーンショットエフェクトの追加
    * README.md をちゃんと書いた

* 1.1.0（2012/05/22）
    * 通知のON/OFF機能追加
    * 保存フォルダを開く機能追加
    * README.md を申し訳程度に追加

* 1.0.0.1（2012/03/11）
    * 初回リリース

---

### ダウンロード

ソースコードとClickOnce 形式のインストーラーを公開しています。

#### バイナリ

[[http://download.daruyanagi.net/Type08ScreenCapture/|download.daruyanagi.net]]

#### ソースコード

[[https://github.com/daruyanagi/Type08ScreenCapture|github.com]]