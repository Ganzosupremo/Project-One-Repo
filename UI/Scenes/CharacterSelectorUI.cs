using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[DisallowMultipleComponent]
public class CharacterSelectorUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate this with the child CharacterSelector gameobject")]
    #endregion
    [SerializeField] private Transform characterSelector;

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro component on the PlayerNameInput gameobject")]
    #endregion
    [SerializeField] private TMP_InputField playerNameInput;

    private List<PlayerDetailsSO> m_playerDetailsList;
    private GameObject m_playerSelectionPrefab;
    private CurrentPlayerSO m_currentPlayer;
    private List<GameObject> m_playerCharacterGameObjectList = new List<GameObject>();
    private Coroutine m_coroutine;
    private int m_selectedPlayerIndex = 0;
    private float m_offset = 4f;

    private void Awake()
    {
        // Load resources
        m_playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
        m_playerDetailsList = GameResources.Instance.playerDetailsList;
        m_currentPlayer = GameResources.Instance.currentPlayer;
    }

    private void Start()
    {
        // Instatiate player characters
        for (int i = 0; i < m_playerDetailsList.Count; i++)
        {
            GameObject playerSelectionObject = Instantiate(m_playerSelectionPrefab, characterSelector);
            m_playerCharacterGameObjectList.Add(playerSelectionObject);
            playerSelectionObject.transform.localPosition = new Vector3((m_offset * i), 0f, 0f);
            PopulatePlayerDetails(playerSelectionObject.GetComponent<PlayerSelectionUI>(), m_playerDetailsList[i]);
        }

        playerNameInput.text = m_currentPlayer.playerName;

        // Initialise the current player
        m_currentPlayer.playerDetails = m_playerDetailsList[m_selectedPlayerIndex];

    }

    /// <summary>
    /// Populate player character details for display
    /// </summary>
    private void PopulatePlayerDetails(PlayerSelectionUI playerSelection, PlayerDetailsSO playerDetails)
    {
        playerSelection.playerHandSpriteRenderer.sprite = playerDetails.playerHandSprite;
        playerSelection.playerHandNoWeapon.sprite = playerDetails.playerHandSprite;
        playerSelection.weaponSpriteRenderer.sprite = playerDetails.initialWeapon.weaponSprite;
        playerSelection.animator.runtimeAnimatorController = playerDetails.runtimeAnimatorController;
    }

    /// <summary>
    /// Select next character - this method is called from the onClick event set in the inspector
    /// </summary>
    public void NextCharacter()
    {
        if (m_selectedPlayerIndex >= m_playerDetailsList.Count - 1)
            return;
        m_selectedPlayerIndex++;

        m_currentPlayer.playerDetails = m_playerDetailsList[m_selectedPlayerIndex];

        MoveToSelectedCharacter(m_selectedPlayerIndex);
    }


    /// <summary>
    /// Select previous character - this method is called from the onClick event set in the inspector
    /// </summary>
    public void PreviousCharacter()
    {
        if (m_selectedPlayerIndex == 0)
            return;

        m_selectedPlayerIndex--;

        m_currentPlayer.playerDetails = m_playerDetailsList[m_selectedPlayerIndex];

        MoveToSelectedCharacter(m_selectedPlayerIndex);
    }


    private void MoveToSelectedCharacter(int index)
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(MoveToSelectedCharacterRoutine(index));
    }

    private IEnumerator MoveToSelectedCharacterRoutine(int index)
    {
        float currentLocalXPosition = characterSelector.localPosition.x;
        float targetLocalXPosition = index * m_offset * characterSelector.localScale.x * -1f;

        while (Mathf.Abs(currentLocalXPosition - targetLocalXPosition) > 0.01f)
        {
            currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, targetLocalXPosition, Time.deltaTime * 10f);

            characterSelector.localPosition = new Vector3(currentLocalXPosition, characterSelector.localPosition.y, 0f);
            yield return null;
        }

        characterSelector.localPosition = new Vector3(targetLocalXPosition, characterSelector.localPosition.y, 0f);
    }

    /// <summary>
    /// Update player name - this method is called from the field changed event set in the inspector
    /// </summary>
    public void UpdatePlayerName()
    {
        playerNameInput.text = playerNameInput.text.ToUpper();

        m_currentPlayer.playerName = playerNameInput.text;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(characterSelector), characterSelector);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerNameInput), playerNameInput);
    }
#endif
    #endregion
}
