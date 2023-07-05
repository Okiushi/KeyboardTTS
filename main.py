# -*- coding: utf-8 -*-

### Installation des modules nécaissaires

import os # Importe le modul de gestion des commandes système

try: # Importe le modul de récupération des touches du clavier
    from pynput import keyboard
except: # Si le module n'est pas installé, on l'installe et on l'importe
    os.system('python -m pip install pynput')
    from pynput import keyboard

try: # Importe le modul de voix synthétique de google
    from gtts import gTTS
except: # Si le module n'est pas installé, on l'installe et on l'importe
    os.system('python -m pip install gTTS')
    from gtts import gTTS

try: # Importe le moodul de lecture de fichiers mp3
    from magicsound import magicsound
except: # Si le module n'est pas installé, on l'installe et on l'importe
    os.system('python -m pip install magicsound')
    from magicsound import magicsound

# Éxecution principale

# Configuration de la langue
langConfig = 'en'

def clearCache(): # Fonction de suppression du cache
    for f in os.listdir('cache'): # Parcours les fichiers du dossier cache
        os.remove(os.path.join('cache', f)) # Supprime les fichiers de cache

def on_press(key):
    if key == keyboard.Key.esc: # Si la touche est échap, on arrête le programme
        return False

    if str(key).split('.')[0] == 'Key': # si la touche est une touche spéciale on récupère son nom
        keychar = str(key).split('.')[1]
    else: # Sinon on récupère le caractère simple
        keychar = key.char

    keychar = keychar.replace("_"," ") # Remplace les _ par des espaces

    print('Key pressed: {0} '.format(keychar))

    unicodeKeychar = '' # Variable de stockage du code unicode de la touche
    for i in keychar: # Parcours les caractères de la touche
        unicodeKeychar += str(ord(i)) + '_' # Récupère le code unicode du caractère
    
    # Lecture du fichier correspondant à la touche
    try: # Si le fichier existe, on le lit
        magicsound('cache/key_'+str(unicodeKeychar)+f'{langConfig}.mp3', False)
    except: # Si le fichier n'existe pas, on le crée et on le lit
        print('audio file not found, creating it...')
        tts = gTTS(text=keychar, lang=langConfig)
        tts.save('cache/key_'+str(unicodeKeychar)+f'{langConfig}.mp3')
        magicsound('cache/key_'+str(unicodeKeychar)+f'{langConfig}.mp3', False)

def on_release(key):
    pass

# clearCache() # Supprime le cache au début du programme

# Collect events until released
with keyboard.Listener(on_press=on_press,on_release=on_release) as listener:
    listener.join()

# clearCache() # Supprime le cache à la fin du programme