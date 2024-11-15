using UnityEngine;
using UnityEngine.UI;
using GoldenRaspberry.Controllers;
using TMPro;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Collections;

namespace GoldenRaspberry.Views
{
    public class MainView : MonoBehaviour
    {
        public Button startButton;
        public Button stopButton;  // Botão de parada
        public TextMeshProUGUI status;
        public TextMeshProUGUI user;
        public TextMeshProUGUI process;
        public TextMeshProUGUI range;
        public TextMeshProUGUI count;
        public TextMeshProUGUI countProcessedText;

        public BigInteger countProcessed;

        [SerializeField] private TMP_InputField usernameInputField; // Campo para o nome de usuário

        [SerializeField]public bool isProcessing;
        [SerializeField]public bool isCalling;
        [SerializeField]public bool success;
        [SerializeField]public bool erro;

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
            status.text = text;
        }

        public void UpdateProcessedText(int count)
        {
            // Inicia a corrotina para atualizar o texto de forma gradual
            StartCoroutine(UpdateProcessedGradually(count));
        }

        // Corrotina para atualizar o texto gradualmente
        private IEnumerator UpdateProcessedGradually(int targetCount)
        {
            BigInteger targetValue = countProcessed + targetCount;

            // Atualiza o texto gradualmente até o valor final
            while (countProcessed < targetValue)
            {
                countProcessed += 1; // Incrementa o valor gradativamente
                countProcessedText.text = countProcessed.ToString();

                // Aguarda um pequeno intervalo para criar o efeito de contagem gradual
                yield return new WaitForSeconds(0.002f);
            }

            countProcessedText.text = targetValue.ToString(); // Garantir que o número final seja exibido
        }

        public void UpdateResultText(string userS, string processS, string rangeS, string countS)
        {
            user.text = "User:" + userS;
            process.text = "Process:" + processS;
            range.text = "Range:" + rangeS;
            count.text = "Count:" + countS;
        }

        public void UpdateVariables(bool isProcessingP, bool isCallingP, bool successP, bool erroP)
        {

        isProcessing = isProcessingP;
        isCalling = isCallingP;
        success = successP;
        erro = erroP;
        }


        private void ValidateInput(string inputText)
        {
            // Habilita o botão de início apenas se o input for um UUID válido
            startButton.interactable = uuidRegex.IsMatch(inputText);
        }
    }
}
