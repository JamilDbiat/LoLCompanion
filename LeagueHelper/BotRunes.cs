using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeagueHelper
{
    internal partial class Bot
    {
        private class RunePageInput
        {
            [JsonPropertyName("name")] public string? Name { get; set; }
            [JsonPropertyName("primaryStyleId")] public int PrimaryStyleId { get; set; }
            [JsonPropertyName("subStyleId")] public int SubStyleId { get; set; }
            [JsonPropertyName("selectedPerkIds")] public List<int>? SelectedPerkIds { get; set; }
        }

        // Re-builds the page from known fields only, so nothing unexpected is sent to League.
        private static string? CleanPage(string json, out string error)
        {
            error = "";
            RunePageInput? page;
            try { page = JsonSerializer.Deserialize<RunePageInput>(json); }
            catch { error = "Bad rune page."; return null; }

            if (page == null || page.SelectedPerkIds == null || page.SelectedPerkIds.Count != 9 || page.SelectedPerkIds.Any(id => id <= 0))
            {
                error = "Pick all runes first.";
                return null;
            }
            if (page.PrimaryStyleId <= 0 || page.SubStyleId <= 0 || page.PrimaryStyleId == page.SubStyleId)
            {
                error = "Pick a primary and a different secondary path.";
                return null;
            }

            string name = (page.Name ?? "").Trim();
            if (name == "") name = "New Page";
            if (name.Length > 25) name = name.Substring(0, 25);

            return JsonSerializer.Serialize(new
            {
                name,
                primaryStyleId = page.PrimaryStyleId,
                subStyleId = page.SubStyleId,
                selectedPerkIds = page.SelectedPerkIds,
                current = true
            });
        }

        private List<int> EditablePageIds()
        {
            var ids = new List<int>();
            if (Client == null) return ids;
            using JsonDocument pages = JsonDocument.Parse(Client.Get("/lol-perks/v1/pages"));
            if (pages.RootElement.ValueKind != JsonValueKind.Array) return ids;
            foreach (JsonElement p in pages.RootElement.EnumerateArray())
                if (IsDeletable(p) && p.TryGetProperty("id", out JsonElement id))
                    ids.Add(id.GetInt32());
            return ids;
        }

        private int OwnedPageCount()
        {
            if (Client == null) return 0;
            try
            {
                using JsonDocument inv = JsonDocument.Parse(Client.Get("/lol-perks/v1/inventory"));
                if (inv.RootElement.TryGetProperty("ownedPageCount", out JsonElement n))
                    return n.GetInt32();
            }
            catch { }
            return 0;
        }

        public string GetRuneInfo()
        {
            if (Client == null) return "{\"owned\":0,\"used\":0}";
            return JsonSerializer.Serialize(new { owned = OwnedPageCount(), used = EditablePageIds().Count });
        }

        public string CreateRunePage(string json)
        {
            if (Client == null) return "League isn't open.";
            string? body = CleanPage(json, out string error);
            if (body == null) return error;

            int owned = OwnedPageCount();
            if (owned > 0 && EditablePageIds().Count >= owned)
                return "full";

            string result = Client.Post("/lol-perks/v1/pages", body);
            using JsonDocument created = JsonDocument.Parse(result);
            if (created.RootElement.TryGetProperty("errorCode", out _))
                return "League said: " + ErrorMessage(result);

            RunePageId = 0;
            if (created.RootElement.TryGetProperty("id", out JsonElement newId))
            {
                Client.Put("/lol-perks/v1/currentpage", newId.GetInt32().ToString());
                return "ok:" + newId.GetInt32();
            }
            return "ok:0";
        }

        public string EditRunePage(int id, string json)
        {
            if (Client == null) return "League isn't open.";
            if (!EditablePageIds().Contains(id)) return "That page can't be edited.";
            string? body = CleanPage(json, out string error);
            if (body == null) return error;

            string response = Client.PutText("/lol-perks/v1/pages/" + id, body);
            if (response.Contains("errorCode"))
                return "League said: " + ErrorMessage(response);

            RunePageId = 0;
            Client.Put("/lol-perks/v1/currentpage", id.ToString());
            return "ok:" + id;
        }

        public string DeleteRunePage(int id)
        {
            if (Client == null) return "League isn't open.";
            if (!EditablePageIds().Contains(id)) return "That page can't be deleted.";
            Client.Delete("/lol-perks/v1/pages/" + id);
            if (RunePageId == id) RunePageId = 0;
            return EditablePageIds().Contains(id) ? "League didn't delete it." : "ok";
        }
    }
}
