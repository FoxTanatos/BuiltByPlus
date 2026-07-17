using System;

namespace BuiltBy
{
    public static class BuiltByText
    {
        public const string Ukrainian = "uk";
        public const string English = "en";

        public static string NormalizeLanguage(string language)
        {
            if (string.Equals(language, English, StringComparison.OrdinalIgnoreCase))
                return English;

            return Ukrainian;
        }

        public static string Get(string language, string key)
        {
            language = NormalizeLanguage(language);

            if (language == English)
                return GetEnglish(key);

            return GetUkrainian(key);
        }

        private static string GetEnglish(string key)
        {
            switch (key)
            {
                case "Language": return "Language";
                case "Ukrainian": return "Ukrainian";
                case "English": return "English";
                case "TabOverview": return "Overview";
                case "TabFactions": return "NPC Factions";
                case "TabAstronauts": return "Astronaut Cleanup";
                case "OverviewTitle": return "BuiltBy";
                case "OverviewText": return "BuiltBy prepares NPC grids for the vanilla NPC claim system by assigning block authorship when configured NPC factions spawn prefabs. It can also remove old station Astronaut NPC identities that remain in the save after the actual character is gone.";
                case "OverviewSafety": return "In safe mode, Astronaut cleanup uses only NPC Astronaut models and ignores identities that have a SteamId. If safe mode is disabled, it deletes by exact DisplayName only.";
                case "FactionTitle": return "NPC faction tags";
                case "FactionText": return "BuiltBy will prepare NPC structures owned by these faction founders.";
                case "Add": return "Add";
                case "Remove": return "Remove";
                case "Save": return "Save changes";
                case "RestartNote": return "Changes take effect after server restart.";
                case "SavedTitle": return "BuiltBy";
                case "SavedMessage": return "Settings saved. Changes take effect after server restart.";
                case "AstronautTitle": return "Astronaut Identity cleanup";
                case "AstronautText": return "Search and remove old NPC Astronaut identity records. Safe mode searches by NPC Astronaut model and excludes real players by SteamId. If safe mode is disabled, only exact DisplayName is checked.";
                case "AutoCleanup": return "Automatically delete 5 minutes after server start";
                case "SafeMode": return "Safe mode: delete by Astronaut NPC model and no SteamId";
                case "FindMatches": return "Find matches";
                case "DeleteFound": return "Delete found";
                case "ConfirmDelete": return "Found records: {0}. Delete them?";
                case "NoCleanup": return "Astronaut Identity cleanup is unavailable.";
                case "ScanResult": return "Found matches: {0}.";
                case "DeleteBusy": return "Deletion is already running.";
                case "DeleteResult": return "Found: {0}. Deleted: {1}.";
                case "DeleteResultLimited": return "Found: {0}. Deleted: {1}. The rest will be removed on the next run.";
                case "DeleteError": return "Astronaut Identity cleanup error: {0}";
                case "WorldNotLoaded": return "World is not loaded yet.";
                case "GridNotFound": return "Structure with exact name '{0}' was not found.";
                case "MultipleGrids": return "Several structures with name '{0}' were found. Rename the needed structure and try again.";
                case "PlayerNotFound": return "Player with exact name '{0}' was not found.";
                case "MultiplePlayers": return "Several players with name '{0}' were found.";
                case "NpcNotPlayer": return "The selected identity is NPC, not a player.";
                case "ManualTransferResult": return "Authorship of {0} blocks from structure '{1}' was transferred to player {2}.";
            }

            return key;
        }

        private static string GetUkrainian(string key)
        {
            switch (key)
            {
                case "Language": return "\u041C\u043E\u0432\u0430";
                case "Ukrainian": return "\u0423\u043A\u0440\u0430\u0457\u043D\u0441\u044C\u043A\u0430";
                case "English": return "English";
                case "TabOverview": return "\u041E\u0433\u043B\u044F\u0434";
                case "TabFactions": return "NPC-\u0444\u0440\u0430\u043A\u0446\u0456\u0457";
                case "TabAstronauts": return "\u041E\u0447\u0438\u0449\u0435\u043D\u043D\u044F Astronaut";
                case "OverviewTitle": return "BuiltBy";
                case "OverviewText": return "BuiltBy \u0433\u043E\u0442\u0443\u0454 NPC-\u0441\u0442\u0440\u0443\u043A\u0442\u0443\u0440\u0438 \u0434\u043E \u0432\u0430\u043D\u0456\u043B\u044C\u043D\u043E\u0457 \u043C\u0435\u0445\u0430\u043D\u0456\u043A\u0438 \u0437\u0430\u0445\u043E\u043F\u043B\u0435\u043D\u043D\u044F NPC: \u043F\u0440\u0438\u0441\u0432\u043E\u044E\u0454 \u0430\u0432\u0442\u043E\u0440\u0441\u0442\u0432\u043E \u0431\u043B\u043E\u043A\u0456\u0432, \u043A\u043E\u043B\u0438 \u043D\u0430\u043B\u0430\u0448\u0442\u043E\u0432\u0430\u043D\u0456 NPC-\u0444\u0440\u0430\u043A\u0446\u0456\u0457 \u0441\u043F\u0430\u0432\u043D\u044F\u0442\u044C \u043F\u0440\u0435\u0444\u0430\u0431\u0438. \u0422\u0430\u043A\u043E\u0436 \u043C\u043E\u0436\u0435 \u0432\u0438\u0434\u0430\u043B\u044F\u0442\u0438 \u0441\u0442\u0430\u0440\u0456 NPC-\u0437\u0430\u043F\u0438\u0441\u0438 Astronaut, \u044F\u043A\u0456 \u0437\u0430\u043B\u0438\u0448\u0438\u043B\u0438\u0441\u044F \u0432 \u0437\u0431\u0435\u0440\u0435\u0436\u0435\u043D\u043D\u0456.";
                case "OverviewSafety": return "\u0423 \u0431\u0435\u0437\u043F\u0435\u0447\u043D\u043E\u043C\u0443 \u0440\u0435\u0436\u0438\u043C\u0456 \u043E\u0447\u0438\u0449\u0435\u043D\u043D\u044F Astronaut \u043F\u0440\u0430\u0446\u044E\u0454 \u043B\u0438\u0448\u0435 \u0437 NPC-\u043C\u043E\u0434\u0435\u043B\u044F\u043C\u0438 Astronaut \u0456 \u0456\u0433\u043D\u043E\u0440\u0443\u0454 \u0437\u0430\u043F\u0438\u0441\u0438 \u0437 SteamId. \u042F\u043A\u0449\u043E \u0432\u0438\u043C\u043A\u043D\u0443\u0442\u0438 \u0431\u0435\u0437\u043F\u0435\u0447\u043D\u0438\u0439 \u0440\u0435\u0436\u0438\u043C, \u0432\u0438\u0434\u0430\u043B\u0435\u043D\u043D\u044F \u0439\u0434\u0435 \u043B\u0438\u0448\u0435 \u0437\u0430 \u0442\u043E\u0447\u043D\u0438\u043C DisplayName.";
                case "FactionTitle": return "\u0422\u0435\u0433\u0438 NPC-\u0444\u0440\u0430\u043A\u0446\u0456\u0439";
                case "FactionText": return "BuiltBy \u0433\u043E\u0442\u0443\u0432\u0430\u0442\u0438\u043C\u0435 NPC-\u0441\u0442\u0440\u0443\u043A\u0442\u0443\u0440\u0438, \u044F\u043A\u0456 \u043D\u0430\u043B\u0435\u0436\u0430\u0442\u044C \u043B\u0456\u0434\u0435\u0440\u0430\u043C \u0446\u0438\u0445 \u0444\u0440\u0430\u043A\u0446\u0456\u0439.";
                case "Add": return "\u0414\u043E\u0434\u0430\u0442\u0438";
                case "Remove": return "\u0412\u0438\u0434\u0430\u043B\u0438\u0442\u0438";
                case "Save": return "\u0417\u0431\u0435\u0440\u0435\u0433\u0442\u0438 \u0437\u043C\u0456\u043D\u0438";
                case "RestartNote": return "\u0417\u043C\u0456\u043D\u0438 \u043D\u0430\u0431\u0443\u0434\u0443\u0442\u044C \u0447\u0438\u043D\u043D\u043E\u0441\u0442\u0456 \u043B\u0438\u0448\u0435 \u043F\u0456\u0441\u043B\u044F \u043F\u0435\u0440\u0435\u0437\u0430\u043F\u0443\u0441\u043A\u0443 \u0441\u0435\u0440\u0432\u0435\u0440\u0430.";
                case "SavedTitle": return "BuiltBy";
                case "SavedMessage": return "\u041D\u0430\u043B\u0430\u0448\u0442\u0443\u0432\u0430\u043D\u043D\u044F \u0437\u0431\u0435\u0440\u0435\u0436\u0435\u043D\u043E. \u0417\u043C\u0456\u043D\u0438 \u043D\u0430\u0431\u0443\u0434\u0443\u0442\u044C \u0447\u0438\u043D\u043D\u043E\u0441\u0442\u0456 \u043F\u0456\u0441\u043B\u044F \u043F\u0435\u0440\u0435\u0437\u0430\u043F\u0443\u0441\u043A\u0443 \u0441\u0435\u0440\u0432\u0435\u0440\u0430.";
                case "AstronautTitle": return "\u041E\u0447\u0438\u0449\u0435\u043D\u043D\u044F Astronaut Identity";
                case "AstronautText": return "\u041F\u043E\u0448\u0443\u043A \u0456 \u0432\u0438\u0434\u0430\u043B\u0435\u043D\u043D\u044F \u0441\u0442\u0430\u0440\u0438\u0445 NPC-\u0437\u0430\u043F\u0438\u0441\u0456\u0432 Astronaut. \u0411\u0435\u0437\u043F\u0435\u0447\u043D\u0438\u0439 \u0440\u0435\u0436\u0438\u043C \u0448\u0443\u043A\u0430\u0454 \u0437\u0430 NPC-\u043C\u043E\u0434\u0435\u043B\u043B\u044E Astronaut \u0456 \u0456\u0433\u043D\u043E\u0440\u0443\u0454 \u0433\u0440\u0430\u0432\u0446\u0456\u0432 \u0437\u0430 SteamId. \u042F\u043A\u0449\u043E \u0439\u043E\u0433\u043E \u0432\u0438\u043C\u043A\u043D\u0443\u0442\u0438, \u043F\u0435\u0440\u0435\u0432\u0456\u0440\u044F\u0454\u0442\u044C\u0441\u044F \u043B\u0438\u0448\u0435 \u0442\u043E\u0447\u043D\u0438\u0439 DisplayName.";
                case "AutoCleanup": return "\u0410\u0432\u0442\u043E\u043C\u0430\u0442\u0438\u0447\u043D\u043E \u0432\u0438\u0434\u0430\u043B\u044F\u0442\u0438 \u0447\u0435\u0440\u0435\u0437 5 \u0445\u0432\u0438\u043B\u0438\u043D \u043F\u0456\u0441\u043B\u044F \u0437\u0430\u043F\u0443\u0441\u043A\u0443 \u0441\u0435\u0440\u0432\u0435\u0440\u0430";
                case "SafeMode": return "\u0411\u0435\u0437\u043F\u0435\u0447\u043D\u0438\u0439 \u0440\u0435\u0436\u0438\u043C: \u0432\u0438\u0434\u0430\u043B\u044F\u0442\u0438 \u0437\u0430 NPC-\u043C\u043E\u0434\u0435\u043B\u043B\u044E Astronaut \u0456 \u0431\u0435\u0437 SteamId";
                case "FindMatches": return "\u0417\u043D\u0430\u0439\u0442\u0438 \u0437\u0431\u0456\u0433\u0438";
                case "DeleteFound": return "\u0412\u0438\u0434\u0430\u043B\u0438\u0442\u0438 \u0437\u043D\u0430\u0439\u0434\u0435\u043D\u0435";
                case "ConfirmDelete": return "\u0417\u043D\u0430\u0439\u0434\u0435\u043D\u043E \u0437\u0430\u043F\u0438\u0441\u0456\u0432: {0}. \u0412\u0438\u0434\u0430\u043B\u0438\u0442\u0438 \u0457\u0445?";
                case "NoCleanup": return "\u041E\u0447\u0438\u0449\u0435\u043D\u043D\u044F Astronaut Identity \u043D\u0435\u0434\u043E\u0441\u0442\u0443\u043F\u043D\u0435.";
                case "ScanResult": return "\u0417\u043D\u0430\u0439\u0434\u0435\u043D\u043E \u0437\u0431\u0456\u0433\u0456\u0432: {0}.";
                case "DeleteBusy": return "\u0412\u0438\u0434\u0430\u043B\u0435\u043D\u043D\u044F \u0432\u0436\u0435 \u0432\u0438\u043A\u043E\u043D\u0443\u0454\u0442\u044C\u0441\u044F.";
                case "DeleteResult": return "\u0417\u043D\u0430\u0439\u0434\u0435\u043D\u043E: {0}. \u0412\u0438\u0434\u0430\u043B\u0435\u043D\u043E: {1}.";
                case "DeleteResultLimited": return "\u0417\u043D\u0430\u0439\u0434\u0435\u043D\u043E: {0}. \u0412\u0438\u0434\u0430\u043B\u0435\u043D\u043E: {1}. \u0420\u0435\u0448\u0442\u0430 \u0431\u0443\u0434\u0435 \u0432\u0438\u0434\u0430\u043B\u0435\u043D\u0430 \u043D\u0430\u0441\u0442\u0443\u043F\u043D\u0438\u043C \u0437\u0430\u043F\u0443\u0441\u043A\u043E\u043C.";
                case "DeleteError": return "\u041F\u043E\u043C\u0438\u043B\u043A\u0430 \u043E\u0447\u0438\u0449\u0435\u043D\u043D\u044F Astronaut Identity: {0}";
                case "WorldNotLoaded": return "\u0421\u0432\u0456\u0442 \u0449\u0435 \u043D\u0435 \u0437\u0430\u0432\u0430\u043D\u0442\u0430\u0436\u0435\u043D\u0438\u0439.";
                case "GridNotFound": return "\u0421\u0442\u0440\u0443\u043A\u0442\u0443\u0440\u0443 \u0437 \u0442\u043E\u0447\u043D\u043E\u044E \u043D\u0430\u0437\u0432\u043E\u044E '{0}' \u043D\u0435 \u0437\u043D\u0430\u0439\u0434\u0435\u043D\u043E.";
                case "MultipleGrids": return "\u0417\u043D\u0430\u0439\u0434\u0435\u043D\u043E \u043A\u0456\u043B\u044C\u043A\u0430 \u0441\u0442\u0440\u0443\u043A\u0442\u0443\u0440 \u0456\u0437 \u043D\u0430\u0437\u0432\u043E\u044E '{0}'. \u041F\u0435\u0440\u0435\u0439\u043C\u0435\u043D\u0443\u0439\u0442\u0435 \u043F\u043E\u0442\u0440\u0456\u0431\u043D\u0443 \u0441\u0442\u0440\u0443\u043A\u0442\u0443\u0440\u0443 \u0442\u0430 \u043F\u043E\u0432\u0442\u043E\u0440\u0456\u0442\u044C.";
                case "PlayerNotFound": return "\u0413\u0440\u0430\u0432\u0446\u044F \u0437 \u0442\u043E\u0447\u043D\u0438\u043C \u0456\u043C'\u044F\u043C '{0}' \u043D\u0435 \u0437\u043D\u0430\u0439\u0434\u0435\u043D\u043E.";
                case "MultiplePlayers": return "\u0417\u043D\u0430\u0439\u0434\u0435\u043D\u043E \u043A\u0456\u043B\u044C\u043A\u0430 \u0433\u0440\u0430\u0432\u0446\u0456\u0432 \u0437 \u0456\u043C'\u044F\u043C '{0}'.";
                case "NpcNotPlayer": return "\u0412\u043A\u0430\u0437\u0430\u043D\u0430 \u043E\u0441\u043E\u0431\u0430 \u0454 NPC, \u0430 \u043D\u0435 \u0433\u0440\u0430\u0432\u0446\u0435\u043C.";
                case "ManualTransferResult": return "\u0410\u0432\u0442\u043E\u0440\u0441\u0442\u0432\u043E {0} \u0431\u043B\u043E\u043A\u0456\u0432 \u0441\u0442\u0440\u0443\u043A\u0442\u0443\u0440\u0438 '{1}' \u043F\u0435\u0440\u0435\u0434\u0430\u043D\u043E \u0433\u0440\u0430\u0432\u0446\u044E {2}.";
            }

            return key;
        }
    }
}
