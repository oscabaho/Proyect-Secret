# Manual de Uso de ScriptableObjects Específicos

> **¡IMPORTANTE!** Este manual está pensado para personas con poca o ninguna experiencia en Unity. Aquí aprenderás desde cero cómo crear, ubicar y asignar ScriptableObjects en tu proyecto, con ejemplos visuales y consejos para evitar errores comunes.

> **NOTA:** Todos los sistemas de movimiento y cámara del proyecto usan el Nuevo Input System de Unity. Si creas ScriptableObjects que interactúan con el movimiento o input, asegúrate de usar `InputActionReference` y no el sistema clásico.

---

## Glosario Básico
- **Script:** Archivo de código (C#) que define lógica o datos.
- **ScriptableObject:** Un tipo especial de asset (archivo) que almacena datos y lógica reutilizable, editable desde el Editor de Unity.
- **Asset:** Cualquier archivo que aparece en el panel `Project` de Unity (imágenes, sonidos, scripts, ScriptableObjects, etc.).
- **Inspector:** Panel a la derecha donde puedes ver y editar las propiedades de un asset o GameObject seleccionado.
- **Arrastrar y soltar:** Acción de hacer clic en un asset y moverlo a un campo del Inspector para asignarlo.

---

## ¿Cómo se ve y se crea un ScriptableObject?
1. **Ubica la carpeta** donde quieres crear el asset (por ejemplo, `Assets/ScriptableObjects/Weapons/`).
2. **Haz clic derecho** en la carpeta y selecciona la opción correspondiente (por ejemplo, `Crear > Inventory > WeaponItem`).
3. **Nombra el asset** (ejemplo: `EspadaBasica`).
4. **Selecciona el asset** recién creado y verás sus campos editables en el Inspector.

**Ejemplo visual:**
- ![Ejemplo de creación de ScriptableObject](https://docs.unity3d.com/uploads/Main/ScriptableObjectInspector.png)

---

## ¿Cómo asignar un ScriptableObject?
1. Selecciona el GameObject (por ejemplo, el jugador) en la jerarquía de la escena.
2. Busca el componente que tiene un campo para ScriptableObject (por ejemplo, `PlayerInventory`).
3. **Arrastra el asset** desde el panel `Project` al campo correspondiente en el Inspector.
4. ¡Listo! El ScriptableObject está asignado y listo para usarse en el juego.

---

## ¿Cómo probar que funciona?
- Si es un arma, inicia el juego y verifica que el jugador pueda equiparla y atacar. El hitbox del arma se instanciará automáticamente y solo se activará durante el ataque.
- Si es una poción, úsala y observa si la vida del jugador aumenta.
- Si es un ítem misterioso, úsalo y revisa si revela su tipo o efecto.

---

## Errores frecuentes y cómo evitarlos
- **No puedes arrastrar el asset:** Asegúrate de que el campo en el Inspector sea del tipo correcto (por ejemplo, `WeaponItem`).
- **No aparece la opción de crear el asset:** El script debe tener `[CreateAssetMenu]`.
- **El asset no aparece en el juego:** Verifica que esté asignado en el Inspector y que todos los campos estén completos.
- **NullReferenceException:** El asset no está asignado o el campo está vacío.
- **El arma no hace daño o no colisiona:** Asegúrate de que el prefab de hitbox asignado en el `WeaponItem` tenga un componente `WeaponHitbox` y un colisionador configurado como `Trigger`.

---

# Manual Específico de ScriptableObjects

Este manual explica cómo crear, asignar y usar cada ScriptableObject de este proyecto, con ejemplos, funciones y resolución de errores frecuentes. Ideal para principiantes en Unity.

---

## 1. WeaponItem
- **¿Qué es?** Define los datos base de un arma (daño, velocidad, durabilidad, curvas de desgaste y maestría, y prefab de hitbox).
- **Cómo crearlo:**
  - Haz clic derecho en la carpeta deseada y selecciona `Crear > Inventory > WeaponItem`.
  - Asigna el prefab de hitbox en el campo `Weapon Hitbox Prefab` (debe tener el componente `WeaponHitbox`).
  - Completa los campos de daño, velocidad, durabilidad, etc.
- **Cómo usarlo:**
  - El arma se equipa desde el inventario y su hitbox se instancia automáticamente.
  - El hitbox solo se activa durante el ataque (controlado por el sistema de animación o el `AttackComponent`).
- **Ejemplo de flujo:**
  1. El jugador equipa el arma desde el inventario.
  2. Al atacar, el sistema activa el hitbox del arma.
  3. Si el hitbox colisiona con un enemigo, se llama a `ApplyDamage` del `WeaponItem`.
- **Consejo:** No es necesario añadir scripts de arma manualmente a GameObjects. Todo se gestiona desde el ScriptableObject y el sistema de inventario/equipamiento.

---

## 2. HealingItem
- **¿Qué es?** Ítem de curación que puede usarse desde el inventario.
- **Cómo crearlo:**
  1. Clic derecho en `Assets/ScriptableObjects/Items/`.
  2. Crear > Inventory > HealingItem.
  3. Nombra el asset (ej: `PocionPequena`).
- **Dónde asignarlo:**
  - En `initialItems` de `PlayerInventory`.
  - En `ItemDatabase`.
- **Función:** Al usarlo, ejecuta `Use` y cura al jugador.
- **Errores frecuentes:**
  - No cura: asset no asignado o método `Use` no llamado.
  - No aparece en inventario: no fue agregado a la lista o no fue recogido.

---

## 3. MysteryItem
- **¿Qué es?** Ítem misterioso, su tipo real se revela al usarlo.
- **Cómo crearlo:**
  1. Clic derecho en `Assets/ScriptableObjects/Items/`.
  2. Crear > Inventory > MysteryItem.
  3. Nombra el asset (ej: `CajaSorpresa`).
- **Dónde asignarlo:**
  - En inventario inicial o como recompensa.
  - En `ItemDatabase`.
- **Función:** Se muestra como misterioso, al usarlo puede revelar su tipo o ejecutar un efecto.
- **Errores frecuentes:**
  - No revela su tipo: método de uso no implementado o UI no actualizada.
  - No se puede usar: no implementa `IUsableItem`.

---

## 4. CombatTransferData
- **¿Qué es?** ScriptableObject para transferir datos temporales entre escenas de combate.
- **Cómo crearlo:**
  1. Clic derecho en `Assets/ScriptableObjects/`.
  2. Crear > Combat > CombatTransferData.
  3. Nombra el asset (ej: `CombatTransferData`).
- **Dónde asignarlo:**
  - En `CombatSceneLoader` y `CombatSceneInitializer`.
- **Función:** Permite transferir referencias de prefabs y datos entre escenas.
- **Errores frecuentes:**
  - No transfiere datos: asset no asignado.
  - Datos incorrectos: no se limpian después (`Clear()` no llamado).

---

## 5. PlayerPersistentData
- **¿Qué es?** ScriptableObject para guardar datos persistentes del jugador entre escenas.
- **Cómo crearlo:**
  1. Clic derecho en `Assets/ScriptableObjects/`.
  2. Crear > Combat > PlayerPersistentData.
  3. Nombra el asset (ej: `PlayerPersistentData`).
- **Dónde asignarlo:**
  - En sistemas de gestión de progreso o persistencia.
- **Función:** Guarda y aplica datos del jugador al cambiar de escena o cargar partidas.
- **Errores frecuentes:**
  - No guarda datos: no se llama a `SaveFromPlayer` o `ApplyToPlayer`.
  - Datos desactualizados: campos serializados no actualizados.

---

## 6. ItemDatabase
- **¿Qué es?** ScriptableObject que actúa como catálogo centralizado de ítems.
- **Cómo crearlo:**
  1. Clic derecho en `Assets/ScriptableObjects/`.
  2. Crear > Inventory > ItemDatabase.
  3. Nombra el asset (ej: `ItemDatabase`).
- **Dónde asignarlo:**
  - En sistemas de inventario, UI, etc.
- **Función:** Permite buscar y obtener cualquier ítem por su ID.
- **Errores frecuentes:**
  - No encuentra ítems: asset no asignado o lista incompleta.
  - IDs duplicados: dos ítems con el mismo ID.

---

# Ejemplos Paso a Paso para Cada ScriptableObject

## WeaponItem (Arma)
**Ejemplo: Crear y asignar una espada básica**
1. Ve al panel `Project` y navega a `Assets/ScriptableObjects/Weapons/`.
2. Haz clic derecho en la carpeta y selecciona `Crear > Inventory > WeaponItem`.
3. Nombra el asset: `EspadaBasica`.
4. Haz clic en el asset `EspadaBasica` y, en el Inspector, completa los campos: Daño = 10, Velocidad = 1.2, etc.
5. Selecciona el GameObject del jugador en la jerarquía.
6. En el componente `PlayerInventory`, busca la lista `initialItems` y haz clic en el símbolo `+` para agregar un nuevo elemento.
7. Arrastra el asset `EspadaBasica` desde el panel `Project` al nuevo campo de la lista en el Inspector.
8. Ejecuta el juego y verifica que el jugador pueda equipar y usar la espada.

---

## HealingItem (Poción de curación)
**Ejemplo: Crear y usar una poción**
1. Ve a `Assets/ScriptableObjects/Items/`.
2. Haz clic derecho y selecciona `Crear > Inventory > HealingItem`.
3. Nombra el asset: `PocionPequena`.
4. Haz clic en el asset y completa los campos: Cantidad de curación = 25, Nombre = "Poción Pequeña".
5. Asigna la poción al inventario inicial del jugador igual que con el arma.
6. Ejecuta el juego, abre el inventario y usa la poción. Observa si la vida del jugador aumenta.

---

## MysteryItem (Ítem misterioso)
**Ejemplo: Crear y usar un ítem misterioso**
1. Ve a `Assets/ScriptableObjects/Items/`.
2. Haz clic derecho y selecciona `Crear > Inventory > MysteryItem`.
3. Nombra el asset: `CajaSorpresa`.
4. Completa los campos: Nombre = "Caja Sorpresa", Descripción = "¿Qué contendrá?".
5. Asígnalo al inventario inicial del jugador o como recompensa en el juego.
6. Ejecuta el juego, usa el ítem y observa si revela su tipo o efecto.

---

## CombatTransferData (Transferencia de datos de combate)
**Ejemplo: Configurar transferencia de datos entre escenas**
1. Ve a `Assets/ScriptableObjects/`.
2. Haz clic derecho y selecciona `Crear > Combat > CombatTransferData`.
3. Nombra el asset: `CombatTransferData`.
4. Selecciona el GameObject que tenga el componente `CombatSceneLoader`.
5. Arrastra el asset `CombatTransferData` al campo correspondiente en el Inspector.
6. Haz lo mismo en el componente `CombatSceneInitializer`.
7. Ejecuta el juego y verifica que los datos se transfieran correctamente entre escenas.

---

## PlayerPersistentData (Datos persistentes del jugador)
**Ejemplo: Guardar y restaurar vida y experiencia**
1. Ve a `Assets/ScriptableObjects/`.
2. Haz clic derecho y selecciona `Crear > Combat > PlayerPersistentData`.
3. Nombra el asset: `PlayerPersistentData`.
4. Asigna el asset a los sistemas de guardado/carga de datos del jugador.
5. Ejecuta el juego, haz que el jugador gane experiencia o pierda vida, cambia de escena y verifica que los datos se mantienen.

---

## ItemDatabase (Catálogo de ítems)
**Ejemplo: Crear y usar una base de datos de ítems**
1. Ve a `Assets/ScriptableObjects/`.
2. Haz clic derecho y selecciona `Crear > Inventory > ItemDatabase`.
3. Nombra el asset: `ItemDatabase`.
4. Haz clic en el asset y, en el Inspector, agrega todos los ítems que quieras registrar (arrastra cada ScriptableObject a la lista).
5. Asigna el asset `ItemDatabase` a los sistemas de inventario o UI que lo requieran.
6. Ejecuta el juego y verifica que los ítems pueden buscarse y usarse correctamente.

---

¿Dudas? Consulta este archivo o pregunta a un compañero. ¡Feliz desarrollo!
