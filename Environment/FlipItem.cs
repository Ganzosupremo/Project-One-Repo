using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class FlipItem : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("The mass of the item, this control how fast the item moves when dragged around")]
    #endregion
    [SerializeField] private float itemMass;

    private BoxCollider2D m_boxCollider2D;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody2D;
    private bool m_itemUsed = false;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_boxCollider2D = GetComponent<BoxCollider2D>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void UseItem()
    {
        if (!m_itemUsed)
        {
            //Get the bounds
            Bounds bounds = m_boxCollider2D.bounds;

            //Calculate the closest point to the player with the collider bounds
            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());

            //If the player is on the right, flip left
            if (closestPointToPlayer.x == bounds.max.x)
            {
                m_animator.SetBool(Settings.flipLeft, true);
            }
            //If the player is on the left, flip right
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                m_animator.SetBool(Settings.flipRight, true);
            }
            //If the player is below, flip up
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                m_animator.SetBool(Settings.flipUp, true);
            }
            //If the player is up, flip down
            else if (closestPointToPlayer.y == bounds.max.y)
            {
                m_animator.SetBool(Settings.flipDown, true);
            }

            //Set the layer of the item to the environment layer. so the item now collides with the bullets
            gameObject.layer = LayerMask.NameToLayer("Environment");

            //Set the mass of the item to the specified one, so the player can move the item around
            m_rigidbody2D.mass = itemMass;

            SoundManager.Instance.PlaySoundEffect(GameResources.Instance.flipTableSoundEffect);

            m_itemUsed = true;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(itemMass), itemMass, false);
    }
#endif
    #endregion
}
