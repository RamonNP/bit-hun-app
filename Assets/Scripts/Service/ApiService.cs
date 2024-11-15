using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using GoldenRaspberry.Controllers;

namespace GoldenRaspberry.Services
{
    public class ApiService : MonoBehaviour
    {
        public static string JwtToken; // Token armazenado
        public LoginController loginController;

        public IEnumerator PostRequest(string url, string jsonPayload, System.Action<string> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                // Adiciona o token JWT ao cabeçalho de autorização, se ele estiver disponível
                if (!string.IsNullOrEmpty(JwtToken))
                {
                    request.SetRequestHeader("Authorization", "Bearer " + JwtToken);
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    onSuccess?.Invoke(request.downloadHandler.text);
                }
                else if ((int)request.responseCode == 403) // Token expirado
                {
                    //Debug.LogWarning("Token expirado. Renovando o token...");
                    yield return loginController.RefreshToken(() =>
                    {
                        //Debug.Log("Token renovado. Reenviando a requisição..."+url);
                        //if(url.Contains(""))
                        StartCoroutine(PostRequest(url, jsonPayload, onSuccess, onError)); // Refaz a requisição
                    }, onError);
                }
                else
                {
                    onError?.Invoke(request.error);
                }
            }
        }
    }
}
