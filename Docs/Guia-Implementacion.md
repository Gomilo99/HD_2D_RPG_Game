# Guía de implementación

## Estructura técnica principal
- `Assets/Game/Scripts/`
  - `PlayerController.cs`: movimiento y entrada del jugador.
  - `PlayerAnimationController.cs`: control de animaciones del jugador.
  - `Combat/`: módulos del sistema de combate.
    - `AI/`: decisiones de combate.
    - `Actions/`: acciones ejecutables (ataque, defensa, habilidades, etc.).
    - `Core/`: orquestación del combate.
    - `Data/`: definiciones de datos (ScriptableObjects).
    - `Effects/`: efectos aplicados en combate.
    - `Interfaces/`: contratos compartidos.
    - `UI/`: control de UI de combate.

## Datos de combate
Los datos de habilidades, items y estadísticas están definidos en:
- `Assets/Game/Scripts/Combat/Data/`
  - `AbilityData.cs`
  - `ItemData.cs`
  - `CharacterStats.cs`

Crea ScriptableObjects en una carpeta de datos (por ejemplo `Assets/Game/Data/`) para mantener los assets configurables.

## Input
- Archivo principal de acciones: `Assets/InputSystem_Actions.inputactions`.
- Mantén las asignaciones de input centralizadas en este archivo.

## Prefabs y escenas
- Prefabs: `Assets/Game/Prefabs/`
- Escenas: `Assets/Game/Scenes/`

## Reglas básicas de organización
- Cada sistema nuevo debe vivir bajo `Assets/Game/Scripts/<Sistema>/`.
- Evita mezclar recursos propios con terceros; todo lo externo va en `Assets/00-Thirds/`.
- Versiona cualquier asset crítico (prefabs, data, escenas) junto a su `.meta`.
