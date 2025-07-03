# 課題01: 基本的なGameObjectとスクリプトの作成

## ?? 学習目標
- Unity の基本的な操作を理解する
- C# スクリプトの作成と基本構造を学ぶ
- MonoBehaviour の概念を理解する
- コンソール出力でデバッグの基礎を学ぶ

## ? 推定所要時間
約 30-45 分

## ?? 前提知識
- Unity エディターの基本操作
- C# の変数とメソッドの基本概念

## ?? 課題内容

### ステップ1: 新しいGameObjectを作成する

1. Unity エディターでシーンを開く
2. Hierarchy ウィンドウで右クリック → Create Empty
3. 作成されたGameObjectの名前を「LearningObject」に変更

### ステップ2: 最初のスクリプトを作成する

Assets/Scripts/Challenges フォルダを作成し、以下のスクリプトを**自分で入力**してください：

```csharp
using UnityEngine;

// あなたの最初のUnityスクリプトです！
// MonoBehaviourを継承することで、Unityの機能を使用できます
public class MyFirstScript : MonoBehaviour
{
    // ===============================
    // パブリック変数（Unityエディターで設定可能）
    // ===============================
    
    [Header("学習用設定")]
    public string playerName = "初心者プレイヤー";    // プレイヤーの名前
    public int playerLevel = 1;                    // プレイヤーのレベル
    public float moveSpeed = 5.0f;                 // 移動速度
    public bool isActive = true;                   // アクティブかどうか
    
    // ===============================
    // プライベート変数（このスクリプト内でのみ使用）
    // ===============================
    
    private int score = 0;                         // スコア
    private float startTime;                       // ゲーム開始時間
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // ゲーム開始時間を記録
        startTime = Time.time;
        
        // コンソールにメッセージを出力（Consoleウィンドウで確認できます）
        Debug.Log("=== ゲーム開始 ===");
        Debug.Log("プレイヤー名: " + playerName);
        Debug.Log("レベル: " + playerLevel);
        Debug.Log("移動速度: " + moveSpeed);
        Debug.Log("開始時間: " + startTime);
        
        // プレイヤーが設定されているかチェック
        if (isActive)
        {
            Debug.Log(playerName + " がゲームに参加しました！");
        }
        else
        {
            Debug.Log("プレイヤーは非アクティブです");
        }
        
        // 初期スコアを設定
        AddScore(100);  // ゲーム開始ボーナス
    }
    
    // Update()は、毎フレーム（通常1秒間に60回）呼び出されます
    void Update()
    {
        // ゲーム開始からの経過時間を計算
        float elapsedTime = Time.time - startTime;
        
        // 10秒ごとにメッセージを表示（% は余り演算子）
        if (elapsedTime > 0 && (int)elapsedTime % 10 == 0 && Time.frameCount % 60 == 0)
        {
            Debug.Log("経過時間: " + (int)elapsedTime + " 秒");
            Debug.Log("現在のスコア: " + score);
        }
        
        // スペースキーが押されたらボーナススコア
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddScore(50);
            Debug.Log("スペースキーボーナス！ (+50点)");
        }
        
        // Enterキーが押されたらレベルアップ
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LevelUp();
        }
    }
    
    // ===============================
    // カスタムメソッド（自分で作る機能）
    // ===============================
    
    // スコアを追加するメソッド
    void AddScore(int points)
    {
        score += points;  // score = score + points と同じ意味
        Debug.Log("スコア追加: +" + points + " (合計: " + score + ")");
    }
    
    // レベルアップするメソッド
    void LevelUp()
    {
        playerLevel++;  // playerLevel = playerLevel + 1 と同じ意味
        moveSpeed += 1.0f;  // 移動速度も少し上昇
        
        Debug.Log("=== レベルアップ！ ===");
        Debug.Log("新しいレベル: " + playerLevel);
        Debug.Log("新しい移動速度: " + moveSpeed);
        
        // レベルアップボーナス
        AddScore(playerLevel * 100);
    }
    
    // ゲーム情報を表示するメソッド
    void ShowGameInfo()
    {
        Debug.Log("=== ゲーム情報 ===");
        Debug.Log("プレイヤー: " + playerName);
        Debug.Log("レベル: " + playerLevel);
        Debug.Log("スコア: " + score);
        Debug.Log("移動速度: " + moveSpeed);
        Debug.Log("経過時間: " + (Time.time - startTime) + " 秒");
    }
}
```

### ステップ3: スクリプトをGameObjectにアタッチする

1. 作成した「LearningObject」を選択
2. Inspector ウィンドウで「Add Component」をクリック
3. 「MyFirstScript」を検索して追加

### ステップ4: パラメータを調整して実行する

1. Inspector で以下の値を変更してみてください：
   - Player Name: あなたの名前
   - Player Level: 5
   - Move Speed: 10.0
   - Is Active: チェックを入れる

2. ゲームを実行（Play ボタン）
3. Console ウィンドウを開いてメッセージを確認

### ステップ5: キー入力をテストする

ゲーム実行中に以下を試してください：
- **スペースキー**: ボーナススコア獲得
- **Enterキー**: レベルアップ

## ?? 実験してみよう

### 実験1: パラメータの変更
- Move Speed を 100 にしてみる
- Player Level を 0 にしてみる
- Is Active のチェックを外してみる

### 実験2: コードの改造
以下の機能を追加してみてください：

```csharp
// Update()メソッド内に追加
if (Input.GetKeyDown(KeyCode.R))
{
    ResetGame();
}

// 新しいメソッドを追加
void ResetGame()
{
    score = 0;
    playerLevel = 1;
    moveSpeed = 5.0f;
    startTime = Time.time;
    Debug.Log("=== ゲームリセット ===");
}
```

### 実験3: デバッグメッセージの改良
```csharp
// より詳細な情報を表示
Debug.Log($"[{Time.time:F1}s] {playerName} (Lv.{playerLevel}) Score: {score}");
```

## ? よくあるエラー

### エラー1: スクリプトが見つからない
**原因**: ファイル名とクラス名が一致していない
**解決方法**: ファイル名を「MyFirstScript.cs」にする

### エラー2: Console にメッセージが表示されない
**原因**: Console ウィンドウが開いていない
**解決方法**: Window → General → Console を開く

### エラー3: キー入力が反応しない
**原因**: Game ビューにフォーカスが当たっていない
**解決方法**: Game ビューをクリックしてからキーを押す

## ?? 学習ポイント

### 理解すべき概念
1. **MonoBehaviour**: Unityのコンポーネントシステム
2. **Start() vs Update()**: 実行タイミングの違い
3. **public vs private**: 変数のアクセス制御
4. **Debug.Log()**: デバッグ出力の重要性
5. **Input クラス**: ユーザー入力の処理

### 覚えておくべきパターン
- 変数の宣言と初期化
- メソッドの定義と呼び出し
- 条件分岐 (if文)
- Unity の属性 ([Header], [SerializeField])

## ?? 次のステップ

この課題が完了したら：
1. **復習**: コードの各行が何をしているか説明できるか確認
2. **実験**: パラメータや値を変更して動作の違いを観察
3. **次の課題**: 課題02「プレイヤーの基本移動システム」に進む

## ?? 追加チャレンジ

余裕がある場合は以下にも挑戦してみてください：

### チャレンジ1: 新しいキー入力を追加
- Q キーでスコアを表示
- E キーでゲーム情報を表示

### チャレンジ2: 条件付きボーナス
- レベルが10以上の時だけスペシャルボーナス
- スコアが1000以上で特別メッセージ

### チャレンジ3: ランダム要素の追加
```csharp
// ランダムボーナス機能
void GiveRandomBonus()
{
    int randomBonus = Random.Range(10, 100);
    AddScore(randomBonus);
    Debug.Log("ランダムボーナス: " + randomBonus);
}
```

おめでとうございます！?? 
あなたは最初のUnityスクリプトを完成させました！