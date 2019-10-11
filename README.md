# DronOS: A Flexible Open-Source Prototyping Framework for Interactive Drone Routines

This repository contains DronOS, a rapid prototyping framework that can track, control, and automate drone routines. DronOS uses off-the-shelf components to enable the programming of custom routines for drones. Thereby, novel use cases for drones can be sketched rapidly.

## Requirements
* Unity
* HTC Vive Controller and HTC Vive Tracker (only required when prototyping routes using HTC Vive Lighthouses)
* A compatible flight controller. We have used a FrSky Taranis X9D (www.frsky-rc.com/product/taranis-x9d-plus-2) with the OpenTX controller software (www.open-tx.org).
* An ESP32 (www.espressif.com/en/products/hardware/esp32/overview) microcontroller as platform for translating the signals for the transmitter

## Installation
The folders "DronOS Unity" and "STL" contain the necessary front- and backend of the framework.

Download and import the folder "DronOS Unity" into Unity. The project will be compiled automatically after successfully importing the project.

The folder "Controller" contains the module which has to be flashed on the ESP32. A computer running DronOS communicates with the ESP32 via USB and translates the control signal into a radio frequency signal for the remote controller.

## 3D Printable Files
The folder STL contains the 3D printable files which have been used for the custom drone.

## Citing DronOS

Below are the BibTex entries to cite DronOS

```
@misc{hoppe:dronos,
  author = {Matthias Hoppe, Marinus Burger, Thomas Kosch},
  title = {DronOS},
  year = {2019},
  publisher = {GitHub},
  journal = {GitHub repository},
  howpublished = {\url{https://github.com/HCUM/dronos}}
}
```

```
@inproceedings{hoppe2019dronos, 
    title={DronOS: A Flexible Open-Source Prototyping Framework for Interactive Drone Routines}, 
    author={Hoppe, Matthias and Burger, Marinus and Schmidt, Albrecht and Kosch, Thomas}, 
    booktitle={Proceedings of the 18th International Conference on Mobile and Ubiquitous Multimedia}, 
    year={2019}, 
    doi={10.1145/3365610.3365642}, 
    organization={ACM}
}
```