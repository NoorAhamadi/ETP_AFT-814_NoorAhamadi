using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private PlayerInputActions PIA;
    private InputAction ia_Movement;
    private float movementInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float Speed;
   

    [Header("Rigid Body")]
    public Rigidbody2D rbPlayer;

    [Header("Movement Related")]
    public float maxMovementSpeed = 3f;
    public float movementSpeed = 5;
    public float JumpForce = 10;

    [Header("Cast or Check Related")]
    public Vector2 BoxSize;
    public float CastDistance = 2;

    [Header("Interaction Related")]
    public float InteractionDistance = 2;

    private void IA_InteractionStarted(InputAction.CallbackContext context)
    {
       InteractionExecution();
    }

    private void IA_JumpStarted(InputAction.CallbackContext context)
    {
        JumpExecution();
    }

    private void Awake()
    {
        PIA = new PlayerInputActions();
        ia_Movement = PIA.Walking.Movement;
        ia_Movement.Enable();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        PIA.Walking.Jump.started += IA_JumpStarted;
        PIA.Walking.Jump.Enable();

    }
  

    void Update()
    {

        movementInput = ia_Movement.ReadValue<float>();
        FlipSprite();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * CastDistance, BoxSize);
    }

    private void FixedUpdate()
    {
        if (Speed < maxMovementSpeed)
        {
            rbPlayer.AddForce(new Vector2(movementInput * movementSpeed, 0));
        }

    }

    private void FlipSprite()
    {

        if (movementInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        else if (movementInput > 0)

        { 
            spriteRenderer.flipX = false;
        }  
    
    }

    public bool IstouchingGround()
    {
    
        int mask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, BoxSize, 0, -transform.up, CastDistance, mask);
        return hit.collider != null;

    }
   

    private void JumpExecution()
    {
        if (IstouchingGround())

        {
            rbPlayer.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);

        }
    }

    private void InteractionExecution()
    {
       int mask = LayerMask.GetMask("Interaction");
       RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right,InteractionDistance, mask);
       Debug.DrawLine(transform.position, transform.position + transform.right * InteractionDistance, Color.red,1f);

        if (hit.collider != null)
        {
            Debug.Log("Interaction Succesfull Object name: " + hit.transform.name);
            if(hit.transform.GetComponent<InteractableItem>()!= null)

            {
                hit.transform.GetComponent<InteractableItem>().InitiateInteraction();
            }
            else
            {
                Debug.Log("Interaction Not succesful");
            }
        }

    }

}
