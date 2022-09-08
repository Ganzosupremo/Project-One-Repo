using UnityEngine;

[CreateAssetMenu(fileName = "MoneyDetails_", menuName = "Scriptable Objects/Money System/Money Details")]
public class MoneyDetailsSO : ScriptableObject
{
    #region Header Money Base Details
    [Space(10)]
    [Header("Money Base Details")]
    #endregion

    #region Tooltip
    [Tooltip("The type of money, i.e Bitcoin, fiat.")]
    #endregion 
    public string moneyType;

    #region Tooltip
    [Tooltip("The sprite for the type of money")]
    #endregion
    public Sprite moneySprite;

    #region Tooltip
    [Tooltip("The money prefab, multiple prefabs can be putted here and one will ramdomly spawn")]
    #endregion 
    public GameObject[] moneyPrefabs;

    #region Tooltip
    [Tooltip("The material for the money")]
    #endregion
    public Material moneyMaterial;

    #region Tooltip
    [Tooltip("The sound effect when the money is picked up")]
    #endregion
    public SoundEffectSO moneySoundEffect;

    #region Tooltip
    [Tooltip("The value of the money, this will be shown in the UI, a higher number means the money has less value")]
    #endregion
    public double moneyValue = 1;
    
    #region Tooltip
    [Tooltip("If the type of money is Bitcoin enable this, if it's fiat disable it")]
    #endregion
    public bool isBitcoin;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, moneyType, moneyType);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(moneyPrefabs), moneyPrefabs);
        HelperUtilities.ValidateCheckNullValue(this, nameof(moneyMaterial), moneyMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(moneySprite), moneySprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(moneySoundEffect), moneySoundEffect);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(moneyValue), moneyValue, false);
    }
#endif
    #endregion
}
