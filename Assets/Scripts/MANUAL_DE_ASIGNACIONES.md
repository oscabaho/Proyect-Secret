# Manual de Asignaciones de Scripts (Unity)

Este documento sirve como guía rápida para la asignación manual de scripts y componentes clave en tu proyecto. Aquí encontrarás qué scripts debes añadir manualmente a cada GameObject relevante y recomendaciones para la organización de la escena.

---

## Asignación Manual de Scripts

### 1. Player (Jugador)
- **Movement**: Control de movimiento en tercera persona.
- **PlayerInventory**: Gestión del inventario del jugador.
- **PlayerEquipmentController**: Controla el equipamiento y auto-equipamiento de armas.
- **StaminaComponent**: Gestión de la stamina del jugador.
- **HealthComponent**: Gestión de la salud del jugador.
- **WeaponMasteryComponent**: Guarda la destreza/maestría por tipo de arma.
- **AttackComponent**: Lógica de ataque y consumo de stamina.

### 2. Enemigos
- **Enemy**: Lógica principal del enemigo.
- **EnemyHealthController**: Control de salud y muerte del enemigo.
- (Opcional) **KryptoniteDebuff**: Si el enemigo puede ser debilitado por un ítem específico del jugador.

### 3. Objetos Destruibles
- **DestructibleHealthController**: Control de salud y destrucción de objetos o NPCs simples.

### 4. Armas (GameObject de arma en la escena, si aplica)
- **(Generalmente no requieren scripts en el GameObject, ya que las armas son ScriptableObjects y se gestionan desde el inventario y equipamiento del jugador)**

### 5. Áreas de Daño
- **AreaDamage**: Aplica daño al entrar en el área.
- **(Se añade automáticamente AreaDamageTimer a los objetos que entran, no es necesario asignarlo manualmente)**

### 6. UI y Subsistemas
- **InventoryUIUpdater**: Actualiza la UI del inventario al cambiar.
- **AchievementSystem**: Sistema de logros, suscriptor a eventos globales.
- **QuestSystem**: Sistema de misiones, suscriptor a eventos globales.

### 7. GameManager y Sistemas Globales
- **GameEventBus**: (Singleton, se crea automáticamente si es necesario)
- **GameServices**: (Singleton, se crea automáticamente si es necesario)
- **GameManager**: (Recomendado) Un GameObject vacío en la escena con scripts de inicialización global, control de flujo de escenas, etc.

---

## Descripción de Entidades Clave

### Player (Jugador)
- Controlado por el usuario. Tiene movimiento, inventario, equipamiento, stamina, salud, ataque y destreza de armas.

### Enemigos
- IA controlada por scripts. Tienen salud y pueden tener debuffs especiales.

### Objetos Destruibles
- Objetos o NPCs simples que pueden recibir daño y ser destruidos.

### Armas
- No son GameObjects en la escena, sino ScriptableObjects que definen daño, velocidad, durabilidad y curva de maestría. Se gestionan desde el inventario y equipamiento del jugador.

### Áreas de Daño
- Zonas en la escena que aplican daño a los objetos que entran en ellas.

### UI y Subsistemas
- Scripts que actualizan la interfaz de usuario y gestionan logros o misiones.

### GameManager
- GameObject vacío recomendado para centralizar lógica global, inicialización de sistemas, y persistencia entre escenas.

---

> Recuerda: Asigna manualmente solo los scripts listados arriba. Los componentes auxiliares y singletons se gestionan automáticamente por el sistema.
