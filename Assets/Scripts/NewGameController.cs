using System.Collections.Generic;
using System.Linq;
using Characters.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameController : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject characterSelectPrefab;
    public Button startGameButton;
    public TooltipController toolTipPanel;
    
    public List<PlayableCharacter> PlayableCharacters { get; private set; }

    private const string Player1CharacterPanelTag = "Player1CharacterPanel";
    private const string Player2CharacterPanelTag = "Player2CharacterPanel";
    
    private const string CharacterSelectionBorderTag = "CharacterSelectionBorder";
    private const string CharacterSelectionButtonTag = "CharacterSelectionButton";
    
    private const string PlayerPrefsCharacter1 = "Character1";
    private const string PlayerPrefsCharacter2 = "Character2";
    
    private GameObject _player1CharacterPanel;
    private GameObject _player2CharacterPanel;

    private GameObject _player1SelectedCharacter;
    private GameObject _player2SelectedCharacter;
    
    void Start()
    {
        _player1CharacterPanel = GameObject.FindWithTag(Player1CharacterPanelTag);
        _player2CharacterPanel = GameObject.FindWithTag(Player2CharacterPanelTag);
        
        PlayableCharacters = characters.Select(x =>
        {
            var characterBase = x.GetComponent<CharacterBase>();
            return new PlayableCharacter
            {
                Name = characterBase.CharacterName,
                Description = characterBase.CharacterDescription,
                PowerDescription = characterBase.SuperPowerDescription + $" [Cooldown: {characterBase.SuperPowerCooldown:0}s.]",
                Texture = GetPrefabSpriteTexture(x)
            };
        }).ToList();

        SetCharacterPanelsContent();
    }

    private void SetCharacterPanelsContent()
    {
        foreach (var playableCharacter in PlayableCharacters)
        {
            var characterSelectPlayer1 = Instantiate(characterSelectPrefab, _player1CharacterPanel.transform);
            var characterSelectPlayer2 = Instantiate(characterSelectPrefab, _player2CharacterPanel.transform);
            
            var player1CharacterImage = characterSelectPlayer1.GetComponent<Image>();
            var player2CharacterImage = characterSelectPlayer2.GetComponent<Image>();
            
            player1CharacterImage.sprite = Sprite.Create(
                playableCharacter.Texture,
                new Rect(0, 0, playableCharacter.Texture.width,
                    playableCharacter.Texture.height),
                new Vector2(0.5f, 0.5f));
            
            player2CharacterImage.sprite = Sprite.Create(
                playableCharacter.Texture,
                new Rect(0, 0, playableCharacter.Texture.width,
                    playableCharacter.Texture.height),
                new Vector2(0.5f, 0.5f));
            
            var player1Border = GetChildByTag(characterSelectPlayer1.transform, CharacterSelectionBorderTag);
            var player2Border = GetChildByTag(characterSelectPlayer2.transform, CharacterSelectionBorderTag);

            var player1Button = GetChildByTag(characterSelectPlayer1.transform, CharacterSelectionButtonTag);
            var player2Button = GetChildByTag(characterSelectPlayer2.transform, CharacterSelectionButtonTag);
            
            player1Border.SetActive(false);
            player2Border.SetActive(false);
            
            player1Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectCharacter(player1Border, ref _player1SelectedCharacter, characterSelectPlayer1);
                PlayerPrefs.SetString(PlayerPrefsCharacter1, playableCharacter.Name);
                PlayerPrefs.Save();
            });

            player2Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectCharacter(player2Border, ref _player2SelectedCharacter, characterSelectPlayer2);
                PlayerPrefs.SetString(PlayerPrefsCharacter2, playableCharacter.Name);
                PlayerPrefs.Save();
            });
            
            AddHoverTooltip(player1Button, playableCharacter);
            AddHoverTooltip(player2Button, playableCharacter);
        }
    }

    private void UpdateStartGameButtonState()
    {
        if (_player1SelectedCharacter != null && _player1SelectedCharacter != null)
        {
            startGameButton.interactable = true;
        }
    }
    
    private void SelectCharacter(GameObject border, ref GameObject selectedCharacter, GameObject characterSelectPrefab)
    {
        if (selectedCharacter == characterSelectPrefab) return;
        
        if (selectedCharacter != null)
        {
            var previousBorder = GetChildByTag(selectedCharacter.transform, CharacterSelectionBorderTag);
            previousBorder.SetActive(false);
        }

        selectedCharacter = characterSelectPrefab;

        border.SetActive(true);
        UpdateStartGameButtonState();
    }
    
    private Texture2D GetPrefabSpriteTexture(GameObject prefab)
    {
        var spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Prefab does not have a SpriteRenderer component!");
            return null;
        }
        
        var sprite = spriteRenderer.sprite;

        if (sprite == null)
        {
            Debug.LogError("Prefab's SpriteRenderer does not have a sprite assigned!");
            return null;
        }
        
        var texture = sprite.texture;
        
        if (!texture.isReadable)
        {
            Debug.LogError("Texture is not readable. Please enable 'Read/Write Enabled' in the texture import settings.");
            return null;
        }
        
        var readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        
        readableTexture.SetPixels(texture.GetPixels());
        readableTexture.Apply();

        return readableTexture;
    }
    
    private void AddHoverTooltip(GameObject characterButton, PlayableCharacter character)
    {
        var trigger = characterButton.GetComponent<EventTrigger>() ?? characterButton.AddComponent<EventTrigger>();
        
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener(_ => toolTipPanel.ShowTooltip(
            character.Name, 
            character.Description, 
            character.PowerDescription));
        
        trigger.triggers.Add(entryEnter);
        
        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener(_ => toolTipPanel.HideTooltip());
        trigger.triggers.Add(entryExit);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public class PlayableCharacter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PowerDescription { get; set; }
        public Texture2D Texture { get; set; }
    }
    
    private GameObject GetChildByTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        
        return null;
    }
}



