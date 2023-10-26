/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

namespace Runtime.Utils
{
    public enum DeathType
    {
        Locker,
        Open,
        Vent
    }

    public static class EnumUtils
    {
        private static readonly string[] OpenDeathMessages = {
            "The creatures are difficult to outrun, time your movements carefully.",
            "Running isn't always the best option, sometimes it's better to hide.",
            "When entering hidable the monster cannot hear you so don’t be afraid to hide when it is approaching.",
            "You can distract the monster using various objects in the environment such as fire alarms, radios and alarm clocks.",
            "Crouching makes you completely silent when moving around.",
            "Crouching behind objects allows you to avoid the monster’s line of sight.",
            "Keep track of what you need to be doing by opening the inventory window with the tab key.",
            "Sometimes you can use vents to escape tricky situations."
        };
        
        private static readonly string[] LockerDeathMessages = {
            "If the creature sees you entering a hidable your chances of survival are slim.",
            "While patrolling the monster might inspect hidables",
            "Hidables aren't your only option to avoid the monster, you can also crouch behind objects."
        };

        private static readonly string[] VentDeathMessages = {
            "The monster can pull you out of a vent if you are not quick enough.",
            "When using vents make sure you are not being chased by the monster.",
        };
        
        public static string GetDeathMessage(DeathType deathType)
        {
            return deathType switch
            {
                DeathType.Locker => LockerDeathMessages[UnityEngine.Random.Range(0, LockerDeathMessages.Length)],
                DeathType.Open => OpenDeathMessages[UnityEngine.Random.Range(0, OpenDeathMessages.Length)],
                DeathType.Vent => VentDeathMessages[UnityEngine.Random.Range(0, VentDeathMessages.Length)],
                _ => "You died",
            };
        }
    }
    
}
