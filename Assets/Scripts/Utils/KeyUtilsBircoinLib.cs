/*using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Asn1.X9;
using NBitcoin;
using System;




public class KeyUtilsBircoinLib
{
    // Função para gerar o ponto público a partir da chave privada
    public static ECPoint PublicPointFromPrivateKey(BigInteger privateKeyInt)
    {
        // Obtenha a especificação da curva secp256k1 diretamente
        var curve = ECNamedCurveTable.GetByName("secp256k1");

        // Ponto base G da curva secp256k1
        ECPoint G = curve.G;

        // Multiplique o ponto base (G) pela chave privada para obter o ponto público
        ECPoint publicKeyPoint = G.Multiply(privateKeyInt);

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
        var pubKey = privateKey.PubKey;

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

/*
     static void Main(string[] args)
    {
        // Sua chave privada em formato hexadecimal
        string privateKeyHex = "SUA_CHAVE_PRIVADA_HEX_AQUI";

        // Converte a chave privada hexadecimal para um array de bytes
        byte[] privateKeyBytes = HexStringToByteArray(privateKeyHex);

        // Adiciona o prefixo 0x80 para indicar que é uma chave privada para a mainnet
        byte[] extendedKey = new byte[privateKeyBytes.Length + 1];
        extendedKey[0] = 0x80;
        Buffer.BlockCopy(privateKeyBytes, 0, extendedKey, 1, privateKeyBytes.Length);

        // Calcula o checksum (SHA256 duplo)
        byte[] checksum = CalculateChecksum(extendedKey);

        // Combina a chave estendida com o checksum
        byte[] extendedKeyWithChecksum = new byte[extendedKey.Length + 4];
        Buffer.BlockCopy(extendedKey, 0, extendedKeyWithChecksum, 0, extendedKey.Length);
        Buffer.BlockCopy(checksum, 0, extendedKeyWithChecksum, extendedKey.Length, 4);

        // Converte para o formato Base58
        string wif = Base58CheckEncoding.Encode(extendedKeyWithChecksum);

        Console.WriteLine($"Chave Privada (Hex): {privateKeyHex}");
        Console.WriteLine($"Chave Privada (WIF): {wif}");
    }

    static byte[] HexStringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    static byte[] CalculateChecksum(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash1 = sha256.ComputeHash(data);
            byte[] hash2 = sha256.ComputeHash(hash1);
            return hash2.Take(4).ToArray();
        }
    }
    
} */
