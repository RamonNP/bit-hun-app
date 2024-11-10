using UnityEngine;
using UnityEngine.UI;
using GoldenRaspberry.Controllers;
using TMPro;
using System.Text.RegularExpressions;

namespace GoldenRaspberry.Views
{
    public class MainView : MonoBehaviour
    {
        public Button startButton;
        public Button stopButton;  // Botão de parada
        public TextMeshProUGUI resultText;

        [SerializeField] private TMP_InputField usernameInputField; // Campo para o nome de usuário

        private MainController controller;

        // Expressão regular para validar o formato UUID
        private readonly Regex uuidRegex = new Regex(@"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$");

        void Start()
        {
            controller = new MainController();
            startButton.onClick.AddListener(OnStartButtonClick);
            stopButton.onClick.AddListener(OnStopButtonClick); // Evento para parar o processamento

            // Recupera o valor armazenado no cache e preenche o campo de entrada
            string savedUsername = PlayerPrefs.GetString("Username", "");
            usernameInputField.text = savedUsername;

            // Adiciona o listener para verificar o valor do campo de entrada sempre que ele mudar
            usernameInputField.onValueChanged.AddListener(ValidateInput);
            ValidateInput(usernameInputField.text); // Executa a validação inicial
        }

        private void OnStartButtonClick()
        {
            // Obtém o texto digitado no campo de entrada e passa para o controller
            string username = usernameInputField.text;
            controller.OnStartRequest(username);

            // Armazena o nome de usuário no cache
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save(); // Garante que o valor será salvo
        }

        private void OnStopButtonClick()
        {
            controller.OnStopRequest(); // Chama o método de parada no controlador
        }

        public void UpdateResultText(string text)
        {
            resultText.text = text;
        }

        private void ValidateInput(string inputText)
        {
            // Habilita o botão de início apenas se o input for um UUID válido
            startButton.interactable = uuidRegex.IsMatch(inputText);
        }
    }
}
