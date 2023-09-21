import os
import sys
import getopt
import signal
from pynput import keyboard
from gtts import gTTS
from magicsound import magicsound
import psutil

if not os.path.exists('./cache'):
    os.makedirs('./cache')

def get_python_pids(process_name):
    pid_list = []
    for process in psutil.process_iter(['pid', 'name', 'cmdline']):
        try:
            if 'python' in process.info['name'] and process_name.lower() in ' '.join(process.info['cmdline']).lower():
                pid_list.append(process.info['pid'])
        except (psutil.NoSuchProcess, psutil.AccessDenied, psutil.ZombieProcess):
            pass
    print(pid_list)
    return pid_list



# Clear caches
def clearCache():  # Fonction de suppression du cache
    for f in os.listdir('cache'):  # Parcours les fichiers du dossier cache
        os.remove(os.path.join('cache', f))  # Supprime les fichiers de cache

def terminate_process(process_name):
    this_process = os.getpid()
    try:
        pid_list = get_python_pids(process_name)
        if not pid_list:
            print("No process found.")
            return
        # Iterate through the list of PIDs and terminate each one
        for pid in pid_list:
            if pid != this_process:
                os.kill(int(pid), signal.SIGTERM)
        print(f'Terminated {process_name} successfully.')
    except Exception as e:
        print(f'An error occurred: {e}')
    os._exit(0)

def restart_process(process_name):
    try:
        pid_list = get_python_pids(process_name)
        if not pid_list:
            print("No process found.")
            return
        for pid in pid_list:
            os.kill(int(pid), signal.SIGTERM)
        print(f'Terminated {process_name} successfully.')
        clearCache()
        os.system(f'python {process_name}')
    except Exception as e:
        print(f'An error occurred: {e}')
    os._exit(0)

opts, args = getopt.getopt(sys.argv[1:], "hm:", ["mode="])

for opt, arg in opts:
    if(opt == '-m' or opt == '--mode'):
        if(arg == 'stop'):
            terminate_process('main.py')
        elif(arg == 'restart'):
            restart_process('main.py')

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

# Éxecution principale

# Configuration de la langue
langConfig = 'fr'


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
                tts = gTTS(text=keychar, lang=langConfig)
                tts.save('cache/key_'+str(unicodeKeychar)+f'{langConfig}.mp3')
                magicsound('cache/key_'+str(unicodeKeychar) +
                           f'{langConfig}.mp3', False)
                print('Key pressed: {0} (new file created)'.format(keychar))
            except:
                print('Key pressed: not suported')
    except:
        print('Key pressed: not supooorted')
        return False
    
# On release
def on_release(key):
    pass

with keyboard.Listener(on_press=on_press, on_release=on_release) as listener:
    listener.join()