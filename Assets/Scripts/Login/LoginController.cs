using UnityEngine;
using UnityEngine.Networking;
using GoldenRaspberry.Services;
using System.Collections;
using TMPro;

namespace GoldenRaspberry.Controllers
{
    public class LoginController : MonoBehaviour
    {
        private string loginUrl = "http://localhost:8080/auth/login";
        private ApiService apiService;

        private string payloadCache;

        [SerializeField] private TMP_InputField email;
        [SerializeField] private TMP_InputField password;

        [SerializeField] private GameObject panel;

        private void Awake()
        {
            apiService = GameObject.FindObjectOfType<ApiService>();
        }

        void Start()
        {
            string emailSave = PlayerPrefs.GetString("email", "");
            email.text = emailSave;
        }

        public void OnLoginButtonClicked()
        {
            PlayerPrefs.SetString("email", email.text);
            PlayerPrefs.Save();

            string jsonPayload = $"{{\"username\": \"{email.text}\", \"password\": \"{password.text}\"}}";
            payloadCache = jsonPayload;
            StartCoroutine(LoginRequest(jsonPayload));
        }

        private IEnumerator LoginRequest(string jsonPayload)
        {
            using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (request.GetResponseHeader("Set-Cookie") != null)
                    {
                        ApiService.JwtToken = request.GetResponseHeader("Set-Cookie");
                        //Debug.Log("Login efetuado com sucesso! Cookie JWT recebido: " + ApiService.JwtToken);
                        panel.SetActive(false);
                    }
                    else
                    {
                        Debug.LogError("Login efetuado, mas cookie JWT não encontrado na resposta.");
                    }
                }
                else
                {
                    Debug.LogError("Erro no login: " + request.error);
                }
            }
        }

        // Método para renovar o token
        public IEnumerator RefreshToken(System.Action onRefreshSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payloadCache);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (request.GetResponseHeader("Set-Cookie") != null)
                    {
                        ApiService.JwtToken = request.GetResponseHeader("Set-Cookie");
                        //Debug.Log("Token renovado com sucesso! Novo token: " + ApiService.JwtToken);
                        onRefreshSuccess?.Invoke();
                    }
                    else
                    {
                        Debug.LogError("Falha ao renovar o token, cookie JWT não encontrado.");
                        onError?.Invoke("Falha ao renovar o token.");
                    }
                }
                else
                {
                    Debug.LogError("Erro ao renovar o token: " + request.error);
                    onError?.Invoke(request.error);
                }
            }
        }
    }
}
