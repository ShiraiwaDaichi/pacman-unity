// ===============================
// オーディオマネージャー
// ===============================
// このスクリプトは、ゲーム内の全ての音響効果を管理します。
// BGM（背景音楽）と効果音を分けて制御し、音量調整やミュート機能も提供します。
// シングルトンパターンを使用して、ゲーム全体で一つのインスタンスのみ存在するようにします。

using UnityEngine;

// MonoBehaviourを継承することで、UnityのGameObjectにアタッチできるコンポーネントになります
public class AudioManager : MonoBehaviour
{
    // ===============================
    // オーディオソース設定（Unityエディターで設定が必要）
    // ===============================
    // AudioSourceは、音声を再生するためのUnityコンポーネントです
    
    [Header("Audio Sources")]
    public AudioSource musicAudioSource;    // BGM用のオーディオソース
    public AudioSource sfxAudioSource;      // 効果音用のオーディオソース
    
    // BGMと効果音を分けることで、それぞれ独立して音量制御やミュートが可能になります
    
    // ===============================
    // 音楽ファイル設定（Unityエディターで設定が必要）
    // ===============================
    
    [Header("Music")]
    public AudioClip backgroundMusic;       // ゲーム中のBGM
    public AudioClip gameOverMusic;         // ゲームオーバー時の音楽
    public AudioClip winMusic;              // 勝利時の音楽
    
    // ===============================
    // 効果音ファイル設定（Unityエディターで設定が必要）
    // ===============================
    
    [Header("Sound Effects")]
    public AudioClip dotSound;              // ドットを食べた時の音
    public AudioClip powerPelletSound;      // パワーペレットを食べた時の音
    public AudioClip ghostEatenSound;       // ゴーストを食べた時の音
    public AudioClip pacmanDeathSound;      // パックマンが死んだ時の音
    public AudioClip powerModeSound;        // パワーモード開始時の音
    
    // ===============================
    // 音量設定（Unityエディターで設定可能）
    // ===============================
    
    [Header("Settings")]
    public float musicVolume = 0.5f;        // BGMの音量（0.0～1.0）
    public float sfxVolume = 0.7f;          // 効果音の音量（0.0～1.0）
    
    // ===============================
    // シングルトンパターンの実装
    // ===============================
    // シングルトンパターンは、クラスのインスタンスが1つだけ存在することを保証する設計パターンです
    
    private static AudioManager instance;   // 唯一のインスタンス
    
    // プロパティを使用して、外部からインスタンスにアクセスできるようにします
    public static AudioManager Instance
    {
        get
        {
            // インスタンスが存在しない場合、シーン内から探します
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    // Awake()は、Start()よりも早く呼び出されます
    // シングルトンの初期化を行うのに適しています
    void Awake()
    {
        // インスタンスが未設定の場合、このオブジェクトをインスタンスに設定
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoadで、シーンが変わってもオブジェクトを保持
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 既にインスタンスが存在する場合、このオブジェクトを削除
            // これにより、常に一つのAudioManagerのみが存在することを保証
            Destroy(gameObject);
            return;
        }
    }
    
    // Start()は、オブジェクトが作成された最初のフレームで一度だけ呼び出されます
    void Start()
    {
        // BGM用オーディオソースの音量を設定
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
        }
        
        // 効果音用オーディオソースの音量を設定
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = sfxVolume;
        }
        
        // ゲーム開始時にBGMを再生
        PlayBackgroundMusic();
    }
    
    // ===============================
    // 音楽再生メソッド（パブリック）
    // ===============================
    
    // 背景音楽を再生します
    public void PlayBackgroundMusic()
    {
        if (musicAudioSource != null && backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;  // 再生する音楽を設定
            musicAudioSource.loop = true;             // ループ再生を有効にする
            musicAudioSource.Play();                  // 再生開始
        }
    }
    
    // ゲームオーバー音楽を再生します
    public void PlayGameOverMusic()
    {
        if (musicAudioSource != null && gameOverMusic != null)
        {
            musicAudioSource.clip = gameOverMusic;
            musicAudioSource.loop = false;            // ループしない（一度だけ再生）
            musicAudioSource.Play();
        }
    }
    
    // 勝利音楽を再生します
    public void PlayWinMusic()
    {
        if (musicAudioSource != null && winMusic != null)
        {
            musicAudioSource.clip = winMusic;
            musicAudioSource.loop = false;            // ループしない（一度だけ再生）
            musicAudioSource.Play();
        }
    }
    
    // ===============================
    // 効果音再生メソッド（パブリック）
    // ===============================
    
    // ドットを食べた時の音を再生
    public void PlayDotSound()
    {
        PlaySFX(dotSound);
    }
    
    // パワーペレットを食べた時の音を再生
    public void PlayPowerPelletSound()
    {
        PlaySFX(powerPelletSound);
    }
    
    // ゴーストを食べた時の音を再生
    public void PlayGhostEatenSound()
    {
        PlaySFX(ghostEatenSound);
    }
    
    // パ