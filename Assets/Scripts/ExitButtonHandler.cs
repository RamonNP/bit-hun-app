using UnityEngine;
using UnityEngine.UI;

public class ExitButtonHandler : MonoBehaviour
{
    // Campo público para o botão
    [SerializeField] private Button exitButton;

    private void Start()
    {
        // Verifica se o botão foi atribuído e adiciona o listener de clique
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClick);
        }
        else
        {
            Debug.LogWarning("Exit button not assigned in the Inspector.");
        }
    }

    // Função que será chamada ao clicar no botão
    private void OnExitButtonClick()
    {
        // Fecha a aplicação
        Application.Quit();

        // Esta linha funciona no Editor do Unity para simular o fechamento
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
