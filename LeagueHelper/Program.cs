using System.Net;
using System.Net.Sockets;
using LeagueHelper;
using QRCoder;

string ip;
using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
{
    socket.Connect("8.8.8.8", 65530);
    ip = ((IPEndPoint)socket.LocalEndPoint!).Address.ToString();
}

string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZephyrLoL");
Directory.CreateDirectory(dataFolder);
string topicFile = Path.Combine(dataFolder, "ntfy-topic.txt");
if (!File.Exists(topicFile))
    File.WriteAllText(topicFile, "zephyr-" + Guid.NewGuid().ToString("N").Substring(0, 12));

var bot = new Bot();
bot.NtfyTopic = File.ReadAllText(topicFile).Trim();
bot.PhoneUrl = "http://" + ip + ":5000";

var botThread = new Thread(() => bot.Run());
botThread.IsBackground = true;
botThread.Start();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");
builder.Logging.ClearProviders();
var app = builder.Build();

string[] swapKinds = { "trades", "swaps", "position-swaps" };
string[] swapVerbs = { "request", "accept", "decline", "cancel" };

app.MapGet("/", () => Results.Content(Page.Html, "text/html"));
app.MapGet("/ping", () => "zephyr");
app.MapGet("/now", () => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
app.MapGet("/qr.png", () =>
{
    using var generator = new QRCodeGenerator();
    using QRCodeData data = generator.CreateQrCode(bot.PhoneUrl, QRCodeGenerator.ECCLevel.Q);
    byte[] png = new PngByteQRCode(data).GetGraphic(10);
    return Results.File(png, "image/png");
});
app.MapGet("/status", () => bot);
app.MapGet("/champions", () => Results.Content(bot.GetChampions(), "application/json"));
app.MapGet("/allchampions", () => Results.Content(bot.GetAllChampions(), "application/json"));
app.MapGet("/runepages", () => Results.Content(bot.GetRunePages(), "application/json"));
app.MapGet("/perkstyles", () => Results.Content(bot.GetPerkStyles(), "application/json"));
app.MapGet("/perks", () => Results.Content(bot.GetPerks(), "application/json"));
app.MapGet("/recommended/{champ:int}/{position:alpha}", (int champ, string position) =>
    Results.Content(bot.GetRecommended(champ, position), "application/json"));
app.MapGet("/champselect", () => Results.Content(bot.GetSession(), "application/json"));
app.MapGet("/pickable", () => Results.Content(bot.GetPickable(), "application/json"));
app.MapGet("/bannable", () => Results.Content(bot.GetBannable(), "application/json"));
app.MapGet("/skins", () => Results.Content(bot.GetSkins(), "application/json"));
app.MapGet("/chat", () => Results.Content(bot.GetChat(), "application/json"));
app.MapGet("/chatme", () => Results.Content(bot.GetChatMe(), "application/json"));
app.MapGet("/summonerspells", () => Results.Content(bot.GetSummonerSpells(), "application/json"));
app.MapGet("/spellinfo", () => Results.Content(bot.GetSpellInfo(), "application/json"));
app.MapGet("/keybinds", () => Results.Content(bot.GetKeybinds(), "application/json"));
app.MapGet("/wards", () => Results.Content(bot.GetWards(), "application/json"));
app.MapGet("/currentward", () => bot.GetCurrentWard());
app.MapGet("/asset", (string path, HttpResponse response) =>
{
    byte[] data = bot.GetAsset(path);
    if (data.Length == 0)
        return Results.NotFound();

    response.Headers.CacheControl = "public, max-age=86400";
    string type = path.EndsWith(".png") ? "image/png" : "image/jpeg";
    return Results.File(data, type);
});

app.MapPost("/accept", () => bot.Accept());
app.MapPost("/decline", () => bot.Decline());
app.MapPost("/hover/{id:int}", (int id) => bot.Hover(id));
app.MapPost("/lock/{id:int}", (int id) => bot.Lock(id));
app.MapPost("/skin/{id:int}", (int id) => bot.SetSkin(id));
app.MapPost("/ward/{id:int}", (int id) => bot.SetWard(id));
app.MapPost("/setspells/{s1:int}/{s2:int}", (int s1, int s2) => bot.SetSpells(s1, s2));
app.MapPost("/usepage/{id:int}", (int id) => bot.UsePage(id));
app.MapPost("/swap/{kind}/{id:int}/{verb}", (string kind, int id, string verb) =>
{
    if (swapKinds.Contains(kind) && swapVerbs.Contains(verb))
        bot.SwapAction(kind, id, verb);
});
app.MapPost("/chat", async (HttpRequest request) =>
{
    string text = await new StreamReader(request.Body).ReadToEndAsync();
    bot.SendChat(text);
});
app.MapPost("/runes", async (HttpRequest request) =>
{
    string json = await new StreamReader(request.Body).ReadToEndAsync();
    int replace = int.TryParse(request.Query["replace"], out int r) ? r : 0;
    return bot.SaveRunes(json, replace);
});
app.MapPost("/settings", (Bot s) =>
{
    bot.AutoAccept = s.AutoAccept;
    bot.AutoPick = s.AutoPick;
    bot.AutoSetup = s.AutoSetup;
    bot.ChampionId = s.ChampionId;
    bot.RunePageId = s.RunePageId;
    bot.Spell1 = s.Spell1;
    bot.Spell2 = s.Spell2;

    if (bot.Phase == "ChampSelect" && bot.AutoSetup)
        bot.ApplySetup();
});

app.MapGet("/lobby", () => Results.Content(bot.GetLobby(), "application/json"));
app.MapGet("/queues", () => Results.Content(bot.GetQueues(), "application/json"));
app.MapGet("/friends", () => Results.Content(bot.GetFriends(), "application/json"));
app.MapGet("/invites", () => Results.Content(bot.GetInvites(), "application/json"));
app.MapGet("/search", () => Results.Content(bot.GetSearch(), "application/json"));
app.MapPost("/lobby/create/{queue:int}", (int queue) => bot.CreateLobby(queue));
app.MapPost("/lobby/leave", () => bot.LeaveLobby());
app.MapPost("/lobby/search", () => bot.StartSearch());
app.MapPost("/lobby/cancel", () => bot.CancelSearch());
app.MapPost("/lobby/invite/{id:long}", (long id) => bot.Invite(id));
app.MapPost("/lobby/kick/{id:long}", (long id) => bot.Kick(id));
app.MapPost("/lobby/roles/{first}/{second}", (string first, string second) => bot.SetRoles(first, second));
app.MapPost("/invites/{id}/{answer}", (string id, string answer) => bot.AnswerInvite(id, answer == "accept"));

app.Start();

var window = new Thread(() =>
{
    Application.SetHighDpiMode(HighDpiMode.SystemAware);
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new MainForm(dataFolder));
});
window.SetApartmentState(ApartmentState.STA);
window.Start();
window.Join();

Environment.Exit(0);