# Prodeus Model Converter

## Introduction

This is a small tool that can convert 3D models in the Wavefront (.obj) format to and from Prodeus maps (.emap).

It takes into account all the essential data like vertices, edges, texture coordinates (UV's) & faces.<br/>
But skips out on data like materials, smoothing groups etc.. These can all be edited in the Prodeus Level Editor itself.

## Why use this?

The Prodeus Level Editor is simply amazing, and what people are able to create with it equally so.<br/>
It's perfect for quickly designing playable spaces and iterating on them.

However if you want to design a small prop, having to stick to a grid and not having all the tools that modelling software provides is sometimes slightly invoncivient.<br/>
That's why I made this tool, to bring that power to Prodeus and have the best of both worlds!

Note that the Prodeus style inherrintly doesn't use many (or any) "props" in the same way as non-retro inspired titles.<br/>
Older titles often used billboards (images that rotate towards the player) for props.
But as custom textures currently can't be uploaded to the workshop, this can be a great alternative.

## How to use?

- Download the latest version from the Builds folder above
- Select an .obj or .emap file in the top textbox
  - Creating an .obj can be done with almost all 3D modelling software (Blender, 3D Max, Maya, etc...)
- Select a destination for the .obj or .emap file in the bottom textbox
  - Prodeus saves it's prefabs in C:\Users\Username\AppData\LocalLow\BoundingBoxSoftware\Prodeus\LocalData\RefMaps
- Convert!
  - Note that with a very large model this can take a couple of seconds.
- Load up your favourite 3D program or the Prodeus Level Editor & open it!

![Image of the Prodeus Model Converter](https://raw.githubusercontent.com/stijndelaruelle/prodeus_converter/main/Prodeus_Converter_Mini_Tutorial.png)

## Disclaimer

#### Prodeus spirit & art style
Even tough in theory you can convert high poly models with this tool, keep in mind that the Prodeus style is very low poly in nature.<br/>
It is advised that an entire level should only around 15k-20k faces! So only use it for low poly props or a slightly higher poly landmark like a statue in the middle of your level.

#### Copyright
Please don't use this tool to convert models that you do not own and claim them as your own.<br/>
If you do use a model that is not yours inside your level, check it's lisence. Either way, always give credit to the original creator/source!

## Bugs

If you encounter any bugs, please let me know in the Prodeus Discord! (My name there is Stijn)

Please note that the current version requires the .NET runtime 4.7.2.<br/>
On most modern computers this should be installed already, but if not check this first.

Enjoy!