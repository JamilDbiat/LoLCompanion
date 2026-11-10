namespace LeagueHelper
{
    internal class Page
    {
        public const string Html = """
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover">
<meta name="apple-mobile-web-app-capable" content="yes">
<meta name="mobile-web-app-capable" content="yes">
<meta name="apple-mobile-web-app-title" content="LoL Companion">
<meta name="theme-color" content="#120a1f">
<title>LoL Companion</title>
<style>
:root{--bg:#120a1f;--card:#1a0f2c;--card2:#22143a;--line:rgba(232,184,74,.2);--gold:#e8b84a;--gold2:#f5e6c4;--text:#f1e9ff;--muted:#a993c9;--red:#e84057;--green:#1fae5b;--blue:#7c3aed}
*{box-sizing:border-box;-webkit-tap-highlight-color:transparent}
body{margin:0;font-family:-apple-system,"Segoe UI",Roboto,sans-serif;color:var(--text);background:radial-gradient(circle at 50% -20%,#1d3a52 0%,var(--bg) 55%) fixed;min-height:100vh}
header{position:sticky;top:0;z-index:10;display:flex;align-items:center;gap:12px;padding:calc(12px + env(safe-area-inset-top)) 18px 12px;background:rgba(10,20,29,.85);backdrop-filter:blur(12px);border-bottom:1px solid var(--line)}
.logo{width:40px;height:40px;border-radius:12px;border:2px solid var(--gold);display:flex;align-items:center;justify-content:center;font-weight:900;font-size:22px;color:var(--gold)}
.brand{font-weight:800;letter-spacing:3px;color:var(--gold2);font-size:15px}
.status{font-size:13px;color:var(--muted);display:flex;align-items:center;gap:6px}
.dot{width:8px;height:8px;border-radius:50%;background:var(--muted)}
.dot.live{background:var(--green);box-shadow:0 0 8px var(--green)}
main{max-width:520px;margin:auto;padding:16px 16px calc(96px + env(safe-area-inset-bottom))}
.tab{display:none}
.tab.on{display:block;animation:fade .2s}
@keyframes fade{from{opacity:0;transform:translateY(6px)}to{opacity:1;transform:none}}
.card{background:linear-gradient(180deg,var(--card2),var(--card));border:1px solid var(--line);border-radius:16px;padding:16px;margin-bottom:14px}
.center{text-align:center}
h3{margin:0 0 10px;color:var(--gold2);font-size:16px;letter-spacing:.5px}
.big{font-size:22px;font-weight:800;color:var(--gold2);margin:6px 0}
.muted{color:var(--muted);font-size:14px}
input,select{width:100%;padding:13px;font-size:16px;border-radius:12px;border:1px solid var(--line);background:#0c1822;color:var(--text);outline:none}
input:focus,select:focus{border-color:var(--gold)}
button{width:100%;padding:14px;font-size:16px;font-weight:800;border:none;border-radius:12px;background:linear-gradient(180deg,#d9bd84,#a8894f);color:#111;margin-top:12px;cursor:pointer}
button:active{transform:scale(.98)}
button.ghost{background:transparent;border:1px solid var(--line);color:var(--gold)}
button.mini{width:auto;padding:5px 10px;font-size:12px;margin:6px 6px 0 0;border-radius:8px}
button.green{background:var(--green);color:#fff}
button.red{background:var(--red);color:#fff}
.setting{display:flex;justify-content:space-between;align-items:center;padding:12px 0;border-bottom:1px solid var(--line)}
.setting:last-child{border-bottom:none}
.setting input[type=checkbox]{width:22px;height:22px;accent-color:var(--gold)}
.field{display:block;margin-top:12px}
.field span{display:block;font-size:13px;color:var(--muted);margin-bottom:6px}
nav{position:fixed;bottom:0;left:0;right:0;display:flex;background:rgba(10,20,29,.92);backdrop-filter:blur(12px);border-top:1px solid var(--line);padding-bottom:env(safe-area-inset-bottom);z-index:10}
nav button{flex:1;background:none;color:var(--muted);margin:0;border-radius:0;padding:10px 0 12px;font-size:12px;font-weight:600}
nav button b{display:block;font-size:22px;margin-bottom:2px}
nav button.on{color:var(--gold)}
.toast{position:fixed;left:50%;transform:translateX(-50%);bottom:calc(84px + env(safe-area-inset-bottom));background:var(--green);color:#fff;padding:8px 16px;border-radius:20px;font-weight:700;font-size:14px;z-index:20}
.toast:empty{display:none}
.spinner{width:42px;height:42px;border:4px solid var(--line);border-top-color:var(--gold);border-radius:50%;margin:10px auto;animation:spin 1s linear infinite}
@keyframes spin{to{transform:rotate(360deg)}}
.found{font-size:30px;font-weight:900;color:var(--gold2);letter-spacing:2px;animation:pulse 1s infinite}
@keyframes pulse{50%{opacity:.5}}
#accept{background:var(--green);color:#fff;font-size:26px;padding:24px}
.qr{width:200px;height:200px;border-radius:12px;background:#fff;padding:8px;margin:14px auto 8px;display:block}
#phaseName{text-align:center;color:var(--muted);font-size:13px;letter-spacing:2px;text-transform:uppercase}
#timer{font-size:30px;font-weight:900;color:var(--gold2);text-align:center}
#turn{text-align:center;font-weight:800;padding:12px;border-radius:12px;margin:12px 0;letter-spacing:1px}
#turn:empty{display:none}
#turn.ban{background:var(--red)}
#turn.pick{background:var(--green)}
#turn.intent{background:var(--blue)}
.offer{background:var(--blue);padding:12px;border-radius:12px;margin:10px 0}
.bans{display:flex;justify-content:space-between;margin:12px 0;min-height:30px}
.bans img{width:30px;height:30px;filter:grayscale(1);margin-right:3px;border-radius:6px;opacity:.8}
.teams{display:flex;gap:8px}
.teams > div{flex:1;min-width:0}
.slot{display:flex;align-items:center;gap:8px;background:#0c1822;border:1px solid var(--line);border-radius:12px;padding:6px;margin-bottom:6px;font-size:13px}
.slot.me{border-color:var(--gold);box-shadow:0 0 12px rgba(200,170,110,.25)}
#theirTeam .slot{flex-direction:row-reverse;text-align:right}
.icon{width:42px;height:42px;border-radius:10px;flex-shrink:0}
.icon.empty{background:#081119}
.faded{opacity:.4}
#grid,#skinGrid,#wardGrid{display:flex;flex-wrap:wrap;gap:8px;max-height:340px;overflow-y:auto;margin-top:10px;justify-content:center}
.cell{width:64px;text-align:center;font-size:11px;color:var(--muted)}
.cell img{width:58px;height:58px;border-radius:10px;border:3px solid transparent}
.cell img.selected{border-color:var(--gold)}
.cell.skin{width:100px}
.cell.skin img{width:94px;height:94px}
.chroma{display:inline-block;width:30px;height:30px;border-radius:50%;margin:5px;border:3px solid transparent}
.chroma.selected{border-color:var(--gold)}
.preview{width:100%;aspect-ratio:16/9;object-fit:cover;border-radius:12px;background:#0c1822;border:1px solid var(--line);margin-bottom:8px}
#lockBtn{font-size:22px;padding:18px}
.spellSlots{display:flex;gap:10px}
.spellSlot{flex:1;display:flex;align-items:center;gap:8px;background:#0c1822;border:2px solid var(--line);border-radius:12px;padding:8px;font-size:13px}
.spellSlot.on{border-color:var(--gold);box-shadow:0 0 12px rgba(200,170,110,.25)}
.spellSlot img{width:40px;height:40px;border-radius:8px}
.spell{width:46px;height:46px;border-radius:8px;margin:4px}
.spell.used{opacity:.35}
#chatBox{background:#0c1822;border:1px solid var(--line);border-radius:12px;padding:10px;height:160px;overflow-y:auto;font-size:14px}
#chatBox .sys{color:var(--muted);font-style:italic}
.chatRow{display:flex;gap:8px;margin-top:8px}
.chatRow button{width:90px;margin-top:0}
.tabs{display:flex;gap:6px;margin-bottom:12px}
.tabs button{margin-top:0;background:#0c1822;border:1px solid var(--line);color:var(--gold);font-size:14px;padding:12px 6px}
.tabs button.on{background:linear-gradient(180deg,#d9bd84,#a8894f);color:#111}
.rec{display:flex;gap:12px;align-items:center;background:#0c1822;border:1px solid var(--line);border-radius:12px;padding:12px;margin-top:10px}
.rec.current{border-color:var(--gold);box-shadow:0 0 12px rgba(200,170,110,.25)}
.keystone{width:58px;height:58px;flex-shrink:0}
.perk{width:30px;height:30px;border-radius:50%;margin:2px}
.perk.big{width:46px;height:46px}
.perk.pick{opacity:.3;filter:grayscale(1)}
.perk.pick.on{opacity:1;filter:none;box-shadow:0 0 0 2px var(--gold)}
.style{width:36px;height:36px;margin:4px;opacity:.3}
.style.on{opacity:1}
.row{display:flex;justify-content:center;flex-wrap:wrap;margin:8px 0}
.label{color:var(--gold);margin-top:16px;font-weight:700;text-align:center;font-size:14px}
/* ===== LoL Companion: purple & gold theme ===== */
html, body {
  background: radial-gradient(ellipse at top, #2a1450 0%, #120a1f 65%) fixed !important;
  color: #f1e9ff !important;
}
header, nav {
  background: rgba(18, 10, 31, .94) !important;
  border-color: #3a2560 !important;
}
[class*="card"] {
  background: linear-gradient(180deg, #22143a, #1a0f2c) !important;
  border: 1px solid #3a2560 !important;
  border-radius: 14px !important;
}
.logo {
  background: linear-gradient(135deg, #8b5cf6, #e8b84a) !important;
  color: #120a1f !important;
  border: none !important;
  border-radius: 12px !important;
  font-weight: 900 !important;
  box-shadow: 0 0 14px rgba(139, 92, 246, .55) !important;
}
.brand { color: #e8b84a !important; letter-spacing: 2px; }
.muted { color: #a993c9 !important; }
input, select, textarea {
  background: #140b24 !important;
  color: #f1e9ff !important;
  border: 1px solid #3a2560 !important;
}
button { border-color: #8b5cf6 !important; }
button:active { transform: scale(.97); }
.active, .selected {
  border-color: #e8b84a !important;
  box-shadow: 0 0 0 2px #e8b84a, 0 0 12px rgba(232, 184, 74, .45) !important;
}
.slot, .spellSlot, .rec, .preview, #chatBox, .tabs button:not(.on) { background: #170d2a !important; }
.icon.empty { background: #0f0820 !important; }
</style>
</head>
<body>
<header>
<div class="logo">LC</div>
<div>
<div class="brand">LOL COMPANION</div>
<div class="status"><span id="dot" class="dot"></span><span id="state">Connecting...</span></div>
</div>
</header>

<main>
<section id="tab-play" class="tab on">

<div id="idleView" class="card center">
<div id="spinner" class="spinner" style="display:none"></div>
<div id="idleTitle" class="big">Ready when you are</div>
<div id="idleText" class="muted">Queue up in League to start.</div>
<div id="qrBox" style="display:none">
<img src="/qr.png" class="qr">
<div class="muted">Scan with your phone camera, or open the LoL Companion app on the same Wi-Fi. It finds this PC automatically.</div>
</div>
</div>

<div id="inviteView"></div>

<div id="lobbyView" style="display:none">
<div class="card">
<h3>Play</h3>
<div id="lobbyNone">
<label class="field"><span>Game mode</span><select id="queueSelect"></select></label>
<button onclick="createLobby()">Create lobby</button>
</div>
<div id="lobbyIn" style="display:none">
<div id="lobbyQueue" class="big center"></div>
<div id="lobbyMembers"></div>
<div id="roleBox" class="spellSlots" style="display:none">
<label class="field" style="flex:1"><span>Main role</span><select id="role1" onchange="setRoles()"></select></label>
<label class="field" style="flex:1"><span>Second role</span><select id="role2" onchange="setRoles()"></select></label>
</div>
<div id="searchInfo" class="center muted" style="margin-top:10px"></div>
<button id="findBtn" class="green" onclick="findMatch()">Find match</button>
<button class="ghost" onclick="leaveLobby()">Leave lobby</button>
</div>
</div>

<div class="card">
<h3>Friends <span class="muted" id="friendCount"></span></h3>
<input id="friendSearch" placeholder="Search friends..." oninput="renderFriends()">
<div id="friendList" style="margin-top:10px;max-height:360px;overflow-y:auto"></div>
</div>
</div>

<div id="readyView" class="card center" style="display:none">
<div class="found">MATCH FOUND</div>
<button id="accept" onclick="send('/accept')">ACCEPT</button>
<button class="red" onclick="send('/decline')">Decline</button>
</div>

<div id="selectView" style="display:none">
<div class="card">
<div id="phaseName"></div>
<div id="timer"></div>
<div id="offers"></div>
<div id="turn"></div>
<div class="bans"><div id="allyBans"></div><div id="enemyBans"></div></div>
<div class="teams"><div id="myTeam"></div><div id="theirTeam"></div></div>
</div>

<div id="pickArea" class="card" style="display:none">
<input id="search" placeholder="Search champion..." oninput="renderGrid()">
<div id="grid"></div>
<button id="lockBtn" onclick="lockIn()">LOCK IN</button>
</div>

<div class="card">
<h3>Runes</h3>
<div id="activeRunes" class="muted">No rune page selected</div>
<button class="ghost" onclick="tab('runes')">Change runes</button>
</div>

<div class="card">
<h3>Summoner spells</h3>
<div class="spellSlots">
<div id="slot1" class="spellSlot on" onclick="pickSlot(1)"></div>
<div id="slot2" class="spellSlot" onclick="pickSlot(2)"></div>
</div>
<div id="spellGrid" class="row"></div>
</div>

<div id="skinArea" class="card" style="display:none">
<h3>Skin</h3>
<img id="skinPreview" class="preview" style="display:none">
<div id="skinGrid"></div>
<div class="center" id="chromas"></div>
<button class="ghost" onclick="randomSkin()">Random skin</button>
</div>

<div class="card">
<h3>Ward skin</h3>
<div id="wardGrid"></div>
</div>

<div class="card">
<h3>Team chat</h3>
<div id="chatBox"></div>
<div class="chatRow">
<input id="chatInput" placeholder="Message..." onkeydown="if(event.key==='Enter')sendChat()">
<button onclick="sendChat()">Send</button>
</div>
</div>
</div>
</section>

<section id="tab-runes" class="tab">
<div class="card">
<h3>Runes <span class="muted" id="runeChamp"></span></h3>
<div class="tabs">
<button id="tabMine" class="on" onclick="runeTabSet('mine')">My pages</button>
<button id="tabRec" onclick="runeTabSet('rec')">Recommended</button>
<button id="tabEd" onclick="runeTabSet('ed')">Editor</button>
</div>
<div id="mineView"></div>
<div id="recView" style="display:none"></div>
<div id="edView" style="display:none"></div>
</div>
</section>

<section id="tab-settings" class="tab">
<div class="card">
<h3>Automation</h3>
<label class="setting">Auto accept<input type="checkbox" id="autoAccept" onchange="save()"></label>
<label class="setting">Auto lock champion<input type="checkbox" id="autoPick" onchange="save()"></label>
<label class="setting">Auto runes + spells<input type="checkbox" id="autoSetup" onchange="save()"></label>
</div>
<div class="card">
<h3>Loadout</h3>
<label class="field"><span>Champion</span>
<input id="champion" list="champList" placeholder="Search champion..." onfocus="this.select()" onchange="save()">
</label>
<datalist id="champList"></datalist>
<label class="field"><span>Rune page</span><select id="runePage" onchange="save()"></select></label>
<label class="field"><span id="key1Label">Spell 1 (D)</span><select id="spell1" onchange="spellSelectChanged()"></select></label>
<label class="field"><span id="key2Label">Spell 2 (F)</span><select id="spell2" onchange="spellSelectChanged()"></select></label>
<button class="ghost" onclick="load()">Reload champions, runes & spells</button>
</div>
<div class="card">
<h3>Notifications</h3>
<div class="muted" id="topic"></div>
</div>
</section>
</main>

<nav>
<button data-tab="play" class="on" onclick="tab('play')"><b>⚔</b>Play</button>
<button data-tab="runes" onclick="tab('runes')"><b>✦</b>Runes</button>
<button data-tab="settings" onclick="tab('settings')"><b>⚙</b>Settings</button>
</nav>
<div id="saved" class="toast"></div>

<script>
const ASSET = p => p ? "/asset?path=" + encodeURIComponent(p) : "";
const ICON = id => ASSET("/lol-game-data/assets/v1/champion-icons/" + id + ".png");
const PIMG = p => ASSET(p);
const POS = {top:"Top",jungle:"Jungle",middle:"Mid",bottom:"ADC",utility:"Support"};
const PHASES = {PLANNING:"Planning",BAN_PICK:"Ban & Pick",FINALIZATION:"Finalizing",GAME_STARTING:"Starting"};
const STATUS = {None:"In client",Lobby:"In lobby",Matchmaking:"In queue",ReadyCheck:"Match found!",ChampSelect:"Champ select",GameStart:"Loading game",InProgress:"In game",WaitingForStats:"Game over",PreEndOfGame:"Game over",EndOfGame:"Game over"};
const SWAPS = [["trades","trades","Trade"],["pickOrderSwaps","swaps","Swap order"],["positionSwaps","position-swaps","Swap role"]];

let names = {};
let champs = {};
let gridList = [];
let gridType = "";
let selected = 0;
let meId = "";
let chatCount = -1;
let skinChamp = 0;
let skins = [];
let skinKey = "";
let savedChampId = 0;
let lastPhase = "";
let timerBase = 0;
let timerAt = 0;
let timerKey = "";
let clockOffset = 0;
let wards = [];
let currentWard = 0;

let spellData = [];
let spellInfo = { level: 0, mode: "CLASSIC" };
let spellSlot = 1;
let curSpells = [4, 14];
let keys = ["D", "F"];
let spellKey = "";

let styles = [];
let perkInfo = {};
let recs = [];
let recKey = "";
let curChamp = 0;
let curPos = "NONE";
let myPages = [];
let editPageId = 0;
let editPageName = "";
let ed = { primary: 0, sub: 0, prim: [0, 0, 0, 0], sec: {}, secOrder: [], shards: [0, 0, 0] };

const getJson = async url => (await fetch(url)).json();
const show = (id, visible) => document.getElementById(id).style.display = visible ? "block" : "none";

function esc(text) {
  const d = document.createElement("div");
  d.textContent = text || "";
  return d.innerHTML;
}

function flash(text) {
  const saved = document.getElementById("saved");
  saved.textContent = text;
  setTimeout(() => saved.textContent = "", 1500);
}

function tab(name) {
  document.querySelectorAll(".tab").forEach(t => t.classList.toggle("on", t.id === "tab-" + name));
  document.querySelectorAll("nav button").forEach(b => b.classList.toggle("on", b.dataset.tab === name));
  window.scrollTo(0, 0);
}

async function syncClock() {
  const start = Date.now();
  const pcNow = await getJson("/now");
  const end = Date.now();
  clockOffset = pcNow - (start + end) / 2;
}

function tickTimer() {
  if (!timerAt) return;
  const left = Math.max(0, timerBase - (Date.now() - timerAt));
  const el = document.getElementById("timer");
  el.textContent = Math.ceil(left / 1000) + "s";
  el.style.color = left < 10000 ? "var(--red)" : "";
}

async function loadWards() {
  const all = await getJson("/wards");
  wards = Array.isArray(all) ? all.filter(w => w.id === 0 || (w.ownership && w.ownership.owned)) : [];
  currentWard = await getJson("/currentward");
  renderWards();
}

function renderWards() {
  document.getElementById("wardGrid").innerHTML = wards.length
    ? wards.map(w => `<div class="cell" onclick="setWard(${w.id})"><img src="${ASSET(w.wardImagePath)}" class="${w.id === currentWard ? "selected" : ""}"><br>${esc(w.name)}</div>`).join("")
    : `<p class="muted">No ward skins found.</p>`;
}

async function setWard(id) {
  const res = await fetch("/ward/" + id, { method: "POST" });
  const result = await res.text();
  if (result !== "ok") {
    alert(result);
    return;
  }
  currentWard = id;
  renderWards();
  flash("Ward set ✓");
}

async function loadNames() {
  const all = await getJson("/allchampions");
  names = {};
  all.forEach(c => { if (c.id > 0) names[c.id] = c.name; });
}

async function loadSpells() {
  const all = await getJson("/summonerspells");
  spellData = Array.isArray(all) ? all.filter(s => s.id > 0 && s.name) : [];
  const info = await getJson("/spellinfo");
  if (info.mode) spellInfo = info;
  spellKey = "";
  fillSpellSelects();
  await loadKeys();
}

async function loadKeys() {
  const k = await getJson("/keybinds");
  if (k.spell1) keys = [k.spell1, k.spell2];
  document.getElementById("key1Label").textContent = "Spell 1 (" + keys[0] + ")";
  document.getElementById("key2Label").textContent = "Spell 2 (" + keys[1] + ")";
  spellKey = "";
}

function availableSpells() {
  const level = spellInfo.level || 999;
  return spellData
    .filter(s => (s.gameModes || []).includes(spellInfo.mode) && (s.summonerLevel || 0) <= level)
    .sort((x, y) => x.name.localeCompare(y.name));
}

function spellById(id) {
  return spellData.find(s => s.id === id);
}

function spellName(id) {
  const s = spellById(id);
  return s ? s.name : "";
}

function spellImg(id) {
  const s = spellById(id);
  return s ? PIMG(s.iconPath) : "";
}

function fillSpellSelects() {
  const list = availableSpells();
  ["spell1", "spell2"].forEach((id, i) => {
    const select = document.getElementById(id);
    select.innerHTML = "";
    list.forEach(s => select.add(new Option(s.name, s.id)));
    select.value = curSpells[i];
  });
}

async function load() {
  const settings = await getJson("/status");
  document.getElementById("autoAccept").checked = settings.autoAccept;
  document.getElementById("autoPick").checked = settings.autoPick;
  document.getElementById("autoSetup").checked = settings.autoSetup;
  document.getElementById("topic").innerHTML = "Install the <b>ntfy</b> app and subscribe to:<br><b>" + esc(settings.ntfyTopic) + "</b>";
  savedChampId = settings.championId;
  curSpells = [settings.spell1, settings.spell2];

  const list = await getJson("/champions");
  const champList = document.getElementById("champList");
  champList.innerHTML = "";
  champs = {};
  list
    .filter(c => c.id > 0 && (c.freeToPlay || (c.ownership && c.ownership.owned)))
    .sort((x, y) => x.name.localeCompare(y.name))
    .forEach(c => {
      champs[c.name] = c.id;
      champList.appendChild(new Option(c.name));
      if (c.id === settings.championId) document.getElementById("champion").value = c.name;
    });

  await loadNames();
  await loadRuneData();
  await renderMyPages();
  document.getElementById("runePage").value = settings.runePageId;
  await loadSpells();
  recKey = "";
}

function renderIdle(phase, connected) {
  let title = "Ready when you are";
  let text = "Queue up in League to start.";
  let spin = false;

  if (!connected) {
    title = "League isn't open";
    text = "Open League of Legends on your PC.";
  } else if (phase === "Matchmaking") {
    title = "Searching for a match";
    text = "You'll get a notification when it pops.";
    spin = true;
  } else if (phase === "GameStart" || phase === "InProgress") {
    title = "In game";
    text = "Good luck, have fun!";
  } else if (["WaitingForStats", "PreEndOfGame", "EndOfGame"].includes(phase)) {
    title = "Game over";
    text = "Queue up again when you're ready.";
  }

  document.getElementById("idleTitle").textContent = title;
  document.getElementById("idleText").textContent = text;
  show("spinner", spin);
}

async function refresh() {
  const b = await getJson("/status");
  const connected = !b.status.startsWith("Waiting");
  document.getElementById("state").textContent = connected ? (STATUS[b.status] || b.status) : "League isn't open";
  document.getElementById("dot").className = connected ? "dot live" : "dot";

  if (b.phase !== lastPhase) {
    if (b.phase === "ReadyCheck" || b.phase === "ChampSelect") tab("play");
    if (b.phase === "ReadyCheck" && navigator.vibrate) navigator.vibrate([400, 150, 400]);
    if (b.phase === "ChampSelect" || b.phase === "Lobby") await loadSpells();
    if (b.phase === "ChampSelect") {
      await syncClock();
      await renderMyPages();
      await loadWards();
    }
    lastPhase = b.phase;
  }

  const inSelect = b.phase === "ChampSelect";
  show("readyView", b.phase === "ReadyCheck");
  show("selectView", inSelect);
  show("idleView", b.phase !== "ReadyCheck" && !inSelect);
  renderIdle(b.phase, connected);

  const lobbyPhase = connected && ["None", "Lobby", "Matchmaking"].includes(b.phase);
  show("lobbyView", lobbyPhase);
  if (connected && Date.now() - friendsAt > 5000) {
    friendsAt = Date.now();
    await loadFriends();
  }
  if (lobbyPhase) await renderLobby(b.phase);
  if (connected) await renderInvites();

  if (inSelect) {
    await renderSelect();
  } else {
    gridType = "";
    chatCount = -1;
    skinChamp = 0;
    curChamp = 0;
    curPos = "NONE";
    timerAt = 0;
  }

  await renderRunes();
}

function slot(p, isMe, s, offers) {
  const id = p.championId || p.championPickIntent;
  const faded = !p.championId && p.championPickIntent ? " faded" : "";
  const icon = id ? `<img src="${ICON(id)}" class="icon${faded}">` : `<div class="icon empty"></div>`;
  const champ = id ? (names[id] || "") : "Picking...";
  const role = POS[p.assignedPosition] || "";
  const who = p.gameName ? esc(p.gameName) : (role || "Teammate");
  const line2 = (p.gameName ? esc(p.gameName) + " · " : "") + role + (isMe ? " (you)" : "");

  let buttons = "";
  if (s && !isMe) {
    SWAPS.forEach(([key, kind, label]) => {
      (s[key] || []).filter(x => x.cellId === p.cellId).forEach(x => {
        if (x.state === "AVAILABLE")
          buttons += `<button class="mini" onclick="swap('${kind}',${x.id},'request')">${label}</button>`;
        if (x.state === "SENT")
          buttons += `<button class="mini" onclick="swap('${kind}',${x.id},'cancel')">Cancel</button>`;
        if (x.state === "RECEIVED")
          offers.push(`<div class="offer">${who} wants to ${label.toLowerCase()}<br>
            <button class="mini green" onclick="swap('${kind}',${x.id},'accept')">Accept</button>
            <button class="mini red" onclick="swap('${kind}',${x.id},'decline')">Decline</button></div>`);
      });
    });
  }

  return `<div class="slot${isMe ? " me" : ""}">${icon}<div><b>${champ}</b><br><small>${line2}</small><br>${buttons}</div></div>`;
}

async function renderSelect() {
  if (Object.keys(names).length === 0) await loadNames();
  if (!spellData.length) await loadSpells();
  if (!wards.length) await loadWards();
  if (!meId) {
    const m = await getJson("/chatme");
    meId = m.id || "";
  }

  const s = await getJson("/champselect");
  if (!s.myTeam) return;
  const me = s.localPlayerCellId;
  const mine = s.myTeam.find(p => p.cellId === me);

  if (mine) {
    curChamp = mine.championId || mine.championPickIntent || 0;
    curPos = (mine.assignedPosition || "none").toUpperCase();
    if (mine.spell1Id && mine.spell2Id) renderSpells(mine.spell1Id, mine.spell2Id);
  }

  const t = s.timer || {};
  document.getElementById("phaseName").textContent = PHASES[t.phase] || "";
  const tKey = (t.phase || "") + ":" + (t.internalNowInEpochMs || 0);
  if (tKey !== timerKey) {
    timerKey = tKey;
    timerBase = t.adjustedTimeLeftInPhase || 0;
    timerAt = t.internalNowInEpochMs ? t.internalNowInEpochMs - clockOffset : Date.now();
    tickTimer();
  }

  const allyBans = [];
  const enemyBans = [];
  let myAction = null;
  let myPickDone = false;

  s.actions.flat().forEach(a => {
    if (a.type === "ban" && a.completed && a.championId > 0)
      (a.isAllyAction ? allyBans : enemyBans).push(a.championId);
    if (a.actorCellId === me && a.isInProgress && !a.completed && (a.type === "ban" || a.type === "pick"))
      myAction = a;
    if (a.actorCellId === me && a.type === "pick" && a.completed)
      myPickDone = true;
  });

  document.getElementById("allyBans").innerHTML = allyBans.map(id => `<img src="${ICON(id)}">`).join("");
  document.getElementById("enemyBans").innerHTML = enemyBans.map(id => `<img src="${ICON(id)}">`).join("");

  const offers = [];
  document.getElementById("myTeam").innerHTML = s.myTeam.map(p => slot(p, p.cellId === me, s, offers)).join("");
  document.getElementById("theirTeam").innerHTML = s.theirTeam.map(p => slot(p, false, null, null)).join("");
  document.getElementById("offers").innerHTML = offers.join("");

  let mode = "";
  if (myAction) mode = myAction.type;
  else if (t.phase === "PLANNING" && !myPickDone) mode = "intent";

  const turn = document.getElementById("turn");
  if (mode) {
    turn.textContent = mode === "ban" ? "YOUR TURN TO BAN" : mode === "pick" ? "YOUR TURN TO PICK" : "PLANNING: SHOW YOUR PICK";
    turn.className = mode;

    if (gridType !== mode) {
      gridType = mode;
      selected = 0;
      document.getElementById("search").value = "";
      const ids = await getJson(mode === "ban" ? "/bannable" : "/pickable");
      gridList = ids.filter(id => names[id]).sort((x, y) => names[x].localeCompare(names[y]));
      document.getElementById("lockBtn").textContent = mode === "ban" ? "BAN" : "LOCK IN";
      show("lockBtn", mode !== "intent");
      renderGrid();
    }
    show("pickArea", true);
  } else {
    turn.textContent = "";
    turn.className = "";
    gridType = "";
    show("pickArea", false);
  }

  if (myPickDone && mine && mine.championId) {
    show("skinArea", true);
    await renderSkins(mine.championId, mine.selectedSkinId);
  } else {
    show("skinArea", false);
  }

  await renderChat(s.myTeam);
}

function renderSpells(s1, s2) {
  curSpells = [s1, s2];
  const key = s1 + ":" + s2 + ":" + spellSlot + ":" + spellData.length + ":" + spellInfo.mode + ":" + keys.join();
  if (key === spellKey) return;
  spellKey = key;

  [1, 2].forEach(n => {
    const id = curSpells[n - 1];
    const el = document.getElementById("slot" + n);
    el.className = "spellSlot" + (spellSlot === n ? " on" : "");
    el.innerHTML = `<img src="${spellImg(id)}"><div><b>${esc(keys[n - 1])}</b><br>${esc(spellName(id))}</div>`;
  });

  document.getElementById("spellGrid").innerHTML = availableSpells()
    .map(s => `<img class="spell${s.id === s1 || s.id === s2 ? " used" : ""}" src="${PIMG(s.iconPath)}" title="${esc(s.name)}" onclick="chooseSpell(${s.id})">`)
    .join("");
}

function pickSlot(n) {
  spellSlot = n;
  spellKey = "";
  renderSpells(curSpells[0], curSpells[1]);
}

async function chooseSpell(id) {
  let a = curSpells[0];
  let b = curSpells[1];

  if (spellSlot === 1) {
    if (id === b) b = a;
    a = id;
  } else {
    if (id === a) a = b;
    b = id;
  }

  await setSpells(a, b);
}

async function setSpells(a, b) {
  await fetch(`/setspells/${a}/${b}`, { method: "POST" });
  curSpells = [a, b];
  document.getElementById("spell1").value = a;
  document.getElementById("spell2").value = b;
  spellKey = "";
  renderSpells(a, b);
  flash("Spells set ✓");
}

async function spellSelectChanged() {
  const a = +document.getElementById("spell1").value;
  const b = +document.getElementById("spell2").value;
  if (a === b) {
    alert("Pick two different spells");
    document.getElementById("spell1").value = curSpells[0];
    document.getElementById("spell2").value = curSpells[1];
    return;
  }
  await setSpells(a, b);
}

function renderGrid() {
  const q = document.getElementById("search").value.toLowerCase();
  document.getElementById("grid").innerHTML = gridList
    .filter(id => names[id].toLowerCase().includes(q))
    .map(id => `<div class="cell" onclick="choose(${id})"><img src="${ICON(id)}" class="${id === selected ? "selected" : ""}"><br>${names[id]}</div>`)
    .join("");
}

async function renderSkins(champ, sel) {
  if (skinChamp !== champ) {
    skinChamp = champ;
    const all = await getJson("/skins");
    skins = Array.isArray(all) ? all.filter(x => x.unlocked && !x.disabled) : [];
    skinKey = "";
  }

  const key = champ + ":" + sel;
  if (key === skinKey) return;
  skinKey = key;

  const base = skins.find(x => x.id === sel || (x.childSkins || []).some(c => c.id === sel));
  const chroma = base ? (base.childSkins || []).find(c => c.id === sel) : null;
  const previewPath = chroma && chroma.chromaPreviewPath ? chroma.chromaPreviewPath : (base ? (base.splashPath || base.tilePath) : "");

  const preview = document.getElementById("skinPreview");
  preview.src = ASSET(previewPath);
  preview.style.display = previewPath ? "block" : "none";

  document.getElementById("skinGrid").innerHTML = skins
    .map(x => `<div class="cell skin" onclick="setSkin(${x.id})"><img src="${ASSET(x.tilePath || x.splashPath)}" class="${base && base.id === x.id ? "selected" : ""}"><br>${esc(x.name)}</div>`)
    .join("");

  const chromas = base ? (base.childSkins || []).filter(c => c.unlocked) : [];
  document.getElementById("chromas").innerHTML = chromas
    .map(c => `<span class="chroma${c.id === sel ? " selected" : ""}" style="background:${(c.colors && c.colors[0]) || "#888"}" onclick="setSkin(${c.id})"></span>`)
    .join("");
}

async function renderChat(team) {
  const msgs = await getJson("/chat");
  if (!Array.isArray(msgs) || msgs.length === chatCount) return;
  chatCount = msgs.length;

  const box = document.getElementById("chatBox");
  box.innerHTML = msgs.map(m => {
    if (m.type === "system") return `<div class="sys">${esc(m.body)}</div>`;
    let from = "Ally";
    if (m.fromId === meId) {
      from = "You";
    } else {
      const p = team.find(x => x.puuid && m.fromPid && m.fromPid.startsWith(x.puuid));
      if (p && p.gameName) from = p.gameName;
    }
    return `<div><b>${esc(from)}:</b> ${esc(m.body)}</div>`;
  }).join("");
  box.scrollTop = box.scrollHeight;
}

async function loadRuneData() {
  const s = await getJson("/perkstyles");
  styles = Array.isArray(s) ? s : [];
  const list = await getJson("/perks");
  perkInfo = {};
  if (Array.isArray(list)) list.forEach(p => perkInfo[p.id] = p);
  if (styles.length > 1 && !ed.primary) {
    ed.primary = styles[0].id;
    ed.sub = styles[1].id;
  }
}

function styleById(id) {
  return styles.find(s => s.id === id);
}

function rows(styleId, type) {
  const s = styleById(styleId);
  return s ? s.slots.filter(x => x.type === type) : [];
}

function perkImg(id) {
  return PIMG(perkInfo[id] && perkInfo[id].iconPath);
}

function perkName(id) {
  return perkInfo[id] ? perkInfo[id].name : "";
}

function runeTabSet(name) {
  show("mineView", name === "mine");
  show("recView", name === "rec");
  show("edView", name === "ed");
  document.getElementById("tabMine").className = name === "mine" ? "on" : "";
  document.getElementById("tabRec").className = name === "rec" ? "on" : "";
  document.getElementById("tabEd").className = name === "ed" ? "on" : "";
  if (name === "ed") renderEditor();
  if (name === "mine") renderMyPages();
}

async function renderMyPages() {
  if (!styles.length) await loadRuneData();
  const data = await getJson("/runepages");
  const all = Array.isArray(data) ? data : [];
  myPages = all.filter(p => p.isDeletable);

  const select = document.getElementById("runePage");
  const prev = select.value;
  select.innerHTML = "";
  select.add(new Option("Don't change", 0));
  all.forEach(p => select.add(new Option(p.name, p.id)));
  select.value = prev || 0;
  if (!select.value) select.value = 0;

  const active = all.find(p => p.current || p.isActive);
  document.getElementById("activeRunes").innerHTML = active
    ? `<div class="rec current"><img class="keystone" src="${perkImg((active.selectedPerkIds || [])[0])}"><div><b>${esc(active.name)}</b><br>${(active.selectedPerkIds || []).slice(1, 6).map(id => `<img class="perk" src="${perkImg(id)}">`).join("")}</div></div>`
    : "No rune page selected";

  let html = myPages.length ? myPages.map((p, i) => {
    const ids = p.selectedPerkIds || [];
    const isActive = p.current || p.isActive;
    return `<div class="rec${isActive ? " current" : ""}">
      <img class="keystone" src="${perkImg(ids[0])}">
      <div>
        <b>${esc(p.name)}</b>${isActive ? ` <small class="muted">· active</small>` : ""}<br>
        ${ids.slice(1, 6).map(id => `<img class="perk" src="${perkImg(id)}">`).join("")}<br>
        <button class="mini green" onclick="usePage(${i})">Use</button>
        <button class="mini" onclick="editPage(${i})">Edit</button>
      </div>
    </div>`;
  }).join("") : `<p class="muted">You don't have any saved rune pages yet.</p>`;

  html += `<button class="ghost" onclick="newPage()">+ Create new page</button>`;
  document.getElementById("mineView").innerHTML = html;
}

async function usePage(i) {
  const p = myPages[i];
  await fetch("/usepage/" + p.id, { method: "POST" });
  await renderMyPages();
  flash("Rune page set ✓");
}

function loadIntoEditor(primary, sub, ids) {
  ed.primary = primary;
  ed.sub = sub;
  ed.prim = ids.slice(0, 4);
  ed.sec = {};
  ed.secOrder = [];

  const subRows = rows(sub, "kMixedRegularSplashable");
  ids.slice(4, 6).forEach(id => {
    const row = subRows.findIndex(x => x.perks.includes(id));
    if (row >= 0) {
      ed.sec[row] = id;
      ed.secOrder.push(row);
    }
  });

  ed.shards = ids.slice(6, 9);
}

function editPage(i) {
  const p = myPages[i];
  loadIntoEditor(p.primaryStyleId, p.subStyleId, p.selectedPerkIds || []);
  editPageId = p.id;
  editPageName = p.name;
  runeTabSet("ed");
}

function newPage() {
  ed = { primary: styles[0] ? styles[0].id : 0, sub: styles[1] ? styles[1].id : 0, prim: [0, 0, 0, 0], sec: {}, secOrder: [], shards: [0, 0, 0] };
  editPageId = 0;
  editPageName = "";
  runeTabSet("ed");
}

async function renderRunes() {
  if (!styles.length) await loadRuneData();
  const champ = curChamp || savedChampId;
  document.getElementById("runeChamp").textContent = names[champ] ? "· " + names[champ] : "";

  const key = champ + ":" + curPos;
  if (!champ || key === recKey) return;
  recKey = key;

  const data = await getJson(`/recommended/${champ}/${curPos}`);
  recs = Array.isArray(data) ? data : [];

  document.getElementById("recView").innerHTML = recs.length ? recs.map((r, i) => {
    const ids = (r.perks || []).map(p => p.id);
    const title = r.recommendationDescriptor || perkName(ids[0]) || "Recommended";
    const spellText = (r.summonerSpellIds || []).map(spellName).filter(x => x).join(" + ");
    return `<div class="rec">
      <img class="keystone" src="${perkImg(ids[0])}">
      <div>
        <b>${esc(title)}</b><br>
        ${ids.slice(1, 6).map(id => `<img class="perk" src="${perkImg(id)}">`).join("")}<br>
        <small class="muted">${esc(spellText)}</small><br>
        <button class="mini green" onclick="useRec(${i})">Use</button>
        <button class="mini" onclick="editRec(${i})">Edit</button>
      </div>
    </div>`;
  }).join("") : `<p class="muted">No recommendations for this champion.</p>`;
}

async function useRec(i) {
  const r = recs[i];
  const ids = (r.perks || []).map(p => p.id);
  await applyRunes(r.primaryPerkStyleId, r.secondaryPerkStyleId, ids, 0, "");
  if (r.summonerSpellIds && r.summonerSpellIds.length === 2)
    await setSpells(r.summonerSpellIds[0], r.summonerSpellIds[1]);
}

function editRec(i) {
  const r = recs[i];
  loadIntoEditor(r.primaryPerkStyleId, r.secondaryPerkStyleId, (r.perks || []).map(p => p.id));
  editPageId = 0;
  editPageName = "";
  runeTabSet("ed");
}

function renderEditor() {
  if (!styles.length) {
    document.getElementById("edView").innerHTML = `<p class="muted">Open League first.</p>`;
    return;
  }

  const styleRow = (sel, fn, exclude) => styles
    .filter(s => s.id !== exclude)
    .map(s => `<img class="style${s.id === sel ? " on" : ""}" src="${PIMG(s.iconPath)}" onclick="${fn}(${s.id})">`)
    .join("");

  const perkRow = (row, chosen, fn, r, big) => `<div class="row">${row.perks
    .map(id => `<img class="perk pick${big ? " big" : ""}${chosen === id ? " on" : ""}" src="${perkImg(id)}" onclick="${fn}(${r},${id})">`)
    .join("")}</div>`;

  const primRows = [...rows(ed.primary, "kKeyStone"), ...rows(ed.primary, "kMixedRegularSplashable")];
  const subRows = rows(ed.sub, "kMixedRegularSplashable");
  const shardRows = rows(ed.primary, "kStatMod");
  const primName = styleById(ed.primary) ? styleById(ed.primary).name : "";
  const subName = styleById(ed.sub) ? styleById(ed.sub).name : "";

  let html = `<div class="center muted">${editPageId ? "Editing · <b>" + esc(editPageName) + "</b>" : "New page"}</div>`;
  html += `<div class="label">Primary · ${esc(primName)}</div><div class="row">${styleRow(ed.primary, "setPrimary", 0)}</div>`;
  primRows.forEach((row, r) => html += perkRow(row, ed.prim[r], "pickPrim", r, r === 0));
  html += `<div class="label">Secondary · ${esc(subName)}</div><div class="row">${styleRow(ed.sub, "setSub", ed.primary)}</div>`;
  subRows.forEach((row, r) => html += perkRow(row, ed.sec[r], "pickSec", r, false));
  html += `<div class="label">Shards</div>`;
  shardRows.forEach((row, r) => html += perkRow(row, ed.shards[r], "pickShard", r, false));
  html += `<button onclick="applyEditor()">${editPageId ? "Save page" : "Apply runes"}</button>`;

  document.getElementById("edView").innerHTML = html;
}

function setPrimary(id) {
  ed.primary = id;
  ed.prim = [0, 0, 0, 0];
  if (ed.sub === id) {
    ed.sub = styles.find(s => s.id !== id).id;
    ed.sec = {};
    ed.secOrder = [];
  }
  renderEditor();
}

function setSub(id) {
  ed.sub = id;
  ed.sec = {};
  ed.secOrder = [];
  renderEditor();
}

function pickPrim(r, id) {
  ed.prim[r] = id;
  renderEditor();
}

function pickSec(r, id) {
  ed.sec[r] = id;
  ed.secOrder = ed.secOrder.filter(x => x !== r);
  ed.secOrder.push(r);
  if (ed.secOrder.length > 2) {
    const old = ed.secOrder.shift();
    delete ed.sec[old];
  }
  renderEditor();
}

function pickShard(r, id) {
  ed.shards[r] = id;
  renderEditor();
}

async function applyEditor() {
  const secIds = Object.keys(ed.sec).sort().map(k => ed.sec[k]);
  const ids = [...ed.prim, ...secIds, ...ed.shards];
  if (ids.length !== 9 || ids.includes(0)) {
    alert("Pick all runes first");
    return;
  }
  await applyRunes(ed.primary, ed.sub, ids, editPageId, editPageName);
}

async function applyRunes(primary, sub, ids, replaceId, keepName) {
  const keystone = perkName(ids[0]);
  const champ = names[curChamp || savedChampId] || "";
  const pageName = keepName || [champ, keystone].filter(x => x).join(" - ") || "Runes";

  const res = await fetch("/runes?replace=" + (replaceId || 0), {
    method: "POST",
    headers: {"Content-Type": "application/json"},
    body: JSON.stringify({ name: pageName, primaryStyleId: primary, subStyleId: sub, selectedPerkIds: ids, current: true })
  });
  const result = await res.text();
  if (!result.startsWith("ok")) {
    alert(result);
    return;
  }

  const newId = +result.split(":")[1] || 0;
  if (editPageId) editPageId = newId;

  document.getElementById("runePage").value = 0;
  await save();
  await renderMyPages();
  flash("Runes set ✓");
}

async function choose(id) {
  selected = id;
  renderGrid();
  await fetch("/hover/" + id, { method: "POST" });
}

async function lockIn() {
  if (!selected) {
    alert("Pick a champion first");
    return;
  }
  await fetch("/lock/" + selected, { method: "POST" });
  selected = 0;
}

async function swap(kind, id, verb) {
  await fetch(`/swap/${kind}/${id}/${verb}`, { method: "POST" });
}

async function setSkin(id) {
  await fetch("/skin/" + id, { method: "POST" });
  skinKey = "";
}

function randomSkin() {
  if (skins.length) setSkin(skins[Math.floor(Math.random() * skins.length)].id);
}

async function sendChat() {
  const input = document.getElementById("chatInput");
  if (!input.value.trim()) return;
  await fetch("/chat", { method: "POST", body: input.value });
  input.value = "";
  chatCount = -1;
}

async function send(url) {
  await fetch(url, { method: "POST" });
  refresh();
}

async function save() {
  const champName = document.getElementById("champion").value;
  if (champName in champs) savedChampId = champs[champName];

  await fetch("/settings", {
    method: "POST",
    headers: {"Content-Type": "application/json"},
    body: JSON.stringify({
      autoAccept: document.getElementById("autoAccept").checked,
      autoPick: document.getElementById("autoPick").checked,
      autoSetup: document.getElementById("autoSetup").checked,
      championId: savedChampId,
      runePageId: +document.getElementById("runePage").value,
      spell1: curSpells[0],
      spell2: curSpells[1]
    })
  });

  recKey = "";
  flash("Saved ✓");
}

const ROLES = [["UNSELECTED", "None"], ["TOP", "Top"], ["JUNGLE", "Jungle"], ["MIDDLE", "Mid"], ["BOTTOM", "ADC"], ["UTILITY", "Support"], ["FILL", "Fill"]];
const GAME_STATUS = {inGame: "In game", championSelect: "Champ select", inQueue: "In queue", outOfGame: "Online"};
const ONLINE_ORDER = {chat: 0, away: 1, dnd: 1, mobile: 2, offline: 3};

let queues = {};
let friends = [];
let friendsAt = 0;
let inLobby = false;
let searching = false;
let roleKey = "";
let inviteKey = "";

function roleName(r) {
  const found = ROLES.find(x => x[0] === r);
  return found ? found[1] : "";
}

function profileIcon(id) {
  return ASSET("/lol-game-data/assets/v1/profile-icons/" + (id || 29) + ".jpg");
}

async function loadQueues() {
  const all = await getJson("/queues");
  if (!Array.isArray(all) || !all.length) return;
  const select = document.getElementById("queueSelect");
  queues = {};
  select.innerHTML = "";
  all
    .filter(q => q.queueAvailability === "Available" && q.isVisible && q.name && (q.category === "PvP" || q.category === "VersusAi"))
    .sort((x, y) => x.id - y.id)
    .forEach(q => {
      const name = q.category === "VersusAi" ? "Co-op vs AI: " + q.name : q.name;
      if (queues[q.id]) return;
      queues[q.id] = name;
      select.add(new Option(name, q.id));
    });
}

async function renderLobby(phase) {
  if (!Object.keys(queues).length) await loadQueues();

  const l = await getJson("/lobby");
  inLobby = !!(l && l.gameConfig);
  show("lobbyNone", !inLobby);
  show("lobbyIn", inLobby);
  if (!inLobby) {
    searching = false;
    roleKey = "";
    return;
  }

  const me = l.localMember || {};
  document.getElementById("lobbyQueue").textContent = queues[l.gameConfig.queueId] || "Lobby";

  document.getElementById("lobbyMembers").innerHTML = (l.members || []).map(m => {
    const name = m.gameName || m.summonerName || "Player";
    const roles = [m.firstPositionPreference, m.secondPositionPreference].map(roleName).filter(r => r && r !== "None").join(" / ");
    const isMe = m.summonerId === me.summonerId;
    const kick = me.isLeader && !isMe ? `<button class="mini red" onclick="kick(${m.summonerId})">Kick</button>` : "";
    return `<div class="slot${isMe ? " me" : ""}"><img class="icon" src="${profileIcon(m.summonerIconId)}"><div><b>${esc(name)}</b>${m.isLeader ? " 👑" : ""}<br><small>${esc(roles)}</small><br>${kick}</div></div>`;
  }).join("");

  const showRoles = !!l.gameConfig.showPositionSelector;
  document.getElementById("roleBox").style.display = showRoles ? "flex" : "none";
  if (showRoles) {
    const key = me.firstPositionPreference + ":" + me.secondPositionPreference;
    if (key !== roleKey) {
      roleKey = key;
      document.getElementById("role1").value = me.firstPositionPreference || "UNSELECTED";
      document.getElementById("role2").value = me.secondPositionPreference || "UNSELECTED";
    }
  }

  searching = phase === "Matchmaking";
  const info = document.getElementById("searchInfo");
  if (searching) {
    const s = await getJson("/search");
    const t = Math.floor(s.timeInQueue || 0);
    const est = s.estimatedQueueTime ? " · estimated " + Math.round(s.estimatedQueueTime) + "s" : "";
    info.textContent = "In queue " + Math.floor(t / 60) + ":" + String(t % 60).padStart(2, "0") + est;
  } else {
    info.textContent = me.isLeader ? "" : "Waiting for the party leader to start";
  }

  const btn = document.getElementById("findBtn");
  btn.textContent = searching ? "Cancel search" : "Find match";
  btn.className = searching ? "red" : "green";
  btn.style.display = me.isLeader ? "block" : "none";
  renderFriends();
}

async function loadFriends() {
  const list = await getJson("/friends");
  friends = Array.isArray(list) ? list : [];
  renderFriends();
}

function friendStatus(f) {
  if (f.availability === "offline") return "Offline";
  if (f.availability === "mobile") return "Mobile";
  const status = f.lol && f.lol.gameStatus;
  if (status && status.startsWith("hosting")) return "In lobby";
  return GAME_STATUS[status] || (f.availability === "away" ? "Away" : "Online");
}

function renderFriends() {
  const q = document.getElementById("friendSearch").value.toLowerCase();
  const online = friends.filter(f => f.availability !== "offline" && f.availability !== "mobile").length;
  document.getElementById("friendCount").textContent = friends.length ? "· " + online + " online" : "";

  const list = friends
    .filter(f => (f.gameName || f.name || "").toLowerCase().includes(q))
    .sort((x, y) => (ONLINE_ORDER[x.availability] ?? 3) - (ONLINE_ORDER[y.availability] ?? 3) || (x.gameName || "").localeCompare(y.gameName || ""));

  document.getElementById("friendList").innerHTML = list.length ? list.map(f => {
    const off = f.availability === "offline" || f.availability === "mobile";
    const button = off ? "" : `<button class="mini" onclick="invite(${f.summonerId})">Invite</button>`;
    return `<div class="slot${off ? " faded" : ""}"><img class="icon" src="${profileIcon(f.icon)}"><div style="flex:1"><b>${esc(f.gameName || f.name || "Friend")}</b><br><small>${friendStatus(f)}</small></div>${button}</div>`;
  }).join("") : `<p class="muted">No friends found.</p>`;
}

async function renderInvites() {
  const list = await getJson("/invites");
  const pending = Array.isArray(list) ? list.filter(i => i.state === "Pending") : [];
  const key = pending.map(i => i.invitationId).join();
  if (key === inviteKey) return;
  if (pending.length && navigator.vibrate) navigator.vibrate([300, 100, 300]);
  inviteKey = key;

  document.getElementById("inviteView").innerHTML = pending.map(i => {
    const f = friends.find(x => x.summonerId === i.fromSummonerId);
    const who = f ? (f.gameName || f.name) : (i.fromSummonerName || "A friend");
    const mode = queues[i.gameConfig && i.gameConfig.queueId] || "a game";
    const id = encodeURIComponent(i.invitationId);
    return `<div class="card center">
      <div class="big">Party invite</div>
      <div class="muted">${esc(who)} invited you to ${esc(mode)}</div>
      <button class="green" onclick="answerInvite('${id}','accept')">Join</button>
      <button class="red" onclick="answerInvite('${id}','decline')">Decline</button>
    </div>`;
  }).join("");
}

async function act(url, okText) {
  const res = await fetch(url, { method: "POST" });
  const result = await res.text();
  if (result !== "ok") {
    alert(result);
    return false;
  }
  if (okText) flash(okText);
  refresh();
  return true;
}

function createLobby() {
  act("/lobby/create/" + document.getElementById("queueSelect").value, "Lobby created ✓");
}

function leaveLobby() {
  act("/lobby/leave", "");
}

function findMatch() {
  act(searching ? "/lobby/cancel" : "/lobby/search", "");
}

async function invite(id) {
  if (!inLobby) {
    const created = await act("/lobby/create/" + document.getElementById("queueSelect").value, "");
    if (!created) return;
    inLobby = true;
    await new Promise(r => setTimeout(r, 800));
  }
  act("/lobby/invite/" + id, "Invite sent ✓");
}

function kick(id) {
  act("/lobby/kick/" + id, "");
}

function answerInvite(id, answer) {
  inviteKey = "";
  act("/invites/" + id + "/" + answer, answer === "accept" ? "Joined ✓" : "");
}

function setRoles() {
  const a = document.getElementById("role1").value;
  const b = document.getElementById("role2").value;
  roleKey = "";
  act("/lobby/roles/" + a + "/" + b, "Roles set ✓");
}

["role1", "role2"].forEach(id => {
  const select = document.getElementById(id);
  ROLES.forEach(([value, text]) => select.add(new Option(text, value)));
});

if (location.hostname === "localhost") show("qrBox", true);

load();
refresh();
setInterval(refresh, 1000);
setInterval(tickTimer, 200);
</script>
</body>
</html>
""";
    }
}