using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Identité")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image avatarImage;

    [Header("Barres de besoins")]
    [SerializeField] private Scrollbar hungerBar;
    [SerializeField] private Scrollbar socialBar;
    [SerializeField] private Scrollbar sleepBar;

    [Header("Inputs")]
    [SerializeField] private InputActionReference nextCharacterAction;
    [SerializeField] private InputActionReference previousCharacterAction;

    private Character[] characters;
    private int currentIndex = 0;

    private void Awake()
    {
        characters = Finder.GameController.CityObjects.Characters;
    }

    private void Start()
    {
        if (characters == null || characters.Length == 0)
            return;

        if (nextCharacterAction != null)
        {
            nextCharacterAction.action.performed += OnNextCharacter;
            nextCharacterAction.action.Enable();
        }

        if (previousCharacterAction != null)
        {
            previousCharacterAction.action.performed += OnPreviousCharacter;
            previousCharacterAction.action.Enable();
        }

        ShowCurrentCharacter();
    }

    private void Update()
    {
        UpdateCurrentCharacterUI();
    }

    private void OnDisable()
    {
        if (nextCharacterAction != null)
        {
            nextCharacterAction.action.performed -= OnNextCharacter;
            nextCharacterAction.action.Disable();
        }

        if (previousCharacterAction != null)
        {
            previousCharacterAction.action.performed -= OnPreviousCharacter;
            previousCharacterAction.action.Disable();
        }
    }

    private void OnNextCharacter(InputAction.CallbackContext context) => NextCharacter();
    private void OnPreviousCharacter(InputAction.CallbackContext context) => PreviousCharacter();

    public void NextCharacter()
    {
        if (characters == null || characters.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % characters.Length;
        ShowCurrentCharacter();
    }

    public void PreviousCharacter()
    {
        if (characters == null || characters.Length == 0)
            return;

        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        ShowCurrentCharacter();
    }

    private void ShowCurrentCharacter()
    {
        Character selectedCharacter = characters[currentIndex];

        foreach (Character character in characters)
        {
            if (character != null)
                character.HidePointer();
        }

        selectedCharacter.ShowPointer();

        avatarImage.sprite = selectedCharacter.Avatar;
        UpdateCurrentCharacterUI();
    }

    private void UpdateCurrentCharacterUI()
    {
        if (characters == null || characters.Length == 0)
            return;

        Character character = characters[currentIndex];
        if (character == null)
            return;

        string stateName = character.stateMachine.currentState != null
            ? character.stateMachine.currentState.StateName
            : "Aucun état";

        nameText.text = $"{character.FullName}\n({stateName})";

        UpdateVitals(character);
    }

    private void UpdateVitals(Character character)
    {
        CharacterVitals vitals = character.Vitals;

        hungerBar.size = Mathf.Clamp01(vitals.Hunger / 100f);
        socialBar.size = Mathf.Clamp01(vitals.Loneliness / 100f);
        sleepBar.size = Mathf.Clamp01(vitals.Sleepiness / 100f);
    }
}