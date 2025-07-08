# Manual de Uso de ScriptableObjects Específicos

> **IMPORTANTE:** Todos los ScriptableObjects de armas y objetos de curación interactúan exclusivamente con los componentes `HealthComponentBehaviour` y `StaminaComponentBehaviour` para modificar vida y stamina en el jugador o enemigos. Asegúrate de que estos behaviours estén presentes en los GameObjects correspondientes. Nunca accedas directamente a `HealthComponent` o `StaminaComponent` desde un ScriptableObject o cualquier otro sistema.

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
  - El daño se aplica siempre a través de `HealthComponentBehaviour` en el objetivo.
- **Ejemplo de flujo:**
  1. El jugador equipa el arma desde el inventario.
  2. Al atacar, el sistema activa el hitbox del arma.
  3. Si el hitbox colisiona con un enemigo, se llama a `ApplyDamage` del `WeaponItem`, que modifica la vida usando el wrapper.
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
- **Función:** Al usarlo, ejecuta `Use` y cura al jugador a través de `HealthComponentBehaviour`.
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

---

## Buenas prácticas para ScriptableObjects
- Siempre interactúa con la vida y stamina a través de los wrappers `HealthComponentBehaviour` y `StaminaComponentBehaviour`.
- No accedas ni modifiques directamente `HealthComponent` o `StaminaComponent` desde ningún ScriptableObject.
- Si necesitas modificar la vida o stamina de un GameObject, primero obtén el wrapper correspondiente y luego accede a la propiedad pública.
