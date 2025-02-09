from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from cryptography.hazmat.backends import default_backend
import os
import base64

def generer_cle():
    """Génère une clé AES de 16 octets"""
    return os.urandom(16)

def generer_iv():
    """Génère un IV de 16 octets"""
    return os.urandom(16)

def chiffrer_message(message, cle, iv):
    """Chiffre le message avec AES en mode CBC"""
    message_bytes = message.encode('utf-8')
    
    padding_length = 16 - (len(message_bytes) % 16)
    message_bytes += bytes([padding_length] * padding_length)
    
    cipher = Cipher(algorithms.AES(cle),
                   modes.CBC(iv),
                   backend=default_backend())
    encryptor = cipher.encryptor()
    
    message_chiffre = encryptor.update(message_bytes) + encryptor.finalize()
    
    return message_chiffre

def main():
    cle = generer_cle()
    iv = generer_iv()
    
    message = input("Entrez le message à chiffrer : ")
    
    message_chiffre = chiffrer_message(message, cle, iv)
    
    cle_b64 = base64.b64encode(cle).decode('utf-8')
    iv_b64 = base64.b64encode(iv).decode('utf-8')
    message_chiffre_b64 = base64.b64encode(message_chiffre).decode('utf-8')
    
    print("\nRésultats :")
    print(f"Clé (base64) : {cle_b64}")
    print(f"IV (base64)  : {iv_b64}")
    print(f"Message chiffré (base64) : {message_chiffre_b64}")

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(f"Une erreur s'est produite : {str(e)}")
