using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace GoldenRaspberry.Models
{
    public class ApiRequestModel : MonoBehaviour
    {
        private string apiUrl = "https://golden-raspberry-awards-4ff08fcfc35b.herokuapp.com/api/ranges/process-range";
        private string jsonPayload = "{\"ip\" : \"127.0.0.1\", \"usertoken\": \"Usuuiausia44dsR444R444444R\"}";

        public IEnumerator SendRequest(System.Action<string> onSuccess, System.Action<string> onError)
        {
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
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
        }
    }
}
