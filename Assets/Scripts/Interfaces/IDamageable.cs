namespace ProyectSecret.Interfaces
{
    /// <summary>
    /// Interfaz para objetos que pueden recibir daño y morir.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount);
        event System.Action OnDeath;
    }
}
