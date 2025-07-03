# Pacman Unity Game

Unity で作成されたパックマンゲームです。

## 機能

- プレイヤーのキーボード操作による移動
- ゴーストのAI（追跡、散乱、逃走状態）
- ドットとパワーペレットの収集
- スコアシステム
- ライフシステム
- パワーモード（ゴーストを食べることができる）
- 勝利・敗北条件
- オーディオシステム
- UI管理

## プログラミング初心者向け情報

### 詳細なコメント

全てのスクリプトファイルに、プログラミング初心者でも理解できるよう詳細なコメントを追加しました：

- **基礎概念の説明**: クラス、メソッド、変数、継承などの基本的なプログラミング概念
- **Unity特有の機能**: MonoBehaviour、Transform、GameObject、コンポーネントなど
- **ゲーム開発の概念**: 当たり判定、状態管理、UI制御、音響システムなど
- **数学的概念**: ベクトル計算、三角関数、内積などゲームで使用される数学
- **設計パターン**: シングルトンパターンなどの実用的な設計手法

### 学習のポイント

1. **PacmanController.cs**: プレイヤー制御の基本
   - 入力処理
   - 移動制御
   - 衝突検出
   - 音響効果

2. **GhostController.cs**: AI（人工知能）の実装
   - 状態管理（enum使用）
   - ベクトル計算による方向決定
   - コルーチンによる時間管理
   - 動的な行動変化

3. **GameManager.cs**: ゲーム全体の管理
   - スコア・ライフ管理
   - 勝利・敗北条件
   - パワーモードの実装
   - シーン管理

4. **MazeGenerator.cs**: 2次元配列を使った迷路生成
   - 2次元配列の活用
   - オブジェクトの動的生成
   - 座標計算
   - プレハブシステム

5. **AudioManager.cs**: 音響システムとシングルトンパターン
   - 設計パターンの実装
   - 音声制御
   - インスタンス管理

6. **UIManager.cs**: ユーザーインターフェース管理
   - イベント処理
   - UI要素の制御
   - 時間制御（ポーズ機能）

7. **CameraController.cs**: カメラ制御システム
   - 滑らかな追従
   - 座標制限
   - ズーム機能

## 使用方法

### プレイヤー操作
- **移動**: 矢印キー または WASD キー
- **ポーズ**: Escキー
- **リスタート**: Rキー

### ゲーム設定

1. **Unityでプロジェクトを開く**
   - Unity Hub から「Open」を選択
   - プロジェクトフォルダを選択

2. **プレハブの作成**
   
   必要なプレハブを作成してください：
   
   - **Wall**: 壁のプレハブ
     - Box Collider 2D
     - Sprite Renderer（青色の正方形）
     - Tag: "Wall"
     - Layer: "Wall"
   
   - **Dot**: ドットのプレハブ
     - Circle Collider 2D (Is Trigger: true)
     - Sprite Renderer（黄色の小さい円）
     - Tag: "Dot"
     - Dotスクリプト
   
   - **PowerPellet**: パワーペレットのプレハブ
     - Circle Collider 2D (Is Trigger: true)
     - Sprite Renderer（黄色の大きい円）
     - Tag: "PowerPellet"
     - PowerPelletスクリプト
   
   - **Pacman**: パックマンのプレハブ
     - Circle Collider 2D (Is Trigger: true)
     - Sprite Renderer（黄色の円）
     - Tag: "Player"
     - PacmanControllerスクリプト
     - AudioSource
   
   - **Ghost**: ゴーストのプレハブ
     - Box Collider 2D (Is Trigger: true)
     - Sprite Renderer（色付きの正方形）
     - Tag: "Ghost"
     - GhostControllerスクリプト

3. **シーン設定**
   - MainSceneを開く
   - MazeGeneratorオブジェクトを作成
   - 各プレハブをMazeGeneratorに割り当て
   - GameManagerオブジェクトにUI要素を割り当て

## コンポーネント説明

### スクリプト

- **PacmanController**: プレイヤーの移動とアイテム収集を制御
- **GhostController**: ゴーストのAIと状態管理
- **GameManager**: ゲーム全体の管理（スコア、ライフ、勝利条件）
- **MazeGenerator**: 迷路の生成
- **UIManager**: UI要素の管理
- **AudioManager**: オーディオの管理
- **CameraController**: カメラの追従とズーム
- **Dot**: ドットの基本動作
- **PowerPellet**: パワーペレットの基本動作

### 設定

- **Tags**: Player, Ghost, Dot, PowerPellet, Wall
- **Layers**: Wall (8番目のレイヤー)
- **Sorting Layers**: Background, Maze, Items, Characters, UI

## 学習リソース

### プログラミング基礎を学ぶために

1. **C#の基本文法**: クラス、メソッド、変数、条件分岐、ループ
2. **Unity基礎**: GameObject、Transform、コンポーネント、シーン
3. **ゲーム開発概念**: 当たり判定、状態管理、イベント処理
4. **数学**: ベクトル、座標系、三角関数

### コードの読み方

1. **まずはコメントを読む**: 各セクションの説明を理解
2. **メソッドごとに理解**: 一つずつの機能を把握
3. **変数の役割を把握**: データの流れを追跡
4. **実際に動かして確認**: 変更してみて結果を観察

## 詰まったポイント

1. **プレハブの作成**: 手動でプレハブを作成する必要があります。
2. **スプライトの設定**: 各オブジェクトのスプライトを設定する必要があります。
3. **オーディオファイル**: 音響ファイルを用意して割り当てる必要があります。
4. **UI要素**: Canvas上にUI要素を配置し、GameManagerに割り当てる必要があります。

## 今後の改良点

- アニメーションの追加
- より複雑な迷路レイアウト
- 複数レベルの実装
- 高スコア保存機能
- より詳細なエフェクト
- モバイル対応

## 注意事項

- このプロジェクトは基本的なフレームワークです
- 実際にゲームを動作させるには、プレハブの作成とスプライトの設定が必要です
- Unity 2022.3.11f1 で動作確認済み
- 全てのスクリプトには詳細なコメントが追加されており、プログラミング初心者の学習に適しています