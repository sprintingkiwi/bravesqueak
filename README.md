# README #
Repository for the Bravesqueak unity project: a role-playing videogame set in a fantasy world of little creatures.

## Touchscreen / Keyboard controls ##
There is a "TouchControls" bool in GameController which allows to switch between touchscreen (emulators-like) buttons and keyboard/gamepad controls, during runtime.

## INSTRUCTIONS TO BUILD SPLITTED MAPS IN THE EDITOR
* GROUND and OVERLAY(when available) container objects must have the passability sprite in their sprite renderers
* The spitted sub-images must be inside a folder representing their layer (Ground or Overlay) and their PIVOT center must be "TOP LEFT".
* In the Map object there is a WorldMap script in which you must specify the layers with the images folder names as "Ground" or "Overlay" and the number in which the map is splitted (for example 10 x 10 if you have splitted your map in 100 sub-images)
* The PASSABILITY sprite MUST have a sufficient max-size as to not be resized and set compression to none (you can revert this after you build the map)
* Finally click "Build Map" BUTTON in the inspector under the WorldMap Script component attached to the Map Object.
