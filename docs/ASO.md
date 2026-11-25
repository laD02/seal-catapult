# ASO — tối ưu trang Google Play (en, vi)

Quyết định 2026-11-13 (user: "hãy làm SEO thật tốt cho dự án như repo dự án one-line-fill"). Game chỉ có trên Google
Play, nên "SEO" ở đây là ASO (xếp hạng trong ô tìm kiếm của Play) + trang web `site/` (Google tìm thấy, chứa privacy).
Làm theo đúng cách của repo one-line-fill (`docs/ASO.md` bên đó). File này là nguồn duy nhất cho chữ trên trang Play;
`docs/RELEASE.md` §6 trỏ về đây. Listing: **en-US** (mặc định), **vi-VN**, và châu Âu **en-GB, de-DE, fr-FR, es-ES, it-IT, pl-PL** (2026-11-13, user:
"làm SEO thật tốt nhé, ưu tiên EU"; giao diện game có đủ 7 ngôn ngữ). Thứ tự công sức ở §8.

## 1. Cách Play xếp hạng (điều cần nhớ)

- Play **không có ô từ khoá** như iOS. Play đọc **tên app (30 ký tự)**, **mô tả ngắn (80)**, **mô tả đầy đủ (4000)**.
  Tên nặng nhất, rồi mô tả ngắn, rồi mô tả đầy đủ.
- Lặp từ khoá chính tự nhiên 3–5 lần trong mô tả đầy đủ (~2–3%). Nhồi từ khoá (>5%, liệt kê từ khoá) bị Play hạ hạng
  hoặc gỡ listing.
- **Cấm** trong tên, icon, tên nhà phát triển: "free", "best", "#1", "top", "new", giá, emoji, VIẾT HOA toàn bộ, tên app
  khác (kể cả tên game tham chiếu trong `GAME_DESIGN.md`). Mô tả ngắn cũng tránh lời quảng cáo kiểu "Best arcade game".
- Không dùng từ khoá sai sự thật: game có **hải cẩu** và **gấu bắc cực**, không có chim cánh cụt, nên không dùng "penguin"
  dù thể loại hay dùng (Play phạt từ khoá gây hiểu lầm, người chơi tìm chim cánh cụt cài rồi gỡ làm hỏng retention).
- Sau khi lên Play, số lượt cài, tỉ lệ giữ chân, **điểm sao và số review** quyết định hạng nhiều hơn chữ.
  Chữ chỉ đưa app vào đúng nhóm tìm kiếm.

## 2. Từ khoá

Nguồn (2026-11-13, WebSearch): game cùng loại "máy bắn + phá công trình" và "phóng con vật bay": Crazy Penguin
Catapult (Digital Chocolate, bắn chim cánh cụt vào gấu bắc cực; trang [rawg](https://rawg.io/games/crazy-penguin-catapult-2),
[TV Tropes](https://tvtropes.org/pmwiki/pmwiki.php/VideoGame/CrazyPenguinCatapult)), Bouncy Catapult King
([App Store](https://apps.apple.com/app/id1450757809), "one-tap physics"), The Catapult
([App Store](https://apps.apple.com/hr/app/the-catapult/id1292179808)), Flying penguin
([appgoblin](https://appgoblin.info/apps/com.dmitsoft.flyingpenguin), "catapult launches the penguin"), Penguin Flight
Master ([BlueStacks](https://www.bluestacks.com/campaign/com.Trelans.FlightMaster/en), "launch… slingshot… upgrades"),
Penguin Homerun ([playgama](https://playgama.com/EN/game/penguin-homerun)), Angry Smash
([BlueStacks vi](https://www.bluestacks.com/campaign/com.tripledot.angrysmash/vi/), "phá thùng gỗ… công trình").
Không tìm thấy game Play nào lấy **hải cẩu** làm nhân vật bắn ([tìm "seal launch"](https://appgoblin.info/apps/com.unity.cream.SealKindergarten)
chỉ ra game nuôi hải cẩu): "seal" ít cạnh tranh, là chỗ đứng riêng. Lượng tìm kiếm là ước đoán (không có số chính thức);
🟢 = có trong tên/mô tả nhiều app cùng loại, 🟡 = ước đoán.

| Từ khoá (en) | Ở đâu | Ghi chú |
|---|---|---|
| catapult 🟢 | tên, mô tả ngắn, mô tả | Có trong tên hầu hết game cùng loại (Catapult King, The Catapult…) |
| launch 🟢 | tên (Pup Launch), mô tả | "launch the penguin", "launch game" |
| seal / seal pup 🟡 | tên, mô tả ngắn, mô tả | Ít cạnh tranh; đúng nhân vật |
| polar bear(s) 🟢 | mô tả ngắn, mô tả | Kẻ địch, đúng thể loại gốc |
| arcade 🟢 | mô tả | Danh mục Play: Arcade |
| physics 🟢 | mô tả | "physics-based", "one-tap physics" |
| one-tap / tap 🟢 | mô tả | "one-tap game for the whole family" |
| ice / smash ice 🟡 | mô tả ngắn, mô tả | Khối băng vỡ là cảm giác chính |
| offline 🟡 | mô tả | Người dùng hay tìm "offline games" |

| Từ khoá (vi) | Ở đâu | Ghi chú |
|---|---|---|
| hải cẩu 🟡 | tên, mô tả ngắn, mô tả | Tên trong game (vi): "Hải Cẩu Bay" |
| game bắn / máy bắn 🟢 | mô tả ngắn, mô tả | Người Việt gọi thể loại này là "game bắn", "bắn đá" |
| phá băng 🟡 | tên, mô tả | Khối băng vỡ |
| gấu bắc cực 🟢 | mô tả ngắn, mô tả | Kẻ địch |
| phá thành 🟡 | mô tả | Thể loại "phá thành trì" ([bluestacks vi](https://www.bluestacks.com/campaign/com.StarblazeStudio.DemolishBuilding/vi)) |
| không cần mạng / offline 🟡 | mô tả ngắn, mô tả | Người Việt tìm cả hai cách viết |

Không dùng: tên app khác ("Crazy Penguin", "Angry…", "Catapult King"), "penguin", "free", "best", "miễn phí" trong tên và
mô tả ngắn.

## 3. Nội dung trang Play (dán vào Play Console)

Tên trong máy (launcher, `PlayerSettings.productName` trong `ProjectBuilder.ConfigurePlayer`) vẫn là "Seal Catapult"; tên
trên Play dài hơn để chứa từ khoá. Đã đếm ký tự (2026-11-13). Sửa chữ thì đếm lại:
`python3 -c 'import sys; print(len(sys.argv[1]))' "<chữ>"`.

Mọi câu dưới đây khớp game thật (`GAME_DESIGN.md`, `Loc.cs`, `LevelDesigns.cs`, `ProjectBuilder.CreateData`). Thêm hay bỏ
tính năng thì sửa §3 (en + vi) và `site/index.html` cùng lúc.

### 3a. Chọn tên (user 2026-11-13: "tên phải đổi để SEO tốt nhất")

Mẫu như one-line-fill: thương hiệu + ":" + từ khoá mạnh nhất của thể loại (§2). Tên nặng nhất trong xếp hạng, nên mỗi chữ
sau ":" phải là từ người chơi gõ thật.

| # | en-US | Ký tự | Vì sao |
|---|---|---|---|
| **1 ✅** | `Pup Launch: Seal Catapult` | 25 | "catapult" 🟢 là từ chung của mọi game cùng loại; "seal" 🟡 gần như không có đối thủ nên dễ lên đầu; "launch", "pup" đã nằm trong thương hiệu. Ngắn, đọc trọn trên mọi màn hình Play |
| 2 | `Pup Launch: Seal vs Polar Bear` | 30 | Có "polar bear" 🟢, nhưng mất "catapult", từ có nhiều lượt tìm nhất; chạm trần 30, dễ bị cắt chữ trong kết quả tìm kiếm |
| 3 | `Pup Launch: Catapult vs Bears` | 29 | Giữ "catapult", nhưng "bears" yếu hơn "polar bear" và mất "seal", điểm khác biệt duy nhất |

| # | vi-VN | Ký tự | Vì sao |
|---|---|---|---|
| **1 ✅** | `Hải Cẩu Bay: Game Bắn Gấu Băng` | 30 | "game bắn" 🟢 là cách người Việt gõ tìm thể loại; có "hải cẩu", "gấu", "băng"; thương hiệu trùng tên trong game (vi) nên người chơi thấy cùng một tên từ Play tới màn hình chính game |
| 2 | `Hải Cẩu Bay: Máy Bắn Phá Băng` | 29 | Có "máy bắn", "phá băng" 🟡, nhưng thiếu "gấu" và "game bắn" |
| 3 | `Pup Launch: Hải Cẩu Phá Băng` | 28 | Giữ thương hiệu tiếng Anh như one-line-fill; nhưng "Pup Launch" không mang nghĩa với người Việt, tốn 10 ký tự không ra từ khoá |

Châu Âu (chỉ một phương án mỗi nước: thương hiệu + "catapult" bản địa 🟢 + "hải cẩu" bản địa 🟡; lý do như en #1,
từ khoá ở mục "Châu Âu" bên dưới):

| Listing | Tên | Ký tự |
|---|---|---|
| en-GB | `Pup Launch: Seal Catapult` | 25 |
| de-DE | `Pup Launch: Robben-Katapult` | 27 |
| fr-FR | `Pup Launch: Catapulte à Phoque` | 30 |
| es-ES | `Pup Launch: Catapulta de Focas` | 30 |
| it-IT | `Pup Launch: Catapulta di Foche` | 30 |
| pl-PL | `Pup Launch: Foki z Katapulty` | 28 |

Dự phòng nếu muốn có gấu trong tên: de `Pup Launch: Eisbären-Katapult` (29), fr `Pup Launch: Phoques vs Ours` (27), es
`Pup Launch: Focas vs Osos` (25), it `Pup Launch: Foche vs Orsi` (25), pl `Pup Launch: Foki kontra Misie` (29).

### 3b. Đổi thương hiệu thành "Seal Catapult" (user 2026-11-16: "trên ggdich là phóng cún con mà"; "làm SEO thật tốt nhé")

"Pup" trước hết nghĩa là **cún con**; nghĩa "hải cẩu con" là nghĩa phụ. Người chơi tiếng Anh và Google Dịch đọc "Pup
Launch" thành "phóng cún con", và không ai tìm game loại này bằng "pup launch". Thương hiệu mới **Seal Catapult** đặt hai
từ khoá mạnh nhất của thể loại ("seal" 🟡 ít đối thủ, "catapult" 🟢) ngay trong tên; phần sau ":" dùng cho từ khoá thứ ba
("polar bear" 🟢) hoặc từ bản địa. Package `com.sealcatapult.game` (kiểu one-fill-line `com.onelinefill.game`), site
`https://lad02.github.io/seal-catapult/`. Tiếng Việt giữ "Hải Cẩu Bay" (đúng nghĩa, có "hải cẩu").

| Listing | Tên | Ký tự |
|---|---|---|
| en-US, en-GB | `Seal Catapult: Polar Bear Game` | 30 |
| vi | `Hải Cẩu Bay: Game Bắn Gấu Băng` | 30 |
| de-DE | `Seal Catapult: Robben-Katapult` | 30 |
| fr-FR | `Seal Catapult: Phoques et Ours` | 30 |
| es-ES | `Seal Catapult: Focas y Osos` | 27 |
| it-IT | `Seal Catapult: Foche e Orsi` | 27 |
| pl-PL | `Seal Catapult: Foki i Misie` | 27 |

Bảng §3a phía trên là lựa chọn ngày 13/11 với thương hiệu cũ, giữ lại làm lịch sử.

**Tên trên màn hình chính** (`PlayerSettings.productName`, Android hiện khoảng 12 ký tự dưới icon): là `Seal Catapult`
(13 ký tự; vài launcher cắt còn "Seal Catapu…", vẫn đọc ra). Không đổi theo ngôn ngữ (Unity chỉ có một `productName`).
**Tên trong game** (`Loc.cs` `title`: "Seal Catapult" / "Hải Cẩu Bay"): giữ nguyên. Hai tên Play ở trên đều bắt đầu bằng đúng
tên trong game của ngôn ngữ đó, nên không cần sửa code.

### en-US (mặc định)

**App name (30/30):** `Seal Catapult: Polar Bear Game` (16/11: thương hiệu đổi từ "Seal Catapult", Google Dịch đọc thành "phóng cún con"; xem §3b. Trước đó so sánh ở
§3a)

**Short description (79/80):** `Launch seal pups from a catapult, dive onto polar bears and smash the ice fort.`

**Full description:**

```
Launch seal pups over the ice and knock out the polar bears! Seal Catapult is a one-tap catapult arcade game: time your tap to whack each swinging pup up through the ice gap, then tap again to send it diving onto the bears' fortress.

The polar bears stole the seal village's winter fish and locked the pups in cages. Only Pip got away. Time to launch a rescue.

How it plays
• Launch: pups swing on a rope above the catapult. Tap at the right moment and the pup flies up through the gap
• Dive: every pup that gets out flies across the bears' arena. Tap just before a bear and the pup noses down and dives
• Bounce from bear to bear for a combo, and break ice and snow blocks so the bears tumble off their towers
• Real physics: blocks shatter, bears tumble, and no two dives play out the same

What's inside
• 18 levels in two areas: Frost Bay (10 fortress levels, a bonus fish level and a boss level with caged pups to free) and the Glacier River (6 harder fortresses)
• Up to 3 stars per level, and a Perfect badge for knocking out every bear
• 5 seal upgrades bought with stars, 3 levels each: Ice Spikes, Bouncy Ball, Pod, Ninja Pup and Blazing Pup
• 50+ skins bought with stars: fur colours, outfits, catapult balls, flight trails, impact effects, catapult paint and ropes
• Floor buttons, ice gates, bumpers and TNT in later levels
• A stats page: bears knocked out, double, triple and quadruple combos, Perfect missions
• Hand-painted cartoon art and a short opening story

Play your way
• One finger, quick levels of about a minute
• Plays offline, no account, no sign-in
• English, German, French, Spanish, Italian, Polish and Vietnamese
• Landscape, with sound and music switches

Seal Catapult is free to play with ads: an occasional ad between levels, and optional rewarded ads you choose to watch.

Ready? Load the catapult and launch your first seal pup.
```

### vi-VN

**Tên app (30/30):** `Hải Cẩu Bay: Game Bắn Gấu Băng` (thương hiệu vi là tên trong game "Hải Cẩu Bay", `Loc.cs`
`title`; phần sau ":" mang "game bắn", "gấu", "băng". So sánh ở §3a.)

**Mô tả ngắn (80/80):** `Đập hải cẩu bay qua khe băng, lao xuống hạ gấu bắc cực. Game bắn không cần mạng.`

**Mô tả đầy đủ:**

```
Phóng hải cẩu con qua khe băng và hạ gục bầy gấu bắc cực! Seal Catapult là game bắn một chạm: chạm đúng lúc để máy bắn đập hải cẩu đang đu dây bay vọt qua khe thoát, rồi chạm lần nữa để nó cắm đầu lao xuống pháo đài của gấu.

Bầy gấu bắc cực cướp kho cá mùa đông của làng hải cẩu và nhốt bầy hải cẩu con vào lồng. Chỉ có Pip thoát được. Đến lúc phóng đi giải cứu.

Cách chơi
• Phóng: hải cẩu đu dây phía trên máy bắn. Chạm đúng lúc là hải cẩu bay vọt qua khe
• Lao: con nào thoát được thì bay ngang đấu trường của gấu. Chạm ngay trước con gấu là hải cẩu cắm đầu lao xuống
• Nảy từ gấu này sang gấu khác để ghi combo, phá khối băng và khối tuyết cho gấu lộn nhào khỏi tháp
• Vật lý thật: khối vỡ vụn, gấu lộn nhào, không cú lao nào giống cú nào

Có gì trong game
• 18 màn ở hai vùng: Vịnh Băng Giá (10 màn pháo đài, một màn thưởng nhặt cá và một màn boss có lồng nhốt hải cẩu con) và Sông Băng (6 pháo đài khó hơn)
• Tối đa 3 sao mỗi màn, và huy hiệu Hoàn hảo khi hạ hết gấu
• 5 nâng cấp hải cẩu mua bằng sao, mỗi cái 3 cấp: Gai băng, Cuộn tròn, Bầy hải cẩu, Hải cẩu ninja và Hải cẩu lửa
• 50+ skin mua bằng sao: màu lông, trang phục, bóng máy phóng, vệt bay, hiệu ứng va chạm, màu máy phóng và dây đu
• Nút sàn, cổng băng, vòng bật và thùng TNT ở các màn sau
• Trang thống kê: tổng gấu đã hạ, số lần hạ 2, 3, 4 gấu một lượt, số màn hoàn hảo
• Tranh hoạt hình vẽ tay và một đoạn truyện mở đầu ngắn

Chơi theo cách của bạn
• Một ngón tay, mỗi màn khoảng một phút
• Chơi không cần mạng, không cần tài khoản, không cần đăng nhập
• Tiếng Việt, tiếng Anh, Đức, Pháp, Tây Ban Nha, Ý và Ba Lan
• Màn hình ngang, bật/tắt âm thanh và nhạc

Seal Catapult chơi miễn phí, có quảng cáo: thỉnh thoảng một quảng cáo giữa các màn, và quảng cáo có thưởng do bạn tự chọn xem.

Sẵn sàng chưa? Nạp máy bắn và phóng hải cẩu con đầu tiên.
```

Mật độ từ khoá (đếm 2026-11-13; en 1684 ký tự, vi 1651; châu Âu 1953–2139, đều dưới 4000): en "seal" 4 lần, "catapult" 3, "polar bear" 2, "pup" 13
(tên nhân vật, tự nhiên); vi "hải cẩu" 13 (tên nhân vật, không nhồi), "máy bắn" 3, "gấu bắc cực" 2, "game bắn" 1 (+1 trong tên). Câu về quảng cáo phải khớp quảng cáo thật
(`Assets/Scripts/Ads/AdsConfig.cs`, quy tắc trong `GAME_DESIGN.md`): interstitial thỉnh thoảng giữa các màn, rewarded tự
chọn, **không banner**. Đổi vị trí quảng cáo thì sửa câu này (en + vi), `site/privacy.html`, `site/index.html` và Data
safety trong `docs/RELEASE.md` §6 cùng lúc.

### Châu Âu (thêm 2026-11-13, user: "làm SEO thật tốt nhé, ưu tiên EU")

Android chiếm phần lớn điện thoại ở châu Âu (StatCounter 9/2026, ghi trong `docs/ASO.md` của one-line-fill: Đức 72%, Tây
Ban Nha 73%, Pháp 65%, Ý 63%, Ba Lan 64%, Anh 53%). Play xếp hạng theo ngôn ngữ của listing, nên mỗi nước một listing
riêng. Tên giữ thương hiệu "Pup Launch" trước ":" (người châu Âu cũng gõ tiếng Anh; game cùng loại giữ tên gốc tiếng Anh
ở mọi nước, ví dụ "Catapult King", "The Catapult"), phần sau ":" là từ khoá bản địa. Giao diện game có đủ 7 ngôn ngữ
(main thread dịch 2026-11-13), nên mô tả ghi danh sách ngôn ngữ đó. Bản dịch do Claude viết, **chưa có người bản xứ đọc
lại**: nhờ một người đọc trước khi dán, nhất là `pl-PL`.

**Tên trong game phải khớp listing.** Tên nâng cấp, vùng và huy hiệu trong mô tả dưới đây là tên đề xuất; khi dịch
`Loc.cs` dùng đúng các tên này (hoặc sửa mô tả theo tên đã dịch), để ảnh chụp và mô tả nói cùng một chữ:

| en (`Loc.cs`) | de | fr | es | it | pl |
|---|---|---|---|---|---|
| Frost Bay | Frostbucht | Baie Gelée | Bahía Helada | Baia Gelata | Mroźna Zatoka |
| Glacier River | Gletscherfluss | Rivière Glacée | Río Glaciar | Fiume Glaciale | Lodowa Rzeka |
| Ice Spikes | Eisstacheln | Pics de glace | Púas de hielo | Punte di ghiaccio | Lodowe kolce |
| Bouncy Ball | Hüpfball | Boule rebondissante | Bola saltarina | Palla rimbalzante | Skacząca piłka |
| Pod | Rudel | Bande | Manada | Branco | Stado |
| Ninja Pup | Ninja-Robbe | Phoque ninja | Foca ninja | Foca ninja | Foka ninja |
| Blazing Pup | Feuer-Robbe | Phoque de feu | Foca de fuego | Foca di fuoco | Ognista foka |
| Perfect (badge) | Perfekt | Parfait | Perfecto | Perfetto | Perfekcyjnie |

#### Từ khoá châu Âu

Nguồn (WebSearch 2026-11-13, mỗi ngôn ngữ một truy vấn thể loại + một truy vấn "game kỹ năng"): game cùng loại có trang
bản địa: Crazy Penguin Catapult 2 ([testberichte.de](https://www.testberichte.de/p/digital-chocolate-tests/crazy-penguin-catapult-2-fuer-iphone-testbericht.html):
"aus einem Katapult schleudern", "Polarbären"; [multiplayer.it](https://multiplayer.it/recensioni/crazy-penguin-catapult.html):
"potente catapulta", "orsi", "picchiata"), Catapult King ([apps.apple.com/de](https://apps.apple.com/de/app/catapult-king/id497936366?l=en),
[xatakandroid](https://www.xatakandroid.com/juegos-android/catapult-king-llega-a-android-derrota-al-enemigo-con-la-catapulta-y-rescata-a-la-princesa):
"usar una catapulta para derribar"), Penguin Swing ([fr](https://www.seeles.ai/games/fr/casual/penguin-swing),
[es](https://www.seeles.ai/games/es/casual/penguin-swing), [pl](https://www.seeles.ai/games/pl/casual/penguin-swing):
"ours polaire", "oso polar", "niedźwiedź polarny"), Bouncemasters ([crazygames fr](https://www.crazygames.com/fr/game/bouncemasters),
[it](https://www.crazygames.com/it/game/bouncemasters)), YOWPLAY ([apps.apple.com/de](https://apps.apple.com/de/app/yowplay/id1613181867):
"physikbasiertes Katapultspiel"), Catapult Quest ([apps.apple.com/fr](https://apps.apple.com/fr/app/catapult-quest/id1503429616?l=en-GB)),
Penguin Toss ([apps.apple.com](https://apps.apple.com/us/app/penguin-toss/id1016649858?uo=4): "katapulta"). Không tìm thấy
trang Play bản địa nào (Play không trả trong kết quả web) và **không có game Play nào lấy hải cẩu làm nhân vật** ở cả 5
nước: "Robbe / phoque / foca / foca / foka" là chỗ trống. 🟢 = có trong tên/mô tả nhiều game cùng loại, 🟡 = ước đoán. Sau
khi lên Play, sửa theo **Search terms** từng nước (§6).

| Ngôn ngữ | Tên (sau ":") | Từ khoá trong mô tả ngắn/đầy đủ |
|---|---|---|
| de | Robben-Katapult | Katapult 🟢, Eisbären 🟢, Robbe 🟡, Arcade 🟢, Physik 🟢, offline 🟡, Geschicklichkeitsspiel 🟡 |
| fr | Catapulte à Phoque | catapulte 🟢, ours polaires 🟢, phoque 🟡, jeu d'arcade 🟢, physique 🟢, hors ligne 🟡, jeu d'adresse 🟡 |
| es | Catapulta de Focas | catapulta 🟢, osos polares 🟢, foca 🟡, arcade 🟢, física 🟢, sin internet 🟡 |
| it | Catapulta di Foche | catapulta 🟢, orsi polari 🟢, foca 🟡, arcade 🟢, fisica 🟢, picchiata 🟡, offline 🟡 |
| pl | Foki z Katapulty | katapulta 🟢, niedźwiedzie polarne 🟢, foka 🟡, gra zręcznościowa 🟡, fizyka 🟢, offline 🟡 |

Quy tắc bản địa: es-ES viết "tú", dấu "¡ ¿" đầu câu, "vídeo" có dấu (es-ES), "sin internet" (người Tây Ban Nha gõ cả
"sin conexión"); fr viết "tu" như game di động, "hors ligne"; de dùng "du", danh từ ghép có gạch nối khi ghép tên riêng
("Robben-Katapult"); pl dùng "ty", "gra zręcznościowa" là tên thể loại Arcade trong Play tiếng Ba Lan. Không dùng "gratis",
"kostenlos", "gratuit", "darmowa", "gratuito" trong tên và mô tả ngắn (được trong mô tả đầy đủ khi nói về giá, như en).

#### en-GB

Giống en-US (tên, mô tả ngắn, mô tả đầy đủ); mô tả không có chữ nào khác chính tả Anh–Mỹ.

#### de-DE

**App-Name (30/30):** `Seal Catapult: Robben-Katapult`

**Kurzbeschreibung (78/80):** `Robben per Katapult starten, auf Eisbären stürzen und die Eisburg zerschlagen.`

**Ausführliche Beschreibung:**

```
Schieß Robbenbabys über das Eis und hau die Eisbären um! Seal Catapult ist ein Katapult-Arcadespiel für nur einen Finger: Tippe im richtigen Moment, damit die schwingende Robbe durch die Eisspalte fliegt, und tippe noch einmal, damit sie im Sturzflug auf die Festung der Eisbären stürzt.

Die Eisbären haben den Fischvorrat des Robbendorfs für den Winter gestohlen und die Robbenbabys in Käfige gesperrt. Nur Pip ist entkommen. Zeit für eine Rettung per Katapult.

So wird gespielt
• Abschuss: Die Robben schwingen an einem Seil über dem Katapult. Tippe im richtigen Moment, und die Robbe fliegt durch die Spalte
• Sturzflug: Jede Robbe, die es hinaus schafft, fliegt über die Arena der Eisbären. Tippe kurz vor einem Bären, und sie stürzt kopfüber hinab
• Spring von Bär zu Bär für Kombos und zerschlage Eis- und Schneeblöcke, damit die Bären von ihren Türmen purzeln
• Echte Physik: Blöcke zersplittern, Bären purzeln, kein Sturzflug gleicht dem anderen

Das steckt drin
• 18 Level in zwei Gebieten: die Frostbucht (10 Festungslevel, ein Bonuslevel mit Fischen und ein Bosslevel mit gefangenen Robbenbabys) und der Gletscherfluss (6 schwerere Festungen)
• Bis zu 3 Sterne pro Level und ein Perfekt-Abzeichen, wenn alle Bären fallen
• 5 Robben-Upgrades für Sterne, je 3 Stufen: Eisstacheln, Hüpfball, Rudel, Ninja-Robbe und Feuer-Robbe
• 50+ Skins für Sterne: Fellfarben, Outfits, Katapultbälle, Flugspuren, Treffereffekte, Katapultfarben und Seile
• Bodenschalter, Eistore, Bumper und TNT in späteren Leveln
• Eine Statistikseite: besiegte Bären, Zweier-, Dreier- und Viererkombos, perfekte Missionen
• Handgemalte Cartoon-Grafik und eine kurze Vorgeschichte

Spiel, wie du willst
• Ein Finger, kurze Level von etwa einer Minute
• Offline spielbar, kein Konto, keine Anmeldung
• Deutsch, Englisch, Französisch, Spanisch, Italienisch, Polnisch und Vietnamesisch
• Querformat, Ton und Musik einzeln abschaltbar

Seal Catapult ist kostenlos spielbar und enthält Werbung: gelegentlich eine Anzeige zwischen den Leveln und freiwillige Belohnungsvideos, die du selbst startest.

Bereit? Lade das Katapult und schick deine erste Robbe los.
```

#### fr-FR

**Nom de l'app (30/30) :** `Seal Catapult: Phoques et Ours`

**Description courte (79/80) :** `Lance des phoques à la catapulte, plonge sur les ours polaires, brise la glace.`

**Description complète :**

```
Lance des bébés phoques par-dessus la glace et mets K.O. les ours polaires ! Seal Catapult est un jeu d'arcade à la catapulte qui se joue d'un seul doigt : touche l'écran au bon moment pour envoyer chaque phoque qui se balance à travers la faille de glace, puis touche encore pour le faire plonger sur la forteresse des ours.

Les ours polaires ont volé les réserves de poisson d'hiver du village des phoques et enfermé les bébés phoques dans des cages. Seul Pip s'est échappé. À la catapulte, pour le sauvetage !

Comment jouer
• Lancer : les phoques se balancent au bout d'une corde au-dessus de la catapulte. Touche au bon moment et le phoque s'envole par la faille
• Plonger : chaque phoque sorti survole l'arène des ours. Touche juste avant un ours et le phoque pique du nez et plonge
• Rebondis d'ours en ours pour enchaîner les combos, et brise les blocs de glace et de neige pour faire tomber les ours de leurs tours
• Vraie physique : les blocs volent en éclats, les ours culbutent, aucun plongeon ne se ressemble

Au programme
• 18 niveaux dans deux zones : la Baie Gelée (10 niveaux de forteresse, un niveau bonus de poissons et un niveau de boss avec des bébés phoques en cage) et la Rivière Glacée (6 forteresses plus dures)
• Jusqu'à 3 étoiles par niveau, et un badge Parfait quand tous les ours tombent
• 5 améliorations achetées avec des étoiles, 3 niveaux chacune : Pics de glace, Boule rebondissante, Bande, Phoque ninja et Phoque de feu
• 50+ skins à acheter avec des étoiles : pelages, tenues, balles de catapulte, traînées, effets d'impact, peintures de catapulte et cordes
• Interrupteurs au sol, portes de glace, bumpers et TNT dans les niveaux suivants
• Une page de statistiques : ours mis K.O., combos doubles, triples et quadruples, missions parfaites
• Des dessins façon dessin animé peints à la main et une courte histoire d'introduction

Joue à ta façon
• Un seul doigt, des niveaux rapides d'environ une minute
• Fonctionne hors ligne, sans compte ni connexion
• Français, anglais, allemand, espagnol, italien, polonais et vietnamien
• Écran paysage, son et musique réglables séparément

Seal Catapult est gratuit avec des publicités : de temps en temps une publicité entre les niveaux, et des vidéos récompensées que tu choisis de regarder.

Prêt ? Charge la catapulte et lance ton premier bébé phoque.
```

#### es-ES

**Nombre de la app (27/30):** `Seal Catapult: Focas y Osos`

**Descripción breve (74/80):** `Lanza focas con la catapulta, cae sobre los osos polares y rompe el hielo.`

**Descripción completa:**

```
¡Lanza crías de foca por encima del hielo y tumba a los osos polares! Seal Catapult es un juego arcade de catapulta que se juega con un solo dedo: toca en el momento justo para que cada foca que se balancea salga volando por la grieta del hielo, y vuelve a tocar para que caiga en picado sobre la fortaleza de los osos.

Los osos polares han robado el pescado para el invierno de la aldea de las focas y han encerrado a las crías en jaulas. Solo Pip ha escapado. ¡Hora de un rescate a golpe de catapulta!

Cómo se juega
• Lanzar: las focas se balancean en una cuerda sobre la catapulta. Toca en el momento justo y la foca sale volando por la grieta
• Caer en picado: cada foca que sale cruza la arena de los osos. Toca justo antes de un oso y la foca se lanza de cabeza
• Rebota de oso en oso para hacer combos y rompe bloques de hielo y nieve para que los osos caigan de sus torres
• Física real: los bloques estallan, los osos dan volteretas y ningún picado es igual a otro

Qué incluye
• 18 niveles en dos zonas: la Bahía Helada (10 niveles de fortaleza, un nivel extra de peces y un nivel de jefe con crías enjauladas) y el Río Glaciar (6 fortalezas más difíciles)
• Hasta 3 estrellas por nivel y una insignia Perfecto si caen todos los osos
• 5 mejoras que se compran con estrellas, 3 niveles cada una: Púas de hielo, Bola saltarina, Manada, Foca ninja y Foca de fuego
• 50+ aspectos que se compran con estrellas: pelajes, atuendos, bolas de catapulta, estelas, efectos de impacto, pintura de catapulta y cuerdas
• Interruptores en el suelo, puertas de hielo, rebotadores y TNT en los niveles posteriores
• Una página de estadísticas: osos derribados, combos dobles, triples y cuádruples, misiones perfectas
• Dibujos animados pintados a mano y una breve historia inicial

Juega a tu manera
• Un solo dedo, niveles rápidos de un minuto más o menos
• Funciona sin internet, sin cuenta y sin registro
• Español, inglés, alemán, francés, italiano, polaco y vietnamita
• Pantalla horizontal, sonido y música por separado

Seal Catapult es gratis y contiene anuncios: de vez en cuando un anuncio entre niveles y vídeos con recompensa que tú eliges ver.

¿Listo? Carga la catapulta y lanza tu primera cría de foca.
```

#### it-IT

**Nome dell'app (27/30):** `Seal Catapult: Foche e Orsi`

**Descrizione breve (76/80):** `Lancia foche con la catapulta, piomba sugli orsi polari e rompi il ghiaccio.`

**Descrizione completa:**

```
Lancia cuccioli di foca oltre il ghiaccio e metti KO gli orsi polari! Seal Catapult è un gioco arcade con la catapulta che si gioca con un dito solo: tocca al momento giusto per far volare ogni foca che dondola attraverso la fessura nel ghiaccio, poi tocca di nuovo per farla piombare in picchiata sulla fortezza degli orsi.

Gli orsi polari hanno rubato il pesce per l'inverno al villaggio delle foche e hanno chiuso i cuccioli in gabbia. Solo Pip è scappato. È ora di una missione di salvataggio a colpi di catapulta!

Come si gioca
• Lancio: le foche dondolano appese a una corda sopra la catapulta. Tocca al momento giusto e la foca vola attraverso la fessura
• Picchiata: ogni foca che esce sorvola l'arena degli orsi. Tocca appena prima di un orso e la foca si tuffa a testa in giù
• Rimbalza da un orso all'altro per fare combo e rompi i blocchi di ghiaccio e neve per far cadere gli orsi dalle torri
• Fisica vera: i blocchi vanno in frantumi, gli orsi capitombolano, nessuna picchiata è uguale all'altra

Cosa c'è dentro
• 18 livelli in due zone: la Baia Gelata (10 livelli fortezza, un livello bonus con i pesci e un livello boss con cuccioli in gabbia) e il Fiume Glaciale (6 fortezze più difficili)
• Fino a 3 stelle per livello e un distintivo Perfetto quando cadono tutti gli orsi
• 5 potenziamenti da comprare con le stelle, 3 livelli ciascuno: Punte di ghiaccio, Palla rimbalzante, Branco, Foca ninja e Foca di fuoco
• 50+ skin da comprare con le stelle: pellicce, completi, palle da catapulta, scie, effetti d'impatto, vernici per la catapulta e corde
• Pulsanti a terra, cancelli di ghiaccio, respingenti e TNT nei livelli successivi
• Una pagina di statistiche: orsi messi KO, combo doppie, triple e quadruple, missioni perfette
• Grafica a cartone animato dipinta a mano e una breve storia iniziale

Gioca come vuoi
• Un dito solo, livelli veloci di circa un minuto
• Si gioca offline, senza account e senza accesso
• Italiano, inglese, tedesco, francese, spagnolo, polacco e vietnamita
• Schermo orizzontale, suono e musica regolabili separatamente

Seal Catapult è gratis con pubblicità: ogni tanto un annuncio tra un livello e l'altro, e video con premio che scegli tu di guardare.

Pronto? Carica la catapulta e lancia il tuo primo cucciolo di foca.
```

#### pl-PL

**Nazwa aplikacji (27/30):** `Seal Catapult: Foki i Misie`

**Krótki opis (73/80):** `Wystrzel foki z katapulty, zanurkuj na niedźwiedzie polarne i rozbij lód.`

**Pełny opis:**

```
Wystrzel małe foki nad lodem i znokautuj niedźwiedzie polarne! Seal Catapult to zręcznościowa gra z katapultą na jeden palec: dotknij ekranu we właściwej chwili, aby kołysząca się foka przeleciała przez szczelinę w lodzie, a potem dotknij jeszcze raz, by zanurkowała na fortecę niedźwiedzi.

Niedźwiedzie polarne ukradły zimowy zapas ryb z wioski fok i zamknęły małe foki w klatkach. Uciekł tylko Pip. Czas na akcję ratunkową z katapulty!

Jak grać
• Start: foki bujają się na linie nad katapultą. Dotknij we właściwej chwili, a foka wyleci przez szczelinę
• Nurkowanie: każda foka, która się wydostanie, leci nad areną niedźwiedzi. Dotknij tuż przed niedźwiedziem, a foka zanurkuje głową w dół
• Odbijaj się od niedźwiedzia do niedźwiedzia, zdobywaj combo i rozbijaj bloki lodu i śniegu, by niedźwiedzie spadały z wież
• Prawdziwa fizyka: bloki pękają, niedźwiedzie koziołkują, żadne nurkowanie nie jest takie samo

Co jest w grze
• 18 poziomów w dwóch krainach: Mroźna Zatoka (10 poziomów z fortecami, poziom bonusowy z rybami i poziom z bossem, gdzie trzeba uwolnić małe foki z klatek) oraz Lodowa Rzeka (6 trudniejszych fortec)
• Do 3 gwiazdek na poziom i odznaka Perfekcyjnie, gdy padną wszystkie niedźwiedzie
• 5 ulepszeń za gwiazdki, każde po 3 poziomy: Lodowe kolce, Skacząca piłka, Stado, Foka ninja i Ognista foka
• 50+ skórki za gwiazdki: kolory futra, stroje, kule katapulty, smugi, efekty uderzeń, farby katapulty i liny
• Przyciski w podłodze, lodowe bramy, odbijacze i TNT w dalszych poziomach
• Strona statystyk: znokautowane niedźwiedzie, combo podwójne, potrójne i poczwórne, perfekcyjne misje
• Ręcznie malowana grafika w stylu kreskówki i krótka historia na początek

Graj po swojemu
• Jeden palec, krótkie poziomy po około minucie
• Działa offline, bez konta i bez logowania
• Polski, angielski, niemiecki, francuski, hiszpański, włoski i wietnamski
• Tryb poziomy, osobno wyłączany dźwięk i muzyka

Seal Catapult jest darmowy i zawiera reklamy: od czasu do czasu reklama między poziomami oraz filmy z nagrodą, które oglądasz tylko wtedy, gdy chcesz.

Gotowy? Załaduj katapultę i wystrzel swoją pierwszą fokę.
```

## 4. Hình ảnh (quyết định tỉ lệ bấm cài)

- **Icon** 512×512: Play bắt buộc 512×512 PNG 32-bit. Icon hiện tại của app là `Assets/Resources/Art/pup_ball.png`
  (285×267, hải cẩu cuộn tròn; `ProjectBuilder.ConfigurePlayer` đặt nó làm icon Android) và `site/icon.png` là bản chép.
  **Cần làm icon 512 riêng** (✋ hoặc 🤖 vẽ bằng ChatGPT theo cách §R1 của `GAME_DESIGN.md`, sửa trên tranh hải cẩu đã chốt):
  hải cẩu Pip mũ len đỏ bay chéo lên trên nền băng xanh, viền nâu-navy dày, không chữ (chữ nhỏ không đọc được ở 48dp).
  Có icon mới thì thay cả icon Android trong `ProjectBuilder` và `site/icon.png`.
- **Feature graphic** 1024×500: bên phải là hải cẩu đang lao xuống pháo đài khối băng, gấu lộn nhào, mảnh băng văng; bên
  trái chữ "Launch. Dive. Free the pups." / "Phóng. Lao. Cứu bầy hải cẩu." trên nền hoàng hôn cam của đấu trường. Không dùng
  "free" theo nghĩa giá ("Free the pups" là giải cứu, giữ được; nếu Play từ chối thì đổi "Save the pups"), không "#1".
- **Ảnh chụp** (6–8 ảnh **ngang** 1920×1080, vì game chỉ chạy màn hình ngang; chụp từ emulator `Pixel_QA_34` theo
  `README.md` "QA on the emulator": `adb emu screenrecord screenshot <dir>` rồi xoay 90°; mỗi ngôn ngữ một bộ, đặt ngôn ngữ
  trong Cài đặt trước khi chụp). Ảnh 1–3 hiện trong kết quả tìm kiếm, nên kể lợi ích, không kể tên tính năng. Chữ trên ảnh
  phải đúng với cảnh trong ảnh. Lưu ở `docs/screenshots/play/<en|vi>/<n>-<tên>.png`.

| # | Màn hình (cách dựng) | Chữ (en) | Chữ (vi) |
|---|---|---|---|
| 1 | Pha 2: hải cẩu đang cắm đầu lao vào gấu, khối băng vỡ, rung camera (màn 6 hoặc 10) | Dive onto the polar bears | Lao xuống hạ gấu bắc cực |
| 2 | Pha 1: hải cẩu đu dây ngay trên máy bắn, cần đang quét lên | Tap to launch through the gap | Chạm đúng lúc, bay qua khe |
| 3 | Combo: chữ "×2"/"×3" lớn giữa màn, gấu lộn nhào | Bounce for big combos | Nảy liên tiếp, combo cực đã |
| 4 | Thẻ kết quả thắng: 3 sao + "Nhiệm vụ hoàn hảo!" | Earn 3 stars and Perfect badges | Giành 3 sao và huy hiệu Hoàn hảo |
| 5 | Cửa hàng Nâng cấp: thẻ 5 nâng cấp | 5 upgrades for your pups | 5 nâng cấp cho hải cẩu |
| 6 | Bản đồ Vịnh Băng Giá với nút boss có lồng | 18 levels in two areas, a bonus and a boss | 18 màn ở hai vùng, màn thưởng và boss |
| 7 | Màn boss: gấu boss trên bệ, lồng nhốt hải cẩu con | Free the caged pups | Giải cứu hải cẩu bị nhốt |
| 8 | Tranh truyện mở đầu (story_5: Pip thoát) | Plays offline, no account | Chơi không cần mạng |

- Châu Âu: ít nhất ảnh 1–3 có chữ bản địa (hiện trong kết quả tìm kiếm), chụp với game đặt đúng ngôn ngữ; ảnh 4–8 dùng bộ
  en nếu chưa kịp chụp. en-GB dùng bộ en.

| # | de | fr | es | it | pl |
|---|---|---|---|---|---|
| 1 | Stürz dich auf die Eisbären | Plonge sur les ours polaires | Cae en picado sobre los osos | Piomba sugli orsi polari | Zanurkuj na niedźwiedzie |
| 2 | Tippen, abschießen, durch die Spalte | Touche, lance, passe la faille | Toca, lanza, cruza la grieta | Tocca, lancia, passa la fessura | Dotknij, wystrzel, przeleć szczelinę |
| 3 | Abprallen für große Kombos | Rebondis pour de gros combos | Rebota para hacer combos | Rimbalza per fare combo | Odbijaj się po wielkie combo |

  Để có màn 6, 10, 11, 12 khi chụp: build bản QA và đẩy save mở khoá theo `README.md` "Reaching later levels in QA".
- **Video** (tuỳ chọn): 15–30 giây quay bằng `adb emu screenrecord start/stop`, một vòng đủ hai pha của màn 6, đăng YouTube,
  dán link vào Play.

## 5. Điểm sao và review

- Trả lời mọi review 1–3 sao trong 24 giờ, bằng ngôn ngữ của người viết; nói rõ bản nào sửa lỗi.
- Chưa có hộp đánh giá trong game (In-App Review). Nếu thêm: hỏi **một lần**, sau khi thắng màn 3 lần đầu (màn đã quen nhịp),
  không bao giờ khi thua, giữa màn hay ngay sau quảng cáo; không hỏi "bạn có thích không" rồi chỉ dẫn người thích sang Play
  (Play cấm lọc review). Thêm thì ghi quyết định vào `GAME_DESIGN.md` trước, như quy tắc của `CLAUDE.md`.

## 6. Sau khi lên Play

- Play Console → **Store listing experiments**: mỗi lần thử một thứ (icon trước, rồi ảnh 1, rồi mô tả ngắn),
  chạy ≥ 7 ngày, chỉ chọn bên thắng khi đạt 95%.
- Theo dõi hằng tuần: Play Console → Statistics → **Store listing acquisition** (lượt xem → lượt cài) và
  **Search terms** (từ khoá thật người dùng gõ). Từ khoá có lượt xem nhưng tỉ lệ cài thấp: sửa ảnh 1–3.
  Từ khoá mới có lượt cài tốt: đưa vào mô tả ngắn.
- Sửa chữ trên Play không cần build mới; Play duyệt lại trong vài giờ.

## 7. Trang web (`site/`)


Đăng ở **`https://lad02.github.io/pup/`**: thư mục `pup/` trong repo GitHub Pages `laD02/laD02.github.io` (gốc repo đó là
trang của One Line Fill). Chép nội dung `site/` của repo này vào `pup/` của repo Pages rồi push.
- **`app-ads.txt` không nằm ở đây.** AdMob chỉ đọc `app-ads.txt` ở **gốc tên miền** ghi trong ô "Website" của tài khoản
  nhà phát triển trên Play. Tài khoản Play "Le Hoang Games" đã ghi `https://lad02.github.io/`, và
  `https://lad02.github.io/app-ads.txt` đã có `google.com, pub-5420799226073285, DIRECT, f08c47fec0942fa0`. Seal Catapult dùng
  cùng tài khoản AdMob (cùng publisher `pub-5420799226073285`) nên file đó đã phủ app này; không tạo file thứ hai. Đổi sang
  tài khoản AdMob khác thì phải thêm dòng của publisher mới vào file ở gốc.
- `robots.txt` cũng chỉ được Google đọc ở gốc tên miền: file ở gốc (của One Line Fill) đang `Allow: /` nên `pup/` được
  index. `site/robots.txt` ở đây chỉ để dùng khi game có tên miền riêng; sitemap của `pup/` phải được khai thêm ở
  `robots.txt` gốc (thêm dòng `Sitemap: https://lad02.github.io/pup/sitemap.xml`) hoặc nộp trong Google Search Console.
- **Một trang mỗi ngôn ngữ** (2026-11-13, ưu tiên EU): `site/index.html` (en, cũng là `x-default`) và
  `site/<vi|de|fr|es|it|pl>/index.html`. Google chỉ coi `hreflang` là hợp lệ khi mỗi ngôn ngữ có URL riêng (không dùng
  `#de`). Mỗi trang có tiêu đề và mô tả theo từ khoá bản địa (§2, mục "Châu Âu"), canonical, đủ 7 `hreflang` + `x-default`,
  Open Graph/Twitter, JSON-LD `MobileApplication`, link Play và link sang các ngôn ngữ khác.
- Các trang được sinh bởi `site/make_pages.py` (bảng `PAGES`): sửa chữ ở đó rồi chạy `python3 -I site/make_pages.py`, không
  sửa tay file html. Script cũng viết `site/sitemap.xml` (7 trang + privacy). Khi đăng, **không** chép `make_pages.py`.
- `site/privacy.html`: chính sách quyền riêng tư (Play bắt buộc; dán URL `https://lad02.github.io/pup/privacy.html` vào
  Play Console → App content → Privacy policy và vào AdMob).
- `site/privacy.html`: một trang vi + en, có mục GDPR (bên kiểm soát, cơ sở pháp lý = đồng ý qua UMP, rút lại đồng ý,
  quyền của người dùng, chuyển dữ liệu ra ngoài EU).
- Liên hệ: `lehoang10112002@gmail.com` (như One Line Fill).
- Link Play dùng package id cố định `com.pufferlaunch.game` (`ProjectBuilder.ConfigurePlayer`), nên đã đúng từ trước khi
  app lên Play (trước đó link báo "không tìm thấy").

## 8. Ưu tiên châu Âu (quyết định 2026-11-13)

User: "làm SEO thật tốt nhé, ưu tiên EU". Giống one-line-fill (§8 bên đó, 2026-10-27): **không trả tiền mua lượt cài**;
mọi lượt cài đến từ ASO (tìm kiếm và đề xuất của Play), tiền đến từ AdMob trong game. Lý do chọn EU: một người chơi ở
Đức/Pháp/Ý mang về khoảng 6–8 lần tiền AdMob so với người chơi ở VN (ARPDAU ước $0,03–0,08 so với $0,005–0,01; số chung
của ngành ghi ở one-line-fill, chưa đo trên game này). Listing vi-VN vẫn giữ nhưng không tốn thêm công.

Thứ tự công sức: **de-DE** (nhiều Android nhất, eCPM cao) → **fr-FR** → **it-IT** / **es-ES** → **pl-PL** → en-GB.

Việc cần làm (theo thứ tự phát hành):
1. Dịch giao diện game sang de/fr/es/it/pl (main thread), dùng đúng bảng tên ở mục "Châu Âu" §3.
2. Nhờ người bản xứ đọc tên, mô tả ngắn và mô tả đầy đủ, ưu tiên de rồi fr; pl cần nhất vì khó dịch nhất.
3. Ảnh 1–3 có chữ bản địa cho cả 5 ngôn ngữ (§4), chụp với game đặt đúng ngôn ngữ; feature graphic có chữ bản địa cho de
   và fr ("Starten. Stürzen. Robben retten." / "Lance. Plonge. Libère les phoques.").
4. Dán listing theo thứ tự: en-US (mặc định) → de-DE → fr-FR → it-IT → es-ES → pl-PL → en-GB → vi-VN, trong cùng lần nộp đầu
   (Play Console → Store presence → Main store listing → Manage translations → thêm bản dịch của riêng mình, không dùng dịch
   máy của Play).
5. Trang `site/` có một trang mỗi ngôn ngữ với `hreflang` (§7), để Google bản địa tìm thấy game bằng từ khoá bản địa.
6. Sau 2 tuần trên Play: đọc **Search terms** và **Store listing acquisition** theo từng nước (§6), sửa mô tả ngắn và ảnh 1–3
   theo từ khoá có lượt cài thật; chạy **Store listing experiments** trên listing de trước.
7. Không có tiền mua lượt cài nên điểm sao và retention đẩy hạng: trả lời review de/fr trong 24 giờ (§5), giữ quảng cáo nhẹ
   tay như quy tắc trong `GAME_DESIGN.md` cho tới khi có số D7.
8. Hằng tuần đọc: Play Console → Retention theo nước; AdMob → báo cáo theo quốc gia (eCPM, ARPDAU).
9. Người dùng EU/UK/CH phải thấy form đồng ý (GDPR message trong AdMob, `docs/RELEASE.md` §3) trước khi có quảng cáo cá nhân
   hoá; `site/privacy.html` có mục GDPR (bên kiểm soát, cơ sở pháp lý, quyền của người dùng).
