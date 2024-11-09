using UnityEngine;

public class AutoMoveFlip : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento do player
    private bool movingRight = true; // Define a direção inicial do movimento

    private Rigidbody2D rb;
    [SerializeField]private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Obtém o Rigidbody2D e SpriteRenderer do player
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Define a velocidade e direção do movimento
        float moveDirection = movingRight ? 1 : -1;
        rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o player colidiu com uma parede (layer configurada como "Wall" ou com tag "Wall")
        if (collision.gameObject.CompareTag("Wall"))
        {
            Flip();
        }
    }

    void Flip()
    {
        // Inverte a direção do movimento
        movingRight = !movingRight;

        // Faz o flip do sprite, invertendo sua escala no eixo X
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
