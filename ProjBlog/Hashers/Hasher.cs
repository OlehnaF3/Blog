using System;
using System.Text;
namespace clickertime.Hashers
{
    /// <summary>
    /// Учебный хешер-заглушка (ТОЛЬКО ДЛЯ ОБУЧЕНИЯ!)
    /// </summary>
    public static class Hasher
    {
        /// <summary>
        /// Хеширует строку и сохраняет результат для проверки
        /// </summary>
        public static string Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "0";

            // Простая учебная хеш-функция
            uint hash = 2166136261;
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            foreach (byte b in bytes)
            {
                hash = (hash * 16777619) ^ b;
            }

            string result = $"LEARN_{hash:X8}";

            return result;
        }

        /// <summary>
        /// Проверяет, соответствует ли строка ранее сохраненному хешу
        /// </summary>
        public static bool Verify(string input, string passwordHash)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            string computedHash = Hash(input);

            return computedHash == passwordHash;
        }

    }
}
