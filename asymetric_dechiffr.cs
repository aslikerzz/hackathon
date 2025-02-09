import React, { useState } from 'react';
import { BigInteger } from 'jsbn';
import CryptoJS from 'crypto-js';
const AsymmetricDecryption: React.FC = () => {
  const [privateKey, setPrivateKey] = useState<string>('');
  const [encryptedKey, setEncryptedKey] = useState<string>('');
  const [decryptedKey, setDecryptedKey] = useState<string>('');
  const [error, setError] = useState<string>('');
  const decryptKey = () => {
    try {
      if (!privateKey || !encryptedKey) {
        setError('Please provide both private key and encrypted key');
        return;
      }
      const privKey = new BigInteger(privateKey, 16);
      const encrypted = new BigInteger(encryptedKey, 16);
      
      const n = new BigInteger("10001", 16); // Example modulus
      const decrypted = encrypted.modPow(privKey, n);
      
      const decryptedHex = decrypted.toString(16);
      const wordArray = CryptoJS.enc.Hex.parse(decryptedHex);
      const decryptedString = CryptoJS.enc.Utf8.stringify(wordArray);
      
      setDecryptedKey(decryptedString);
      setError('');
    } catch (err) {
      setError('Decryption failed. Please check your inputs.');
      console.error(err);
    }
  };
  return (
    <div className="p-6 max-w-2xl mx-auto">
      <h1 className="text-2xl font-bold mb-6">Asymmetric Key Decryption</h1>
      
      <div className="space-y-4">
        <div>
          <label className="block text-sm font-medium mb-2">
            Private Key (Hex):
          </label>
          <input
            type="text"
            value={privateKey}
            onChange={(e) => setPrivateKey(e.target.value)}
            className="w-full p-2 border rounded"
            placeholder="Enter private key in hex format"
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-2">
            Encrypted Key (Hex):
          </label>
          <input
            type="text"
            value={encryptedKey}
            onChange={(e) => setEncryptedKey(e.target.value)}
            className="w-full p-2 border rounded"
            placeholder="Enter encrypted key in hex format"
          />
        </div>
        <button
          onClick={decryptKey}
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
        >
          Decrypt Key
        </button>
        {error && (
          <div className="text-red-500 mt-4">
            {error}
          </div>
        )}
        {decryptedKey && (
          <div className="mt-4 p-4 border rounded">
            <h2 className="font-bold mb-2">Decrypted Key:</h2>
            <p className="break-all">{decryptedKey}</p>
          </div>
        )}
      </div>
    </div>
  );
};
export default AsymmetricDecryption;
