# Mixed Reality Wall Game with HoloLens 1

## 1 INTRODUCTION

Mixed reality is a new thing in the world of video games. It makes it easy to blend the real world with the virtual world. With Microsoft's HoloLens headset, you can create very cool and interactive gaming experiences. In this project, we use these features to make a game where the player's location becomes the game area.

This game turns the player's environment into a battlefield using technology that maps the space around them. The game's idea is to use a black hole as the center, where enemies come from. The player must fight different robots that attack in different ways and eliminate them with laser shots.

To develop this project, I used important technologies like Unity to create the game, Microsoft's Mixed Reality Toolkit to interact with the real world, and AI systems to manage the enemies. The idea was to make a game that’s easy to understand, where the lines between real and virtual disappear, offering a new way to have fun.

This report explains all the steps of creating and developing the project, from choosing technologies at the start to setting up game mechanics and AI systems, including problems faced and solutions found.
## 2 DEVELOPMENT ENVIRONMENT

### 2.1 Unity 2019.4 LTS
I used Unity as the main development tool. I chose version 2019.4 LTS because it is highly compatible with HoloLens 1. This version is recommended by Microsoft for creating applications for this HoloLens model [1].

### 2.2 Mixed Reality Toolkit (MRTK)
The Mixed Reality Toolkit (MRTK) is a set of tools developed by Microsoft to help create applications for HoloLens. With MRTK, I was able to scan the environment around the player using the Spatial Mapping Awareness feature. This allowed me to adapt the game to the player’s real space, using walls, the floor, and obstacles as game elements. Additionally, MRTK made it easy to connect the HoloLens headset to Unity, simplifying the creation and integration of game elements.

### 2.3 HoloLens Hardware (1st Generation)
HoloLens (1st Generation) is a wireless holographic computer. It creates 3D holograms around you using advanced sensors and optics.
Components of HoloLens:

Visor: Contains the sensors and displays.

Headband: For adjusting the headset.

Brightness and volume buttons: Located on the sides of the device.

Device arm: For putting on and removing the HoloLens.

Micro-USB cable: For charging the device or connecting it to a computer.

Power supply: To plug the HoloLens into a power outlet [2].
![](Images/img1.png)
![](Images/img2.png)

 
