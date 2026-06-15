using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D body;
    private Vector2 movementInput;

    public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D collider2d = GetComponent<CircleCollider2D>();
        if (collider2d == null)
        {
            collider2d = gameObject.AddComponent<CircleCollider2D>();
        }
        collider2d.radius = 0.45f;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = PlaceholderSprites.Circle;
        spriteRenderer.color = new Color(0.25f, 0.75f, 1f);
        spriteRenderer.sortingOrder = 5;
    }

    private void Update()
    {
        movementInput = ReadMovementInput();
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = body.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(nextPosition);
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    private static Vector2 ReadMovementInput()
    {
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                input.y += 1f;
            }
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }
        }
#else
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
#endif

        return input.sqrMagnitude > 1f ? input.normalized : input;
    }
}
