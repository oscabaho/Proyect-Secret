# Documentación del Proyecto Unity: Manual de Clases, Métodos y Variables

## Índice
1. [Clases Principales](#clases-principales)
2. [Manual de Métodos](#manual-de-métodos)
3. [Manual de Variables](#manual-de-variables)

---

## Clases Principales

### Stats.StatComponent
- Clase base abstracta para componentes de estadísticas (vida, stamina, etc).
- Métodos: `Awake()`, `AffectValue(int value)`
- Propiedades: `MaxValue`, `CurrentValue`, `OnValueChanged`

### Components.HealthComponent
- Hereda de `StatComponent`. Gestiona la vida.
- Métodos: `AffectValue(int value)`

### Components.StaminaComponent
- Hereda de `StatComponent`. Gestiona la stamina.
- Métodos: `UseStamina(int amount)`, `RecoverStamina(int amount)`, `SetStamina(int value)`
- Propiedades: `MaxStamina`, `CurrentStamina`

### Base.BaseStatHolder
- Clase abstracta para entidades con estadísticas.
- Métodos: `Awake()`, `TakeDamage(int amount)`
- Propiedades: `Health`

### Holders.PlayableStatHolder
- Portador de estadísticas para el jugador. Implementa `IDamageable`.
- Métodos: `SubscribeOnDeath(Action)`, `UnsubscribeOnDeath(Action)`
- Propiedades: `Health`, `Stamina`, `OnDeath`

### NonPlayableStatHolder
- Portador de estadísticas para NPCs. Implementa `IDamageable`.
- Métodos: `SubscribeOnDeath(Action)`, `UnsubscribeOnDeath(Action)`
- Propiedades: `Health`, `OnDeath`

### Enemies.Enemy
- Lógica de enemigo. Implementa `IDamageable`.
- Métodos: `SubscribeOnDeath(Action)`, `UnsubscribeOnDeath(Action)`, `TakeDamage(int)`
- Propiedades: `Health`, `OnDeath`

### Combat.Behaviours.AttackComponent
- Componente modular de ataque.
- Métodos: `TryAttack()`

### Combat.Attack
- Script de ataque para el jugador.
- Métodos: `Awake()`, `Update()`, `Attacking()`

---

## Manual de Métodos

- **Awake()**: Inicializa componentes y valida referencias.
- **AffectValue(int value)**: Modifica el valor de la estadística y dispara eventos.
- **UseStamina(int amount)**: Consume stamina.
- **RecoverStamina(int amount)**: Recupera stamina.
- **SetStamina(int value)**: Fija el valor de stamina.
- **TakeDamage(int amount)**: Aplica daño a la entidad.
- **SubscribeOnDeath(Action callback)**: Permite suscribirse al evento de muerte.
- **UnsubscribeOnDeath(Action callback)**: Permite desuscribirse del evento de muerte.
- **TryAttack()**: Realiza un ataque si el cooldown lo permite.
- **Attacking()**: Lógica de ataque en el script `Attack`.

---

## Manual de Variables

- **[SerializeField] private int maxValue**: Valor máximo de la estadística.
- **[SerializeField] private int currentValue**: Valor actual de la estadística.
- **public int MaxValue**: Propiedad de solo lectura para el valor máximo.
- **public int CurrentValue**: Propiedad de solo lectura para el valor actual.
- **[SerializeField] private HealthComponent health**: Componente de vida.
- **[SerializeField] private StaminaComponent stamina**: Componente de stamina.
- **public HealthComponent Health**: Propiedad de solo lectura para vida.
- **public StaminaComponent Stamina**: Propiedad de solo lectura para stamina.
- **public event Action OnDeath**: Evento disparado al morir la entidad.
- **[SerializeField] private float attackRange**: Rango de ataque.
- **[SerializeField] private int damage**: Daño infligido por ataque.
- **[SerializeField] private float attackCooldown**: Tiempo de espera entre ataques.
- **private float lastAttackTime**: Marca de tiempo del último ataque.

---

> Esta documentación cubre las clases, métodos y variables principales del proyecto, siguiendo las mejores prácticas de encapsulamiento y serialización para Unity/C#.
