using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace GoldenRaspberry.Services
{
    public class ApiService : MonoBehaviour
    {
        public IEnumerator PostRequest(string url, string jsonPayload, System.Action<string> onSuccess, System.Action<string> onError)
        {
            // Alteração: Usando bloco `using` para garantir a disposição do `UnityWebRequest`
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    onSuccess?.Invoke(request.downloadHandler.text);
                }
                else
                {
                    onError?.Invoke(request.error);
                }
            } // `UnityWebRequest` será automaticamente descartado aqui devido ao `using`
        }
    }
}
