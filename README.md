# Stage_Camera2020

## Unity Atoms

J'ai utilisé Unity Atoms pour faire en sorte que le projet soit Data Driven et que ce soit plus facile de tout récupérer : [Unity Atoms Doc](https://unity-atoms.github.io/unity-atoms/)

C'est sympa mais ça demande une mise en place particulière donc certains trucs sont pas pratiques, les presets c'est un peu chiant vu que par exemple pour un slider il y a :
- Une valeur min et max pour les bornes (qui peut être évolutive pour les virages mais ça c'est mal fait)
- La valeur du slider

Ça fait qu'il y a 2 AtomVariable pour un preset, et grosso modo il y a une class mère : UIDataManager, utilisée pour setup les data dans les différentes inputs, ça handle :
- Slider (SliderManager)
- Input Field (InputManager)
- Toggle (ToggleManager)

Pour la sauvegarde des presets et tout ça c'est du csv classico del madrido 

## Packages et autres :

Et sinon j'ai utilisé le rail de cinemachine pour le déplacement et les caméras, DOTween et Fmod aussi, malheureusement j'avais le repo FMod en local vu qu'on était que 2 et de toute façon les sons étaient fait à l'arrache

Et pour le bonhomme et le visuel c'est Mixamo et les trucs gratuits qu'on a trouvé sur le net, amusez-vous la-dessus il y avait pas de DA et flemme de passer 10h à trouver des trucs intéressants 

## Génération du Laby

La génération du laby : c chô

Cette partie est pas terrible est pas modulaire donc c'est chiant, si vous pouvez la refaire avec des blocs ça sera sûrement plus intéressant




Cᴇᴛᴛᴇ ᴘʜʀᴀsᴇ ᴇsᴛ sᴇᴜʟᴇᴍᴇɴᴛ ᴘᴏᴜʀ ʟᴇ 100èᴍᴇ ᴄᴏᴍɪᴛ
