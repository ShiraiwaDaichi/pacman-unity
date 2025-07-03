# 課題06: 音響システムの実装

## ?? 学習目標
- Unity Audio System の理解と実装
- AudioSource と AudioClip の使用方法
- 音響効果とBGMの管理システム
- 3D音響とSpatial Audio の基礎
- 音量制御とミキシング技術

## ? 推定所要時間
約 45-60 分

## ?? 前提知識
- 課題05の完了
- Audio コンポーネントの基本知識
- コルーチンの理解

## ?? 課題内容

### ステップ1: 音響リソースの準備

#### 1.1 フォルダ構成
```
Assets/
├── Audio/
│   ├── BGM/
│   ├── SFX/
│   └── Voice/
└── Scripts/
    └── Audio/
```

#### 1.2 オーディオファイルの準備
- BGM用: .mp3 または .wav ファイル
- 効果音用: .wav ファイル（短時間）
- 設定: Import Settings を Audio に設定

### ステップ2: 音響管理システムの作成

以下のスクリプトを **自分で入力** してください：

```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 音響システムの統合管理クラス
public class AudioManager : MonoBehaviour
{
    // ===============================
    // シングルトンパターン
    // ===============================
    
    public static AudioManager Instance { get; private set; }
    
    // ===============================
    // 音響設定
    // ===============================
    
    [Header("音響設定")]
    public float masterVolume = 1.0f;           // マスター音量
    public float musicVolume = 0.8f;            // BGM音量
    public float sfxVolume = 1.0f;              // 効果音音量
    public float voiceVolume = 1.0f;            // ボイス音量
    
    [Header("BGM設定")]
    public AudioClip[] backgroundMusic;         // BGM配列
    public bool loopBGM = true;                 // BGMループ設定
    public float bgmFadeTime = 2.0f;            // BGMフェード時間
    
    [Header("効果音設定")]
    public AudioClip[] soundEffects;           // 効果音配列
    public int maxSFXSources = 10;             // 最大効果音同時再生数
    
    [Header("3D音響設定")]
    public bool use3DAudio = true;              // 3D音響使用
    public float maxDistance = 20f;             // 最大聞こえる距離
    public float minDistance = 1f;              // 最小距離
    
    // ===============================
    // オーディオソース
    // ===============================
    
    private AudioSource bgmSource;              // BGM専用AudioSource
    private AudioSource[] sfxSources;           // 効果音用AudioSource配列
    private AudioSource voiceSource;            // ボイス専用AudioSource
    
    // ===============================
    // 内部管理変数
    // ===============================
    
    private Dictionary<string, AudioClip> audioClips;  // 音響クリップ辞書
    private Dictionary<string, float> lastPlayTime;    // 最終再生時間記録
    private int currentSFXIndex = 0;                   // 現在の効果音インデックス
    private bool isMuted = false;                      // ミュート状態
    
    // フェード処理用
    private Coroutine bgmFadeCoroutine;
    private bool isFading = false;
    
    // ===============================
    // 音響効果の定義
    // ===============================
    
    public enum SoundType
    {
        BGM,
        SFX,
        Voice
    }
    
    public enum SFXType
    {
        ButtonClick,
        ItemCollect,
        EnemyHit,
        PlayerHurt,
        PowerUp,
        GameOver,
        Victory,
        Jump,
        Footstep,
        Explosion
    }
    
    public enum BGMType
    {
        MainMenu,
        GamePlay,
        GameOver,
        Victory,
        Boss
    }
    
    // ===============================
    // Unity標準メソッド
    // ===============================
    
    void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadAudioSettings();
        Debug.Log("=== 音響システム開始 ===");
    }
    
    void Update()
    {
        // デバッグ用キー入力
        HandleDebugInput();
    }
    
    // ===============================
    // 初期化メソッド
    // ===============================
    
    void InitializeAudioSystem()
    {
        // BGMソースの作成
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = loopBGM;
        bgmSource.playOnAwake = false;
        bgmSource.volume = musicVolume * masterVolume;
        
        // 効果音ソースの作成
        sfxSources = new AudioSource[maxSFXSources];
        for (int i = 0; i < maxSFXSources; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
            sfxSources[i].loop = false;
            sfxSources[i].volume = sfxVolume * masterVolume;
        }
        
        // ボイスソースの作成
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.volume = voiceVolume * masterVolume;
        
        // 音響クリップ辞書の初期化
        audioClips = new Dictionary<string, AudioClip>();
        lastPlayTime = new Dictionary<string, float>();
        
        // 音響クリップの登録
        RegisterAudioClips();
    }
    
    void RegisterAudioClips()
    {
        // BGMの登録
        for (int i = 0; i < backgroundMusic.Length; i++)
        {
            if (backgroundMusic[i] != null)
            {
                audioClips[backgroundMusic[i].name] = backgroundMusic[i];
            }
        }
        
        // 効果音の登録
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i] != null)
            {
                audioClips[soundEffects[i].name] = soundEffects[i];
            }
        }
        
        Debug.Log($"音響クリップ登録完了: {audioClips.Count}個");
    }
    
    // ===============================
    // BGM制御メソッド
    // ===============================
    
    public void PlayBGM(string clipName, bool fadeIn = true)
    {
        if (audioClips.ContainsKey(clipName))
        {
            PlayBGM(audioClips[clipName], fadeIn);
        }
        else
        {
            Debug.LogWarning($"BGM '{clipName}' が見つかりません");
        }
    }
    
    public void PlayBGM(AudioClip clip, bool fadeIn = true)
    {
        if (clip == null || bgmSource == null) return;
        
        if (fadeIn && bgmSource.isPlaying)
        {
            // フェードアウト→フェードイン
            StartCoroutine(FadeBGM(clip));
        }
        else
        {
            // 直接再生
            bgmSource.clip = clip;
            bgmSource.Play();
            Debug.Log($"BGM再生: {clip.name}");
        }
    }
    
    public void PlayBGM(BGMType bgmType, bool fadeIn = true)
    {
        string clipName = GetBGMClipName(bgmType);
        PlayBGM(clipName, fadeIn);
    }
    
    public void StopBGM(bool fadeOut = true)
    {
        if (bgmSource == null) return;
        
        if (fadeOut)
        {
            StartCoroutine(FadeOutBGM());
        }
        else
        {
            bgmSource.Stop();
            Debug.Log("BGM停止");
        }
    }
    
    public void PauseBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Pause();
            Debug.Log("BGM一時停止");
        }
    }
    
    public void ResumeBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.UnPause();
            Debug.Log("BGM再開");
        }
    }
    
    // ===============================
    // 効果音制御メソッド
    // ===============================
    
    public void PlaySFX(string clipName, float volume = 1.0f, float pitch = 1.0f)
    {
        if (audioClips.ContainsKey(clipName))
        {
            PlaySFX(audioClips[clipName], volume, pitch);
        }
        else
        {
            Debug.LogWarning($"効果音 '{clipName}' が見つかりません");
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null || sfxSources == null) return;
        
        // 同じ音を短時間で連続再生するのを防ぐ
        if (lastPlayTime.ContainsKey(clip.name))
        {
            if (Time.time - lastPlayTime[clip.name] < 0.1f)
            {
                return;
            }
        }
        
        // 利用可能なAudioSourceを探す
        AudioSource availableSource = GetAvailableSFXSource();
        if (availableSource != null)
        {
            availableSource.clip = clip;
            availableSource.volume = volume * sfxVolume * masterVolume;
            availableSource.pitch = pitch;
            availableSource.Play();
            
            lastPlayTime[clip.name] = Time.time;
            Debug.Log($"効果音再生: {clip.name}");
        }
        else
        {
            Debug.LogWarning("利用可能な効果音ソースがありません");
        }
    }
    
    public void PlaySFX(SFXType sfxType, float volume = 1.0f, float pitch = 1.0f)
    {
        string clipName = GetSFXClipName(sfxType);
        PlaySFX(clipName, volume, pitch);
    }
    
    public void PlaySFX3D(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        if (clip == null || !use3DAudio) return;
        
        // 3D音響用のAudioSourceを作成
        GameObject tempAudioGO = new GameObject("TempAudio");
        tempAudioGO.transform.position = position;
        
        AudioSource tempAudioSource = tempAudioGO.AddComponent<AudioSource>();
        tempAudioSource.clip = clip;
        tempAudioSource.volume = volume * sfxVolume * masterVolume;
        tempAudioSource.spatialBlend = 1.0f;  // 3D音響
        tempAudioSource.rolloffMode = AudioRolloffMode.Linear;
        tempAudioSource.minDistance = minDistance;
        tempAudioSource.maxDistance = maxDistance;
        tempAudioSource.Play();
        
        // 再生終了後にオブジェクトを削除
        Destroy(tempAudioGO, clip.length);
        
        Debug.Log($"3D効果音再生: {clip.name} at {position}");
    }
    
    // ===============================
    // ボイス制御メソッド
    // ===============================
    
    public void PlayVoice(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null || voiceSource == null) return;
        
        voiceSource.clip = clip;
        voiceSource.volume = volume * voiceVolume * masterVolume;
        voiceSource.Play();
        
        Debug.Log($"ボイス再生: {clip.name}");
    }
    
    public void StopVoice()
    {
        if (voiceSource != null)
        {
            voiceSource.Stop();
            Debug.Log("ボイス停止");
        }
    }
    
    // ===============================
    // 音量制御メソッド
    // ===============================
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        Debug.Log($"マスター音量: {masterVolume:F2}");
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume * masterVolume;
        }
        Debug.Log($"BGM音量: {musicVolume:F2}");
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (sfxSources[i] != null)
            {
                sfxSources[i].volume = sfxVolume * masterVolume;
            }
        }
        Debug.Log($"効果音音量: {sfxVolume:F2}");
    }
    
    public void SetVoiceVolume(float volume)
    {
        voiceVolume = Mathf.Clamp01(volume);
        if (voiceSource != null)
        {
            voiceSource.volume = voiceVolume * masterVolume;
        }
        Debug.Log($"ボイス音量: {voiceVolume:F2}");
    }
    
    public void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateAllVolumes();
        Debug.Log($"ミュート: {isMuted}");
    }
    
    void UpdateAllVolumes()
    {
        float multiplier = isMuted ? 0f : 1f;
        
        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume * masterVolume * multiplier;
        }
        
        if (voiceSource != null)
        {
            voiceSource.volume = voiceVolume * masterVolume * multiplier;
        }
        
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (sfxSources[i] != null)
            {
                sfxSources[i].volume = sfxVolume * masterVolume * multiplier;
            }
        }
    }
    
    // ===============================
    // フェード処理
    // ===============================
    
    IEnumerator FadeBGM(AudioClip newClip)
    {
        if (isFading) yield break;
        
        isFading = true;
        
        // フェードアウト
        float startVolume = bgmSource.volume;
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / bgmFadeTime);
            yield return null;
        }
        
        // 新しいクリップを設定
        bgmSource.clip = newClip;
        bgmSource.Play();
        
        // フェードイン
        float targetVolume = musicVolume * masterVolume;
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, targetVolume, t / bgmFadeTime);
            yield return null;
        }
        
        bgmSource.volume = targetVolume;
        isFading = false;
        
        Debug.Log($"BGMフェード完了: {newClip.name}");
    }
    
    IEnumerator FadeOutBGM()
    {
        if (isFading) yield break;
        
        isFading = true;
        float startVolume = bgmSource.volume;
        
        for (float t = 0; t < bgmFadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / bgmFadeTime);
            yield return null;
        }
        
        bgmSource.Stop();
        bgmSource.volume = musicVolume * masterVolume;
        isFading = false;
        
        Debug.Log("BGMフェードアウト完了");
    }
    
    // ===============================
    // ユーティリティメソッド
    // ===============================
    
    AudioSource GetAvailableSFXSource()
    {
        // 再生中でないソースを探す
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (sfxSources[i] != null && !sfxSources[i].isPlaying)
            {
                return sfxSources[i];
            }
        }
        
        // 全て再生中の場合、最も古いものを使用
        currentSFXIndex = (currentSFXIndex + 1) % sfxSources.Length;
        return sfxSources[currentSFXIndex];
    }
    
    string GetSFXClipName(SFXType sfxType)
    {
        return sfxType switch
        {
            SFXType.ButtonClick => "ButtonClick",
            SFXType.ItemCollect => "ItemCollect",
            SFXType.EnemyHit => "EnemyHit",
            SFXType.PlayerHurt => "PlayerHurt",
            SFXType.PowerUp => "PowerUp",
            SFXType.GameOver => "GameOver",
            SFXType.Victory => "Victory",
            SFXType.Jump => "Jump",
            SFXType.Footstep => "Footstep",
            SFXType.Explosion => "Explosion",
            _ => "DefaultSFX"
        };
    }
    
    string GetBGMClipName(BGMType bgmType)
    {
        return bgmType switch
        {
            BGMType.MainMenu => "MainMenuBGM",
            BGMType.GamePlay => "GamePlayBGM",
            BGMType.GameOver => "GameOverBGM",
            BGMType.Victory => "VictoryBGM",
            BGMType.Boss => "BossBGM",
            _ => "DefaultBGM"
        };
    }
    
    void LoadAudioSettings()
    {
        // 保存された音響設定を読み込み
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1.0f);
        
        UpdateAllVolumes();
    }
    
    public void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
        PlayerPrefs.Save();
        
        Debug.Log("音響設定保存完了");
    }
    
    void HandleDebugInput()
    {
        // デバッグ用キー入力
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayBGM(BGMType.MainMenu);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayBGM(BGMType.GamePlay);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlaySFX(SFXType.ButtonClick);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlaySFX(SFXType.ItemCollect);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StopBGM();
        }
    }
    
    // ===============================
    // パブリックメソッド
    // ===============================
    
    public bool IsBGMPlaying()
    {
        return bgmSource != null && bgmSource.isPlaying;
    }
    
    public float GetBGMPosition()
    {
        return bgmSource != null ? bgmSource.time : 0f;
    }
    
    public void SetBGMPosition(float time)
    {
        if (bgmSource != null)
        {
            bgmSource.time = time;
        }
    }
    
    public AudioClip GetCurrentBGM()
    {
        return bgmSource?.clip;
    }
    
    public void AddAudioClip(string name, AudioClip clip)
    {
        if (clip != null)
        {
            audioClips[name] = clip;
            Debug.Log($"音響クリップ追加: {name}");
        }
    }
    
    public void RemoveAudioClip(string name)
    {
        if (audioClips.ContainsKey(name))
        {
            audioClips.Remove(name);
            Debug.Log($"音響クリップ削除: {name}");
        }
    }
    
    // ===============================
    // 終了処理
    // ===============================
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // アプリケーションが非アクティブになった時の処理
            PauseBGM();
        }
        else
        {
            // アプリケーションがアクティブになった時の処理
            ResumeBGM();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseBGM();
        }
        else
        {
            ResumeBGM();
        }
    }
    
    void OnDestroy()
    {
        SaveAudioSettings();
    }
}
```

### ステップ3: 音響連携スクリプト

```csharp
using UnityEngine;

// 他のスクリプトからの音響呼び出し例
public class AudioIntegrationExample : MonoBehaviour
{
    void Start()
    {
        // ゲーム開始時のBGM再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.BGMType.GamePlay);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // アイテム収集音の再生
            AudioManager.Instance?.PlaySFX(AudioManager.SFXType.ItemCollect);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 3D音響でヒット音を再生
            AudioManager.Instance?.PlaySFX3D(
                AudioManager.Instance.audioClips["EnemyHit"], 
                transform.position
            );
        }
    }
}
```

### ステップ4: 設定とテスト

1. AudioManager オブジェクトを作成
2. 音響ファイルをインポート
3. Inspector で音響クリップを設定
4. テストスクリプトで動作確認

## ?? 実験してみよう

### 実験1: 音響のランダム化

```csharp
public AudioClip[] footstepSounds;

public void PlayRandomFootstep()
{
    if (footstepSounds.Length > 0)
    {
        int randomIndex = Random.Range(0, footstepSounds.Length);
        PlaySFX(footstepSounds[randomIndex]);
    }
}
```

### 実験2: 音響シーケンス

```csharp
public class AudioSequence : MonoBehaviour
{
    public AudioClip[] sequenceClips;
    public float[] delays;
    
    public void PlaySequence()
    {
        StartCoroutine(PlaySequenceCoroutine());
    }
    
    IEnumerator PlaySequenceCoroutine()
    {
        for (int i = 0; i < sequenceClips.Length; i++)
        {
            AudioManager.Instance?.PlaySFX(sequenceClips[i]);
            yield return new WaitForSeconds(delays[i]);
        }
    }
}
```

### 実験3: 音響可視化

```csharp
public class AudioVisualizer : MonoBehaviour
{
    public RectTransform[] visualBars;
    public AudioSource audioSource;
    
    void Update()
    {
        if (audioSource.isPlaying)
        {
            float[] spectrum = new float[256];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            
            for (int i = 0; i < visualBars.Length; i++)
            {
                float height = spectrum[i] * 1000f;
                visualBars[i].sizeDelta = new Vector2(visualBars[i].sizeDelta.x, height);
            }
        }
    }
}
```

## ? よくあるエラー

### エラー1: 音が再生されない
**原因**: AudioListener がない、または音量が0
**解決方法**: MainCamera にAudioListener があるか確認

### エラー2: 音が重複再生される
**原因**: 同じ音を短時間で複数回再生
**解決方法**: 再生間隔制限の実装

### エラー3: 3D音響が機能しない
**原因**: spatialBlend設定 または AudioListener位置
**解決方法**: spatialBlend = 1.0f に設定

## ?? 学習ポイント

### 重要な概念
1. **AudioSource**: 音響再生コンポーネント
2. **AudioClip**: 音響データ
3. **AudioListener**: 音響受信
4. **Spatial Audio**: 3D音響
5. **Audio Mixing**: 音響ミキシング

### 音響設計のポイント
- **Performance**: 同時再生数の制限
- **Memory**: 音響ファイルの最適化
- **User Experience**: 適切な音量バランス
- **Accessibility**: 音響設定の提供

## ?? 次のステップ

この課題完了後：
1. **理解度チェック**: 音響システムの構造説明
2. **実験**: 3D音響の実装
3. **次の課題**: 課題07「アニメーションシステム」

## ?? 追加チャレンジ

### チャレンジ1: オーディオプール
```csharp
public class AudioPool : MonoBehaviour
{
    public int poolSize = 20;
    private Queue<AudioSource> audioPool;
    
    void Start()
    {
        audioPool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateAudioSource();
        }
    }
    
    void CreateAudioSource()
    {
        GameObject audioGO = new GameObject("PooledAudio");
        audioGO.transform.SetParent(transform);
        AudioSource audioSource = audioGO.AddComponent<AudioSource>();
        audioPool.Enqueue(audioSource);
    }
    
    public AudioSource GetAudioSource()
    {
        if (audioPool.Count > 0)
        {
            return audioPool.Dequeue();
        }
        else
        {
            CreateAudioSource();
            return audioPool.Dequeue();
        }
    }
}
```

### チャレンジ2: 動的音響生成
```csharp
public class ProceduralAudio : MonoBehaviour
{
    public float frequency = 440f;
    public float amplitude = 0.5f;
    public int sampleRate = 44100;
    
    void Start()
    {
        AudioClip clip = GenerateTone(frequency, 2f);
        AudioManager.Instance?.PlaySFX(clip);
    }
    
    AudioClip GenerateTone(float freq, float duration)
    {
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("GeneratedTone", samples, 1, sampleRate, false);
        
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * i / sampleRate) * amplitude;
        }
        
        clip.SetData(data, 0);
        return clip;
    }
}
```

### チャレンジ3: 音響イベントシステム
```csharp
public class AudioEventSystem : MonoBehaviour
{
    public static System.Action<string, Vector3> OnSFXRequested;
    public static System.Action<string> OnBGMRequested;
    
    void Start()
    {
        OnSFXRequested += PlaySFXAtPosition;
        OnBGMRequested += PlayBGMByName;
    }
    
    void PlaySFXAtPosition(string sfxName, Vector3 position)
    {
        // 3D音響で再生
        AudioManager.Instance?.PlaySFX3D(
            AudioManager.Instance.audioClips[sfxName], 
            position
        );
    }
    
    void PlayBGMByName(string bgmName)
    {
        AudioManager.Instance?.PlayBGM(bgmName);
    }
}
```

完璧です！?? 
包括的な音響システムを完成させました！