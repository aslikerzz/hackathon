using System;
using System.Numerics;
using System.Security.Cryptography;

// je débute encore le C#, j'espère que mes nuits auront été suffisament rentables mdrr

class RSAImplementation
{
    private static bool IsPrime(BigInteger n)
    {
        if (n <= 1) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;

        BigInteger boundary = (BigInteger)Math.Floor(Math.Sqrt((double)n));

        for (BigInteger i = 3; i <= boundary; i += 2)
        {
            if (n % i == 0) return false;
        }
        return true;
    }

    private static BigInteger GeneratePrime(int bitLength)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            while (true)
            {
                byte[] bytes = new byte[bitLength / 8];
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] |= 0x01; // Assure que le nombre est impair
                BigInteger number = new BigInteger(bytes);

                if (IsPrime(number))
                    return number;
            }
        }
    }

    private static BigInteger CalculateE(BigInteger phi)
    {
        BigInteger e = 65537; // Valeur commune pour e (2^16 + 1)
        while (BigInteger.GreatestCommonDivisor(e, phi) != 1)
        {
            e += 2;
        }
        return e;
    }

    private static BigInteger ModInverse(BigInteger a, BigInteger n)
    {
        BigInteger t = 0, newT = 1;
        BigInteger r = n, newR = a;

        while (newR != 0)
        {
            BigInteger quotient = r / newR;
            
            BigInteger temp = t;
            t = newT;
            newT = temp - quotient * newT;

            temp = r;
            r = newR;
            newR = temp - quotient * newR;
        }

        if (r > 1)
            throw new Exception("a n'est pas inversible");
        if (t < 0)
            t += n;

        return t;
    }

    public class RSAKeyPair
    {
        public (BigInteger n, BigInteger e) PublicKey { get; set; }
        public (BigInteger n, BigInteger d) PrivateKey { get; set; }
    }

    public static RSAKeyPair GenerateKeys(int bitLength = 1024)
    {
        BigInteger p = GeneratePrime(bitLength / 2);
        BigInteger q = GeneratePrime(bitLength / 2);
        while (p == q) // Assure que p et q sont différents
            q = GeneratePrime(bitLength / 2);

        // calcul de n et phi
        BigInteger n = p * q;
        BigInteger phi = (p - 1) * (q - 1);

        // calcul e
        BigInteger e = CalculateE(phi);

        // calcul clé privée
        BigInteger d = ModInverse(e, phi);

        return new RSAKeyPair
        {
            PublicKey = (n, e),
            PrivateKey = (n, d)
        };
    }

    public static BigInteger Encrypt(string message, (BigInteger n, BigInteger e) publicKey)
    {
        // conversion du message en nombre
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        BigInteger m = new BigInteger(messageBytes.Concat(new byte[] { 0 }).ToArray());

        if (m >= publicKey.n)
            throw new Exception("Message trop long pour la clé");

        // chiffrement : c = m^e mod n
        return BigInteger.ModPow(m, publicKey.e, publicKey.n);
    }

    public static string Decrypt(BigInteger cipher, (BigInteger n, BigInteger d) privateKey)
    {
        // dechiffrement : m = c^d mod n
        BigInteger m = BigInteger.ModPow(cipher, privateKey.d, privateKey.n);

        // conversion du nombre en message
        byte[] messageBytes = m.ToByteArray();
        return System.Text.Encoding.UTF8.GetString(messageBytes.TakeWhile(b => b != 0).ToArray());
    }

    public static void Main()
    {
        try
        {
            Console.WriteLine("Génération des clés RSA...");
            var keyPair = GenerateKeys();

            Console.WriteLine("\nClé publique (n, e):");
            Console.WriteLine($"n = {keyPair.PublicKey.n}");
            Console.WriteLine($"e = {keyPair.PublicKey.e}");

            Console.WriteLine("\nClé privée (n, d):");
            Console.WriteLine($"n = {keyPair.PrivateKey.n}");
            Console.WriteLine($"d = {keyPair.PrivateKey.d}");

            // test
            string messageOriginal = "Hello, RSA!";
            Console.WriteLine($"\nMessage original: {messageOriginal}");

            // chiffrement
            BigInteger messageChiffre = Encrypt(messageOriginal, keyPair.PublicKey);
            Console.WriteLine($"Message chiffré: {messageChiffre}");

            // dehiffrement
            string messageDechiffre = Decrypt(messageChiffre, keyPair.PrivateKey);
            Console.WriteLine($"Message déchiffré: {messageDechiffre}");

            // vérification
            Console.WriteLine($"\nLe message a été correctement déchiffré: {messageOriginal == messageDechiffre}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Une erreur s'est produite: {ex.Message}");
        }
    }
}
