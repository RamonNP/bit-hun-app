using UnityEngine;
using GoldenRaspberry.Models;
using GoldenRaspberry.Views;
using GoldenRaspberry.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Math;
using System;


namespace GoldenRaspberry.Controllers
{
    public class MainController
    {
        //private string baseUrl = "https://bit-hunt-6f29e3f40fc5.herokuapp.com";//DEV
        //private string baseUrl = "http://localhost:8080";
        private string baseUrl = "https://bit-hunt-prod-18b1dd48caee.herokuapp.com";//PROD
        private ApiService apiService;
        private MainView view;
        private PrivateKeyGenerate privateKeyGenerate;
        private bool isProcessing;  // Controle de execução
        private bool isCalling = true;  // Controle de execução

        public MainController()
        {
            apiService = GameObject.FindObjectOfType<ApiService>();
            view = GameObject.FindObjectOfType<MainView>();
            isProcessing = false;
        }

        public void OnStartRequest(string user)
        {
            if (!isProcessing)
            {
                isProcessing = true;
                view.UpdateResultText("Processing...");
                apiService.StartCoroutine(ProcessRangeLoop(user));
            }
        }

        public string CheckKeys()
        {
            var listaTest = TestKeys.GetEnderecosTest(); // Método fictício, substitua pela fonte dos endereços de teste
            foreach (var item in listaTest)
            {
                try
                {
                    privateKeyGenerate = new PrivateKeyGenerate();
                    //List<string> findedList = privateKeyGenerate.Run(keys, addressList, apiResponse.id == 717);
                    privateKeyGenerate.Run(new List<string> { item.Key }, new List<string> { item.Value });
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Erro ao verificar chaves: {ex.Message}");
                }
            }
            return "";
        }

        public void OnStopRequest()
        {
            isProcessing = false;
            view.UpdateResultText("Processamento parado.");
        }

        private IEnumerator ProcessRangeLoop(string user)
        {
            string apiUrl = baseUrl + "/api/ranges/process-range";
            string jsonPayload = $"{{\"ip\" : \"127.0.0.1\", \"usertoken\": \"{user}\"}}";

            while (isProcessing)
            {
                if (isCalling)
                {
                    //Debug.Log("isProcessing " + isProcessing + " isCalling " + isCalling);
                    isCalling = false;

                    bool success = false;
                    bool erro = false;
                    //Debug.Log("success " + success);
                    while (!success)
                    {
                        try
                        {
                            // Tenta enviar a requisição
                            //Debug.Log("apiService.StartCoroutine ");
                            apiService.StartCoroutine(apiService.PostRequest(apiUrl, jsonPayload, OnRequestSuccess, OnRequestError));
                            success = true; // Define como sucesso se a requisição for bem-sucedida
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Erro ao processar requisição: {ex.Message}");
                            erro = true;
                        }
                        if (erro)
                        {
                            Debug.LogError($"AGUARDANDO 5 MINUTOS");
                            yield return new WaitForSeconds(1);
                            Debug.Log("Continuando");
                        }
                    }
                }

                // Espera 0.2 segundos antes da próxima iteração
                yield return new WaitForSeconds(0.2f);
            }
        }



        private async void OnRequestSuccess(string response)
        {
            RangeProcess apiResponse = JsonUtility.FromJson<RangeProcess>(response);
            view.UpdateResultText(apiResponse.userToken, apiResponse.id.ToString(), apiResponse.initialRange, apiResponse.quantity.ToString());
            view.UpdateProcessedText(apiResponse.quantity);

            //Debug.Log(" response "+ response+" resultText "+ resultText);

            List<Win> returned = await Task.Run(() =>
            {
                List<string> keys = ListPrivateKeys(apiResponse.initialRange, apiResponse.quantity, false);
                List<string> addressList = apiResponse.puzzles
                                                       //.Where(p => p.status)  // Filtra apenas os itens com status true
                                                       .Select(p => p.bitcoinAddress)
                                                       .ToList();

                // Imprime a lista de keys
                //Debug.Log("Keys: " + string.Join(", ", keys));

                // Imprime a lista de addressList
                //Debug.Log("Address List: " + string.Join(", ", addressList));
                privateKeyGenerate = new PrivateKeyGenerate();
                List<Win> findedList = privateKeyGenerate.Run(keys, addressList);
                return findedList;
            });

            if (returned.Count > 0)
            {
                Debug.Log("CHAVE ADICIONADA QTD" + returned.Count);
                Debug.Log("ACHOUUUUUUUU");
                SendWinData(returned, apiResponse.id);
            }

            SendNoWinData(apiResponse);

            isCalling = true;

            //view.UpdateResultText(apiResponse.userToken, apiResponse.id.ToString(), apiResponse.initialRange, apiResponse.quantity.ToString());
            //TODO QUANDO CHEGAR AQUI Spownar na tela um objeto que vou arratar para o campo, mostre apenas alteração

        }
        private void OnRequestError(string error)
        {
            view.UpdateResultText("Reconnecting... Erro: " + error);
            apiService.StartCoroutine(WaitAndRetry());
        }

        private IEnumerator WaitAndRetry()
        {
            // Aguarda 50 segundos antes de redefinir isCalling
            yield return new WaitForSeconds(60);
            view.UpdateResultText("Processing...");
            isCalling = true;
        }

        private List<string> ListPrivateKeys(string keyBase, int numberKeys, bool log)
        {
            var returnList = new List<string>();
            string paddedString = keyBase.PadLeft(64, '0');

            for (int i = 1; i <= numberKeys; i++)
            {
                string keyGenerated = KeyUtils.IncrementHexValue(paddedString, i);
                returnList.Add(keyGenerated);
                if (log)
                {
                    Debug.Log("Chave Gerada " + keyGenerated);
                }
            }

            return returnList;
        }

        private void SendWinData(List<Win> returned, int rangeProcessId)
        {

            foreach (var item in returned)
            {
                string winApiUrl = baseUrl + "/api/bit-hunt-wins/save";

                // Construção do JSON com os três campos
                string jsonPayload = $"{{ \"keyWin\": \"{item.keyWin}\", \"address\": \"{item.address}\", \"privateKeyHex\": \"{item.privateKeyHex}\", \"id\": 1, \"rangeProcess\": {{ \"id\": {rangeProcessId} }} }}";

                apiService.StartCoroutine(apiService.PostRequest(winApiUrl, jsonPayload,
                    success => Debug.Log("Vitória enviada com sucesso."),
                    error => Debug.LogError("Erro ao enviar dados de vitória: " + error)
                ));
            }
        }


        private void SendNoWinData(RangeProcess apiResponse)
        {
            string teste = KeyUtils.IncrementHexValueWithOutZeroLeft("f8d6", 24);

            BigInteger testeFinal = new BigInteger(teste, 16);

            apiResponse.finalRange = KeyUtils.IncrementHexValueWithOutZeroLeft(apiResponse.initialRange, apiResponse.quantity);

            BigInteger finalBigInt = new BigInteger(apiResponse.finalRange, 16);

            apiResponse.finalRange = apiResponse.finalRange.TrimStart('0');
            string noWinApiUrl = baseUrl + "/api/ranges/results";
            string jsonPayload = $"{{ \"id\": {apiResponse.id}, \"userToken\": \"{apiResponse.userToken}\", " +
                                 $"\"initialRange\": \"{apiResponse.initialRange}\", \"finalRange\": \"{apiResponse.finalRange}\", " +
                                 $"\"hashValidate\": \"{finalBigInt}\", \"quantity\": {apiResponse.quantity} }}";

            //Debug.Log("finalBigInt " + finalBigInt + " final  " + apiResponse.finalRange + " ----" + jsonPayload);
            apiService.StartCoroutine(apiService.PostRequest(noWinApiUrl, jsonPayload,
                success => { },
                error => { Debug.LogError("Erro ao enviar dados de não-vencedor: " + error); }
            ));
        }

        private string CalculateSHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);

                StringBuilder hexString = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    hexString.Append(b.ToString("x2"));
                }
                return hexString.ToString();
            }
        }
    }
}
