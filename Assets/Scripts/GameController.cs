using System;
using System.Collections;
using System.Linq;
using Characters.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    public float maxPoints = 15.0f;
    public Transform player1Pos;
    public Transform player2Pos;
    public GameObject[] playableCharacters;
    
    public CharacterBase player1Instance;
    public CharacterBase player2Instance;

    private float _player1Points = 0.0f;
    private float _player2Points = 0.0f;
    
    private const string BallTag = "Ball";
    private const string Player1ScoreTag = "Player1Score";
    private const string Player2ScoreTag = "Player2Score";
    private const string SeparatorTextTag = "SeparatorText";
    private const string InfoTextTag = "InfoText";

    private readonly Vector3 _defaultBallPosition = new(0.0f, 1.5f, -2.001f);

    private GameObject _ballInstance;

    private BasketballController _basketballController;
    
    private TextMeshProUGUI _player1ScoreInstance;
    private TextMeshProUGUI _player2ScoreInstance;
    private TextMeshProUGUI _separatorTextInstance;
    private TextMeshProUGUI _infoTextInstance;

    private GameObject _player1Character;
    private GameObject _player2Character;
    
    private const string PlayerPrefsCharacter1 = "Character1";
    private const string PlayerPrefsCharacter2 = "Character2";
    private const string SeparatorDefaultText = ":";

    private const string ScoredText = "scored for {0} points!";
    
    void Start()
    {
        _ballInstance = GameObject.FindWithTag(BallTag);

        _basketballController = _ballInstance.GetComponent<BasketballController>();
        
        _player1ScoreInstance = GameObject.FindWithTag(Player1ScoreTag).GetComponent<TextMeshProUGUI>();
        _player2ScoreInstance = GameObject.FindWithTag(Player2ScoreTag).GetComponent<TextMeshProUGUI>();
        _separatorTextInstance = GameObject.FindWithTag(SeparatorTextTag).GetComponent<TextMeshProUGUI>();
        _infoTextInstance = GameObject.FindWithTag(InfoTextTag).GetComponent<TextMeshProUGUI>();
        
        var player1CharacterName = PlayerPrefs.GetString(PlayerPrefsCharacter1);
        var player2CharacterName = PlayerPrefs.GetString(PlayerPrefsCharacter2);

        _player1Character =
            playableCharacters.Single(x => x.GetComponent<CharacterBase>().CharacterName == player1CharacterName);
        
        _player2Character =
            playableCharacters.Single(x => x.GetComponent<CharacterBase>().CharacterName == player2CharacterName);
        
        CreatePlayer1();
        CreatePlayer2();
        
    }

    public void RestartGame()
    {
        _player1Points = 0.0f;
        _player2Points = 0.0f;
        
        ResetBall();
        
        player1Instance.MoveToDefaultPosition();
        player2Instance.MoveToDefaultPosition();
        
        UpdateUi();
    }

    public void PointScored(bool isPlayer2Hoop)
    {
        var points = ScoredBonusDistance(isPlayer2Hoop)
            ? 3
            : 2;
        
        if (isPlayer2Hoop)
        {
            _player1Points += points;
        }
        else
        {
            _player2Points += points;
        }

        if (_player1Points >= maxPoints)
        {
            _infoTextInstance.text = string.Empty;
            _separatorTextInstance.text = "Player 1 won!";
            _player1ScoreInstance.gameObject.SetActive(false);
            _player2ScoreInstance.gameObject.SetActive(false);
            Destroy(_ballInstance);
        }
        
        if (_player2Points >= maxPoints)
        {
            _infoTextInstance.text = string.Empty;
            _separatorTextInstance.text = "Player 2 won!";
            _player1ScoreInstance.gameObject.SetActive(false);
            _player2ScoreInstance.gameObject.SetActive(false);
            Destroy(_ballInstance);
        }
        
        var scoredByPlayerName = isPlayer2Hoop
            ? player1Instance.CharacterName
            : player2Instance.CharacterName;

        var scoredText = string.Format($"{scoredByPlayerName} {ScoredText}", points);
        
        HandlePostGoal(scoredText);
    }

    private void HandlePostGoal(string scoredByText)
    {
        var freezeDuration = 2.5f;
        player1Instance.FreezeInput(freezeDuration);
        player2Instance.FreezeInput(freezeDuration);
        _basketballController.FreezeBall(true);

        StartCoroutine(ShowText(scoredByText, freezeDuration, ResetRound));
    }
    
    private IEnumerator ShowText(string scoredByText, float duration, Action customAction = null)
    {
        _infoTextInstance.text = scoredByText;
        
        yield return new WaitForSeconds(duration);
        
        _infoTextInstance.text = string.Empty;
        
        UpdateUi();

        customAction?.Invoke();
    }

    private void UpdateUi()
    {
        _player1ScoreInstance.text = _player1Points.ToString("F0");
        _player2ScoreInstance.text = _player2Points.ToString("F0");
    }

    private void ResetRound()
    {
        player1Instance.MoveToDefaultPosition();
        player2Instance.MoveToDefaultPosition();
        ResetBall();
    }

    private void ResetBall()
    {
        _ballInstance.transform.position = _defaultBallPosition;
        var ballRigidbody = _ballInstance.GetComponent<Rigidbody2D>();
        ballRigidbody.angularVelocity = 0;
        ballRigidbody.linearVelocity = Vector2.zero;
        _basketballController.FreezeBall(false);
    }

    private void CreatePlayer1()
    {
        var instance = Instantiate(_player1Character);
        instance.transform.position = player1Pos.position;
        player1Instance = instance.GetComponent<CharacterBase>();
        player1Instance.isSecondPlayer = false;
        player1Instance.defaultPosition = player1Pos.position;
        player1Instance.SetInput();
        player1Instance.SetSlider();
    }

    private void CreatePlayer2()
    {
        var instance = Instantiate(_player2Character);
        instance.transform.position = player2Pos.position;
        player2Instance = instance.GetComponent<CharacterBase>();
        player2Instance.isSecondPlayer = true;
        player2Instance.defaultPosition = player2Pos.position;
        player2Instance.SetInput();
        player2Instance.SetSlider();
    }

    private bool ScoredBonusDistance(bool isPlayer2Hoop)
    {
        switch (isPlayer2Hoop)
        {
            case true
                when !_basketballController.lastTouchCharacter.isSecondPlayer
                     && _basketballController.lastTouchPosition.x >= 0:
            case false
                when _basketballController.lastTouchCharacter.isSecondPlayer
                     && _basketballController.lastTouchPosition.x <= 0:
                return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}