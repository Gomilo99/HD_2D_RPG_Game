# Guía de uso

## Objetivo
Esta guía explica cómo abrir el proyecto en Unity y cómo ubicar los recursos principales para trabajar rápidamente.

## Requisitos
- Unity **6000.3.6f1** (ver `ProjectSettings/ProjectVersion.txt`).
- Paquete **Input System** habilitado (ya incluido en el proyecto).

## Abrir el proyecto
1. Abre **Unity Hub**.
2. Añade la carpeta del repositorio como proyecto.
3. Abre el proyecto con la versión requerida de Unity.
4. Espera a que Unity importe paquetes y genere el cache inicial.

## Escena base
- Escena principal actual: `Assets/Game/Scenes/SampleScene.unity`.
- Para probar rápidamente: abre la escena y presiona **Play**.

## Estructura de recursos (rápido)
- **Recursos propios del juego**: `Assets/Game/`
  - Sprites: `Assets/Game/Sprites/`
  - Materiales: `Assets/Game/Materials/`
  - Prefabs: `Assets/Game/Prefabs/`
  - Animaciones: `Assets/Game/Animations/`
  - Scripts: `Assets/Game/Scripts/`
  - Escenas: `Assets/Game/Scenes/`
- **Recursos de terceros**: `Assets/00-Thirds/`

## Flujo recomendado de trabajo
1. Coloca o crea arte en `Assets/Game/Sprites/` y materiales en `Assets/Game/Materials/`.
2. Genera prefabs en `Assets/Game/Prefabs/`.
3. Usa scripts dentro de `Assets/Game/Scripts/`.
4. Mantén recursos externos en `Assets/00-Thirds/` para facilitar licencias y actualizaciones.
