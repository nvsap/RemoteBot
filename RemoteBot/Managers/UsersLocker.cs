using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteBot.Managers
{
    public static class UserLocker
    {
        private static Dictionary<long, bool> UserLockers { get; set; } = new Dictionary<long, bool>();

        public static void LockUser(long userId)
        {
            if(UserLockers.TryGetValue(userId, out bool result))
                UserLockers[userId] = true;
        }

        public static void UnlockUser(long userId)
        {
            if (UserLockers.TryGetValue(userId, out bool result))
                UserLockers[userId] = false;
        }

        public static bool IsUserLocked(long userId)
        {
            if (UserLockers.TryGetValue(userId, out bool result))
                return result;
            return false;
        }

        public static void AddUser(long userId)
        {
            UserLockers.Add(userId, false);
        }
    }
}
