# Esquemas de UI

## UI de combate (actual)
Controlada por `Assets/Game/Scripts/Combat/UI/BattleUIController.cs`.

### Jerarquía sugerida
```
Canvas (BattleUI)
├─ ActionMenuPanel
├─ TargetSelectPanel
├─ AbilityMenuPanel
├─ ItemMenuPanel
├─ OverlayPanel
└─ MessageLogText (Text)
```

### Mapeo de referencias
`BattleUIController` espera:
- `actionMenuPanel`: panel de acciones base.
- `targetSelectPanel`: panel de selección de objetivos.
- `abilityMenuPanel`: lista de habilidades.
- `itemMenuPanel`: lista de ítems.
- `overlayPanel`: overlay para estados/bloqueos.
- `messageLogText`: texto de log de combate.

## HUD de exploración (propuesto)
```
Canvas (ExplorationHUD)
├─ StatusPanel (HP/MP/Estados)
├─ QuestTracker
└─ NotificationArea
```

## Menús generales (propuesto)
```
Canvas (MainMenu)
├─ StartButton
├─ ContinueButton
├─ OptionsButton
└─ ExitButton
```

## Convenciones
- Usa nombres claros para los objetos de UI y asigna referencias por inspector.
- Mantén los paneles del combate en un único Canvas para simplificar la gestión de activación.
- Si se migra a TextMeshPro, actualizar `BattleUIController` para soportarlo.
