# Preguntas para Mejorar el Game-Design

## Resumen
Este documento sirve de guía mental para mejorar ideas, refinar experiencias, corregir problemas estructurales y establecer las bases de diseño que van a regir el proyecto.

### Preproducción
- ¿Cuál es la fantasía central del jugador?

- ¿Qué 3 pilares de diseño NO se negocian?
    1. El jugador debe poder modificar su estrategia de combate dependiendo del enemigo o la situación en la que se encuentre.
    2. El jugador puede explorar en buscar de objetos dispersos por el mundo e interactuar con otros personajes ya sea para comprar o vender objetos de su inventario.
    3. El jugador mejora sus habilidades y su equipo conforme sube de nivel y adquiere nuevas habilidades.

- ¿Qué contenido cabe en un vertical slice realista?
    1. Escenario pueblo donde haya:
        - Casas, iglesia, una fuente en el medio del pueblo, caminos de piedra.
        - Efectos visuales para hojas que caigan
        - 2 Enemigos
        - 1 cofre de objeto
        - 1 npc de tienda
    2. Los enemigos deben tener:
        - animacion de movimiento, ataque, muerte.
        - Movimiento de un punto a otro.
        - Movimiento perseguidor cuando detecta al jugador.
    3. Las variables (personajes del juego controlados por el jugador) tienen:
        - Características básicas: Nombre, descripción, tipo
        - Sprite y animaciones
        - Nivel actual
        - Estadísiticas: Cordura (vida), Inteligencia (ataque), Memoria (defensa), rapidez (velocidad), fealdad (suerte).
        - Funciones (Habilidades): cada conjunto de funciones a invocar es único para cada variable, puede aprender más funciones en un futuro.
        - Equipamiento: un objeto que modifica algún aspecto de la variable, ya sea una estadística o modifica un parametro cuando invoca a una función, como mayor daño o mas tiempo de buffeo.
    4. El jugador (el usuario) tiene:
        - Un conjunto de variables.
        - Un inventario con objetos de consumo (en batalla y fuera) y equipamientos.
        - Dinero.
    5. Sistema de combate dónde esté:
        - Uso de habilidades: Ataque, Buffeo, Debuffeo, Curación, Estados alterados.
        - Estados alterados: Parálisis, envenenado.
        - Ataque básico: hace daño basado en inteligencia: cordura -= daño; daño = inteligencia - (memoria / 2)
        - Defender: eleva la memoria (defensa) un valor maximo entre 1 y memoria/2
        - Huir: para huir de un combate se lanza una número que menor que: fealdad (suerte) * 0.01f (valor que convierte en prob base 100) + probabilidad_base_enemigo
    6. Escena de combate
        - Se inicia cuando se colisiona con un enemigo
        - Se cargan todos las variables del jugador (personajes).
        - Se cargan todos los enemigos (intrinseco al enemigo y pueden ser 1 o más)
        - Animaciones de introucción a combate
        - Efectos visuales de ataque.
        - Selección inteligente con preview de enemigos cuando se selecciona un ataque, objeto o se invoca a una función (habilidad).
        - Selección inteligente con preview de variables (personajes del jugador) cuando se selecciona una invocación o un objeto.
        - Visualización de estado de los personajes y enemigos con: vida, actual y restante con una barra que actualice dinámicamente con una bara gris que visualice que porcentaje de vida se es reducido y se desvanezca.
        - Pantalla de victoria y derrota.
        - Si es victoria se regresa al mundo.
        - Si es derrota se establece opción de reiniciar la batalla o volver a menú principal (sin guardar).
    5. Sistema de Guardado.
        - Posición del jugador.
        - Estadísticas de las variables (personajes).
        - Habilidades de cada variable.
        - Dinero
        - Tiempo de juego.
        - Nivel
        - Ciudad donde se encuentra el jugador.
    6. Sistema de niveles
        - Cada variable (personaje) puede subir de nivel.
        - En cada nivel se suman puntos a las estadísticas.
        - En ciertos niveles se desbloquean nuevas funciones (habilidades).
    7. Sistema de Looteo
        - Cada enemigo puede soltar una serie de objetos al morir, puede soltar un objeto entre su lista con una probabilidad fija.
        - Los objetos looteables pueden ser consumibles o de equipamiento.
    8. Sistema de Equipamiento
        - Existen objetos (de equipamiento) que, al equiparse a una variable pueden mejorar una o muchas estadísticas en un número determinado.
        - Existen otros objetos (opcionales) que pueden tener efectos adicionles como aumentar el efecto o el tiempo de buff/debuff en la invocación de una función (habilidad).
        - Tanto los objetos consumibles como los de equipamiento tienen un valor (dinero) asociado con el que se pueden vender.
    9. Sistema de cofres, los cofres deben tener
        - Uno o varios objetos que es el drop, definidos por el programador.
        - Animación, VFX de particulas, luces, etc.

- ¿Qué riesgo técnico/artístico puede tumbar el proyecto?
    1. Los principales riesgos del proyecto es el tiempo, debido a que la universidad puede opacar el desarrollo del videojuego y aplazar todos las metas, volviendo mas laxo y difícil de retomar.
        - Lo principal en la primera parte es completar un vertical slice con la mayor cantidad de sistemas interados. Que sea jugable de principio a fin.
    2. Por la parte técnica, el riesgo principal es la dificultad de desarrollo y entendimiento de los sistemas internos del juego, la arquitectura de diseño, la implementación de nuevos sistemas y la depuración.
    3. El reto artístico es uno de los más grandes, debido a la falta de experiencia y estudios en creación de sprites 2d y modelos 3d necesarios para todos los aspectos del proyecto. Eso significa que se requiriría de un equipo o de alguna persona que pueda crear arte para el proyecto.

### Producción

¿Cada feature mejora el core loop o lo distrae?

¿Estamos midiendo dificultad, ritmo y tiempo de sesión?

¿Esta tarea agrega deuda técnica o la reduce?

¿El arte/UI/audio mantiene coherencia de tono?

### Postproducción (cada hito):

¿Qué funcionó, qué no, y por qué?

¿Qué feedback de jugadores se repite?

¿Qué recortar, pulir o escalar en el siguiente sprint?

¿Se cumplieron objetivos de calidad/rendimiento?

# Extras

## ¿Qué es un vertical slice?
Es un pedazo pequeño del juego que ya está jugable de principio a fin, con calidad cercana a la final, pero con muy poco contenido. Sirve para validar el “core loop”.

Ejemplo de vertical slice para tu RPG:
- 1 escenario pequeño
- 1 combate completo (entrada, turno, UI, daño, victoria)
- 1 enemigo
- 1 interacción simple (cofre, diálogo)
- Música + sonido básico
- Transición de mundo a combate y vuelta

Cómo hacerlo:
- Define el **core loop** (explorar → encontrar enemigo → combate → recompensa).
- Haz **solo lo mínimo** para que eso funcione end‑to‑end.
- Pule ese pedazo como si fuera final (UI, feedback, sonido).
- No agregues más contenido, solo calidad.

## Sistemas clave a montar (prioridad)
- **Core loop**: Exploración → Encuentro → Combate → Recompensa → Progresión (ya está en tu GDD).
- **Combate**: mantener modular (acciones, efectos, IA, resolver, UI), como ya tienes en ``Assets/Game/Scripts/Combat/``.
- **Progresión RPG**: stats, niveles, habilidades, estados, equipo.
- **Economía**: loot, tienda, crafting básico.
- **Narrativa**: quests + diálogos + flags de mundo.
- **Persistencia**: save/load por slots + checkpoints.
- **Mundo**: interacción, triggers, eventos, transición de escenas.
- **UX transversal**: feedback visual/sonoro, accesibilidad, telemetría básica.

## Iluminación y sombras para ambiente 2D HD (URP 2D)
- Base con **Global Light 2D** suave (día/noche por bioma/escena).
- Luces de acento con **Point/Freeform Light 2D** (interiores, antorchas, magia).
- **Shadow Caster 2D** en arquitectura/props relevantes (no en todo, por rendimiento/ruido visual).
- Usar **normal maps** en sprites clave (personajes, props importantes) para look HD-2D.
- Separar capas visuales (fondo, mid, gameplay, foreground) y ajustar iluminación por capa.
- Añadir postproceso leve (color grading/viñeta/bloom mínimo) para cohesión, sin “lavar” pixel-art.
- Definir 2–3 “presets de atmósfera” reutilizables (pueblo, cueva, noche) para consistencia.

## Pipeline de trabajo / Gitflow recomendado
- Para equipo pequeño: **trunk-based** + **PRs cortas** (mejor que gitflow clásico pesado).
- Ramas: ``main`` (estable), ``feature/<sistema>``, ``fix/<bug>``, ``chore/<mantenimiento>``.
- PR pequeña por objetivo concreto + checklist de validación.
- Convención Unity:
    - escenas/prefabs versionados con ``.meta``,
    - evitar cambios simultáneos en la misma escena grande,
- preferir prefabs y escenas aditivas para reducir conflictos.
- Definir “Definition of Done”: jugable, sin errores críticos, test manual mínimo, docs actualizadas.