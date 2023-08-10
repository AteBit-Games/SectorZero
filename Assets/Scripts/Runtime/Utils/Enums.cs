/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

namespace Runtime.Utils
{
    public enum DeathType
    {
        Locker,
        Open
    }

    public static class EnumUtils
    {
        private static readonly string[] OpenDeathMessages = {
            "The creatures are difficult to outrun, time your movements carefully.",
            "Running isn't always the best option, sometimes it's better to hide.",
            "Use the environment when sneaking to avoid the creature's line of sight.",
        };
        
        private static readonly string[] LockerDeathMessages = {
            "If the creature sees you entering a locker your chances of survival are slim.",
        };
        
        public static string GetDeathMessage(DeathType deathType)
        {
            return deathType switch
            {
                DeathType.Locker => LockerDeathMessages[UnityEngine.Random.Range(0, LockerDeathMessages.Length)],
                DeathType.Open => OpenDeathMessages[UnityEngine.Random.Range(0, OpenDeathMessages.Length)],
                _ => "You died",
            };
        }
    }
    
}
