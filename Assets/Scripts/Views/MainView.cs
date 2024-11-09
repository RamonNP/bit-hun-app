using UnityEngine;
using UnityEngine.UI;
using GoldenRaspberry.Controllers;
using TMPro;

namespace GoldenRaspberry.Views
{
    public class MainView : MonoBehaviour
    {
        public Button startButton;
        public Button stopButton;  // Botão de parada
        public TextMeshProUGUI resultText;

        private MainController controller;

        void Start()
        {
            controller = new MainController();
            startButton.onClick.AddListener(OnStartButtonClick);
            stopButton.onClick.AddListener(OnStopButtonClick); // Evento para parar o processamento
        }

        private void OnStartButtonClick()
        {
            controller.OnStartRequest();
        }

        private void OnStopButtonClick()
        {
            controller.OnStopRequest(); // Chama o método de parada no controlador
        }

        public void UpdateResultText(string text)
        {
            resultText.text = text;
        }
    }
}
