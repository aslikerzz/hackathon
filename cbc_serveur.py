import requests
import base64
import sys

class PaddingOracleAttack:
    def __init__(self, url):
        self.url = url
        self.block_size = 16

    def check_padding(self, ciphertext):
        """Interroge le serveur pour vérifier si le padding est valide"""
        response = requests.post(f"{self.url}/check_padding", 
                               json={'ciphertext': base64.b64encode(ciphertext).decode()})
        return response.json()['valid_padding']

    def decrypt_block(self, prev_block, target_block):
        """Déchiffre un bloc en utilisant l'attaque padding oracle"""
        intermediate = [0] * 16
        decrypted = [0] * 16

        for pad_len in range(1, 17):
            pos = 16 - pad_len

            test_block = bytearray(prev_block)
            for i in range(16 - pad_len + 1, 16):
                test_block[i] = intermediate[i] ^ pad_len

            for guess in range(256):
                test_block[pos] = guess
                if self.check_padding(bytes(test_block) + target_block):
                    intermediate[pos] = guess ^ pad_len
                    decrypted[pos] = intermediate[pos] ^ prev_block[pos]
                    break

        return bytes(decrypted)

    def decrypt_ciphertext(self, ciphertext):
        """Déchiffre le texte complet"""
        raw_data = base64.b64decode(ciphertext)
        blocks = [raw_data[i:i+16] for i in range(0, len(raw_data), 16)]
        
        iv = blocks[0]
        ciphertext_blocks = blocks[1:]
        
        plaintext = b''
        prev_block = iv
        
        for block in ciphertext_blocks:
            decrypted_block = self.decrypt_block(prev_block, block)
            plaintext += decrypted_block
            prev_block = block

        padding_len = plaintext[-1]
        return plaintext[:-padding_len]

def main():
    server_url = "http://localhost:5000" # <= à changer pour sandbox
    
    response = requests.post(f"{server_url}/encrypt", 
                           json={'plaintext': "J'ai réussi l'attaque padding oracle!"})
    ciphertext = response.json()['ciphertext']
    
    print(f"Texte chiffré reçu: {ciphertext}")
    
    attacker = PaddingOracleAttack(server_url)
    decrypted = attacker.decrypt_ciphertext(ciphertext)
    
    print(f"Texte déchiffré: {decrypted.decode()}")
    
    response = requests.post(f"{server_url}/submit", 
                           json={'decrypted_text': decrypted.decode()})
    
    if response.json().get('success'):
        print(f"Félicitations! Flag: {response.json()['flag']}")
    else:
        print("Échec de l'attaque. Réessayez!")

if __name__ == "__main__":
    main()
