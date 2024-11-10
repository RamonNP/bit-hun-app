using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using GoldenRaspberry.Models;
using UnityEngine;

public class PrivateKeyGenerate
{
    /*
        Método que valida a chave privada para descobrir se ela corresponde a um endereço desejado.
    */
    public List<Win> Run(List<string> listKeys, List<string> listAddress, bool log)
    {
        List<Win> finded = new List<Win>();
        int i = 0;
        foreach (var key in listKeys)
        {
            try
            {
                string privateKeyHex = key;
                string wifPrivateKey = KeyUtils.PrivateKeyToWIF(key); // Importa na carteira para movimentar, se der certo precisa dessa chave
                byte[] publicKeyCompressed = KeyUtils.PrivateKeyToCompressedPublicKey(privateKeyHex);
                string addressGenerated = KeyUtils.PublicKeyToLegacyCompressedAddress(publicKeyCompressed);
                if ("0000000000000000000000000000000000000000000000000000000000000001" == privateKeyHex)
                {
                    Debug.Log(privateKeyHex);
                    Debug.Log(publicKeyCompressed);
                    Debug.Log(addressGenerated);
                    //Debug.Log(address);
                }
                if ("0000000000000000000000000000000000000000000000000000000000000003" == privateKeyHex)
                {
                    Debug.Log(privateKeyHex);
                    Debug.Log(publicKeyCompressed);
                    Debug.Log(addressGenerated);
                    //Debug.Log(address);
                }

                foreach (var address in listAddress)
                {

                    if (address.Equals(addressGenerated))
                    {
                        // Chamar a API... Sucesso
                        // ENVIAR ISSO -> wifPrivateKey
                        Debug.Log($"Chave válida encontrada! WIF: {wifPrivateKey}");
                        //return; // Opcional: sair da função ao encontrar uma chave válida
                        Win win = new Win();
                        win.keyWin = wifPrivateKey;
                        win.address = address;
                        win.privateKeyHex = privateKeyHex;
                        finded.Add(win);
                        Debug.Log("CHAVE ADICIONADA");
                    }
                }
            }
            catch (CryptographicException ex)
            {
                Debug.LogError($"Erro de criptografia: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro geral: {ex.Message}");
            }
        }

        // Retorno caso não encontre a chave
        //Debug.Log("Nenhuma chave válida encontrada.");
        return finded;
    }

    public List<string> Run(Dictionary<string, string> keys)
    {
        List<string> finded = new List<string>();
        foreach (var key in keys)
        {
            try
            {
                string privateKeyHex = key.Key;
                string wifPrivateKey = KeyUtils.PrivateKeyToWIF(key.Key); // Importa na carteira para movimentar, se der certo precisa dessa chave
                byte[] publicKeyCompressed = KeyUtils.PrivateKeyToCompressedPublicKey(privateKeyHex);
                string addressGenerated = KeyUtils.PublicKeyToLegacyCompressedAddress(publicKeyCompressed);

                if (key.Value.Equals(addressGenerated))
                {
                    // Chamar a API... Sucesso
                    // ENVIAR ISSO -> wifPrivateKey
                    Debug.Log($"Chave válida encontrada! WIF: {wifPrivateKey}");
                    //return; // Opcional: sair da função ao encontrar uma chave válida
                    finded.Add(wifPrivateKey);
                }

            }
            catch (CryptographicException ex)
            {
                Debug.LogError($"Erro de criptografia: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro geral: {ex.Message}");
            }
        }

        // Retorno caso não encontre a chave
        //Debug.Log("Nenhuma chave válida encontrada.");
        return finded;
    }

    public string CheckKeys()
    {
        var listaTest = TestKeys.GetEnderecosTest(); // Método fictício, substitua pela fonte dos endereços de teste
        foreach (var item in listaTest)
        {
            try
            {
                Run(new List<string> { item.Key }, new List<string> { item.Value }, false);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erro ao verificar chaves: {ex.Message}");
            }
        }
        return "";
    }

    public List<string> ListPrivateKeys(string keyBase, int numberKeys)
    {
        var returnList = new List<string>();
        for (int i = 1; i <= numberKeys; i++)
        {
            string keyGenerated = KeyUtils.IncrementHexValue(keyBase, i);
            returnList.Add(keyGenerated);
        }
        return returnList;
    }
}
