# License #
* The **images, the story and the characters** are licensed [Creative Commons Attribution-ShareAlike 4.0 International](https://creativecommons.org/licenses/by-sa/4.0/)
* The author of all the graphics is [Valentina Pantaleoni](https://www.instagram.com/topi.nambur/)


# Credits #

## Sounds ##

Weapons/Armors clashing sound Effects by Claudio Marchi

Short jingles AND Digital SFX by Kenney Vleugels (www.kenney.nl)
LICENSE: CC0 1.0 Universal (CC0 1.0)Public Domain Dedication http://creativecommons.org/publicdomain/zero/1.0/

Fantasy SFX by Little Robot Sound Factory
https://assetstore.unity.com/packages/audio/sound-fx/fantasy-sfx-32833
https://assetstore.unity.com/publishers/5673

Pterodactyl Screech, recorded by Mike Koenig
http://soundbible.com/1860-Pterodactyl-Screech.html
Uploaded: 06.24.11
LICENSE: Attribution 3.0

Fireball:
Julien Matthey on https://freesound.org/people/Julien%20Matthey/sounds/105016/
LICENSE: https://creativecommons.org/publicdomain/zero/1.0/


## Textures ##


https://unsplash.com
http://www.wildtextures.com
https://www.pexels.com
https://www.freecreatives.com
https://jooinn.com/s/license.html
http://www.freepik.com Designed by rawpixel.com/Freepik


Wood Texture by Simon Stankowski
https://unsplash.com/photos/jVVgYBLKZ5s
LICENSE: https://unsplash.com/license


Old Grunge Paper Texture
http://www.wildtextures.com/free-textures/old-grunge-paper-texture/
LICENSE: http://www.wildtextures.com/terms-of-use/


Background Brown Paper Texture
https://www.pexels.com/photo/background-brown-paper-texture-268372/
LICENSE: https://www.pexels.com/photo-license/


Ice Texture
https://www.freecreatives.com/textures/ice-texture.html
https://www.freecreatives.com/about-us


ChalkBoard Texture
https://mattiamc.deviantart.com/art/ChalkBoard-Texture-MC2015-506107812
LICENSE: CC 4.0 Attribution


# Notes #

## Touchscreen / Keyboard controls ##
There is a "TouchControls" bool in GameController which allows to switch between touchscreen (emulators-like) buttons and keyboard/gamepad controls, during runtime.

## INSTRUCTIONS TO BUILD SPLITTED MAPS IN THE EDITOR
* GROUND and OVERLAY(when available) container objects must have the passability sprite in their sprite renderers
* The spitted sub-images must be inside a folder representing their layer (Ground or Overlay) and their PIVOT center must be "TOP LEFT".
* In the Map object there is a WorldMap script in which you must specify the layers with the images folder names as "Ground" or "Overlay" and the number in which the map is splitted (for example 10 x 10 if you have splitted your map in 100 sub-images)
* The PASSABILITY sprite MUST have a sufficient max-size as to not be resized and set compression to none (you can revert this after you build the map)
* Finally click "Build Map" BUTTON in the inspector under the WorldMap Script component attached to the Map Object.
