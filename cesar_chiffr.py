def chiffrement_cesar(message, cle):
    message_chiffre = ""
    
    for caractere in message:
        if caractere.isalpha():
            decalage = 65 if caractere.isupper() else 97
            lettre_chiffree = chr((ord(caractere) - decalage + cle) % 26 + decalage)
            message_chiffre += lettre_chiffree
        else:
            message_chiffre += caractere
    
    return message_chiffre

message = "Le chat gris bois de l'eau"
cle = 7

message_chiffre = chiffrement_cesar(message, cle)

print("Message original :", message)
print("Message chiffré :", message_chiffre)
