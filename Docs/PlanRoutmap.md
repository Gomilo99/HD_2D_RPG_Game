Sí puedes usar luces 3D y 2D en el mismo proyecto, **pero no es buena idea mezclarlas en la misma cámara/flujo al inicio** (más complejidad y bugs para un perfil junior).
En tu repo ahora estás en URP con renderers forward (``Assets/Settings/PC_Renderer.asset, Mobile_Renderer.asset``), así que mi recomendación práctica es:

1. **Vertical slice: priorizar 2D Lights para sprites jugables** (Global/Point + Shadow Caster 2D + normal maps en elementos clave).
2. Mantener luces 3D solo en escenas/elementos puntuales donde realmente aporten.
3. Si luego quieres mezcla avanzada, hacerlo en una fase posterior con pipeline/cámaras separadas.

## Roadmap (orden recomendado, alcance moderado y divertido)
- [ ] Fase 0: Congelar alcance del vertical slice (1 mapa, 1 combate completo, 1 cofre, 1 tienda, 1 transición).
- [ ] Fase 1: Base técnica HD-2D (iluminación, capas visuales, presets atmósfera).
- [ ] Fase 2: Cerrar core loop end-to-end (explorar → encuentro → combate → recompensa → progreso mínimo).
- [ ] Fase 3: Progresión RPG mínima (nivel, stats, 2–3 habilidades, estados básicos).
- [ ] Fase 4: Economía mínima (loot + tienda simple + equipamiento básico).
- [ ] Fase 5: Persistencia (save/load por slot + checkpoint).
- [ ] Fase 6: UX y pulido (feedback, ritmo, balance inicial, rendimiento).
- [ ] Fase 7: Documentación técnica en español + guía de corrida en frío por sistema.

## Implementación paso a paso (por aspecto nuevo)
1) Iluminación HD-2D
    - [ ] Crear renderer 2D y usarlo en la escena del slice.
    - [ ] Configurar 3 presets: pueblo, interior, noche.
    - [ ] Añadir Shadow Caster 2D solo en arquitectura/props importantes.
    - [ ] Normal maps en sprites clave (jugador, enemigo principal, props protagonistas).
    - [ ] Validar rendimiento en PC y móvil.
2) Mundo + encuentro
    - [ ] Patrulla enemigo + persecución + trigger de combate.
    - [ ] Cofre interactuable con recompensa real.
    - [ ] NPC tienda con compra/venta mínima.
    - [ ] Transición limpia mundo↔combate↔mundo.
3) Combate (sobre tu base actual modular)
    - [ ] Mantener arquitectura modular existente (acciones, resolver, IA, UI).
    - [ ] Completar estado alterado mínimo: parálisis + veneno.
    - [ ] Preview de objetivos al seleccionar acción/objeto/habilidad.
    - [ ] Pantallas claras de victoria/derrota con flujo correcto.
4) Progresión y economía
    - [ ] Sistema de nivel para variables/personajes.
    - [ ] Tabla simple de crecimiento de stats.
    - [ ] Desbloqueo de habilidades por nivel.
    - [ ] Loot por enemigo con probabilidad fija.
    - [ ] Equipamiento que modifique stats.
5) Guardado
    - [ ] Guardar: posición, stats, habilidades, dinero, tiempo, nivel, ciudad.
    - [ ] Cargar consistente en mundo y combate.
    - [ ] Checkpoint básico y validación de corrupción de datos.
6) UX/pulido
    - [ ] Feedback visual y sonoro por acción importante.
    - [ ] Ritmo de combate (tiempos muertos mínimos).
    - [ ] Balance inicial de daño/defensa/huida.
    - [ ] Telemetría simple (tiempo de combate, derrotas, uso de habilidades).
    - [ ] Reglas de implementación (SOLID + documentación)
    - [ ] Cada sistema nuevo con responsabilidades separadas (S de SOLID).
    - [ ] Programar contra interfaces para desacoplar UI/lógica/datos.
    - [ ] Evitar clases “Dios”; dividir en servicios pequeños.
    - [ ] Documentar código en español (variables pueden quedarse en inglés).
    - [ ] Crear guía por sistema con:
        - propósito,
        - flujo de datos,
        - dependencias,
        - errores fr ecuentes,
        - **corrida en frío** (paso a paso desde input hasta resultado para depurar).
Si quieres, el siguiente paso te lo puedo dar como Sprint 1 exacto (tareas concretas de 1 semana, en orden diario) empezando por iluminación + cierre de core loop.