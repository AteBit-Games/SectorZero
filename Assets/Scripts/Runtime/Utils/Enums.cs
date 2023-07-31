
namespace Runtime.Utils
{
    public enum DeathType
    {
        Locker,
    }

    public static class EnumUtils
    {
        public static string GetDeathMessage(DeathType deathType)
        {
            return deathType switch
            {
                DeathType.Locker => "If the monster sees you entering a locker your chances of survival are very low.",
                _ => "You died",
            };
        }
    }
    
}
