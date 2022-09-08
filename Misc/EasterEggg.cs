using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EasterEggg : MonoBehaviour, IUseable
{
    private BoxCollider2D boxCollider2D;
    private TextMeshPro textMesh;
    private PlayerInput inputActions;
    private bool pressE;
    public CurrentPlayerSO playerControl;


    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        textMesh = GetComponentInChildren<TextMeshPro>();
        inputActions = new();
    }

    private void Start()
    {
        SetText();
        pressE = inputActions.Player.Interactions.triggered;
        boxCollider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        gameObject.SetActive(true);

        if (pressE)
        {
            UseItem();
            inputActions.MainMenu.Enable();
            inputActions.Player.Disable();
        }
    }

    private void SetText()
    {
        textMesh.text = "Answer!";
    }

    public void UseItem()
    {
        MusicManager.Instance.StopMusic();

        GameManager.Instance.parentCanvas.SetActive(false);

        SceneManager.LoadScene("Test", LoadSceneMode.Additive);
    }
}
