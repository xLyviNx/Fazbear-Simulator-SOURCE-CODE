# Fazbear Simulator [Source Code]

This is the full Unity project source code for **Fazbear Simulator**, an abandoned FNaF tycoon/sandbox fan-game originally created for Freddit Jam in 2022.

- Original Game Page: https://gamejolt.com/games/fazbearsim/723526
- Post regarding cancelled features and development ideas: https://gamejolt.com/p/hey-guys-now-that-as-we-ve-all-known-for-years-this-game-will-not-b-aiywhwnt

---

## 1. Engine & Setup Requirements

* **Engine Version:** Unity 2021.3.7f1
* Official production credentials, webhooks, and GameJolt API keys have been removed from this repository. The project **will not function out-of-the-box** until you configure your own backend infrastructure.

### Global DataStore
The game relies on GameJolt's DataStore API for item prices, version checking, and core balance values.
* Review the `/GAMEJOLT_GLOBAL_DATASTORE` folder in this repository.
* You must replicate these global key-value pairs inside your own GameJolt game DataStore, or rewrite the system to read from local files/custom servers.

### Missing IDs & Constants
Several scripts contain placeholder IDs, leaderboard keys, trophy identifiers, Discord Application IDs, and webhook constants. Search the codebase for placeholder strings before attempting to build or run the game.

---

## 2. Forking and Revival Guidelines

If you plan to revive, fix, or expand this project:
- Read `LICENSE` carefully.
- Direct 1:1 reuploads without significant changes are not permitted.
- Never connect your build to the original game's GameJolt keys, webhooks, or player data.
- Keep all derivative projects strictly non-profit and free of generative AI artistic assets.

---

## 3. Original Team Credits

- LyviN - Project Lead, Programming, 3D Models, SFX
- Igrisa - 2D Art, 3D Models, Assistance
- kampe1234 - Support Programming, Testing, 3D Models
- Benkher - Music Provider
- poazdev - Original Soundtrack (OST)
- GIOVyy - Testing and Support

### Third-Party Assets
SFX provided via mixkit.co:
- mixkit-sword-slash-swoosh-1476
- mixkit-truck-crash-with-explosion-1616
- mixkit-construction-tool-box-829
- mixkit-dagger-woosh-1487
- mixkit-metal-medieval-construction-818

---
## 4. Ending notes from the Developer
Please be aware the quality of this code may be bad. Some things are beyond regular stupidity :)
I wish you good luck :)

## 5. Legal Disclaimer

Five Nights at Freddy's and all related concepts, characters, and trademarks belong to Scott Cawthon. This is a non-commercial, non-profit fan project. The original creators are not affiliated with Scott Cawthon, GameJolt, or any third-party revivals.
