using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Money : MonoBehaviour, IMoney
{
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;
    private MoneyDetailsSO moneyDetails;

    private double moneyValue;
    private bool isBitcoin;
    private bool isPicked; // This will avoid the same Prefab being picked twice
    private SoundEffectSO soundEffect = null;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        SetPhysicsShape();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnMoneyEvent += StaticEventHandler_OnMoneyPicked;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnMoneyEvent -= StaticEventHandler_OnMoneyPicked;
    }

    /// <summary>
    /// Money Event Handler
    /// </summary>
    private void StaticEventHandler_OnMoneyPicked(MoneyEventArgs moneyEventArgs)
    {
        moneyEventArgs.isBitcoin = isBitcoin;
        moneyEventArgs.value = moneyValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPicked) return;

        if (collision.CompareTag(Settings.playerTag))
        {
            AddMoneyToUI();

            PlaySoundEffect();

            DisableMoney();

            isPicked = true;
        }
    }

    public void InitializeMoney(MoneyDetailsSO moneyDetailsSO, double moneyValue)
    {
        this.moneyDetails = moneyDetailsSO;

        spriteRenderer.sprite = moneyDetailsSO.moneySprite;

        spriteRenderer.material = moneyDetailsSO.moneyMaterial;

        soundEffect = moneyDetailsSO.moneySoundEffect;

        isBitcoin = moneyDetailsSO.isBitcoin;

        if (moneyDetailsSO.isBitcoin)
        {
            this.moneyValue = moneyDetailsSO.moneyValue;
        }
        else
        {
            int inflation = Random.Range(0, 999);

            inflation %= 10;

            this.moneyValue = moneyDetailsSO.moneyValue + inflation;
        }

        isPicked = false;
    }

    /// <summary>
    /// Plays some good music
    /// </summary>
    private void PlaySoundEffect()
    {
        if (moneyDetails.moneySoundEffect != null)
        SoundManager.Instance.PlaySoundEffect(moneyDetails.moneySoundEffect);
    }

    /// <summary>
    /// Adds the money to the UI
    /// </summary>
    private void AddMoneyToUI()
    {
        if (isBitcoin)
        {
            StaticEventHandler.CallMoneyEvent(moneyValue, true);
        }
        else
        {
            StaticEventHandler.CallMoneyEvent(moneyValue, false);
        }
    }

    private void SetPhysicsShape()
    {
        if (polygonCollider != null && spriteRenderer.sprite != null)
        {
            //Get the sprite physics shape - this returns the sprite physics shape as a list of vectors
            List<Vector2> spritePhysicsShapePointsList = new();
            spriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            //Set the polygon collider points based on the sprite physics shape points
            polygonCollider.points = spritePhysicsShapePointsList.ToArray();
        }
    }

    /// <summary>
    /// Disables the money so it can return to the object pool
    /// </summary>
    private void DisableMoney()
    {
        gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        if (isBitcoin)
        {
            this.moneyValue = data.satsOnHold;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.satsOnHold = this.moneyValue;
    }
}
