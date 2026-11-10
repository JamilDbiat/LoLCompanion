using System.Text.Json;
using System.Text.RegularExpressions;

namespace LeagueHelper
{
    internal partial class Bot
    {
        private static readonly Regex SafeName = new Regex("^[A-Za-z0-9_\\-]+$");

        public string GetLoot() => Client == null ? "[]" : Client.Get("/lol-loot/v1/player-loot");

        public string GetRecipes(string lootId)
        {
            if (Client == null || !SafeName.IsMatch(lootId))
                return "[]";
            return Client.Get("/lol-loot/v1/recipes/initial-item/" + lootId);
        }

        public string Craft(string recipeName, string lootIdsJson, int repeat)
        {
            if (Client == null)
                return JsonSerializer.Serialize(new { error = "League isn't open." });
            if (!SafeName.IsMatch(recipeName) || repeat < 1 || repeat > 50)
                return JsonSerializer.Serialize(new { error = "Invalid craft." });

            string[]? ids = JsonSerializer.Deserialize<string[]>(lootIdsJson);
            if (ids == null || ids.Length == 0 || ids.Any(id => !SafeName.IsMatch(id)))
                return JsonSerializer.Serialize(new { error = "Invalid items." });

            string result = Client.Post("/lol-loot/v1/recipes/" + recipeName + "/craft?repeat=" + repeat, JsonSerializer.Serialize(ids));
            if (result.Contains("\"errorCode\""))
                return JsonSerializer.Serialize(new { error = "League said: " + ErrorMessage(result) });
            return result;
        }
    }
}
