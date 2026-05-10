# Sistema de Iluminación HD-2D — Guía técnica

## Propósito
Definir y transicionar entre presets de atmósfera de iluminación para el juego,
aprovechando URP con Volume de post-proceso para lograr el estilo HD-2D.

---

## Estrategia de iluminación recomendada

El proyecto usa **URP Forward (3D)** como pipeline base. Para sprites HD-2D se recomienda:

1. **Luces 3D para el entorno**: luz direccional como "sol", luces puntuales para antorchas y magia.
2. **Global Light 2D / Point Light 2D (opcional)**: si se migra a un renderer 2D, dan sombras
   sobre sprites con `Shadow Caster 2D` y normal maps.
3. **Post-proceso (Volume)**: Bloom, Viñeta y Color Grading para cohesión visual.
4. **Normal maps en sprites clave**: jugador, enemigo protagonista, props importantes.

> **Nota:** No mezclar el renderer 3D y el 2D en la misma cámara al inicio.
> Migrar a renderer 2D en una fase posterior cuando el equipo tenga más experiencia.

---

## Componentes clave

| Clase | Rol |
|---|---|
| `LightingPresetData` | ScriptableObject con los parámetros de un preset de atmósfera |
| `AtmosphereManager` | Singleton que aplica y transiciona entre presets |

---

## Presets recomendados para el vertical slice

### Pueblo (día)
```
globalLightColor:    (1.0, 0.95, 0.8, 1) — blanco cálido
ambientColor:        (0.3, 0.28, 0.22)
fogEnabled:          false
colorTemperature:    +15
saturation:          +10
bloomEnabled:        false
vignetteEnabled:     false
transitionDuration:  1.0
```

### Interior (edificios, cueva)
```
globalLightColor:    (0.6, 0.55, 0.4, 1) — naranja tenue (antorcha)
ambientColor:        (0.1, 0.08, 0.05)
fogEnabled:          true   fogDensity: 0.02
colorTemperature:    +25
saturation:          -10
bloomEnabled:        true   threshold: 0.85   intensity: 0.3
vignetteEnabled:     true   intensity: 0.4    smoothness: 0.3
transitionDuration:  0.8
```

### Noche
```
globalLightColor:    (0.3, 0.35, 0.6, 1) — azul nocturno
ambientColor:        (0.05, 0.05, 0.12)
fogEnabled:          true   fogDensity: 0.015
colorTemperature:    -30
saturation:          -20
bloomEnabled:        true   threshold: 0.75   intensity: 0.2
vignetteEnabled:     true   intensity: 0.5    smoothness: 0.25
transitionDuration:  1.5
```

---

## Cómo configurar en la escena

1. Crar 3 assets de `LightingPresetData` (Pueblo, Interior, Noche).
2. Crear un `Volume Global` en la escena:
   - Modo: Global
   - Añadir overrides: **Color Adjustments**, **Bloom**, **Vignette**
3. Crear un GameObject y añadir `AtmosphereManager`:
   - Asignar los 3 presets en los campos correspondientes.
   - Asignar el Volume Global.
4. Para cambiar de atmósfera en código:
   ```csharp
   AtmosphereManager.Instance.LoadPresetNoche();
   // O con referencia directa:
   AtmosphereManager.Instance.LoadPreset(miPresetPersonalizado);
   ```

---

## Corrida en frío — Cambio de atmósfera

```
1. El jugador entra en la cueva (trigger de zona)
2. El script de zona llama AtmosphereManager.Instance.LoadPresetInterior()

3. AtmosphereManager.LoadPreset(presetInterior)
   ├─ presetInterior ≠ presetActual → iniciar transición
   └─ StartCoroutine(TransicionarPreset(presetInterior))

4. TransicionarPreset (durante 0.8s)
   ├─ Cada frame:
   │   ├─ RenderSettings.ambientLight = Lerp(origen, destino.ambientColor, t)
   │   ├─ bloomOverride.intensity.value = Lerp(0, 0.3, t)
   │   └─ vignetteOverride.intensity.value = Lerp(0, 0.4, t)
   └─ Al finalizar: AplicarInstantaneo(presetInterior)
```

---

## Normal maps en sprites (opcional avanzado)

Para añadir profundidad a los sprites con iluminación 3D:
1. En el Sprite Renderer, cambiar el material a `Sprites/Lit`.
2. Crear un normal map para el sprite (en Photoshop, GIMP o Sprite Illuminator).
3. Asignar el normal map en el material.
4. La luz 3D del entorno creará sombreado en el sprite.

---

## Errores frecuentes

| Error | Causa probable | Solución |
|---|---|---|
| Los overrides de post-proceso no cambian | Volume Global no asignado en AtmosphereManager | Asignar el Volume en el Inspector |
| Preset no carga | El preset asignado es el mismo que el actual | `LoadPreset()` no hace nada si el preset ya es el activo |
| La transición se corta bruscamente | `transitionDuration = 0` en el preset | Asignar un valor > 0 en el ScriptableObject |
| Color Grading no responde | El override de Color Adjustments no está activo en el Volume | Activar el override en el Volume y marcar cada parámetro como "override" |
