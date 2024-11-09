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
        private string baseUrl = "https://bit-hunt-6f29e3f40fc5.herokuapp.com";
        //private string baseUrl = "http://localhost:8080";
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

        public void OnStartRequest()
        {
            if (!isProcessing)
            {
                isProcessing = true;
                view.UpdateResultText("Processando...");
                apiService.StartCoroutine(ProcessRangeLoop());
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
                        privateKeyGenerate.Run(new List<string> { item.Key }, new List<string> { item.Value }, true);
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

        private IEnumerator ProcessRangeLoop()
        {
            //CheckKeys();

            string apiUrl = baseUrl + "/api/ranges/process-range";
            string jsonPayload = "{\"ip\" : \"127.0.0.1\", \"usertoken\": \"60600001-1a49-46de-8b98-c452f8aa3115\"}";

            while (isProcessing)
            {
                if (isCalling)
                {
                    isCalling = false;
                    // Envia a requisição
                    apiService.StartCoroutine(apiService.PostRequest(apiUrl, jsonPayload, OnRequestSuccess, OnRequestError));
                }

                // Espera 2 segundos antes da próxima iteração
                yield return new WaitForSeconds(0.2f);
            }
            //yield return new WaitForSeconds(0.2f);
        }

        private async void OnRequestSuccess(string response)
        {
            RangeProcess apiResponse = JsonUtility.FromJson<RangeProcess>(response);

            string resultText = $"Response:\n" +
                    $"ID: {apiResponse.id}\n" +
                    $"User Token: {apiResponse.userToken}\n" +
                    $"Initial Range: {apiResponse.initialRange}" +
                    $" - Valor: {new BigInteger(apiResponse.initialRange, 16)}\n" +
                    $"Quantity: {apiResponse.quantity}\n"
                    ;

            //Debug.Log(" response "+ response+" resultText "+ resultText);

            List<string> returned = await Task.Run(() =>
            {
                List<string> keys = ListPrivateKeys(apiResponse.initialRange, apiResponse.quantity, false);
                List<string> addressList = apiResponse.puzzles
                                                       .Where(p => p.status)  // Filtra apenas os itens com status true
                                                       .Select(p => p.bitcoinAddress)
                                                       .ToList();

                privateKeyGenerate = new PrivateKeyGenerate();
                List<string> findedList = privateKeyGenerate.Run(keys, addressList, apiResponse.id == 717);
                return findedList;
            });

            if (returned.Count > 0)
            {
                Debug.Log("ACHOUUUUUUUU");
                SendWinData(returned[0], apiResponse.id);
            }
            else
            {

                SendNoWinData(apiResponse);
            }

            isCalling = true;
            view.UpdateResultText(resultText);
        }

        private void OnRequestError(string error)
        {
            view.UpdateResultText("Erro: " + error);
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

        private void SendWinData(string keyWin, int rangeProcessId)
        {
            string winApiUrl = baseUrl + "/api/bit-hunt-wins/save";
            string jsonPayload = $"{{ \"keyWin\": \"{keyWin}\", \"id\": 1, \"rangeProcess\": {{ \"id\": {rangeProcessId} }} }}";

            apiService.StartCoroutine(apiService.PostRequest(winApiUrl, jsonPayload,
                success => Debug.Log("Vitória enviada com sucesso."),
                error => Debug.LogError("Erro ao enviar dados de vitória: " + error)
            ));
        }

        private void SendNoWinData(RangeProcess apiResponse)
        {
            string teste = KeyUtils.IncrementHexValueWithOutZeroLeft("f8d6", 24);

            BigInteger testeFinal = new BigInteger(teste, 16);

            //Debug.Log("finalBigInt "+teste+" final  "+testeFinal );

            apiResponse.finalRange = KeyUtils.IncrementHexValueWithOutZeroLeft(apiResponse.initialRange, apiResponse.quantity);

            BigInteger finalBigInt = new BigInteger(apiResponse.finalRange, 16);

            //string hashValidate = combinedRange;//CalculateSHA256Hash(apiResponse.finalRange);

            //Debug.Log("ID " + apiResponse.id + " Inicial: " + apiResponse.initialRange + " Final: " + apiResponse.finalRange + " Hash: " + hashValidate);

            apiResponse.finalRange = apiResponse.finalRange.TrimStart('0');
            string noWinApiUrl = baseUrl + "/api/ranges/results";
            string jsonPayload = $"{{ \"id\": {apiResponse.id}, \"userToken\": \"{apiResponse.userToken}\", " +
                                 $"\"initialRange\": \"{apiResponse.initialRange}\", \"finalRange\": \"{apiResponse.finalRange}\", " +
                                 $"\"hashValidate\": \"{finalBigInt}\", \"quantity\": {apiResponse.quantity} }}";

            Debug.Log("finalBigInt " + finalBigInt + " final  " + apiResponse.finalRange + " ----" + jsonPayload);
            apiService.StartCoroutine(apiService.PostRequest(noWinApiUrl, jsonPayload,
                success => { },
                error => Debug.LogError("Erro ao enviar dados de não-vencedor: " + error)
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
