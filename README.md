# LoL Companion

Control League of Legends from your phone. Accept matches, ban, pick, set runes, spells, skins and ward skins, and chat with your team, all from your phone while you're away from your PC.

## Features

- **Match found alerts** – get a phone notification and accept from anywhere in your home
- **Auto accept** – never miss a queue pop
- **Lobby from your phone** – pick a game mode, choose your roles, find a match or cancel
- **Friends & party** – see who's online, invite friends, join parties you're invited to, kick players
- **Messages** – chat with friends and get a notification when someone messages you
- **Profile** – rank, mastery, and your whole collection: skins, chromas, emotes, icons, wards, spells, items
- **Hextech crafting** – open chests, disenchant, unlock and reroll shards, with a confirmation before every craft
- **Shop** – Your Shop offers, buy skins with RP (always asks first), Sanctum banners and your wallet
- **Full champ select on your phone** – bans, picks, teammates, timer, trades and swaps
- **Runes** – use your saved pages, League's recommended pages, or build your own in the editor
- **Summoner spells** – change them live, shown with your own keybinds
- **Skins & chromas** – see and pick your skins with images
- **Ward skins**
- **Team chat**
- Everything saves instantly, no Save button

## Download

1. Go to [Releases](../../releases) and download the latest `LoLCompanion.zip`
2. Unzip it anywhere
3. Run `LoLCompanion.exe`
4. If Windows asks about the firewall, click **Allow access**. Your phone needs this to connect.

No install needed. Windows 10/11 only.

## How to use

1. Open **LoL Companion** on your PC (League can be open or closed)
2. Make sure your phone is on the **same Wi-Fi** as your PC
3. Scan the **QR code** in the PC window with your phone camera
4. On your phone, tap **Share → Add to Home Screen** so it opens like an app
5. Closing the window keeps it running in the tray. Right-click the tray icon → **Start with Windows** so it's always ready.

### Notifications

To get a notification when a match is found:

1. Install the free **ntfy** app ([Android](https://play.google.com/store/apps/details?id=io.heckel.ntfy) / [iPhone](https://apps.apple.com/app/ntfy/id1625396347))
2. Open **Settings** in LoL Companion and copy your topic name
3. In ntfy, tap **+** and subscribe to that topic

Your topic is random and private to you.

## How it works

The program runs on your PC and talks to the League client through its official local connection (the same one the client uses itself). It opens a small web page on your Wi-Fi that your phone connects to. Nothing is sent to any server except the match-found notification.

## FAQ

**My phone can't open the page.**
Make sure both devices are on the same Wi-Fi, and that you allowed LoL Companion through the Windows firewall. You can also run this once in an admin Command Prompt:
```
netsh advfirewall firewall add rule name="LoL Companion" dir=in action=allow protocol=TCP localport=5000
```

**Does it work in my country?**
Yes, on every Riot server except China (Tencent).

**Does it work on Mac?**
Not yet. Windows only.

**Is it safe?**
It doesn't touch the game itself, only the client (lobby, champ select). Use auto lock and other automation at your own risk.

## Build from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```
git clone https://github.com/JamilDbiat/LoLCompanion.git
cd LoLCompanion/LeagueHelper
dotnet run
```

Make a release build:

```
cd LeagueHelper
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

## Legal

LoL Companion isn't endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing Riot Games properties. Riot Games, and all associated properties are trademarks or registered trademarks of Riot Games, Inc.
