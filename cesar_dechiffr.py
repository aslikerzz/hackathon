def dechiffrement_cesar(message_chiffre, cle):
    message_dechiffre = ""
    
    for caractere in message_chiffre:
        if caractere.isalpha():
            decalage = 65 if caractere.isupper() else 97
            lettre_dechiffree = chr((ord(caractere) - decalage - cle) % 26 + decalage)
            message_dechiffre += lettre_dechiffree
        else:
            message_dechiffre += caractere
    
    return message_dechiffre

message = "Le chat gris a bu, il va dormir"
cle = 15

message_dechiffre = dechiffrement_cesar(message, cle)

print("Message original :", message)
print("Message déchiffré :", message_dechiffre)
