using System;

namespace DS.Core.Utils
{
    public static class KeyNamingRules
    {
        // Генерация ключа для данных игрока
        public static string PlayerData(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                throw new ArgumentException("Player ID cannot be null or empty.");
            return $"playerdata_{playerId}";
        }

        // Генерация ключа для данных уровня
        public static string LevelData(string playerId, string levelId)
        {
            if (string.IsNullOrEmpty(playerId))
                throw new ArgumentException("Player ID cannot be null or empty.");
            if (string.IsNullOrEmpty(levelId))
                throw new ArgumentException("Level ID cannot be null or empty.");
            return $"leveldata_{playerId}_{levelId}";
        }

        // Генерация ключа для списка уровней
        public static string LevelsList(string filter = null)
        {
            return string.IsNullOrEmpty(filter) ? "levelslist" : $"levelslist_{filter}";
        }

        // Генерация ключа для данных оценок уровня
        public static string ScoreData(string levelId)
        {
            if (string.IsNullOrEmpty(levelId))
                throw new ArgumentException("Level ID cannot be null or empty.");
            return $"scoredata_{levelId}";
        }

        // Генерация ключа для настроек
        public static string Settings(string settingsType = null)
        {
            return string.IsNullOrEmpty(settingsType) ? "settings" : $"settings_{settingsType}";
        }

        // Генерация ключа для произвольного типа данных
        public static string Custom(string prefix, params string[] parts)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("Prefix cannot be null or empty.");
            return $"{prefix}{(parts.Length > 0 ? "_" : "")}{string.Join("_", parts)}";
        }
    }
}