namespace ProyectSecret.Interfaces
{
    public interface IUsableItem
    {
        void Use(UnityEngine.GameObject user);
        string GetId();
    }
}
