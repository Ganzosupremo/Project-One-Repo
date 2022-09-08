using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class HelperUtilities
{
    public static Camera mainCamera;

    /// <summary>
    /// Get The World Position Of The Mouse
    /// </summary>
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        //Clamp mouse position to the screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    public static Vector3 GetMousePosForController()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        PlayerInput playerInput = new();

        Vector2 ControllerScreenPosition = playerInput.Player.Aim.ReadValue<Vector2>();

        //Clamp mouse position to the screen size
        ControllerScreenPosition.x = Mathf.Clamp(ControllerScreenPosition.x, 0f, Screen.width);
        ControllerScreenPosition.y = Mathf.Clamp(ControllerScreenPosition.y, 0f, Screen.height);

        Vector3 worldControllerPosition = mainCamera.ScreenToWorldPoint(ControllerScreenPosition);

        worldControllerPosition.z = 0f;

        return worldControllerPosition;
    }

    /// <summary>
    /// Get the camera viewport lower and upper bounds
    /// </summary>
    public static void CameraWorldPositionBounds(out Vector2Int worldPositionLowerBounds, out Vector2Int worldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        worldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        worldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }

    /// <summary>
    /// Gets The Angle In Degrees From A Vector
    /// </summary>
    /// <returns></returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);
        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    /// <summary>
    /// Converts An Angle To A Direction Vector
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }
    
    /// <summary>
    /// Gets The AimDirection Enum Value From The Passed angleDegrees Variable
    /// </summary>
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        //Set the player direction
        //Up Right
        if (angleDegrees >= 22f && angleDegrees <= 67f)
            aimDirection = AimDirection.UpRight;
        //Up
        else if (angleDegrees > 67f && angleDegrees <= 112f)
            aimDirection = AimDirection.Up;
        //Up Left
        else if (angleDegrees > 112f && angleDegrees <= 158f)
            aimDirection = AimDirection.UpLeft;
        //Left
        else if (angleDegrees <= 180f && angleDegrees > 158f || (angleDegrees > -180f && angleDegrees <= -135f))
            aimDirection = AimDirection.Left;
        //Down
        else if (angleDegrees > -135f && angleDegrees <= -45f)
            aimDirection = AimDirection.Down;
        //Right
        else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees >= 0 && angleDegrees < 22f))
            aimDirection = AimDirection.Right;
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }

    /// <summary>
    /// Convert The Linear Volume Scale To Decibels
    /// </summary>
    public static float LinearToDecibels(int linearValue)
    {
        float linearScaleRange = 20f;

        //formula to convert from the linear scale to the logarithmic decibel scale
        return Mathf.Log10((float)linearValue / linearScaleRange) * 20f;
    }

    ///<summary>
    ///Empty String Debug Check
    ///</summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " Is Empty And Must Contain A Value In Object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// This Checks If There Are Null Values In Some Object
    /// </summary>
    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
        }

        return false;
    }

    ///<summary>
    ///List Empty Or Contains Null Value Check - Returns True If There's An Error
    ///</summary>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }


        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " Has Null Values In Object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " Has No Values In Object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    /// <summary>
    /// Positive Value Debug Check - If Zero Is Allowed Set The bool isZeroAllowed To True. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName,int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// Positive Value Float Debug Check - If Zero Is Allowed Set The bool isZeroAllowed To True. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// Positive Value Double Debug Check - If Zero Is Allowed Set The bool isZeroAllowed To True. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, double valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// Positive Range Debug Check - Set The bool isZeroAllowed To True If Both The Min And Max Range Can Be Zero. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMin, float valueToCheckMin, 
        string fieldNameMax,float valueToCheckMax,bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMin > valueToCheckMax)
        {
            Debug.Log(fieldNameMin + "must be less or equal than " + fieldNameMax + " in Object " + thisObject.name.ToString());
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMin, valueToCheckMin, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMax, valueToCheckMax, isZeroAllowed)) error = true;

        return error;
    }

    /// <summary>
    /// Positive Range Debug Check - Set The bool isZeroAllowed To True If Both The Min And Max Range Can Be Zero. Returns True If there's An Error
    /// </summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMin, int valueToCheckMin,
        string fieldNameMax, int valueToCheckMax, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMin > valueToCheckMax)
        {
            Debug.Log(fieldNameMin + "must be less or equal than " + fieldNameMax + " in Object " + thisObject.name.ToString());
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMin, (float)valueToCheckMin, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMax, (float)valueToCheckMax, isZeroAllowed)) error = true;

        return error;
    }

    /// <summary>
    /// Get The Nearest Spawn Point To The Player
    /// </summary>
    public static Vector3 GetSpawnPointNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0);

        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            //Convert the local spawn grid positions to world positions values
            Vector3 worldSpawnPosition = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            //If the distance btw worldSpawnPos and the playerPos is less than the nearesSpawnPos and playerPos, then we have a new spawn position for the player
            if (Vector3.Distance(worldSpawnPosition, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                //This is now the nearest spawn position
                nearestSpawnPosition = worldSpawnPosition;
            }
        }

        return nearestSpawnPosition;
    }
}
