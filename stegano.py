from PIL import Image

def extraire_message(chemin_image):
    image = Image.open(chemin_image)
    if image.mode != 'RGB':
        image = image.convert('RGB')
    
    pixels = image.load()
    largeur, hauteur = image.size
    
    message_binaire = ""
    message = ""
    compteur = 0
    
    for y in range(hauteur):
        for x in range(largeur):
            pixel = pixels[x, y]
            for valeur in pixel:
                message_binaire += str(valeur & 1)
                compteur += 1
                if compteur == 8:
                    caractere = chr(int(message_binaire, 2))
                    message += caractere
                    message_binaire = ""
                    compteur = 0
                    if caractere == '$':  # $ est utilisé comme marqueur de fin
                        return message[:-1]  
    
    return message

try:
    chemin_image = "image_avec_message.png" 
    message_cache = extraire_message(chemin_image)
    print("Message extrait:", message_cache)
except Exception as e:
    print("Une erreur s'est produite:", str(e))
