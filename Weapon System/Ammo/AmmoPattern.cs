using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AmmoPattern : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate the array with individual child ammo gameobjects")]
    #endregion
    [SerializeField] private Ammo[] ammoArray;

    private float m_ammoRange;
    private float m_ammoSpeed;
    private Vector3 m_fireDirectionVector;
    private float m_fireDirectionAngle;
    private AmmoDetailsSO m_ammoDetails;
    private float m_ammoChargeTimer;

    private void Update()
    {
        if (m_ammoChargeTimer > 0f)
        {
            m_ammoChargeTimer -= Time.deltaTime;
            return;
        }

        //Calculate the vector to move the ammo/bullet to
        Vector3 distanceVector = m_fireDirectionVector * m_ammoSpeed * Time.deltaTime;
        transform.position += distanceVector;

        //Rotate the ammo pattern
        transform.Rotate(new Vector3(0f, 0f, m_ammoDetails.ammoRotationSpeed * Time.deltaTime));

        //Disable the ammo/bullet after it has reached it's max range
        m_ammoRange -= distanceVector.magnitude;
        if (m_ammoRange < 0f)
        {
            DisableAmmo();
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        this.m_ammoDetails = ammoDetails;
        this.m_ammoSpeed = ammoSpeed;

        //Sets the fire direction of the ammo/bullet
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        this.m_ammoRange = ammoDetails.ammoRange;

        gameObject.SetActive(true);

        //Loop through all ammo objects and initialise them
        foreach (Ammo ammo in ammoArray)
        {
            ammo.InitialiseAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
        }

        //Set the ammo charge timer, this will hold the ammo briefly before being fired
        if (ammoDetails.ammoChargeTime > 0f)
        {
            m_ammoChargeTimer = ammoDetails.ammoChargeTime;
        }
        else
        {
            m_ammoChargeTimer = 0f;
        }
    }

    /// <summary>
    /// Set The Ammo Fire Direction Based On The Direction Adjusted By The Random Spread
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        float spreadRandomAngle = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        //Get a random toggle between 1 or -1
        int spreadRandomToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            m_fireDirectionAngle = aimAngle;
        }
        else
        {
            m_fireDirectionAngle = weaponAimAngle;
        }

        //Adjust the bullet fire angle with the random spread
        m_fireDirectionAngle += spreadRandomToggle * spreadRandomAngle;

        //Set the bullet fire direction
        m_fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(m_fireDirectionAngle);
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
    }
#endif
    #endregion
}
