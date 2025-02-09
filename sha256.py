import hashlib

message = "Le SHA-256 est une methode de hachage"

message_bytes = message.encode('utf-8')

hash_object = hashlib.sha256()

hash_object.update(message_bytes)

hash_resultat = hash_object.hexdigest()

print("Message original:", message)
print("Hash SHA-256:", hash_resultat)
