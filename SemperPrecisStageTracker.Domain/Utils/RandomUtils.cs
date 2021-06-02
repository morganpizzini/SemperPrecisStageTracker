using System;
using System.Linq;
namespace SemperPrecisStageTracker.Domain.Utils
{
    public class RandomUtils
    {
        private static Random random = new Random();
        public static string RandomString(int length = 5)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
