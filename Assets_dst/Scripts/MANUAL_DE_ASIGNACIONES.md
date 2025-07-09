# Manual de Asignaciones de Scripts (Unity)

Este documento sirve como guía rápida para la asignación manual de scripts y componentes clave en tu proyecto. Aquí encontrarás qué scripts debes añadir manualmente a cada GameObject relevante y recomendaciones para la organización de la escena.

---

## Asignación Manual de Scripts


### 1. Player (Jugador)
- **PaperMarioPlayerMovement**: Movimiento lateral estilo Paper Mario (Nuevo Input System).
- **PaperMarioCameraController**: Cámara lateral/fija que sigue al jugador.
- **BillboardCharacter**: Hace que el personaje mire siempre a la cámara.
- **PlayerInventory**: Gestión del inventario del jugador.
- **PlayerEquipmentController**: Controla el equipamiento y auto-equipamiento de armas.
- **StaminaComponentBehaviour**: Gestión de la stamina del jugador (añadir este componente al GameObject, acceso obligatorio a través de este wrapper).
- **HealthComponentBehaviour**: Gestión de la salud del jugador (añadir este componente al GameObject, acceso obligatorio a través de este wrapper).
- **WeaponMasteryComponent**: Guarda la destreza/maestría por tipo de arma.
- **AttackComponent**: Lógica de ataque y consumo de stamina. Ahora activa/desactiva el hitbox del arma durante la animación de ataque y consume stamina usando el wrapper.

> Asegúrate de tener un `InputActionAsset` configurado y asignado en los campos `InputActionReference` de los scripts de movimiento.


### 2. Enemigos
- **Enemy**: Lógica principal del enemigo.
- **EnemyHealthController**: Control de salud y muerte del enemigo.
- **HealthComponentBehaviour**: Componente obligatorio para que la vida del enemigo funcione correctamente (nuevo sistema, acceso obligatorio a través de este wrapper).
- (Opcional) **KryptoniteDebuff**: Si el enemigo puede ser debilitado por un ítem específico del jugador.

### 3. Objetos Destruibles
- **DestructibleHealthController**: Control de salud y destrucción de objetos o NPCs simples.
- **HealthComponentBehaviour**: Obligatorio para que el daño funcione correctamente.

### 4. Armas (GameObject de arma en la escena, si aplica)
- **WeaponHitbox**: Solo si el arma tiene representación física en la escena. El prefab de hitbox se asigna en el ScriptableObject `WeaponItem` y se instancia automáticamente al equipar el arma.
- **Nota:** Las armas se gestionan exclusivamente desde ScriptableObjects y el sistema de inventario/equipamiento. No asignes scripts de arma manualmente a GameObjects salvo que implementes comportamientos avanzados.

### 5. Áreas de Daño
- **AreaDamage**: Aplica daño al entrar en el área. Usa el wrapper `HealthComponentBehaviour` para modificar la vida.
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
- El movimiento y la cámara usan el Nuevo Input System de Unity.
- **IMPORTANTE:** Todos los sistemas que modifiquen vida o stamina deben hacerlo a través de los componentes `HealthComponentBehaviour` y `StaminaComponentBehaviour`.

### Enemigos
- IA controlada por scripts. Tienen salud y pueden tener debuffs especiales.
- **IMPORTANTE:** El daño y la curación se aplican siempre a través de `HealthComponentBehaviour`.
