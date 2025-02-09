
using System;
using System.Numerics;
using System.Security.Cryptography;
class DiffieHellman
{
    public class DiffieHellmanParameters
    {
        public BigInteger P { get; private set; }
        public BigInteger G { get; private set; }
        public BigInteger PrivateKey { get; private set; }
        public BigInteger PublicKey { get; private set; }
        public BigInteger SharedKey { get; private set; }
        public DiffieHellmanParameters(BigInteger p, BigInteger g)
        {
            P = p;
            G = g;
            GenerateKeys();
        }
        private void GenerateKeys()
        {
            byte[] privateKeyBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(privateKeyBytes);
            }
            PrivateKey = new BigInteger(privateKeyBytes.Concat(new byte[] { 0 }).ToArray());
            PrivateKey = BigInteger.Abs(PrivateKey) % (P - 1) + 1;
            PublicKey = BigInteger.ModPow(G, PrivateKey, P);
        }
        public void CalculateSharedKey(BigInteger otherPublicKey)
        {
            SharedKey = BigInteger.ModPow(otherPublicKey, PrivateKey, P);
        }
    }
    private static BigInteger GenerateLargePrime(int bitLength)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            while (true)
            {
                byte[] bytes = new byte[bitLength / 8];
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] |= 0x01;
                BigInteger number = new BigInteger(bytes.Concat(new byte[] { 0 }).ToArray());
                if (IsProbablyPrime(number))
                    return number;
            }
        }
    }
    private static bool IsProbablyPrime(BigInteger n)
    {
        if (n <= 1) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;
        BigInteger boundary = (BigInteger)Math.Floor(Math.Sqrt((double)n));
        for (BigInteger i = 3; i <= Math.Min((double)boundary, 10000); i += 2)
        {
            if (n % i == 0) return false;
        }
        return true;
    }
    private static BigInteger FindGenerator(BigInteger p)
    {
        BigInteger g = 2;
        while (g < p)
        {
            if (BigInteger.ModPow(g, (p - 1) / 2, p) != 1)
                return g;
            g++;
        }
        throw new Exception("Générateur non trouvé");
    }
    public static void Main()
    {
        try
        {
            BigInteger p = GenerateLargePrime(512);
            BigInteger g = FindGenerator(p);
            var alice = new DiffieHellmanParameters(p, g);
            var bob = new DiffieHellmanParameters(p, g);
            alice.CalculateSharedKey(bob.PublicKey);
            bob.CalculateSharedKey(alice.PublicKey);
            bool keysMatch = alice.SharedKey.Equals(bob.SharedKey);
            if (keysMatch)
            {
                byte[] sharedKeyBytes = alice.SharedKey.ToByteArray();
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(sharedKeyBytes);
                    string finalKey = BitConverter.ToString(hashBytes).Replace("-", "");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nUne erreur s'est produite : {ex.Message}");
        }
    }
}
