using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Asn1.X9;
using NBitcoin;
using System;


public class KeyUtilsList
{
    // Função para gerar o ponto público a partir da chave privada
    public static ECPoint PublicPointFromPrivateKey(BigInteger privateKeyInt)
    {
        System.Diagnostics.Stopwatch stopwatchCurve = System.Diagnostics.Stopwatch.StartNew();
        // Obtenha a especificação da curva secp256k1 diretamente
        var curve = ECNamedCurveTable.GetByName("secp256k1");
        stopwatchCurve.Stop();

        long timeCurve = stopwatchCurve.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"Tempo para ECNamedCurveTable.GetByName(\"secp256k1\"): {timeCurve} ms");

        // Ponto base G da curva secp256k1
        ECPoint G = curve.G;

        System.Diagnostics.Stopwatch stopwatchMultiply = System.Diagnostics.Stopwatch.StartNew();
        // Multiplique o ponto base (G) pela chave privada para obter o ponto público
        ECPoint publicKeyPoint = G.Multiply(privateKeyInt);
        stopwatchMultiply.Stop();
        long timeMultiply = stopwatchMultiply.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"Tempo para G.Multiply(privateKeyInt): {timeMultiply} ms");

        return publicKeyPoint;
    }

    // Função para converter a chave privada em uma chave pública comprimida
    public static byte[] PrivateKeyToCompressedPublicKey(string privateKeyHex)
    {
        // Converta a chave privada de hexadecimal para BigInteger
        BigInteger privateKeyInt = new BigInteger(privateKeyHex, 16);

        // Gere o ponto público da chave privada
        ECPoint publicPoint = PublicPointFromPrivateKey(privateKeyInt);

        // Codifique a chave pública comprimida (o parâmetro 'true' indica chave comprimida)
        byte[] publicKeyCompressed = publicPoint.GetEncoded(true);

        return publicKeyCompressed;
    }

    public static string IncrementHexValue(string hexValue, int increment)
    {
        // Converte o valor hexadecimal para um BigInteger decimal
        BigInteger decimalValue = new BigInteger(hexValue, 16);

        // Incrementa o valor decimal
        BigInteger incrementedValue = decimalValue.Add(BigInteger.ValueOf(increment));

        string hexResult = incrementedValue.ToString(16);

        // Converte o valor incrementado de volta para hexadecimal e formata com 64 caracteres (com zeros à esquerda)
        return hexResult.PadLeft(64, '0');
    }
    public static string IncrementHexValueWithOutZeroLeft(string hexValue, int increment)
    {
        // Converte o valor hexadecimal para BigInteger da BouncyCastle
        BigInteger decimalValue = new BigInteger(hexValue, 16);

        // Incrementa o valor
        BigInteger incrementedValue = decimalValue.Add(BigInteger.ValueOf(increment));

        // Converte o valor incrementado para hexadecimal
        string hexResult = incrementedValue.ToString(16);

        // Remove zeros à esquerda, se houver
        hexResult = hexResult.TrimStart('0');

        return hexResult;
    }


    // Função para converter uma chave privada para WIF no formato comprimido
    public static string PrivateKeyToWIF(string privateKeyHex)
    {
        // Converte a chave privada hexadecimal para um array de bytes
        byte[] privateKeyBytes = HexStringToByteArray(privateKeyHex);

        // Cria uma chave privada usando os bytes convertidos
        var privateKey = new Key(privateKeyBytes);

        // Gera a chave pública comprimida a partir da chave privada
        //var pubKey = privateKey.PubKey;

        // Cria o formato WIF com a chave privada e indica a compressão
        var wif = privateKey.GetWif(Network.Main);

        // Retorna a chave privada no formato WIF comprimido
        return wif.ToString();
    }

    // Função auxiliar para converter uma string hexadecimal em um array de bytes
    private static byte[] HexStringToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    public static string PublicKeyToLegacyCompressedAddress(byte[] publicKeyCompressed)
    {
        // Cria uma chave pública a partir dos bytes
        PubKey pubKey = new PubKey(publicKeyCompressed);

        // Obtém o endereço legível
        //BitcoinPubKeyAddress address = pubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);
        BitcoinAddress address = pubKey.GetAddress(ScriptPubKeyType.Legacy, Network.Main);

        return address.ToString();
    }

}
