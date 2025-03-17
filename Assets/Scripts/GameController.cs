using System.Linq;
using Characters.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float maxPoints = 7.0f;
    public Transform player1Pos;
    public Transform player2Pos;
    public GameObject[] playableCharacters;

    private float _player1Points = 0.0f;
    private float _player2Points = 0.0f;
    
    private const string BallTag = "Ball";
    private const string Player1ScoreTag = "Player1Score";
    private const string Player2ScoreTag = "Player2Score";
    private const string SeparatorTextTag = "SeparatorText";

    private readonly Vector3 _defaultBallPosition = new(0.0f, 1.5f, -2.001f);

    private GameObject _ballInstance;
    private CharacterBase _player1Instance;
    private CharacterBase _player2Instance;
    
    private TextMeshProUGUI _player1ScoreInstance;
    private TextMeshProUGUI _player2ScoreInstance;
    private TextMeshProUGUI _separatorTextInstance;

    private GameObject _player1Character;
    private GameObject _player2Character;
    
    private const string PlayerPrefsCharacter1 = "Character1";
    private const string PlayerPrefsCharacter2 = "Character2";
    
    void Start()
    {
        _ballInstance = GameObject.FindWithTag(BallTag);
        
        _player1ScoreInstance = GameObject.FindWithTag(Player1ScoreTag).GetComponent<TextMeshProUGUI>();
        _player2ScoreInstance = GameObject.FindWithTag(Player2ScoreTag).GetComponent<TextMeshProUGUI>();
        _separatorTextInstance = GameObject.FindWithTag(SeparatorTextTag).GetComponent<TextMeshProUGUI>();
        
        string player1CharacterName = PlayerPrefs.GetString(PlayerPrefsCharacter1);
        string player2CharacterName = PlayerPrefs.GetString(PlayerPrefsCharacter2);

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
        
        _player1Instance.MoveToDefaultPosition();
        _player2Instance.MoveToDefaultPosition();
        
        UpdateUi();
    }

    public void PointScored(bool isPlayer2Hoop)
    {
        if (isPlayer2Hoop)
        {
            _player1Points++;
        }
        else
        {
            _player2Points++;
        }

        if (_player1Points >= maxPoints)
        {
            _separatorTextInstance.text = "Player 1 won!";
            _player1ScoreInstance.gameObject.SetActive(false);
            _player2ScoreInstance.gameObject.SetActive(false);
            Destroy(_ballInstance);
        }
        
        if (_player2Points >= maxPoints)
        {
            _separatorTextInstance.text = "Player 2 won!";
            _player1ScoreInstance.gameObject.SetActive(false);
            _player2ScoreInstance.gameObject.SetActive(false);
            Destroy(_ballInstance);
        }

        ResetBall();
        
        _player1Instance.MoveToDefaultPosition();
        _player2Instance.MoveToDefaultPosition();

        UpdateUi();
    }

    private void UpdateUi()
    {
        _player1ScoreInstance.text = _player1Points.ToString("F0");
        _player2ScoreInstance.text = _player2Points.ToString("F0");
    }

    private void ResetBall()
    {
        _ballInstance.transform.position = _defaultBallPosition;
        var ballRigidbody = _ballInstance.GetComponent<Rigidbody2D>();
        ballRigidbody.angularVelocity = 0;
        ballRigidbody.linearVelocity = Vector2.zero;
    }

    private void CreatePlayer1()
    {
        var instance = Instantiate(_player1Character);
        instance.transform.position = player1Pos.position;
        _player1Instance = instance.GetComponent<CharacterBase>();
        _player1Instance.isSecondPlayer = false;
        _player1Instance.defaultPosition = player1Pos.position;
        _player1Instance.SetInput();
        _player1Instance.SetSlider();
    }

    private void CreatePlayer2()
    {
        var instance = Instantiate(_player2Character);
        instance.transform.position = player2Pos.position;
        _player2Instance = instance.GetComponent<CharacterBase>();
        _player2Instance.isSecondPlayer = true;
        _player2Instance.defaultPosition = player2Pos.position;
        _player2Instance.SetInput();
        _player2Instance.SetSlider();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}