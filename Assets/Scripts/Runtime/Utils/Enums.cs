
namespace Runtime.Utils
{
    public enum DeathType
    {
        Locker,
        Open
    }

    public static class EnumUtils
    {
        private static string[] _openDeathMessages = {
            "",
            ""
        };
        
        public static string GetDeathMessage(DeathType deathType)
        {
            return deathType switch
            {
                DeathType.Locker => "If the monster sees you entering a locker your chances of survival are very low.",
                DeathType.Open => _openDeathMessages[UnityEngine.Random.Range(0, _openDeathMessages.Length)],
                _ => "You died",
            };
        }
    }
    
}
