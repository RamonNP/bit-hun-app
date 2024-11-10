using UnityEngine;
using TMPro;

public class PlayerCoinCollector : MonoBehaviour
{
    // Variável para armazenar a quantidade de moedas coletadas
    private int coinCount = 0;

    // Referência ao TextMeshPro para exibir a quantidade de moedas
    [SerializeField] private TextMeshProUGUI coinText;

    private void Start()
    {
        // Inicializa o texto com a quantidade inicial de moedas
        UpdateCoinText();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        // Verifica se o player colidiu com uma parede (layer configurada como "Wall" ou com tag "Wall")
        if (collision.gameObject.CompareTag("Coin"))
        {
            // Incrementa o contador de moedas
            coinCount++;

            // Atualiza o texto na interface
            UpdateCoinText();

            // Destroi o objeto "Coin" para simular a coleta
            Destroy(collision.gameObject);
        }
         

    }

    // Método para atualizar o texto da interface com a quantidade de moedas
    private void UpdateCoinText()
    {
        coinText.text = "Coins: " + coinCount;
    }
}
