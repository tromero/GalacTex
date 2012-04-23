GalacTex
========

A procedural texture renderer for XNA Game Studio

GalacTex is a GPU-accelerated texture renderer, inspired by the "Texture Bake" tools in Blender. With it, you can create compact procedural textures using HLSL that are rendered onto your models at run-time.

To use it, simply create your models in the 3D program of your choice, unwrap their texture coordinates (or UVs), and load it through XNA's Content Pipeline. Write your procedural texture in HLSL, using the included HLSL functions as a starting point.