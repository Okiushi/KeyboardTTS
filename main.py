# -*- coding: utf-8 -*-

# Installation des modules nécaissaires

import os  # Importe le modul de gestion des commandes système

try:  # Importe le modul de récupération des touches du clavier
    from pynput import keyboard
except:  # Si le module n'est pas installé, on l'installe et on l'importe
    os.system('python -m pip install pynput')
    from pynput import keyboard

try:  # Importe le modul de voix synthétique de google
    from gtts import gTTS
except:  # Si le module n'est pas installé, on l'installe et on l'importe
    os.system('python -m pip install gTTS')
    from gtts import gTTS

try:  # Importe le moodul de lecture de fichiers mp3
    from magicsound import magicsound
except:  # Si le module n'est pas installé, on l'installe et on l'importe
    os.system('python -m pip install magicsound')
    from magicsound import magicsound

# Création d'un dossier cache s'il n'existe pas
if not os.path.exists('./cache'):
    os.makedirs('./cache')

# Éxecution principale

# Configuration de la langue
langConfig = 'en'


def clearCache():  # Fonction de suppression du cache
    for f in os.listdir('cache'):  # Parcours les fichiers du dossier cache
        os.remove(os.path.join('cache', f))  # Supprime les fichiers de cache


def on_press(key):
    try:
        if key == keyboard.Key.esc:  # Si la touche est échap, on arrête le programme
            return False

        # si la touche est une touche spéciale on récupère son nom
        if str(key).split('.')[0] == 'Key':
            keychar = str(key).split('.')[1]
        else:  # Sinon on récupère le caractère simple
            keychar = key.char

            # Remplace les _ par des espaces
            keychar = keychar.replace("_", " ")

        unicodeKeychar = ''  # Variable de stockage du code unicode de la touche
        for i in keychar:  # Parcours les caractères de la touche
            # Récupère le code unicode du caractère
            unicodeKeychar += str(ord(i)) + '_'

        # Lecture du fichier correspondant à la touche
        try:  # Si le fichier existe, on le lit
            magicsound('cache/key_'+str(unicodeKeychar) +
                       f'{langConfig}.mp3', False)
            print('Key pressed: {0}'.format(keychar))
        except:  # Si le fichier n'existe pas, on le crée et on le lit
            try:
                try:
                    tts = gTTS(text=keychar, lang=langConfig)
                except Exception as e:
                    print(e)
                tts.save('cache/key_'+str(unicodeKeychar)+f'{langConfig}.mp3')
                magicsound('cache/key_'+str(unicodeKeychar) +
                           f'{langConfig}.mp3', False)
                print('Key pressed: {0} (new file created)'.format(keychar))
            except Exception as e:
                print('Error: No corresponding file for key {0})'.format(keychar))
    except:
        print('Key pressed: not supooorted')
        return False


def on_release(key):
    pass

# clearCache() # Supprime le cache au début du programme


# Collect events until released
with keyboard.Listener(on_press=on_press, on_release=on_release) as listener:
    listener.join()

# clearCache() # Supprime le cache à la fin du programme
