from collections import Counter
import re

FREQ_FR = {
    'e': 14.7, 'a': 7.6, 'i': 7.5, 's': 7.9, 'n': 7.1, 'r': 6.5, 't': 7.2,
    'o': 5.4, 'l': 5.5, 'u': 6.3, 'd': 3.7, 'c': 3.3, 'm': 2.9, 'p': 2.9,
    'g': 1.0, 'b': 0.9, 'v': 1.6, 'h': 0.7, 'f': 1.1, 'q': 1.4, 'y': 0.3,
    'x': 0.4, 'j': 0.3, 'k': 0.1, 'w': 0.1, 'z': 0.1
}

def nettoyer_texte(texte):
    """Nettoie le texte en gardant uniquement les lettres et en les mettant en minuscule"""
    return re.sub(r'[^a-zA-Z]', '', texte.lower())

def calculer_frequences(texte):
    """Calcule les fréquences des lettres dans le texte"""
    total = len(texte)
    compteur = Counter(texte)
    return {char: (count / total) * 100 for char, count in compteur.items()}

def trouver_correspondances(freq_texte):
    """Trouve les correspondances probables entre les lettres chiffrées et les lettres françaises"""
    correspondances = {}
    freq_triees = sorted(freq_texte.items(), key=lambda x: x[1], reverse=True)
    freq_fr_triees = sorted(FREQ_FR.items(), key=lambda x: x[1], reverse=True)
    
    for (lettre_chiffree, _), (lettre_fr, _) in zip(freq_triees, freq_fr_triees):
        correspondances[lettre_chiffree] = lettre_fr
    
    return correspondances

def dechiffrer(texte_chiffre, correspondances):
    """Déchiffre le texte en utilisant les correspondances"""
    texte_dechiffre = ""
    for char in texte_chiffre:
        if char.isalpha():
            texte_dechiffre += correspondances.get(char.lower(), char)
        else:
            texte_dechiffre += char
    return texte_dechiffre

def main():
    texte_chiffre = input("Entrez le texte chiffré (200-275 caractères) : ")
    
    if not (200 <= len(texte_chiffre) <= 275):
        print("Le texte doit contenir entre 200 et 275 caractères.")
        return
    
    texte_nettoye = nettoyer_texte(texte_chiffre)
    
    frequences = calculer_frequences(texte_nettoye)

    correspondances = trouver_correspondances(frequences)
    
    texte_dechiffre = dechiffrer(texte_chiffre, correspondances)
    
    print("\nAnalyse des fréquences :")
    for char, freq in sorted(frequences.items()):
        print(f"'{char}': {freq:.2f}%")
    
    print("\nCorrespondances proposées :")
    for chiffre, clair in correspondances.items():
        print(f"'{chiffre}' → '{clair}'")
    
    print("\nTexte déchiffré :")
    print(texte_dechiffre)
    
    while True:
        choix = input("\nVoulez-vous modifier une correspondance ? (o/n) : ")
        if choix.lower() != 'o':
            break
            
        ancien = input("Entrez la lettre chiffrée à modifier : ").lower()
        nouveau = input("Entrez la nouvelle correspondance : ").lower()
        
        if ancien in correspondances:
            correspondances[ancien] = nouveau
            print("\nNouveau texte déchiffré :")
            print(dechiffrer(texte_chiffre, correspondances))

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(f"Une erreur s'est produite : {str(e)}")
