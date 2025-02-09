import React, { useState } from 'react';
import { BigInteger } from 'jsbn';

interface DiffieHellmanParameters {
  p: BigInteger;
  g: BigInteger;
  privateKey: BigInteger;
  publicKey: BigInteger;
  sharedKey: BigInteger | null;
}

const DiffieHellmanComponent: React.FC = () => {
  const [message, setMessage] = useState<string>("");

  const generateLargePrime = (bitLength: number): BigInteger => {
    const rng = window.crypto.getRandomValues(new Uint8Array(bitLength / 8));
    let number = new BigInteger(Array.from(rng).join(''));
    while (!isProbablyPrime(number)) {
      number = number.add(BigInteger.ONE);
    }
    return number;
  };

  const isProbablyPrime = (n: BigInteger): boolean => {
    if (n.compareTo(BigInteger.ONE) <= 0) return false;
    if (n.equals(new BigInteger('2'))) return true;
    if (n.mod(new BigInteger('2')).equals(BigInteger.ZERO)) return false;

    const boundary = n.sqrt();
    for (let i = new BigInteger('3'); i.compareTo(boundary) <= 0; i = i.add(new BigInteger('2'))) {
      if (n.mod(i).equals(BigInteger.ZERO)) return false;
    }
    return true;
  };

  const findGenerator = (p: BigInteger): BigInteger => {
    let g = new BigInteger('2');
    const pMinusOne = p.subtract(BigInteger.ONE);
    const halfP = pMinusOne.divide(new BigInteger('2'));

    while (g.compareTo(p) < 0) {
      if (!g.modPow(halfP, p).equals(BigInteger.ONE)) {
        return g;
      }
      g = g.add(BigInteger.ONE);
    }
    throw new Error("Generator not found");
  };

  const generateKeys = (p: BigInteger, g: BigInteger): DiffieHellmanParameters => {
    const privateKeyBytes = window.crypto.getRandomValues(new Uint8Array(32));
    const privateKey = new BigInteger(Array.from(privateKeyBytes).join(''));
    const adjustedPrivateKey = privateKey.mod(p.subtract(BigInteger.ONE)).add(BigInteger.ONE);
    const publicKey = g.modPow(adjustedPrivateKey, p);

    return {
      p,
      g,
      privateKey: adjustedPrivateKey,
      publicKey,
      sharedKey: null
    };
  };

  const calculateSharedKey = (params: DiffieHellmanParameters, otherPublicKey: BigInteger): BigInteger => {
    return otherPublicKey.modPow(params.privateKey, params.p);
  };

  const handleGenerateKeys = () => {
    try {
      const p = generateLargePrime(512);
      const g = findGenerator(p);

      const alice = generateKeys(p, g);
      const bob = generateKeys(p, g);

      const aliceSharedKey = calculateSharedKey(alice, bob.publicKey);
      const bobSharedKey = calculateSharedKey(bob, alice.publicKey);

      if (aliceSharedKey.equals(bobSharedKey)) {
        const sharedKeyStr = aliceSharedKey.toString(16);
        setMessage(`Shared key successfully generated: ${sharedKeyStr.substring(0, 32)}...`);
      } else {
        setMessage("Error: Shared keys do not match!");
      }
    } catch (error) {
      setMessage(`Error: ${error instanceof Error ? error.message : 'Unknown error occurred'}`);
    }
  };

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Diffie-Hellman Key Exchange</h1>
      <button
        onClick={handleGenerateKeys}
        className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
      >
        Generate Keys
      </button>
      {message && (
        <div className="mt-4 p-4 border rounded">
          {message}
        </div>
      )}
    </div>
  );
};

export default DiffieHellmanComponent;
