using System.Diagnostics;
using System.Text.Json;

namespace LeagueHelper
{
    internal partial class Bot
    {
        public bool AutoAccept { get; set; } = false;
        public bool AutoPick { get; set; } = false;
        public bool AutoSetup { get; set; } = true;
        public int ChampionId { get; set; } = 122;
        public int Spell1 { get; set; } = 4;
        public int Spell2 { get; set; } = 14;
        public int RunePageId { get; set; } = 0;
        public string Status { get; set; } = "Waiting for League client...";
        public string Phase { get; set; } = "None";
        public string NtfyTopic { get; set; } = "";
        public string PhoneUrl { get; set; } = "";

        private LeagueClient? Client { get; set; }
        private static HttpClient Ntfy { get; } = new HttpClient();

        public void Run()
        {
            while (true)
            {
                try
                {
                    RunSession();
                }
                catch
                {
                    Client = null;
                    Phase = "None";
                    Status = "Waiting for League client...";
                    Thread.Sleep(3000);
                }
            }
        }

        private void RunSession()
        {
            string? lockfilePath = FindLockfile();
            while (lockfilePath == null)
            {
                Thread.Sleep(3000);
                lockfilePath = FindLockfile();
            }

            var client = new LeagueClient(lockfilePath);
            Client = client;
            bool setupDone = false;
            bool notified = false;
            int lastTurnId = -1;

            while (true)
            {
                string phase = client.Get("/lol-gameflow/v1/gameflow-phase").Trim('"');
                if (phase.StartsWith("{"))
                    phase = "None";

                Phase = phase;
                Status = phase;

                if (phase == "ReadyCheck")
                {
                    if (!notified)
                    {
                        if (AutoAccept)
                        {
                            Accept();
                            Notify("Match found", "Accepted automatically", false);
                        }
                        else
                        {
                            Notify("Match found!", "Accept or decline", true);
                        }
                        notified = true;
                    }
                }
                else
                {
                    notified = false;
                }

                if (phase == "None" || phase == "Lobby")
                    CheckInvites(client);

                CheckMessages(client);

                if (phase == "ChampSelect")
                {
                    if (!setupDone)
                    {
                        if (AutoSetup)
                            ApplySetup();
                        setupDone = true;
                    }

                    var action = FindMyAction(client, false);
                    if (action != null)
                    {
                        if (action.Value.Type == "pick" && AutoPick)
                        {
                            CompleteAction(client, action.Value.Id, ChampionId);
                            Status = "Champion locked";
                        }
                        else if (action.Value.Id != lastTurnId)
                        {
                            Notify("Your turn!", "Time to " + action.Value.Type, false);
                            lastTurnId = action.Value.Id;
                        }
                    }
                }
                else
                {
                    setupDone = false;
                }

                Thread.Sleep(1000);
            }
        }

        public void Accept()
        {
            Client?.Post("/lol-matchmaking/v1/ready-check/accept", "");
        }

        public void Decline()
        {
            Client?.Post("/lol-matchmaking/v1/ready-check/decline", "");
        }

        public void ApplySetup()
        {
            if (Client == null)
                return;

            if (RunePageId != 0)
                Client.Put("/lol-perks/v1/currentpage", RunePageId.ToString());

            Client.Patch("/lol-champ-select/v1/session/my-selection", $"{{\"spell1Id\":{Spell1},\"spell2Id\":{Spell2}}}");
        }

        public string SaveRunes(string json, int replaceId)
        {
            if (Client == null)
                return "League isn't open.";

            if (replaceId > 0)
                Client.Delete("/lol-perks/v1/pages/" + replaceId);

            string result = Client.Post("/lol-perks/v1/pages", json);

            if (result.Contains("errorCode"))
            {
                int? target = FindReplaceablePage();
                if (target == null)
                    return "League said: " + ErrorMessage(result);

                Client.Delete("/lol-perks/v1/pages/" + target);
                result = Client.Post("/lol-perks/v1/pages", json);
            }

            RunePageId = 0;

            using JsonDocument created = JsonDocument.Parse(result);

            if (created.RootElement.TryGetProperty("errorCode", out _))
                return "League said: " + ErrorMessage(result);

            if (created.RootElement.TryGetProperty("id", out JsonElement newId))
            {
                Client.Put("/lol-perks/v1/currentpage", newId.GetInt32().ToString());
                return "ok:" + newId.GetInt32();
            }

            return "ok:0";
        }

        public void UsePage(int pageId)
        {
            Client?.Put("/lol-perks/v1/currentpage", pageId.ToString());
        }

        public void SetSpells(int spell1, int spell2)
        {
            Spell1 = spell1;
            Spell2 = spell2;
            Client?.Patch("/lol-champ-select/v1/session/my-selection", $"{{\"spell1Id\":{spell1},\"spell2Id\":{spell2}}}");
        }

        public string GetWards()
        {
            if (Client == null)
                return "[]";

            using JsonDocument me = JsonDocument.Parse(Client.Get("/lol-summoner/v1/current-summoner"));
            if (!me.RootElement.TryGetProperty("summonerId", out JsonElement summonerId))
                return "[]";

            return Client.Get("/lol-collections/v1/inventories/" + summonerId.GetInt64() + "/ward-skins");
        }

        public int GetCurrentWard()
        {
            if (Client == null)
                return 0;

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-loadouts/v4/loadouts/scope/account"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return 0;

            foreach (JsonElement loadout in doc.RootElement.EnumerateArray())
            {
                if (loadout.TryGetProperty("loadout", out JsonElement slots)
                    && slots.TryGetProperty("WARD_SKIN_SLOT", out JsonElement ward)
                    && ward.TryGetProperty("itemId", out JsonElement itemId))
                {
                    return itemId.GetInt32();
                }
            }

            return 0;
        }

        public string SetWard(int wardId)
        {
            if (Client == null)
                return "League isn't open.";

            string? loadoutId = null;

            using (JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-loadouts/v4/loadouts/scope/account")))
            {
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement loadout in doc.RootElement.EnumerateArray())
                    {
                        if (loadout.TryGetProperty("id", out JsonElement id))
                        {
                            loadoutId = id.ToString();
                            break;
                        }
                    }
                }
            }

            if (loadoutId == null)
                return "Couldn't find your ward loadout.";

            string json = $"{{\"loadout\":{{\"WARD_SKIN_SLOT\":{{\"inventoryType\":\"WARD_SKIN\",\"itemId\":{wardId}}}}}}}";
            string result = Client.Patch("/lol-loadouts/v4/loadouts/" + loadoutId, json);

            if (result.Contains("errorCode"))
                return "League said: " + ErrorMessage(result);

            return "ok";
        }

        public byte[] GetAsset(string path)
        {
            if (Client == null || !path.StartsWith("/lol-game-data/assets/"))
                return Array.Empty<byte>();
            return Client.GetBytes(path);
        }

        public string GetKeybinds()
        {
            if (Client == null)
                return "{}";

            string spell1 = "D";
            string spell2 = "F";

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-game-settings/v1/input-settings"));
            if (doc.RootElement.TryGetProperty("GameEvents", out JsonElement events))
            {
                if (events.TryGetProperty("evtCastAvatarSpell1", out JsonElement key1))
                    spell1 = FormatKey(key1.GetString(), "D");
                if (events.TryGetProperty("evtCastAvatarSpell2", out JsonElement key2))
                    spell2 = FormatKey(key2.GetString(), "F");
            }

            return JsonSerializer.Serialize(new { spell1, spell2 });
        }

        public string GetSummonerSpells()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-game-data/assets/v1/summoner-spells.json");
        }

        public string GetSpellInfo()
        {
            if (Client == null)
                return "{}";

            int level = 0;
            string mode = "CLASSIC";

            using (JsonDocument me = JsonDocument.Parse(Client.Get("/lol-summoner/v1/current-summoner")))
            {
                if (me.RootElement.TryGetProperty("summonerLevel", out JsonElement summonerLevel))
                    level = summonerLevel.GetInt32();
            }

            using (JsonDocument flow = JsonDocument.Parse(Client.Get("/lol-gameflow/v1/session")))
            {
                if (flow.RootElement.TryGetProperty("gameData", out JsonElement gameData)
                    && gameData.TryGetProperty("queue", out JsonElement queue)
                    && queue.TryGetProperty("gameMode", out JsonElement gameMode)
                    && gameMode.GetString() != "")
                {
                    mode = gameMode.GetString() ?? "CLASSIC";
                }
            }

            return JsonSerializer.Serialize(new { level, mode });
        }

        public void Hover(int championId)
        {
            if (Client == null)
                return;

            var action = FindMyAction(Client, true);
            if (action != null)
                Client.Patch("/lol-champ-select/v1/session/actions/" + action.Value.Id, $"{{\"championId\":{championId}}}");
        }

        public string Lock(int championId)
        {
            if (Client == null)
                return "League isn't open.";

            var action = FindMyAction(Client, false);
            if (action == null)
                return "It's not your turn yet.";

            return CompleteAction(Client, action.Value.Id, championId);
        }

        private string CompleteAction(LeagueClient client, int actionId, int championId)
        {
            string url = "/lol-champ-select/v1/session/actions/" + actionId;
            string hover = client.Patch(url, $"{{\"championId\":{championId}}}");
            if (hover.Contains("errorCode"))
                return "League said: " + ErrorMessage(hover);

            string result = client.Post(url + "/complete", "");
            if (result.Contains("errorCode"))
                result = client.Patch(url, $"{{\"championId\":{championId},\"completed\":true}}");

            if (result.Contains("errorCode"))
                return "League said: " + ErrorMessage(result);

            return "ok";
        }

        public void SetSkin(int skinId)
        {
            Client?.Patch("/lol-champ-select/v1/session/my-selection", $"{{\"selectedSkinId\":{skinId}}}");
        }

        public void SwapAction(string kind, int id, string verb)
        {
            Client?.Post($"/lol-champ-select/v1/session/{kind}/{id}/{verb}", "");
        }

        public void SendChat(string text)
        {
            if (Client == null || text.Trim() == "")
                return;

            string? id = FindChatId();
            if (id == null)
                return;

            string json = JsonSerializer.Serialize(new { body = text, type = "groupchat" });
            Client.Post("/lol-chat/v1/conversations/" + Uri.EscapeDataString(id) + "/messages", json);
        }

        public string GetChat()
        {
            if (Client == null)
                return "[]";

            string? id = FindChatId();
            if (id == null)
                return "[]";

            return Client.Get("/lol-chat/v1/conversations/" + Uri.EscapeDataString(id) + "/messages");
        }

        public string GetChatMe()
        {
            if (Client == null)
                return "{}";
            return Client.Get("/lol-chat/v1/me");
        }

        public string GetSkins()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-champ-select/v1/skin-carousel-skins");
        }

        public string GetSession()
        {
            if (Client == null)
                return "{}";
            return Client.Get("/lol-champ-select/v1/session");
        }

        public string GetPickable()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-champ-select/v1/pickable-champion-ids");
        }

        public string GetBannable()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-champ-select/v1/bannable-champion-ids");
        }

        public string GetAllChampions()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-game-data/assets/v1/champion-summary.json");
        }

        public string GetChampions()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-champions/v1/owned-champions-minimal");
        }

        public string GetRunePages()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-perks/v1/pages");
        }

        public string GetPerkStyles()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-perks/v1/styles");
        }

        public string GetPerks()
        {
            if (Client == null)
                return "[]";
            return Client.Get("/lol-perks/v1/perks");
        }

        public string GetRecommended(int championId, string position)
        {
            if (Client == null)
                return "[]";
            return Client.Get($"/lol-perks/v1/recommended-pages/champion/{championId}/position/{position}/map/11");
        }

        private string FormatKey(string? raw, string fallback)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return fallback;

            string first = raw.Split(',')[0].Trim();
            string key = first.Replace("][", "+").Trim('[', ']').ToUpper();
            key = key.Replace("BUTTON ", "MOUSE ");
            return key == "" ? fallback : key;
        }

        private int? FindReplaceablePage()
        {
            if (Client == null)
                return null;

            using (JsonDocument current = JsonDocument.Parse(Client.Get("/lol-perks/v1/currentpage")))
            {
                if (current.RootElement.TryGetProperty("id", out JsonElement currentId) && IsDeletable(current.RootElement))
                    return currentId.GetInt32();
            }

            using JsonDocument pages = JsonDocument.Parse(Client.Get("/lol-perks/v1/pages"));
            if (pages.RootElement.ValueKind != JsonValueKind.Array)
                return null;

            int? best = null;
            long bestTime = -1;

            foreach (JsonElement page in pages.RootElement.EnumerateArray())
            {
                if (!IsDeletable(page))
                    continue;

                long time = page.TryGetProperty("lastModified", out JsonElement modified) ? modified.GetInt64() : 0;
                if (time > bestTime)
                {
                    bestTime = time;
                    best = page.GetProperty("id").GetInt32();
                }
            }

            return best;
        }

        private string ErrorMessage(string result)
        {
            using JsonDocument doc = JsonDocument.Parse(result);
            if (doc.RootElement.TryGetProperty("message", out JsonElement message))
                return message.GetString() ?? result;
            return result;
        }

        private bool IsDeletable(JsonElement page)
        {
            return page.TryGetProperty("isDeletable", out JsonElement deletable) && deletable.GetBoolean();
        }

        private string? FindChatId()
        {
            if (Client == null)
                return null;

            using JsonDocument doc = JsonDocument.Parse(Client.Get("/lol-chat/v1/conversations"));
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return null;

            foreach (JsonElement conversation in doc.RootElement.EnumerateArray())
            {
                if (conversation.GetProperty("type").GetString() == "championSelect")
                    return conversation.GetProperty("id").GetString();
            }

            return null;
        }

        private (int Id, string Type)? FindMyAction(LeagueClient client, bool includeIntent)
        {
            using JsonDocument doc = JsonDocument.Parse(client.Get("/lol-champ-select/v1/session"));
            JsonElement session = doc.RootElement;
            if (!session.TryGetProperty("localPlayerCellId", out JsonElement cell))
                return null;

            int myCell = cell.GetInt32();
            (int Id, string Type)? intent = null;

            foreach (JsonElement group in session.GetProperty("actions").EnumerateArray())
            {
                foreach (JsonElement action in group.EnumerateArray())
                {
                    string type = action.GetProperty("type").GetString() ?? "";
                    bool mine = action.GetProperty("actorCellId").GetInt32() == myCell;
                    bool completed = action.GetProperty("completed").GetBoolean();

                    if (mine && (type == "pick" || type == "ban") && action.GetProperty("isInProgress").GetBoolean() && !completed)
                        return (action.GetProperty("id").GetInt32(), type);

                    if (mine && type == "pick" && !completed && intent == null)
                        intent = (action.GetProperty("id").GetInt32(), "intent");
                }
            }

            return includeIntent ? intent : null;
        }

        private void Notify(string title, string message, bool buttons)
        {
            if (NtfyTopic == "")
                return;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://ntfy.sh/" + NtfyTopic);
            request.Content = new StringContent(message);
            request.Headers.TryAddWithoutValidation("Title", title);
            request.Headers.TryAddWithoutValidation("Priority", "urgent");
            request.Headers.TryAddWithoutValidation("Tags", "video_game");
            request.Headers.TryAddWithoutValidation("Click", PhoneUrl);

            if (buttons)
                request.Headers.TryAddWithoutValidation("Actions",
                    $"http, Accept, {PhoneUrl}/accept, method=POST, clear=true; http, Decline, {PhoneUrl}/decline, method=POST, clear=true");

            Ntfy.SendAsync(request).Wait();
        }

        private string? FindLockfile()
        {
            if (Process.GetProcessesByName("LeagueClientUx").Length == 0)
                return null;

            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string settings = Path.Combine(programData, "Riot Games", "Metadata", "league_of_legends.live", "league_of_legends.live.product_settings.yaml");

            if (File.Exists(settings))
            {
                foreach (string line in File.ReadAllLines(settings))
                {
                    if (line.Trim().StartsWith("product_install_full_path:"))
                    {
                        string folder = line.Split(':', 2)[1].Trim().Trim('"');
                        string path = Path.Combine(folder, "lockfile");
                        if (File.Exists(path))
                            return path;
                    }
                }
            }

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                string path = Path.Combine(drive.Name, "Riot Games", "League of Legends", "lockfile");
                if (File.Exists(path))
                    return path;
            }

            return null;
        }
    }
}