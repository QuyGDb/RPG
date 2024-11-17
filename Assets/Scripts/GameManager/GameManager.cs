using DG.Tweening;
using Esper.ESave;
using System;
using TMPEffects.Components;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [HideInInspector] public Player player;
    public EnemyManagerDetailsSO[] enemyManagerDetailsSO;
    [HideInInspector] public SaveFileSetup saveFileSetup;
    public GameState gameState;
    [HideInInspector] public Action<GameState> OnGameStateChange;
    [SerializeField] public Image beginSreen;
    [SerializeField] private Image wonPanel;
    [SerializeField] private Image wonScreen;
    [SerializeField] private TextMeshProUGUI wonText;
    private void OnEnable()
    {
        StaticEventHandler.OnPlayerChanged += StaticEventHandler_OnPlayerChanged;
        StaticEventHandler.OnRestInBonfire += StaticEventHandler_OnRestInBonfire;
    }
    private void OnDisable()
    {
        StaticEventHandler.OnPlayerChanged -= StaticEventHandler_OnPlayerChanged;
        StaticEventHandler.OnRestInBonfire -= StaticEventHandler_OnRestInBonfire;
    }

    private void StaticEventHandler_OnPlayerChanged(OnPlayerChangedEventArgs onPlayerChangedEventArgs)
    {
        this.player = onPlayerChangedEventArgs.player;
    }

    private void StaticEventHandler_OnRestInBonfire()
    {
        InitializeEnemyManagerData();
    }
    override protected void Awake()
    {
        base.Awake();
        // SceneManager.LoadScene("Coast", LoadSceneMode.Additive);
        saveFileSetup = GetComponent<SaveFileSetup>();

    }
    public void TransitionToGame()
    {
        if (saveFileSetup.GetSaveFile().HasData("Checkpoint"))
        {
            string checkpoint = saveFileSetup.GetSaveFile().GetData<string>("Checkpoint");
            if (checkpoint == "Coast")
            {
                player.transform.position = player.spawnPosition[0];
                SceneManager.LoadScene("Coast", LoadSceneMode.Additive);
            }
            else if (checkpoint == "The Forest")
            {
                player.transform.position = player.spawnPosition[1];
                SceneManager.LoadScene("The Forest", LoadSceneMode.Additive);
            }
        }
        GameResources.Instance.beginUI.SetFloat("_FadeAmount", 0);
        beginSreen.color = new Color(0, 0, 0, 1);
        beginSreen.gameObject.SetActive(true);
        beginSreen.DOFade(0f, 1.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            beginSreen.gameObject.SetActive(false);
            HandleGameState(GameState.Play);
        });
    }
    public void HandleGameState(GameState gameState)
    {
        this.gameState = gameState;
        switch (gameState)
        {
            case GameState.Intro:
                Intro();
                break;
            case GameState.Begin:
                break;
            case GameState.Instruct:
                break;
            case GameState.Play:
                Play();
                break;
            case GameState.EngagedBoss:
                EngagedBoss();
                break;
            case GameState.Won:
                WonScene();
                break;
            default:
                break;
        }
        OnGameStateChange?.Invoke(gameState);
    }

    private void WonScene()
    {
        SceneManager.UnloadSceneAsync("North Of The Forest");
        wonPanel.gameObject.SetActive(true);
        wonPanel.color = new Color(0, 0, 0, 0);
        wonPanel.DOFade(1, 1).OnComplete(() =>
        {
            wonText.gameObject.SetActive(true);
            var tmpWriter = wonText.GetComponent<TMPWriter>();
            tmpWriter.OnFinishWriter.RemoveAllListeners();
            tmpWriter.OnFinishWriter.AddListener((TMPWriter tMP) =>
             {
                 wonText.DOFade(0, 3).OnComplete(() =>
                 {
                     wonText.gameObject.SetActive(false);
                     wonScreen.gameObject.SetActive(true);
                     wonScreen.transform.DOScale(0, 3).OnComplete(() =>
                     {
                         SceneManager.LoadScene("MainMenu");
                     });
                 });

             });

        });

    }

    public void Intro()
    {
        GameResources.Instance.beginUI.SetFloat("_FadeAmount", 0);
        beginSreen.gameObject.SetActive(true);
        beginSreen.DOColor(new Color(0, 0, 0, 1), 3f).OnComplete(() =>
        {
            SceneManager.LoadScene("Coast", LoadSceneMode.Additive);
            saveFileSetup.GetSaveFile().AddOrUpdateData("Checkpoint", "Coast");
            saveFileSetup.GetSaveFile().Save();
            GameResources.Instance.beginUI.DOFloat(1, "_FadeAmount", 2f).OnComplete(() =>
            {
                beginSreen.gameObject.SetActive(false);

                HandleGameState(GameState.Begin);

            });
        });
    }


    public void Play()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.mass = 0.5f;

    }
    public void EngagedBoss()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.mass = 1000000f;
    }
    private void Start()
    {
        InitializeEnemyManagerData();
        //SceneManager.LoadScene("Coast", LoadSceneMode.Additive);
        //HandleGameState(GameState.Instruct);
    }

    public void InitializeEnemyManagerData()
    {
        foreach (var enemyManagerDetails in enemyManagerDetailsSO)
        {
            EnemyManagerData enemyManagerData = new EnemyManagerData(enemyManagerDetails.enemyName, enemyManagerDetails.enemyPostionList.Count, -1);
            saveFileSetup.GetSaveFile().AddOrUpdateData(enemyManagerDetails.enemyManagerDataKey, enemyManagerData);
            saveFileSetup.GetSaveFile().Save();
        }
    }

}
