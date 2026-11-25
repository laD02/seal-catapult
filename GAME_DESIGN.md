# Seal Catapult — Tài liệu thiết kế game (GDD)

> Bản 10 · 12/11/2026 · **đã chốt, đang thực hiện** (người dùng: "từ báo cáo đó thiết kế lại game, redesign lại toàn bộ cx
> đc, vẽ thì dùng chatgpt nhé phải thật mượt mà" → "làm đi"). Nguồn: `docs/cpc-reference-report.md` (chơi thật 3 lần + 5 video).
> Bản 9 (11/11) vẫn đúng ở mọi chỗ §R không ghi đè.
> Tên tiếng Việt tạm: **Hải Cẩu Bay** · Tên mã trong code: `SealCatapult` · Bản cũ: `docs/archive/GAME_DESIGN_v6_2026-11-11.md`

Hải cẩu con Pip và bầy bạn giành lại kho cá mùa đông từ bầy gấu bắc cực. Mỗi màn hai pha: **đập đúng nhịp** để hải cẩu
đang đu dây bay lọt khe thoát của hố băng, rồi **chạm đúng lúc** để từng con lao chéo xuống đấu trường hạ hết gấu.
Bố cục màn chơi học theo thể loại "máy bắn + phá công trình" (ảnh mẫu Crazy Penguin Catapult bản Android); nhân vật,
hình, âm thanh, chữ và màn chơi là của riêng game.
Báo cáo chơi thật CPC (12/11/2026, 3 lần chơi + 5 video + đối chiếu báo cáo repo FrostFling): `docs/cpc-reference-report.md`.

Ký hiệu trạng thái trong tài liệu: ✅ đã có trong build · 🔧 có nhưng cần làm lại/hoàn thiện · 🆕 chưa làm · ❓ cần bạn chốt.

## R. Bản 10: làm lại toàn bộ theo báo cáo (12/11/2026)

Mục này **ghi đè** các mục sau khi khác nhau. Mọi thứ vẫn là của riêng game: hải cẩu, gấu ác, Vịnh Băng Giá, chữ, màn chơi.
Chỉ học **nhịp, bố cục, cách phản hồi** của thể loại.

### R1. Hình vẽ: toàn bộ bằng ChatGPT
- **Mọi thứ người chơi nhìn thấy** đều do ChatGPT vẽ: nhân vật, nền hố bắn, nền đấu trường, ô đá/băng/gỗ, máy bắn, bản đồ,
  menu, khung UI, nút, icon, tranh truyện mở đầu, tranh thắng/thua. `make_art.py` (SVG) chỉ còn làm hình dự phòng khi
  chưa có tranh, và vẽ những thứ thuần hình học (vệt, hạt).
- Cách làm: Claude tự lái ChatGPT trong Chrome. Luôn đính kèm một tranh đã chốt và bấm **Chỉnh sửa** để giữ nét (tranh mới
  từ đầu bị lệch nét). Tranh tải về chuyển vào `Assets/Art/Reference/`, script trong `Tools/` cắt ra `Assets/Resources/Art/`.
- Ô lặp (đá cuội, băng, gỗ) phải lặp không thấy mối nối. ChatGPT không tự làm được, nên `Tools/` có bước làm liền mép.
- Nền hố bắn và bản đồ là **một tranh lớn** vì giống nhau mọi màn. Va chạm đặt theo tranh, toạ độ ghi trong code.
  Đấu trường đổi theo màn nên vẫn ghép từ ô và vật thể.
- Phong cách chung: cartoon 2D vẽ tay, viền nâu-navy dày, tô màu mềm hai tông, highlight trắng. Hố bắn lạnh (xanh băng, đá
  xám, mây hồng), đấu trường ấm (hoàng hôn cam, băng xanh trong).

### R2. Mượt mà
- Chạy **60 fps** (`Application.targetFrameRate = 60`), không cấp phát mỗi frame.
- Vòng lặp chính có **8 frame vẽ tay**: bay, đu dây, đi, gấu thở. Hành động ngắn có 4–6 frame.
- Thêm chuyển động phụ bằng code cho mọi nhân vật:
  - nén-giãn (squash/stretch) khi đáp, va, bật lên;
  - lắc nhẹ khi đứng (thở);
  - ease cho mọi tween;
  - camera bám mềm (SmoothDamp), rung theo lực va.
- UI: panel bật vào có nảy (overshoot), nút nhún khi chạm, số đếm chạy lên, sao bay vào từng cái.
- Chuyển cảnh không bao giờ cắt cụt: nước dâng/rút, panel trượt.

### R3. Pha 1 (thay §4 khi khác)
- **Máy bắn đặt bên phải dưới đáy cung đu.** Sửa 12/11 sau khi xem video Flash từng 0,06 s: cầu tuyết **gắn cố định ở
  đầu cần, không bay đi**. Chạm thì cần quét lên qua đỉnh, chính quả cầu **đập vào hải cẩu** (có chớp sáng chỗ va).
  Cần chỉ với tới đoạn cung từ đáy sang nửa phải, nên phải chạm lúc hải cẩu đi qua đó. Trúng thì hải cẩu văng
  lên-phải; quét hụt thì cần quay về, nạp lại.
- **Đu rộng**: biên độ 46°–57° (Flash gần nằm ngang ở hai đầu), chu kỳ khoảng 1,1 s.
- Ba mức phản hồi (chữ to giữa màn, nảy vào rồi mờ, luôn kèm hình chứ không chỉ màu):
  - **"TUYỆT!"** vàng có sao: trúng trong cửa sổ vàng (±8° quanh đáy). Hải cẩu bay thẳng vào khe, xoay vòng đẹp.
  - **"OK!"** cam: thoát được.
  - Con thoát **bay luôn ra khỏi màn, không đứng chờ trên gờ** (user 12/11: "hải cẩu bay luôn chứ ko đứng nữa"). Vận tốc
    lúc lọt khe được ghi lại (`LaunchPhase.Exits`).
- **Khe khó hơn** (user 12/11: "bay qua khe khá dễ"): `aimAssist` 0.65 → 0.4, chỉ cú "TUYỆT!" mới được nắn thẳng vào khe.
  Lần 2 ("khe to còn quá dễ qua"): tường phải cao thêm 1.0 (`WallRaise`; thử 1.5 thì chạm mù chỉ lọt 1/6 trên máy ảo, quá khó), vẽ bằng một lớp đá cuội cùng kiểu đấu
  trường (`rock_cobble` + viền tuyết `rock_top`). Mô phỏng quỹ đạo (13 đơn vị/s, trọng lực 0.55, cản 0.1) từ đáy cung:
  khe gốc nhận góc phóng 32°–52°, tường cao 1.0 còn 40°–52° (1.5 là 44°–52°); kéo dài trần sang phải thì chặn luôn cú hoàn hảo nên
  không làm. `strikeAngle` 52 → 48 để cú "TUYỆT!" nằm giữa cửa sổ. Bot trong test chạm lúc hải cẩu đu về đáy
  (`LaunchPhase.ApproachingBottom`).
  - **"ỐI!"** đỏ: văng lệch. Hải cẩu **kẹt lại đến hết pha, giãy chân** (frame `stuck`): đập trần thì cắm ngược trong nhũ
    băng; đập tường thì trượt xuống nằm ngửa trên sàn hố (không đè lên máy bắn). Số hải cẩu kẹt là thước đo thất bại
    nhìn thấy được. Thay cho rơi mất khỏi màn. *Quyết của Claude: nóc trần nằm ngoài khung nhìn nên kẹt "trong nhũ băng".*
- **Bắn hụt:** không mất hải cẩu, chỉ mất thời gian nạp lại máy. (Bỏ "cầu tuyết bay trúng con đang chờ": cầu không bay.)
- **Con kế tiếp** đi tới đầu cầu, cúi, nắm dây, tụt xuống (hoạt ảnh nắm dây mới). Biên độ đu tăng dần theo từng con.
  Tốc độ đu tăng theo màn.
- Hết hải cẩu: chuyển cảnh nước dâng như cũ.

### R4. Pha 2 (thay §5 khi khác)
- **Thẻ nhiệm vụ** trước pha 2: "Màn N · Hạ X gấu" + "Tổng gấu: T" + hình hải cẩu **khuỵu xuống rồi đứng lên chào** (CPC: cúi
  nhặt mũ rồi chào, §14.3; dùng frame leap_0 + salute có sẵn; frame grab có vẽ dây nên không dùng). Nút Tiếp tục.
- Hải cẩu bay ngang, ra mép phải thì vào lại mép trái, **bay mãi tới khi chạm** (bỏ `maxLoops`). Sau 2 vòng, icon ngón tay
  nhấp nháy nhắc chạm.
- **Hải cẩu pha 2 nhỏ hơn**: ×0.75 so với pha 1 (`PufferController.ArenaScale`, collider co theo).
- **Lướt giữ nguyên tốc độ** (không cản khi lướt); cản `airDrag` chỉ bật khi lao hoặc va. Trước đây lướt chậm dần nên con
  vào chậm không bao giờ bay hết 2 vòng: không có gợi ý chạm, không tự lao, màn không kết thúc được (review 12/11).
- **Lao cắm đầu, nhanh** (xem lại CPC từng khung 12/11 đêm, báo cáo §14.2, thay cho bản "lao chéo chậm" trước đó): chạm
  thì hải cẩu cắm đầu và lao theo hướng 65° dưới ngang (`diveAngle`) với tốc độ bằng 2 × tốc độ bay (`diveSpeedScale`),
  `diveGravity` 0.4 kéo dốc dần. Chạm **ngay trước** con gấu (dẫn trước khoảng nửa độ cao).
- **Quỹ đạo pha 1 quyết định đường bay pha 2** (`AttackPhase.Entry`): con thoát khe với góc dốc (80°) vào cao 7.8 trên
  sàn và bay chậm; góc thoải (25°) vào thấp 6.0 và bay nhanh. Tốc độ ngang = 0.55 × tốc độ ngang lúc thoát, kẹp 3.2–6.2.
  Mỗi con một đường bay, nên phải canh lại thời điểm lao. Con không có dữ liệu thoát dùng mặc định (cao 7.0, 4.5).
- **Chạm ở đâu cũng nhận** (bản tham chiếu có lúc nuốt chạm: không làm theo).
- Gấu đứng yên tại chỗ, mỗi con một kiểu đứng chơi lặp lại:
  - đấm gió;
  - ăn tô cá;
  - ngồi ôm gối, xoay đầu;
  - khoanh tay vênh mặt;
  - hai con huých nhau;
  - ngủ;
  - tung hứng;
  - cười.
  Thêm bộ **punch / sit / smug / nudge**.
- Nảy chuỗi + combo "×2/×3" (đã có). Băng vỡ thành mảnh vuông nhỏ, khối trên mất đỡ thì sập theo. Gấu trên khối bị phá
  rơi theo và vẫn tính. Hải cẩu sau va chạm: lăn, bụi tròn, ngồi choáng, đứng dậy đi ra.
- HUD: trên-trái số hải cẩu; giữa 4 nút tròn (loa, nhạc, ?, tạm dừng); trên-phải mặt gấu + "Tổng T" + "đã hạ/cần".
- Màn chỉ kết thúc khi con cuối dừng hẳn.

### R5. Kết quả, thua, thống kê
- **Thắng:** panel "Màn N" + "Thắng rồi!" (đủ chỉ tiêu) hoặc **"Hoàn hảo!"** (hạ hết gấu). Tranh: bầy hải cẩu nhảy múa quanh
  gấu nằm ngửa mắt xoáy. Dòng "Gấu đã hạ X/Y", "Tổng gấu T", sao 1–3 bay vào, nút Bản đồ · Chơi lại · Tiếp.
- **Thua:** panel **"Thất bại!"** + mặt gấu + số đã hạ + 3 sao rỗng. Nút theo R9 (Bản đồ · Chơi lại · Quảng cáo: +1 hải cẩu · Tấn công lại); Thống kê không nằm trên thẻ thua. Sau khi
  đóng panel: cảnh ngắn gấu xách lồng có hải cẩu chạy ngang (đã có ở màn tải).
- **Thống kê** (panel, mở từ Cài đặt): tổng gấu đã hạ, số lần ×2, ×3, ×4 trở lên, màn Hoàn hảo. Lưu trong
  save, kiểm tra từng trường.
- Màn 1–3 là màn dạy: rộng tay, gần như không thua. Từ màn 4 siết lại để thua là có thể xảy ra.

### R6. Truyện mở đầu
- Khoảng 20 s, có nút **Bỏ qua**. Hiện ở lần chạy đầu, xem lại được từ menu.
- 5 tranh ChatGPT, mỗi tranh có lia/zoom chậm + vài sprite động:
  1. Làng hải cẩu ngày nắng, bầy hải cẩu chơi trên băng.
  2. Bầy gấu ác đứng khoanh tay trên đồi, cười gian.
  3. Gấu lăn một quả cầu tuyết khổng lồ, cuốn bầy hải cẩu con vào.
  4. Gấu nhốt hải cẩu vào lồng, xách đi. Kho cá bị khiêng theo.
  5. Pip trốn thoát, đội mũ len, nhìn về phía máy bắn của ông Hải Mã.
- Mỗi cảnh có một dòng chữ (qua `Loc`).

### R7. Bản đồ và menu
- Vẽ lại bằng ChatGPT: đảo băng nhìn từ trên, đường đá tròn uốn chữ S. Lồng có hải cẩu kêu "CỨU!" ở nút boss. Nút đã
  qua có sao, nút hiện tại có Pip đứng nhún.
- Menu chính: tranh tiêu đề ChatGPT (logo "Seal Catapult" + cảnh hoàng hôn), nút Chơi to, nút phụ nhỏ.

### R7b. Bố cục hố bắn theo tranh (đã làm 12/11)
Toạ độ đo trên `pit_bg.png` (1536×1024), tranh cao 11 đơn vị, tâm (0, 0.5): tường trái x −7.0, tường phải x 6.0 cao tới
y 2.3 (gờ chờ), trần dưới y 4.5, đầu nhũ y 3.8, mép trần x 2.4, sàn y −3.3, mặt cầu y 1.7 tới x −1.3, neo dây (−1.5, 1.6).
Dây dài 1.9 (cũ 2.4) để đáy cung nằm ngay trên cốc máy bắn đặt trên sàn tranh. Va chạm là hộp vô hình (`LaunchPhase.Solid`).
Đổi tranh thì đo lại các số `W(px)`/`H(px)` trong `LaunchPhase.cs`.

### R7c. Bổ sung từ báo cáo FrostFling (đã làm 12/11)
- **Pha 1 kiểu bungee** (`LevelData.launcher`): màn 5 và 8. Dây treo thẳng đứng co giãn, hải cẩu nhún lên xuống trên
  máy bắn. Đáy nhún trùng điểm máy bắn ngắm (đáy cung của dây đu). Biên độ 0,4–0,75 và tốc độ tăng theo từng con.
  "TUYỆT!" khi dây giãn hết. Ray ngắm 40° (`strikeAngle` − 8), kéo 0,7 (`BungeeAssist`; 0,85 cho chạm mù lọt 13/14 ở QA
  12/11 màn 5 và 8), nên đập lúc bầu dây đang rơi xuống mới lọt khe.
- **Sao bay vào bộ đếm gấu** mỗi khi hạ một gấu.
- **Lồng nổ tung** ở màn boss: chấn song bay ra, sao xoay, hải cẩu được thả đáp đất vẫy tay.
- **Ăn mừng trên bụng gấu**: hải cẩu dừng trên gấu đã gục thì đứng lên ăn mừng.
- **Ninja đá chéo** vào gấu đứng gần nhất phía dưới (thay cho rơi thẳng khi có gấu trong tầm 5 đơn vị).
- **Vua Gấu bốc hơi giận** từ hai tai khi đứng trên bệ.

### R7d. Sau khi chơi lại bản flashmuseum (đã làm 12/11)
- **Hang ngầm** (màn 6, 9): khe trên mặt đất rơi xuống một hang đá tối có nhũ băng. Hang sâu `AttackPhase.CaveDepth`
  2,6, trần hang dày 0,8, camera xuống theo thêm 1,6. Màn 9: nút mở cổng băng nằm dưới hang.
  Khe phải rộng ≥ 1,5 vì hải cẩu cuộn tròn có đường kính khoảng 1.
- **Gấu bị hạ văng lên xoay 1 vòng** (0,55 s) kèm mảnh băng, rồi nằm lại.
- **Choáng = 3 quả cầu tuyết nhỏ xoay quanh đầu**, cả ở pha 2 (đứng dậy) lẫn pha 1 (kẹt).
- **Vệt gió trắng** khi bổ nhào nhanh.
- **Gấu ngồi thõng chân** ở mép bệ (`BearMood.Dangle`), đặt sát mép, chân treo ngoài.
- **Thẻ nhiệm vụ**: hải cẩu chào tay (4 frame lặp).

### R7e. Giao diện theo bản tham chiếu (người dùng 12/11 đêm: "thiết kế lại UIUX, đồ họa"; báo cáo §15)
- **HUD** (cả hai pha): trên-trái mặt hải cẩu tròn (`hud_pup`) + "× N" chữ to viền; trên-giữa 4 nút tròn (loa, nhạc, ?,
  tạm dừng); trên-phải (chỉ pha 2, hiện khi vào đấu trường) mặt gấu tròn (`hud_bear`) + "Tổng N" nhỏ trên + "đã hạ/cần"
  to dưới. **Không có điểm và thanh sao trên HUD**: điểm chỉ hiện ở bảng kết quả. Quy ước góc ở §10 thay bằng dòng này.
- **Thẻ nhiệm vụ** đứng chờ nút **Tiếp tục** (chạm bất kỳ cũng được; bot test gọi `GameManager.ContinueMission`): "Nhiệm
  vụ N", hải cẩu khuỵu rồi chào, "Hạ N gấu để sang màn tiếp theo", "Tổng gấu: N" (tổng mọi lần chơi), "N hải cẩu thoát".
- **Kết quả**: "Nhiệm vụ N" / "Chúc mừng!" (hoặc "Thất bại!"), hải cẩu đứng trên bụng gấu, "Gấu đã hạ: d/r", "Tổng gấu:
  N", rồi phần riêng của game: 3 sao + điểm, nút Bản đồ / Chơi lại / **Tiếp tục** (vàng, sang màn kế).
- **Tạm dừng**: màn tối đi, chữ "TẠM DỪNG" lớn, hàng nút Tiếp tục / Chơi lại / Bản đồ / Âm thanh (không có panel).
- **Bản đồ**: nút màn là **đá "?"** (`node_stone`), màn đã qua = **đá có sao vàng** (`node_star`), màn kế = hải cẩu đứng
  trên **đệm xanh phát sáng** (`node_glow`); boss = lồng trên đá, màn thưởng = cá; không số màn, không chú thích, không tên
  vùng. Giữ của game: đếm sao góc phải, nút Nâng cấp, hàng 3 sao nhỏ dưới đá đã qua, khóa dưới đá chưa mở.
- **Menu chính**: logo ngang giữa trên, nút Chơi vàng to ở giữa, hàng Cách chơi / Cài đặt / Giới thiệu dưới; loa / nhạc /
  ? trên-giữa. **Lần Chơi đầu** (chưa có sao màn 1) vào thẳng màn 1, không qua thẻ Cách chơi và bản đồ: màn 1 tự dạy bằng gợi ý trong game (R9, 15/11; trước đó mở thẻ Cách chơi).
- **Tường phải hố bắn** cao thêm `WallRaise` ngay trong tranh: `Tools/prep_backgrounds.py` `raise_wall()` kéo giãn phần
  trên của tường vẽ lên `WALL_RAISE` 98 px (= 1,0 đơn vị; đổi `WallRaise` thì đổi cả hai). Không vẽ khối đá riêng đè lên:
  đá nâu của đấu trường lệch màu với đá xám của hố (người dùng 13/11: "tường ko nhất quán").
- Sau khi lao, camera **giữ ở mức thấp nhất đã xuống** cho tới khi con đó xong (CPC §16.2); nảy khỏi gấu `bearBounce` 7,5.
- Màn dạy chơi (`showTutorial`) kéo cú bắn về khe mạnh hơn: `TutorialAssist` 0,8 thay cho `aimAssist` 0,4.
- **Boss theo CPC** (người dùng 13/11: "cpc đâu có thanh máu", "cx ko có vua gấu"; báo cáo §10.6): boss là **gấu cỡ
  thường** bốc hơi giận trên bệ, **không thanh máu, không "Vua Gấu" khổng lồ**. Hải cẩu lao trúng trực tiếp = mất 1 mạng
  (sao, choáng); màn 12 boss có 2 mạng. Nút sàn chỉ **hất boss khỏi bệ xuống sàn một lần** (dễ trúng hơn), không trừ mạng.
  Các mục cũ nói "Vua Gấu Băng to gấp đôi / thanh máu dọc / trèo lại bệ" ở §0, §2, §8, §11 không còn đúng.
  `ui_bar` (64×32 px) dùng viền 9-slice 14 px (viền 44 px chung của `ui_*` lớn hơn ảnh).
- **Giữ cửa hàng và sao** (người dùng 13/11: "vẫn giữ cửa hàng"): bản đồ có đếm sao + nút Nâng cấp + hàng sao nhỏ; bảng
  kết quả có 3 sao. **Bỏ điểm**: không hiện điểm ở kết quả, không còn chữ "+điểm" bay lên (điểm vẫn tính ngầm cho sao thứ 3).
- **Pha 1 theo CPC §16.1** (người dùng 13/11: "xem kĩ lại cpc để sửa lại pha 1 và 2"): lối thoát là cả khoảng hở trên tường
  phải (`WallRaise` 0, tranh không kéo tường nữa: `WALL_RAISE` 0), cú bắn dẹt hơn (`strikeAngle` 40). Thay các số 12/11
  (tường nâng 1,0, góc 48).
- **Pha 2 theo CPC §16.2**: mọi con bay ngang cùng độ cao và tốc độ (`Entry(null)`: cao 7,0, 4,5 u/s); **bỏ liên kết cú bắn
  pha 1 → đường bay pha 2** (`GameManager` không truyền `launch.Exits` nữa).
- **Khối đứng yên** (CPC §17.1): mọi `Block` là `Static`; khối bị trúng thì vỡ, khối bên cạnh giữ nguyên chỗ, không có tháp đổ.
  Gấu vẫn là vật lý nên rơi khi khối dưới chân vỡ.
- **Vật lý và hoạt ảnh theo CPC** (người dùng 13/11): mọi hải cẩu nảy thêm `BaseBounces` 2 lần như quả bóng sau cú trúng đầu
  (nâng cấp Cuộn tròn cộng thêm), kéo **đuôi khói xám** (`Fx.Smoke`) khi đang nảy; **gấu luôn quay mặt sang trái** (phía
  hải cẩu bay vào; người dùng 13/11), không quay theo hải cẩu.
  Màn 2 = cổng vòm (2 cột 3 khối + dầm 5 khối, cần 3), màn 3 = tháp 6 khối + dầm chữ T + bậc băng (cần 4), mỗi màn 6 con;
  `Design.Row` xếp dầm ngang.
- **Mũ văng** (`pup_hat`) ở cú trúng đầu tiên; **vết lõm** (`dent`) chỗ trúng đá; **khối nêm băng** (`block_wedge`,
  `BlockSpec.wedge` ±1, `Design.Wedge`) ở chân cột và đầu dầm; **màn nước kéo từ trên xuống** giữa hai pha
  (`HudView.Curtain`): phủ hố, camera sang đấu trường sau màn nước, rồi kéo lên lộ thẻ nhiệm vụ. Hình mới:
  `Assets/Art/Reference/arena_extras.png`. Sau khi mũ văng, quả bóng đổi sang `pup_ball_bare` (đầu trần). Bộ hình đầu trần (đứng, vùng vẫy, đi, choáng,
  đứng dậy, cổ vũ, bóng) cắt từ `pup_sheet_{a,c,e}_bare.png` và `pup_walk_bare.png` (ChatGPT sửa bảng gốc, bỏ mũ) bằng
  `slice_sheets.py` với cùng tỉ lệ bảng gốc; `PupRig.GoBare()` chuyển sang bộ này. `pup_ball_bare.png` trong Reference là
  bản thử đầu, không còn dùng. Shortcut: khi
  đứng dậy đi ra thì hình vẫn có mũ. Màn 5, 6, 7, 9, 10 đã thêm cổng vòm, dầm che gấu và nêm theo kiểu CPC.
- **Bỏ nốt những thứ CPC không có** (người dùng 13/11): thẻ "Màn N" đầu màn, tuyết rơi nền. Bản đồ có chữ **"CỨU!"** đỏ
  nháy cạnh lồng cho tới khi thắng boss (`help_cry`).
- **Va chạm theo CPC, đo lại từng khung 13/11 (báo cáo §18)**: gấu trúng đòn lộn nhào có sao, nhảy nhẹ (`KnockUp` 3,5) rồi
  **rơi xuyên sàn, biến khỏi màn** (không nằm lại; bỏ cổ vũ trên bụng gấu). Hải cẩu nảy thấp: `bearBounce` 3,6,
  `bounceKeep` 0,5, vật liệu nảy 0,15; **trượt** trên tuyết và dốc (ma sát 0,08).
- **Pha 1 giữ khó + liên kết sang pha 2** (người dùng 13/11 tối: "còn quá dễ", "chưa có nhiều quỹ đạo ảnh hưởng pha 2"):
  `WallRaise` 1,0, `strikeAngle` 48, `TutorialAssist` 0,6, `GameManager` lại truyền `launch.Exits`. Bỏ thử nghiệm "lối
  thoát rộng, bay ngang cố định" của buổi chiều.
- **Hố bắn là hộp đá kín như CPC** (người dùng 13/11: "bố cục đất đá chưa giống cpc ở pha 1"): trần đá chạy suốt, đầu phải
  cắt chéo; cột tường phải cao (đỉnh `RightWallTop` = H(320) (ở 350 chạm mù lọt 6/6 trên máy ảo); ở 250 không con nào lọt, ở 290 chỉ cú 41° hoàn hảo lọt, 35° và 47° trượt). Tính tay: khe nhận góc khoảng 37–44° từ đáy
  cung đu (mái chéo chặn phía trên), nên `strikeAngle` 41 và ray bungee không hạ góc nữa (`BungeeAimDrop` 0); lối thoát duy nhất là khe chéo giữa hai thứ đó. Sàn: khối
  đá dưới cầu, dốc xuống **hốc trũng** đặt máy bắn, bờ thấp dốc lên chân cột. Tranh: `pit_bg_box.png` (bố cục) + cột của
  `pit_bg_box2.png` hạ 220 px + viền `pit_side_walls_box.png`, ghép bởi `Tools/prep_backgrounds.py`. Va chạm đo lại trên
  tranh mới (`Poly` cho mái chéo, khối trái, bờ phải). `WallRaise`/`raise_wall` không còn dùng (`WALL_RAISE` 0).
- **Đá pha 2 = đá pha 1** (người dùng 13/11: "pha 2 đất đá chưa nhất quán"): `rock_cobble` và `rock_ramp` giờ cắt từ chính
  tranh đá của hố (`pit_side_walls_box.png`, nửa trái) trong `prep_backgrounds.build_terrain`; không còn dùng đá nâu và
  dốc vẽ riêng của `arena_terrain.png` (chỉ còn lấy viền tuyết `rock_top` từ đó).
- **Dốc đá pha 2** vẽ bằng chính ô đá cuội lát của khối (`rock_cobble`) trên một **lưới tam giác** cùng tỉ lệ ô đá (SpriteMask không hiện
  khi chụp; `rock_ramp` không còn dùng), viền tuyết `rock_top` xoay theo mặt dốc: dốc và khối cùng cỡ đá, cùng viền.
- **Hải cẩu bắn trượt** (pha 1): **đập vào đâu dính ngay tại đó** đến hết pha (người dùng 13/11), đầu cắm vào đá, chân
  đạp ra ngoài theo pháp tuyến mặt đá (trần, mái chéo, tường, bờ, sàn). Không rơi, không dời chỗ.
- Mọi mảnh đá pha 2 (gò, sàn, vách, dốc) vẽ bằng `AttackPhase.Cobble`: lưới phủ vân đá theo **toạ độ thế giới**, nên đá
  chạy liền từ mảnh này sang mảnh kia, không còn đường nối. Hải cẩu trượt ở pha 1 dùng tư thế `Stuck` xoay theo mặt đá.
- Ô đá `rock_cobble` làm lặp bằng `crossfade_tile` (hoà chồng mép 100 px); `seamless()` cũ để lại đường cắt cứng giữa mỗi ô.
- **Viền tuyết** `rock_top` và **khớp tròn** `snow_knob` do `prep_backgrounds.draw_snow` vẽ (trắng, bóng xanh nhạt, nét
  navy như tuyết của hố); không lấy từ `arena_terrain.png` nữa (dính dải đá nâu). Mỗi đầu dốc có một khớp tròn đặt đúng **giao điểm hai đường tâm
  dải tuyết** (dốc với mặt bằng trên/dưới); dải viền dốc chạy đúng từ khớp này tới khớp kia. Khối không có khớp riêng.
- **Tuyết và đá pha 2 lấy thẳng từ tranh hố pha 1**: `rock_top` là dải mũ tuyết vẽ tay của hố (cắt từ
  `pit_side_walls_box.png`), cao `RimHeight` 0,7; ô đá `rock_cobble` trải 5,5 đơn vị mỗi ô để viên đá to cỡ đá của hố.
- **Tranh hố là MỘT tranh liền** `pit_bg_wide4.png` (16/11: bản wide3 có khối đá dưới cầu bị khoét bớt, ChatGPT sửa) (16:9, 1672×941; đá bao quanh nằm sẵn trong tranh), `PaintingHeight`
  12,64 nên rộng 22,5 đơn vị, đủ cho 20:9 (21:9 hở 0,4 đơn vị mỗi bên). **Mặt trên cột phải dốc chéo song song với mái
  chéo** (người dùng 13/11), thành máng thoát chéo; va chạm `Column Slope`. Không còn ghép cột, ghép viền (`pit_bg_box*.png`, `pit_side_walls_box.png` chỉ
  còn là nguồn vân đá/tuyết cho pha 2). Số đo `LaunchPhase` lấy lại trên tranh này (mép trong cột H(355)).
- **Đá pha 2** phủ bằng một mảng tường đá nguyên của tranh (không hoà chồng), lật gương theo chiều ngang
  (`wrapModeU = Mirror`), cao 11 đơn vị nên không lặp theo chiều dọc.
- Vùng "đập đẹp" của dây đu `SweetWindow` 4° (8° cho 6/16 nhịp lọt máng, quá dễ).
- **Pha 2 xa hơn, bay cao hơn, lao chéo chậm hơn** (người dùng 13/11): `AttackZoom` 1,15 (trước 0,9), tâm khung nhìn cao
  4,5 trên sàn, hải cẩu vào ở độ cao 7,4–9,0 (`SpawnPoint` 8,2). Lao `diveAngle` 45° (trước 65°), `diveSpeedScale` 1,3
  (trước 2). Hải cẩu bay vào và vòng lại **ngoài mép khung nhìn** (`AttackPhase.ViewLeft/ViewRight`), không hiện ra giữa màn.
  Bầu trời rộng 30, sàn kéo thêm 12 về bên phải cho khung nhìn rộng.
- **Tuyết pha 2 là MỘT dải liền** (người dùng 13/11: "tuyết nối vẫn chưa khớp"): `AttackPhase.BuildSnow` lấy đường
  viền cao nhất của mọi mặt đá (mặt khối, mặt dốc), phủ một dải lưới `rock_top` chạy theo nó, gấp góc vát (miter) ở mỗi
  khúc cua, vân tuyết liền mạch; chỉ ngắt ở vách đứng. Khối và dốc không còn dải tuyết riêng, không còn khớp tròn.
- Hải cẩu bay cao hơn nữa: vào ở 8,8–10,4 (`SpawnPoint` 9,6), `AttackZoom` 1,3, tâm khung nhìn 5,3 trên sàn.
- Gờ băng nổi dùng cùng dải tuyết `SnowStrip`. Vết lõm chỉ in trên vách đá đứng (mặt trên và dốc đều có tuyết phủ). Mũ văng là vật lý
  nhỏ (không va gấu, không va hải cẩu), nằm trên đá 2,5 giây rồi mờ dần (`FadeAway`).
- **Gờ nổi bên phải là đá cuội** (người dùng 13/11: băng không vỡ được thì thay bằng đá): chỉ khối phá được mới là băng.
- **Độ bền khối theo CPC** (người dùng 13/11: "có các loại băng sau bao nhiêu lần chạm mới vỡ, xem lại cpc"; báo cáo §19):
  CPC chỉ có **một loại băng, vỡ ngay lần chạm đầu**; **thùng gỗ không vỡ khi bị đập**, chỉ biến mất khi đáp nút cùng nhóm
  (cầu thùng gỗ ở màn boss). Game: băng và tuyết vỡ khi hải cẩu chạm (bỏ máu theo lực va); `Stone` (vẽ là thùng gỗ) bất
  hoại trừ khi là cổng có nhóm. `iceHp`/`snowHp`/`stoneHp`/`minDamageImpulse` không còn dùng cho va chạm.
- **Va chạm theo CPC màn 1–5 chơi tay** (báo cáo §20): hải cẩu **đang bay ngang chạm băng thì phá khối và bay xuyên tiếp**;
  **lao trúng băng** thì khối vỡ, hải cẩu bật ngược sang trái ngắn (`IceRebound` 1,4); **lao trúng dốc đá thì không nảy mà
  trượt bụng xuống dốc** (tối thiểu `SlideSpeed` 4) với tia lửa ma sát; nảy khỏi gấu thấp (`bearBounce` 2,6).
- Vách trái đấu trường chỉ chạy sang trái `CliffReach` 14 (30 thì ló vào mép phải khung hố trên máy 20:9).
- Sprite mới từ ChatGPT: `Assets/Art/Reference/map_ui_kit.png` → `hud_pup`, `node_stone`, `node_star`, `node_glow`,
  `hud_bear` (thay bản cũ cùng vòng viền với hải cẩu), `ui_card` (panel trơn). Cắt bằng `Tools/cut_props.py`.

### R8. Thứ tự làm (mỗi bước: build + test + chơi trên máy ảo)
1. Tranh ChatGPT: nền hố bắn, máy bắn, ô đấu trường, khối, UI kit, bản đồ, menu, truyện, thắng/thua, hải cẩu và gấu frame mới.
2. Cắt tranh và nối vào game (đặt va chạm hố bắn theo tranh).
3. Luật pha 1 (R3), pha 2 (R4), kết quả/thống kê (R5), truyện (R6).
4. Mượt (R2): frame rate, squash/stretch, tween UI, camera.
5. Kiểm tra: EditMode + PlayMode, review theo khu vực thay đổi, APK, QA trên `Pixel_QA_34`.

### R9. Rà soát toàn dự án (15/11/2026, người dùng: "rà soát lại toàn bộ dự án … sửa và kiểm hết")
Luật áp dụng cho mọi màn hình và màn chơi từ nay:
- **Nút Back của Android hoạt động ở mọi màn hình**: trong màn chơi = tạm dừng/tiếp tục; thẻ kết quả = về bản đồ; shop, cài đặt,
  thống kê, hỏi xóa, cách chơi, giới thiệu, truyện = đóng cái trên cùng; bản đồ = về menu; menu = thoát game. Màn hình mới
  phải có `HandleBack()` và được gọi từ view gốc của scene.
- **Chạm**: mọi ngón tay đều tính (`GameInput.ActionPressed` duyệt hết `touches`), không chỉ ngón đầu tiên.
- **Cụm nút tròn trên-giữa** cách nhau đúng `UiKit.MinHit` (130) để vùng chạm không chồng nhau. Nhãn nút chừa 20 px mỗi đầu.
- **Shop**: 3 thẻ nằm trong ±800 (vừa 4:3), dùng safe area; nút giá chỉ ghi số + sao; chưa đủ sao thì nút xám + ổ khóa, bấm vào
  hiện "Cần thêm N sao". Nút Nâng cấp trên bản đồ có huy hiệu sao ghi số sao còn tiêu được, nhấp nháy khi mua được gì đó.
- **Thẻ kết quả**: bỏ dòng "Tổng gấu" (vẫn có ở thẻ nhiệm vụ và trang Thống kê) để lấy chỗ ghi luật dưới 3 sao; mỗi sao nằm trên đúng luật của nó (sao giữa có thể trống khi sao thứ ba đạt; ở màn boss luật sao giữa ghi "Hạ trùm khi còn dư hải cẩu"). Thắng màn boss
  ghi câu kết. Màn bonus ghi "Cá đã gom x/y".
- **Màn 1–3 là màn dạy** (`tutorial: true`): màn 3 cần 3 gấu (trước là 4 và tắt trợ giúp). Màn dạy vẽ 8 chấm theo đường lao
  chéo từ chỗ hải cẩu đang bay; chữ gợi ý là "Chạm sớm! Hải cẩu lao chéo về phía trước".
- **Nút "?" trong màn chơi** chỉ hiện gợi ý của pha hiện tại.
- **Hiệu năng**: texture Android nén ASTC (tranh nền 6×6, sprite và UI 4×4); bản đồ (màn tĩnh) chạy 30 fps và không dựng
  backdrop dưới `map_bg`; tranh truyện được giải phóng sau khi xem (`Art.Release`).
- **Quảng cáo**: chỉ bản cài từ Google Play mới dùng ad unit thật (`Application.installerName`); mọi bản khác dùng unit test.
  Nội dung quảng cáo giới hạn ở mức PG.
- **Chơi lại trên thẻ thua = chỉ pha tấn công** (người dùng 15/11: "làm những j chưa làm đi"): nút vàng ghi "Tấn công lại",
  vào thẳng đấu trường với đúng những hải cẩu đã thoát lần đó (cùng độ cao và tốc độ bay vào), không qua hố bắn và thẻ nhiệm
  vụ. Quảng cáo +1 hải cẩu cũng vậy, thêm 1 con. Khi đó thẻ thua có 4 nút: Bản đồ (chỉ còn biểu tượng) · Chơi lại (cả màn từ
  hố bắn, để người thoát được quá ít hải cẩu không bị kẹt) · Quảng cáo: +1 hải cẩu · Tấn công lại (vàng). Không con nào
  thoát thì chỉ có "Chơi lại" cả màn. Chơi lại ở menu tạm dừng và thẻ thắng luôn là **cả màn** từ hố bắn (`GameManager.Retry` so với `RetryLost`, `SceneLoader.KeptExits`).
- **Cảm giác đánh trúng**: mỗi gấu ngã có một nhịp khựng (thời gian ×0,05 trong 0,06 s thực) và một rung nhẹ; con gấu cuối
  của màn (hoặc trùm) ngã chậm (×0,3 trong 0,45 s). Tạm dừng luôn thắng: nhịp khựng không bắt đầu hay kết thúc đè lên nó
  (`GameManager.TimeDip`). Rung dùng phản hồi chạm của Android (`Haptics.Tap`, không cần quyền, theo cài đặt máy); cũng rung khi bắn "TUYỆT!".
- **Phần thưởng quảng cáo**: sau khi quảng cáo đóng, game chờ 1 giây cho callback thưởng (trước là 0,3 s: callback chậm thì xem
  xong mà không được hải cẩu).
- **Vùng 2 — Sông Băng / Glacier River** (người dùng 16/11: "6 màn vùng 2"): màn 13–18, `zone = 1` (bảng màu ban ngày, đá
  xám), mở sau khi thắng boss. Dùng lại các vật thể đã có (bumper, TNT, hang, nút + cổng băng, bungee), nhiều gấu hơn và cần
  hạ nhiều hơn (4–5). Bản đồ có **trang**: trang 1 `map_bg` (màn 1–12), trang 2 `map_bg_2` (tranh ChatGPT
  `Assets/Art/Reference/map_glacier.png`); nút mũi tên vàng ở mép trái/phải đổi trang, Back lùi trang rồi mới về menu, bản đồ
  mở ở trang có màn đang chơi. Thắng boss: thẻ ghi "Vịnh Băng Giá đã được giải cứu! Tiếp tục tới Sông Băng" và Tiếp tục
  sang màn 13. Thẻ kết thúc và luật sao "hạ trùm" gắn với **màn boss** (`LevelKind.Boss`), không phải màn cuối.
- **Tên game** (người dùng 16/11: "trên ggdich là phóng cún con mà", "làm SEO thật tốt nhé"): thương hiệu tiếng Anh đổi
  từ "Pup Launch" thành **Seal Catapult** (`docs/ASO.md` §3b); tiếng Việt giữ "Hải Cẩu Bay". Package `com.sealcatapult.game`,
  namespace `SealCatapult`.

### R10. Giao diện phương án A "Arcade Bold" (người dùng 16/11: "làm A thử đi"; 6 phương án ở canvas Seal Catapult UI Concepts)
Học theo Angry Birds 2; thay cụm nút tròn trên-giữa của R7e:
- **Menu chính**: góc trái trên = chân dung hải cẩu + tên vùng + "Màn N" (màn sẽ chơi); góc phải trên = số sao còn tiêu
  · nút Cách chơi · nút Cài đặt. Logo giữa trên. Hai **dải nút lớn** hai bên (300×112): trái Nâng cấp (mở shop ngay tại menu)
  · Truyện; phải Bản đồ · Thống kê. Dưới giữa: **một nút Chơi to** "Chơi · Màn N" vào thẳng màn tiếp theo
  (`MapView.CurrentLevel`), không qua bản đồ. Bật/tắt âm thanh, nhạc nằm trong Cài đặt (thanh trượt) và menu tạm dừng.
  Nút Giới thiệu bỏ khỏi menu (tác giả, phông chữ đã có trong Cài đặt).
- **HUD**: chỉ còn **một nút Tạm dừng** ở góc phải trên; bộ đếm gấu dời sang trái nó. Giữa trên (chỉ ở đấu trường, không có
  ở màn cá): **thanh tiến tới sao thứ 3** (điểm phá / `threeStarScore`), ngôi sao cuối thanh sáng lên khi đạt. Nút "?" bỏ
  (gợi ý tự hiện ở màn dạy và khi hải cẩu bay vòng lâu).
- **Menu tạm dừng**: thêm nút tròn bật/tắt nhạc dưới hàng nút (nút Âm thanh vẫn ở cuối hàng).
- **Thẻ thắng**: Bản đồ và Chơi lại thành nút vuông chỉ có hình (120×108); "Tiếp tục" là nút lớn duy nhất (560).
- **Nhất quán mọi trang** (người dùng 16/11: "nhất quán lại mọi thứ của app"): màn toàn trang (Bản đồ, Nâng cấp) dùng chung
  **thanh trên** `UiKit.TopBar`: nút Back vuông 104 ở góc trái, tiêu đề 64 px cạnh nó, viên sao ở góc phải. Bản đồ: tiêu đề
  là tên vùng của trang, không còn nút âm thanh/nhạc. Bảng popup (Cách chơi, Giới thiệu, Thống kê, Cài đặt) dùng chung dải tiêu đề
  `UiKit.Ribbon`; bảng cao (Cách chơi, Cài đặt) thu nhỏ theo chiều cao màn hình để dải không bị cắt trên 21:9. Nút chính luôn Vàng, nút phụ Xanh, nút nhấn mạnh Cam; trang mới phải dùng hai helper này.

## Mục lục

0. [Quan sát từ bản tham chiếu](#0-quan-sát-từ-bản-tham-chiếu-chơi-thật--xem-3-video-11112026)
1. [Tầm nhìn & trụ cột](#1-tầm-nhìn--trụ-cột)
2. [Thế giới & nhân vật](#2-thế-giới--nhân-vật)
3. [Vòng chơi](#3-vòng-chơi)
4. [Pha 1 · Hố bắn](#4-pha-1--hố-bắn)
5. [Pha 2 · Đấu trường](#5-pha-2--đấu-trường)
6. [Kỹ năng bay & nâng cấp](#6-kỹ-năng-bay--nâng-cấp)
7. [Vật thể trong đấu trường](#7-vật-thể-trong-đấu-trường)
8. [Màn chơi & độ khó](#8-màn-chơi--độ-khó)
9. [Điểm, sao, hoàn hảo](#9-điểm-sao-hoàn-hảo)
10. [Màn hình & trải nghiệm](#10-màn-hình--trải-nghiệm)
11. [Nghệ thuật](#11-nghệ-thuật)
12. [Âm thanh](#12-âm-thanh)
13. [Kỹ thuật](#13-kỹ-thuật)
13b. [Quảng cáo](#13b-quảng-cáo-admob-người-dùng-13112026)
14. [Kế hoạch sản xuất](#14-kế-hoạch-sản-xuất)
15. [Rủi ro](#15-rủi-ro)
16. [Quyết định cần bạn chốt](#16-quyết-định-cần-bạn-chốt)

---

### R11. Hành động, trạng thái, hoạt ảnh theo CPC (người dùng 16/11: "xem lại cpc … thiết kế lại tất cả … thật cẩn thận")
Đối chiếu đầy đủ ở `docs/cpc-gap.md` (chơi lại CPC trên flashmuseum 16/11 + báo cáo cũ). Đã làm:
- **Lao**: xoay đầu xuống trong 0,12 s rồi giữ thẳng như ngư lôi theo hướng bay; chỉ cuộn tròn và xoay khi chạm vật đầu tiên
  (`PufferController.NoseDown` / `BallUpOnImpact`).
- **Hải cẩu nằm**: đứng dậy, ngồi choáng tại chỗ (sao quay, khói trên đầu) 1,1 s rồi mờ dần; không đi bộ ra nữa.
- **Đu dây**: thân văng rộng hơn dây 1,5 lần (tối đa 95°), tay vẫn ở đầu dây; ở hai đầu cú đu thân gần nằm ngang.
- **Hàng chờ**: mỗi con có nhịp riêng, cứ 2–5,5 s lại reo 1 s hoặc quay lại nhìn hàng; con lên dây ngồi xổm 0,25 s ở mép cầu.
- **Chữ OK / GREAT / OOPS**: nghiêng 10°, đứng yên, nằm dưới cầu bên trái. **×N combo**: nghiêng 12° ngay chỗ gấu ngã.
- **Va chạm**: vòng bụi tuyết bắn ra khi vỡ băng và khi chạm gấu; chớp trắng tròn khi đập đá; 8 mảnh băng lơ lửng 3–4,5 s sau khi vỡ.
- **Màn nước** chuyển pha: hải cẩu khổng lồ treo ở mép sóng, đung đưa.
- **Gấu canh** quay mặt theo hải cẩu vừa bay qua (`Bear.WatchPup`).
- **Trùm**: sao quay trên đầu mỗi lần trúng; bị hạ thì văng lên và lộn nhào ra khỏi đấu trường.
- **Thẻ kết quả**: thắng thường 1 hải cẩu reo; Perfect 4 con không mũ nhảy múa + sao lấp lánh quanh tiêu đề; thua: 2 gấu cười.
- **Bản đồ**: lần đầu qua một màn, mở bản đồ thấy hải cẩu nhảy vòng cung từ ô vừa thắng sang ô kế (`MapView.HopFrom`).
- **Tranh mới** `pup_sheet_h.png` (ChatGPT sửa từ sheet F; người dùng 16/11: "vẽ hải cẩu quay về trước", nên vẽ **nhìn thẳng**):
  vẫy tay, ngả mũ, nhìn quanh (động tác của hàng chờ, chọn ngẫu nhiên cùng reo), và hải cẩu kẹt **mất mũ**, mũ nằm cạnh
  (`PupRig.Pose.Wave/HatTip/Look`, `stuck_bare` khi `GoBare`).
- `pup_sheet_i.png` (người dùng 16/11: "vẽ lại cả khung đứng chờ và đi bộ nhìn thẳng"): đứng chờ (khung 3 là chớp mắt, slicer
  `REORDER` đổi thứ tự), đi bộ lạch bạch 4 khung, đứng không mũ, **nhặt mũ đội lại** (thẻ nhiệm vụ: nhặt mũ rồi đứng thở).
- `pup_sheet_j.png` (người dùng: "nhiều hoạt ảnh ngộ nghĩnh"): nhảy cẫng, lắc mông, ngáp vươn vai, hắt xì bay mũ.
  Hàng chờ chọn ngẫu nhiên 7 động tác (vẫy, ngả mũ, nhìn quanh + 4 cái này), mỗi con 0,8–3 s một lần, kéo 1,6 s.
- **Hàng chờ thoáng** (người dùng: "cho hàng đứng thoáng ra"): cách nhau 0,85 (cũ 0,6); hàng trước 6 con, con thứ 7 trở đi
  đứng hàng sau cao hơn 0,18, chen giữa hai con hàng trước (cách 0,78), đủ 12 con (8 + School 3 + quảng cáo 1).
- `pup_sheet_k.png` (người dùng: "thêm hoạt ảnh đu nữa"): đu dây nhìn thẳng (hai tay giơ lên bám, đầu ngón tay = điểm dây),
  **đu nhanh** hớn hở đá chân (`hang_fast`, tự đổi khi tốc độ dây > 2,2 rad/s), với tay bám dây (`reach`), ngồi chờ ở mép (`ready`).
- Tất cả tranh hải cẩu nhìn thẳng (H, I, J, K) dùng chung `FRONT_SCALE = 1.0` trong slicer: đứng cao ~270 px, bằng con đang đu
  (người dùng 16/11: "hàng ở trên bị to hơn con đang đu" ở tỉ lệ 1,226).
- Camera đấu trường trôi ngang ±0,15 (chạm theo thời điểm, không theo vị trí, nên không lệch ngắm); bụi tuyết ở đầu cần bắn mỗi phát.
- **Đá dưới cầu khoét bớt** (người dùng 16/11: "cắt bớt đất đá bên dưới đi để hải cẩu đu vào đó ko bị đè"): khối đá giờ
  kết thúc dưới giữa cầu (`BlockEndX` W(478), mặt dốc tới đáy hố ở W(620)); hải cẩu đu không còn lấn lên đá.
- **Hàng chờ một hàng đều** (16/11: hàng sau chen chúc, "đang lỗi khá nhiều"): khoảng cách = min(0,85; 4,37/(số con−1)),
  tính lúc dựng hàng. Động tác chờ chạy khung qua lại 0-1-2-3-2-1 (`PupRig.Anim.PingPong`) ở 5–8 khung/giây.
- **8 khung mỗi động tác** (người dùng 16/11: "chưa đủ khung hình để hoạt ảnh mượt mà"): `pup_sheet_l`–`o` (ChatGPT), mỗi
  động tác trải 2 hàng: vẫy, ngả mũ, nhìn quanh, nhảy, múa, ngáp, hắt xì, đi bộ. `PupRig.LoadAll` lấy đủ số khung có sẵn và
  đặt tốc độ theo thời lượng một vòng, nên 4 hay 8 khung chạy cùng nhịp. Sheet J (4 khung) bỏ. Mọi sheet nhìn thẳng được
  cân **theo cỡ mũ đỏ** (`FRONT_BEANIE` 40,8 px) vì mỗi lần ChatGPT vẽ hải cẩu to nhỏ khác nhau; 2 ô "ACHOO" bị nền cắt
  lẹm mặt nên dùng ô bên cạnh (`REORDER`).
- **Chống nhấp nháy** (người dùng 16/11: "hải cẩu trên hàng bị nhấp nháy"): đứng chờ là MỘT khung (`pup_idle_2`), thở bằng
  co giãn trong code (±1,8 %, lệch pha từng con); slicer xoá các mảng rời nhỏ (tia vàng, gạch chuyển động: `keep_body`) và
  cân từng khung theo mũ của chính nó (`even_size`) với các động tác mũ luôn trên đầu. Con ở mép cầu: ngồi xổm → bật lên
  dáng đu → theo đầu dây xuống (bỏ dáng với tay vì trông như chìm qua cầu).
- **Xem lại CPC 17/11** (người dùng: "xem lại pha 1 và pha 2 của cpc thật kĩ, quan sát vật lí, hoạt ảnh"; video Flash
  `1Dp_WM-9ag0` từng khung 60 fps + chơi lại flashmuseum): 
  - Hải cẩu bị đập bay ra **không xoay vòng**: đầu đi trước theo hướng bay, vây ra sau, chỉ lắc ±18° tắt dần
    (`LaunchPhase.Fly`, cũ: xoay 540° trong 0,35 s).
  - **Chớp trắng** ở chỗ cầu tuyết chạm hải cẩu và **tia vàng** ở đầu dây vừa trống.
  - **Dây cong**: vẽ bằng 6 đoạn sprite lát (`SpriteDrawMode.Tiled`, `rope` là FullRect) theo đường cong bậc hai; giữa dây
    trễ sau đầu dây theo tốc độ đu (`BowPerOmega`), thẳng lại ở hai đầu cú đu.
  - Lao trúng gấu: **mũ hải cẩu bay lên đội trên đầu gấu** rồi lộn nhào rơi cùng gấu (`HatOntoBear`); trúng thứ khác mũ vẫn văng tự do.
  - Hết lượt nảy bóng mà còn trớn: hải cẩu **duỗi ra lộn nhào** (`Pose.Tumble`); chạm dốc đá thì **nằm bụng trượt**
    theo dốc (`Pose.Slide`), ra tới chỗ phẳng thì nằm ngang lại. Tranh mới `pup_sheet_s.png` (ChatGPT sửa sheet E không mũ,
    8 khung trượt + 8 khung lộn); viền đầu bị hở nên slicer dùng ngưỡng nền 248 cho riêng sheet này (`WHITE_FOR`).
- **Vỡ băng theo CPC** (người dùng 17/11: "xem lại băng cpc xem cơ chế vỡ, hoạt ảnh"; "xem các loại băng sau bao lần va
  chạm là vỡ"; video 60 fps màn 1–12):
  - **Độ bền**: cả 12 màn chỉ có MỘT loại băng (khối vuông + nêm), vỡ ngay lần chạm đầu ở mọi tốc độ (bay ngang, lao,
    lăn, trượt); thùng gỗ màn boss không vỡ khi đập. Game đã đúng (khối tuyết riêng của mình cũng 1 chạm).
  - **Hoạt ảnh vỡ**: khối biến thành **đống cục băng nhỏ lấp đúng hình khối**, treo tại chỗ ~1,2 s rồi lấp lánh và tan
    (không rơi, không nằm lại); 4–6 cục **bật lên chậm** rồi rơi; chỗ va có **vòng tròn vàng** loé ra và **khói xám**.
    Code `Fx.IceBreak` + `Fx.HitRings`, tranh `ice_fx.png` (ChatGPT theo nét `block_ice`, `cut_props.py`: `ice_chunk_0..8`,
    `impact_ring(_small)`, `smoke_ball`, `twinkle`). Bỏ mảnh tam giác vẽ code, đá vụn nằm lại mãi và mảnh lơ lửng 4 s cũ.
  - **Lăn xuyên**: hải cẩu đã chạm lần đầu mà còn lăn/trượt thì chạm băng nào phá băng đó và đi tiếp (giữ 85 % tốc độ),
    không nảy ngược; phá liên tiếp cả hàng như CPC.
- **Hải cẩu kẹt tường** (người dùng 17/11: "hải cẩu dính tường chưa đẹp và hợp lí"): bản cũ dán khung ngồi có mũ nằm
  cạnh lên đá, nên mũ dính lơ lửng và nhiều con chồng lên một chỗ. Giờ: tranh `pup_sheet_t.png` (8 khung nằm ngửa, lưng
  phẳng ở mép dưới khung, tay chân dang đạp, không mũ), cắt neo đáy, đặt mép lưng đúng điểm chạm (`StuckSink` 0,04);
  mũ văng ra và rơi khỏi màn (`KnockHatOff`); con sau chạm gần con trước thì trượt dọc vách sang trái 0,55 cho tới chỗ
  trống (`ClearOfStuck`). Thẻ thua dùng cùng 8 khung đạp chân.
- **Chiều lao** (người dùng 17/11: "hải cẩu lao cx chưa đúng chiều"): khung `pup_dive_1` vẽ sẵn đầu chúc ~−70°, code lại
  xoay thêm cả góc bay nên đầu quay ra sau. Giờ xoay `góc bay − DiveArtAngle`: đầu luôn đi trước theo hướng lao.
- **Bỏ quả bóng tròn, nhiều kiểu hoạt ảnh** (người dùng 17/11: "bỏ hoạt ảnh tròn khi văng đi, thay vào đó là hoạt ảnh văng
  mới như cpc, nhiều kiểu vào … mỗi loại hoạt ảnh sẽ có nhiều kiểu"; "tăng khung hình lên để hoạt ảnh mượt mà"; "cẩn thận vẽ lỗi"):
  - Cú chạm đầu tiên không còn cuộn thành bóng: hải cẩu văng ra kiểu búp bê vải, khung hình tự xoay (thân không quay).
    4 kiểu văng: lộn nhào, nhào lộn dang tay (`fling_cartwheel`), lộn ngược (`fling_backflip`), xoắn ốc (`fling_corkscrew`).
    Chỉ nâng cấp Ice Spikes còn là quả bóng gai (đó là hình của nâng cấp).
  - Mỗi hải cẩu bốc một kiểu riêng (`PupRig.Variant`) cho từng loại: bay ra ở pha 1 (cũ, hoảng loạn `launch_panic`, siêu
    nhân `launch_hero`; mỗi bộ có góc vẽ riêng `ArtAngle` 35°/25°/15°), kẹt vách (nằm ngửa đạp chân, vùi đầu `stuck_buried`,
    mặt bẹp `stuck_splat`), nghỉ (ngồi choáng, hoặc nằm bẹp co giật `flat`).
  - Tranh: sheet U, V, W, X (ChatGPT sửa từ S, D, T). Mỗi hoạt ảnh được vẽ lại 16 khung (`pup_sheet_16_<tên>.png`, chèn khung
    giữa); slicer tự thay bộ 8 khung khi có bản 16 (`with_smooth`), `PupRig.Loop` đếm số khung và giữ thời lượng một vòng.
  - Soi từng khung trước khi nhận: bản 16 khung "bay hoảng" đầu tiên đổi sang mặt siêu nhân ở nửa sau nên bị loại, vẽ lại.
- **Ba kiểu thả có hoạt ảnh riêng, hài hước vừa phải** (người dùng 17/11: "3 hoạt ảnh dây lắc lư, dây nhún, nhảy ko dây cx
  phải khác nhau … hài hước"; rồi "hài hước đừng lố quá"): đu dây = `pup_sheet_y` (16 khung, thân lắc theo dây, chân văng),
  dây nhún = `pup_sheet_z` (`Pose.Bungee`, 16 khung: chìm duỗi chân, bật co gối cười), nhảy không dây = `pup_sheet_q2`
  (bịt mũi nhảy, rơi vỗ vây). Khung đu và nhún chọn theo pha dây (`SwingRope.Phase` → `PupRig.DrivenPhase`), không chạy
  tự do, nên thân khớp nhịp dây; khung đu tự nghiêng nên dây không vung thân thêm (`SwingIsDrawn`). Bản vẽ "lố" đầu tiên
  (lè lưỡi, thân kéo dãn, nhún treo ngược) bị loại.
- **Kẹt vách kiểu CPC** (người dùng 17/11: "vẽ lại hải cẩu dính … dính chưa nằm đúng vị trí", "nhìn hơi xấu, học kiểu của
  cpc"; chơi CPC trực tiếp): chim kẹt trần nằm ngửa ép lưng vào đá, mặt và bụng quay ra, vây dang chữ T, chân đạp, mất mũ;
  chim kẹt dốc nằm bẹp dọc mặt đá. Game: chạm trần (normal.y < −0,3) → `Pose.Stuck` = `pup_sheet_t3` (16 khung, neo đỉnh
  đầu vào đá); chạm vách/dốc → `Pose.StuckSlope` = `pup_sheet_t2` hàng 3–4 (neo bụng vào đá, đầu hướng ra chỗ trống). Chỗ
  kẹt phải có đá thật (`ClearOfStuck` dò lại mặt đá bằng `NearestRock`; trước đó con bị đẩy quá mép vách lơ lửng giữa trời).
  Bỏ các bản vùi đầu / mặt bẹp / cụm tuyết (sheet X, X2) vì xấu và không giống CPC.
- **Văng theo CPC** (video 60 fps): cú chạm đầu ném hải cẩu theo cung dài cao (`FlingSpeed` 7,5, góc ≥50°, thường bật
  ngược lại), mỗi lần đập vách sau đó nảy mạnh kèm vòng vàng + khói (`FlingKeep` 0,65); trúng gấu cũng văng xa như vậy
  (bỏ cú nảy thấp `bearBounce`).
- **Sửa 17/11 (tiếp)**: không còn va chạm với cầu (hải cẩu bay qua cầu, chỉ đá chặn); hải cẩu kẹt = một kiểu bẹp như bánh,
  tay chân dang, mặt quay ra (`pup_sheet_t4`, 16 khung, lưng áp đá), dính ngay chỗ chạm, chồng lên nhau được (người dùng:
  "dính đè nhau cx đc"). Đu dây cân hai bên: chỉ dùng nửa trái của bộ khung (0 = xa trái … 4 = thẳng) và lật gương cho bên
  phải theo góc dây (`SwingRope.Lean` → `PupRig.SwingLean`). Văng theo CPC (xem lại 20–30 fps): giữ MỘT tư thế vùng vẫy và
  xoay cả thân ~1 vòng/0,5 s theo chiều bay (`Tumble`/`Spin`), không cuộn tròn, không đổi sang khung lộn nhào; mỗi lần đập
  có mây tuyết trắng + vòng vàng + vài cục tuyết (`Fx.Splat`), nảy 3 lần (`BaseBounces` 3, giữ 75 % tốc độ).
- **Văng theo CPC, xem lại 60 fps (17/11)** (người dùng: "vẽ lại văng, ko đc tận dụng", "cpc có kiểu văng lộn lung tung
  các hướng và kiểu lộn tròn"): trên không là MỘT dáng vẽ riêng `pup_sheet_f2` (gập người về trước như ngồi giữa không,
  chân thò dưới bụng, vây hất ra sau; 16 khung rung nhẹ) và code xoay cả thân, mỗi lần đập đổi chiều/tốc độ ngẫu nhiên và
  có khi lật mặt (`Spin`) → lộn lung tung mọi hướng. Hết lượt nảy mà còn trớn trên sàn thì cuộn tròn `pup_sheet_r2` và
  lăn như bánh xe (`RollOn`, quay theo tốc độ dọc mặt đất). Bỏ các bản: thân cong cứng, co người nhìn lưng.
- **Luồng sang màn như CPC** (người dùng 17/11: "khi sang màn mới phải hiện map để hải cẩu di chuyển sang ô mới đã thì mới
  sang màn tiếp theo"): "Tiếp" ở thẻ thắng mở bản đồ, hải cẩu nhảy từ ô vừa thắng sang ô kế (`MapView.HopFrom`), đáp xong
  0,6 s thì tự vào màn đó (`MapView.PlayAfterHop`); không có cú nhảy (màn cũ đã thắng) thì chờ 1,2 s rồi vào.
- **Mỗi thành một bản đồ riêng, 10 thành, 41 màn/thành** (người dùng 17/11: "sang thành khác thì map cx phải khác", "màu
  nền của các map cx phải khác nhau", "mỗi thành có 41 màn", "tối đa 10 thành"): Vịnh Băng Giá (xanh dương, map 1), Hẻm
  Núi Băng (tím), Rừng Cực Quang (xanh ngọc), Cảng Đóng Băng (hoàng hôn), Hang Pha Lê (ngọc lam), Núi Lửa (đỏ cam), Đảo
  Anh Đào (hồng), Cung Điện Băng (xanh đêm + vàng), Đồng Cỏ Hổ Phách (vàng nâu), Ngai Vua Gấu (xám bão). Bảng thế giới:
  `WorldTheme.cs` (tranh, tên `world_*` 7 thứ tiếng, màu thành). Pha bắn và pha tấn công nhuộm theo màu thành
  (`WorldTheme.Tint`; người dùng: "các pha cx phải theo màu của thành").
  **Luật bản đồ** (người dùng 17/11): chỉ MỘT con đường, không rẽ nhánh, không nhánh cụt; đường trải khắp bản đồ; mỗi bản
  một dáng đường riêng; nhìn tự nhiên như map 1, không phải hình học đè lên tranh; làng ở đầu đường, cổng pháo đài ở cuối
  đường (đường không chạy quá đích). Cách làm (`README.md` "World maps"): mình vẽ sơ đồ một nét (`Tools/map_plan.py`),
  ChatGPT tô theo sơ đồ với map 1 làm mẫu phong cách và chấm đỏ giữa đường, `Tools/map_roadcheck.py` chấm PASS/FAIL,
  `Tools/map_trail.py` dò chấm → `MapTrails.cs` và xoá chấm thành tranh sạch. Để ChatGPT tự nghĩ đường thì lần nào cũng
  ra ngã ba; hai khúc đường gần nhau trong sơ đồ cũng bị nhập thành ngã ba (giữ ≥ 250 px).
  41 màn: lồng ở màn 10, 20, 30, 40, trùm thành ở 41 (`LevelGenerator.PerCastle` 41). Lưu ý: đổi 40→41 dời chỉ số màn của
  thành 2 trở đi một ô.
- **Bản đồ sống động như CPC** (người dùng 17/11: "xem trên cpc ở trang map thì các animation, hoạt ảnh, UIUX nào mà game còn
  thiếu thì thêm vào"; xem video 60 fps ở 12 lần mở bản đồ: 10, 63, 122, 206, 269, 347, 419, 514, 585, 664, 720, 785 s).
  CPC có và game đã có sẵn: đá "?" xám, đá đã thắng thành đĩa sao, đệm xanh ở màn hiện tại, hải cẩu nhảy sang ô kế rồi tự
  vào màn, vẫy tay / ngả mũ khi chờ, lồng trên đế cam, chữ "HELP!" đỏ ở lồng gần nhất, trùm ở cổng thành.
  Thêm 17/11: (1) **cá nhảy khỏi mặt nước** theo vòng cung, để lại vòng sóng, vài giây một lần (`MapLife`; điểm nước do
  `Tools/map_trail.py` tìm trên tranh → `MapTrails.<World>Water`; chỉ thế giới có nước hở: Vịnh, Hẻm núi, Cảng, Hang,
  Cung điện, Ngai vua; dùng lại 1 cá + 2 vòng, không cấp phát mỗi lần); (2) **tia lấp lánh** bay ra khi ô vừa thắng bật
  thành sao (`Sparkle`) kèm tiếng `star` (ô lồng: tiếng `cage`); (3) **đệm xanh ô kế bật sáng** trước khi hải cẩu nhảy;
  (4) **lồng rung** từng đợt khi tù nhân kêu cứu (`Rattle`); (5) tiếng `boing` lúc bật nhảy, `pop` lúc đáp.
  Không làm theo CPC: nút âm thanh / nhạc / trợ giúp ngay trên bản đồ (game này để trong Cài đặt, §R10), gấu đứng trang
  trí giữa đảo (tranh mỗi thành đã vẽ sẵn cảnh).
- **Xem lại bản đồ CPC 18/11** (người dùng: "xem lại map trên cpc đi xem còn thiếu hoạt ảnh, animation… thì bổ sung"; video
  60 fps, phóng to từng vùng ở 9,8–11,6 s và 513–521 s). CPC có mà game còn thiếu, đã thêm:
  (1) **gấu trắng đứng chơi cạnh đường**: thở, gãi, vài giây quay đầu nhìn phía kia (`MapBear`; chỗ đứng do
  `Tools/map_trail.py bear_spots` tìm: thân gấu nằm trọn trên tuyết cùng màu mặt đường, phẳng, cách mọi ô đá ≥ 80 px,
  trong 260 px quanh đường; `MapTrails.<World>Bears`). Hiện có ở Vịnh Băng Giá (2) và Đảo Anh Đào (1); các bản đồ khác vẽ
  dày (cây, đá, nhà sát đường) nên không còn chỗ trống, để trống còn hơn đặt gấu đè lên cảnh;
  (2) **tù nhân trong lồng vẫy tay** cho tới khi được cứu (tranh lồng trống; hải cẩu `pup_wave_` vẽ sau song lồng);
  (3) **trùm đấm gió** (`bear_punch_`) xen dáng đứng và khiêu khích.
  CPC còn có hải cẩu ngoi lên khỏi nước rồi lặn, game đã có cá nhảy cùng chỗ nước nên giữ cá.
- **Hải cẩu trên ô và tù nhân, học CPC** (người dùng 18/11: "hải cẩu di chuyển trên ô nhìn chưa nổi bật", "hải cẩu trong
  lồng bị lồng che … chưa có hoạt ảnh help", "nhìn cách thiết kế của cpc mà học tập"; video 514–517 s): hải cẩu to gần
  gấp đôi ô đá, bóng dưới chân, đệm xanh lớn nhịp thở (thử mũi tên vàng: nhỏ, đè ô bên trên, CPC không có → bỏ); sang ô mới thì **đi lạch bạch** dọc
  đường (khung `pup_walk_`, nảy nhẹ từng bước, 1,1 s) như CPC chứ không nhảy vòng cao. Tù nhân: hoạt ảnh mới `pup_help_`
  (8 khung, giơ hai tay vẫy, há miệng kêu, nhảy nhẹ; `pup_sheet_help.png`) đứng trong lồng, song sau tối, song trước
  mờ 35% nên thấy rõ; lồng gần nhất có chữ **CỨU!** đỏ to viền trắng, lắc lư, hiện 2 s lúc mở bản đồ rồi thu nhỏ biến mất,
  5 s lặp lại (`Shout`), lồng rung song.
- **Không khí riêng từng thành trên bản đồ** (người dùng 17/11: "ở các map hãy thêm các thứ sinh động phù hợp với phong
  cảnh thành để thêm sinh động, hiện tại hơi tĩnh"; `MapWeather.cs`, bảng `Of(world)`): Vịnh Băng Giá tuyết rơi + ánh
  lấp lánh trên băng; Hẻm Núi tuyết tím nhạt + pha lê lấp lánh; Rừng tuyết dày + dải cực quang nhấp nháy; Cảng ánh nắng
  vàng lấp lánh + ít tuyết; Hang đốm sáng ngọc lam bay lên + pha lê lấp lánh; Núi Lửa tàn lửa cam bay lên; Anh Đào cánh
  hoa hồng bay xoay; Cung Điện sao vàng lấp lánh + tuyết thưa; Đồng Cỏ lá cam bay ngang theo gió; Ngai Vua bão tuyết xiên
  + chớp sáng hai nhịp. Thêm thành mới = thêm một dòng trong `Of`. Mọi hạt gộp sẵn (không cấp phát sau `Start`), nằm trên
  Canvas con riêng để hạt bay không làm dựng lại ~200 hình ô đá; vẽ trên ô đá, dưới các nút.
- **Phong cảnh hai pha theo thành** (người dùng 17/11: "mỗi thành thì cx thay đổi phong cảnh trong cả pha 1 và pha 2 (đất
  đá, băng…)", "đổi cả thanh đứng nữa"): mỗi thành có 3 tranh riêng, ChatGPT vẽ, đã có đủ 9 thành (Vịnh Băng Giá dùng
  tranh gốc):
  - Pha 1 `pit_<thành>.png`: bản SỬA của `pit_bg_wide5.png`, giữ nguyên mọi đường viền (va chạm đo trên tranh gốc) nhưng
    đổi đá, băng, trời, cảnh xa và chất liệu thanh cầu (pha lê tím, gỗ thông, ván tàu, gỗ mỏ, sắt đinh đỏ, gỗ sơn đỏ, khung
    vàng kính, gỗ lũa, sắt gai). Kiểm tra: chồng viền với tranh gốc ≥ 92% (Ngai Vua 70% do đá đổi dáng, chơi thử vẫn khớp).
  - Pha 2 `arena_sky_<thành>.png` (nền, giữ đường chân trời cùng độ cao) và `wall_<thành>.png` (mảng đá lát).
  `Tools/prep_backgrounds.py` → `bg_pit_<thành>`, `bg_arena_sky_<thành>`, `rock_cobble_<thành>`; `WorldTheme.Scene` chọn
  tranh của thành, thiếu tranh thì dùng tranh Vịnh Băng Giá nhuộm màu thành. Chỉ dùng tranh vẽ: không tô lại màu bằng code
  (người dùng từ chối cách đó).
  Lưu ý ChatGPT: hết lượt tạo ảnh của gói Plus sau khoảng 30 ảnh, mở lại sau 14 giờ; đôi khi trả ảnh sai cỡ (1536×1024)
  làm lệch khung → gửi lại đúng tranh đã khớp vào chat mới và yêu cầu giữ 1672×941; đôi khi đưa 2 phương án "Bạn thích
  ảnh nào hơn?" phải chọn trước khi tải; Chrome chặn tải nhiều lần từ một tab → mở tab mới cùng chat để tải.
- **Dọn ảnh không dùng** (người dùng 17/11: "các ảnh nào ko dùng nữa thì xóa đi cho nhẹ dự án"): bỏ các tranh hố cũ
  (`pit_bg`, `pit_bg_wide`–`wide4`, `pit_bg_box`, `pit_bg_box2`, `pit_side_walls`; tranh dùng là `pit_bg_wide5`), sheet hải
  cẩu bị loại (`pup_sheet_b/c_rejected_style`, `pup_reference*`, `pup_ball_bare`), `map_world_stones` (dùng bản `stones40`),
  và trang bản đồ 2 cũ (`map_glacier` → `map_bg_2`; bản đồ giờ là một tranh `map_bg_world`). Còn giữ: ảnh `cpc_ref_*`
  (tham chiếu), `feature_graphic`, `app_icon`.
  Dọn lần 2 (18/11, người dùng: "tranh nào ko dùng thì xóa đi"): bỏ 6 hình game không còn gọi (`hud_badge`, `rock_ramp`,
  `sign`, `snow_knob`, `whacker_arm`, `whacker_base`) và bỏ luôn chỗ công cụ vẽ ra chúng (make_art, prep_backgrounds,
  cut_props); bỏ sheet 8 khung `pup_sheet_s/t/u/v/w` (đã có bản 16 khung thay) và `arena_terrain`. Cách tìm: tên hình không
  có trong code C# (kể cả ghép tên kiểu "pup_" + tên hoạt ảnh), tranh tham chiếu không công cụ nào đọc ngoài chú thích.
Chưa làm: mây trôi riêng (mây vẽ liền trong tranh nền); bánh răng quay (cần tranh bánh răng).

### R12. Thành không giới hạn, mỗi thành một bản đồ 60 màn như CPC
(người dùng 16/11: "vô hạn thành và nhiều màn như cpc"; "thiết kế lại trang map, học tập cpc, và thiết kế cho t nhiêu đó màn
trong 1 thành như cpc")
- **Mỗi thành = 1 bản đồ 60 màn**, chơi lần lượt từng ô như CPC (bản đồ CPC ~50–60 ô, 5 lồng cách nhau ~10 ô, gấu trùm cuối).
  Màn 10, 20, 30, 40, 50 là **lồng giải cứu** (trùm nhỏ, lời "các bạn hải cẩu trong lồng đã được thả"), màn 60 là
  **trùm của thành** ("{tên thành} đã được giải cứu!"), màn 25 và 45 là **cá thưởng**. Mở khoá: màn trước đã có sao
  (màn đã thắng luôn mở). Thành sau mở khi hạ trùm màn 60. Chỉ số: thành k = k·60 … k·60+59 (`LevelCatalog`).
- **18 màn đầu vẽ tay** (asset, giữ nguyên thứ tự: màn 11 cá, màn 12 trùm/lồng); mọi màn sau do **`LevelGenerator`**
  dựng lúc chạy từ bộ khối `LevelBlueprint` (cột đá, tháp băng có nêm, vòm, cao nguyên, đồi, lính gác sau khối tuyết, hang,
  cổng băng + nút, TNT, bệ nảy, mỏm rìa). Hạt giống = vị trí màn → chơi lại vẫn đúng màn đó.
- **Độ khó** d = thành·6 + vị trí/10 (tăng 1 mỗi 10 màn): gấu 4 → 10, dư gấu 2 → 1 → 0, dư hải cẩu 3 → 1, mũ giáp tới 60 %,
  khối đá tới 40 %, hang/cổng/bệ nảy nhiều dần; trùm 2 → 5 mạng (trùm cuối +1). Bảng màu đấu trường đổi theo từng đảo
  (mỗi 20 màn).
- **Bản đồ** (`MapView`): tranh `map_world.png` (ChatGPT, 3 đảo băng, đường mòn chữ S từ làng igloo tới pháo đài gấu, cầu gỗ,
  thang dây). 60 ô "?" nhỏ (58 px) rải đều theo đường mòn (`Trail`, đo trên tranh; `Resample`), ô lồng/trùm đứng trên đế cam,
  "HELP!" ở lồng gần nhất chưa mở, ô đã qua hiện sao nhỏ, hải cẩu đứng ở ô kế. Ô co giãn cùng tranh trên mọi tỉ lệ màn hình.
  Mỗi thành một trang (tông màu riêng), trang thành kế hiện sẵn (khoá). Bảng sao: sao của thành /180.
- Bệ nảy đặt cao hơn vật cao nhất bên dưới ≥ 1,0 (bỏ nếu phải cao quá 5,6); đồi chỉ ở lô rộng ≥ 3,4; khe hang rộng 2,0;
  gấu sau cổng băng ở 10,1 / 12,0 cách TNT 11,05.
- **16/11 (sau)**: mỗi thành **40 màn** (người dùng: "cho 40 màn cho thoáng ra"): màn 10, 20, 30 lồng, 40 trùm thành; cá ở 15 và
  35; d = thành·4 + vị trí/10. Đường mòn lấy từ 51 viên đá ChatGPT vẽ lên tranh (`map_world_stones40.png`, `Tools/map_stones.py`),
  40 ô rải đều trên đó, vẽ trên tranh sạch `map_world.png`. Hoạt ảnh bản đồ như CPC: ô vừa thắng bật sao, hải cẩu nhảy sang ô
  kế (khung `drop_jump`), chờ thì vẫy tay và ngả mũ, gấu trùm ở cổng cử động.
- **3 kiểu thả** (người dùng: "đu dây, dây lên xuống và hải cẩu nhảy xuống ko dây, tùy màn"): `LaunchStyle.Swing/Bungee/Drop`.
  Drop: hải cẩu ngồi xổm ở mép cầu, nhảy (0,25 s), rơi nhanh qua tầm cần bắn (trọng lực 9, tối đa 5,5); trượt thì rơi xuống
  tuyết choáng, OOPS. Màn tự sinh xen kẽ: vị trí%5==2 dây nhún, ==4 nhảy.
- **Xe bắn mới** (`catapult_parts_2.png`): bỏ nồi hơi, sàn gỗ phẳng; người kéo cần đứng NGANG quay về hố (`pup_sheet_r.png`,
  cần gạt riêng, chờ + kéo 8 khung). Tranh mới vẽ riêng cho từng việc, không mượn khung cũ.
- **Lưu**: mảng sao/hoàn hảo tự nới (`SaveData.Record`, `StarsOf`); `LevelCount` = 18 màn vẽ tay.
- Test: `LevelGeneratorTests` (ổn định; mỗi thành 5 lồng + trùm cuối + 2 cá; 8 thành × 60 màn: gấu đứng trên vật, không chôn,
  không chồng, bệ nảy/TNT không chạm gấu, đủ hải cẩu; thành 5 khó hơn thành 1).

## 0. Quan sát từ bản tham chiếu (chơi thật + xem 3 video, 11/11/2026)

Nguồn: tự chơi bản Flash (Snokido, 2 màn, mở Instructions/Pause/Help); xem khung hình theo mốc thời gian của 3 video:
bản Flash no-commentary 13 phút (11 màn đầu + 1 boss), bản **Android** v1.1.2 (menu, map, nâng cấp, 3 màn), bản Java
longplay 53 phút (đủ 3 vùng + boss cuối + kết thúc). Chỉ lấy **cách chơi, bố cục, nhịp, hoạt ảnh**; hình, tên, chữ là của mình.

### Luồng app (bản Android)
Splash logo (chim đội mũ bay ngang qua chữ) → **menu dạng danh sách** trên nền cam: Play Campaign, Play Strategy Game,
Instructions, High Scores, Settings, About; mũi tên lên/xuống cuộn danh sách, nút ✓ và Exit ở hai góc dưới → **bản đồ**
→ màn tải (gấu xách lồng có chim bị nhốt chạy ngang, thanh tiến độ) → pha 1 → pha 2 → thẻ kết quả → bản đồ. Bản Flash:
cảnh mở đầu chim bị nhốt lồng (có Skip), màn chào chỉ có Play, thanh trên 3 nút tròn (loa, nhạc, ?). Chuyển màn: logo +
**mặt nước dâng lên che màn** rồi rút xuống. Kết thúc game: "THE END", gấu và chim đứng trên tảng băng trôi.

### Bản đồ
- Flash: một đảo nhìn từ trên, ~45 nút; nút chưa chơi "?", đã qua **chấm xanh lá**, Perfect **sao vàng**; **lồng = boss**;
  chim đội mũ đứng nhún ở nút hiện tại; nút kế tiếp có chữ "HELP!" đỏ nhấp nháy khi là boss.
- Android/Java: bản đồ **cuộn được** theo các vùng (đảo băng → đảo cọ → núi lửa ở vùng cuối); dưới nút hiện **1–3 sao**;
  góc trên-phải `★ đã có/tối đa`; chú thích dưới: "Tap the Penguin to start a mission", "2 more stars to earn an upgrade!".

### Pha 1 · hố bắn (giống nhau ở mọi màn, chỉ đổi màu đá theo vùng)
- Khung: đá cuội vân băng bao quanh, trần nghiêng, nhũ băng; nền làng lều băng + mây hồng. Cầu dầm thép từ trái, chim
  xếp hàng trên cầu; **bảng số** (hiện số chim còn trên cầu) treo dưới mép cầu; dây buông từ mép cầu.
- Máy bắn: bánh xe gỗ, nồi hơi có ống khói, cần dài có **muỗng** chứa cầu tuyết; một chim đứng gạt cần cạnh máy.
- Chạm: cần vung, **cầu tuyết bay ra**; trúng chim thì chim bay vọt lên-phải, **chữ "OK!" cam** hiện ở chỗ trúng; trượt thì
  chữ **"Grr"** xanh lá (chim gạt cần bực). Chim thoát bay ra **góc trên-phải** (Flash: đáp gờ và xếp hàng chờ).
- Máy có **khói phụt** khi bắn; cầu tuyết vỡ bụi khi rơi xuống đáy.

### Pha 2 · đấu trường
- Bố cục màn: 1 màn rộng hơn màn hình một chút, **camera lia theo chim** (cả dọc và ngang). Địa hình: dốc, bệ, cột, hang,
  **tường/trần đá**, **khối băng** (lập phương, tam giác, gai), **sàn gỗ ván**, **cầu băng**. Vùng 1 hoàng hôn cam + biển băng;
  vùng 2 tuyết trắng + đá xám, trời vàng; vùng 3 **hang băng**: trần nhũ băng khổng lồ, nền cam đất, sao lấp lánh.
- HUD: trên-trái **khung gỗ có số chim còn**; trên-phải **mặt gấu + `đã hạ/cần`**; Flash thêm `Total` (tổng gấu cả game).
- Bay: chim bay ngang từ mép trái, **ra mép phải vòng lại mép trái**. Chạm: chim **cắm đầu, xoay**, rơi nhanh dần.
- Trúng gấu: gấu **ngã ngửa, sao choáng xoay**, chim **nảy lên** và rơi tiếp; nhiều gấu một lượt → chữ **×2, ×3, ×5** lớn
  màu nâu giữa màn. Gấu cầm **tô cá** ăn, có con **ngủ**, có con giơ tay. Gấu bị hạ nằm lại, không biến mất ngay.
- Trúng khối băng: **vỡ vụn** từng mảnh, chim xuyên tiếp. Trúng đá/gỗ: nảy. Đáp sàn: bụi tuyết, chim đứng dậy rồi đi ra.
- **Vòng bật cam** (bumper hình bánh răng): chạm vào là chim **bật ngược mạnh**, dùng để đánh tới gấu khuất.
- **Thùng TNT** đỏ: trúng thì nổ, phá khối và hất gấu xung quanh.
- **Nút đỏ/xanh** trên bệ: chim đáp vào nút → kích hoạt (bật gấu boss xuống, mở cửa băng, thả khối).
- Boss: gấu to trên bệ cao, **thanh máu dọc** mép phải; hạ bằng cách đáp nút để hất nó xuống nhiều lần; khi hết máu thì
  **lồng mở**, chim con nhảy ra. Lời nhắc dưới màn: "Hit the boss bear several times to knock it out".
- Kết quả: "PERFECT MISSION" + mặt gấu `X/Y` + **1–3 sao** (Android); "MISSION SUCCESSFUL" (Java); Flash: "Perfect!" + hình
  4 chim nhảy trên gấu nằm, "Bears hit X/Y", "Total bears".

### Kỹ năng (power-ups)
| Kỹ năng | Hình ảnh | Cách dùng |
|---|---|---|
| Twins | 2 chim song song | 1 lượt phóng ra 2 chim |
| Rubber Penguin | chim nảy như bóng | thêm số lần nảy ("Two more bounces") |
| Penguin on Fire | **đuôi lửa vàng-cam** khi bay | xuyên băng, mất lửa khi đụng vật cứng |
| Ninja Penguin | **bóng thoại tròn có hình chim ninja**, chim **khựng giữa không** | chạm lần 2: dừng rồi lao theo hướng mới |
Android: thẻ "Star Upgrade" sau khi đủ sao: danh sách kỹ năng, chọn một, dòng mô tả dưới, nút OK. Mỗi kỹ năng có cấp.

### Hoạt ảnh nhân vật thấy được
- Chim: xếp hàng nhún, đi trên cầu, đu dây (vây giơ lên ôm dây), bị đập bay xoay, bay ngang (vỗ cánh 2 frame), cắm đầu,
  cuộn (Rubber), bốc lửa, ninja khựng, đáp đất choáng rồi đứng dậy đi, nhảy mừng (thẻ kết quả), xoay 360° trên bệ (thẻ màn).
- Gấu: đứng thở, ăn tô cá (múc ăn), ngủ, giơ tay trêu, bị trúng ngã ngửa + sao, boss bị hất văng rồi trèo lại, xách lồng chạy.
- Hiệu ứng: chữ "OK!" / "Grr", khói máy, bụi tuyết, mảnh băng, sao choáng, chữ combo ×N, nổ TNT, mặt nước chuyển cảnh.

### Khác với game mình đang có
| Hạng mục | Tham chiếu | Game mình (build hiện tại) | Quyết định |
|---|---|---|---|
| Vật bắn | quả cầu tuyết văng từ máy, trúng chim | chày của máy đập quét trúng hải cẩu | **Theo tham chiếu** (người dùng 11/11: "bố cục, các thứ của CPC luôn"): cốc trên cần thả cầu tuyết ở 12°, cầu bay theo trọng lực 9, trúng thì hải cẩu bay theo hướng cầu; trượt thì cầu nổ bụi + "Grr…". Tường hố = đá cuội như đấu trường. HUD: cụm 4 nút tròn trên-giữa (loa, nhạc, ?, tạm dừng), điểm dưới bộ đếm gấu |
| Chim thoát | đáp lên gờ góc trên-phải, xếp hàng chờ | bay khỏi màn, chỉ đếm | **Làm gờ chờ**: hải cẩu thoát đáp xuống gờ, xếp hàng, thấy ngay số đạn |
| Pha 2 bay | ngang, lặp vòng từ mép trái | ngang, ra mép phải là mất | **Lặp vòng** 2 lần rồi mất (cho người mới thở) |
| Lao | cắm đầu rơi gần thẳng | lao chéo chậm 45° | **Theo tham chiếu**: cắm đầu, rơi nhanh dần, giữ 30% đà ngang (người dùng từng muốn "chéo, chậm": làm số điều chỉnh `diveKeepHorizontal`, `diveGravity` để thử cả hai) ❓ |
| Trúng gấu | nảy lên theo pháp tuyến, combo ×2 | đè bẹp, dừng | **Nảy + combo** |
| Camera pha 2 | lia theo chim, màn rộng hơn 1 màn | cố định | **Lia theo chim** (dọc + ngang nhẹ, màn rộng 1.3 màn hình) |
| Kỹ năng | gắn vào loại chim, mở theo màn | mua bằng sao | **Gộp**: loại hải cẩu mở theo màn, sao mua cấp |
| Kết quả | 1 thẻ, không sao | 3 sao | Giữ 3 sao + huy hiệu Perfect |
| Bản đồ | 45 nút, 5 boss, không tiêu đề | 10 nút, 1 vùng | Vùng 1: 10 + thưởng + boss; mở rộng dần |
| Phản hồi pha 1 | "OK!" khi trúng, "Grr" khi trượt, khói máy | chỉ bụi + rung | **Thêm** chữ OK/Grr và khói |
| Vật thể | vòng bật, TNT, nút, sàn gỗ, cầu băng | khối băng/tuyết, đá | **Thêm** vòng bật, TNT, nút (theo mốc M3) |
| Gấu | ăn tô cá, ngủ, trêu; bị hạ nằm lại | thở, trêu, lộn nhào mờ dần | **Thêm** ăn/ngủ, nằm lại tới hết màn |
| Boss | đáp nút hất boss, thanh máu dọc, lồng mở | chưa có | **Theo tham chiếu** (M4) |
| Vùng | hoàng hôn → tuyết xám → hang băng | 1 vùng | 3 vùng, đổi màu đá + nền |
| Chuyển cảnh | mặt nước dâng che màn | mờ dần | **Thêm** sóng nước che màn |

## 1. Tầm nhìn & trụ cột

| Mục | Nội dung |
|---|---|
| Một câu | Game canh nhịp + vật lý vui nhộn, mỗi màn 30–90 giây, chơi lại để lấy 3 sao |
| Người chơi | Mọi lứa tuổi, chơi bằng một ngón tay, quốc tế (EN mặc định, VI) |
| Nền tảng | Android, Google Play, màn hình ngang (cả hai chiều), 16:9 chuẩn, 4:3 hẹp nhất, 20:9 rộng nhất |
| Phiên chơi | 2–5 phút (3–5 màn), không cần mạng |
| Tiền | Miễn phí, có quảng cáo AdMob nhẹ tay (người dùng 13/11/2026: "đẩy dự án lên admob"), xem §13b |

**Trụ cột** (mọi quyết định đối chiếu với 4 điều này):
1. **Canh nhịp dễ hiểu, khó thành thạo:** một nút chạm cho cả hai pha; nhìn là biết, luyện mới giỏi.
2. **Phá phải đã tay:** khối văng, gấu lộn nhào, rung camera, điểm bay lên; vật lý thật nhưng hướng về niềm vui.
3. **Dễ thương, không bạo lực:** gấu bị "hạ" là lộn nhào choáng váng, không máu, không đau.
4. **Chơi lại có lý do:** sao, danh hiệu "Hoàn hảo", cá giấu, nâng cấp mở thêm cách chơi.

## 2. Thế giới & nhân vật

### Bối cảnh
Bầy gấu bắc cực chiếm Vịnh Băng Giá và giấu kho cá mùa đông của làng hải cẩu lên các bệ đá, cột băng và pháo đài khắp
vịnh. Ông Hải Mã chế máy đập đặt trong hố băng; bầy hải cẩu con xếp hàng, từng con đu dây, chờ ông đập cho bay vọt qua
khe thoát sang đấu trường của gấu. Cuối mỗi vùng, Vua Gấu Băng nhốt vài hải cẩu con trong lồng; thắng boss thì cứu được.

### Nhân vật
- **Pip và bầy hải cẩu con** ✅ (anh hùng): lông trắng, mắt to, mũ len đỏ quả bông, kính phi công, khăn xanh ngọc. Đi bằng
  hai chân sau, lao xuống thì cuộn thành quả bóng. Vẽ frame-by-frame bằng ChatGPT (sheet trong `Assets/Art/Reference/`,
  cắt bằng `Tools/slice_sheets.py`, hải cẩu đứng cao 300 px, tỉ lệ đo theo mũ đỏ). Bộ hoạt ảnh: idle 4, walk 6, leap 4,
  hang 4 (có dây + nút thắt nối tiếp dây game), flail 4, cheer 4, glide 4, dizzy 4, ball 1 (xoay bằng code), lever 8,
  launch 4 (bay chéo 35°), fall 4. Sheet mới phải dùng **Chỉnh sửa** trên sheet cũ để giữ nét (sheet tạo mới bị lệch nét).
- **Hải cẩu gạt cần** ✅: một con đứng sau đuôi xe trượt, gạt cần khi người chơi chạm (frame lever).
- **Ông Hải Mã** 🆕 (chỉ trong truyện/giới thiệu, không có trong màn chơi; có thể xuất hiện ở màn giới thiệu vùng sau).
- **Gấu bắc cực** 🔧 (kẻ địch; người dùng 11/11: thay cua bằng gấu): đứng trần trên đỉnh dốc, cột, chồng khối; thỉnh thoảng
  giơ hai tay trêu. Bộ hoạt ảnh: idle 4 (thở + chớp mắt), taunt 4, dizzy 4 (mắt xoáy, sao), ko 4 (nằm ngửa mắt X); hit = 2
  frame đầu của dizzy. Vẽ bằng ChatGPT theo `docs/bear-art-brief.md` → `bear_sheet.png` → `slice_sheets.py` (hiện là frame
  tạm vẽ bằng code, cùng tên file nên thay là xong). Loại **đội mũ băng** chịu 2 đòn (mũ là sprite rời `bear_hat`).
- **Vua Gấu Băng** 🆕 (boss cuối vùng): to gấp đôi, 8 máu, đứng trên pháo đài nhiều tầng, mỗi đòn trúng thì rụng một mảng
  áo giáp băng; có lồng nhốt hải cẩu con bên cạnh.
- **Hải cẩu con bị nhốt** 🆕: trong lồng ở màn boss và hiện trên bản đồ tại nút boss (giống ảnh mẫu #14).
- **Đàn nhạn biển** 🆕 (vùng 2+): bay cắt ngang pha 2, chạm thì hải cẩu đổi hướng.

### Cảm giác
Vui, lộn xộn, "póc" khi băng vỡ, bụi tuyết tung, gấu lộn nhào kêu "oof", sao bay. Hoàng hôn cam trên biển băng ở đấu
trường tương phản với đêm cực quang ở hố bắn và menu.

## 3. Vòng chơi

```
Bản đồ (chọn màn) → Màn chơi: Pha 1 (bắn N hải cẩu) → Chuyển cảnh (camera lia) → Pha 2 (M hải cẩu đã thoát tấn công)
→ Kết quả (sao, Hoàn hảo, cá) → Bản đồ (mở màn kế / nâng cấp)
```
- Số hải cẩu mỗi màn N = 5–8 (+ nâng cấp Bầy). Chỉ con **thoát được** hố mới được tấn công; con bay hỏng là mất.
- Thắng khi hạ hết gấu. Thua khi hết hải cẩu mà gấu còn. Còn dư hải cẩu = thưởng sao.
- Sao mua nâng cấp. Danh hiệu Hoàn hảo và cá giấu là mục tiêu chơi lại.

## 4. Pha 1 · Hố bắn

✅ Đã có trong build, bố cục ngang theo ảnh mẫu #4. Toạ độ thế giới, sàn `Backdrop.GroundY` = −4.5, tâm nhìn (0, 0.1),
camera cao 10 đơn vị (`orthographicSize` 5; màn 4:3 nới cao để luôn thấy 14.2 đơn vị ngang).

| Phần | Vị trí / số | Ghi chú |
|---|---|---|
| Trần đá + nhũ băng | y = 4.6, từ mép trái tới x = 2.6 | nhũ băng lát theo ô 4.35, ô cuối cắt đúng mép |
| Tường trái / phải | x < −6.3 / x > 6.3, tường phải chỉ cao tới y = 4.0 | khe thoát = góc trên-phải |
| Cầu dầm | y = 2.1, từ tường trái tới x = −0.4 | hải cẩu xếp hàng, bước 0.6, không cần tầng 2 |
| Bảng đếm | (−2.7, 1.35) | lùi trái để dây đu hết biên không quét qua |
| Dây | neo (−0.65, 2.16), dài 2.4; đáy cung y ≈ −0.83 | biên độ mỗi lượt ngẫu nhiên 24°–40° |
| Máy đập | hub trên cung chày đi qua đáy cung đu; mặt bệ y ≈ −3.2 | quay mặt vào hố; hải cẩu gạt cần cách tâm máy 1.75 |

**Gờ chờ** 🆕 (theo tham chiếu): hải cẩu lọt khe đáp xuống một gờ đá ở góc trên-phải (ngoài tường phải, y ≈ 4.2), hoạt ảnh
hạ cánh + đứng xếp hàng (idle/cheer), hàng dài theo số con thoát; thẻ chuyển pha hiện số con trên gờ.

**Luật:** chạm → cần gạt, chày quét lên theo chiều kim đồng hồ 0.24 s. Chày chạm hải cẩu thật (test tròn-tròn, chia nhỏ
bước) thì hải cẩu bay theo pháp tuyến va chạm + đà đu (`swingInfluence` 0.6) + đà chày, nắn nhẹ về `strikeAngle` 48°
(`aimAssist` 0.4), kẹp 20°–100°, tốc độ `strikePower` 13. Lọt khe (40°–52° từ đáy cung, xem R3) → bay tiếp sang pha 2. Đụng tường/trần/cầu →
choáng, rơi khỏi màn, mất. Đập trượt → đu tiếp, không mất gì. Hải cẩu bay nghiêng theo hướng bay (frame launch).

**Nhịp:** đập khi hải cẩu ở đáy cung hoặc vừa qua trái → lọt; đập sớm (còn bên phải) → thấp, đụng tường phải; đập muộn
(đang lên cao bên trái) → dốc, đụng trần. Gợi ý ở màn 1–2: "Chạm để đập! Đập lúc nó đu xuống thấp nhất".

🔧 Cần làm thêm: hoạt ảnh ông Hải Mã? (không — giữ hải cẩu gạt cần); hiệu ứng chày trúng (bụi tuyết, "bốp") ✅; sao trail ✅.

## 5. Pha 2 · Đấu trường

✅ Bố cục theo ảnh mẫu #5/#6/#10/#12/#13 (xem kỹ 11/11) và chơi lại 12/11 đêm (báo cáo §14.1): trời hoàng hôn cam nửa
trên, dải đồi cam, chân trời núi băng, biển băng vụn nửa dưới; sàn đá cuội mỏng; **vách đá cuội có mũ tuyết bên TRÁI**
(khối cao 4.2 tới x = −1.2, dốc 1.8 xuống sàn ở x = 0.6), **bên phải mở**, không tường. Đấu trường rộng 13 đơn vị, gốc
(40, `GroundY`+0.8), tâm nhìn gốc+(6.0, 3.7) (lệch trái 0.5 để thấy vách, gờ xa bên phải bị cắt nửa như CPC), camera thu
10% (`AttackZoom` 0.9), cố định. Khối băng vỡ để lại mảnh (`Fx.Rubble`) đến hết màn.

| Thành phần | Mô tả | Trạng thái |
|---|---|---|
| Dốc đá (`Ramp`) | tam giác vuông, nghiêng trái/phải, `rock_ramp` kéo giãn, `PolygonCollider2D` | ✅ |
| Bệ đá (`Rock`), cột đá (`Pillar`) | chữ nhật, `rock_cobble` lát + viền tuyết `rock_top` | ✅ |
| Khối băng lập phương (`Cubes`) | 0.8×0.8, vật lý thật, vỡ khi va mạnh (`iceHp` 7), xếp chồng | ✅ |
| Khối tuyết | vỡ dễ (`snowHp` 2), bụi tuyết | ✅ |
| Gấu | đứng trần trên đỉnh; hạ khi va ≥ `bearHitImpulse` 3 hoặc rơi ≥ 1.1; bị hạ thì **văng ngược xa** (`KnockUp` 7, ngang 1.8–3) | ✅ |
| Gờ băng nổi (`IceShelf`) | tấm băng dày 0.7 có mũ tuyết, lơ lửng, cứng; `EdgePerch` đặt ở x 13.6 (ngoài sân 13) với một gấu, bị mép màn cắt nửa như CPC (báo cáo §14.1); màn 1, 3, 7 | ✅ |

**Bay (theo tham chiếu):** hải cẩu vào từ mép trái gần đỉnh màn, **bay ngang không rơi** (4.5 đơn vị/s, ~3 s/màn), vỗ
vây 2 frame; ra mép phải thì **vòng lại từ mép trái** (tối đa 2 vòng, vòng 3 thì rơi mất: `maxLoops` 🆕). **Chạm:** cuộn
tròn + **quay đầu xuống** 0.2 s rồi rơi nhanh dần (`diveGravity` 0.9 🔧, giữ 30% đà ngang `diveKeepHorizontal` 0.3 🔧; vệt
mờ). **Trúng gấu:** gấu lật 90° bay ra sau + sao choáng + "+1"; hải cẩu **nảy lên theo pháp tuyến** (`bounceKeep` 0.75) và
rơi tiếp; trúng con thứ hai trong cùng lượt → "×2" lớn giữa màn, ×3… (combo 🆕, điểm nhân). **Trúng khối băng hoặc tuyết:** vỡ mảnh,
xuyên tiếp (chậm 15%). **Trúng đá:** nảy, không phá. **Đáp đất:** bụi tuyết, đứng dậy **đi bộ ra mép** rồi biến mất 🆕
(frame walk), con kế bay vào. **Camera** lia theo hải cẩu theo trục dọc (và ngang nhẹ) trong đấu trường rộng 1.3 màn 🆕.
Hạ đủ mục tiêu vẫn chơi tiếp các con còn lại để lấy Perfect (hạ **tất cả** gấu) 🆕.

🆕 Cần làm: **vòng tăng tốc** (hoop, ảnh #9), **lửa** (ảnh #10), **đổi hướng ninja** (ảnh #13) — xem §6; **cá giấu** và
**lồng** — xem §7.

## 6. Kỹ năng bay & nâng cấp

Tham chiếu gắn kỹ năng vào **loại chim** trong hàng (mũ khác nhau), mở theo màn, không mua. Game mình gộp: **loại hải cẩu**
(mũ len màu khác) xuất hiện trong hàng từ màn mở khoá, mỗi loại có kỹ năng riêng tự kích hoạt; **sao mua cấp** để kỹ năng mạnh
hơn (3 cấp). Hàng chờ hiện rõ mũ nào sắp tới lượt nên người chơi lên kế hoạch được.

| Kỹ năng | Tác dụng trong pha 2 | Cách dùng | Mở ở | Trạng thái |
|---|---|---|---|---|
| Gai băng (Ice Spikes) | xuyên 1/2/3 khối băng khi lao | tự động | màn 3 | ✅ |
| Cuộn tròn (Bouncy) | nảy thêm 1/2/3 lần sau va chạm | tự động | màn 2 | ✅ |
| Bầy hải cẩu (Pod) | +1/+2/+3 hải cẩu mỗi màn | tự động | màn 4 | ✅ |
| Chân chèo lái (Steer) | đổi hướng trên không 1/2/3 lần | vuốt trái/phải | màn 6 | ✅ (chưa có hướng dẫn) |
| Vòng băng (Boost Ring) 🆕 | vòng đặt trong màn; bay qua vòng → tăng tốc, bay thẳng tiếp 3 đơn vị | bay qua | màn 5 (vật thể màn) | 🆕 |
| Cầu lửa (Blaze) 🆕 | chạm lần 2 khi đang lao → bốc lửa, xuyên mọi khối băng cho tới cú va đá/gấu; mất lửa khi va | chạm lần 2 | màn 7 | 🆕 |

❓ Có giữ Steer (vuốt) không, hay gộp vào "đổi hướng ninja" (chạm lần 2 khi đang lao → quay 90° xuống thẳng)? Một ngón tay
chạm nhất quán hơn vuốt.

Giá (sao) cấp 1/2/3: Gai 1/2/3 · Cuộn 1/2/3 · Bầy 1/2/3 · Ninja 2/2/3 · Lửa 2/2/3 = 32 sao cho cả shop; 12 màn cho tối đa
36 sao (R9, 15/11: giá cũ cộng lại 117 sao nên không bao giờ mua nổi 1/3 shop; test `EconomyTests` giữ tổng giá ≤ số sao có trong game).

## 7. Vật thể trong đấu trường

| Vật thể | Mô tả | Trạng thái |
|---|---|---|
| Cá giấu 🆕 | **Chưa làm** (15/11: code chỉ có cá ở màn bonus; Hoàn hảo = hạ hết gấu, và gom đủ cá ở màn bonus). Ý tưởng cũ: 3 con cá mỗi màn đặt ở chỗ khó (sau cột, trên cao); bay/lăn qua là nhặt | 🆕 |
| Vòng băng 🆕 | xem §6 | 🆕 |
| Thùng gỗ 🆕 | khối nặng (`stoneHp` 22), không vỡ dễ, dùng làm bệ như ảnh #12 | 🆕 (sprite có `block_stone`) |
| Lồng băng 🆕 | ở màn boss, treo cạnh boss; thắng boss thì mở, hải cẩu con nhảy ra | 🆕 |
| Nhũ băng rơi 🆕 (vùng 2) | va vào trần đấu trường làm nhũ rơi xuống gấu | 🆕 |

## 8. Màn chơi & độ khó

**Cấu trúc:** 3 vùng × (10 màn + 1 màn thưởng + 1 boss). MVP = vùng 1 đủ 10 màn ✅ + màn thưởng 🆕 + boss 🆕.

| Vùng | Chủ đề | Cái mới | Trạng thái |
|---|---|---|---|
| 1. Vịnh Băng Giá | hoàng hôn, biển băng | dốc, cột, khối băng, gấu mũ băng, vòng băng (màn 5), lửa (màn 7) | ✅ 10 màn (chưa có vòng/lửa) |
| 2. Sông Băng | ban ngày xanh | thùng gỗ, nhũ băng rơi, nhạn biển, khối trượt | 🆕 |
| 3. Đêm Cực | đêm cực quang | tảng băng trôi di chuyển, gió ngang, boss cuối | 🆕 |

**Màn thưởng** (ảnh #11): không có gấu; 24 con cá rải trên không; mỗi hải cẩu bay qua nhặt; điểm = số cá; không mất sao.
**Màn boss** (ảnh #12): Vua Gấu Băng 8 máu trên pháo đài 3 tầng + lồng; nút boss trên bản đồ có hình hải cẩu bị nhốt (ảnh #14).

**Vùng 1 — tóm tắt 10 màn** (chi tiết toạ độ trong `Assets/Editor/LevelDesigns.cs`):

| Màn | Hải cẩu | Gấu | Địa hình | Dạy gì |
|---|---|---|---|---|
| 1 | 5 | 1 | đồi giữa (dốc–bệ–dốc) | đập nhịp, chạm lao chéo |
| 2 | 5 | 2 | đồi thấp trái, bệ cao phải | chạm sớm khi mục tiêu cao |
| 3 | 6 | 2 | gấu trên cột khối băng + gờ phải | phá khối băng để gấu rơi |
| 4 | 6 | 3 | cột đá cao (gấu mũ băng), bệ + khối | gấu 2 đòn |
| 5 | 6 | 3 | dốc dài, cột khối giữa, bệ cao phải | 🆕 vòng băng đầu tiên |
| 6 | 7 | 3 | gờ dài, 3 tháp khối băng | chọn thứ tự mục tiêu |
| 7 | 7 | 4 | đồi lớn + gấu dưới sàn | 🆕 cầu lửa xuyên tháp |
| 8 | 7 | 3 | 3 cột cao thấp, tháp khối giữa | cột hẹp cần lao chính xác |
| 9 | 8 | 3 | tháp đôi trên bệ, gấu sát tường | nảy khỏi tường phải |
| 10 | 8 | 5 | pháo đài khối băng trên bệ rộng + 2 cột gác | tổng hợp |
| Thưởng | 6 | 0 | cá rải trên không | 🆕 |
| Boss | 8 | Vua + 2 | pháo đài 3 tầng, lồng | 🆕 |

Độ khó tăng bằng: số gấu, độ cao/độ hẹp của bệ, gấu mũ băng, ít hải cẩu dư. Mỗi màn chơi thử phải thắng được bằng "bot"
trong test tự động (`FullLevelReachesAResult`) và bằng tay trên máy ảo.

## 9. Điểm, sao, hoàn hảo

- Điểm: tuyết 50, băng 120, thùng 200, gấu 500, cá 100 (🆕), còn dư hải cẩu +200/con (🆕).
- Sao: ★ qua màn (hạ đủ số gấu yêu cầu) · +★ còn dư ≥ 1 hải cẩu (chỉ xảy ra khi hạ hết gấu trước con cuối) · +★ điểm ≥ 60% tổng điểm phá được của màn (tự tính khi dựng màn). Thẻ kết quả ghi luật dưới từng sao (R9).
- Màn bonus (cá) luôn dùng hết hải cẩu nên chấm theo cá: ★ chơi xong · +★ gom ≥ 50% cá · +★ gom ≥ 80% cá (`StarRating.ComputeBonus`).
- **Hoàn hảo** 🆕 (ảnh #7 "PERFECT MISSION"): hạ hết gấu **và** nhặt đủ 3 cá → huy hiệu vàng trên nút màn, +1 sao thưởng (❓).
- Lưu: sao/màn, huy hiệu/màn, cấp nâng cấp, cài đặt, ngôn ngữ (JSON, có kiểm tra từng trường, lỗi thì về mặc định).

## 10. Màn hình & trải nghiệm

Canvas 1920×1080, `matchWidthOrHeight` 0.5. Quy ước góc (R7e, 12/11): **trên-trái** mặt hải cẩu + "× N"; **trên-giữa** 4
nút tròn (loa, nhạc, ?, tạm dừng); **trên-phải** mặt gấu + Tổng + `đã hạ/cần` (pha 2); **dưới-giữa** thanh chú thích một
dòng. Không có điểm trên HUD. Panel nằm ngang, nút xếp hàng. (Từ R10: HUD chỉ còn **một** nút Tạm dừng ở góc trên-phải,
loa và nhạc nằm trong bảng Tạm dừng.) Vùng chạm ≥ **130** đơn vị canvas (`UiKit.MinHit`, ~48 dp; 1 đơn vị ≈ 0,37 dp):
nút nhỏ hơn được đệm vùng chạm vô hình. Luật bố cục chung ở mục "Rà soát 20/11" bên dưới.

| Màn hình | Nội dung | Trạng thái |
|---|---|---|
| Khởi động | logo, nạp save, ≤ 2 s | ✅ |
| Menu chính | sân khấu trái (hải cẩu bay vệt sao, máy đập, gấu trên khối băng), logo + cột nút phải | ✅ |
| Bản đồ | đảo 1920×1080, 10 nút hình S (hàng dưới 1→6, hàng trên 7→10), nút thưởng + boss có lồng 🆕, dấu chân, hải cẩu đứng ở màn kế, huy hiệu Hoàn hảo 🆕 | ✅ (thiếu thưởng/boss/huy hiệu) |
| HUD | như quy ước góc; gợi ý chạm ở màn 1–2 | ✅ |
| Tạm dừng | 2×2: Tiếp tục, Chơi lại, Bản đồ, Âm thanh | ✅ |
| Kết quả thắng | dải tiêu đề, 3 sao lần lượt, điểm, +sao, "Hoàn hảo!", cá 🆕; Bản đồ · Chơi lại · Màn tiếp | ✅ (thiếu cá) |
| Kết quả thua | hải cẩu choáng; Bản đồ · Chơi lại (cả màn) · Quảng cáo: +1 hải cẩu · Tấn công lại (nút chính, vàng; xem R9) | ✅ |
| Cửa hàng | thẻ 2×2 (→ 2×3 khi thêm Lửa 🆕), icon · tên · 3 viên cấp · Mua + giá; thanh mô tả | ✅ |
| Cài đặt | nhạc, hiệu ứng, ngôn ngữ, xoá dữ liệu (xác nhận), credits | ✅ |
| Cách chơi | 4 bước 2×2 | 🔧 cập nhật chữ theo gấu/lao chéo |
| Giới thiệu vùng 🆕 | thẻ 1 ảnh + 1 câu khi vào vùng mới | 🆕 |

Hướng dẫn bằng hình (icon chạm nhún) + một câu; chữ ít để dịch nhanh. Mọi chữ qua `Loc.cs` (EN/VI).

## 11. Nghệ thuật

**Phong cách:** vector mềm, viền navy `#16304f` ~5 px, chuyển màu nhẹ, highlight trắng; nhân vật chibi mắt to. Hai bảng
màu: đêm cực quang (menu, hố bắn: xanh đêm, băng, cực quang lục) và hoàng hôn (đấu trường: cam `#ff8a3d`→vàng→kem, biển
băng `#a9d8f0`). Cam/đỏ (mũ, lửa, vòng) là màu nhấn.

**Pipeline:** nhân vật (hải cẩu, gấu, boss) = sheet ChatGPT 4×4 → `slice_sheets.py` (brief trong `docs/`). Vật thể, nền,
UI = SVG trong `Tools/make_art.py` → `rsvg-convert` → `Assets/Resources/Art/` (không xoá `pup_*`; `bear_*` từ sheet sẽ bị
make_art ghi đè chừng nào `BEAR_ANIMS` còn trong `ASSETS` → khi có sheet, bỏ các mục `bear_*` khỏi `ASSETS` ❗).

**Thiết kế hoạt ảnh** (theo tham chiếu §0; bộ ✅ đã có sheet, 🆕 cần vẽ theo `docs/pup-art-brief-2.md` / `docs/bear-art-brief.md`, code đã nhận và dùng bộ cũ thay thế khi chưa có):

| Nhân vật | Bộ | Frame · fps | Khi nào | Trạng thái |
|---|---|---|---|---|
| Hải cẩu | idle / walk / leap / hang / flail / cheer / dizzy / ball / lever / launch / fall / glide | 4·3, 6·theo bước, 4·10, 4·5 (nhanh theo đu), 4·10, 4·8, 4·6, 1, 8·3/14, 4·10, 4·8, 4·8 | hàng chờ, đi trên cầu, bật chụp dây, đu, bị đập bay, mừng trên gờ, choáng, cuộn rơi, gạt cần, bay chéo, rơi khỏi hố, dự phòng bay | ✅ |
| Hải cẩu | **fly** 8·9 | bay ngang qua đấu trường, vây vỗ như ngư lôi | ✅ sheet E (ChatGPT 11/11) |
| Hải cẩu | **dive** 2·10 + xoay −90° trong 0.2 s rồi cuộn (ball) | chạm lần 1 | ✅ sheet E |
| Hải cẩu | **ninja** 1 (trong bong bóng `bubble_ninja`, khựng 0.25 s) | chạm lần 2 có Ninja | ✅ sheet E |
| Hải cẩu | **getup** 4·7 (nằm → chống tay → ngồi → đứng) rồi walk đi ra + mờ dần | nằm yên 0.8 s sau khi đáp | ✅ sheet E |
| Hải cẩu | lửa = ball + vệt tia lửa cam mỗi 0.025 s | Blaze | ✅ (hiệu ứng code) |
| Gấu | idle 4·3 (thở, chớp) / taunt 4·7 / dizzy 4·6 (+2 frame đầu = hit) / ko 4·5 (nằm, mắt X) | đứng gác, trêu, bị trúng, bị hạ nằm lại (frame vẽ nằm sẵn; gốc không xoay, collider nằm ngang) | ✅ `bear_sheet.png` (ChatGPT 11/11, gấu phe ác: cau có, nanh, sẹo, bụng phệ — người dùng: "gấu bên phe ác, nhìn hơi trẻ con" → vẽ lại) |
| Gấu | **eat** 4·4 (tô cá) / **sleep** 4·2 (z bay) | tâm trạng `BearMood` theo màn | ✅ `bear_sheet_b.png` |
| Vua Gấu | dùng bộ gấu ×2 cỡ; bay khỏi bệ (dizzy) → đáp (-1 mạng) → trèo lại (taunt, cung 1.4 s) → idle; hết mạng = ko | nút sàn cùng nhóm | ✅ code, art = gấu |
| Vật thể | vòng bật xoay 40°/s + giật khi trúng; TNT nổ (tia lửa + bụi + rung); nút lún khi đè; cá lắc ±12°; lồng đu ±4°, mở cửa + hải cẩu nhảy ra | | ✅ |
| Hiệu ứng chữ | "OK!" cam tại hải cẩu, "Grr…" xanh tại hải cẩu gạt cần, "+500"/"+100", "×N" nâu lớn giữa màn, "-1" đỏ ở boss | | ✅ |
| Chuyển cảnh | nước dâng 0.4 s (ease) → gấu xách lồng chạy ngang nhún trong lúc tải (≥ 0.5 s) → nước rút 0.45 s | mọi lần đổi scene | ✅ |

**Danh sách asset:**

| Nhóm | Có ✅ | Cần làm 🆕 |
|---|---|---|
| Hải cẩu | 12 bộ frame | — |
| Gấu | frame tạm (idle/taunt/dizzy/ko), mũ băng | sheet ChatGPT; Vua Gấu Băng (sheet riêng, 2× to); hải cẩu trong lồng |
| Hố bắn | tường/trần đá 9-slice, nhũ, cầu dầm, dây, bảng đếm, máy đập + cần | lớp đá có vân/nhũ phong phú hơn (hiện phẳng) 🔧 |
| Đấu trường | trời hoàng hôn, dải đồi + chân trời + biển băng, đá cuội lát, viền tuyết, dốc, khối băng/tuyết/đá | vòng băng, cá, lồng, thùng gỗ texture, nhũ rơi |
| Bản đồ | đảo vùng 1 | nút thưởng/boss, huy hiệu Hoàn hảo, đảo vùng 2–3 |
| UI | nút 3 màu, pill, panel, ribbon, icon 4 nâng cấp, glyph | icon Lửa, icon cá, huy hiệu |
| Hiệu ứng | bụi tuyết, mảnh băng, sao, vệt, tuyết rơi | lửa (trail), vòng sáng khi qua vòng |

## 12. Âm thanh

Hiện: tiếng tổng hợp từ `Tools/make_audio.py` (click, launch, boing, thud, pop, sand, dive, puff, star, win, lose, zap,
bear_down, nhạc nền `music_reef`). 🔧 Thay bằng bộ CC0 (Kenney) + nhạc nền riêng cho đấu trường (nhịp nhanh hơn) và menu;
🆕 tiếng gấu "oof", tiếng vòng băng, tiếng lửa, tiếng nhặt cá; ghi nguồn trong Cài đặt.

## 13. Kỹ thuật

Unity 6.3 LTS (6000.3.28f1), URP 2D, Physics2D, Cinemachine (lia + rung), Input System, uGUI + TMP (Baloo 2), lưu JSON.
Mọi màn hình dựng bằng code (`UiKit`), không prefab; mọi số "cảm giác" trong `Assets/Data/Tuning.asset`; màn trong
`LevelDesigns.cs` → `Level_XX.asset` (`RebuildLevels` sau khi sửa). Kiến trúc: `Core` (GameManager, Backdrop, Fx, Loc,
Save, PupRig), `Launch` (LaunchPhase, SwingRope, WhackerMachine), `Attack` (AttackPhase, PufferController, Block, Bear,
BearRig), `Progress` (data), `UI`. Test: EditMode (sao, cửa hàng, save) + PlayMode (màn đứng yên, lao phá, chơi hết một
màn bằng bot, quét nhịp đập, quay frame, chụp menu). Build: `ProjectBuilder` (APK test, AAB ký bằng biến môi trường).
Máy: M1 8 GB, ổ trống ≥ 10 GB; QA trên AVD `Pixel_QA_34` (chụp qua `adb emu screenrecord screenshot`).

## 13b. Quảng cáo (AdMob, người dùng 13/11/2026)

Lấy theo quy tắc của one-fill-line (`docs/BAO-CAO-KHAO-SAT.md` §6b ở repo đó): quảng cáo **nhẹ tay**, không bao giờ
chen giữa màn, mỗi quy tắc chặn đúng một lời chê hay gặp ở thể loại.

| Vị trí | Quy tắc | Chặn lời chê nào |
|---|---|---|
| Banner | **Không có.** Game ngang, hành động: banner che sân chơi và gây chạm nhầm | Banner che, chạm nhầm |
| Interstitial | Chỉ khi bấm **Tiếp tục** trên thẻ thắng, trước khi sang màn sau. Tối đa **1 lần mỗi 4 màn thắng**; cách mọi quảng cáo toàn màn **≥ 180 giây**; chưa có khi người chơi mới qua dưới **3 màn**; không có sau màn thưởng và màn boss (thẻ kết thúc chặng), không có khi bấm Chơi lại hay Bản đồ | "Quảng cáo sau mỗi màn", quảng cáo ngay sau phần hướng dẫn |
| Rewarded | Chỉ khi người chơi tự bấm nút có huy hiệu ▶: trên **thẻ thua**, nút "QC: +1 hải cẩu ▶" (tiếng Việt viết tắt cho vừa nút; ngôn ngữ khác ghi đủ chữ quảng cáo, vd. "Ad: +1 pup"); nút vàng chính luôn là lượt chơi lại miễn phí (R9) — chơi lại đúng màn đó với thêm 1 hải cẩu, dùng **một lần** (màn kế hay lần chơi lại sau đó trở lại bình thường) | Ép xem video; thua mãi là kẹt |
| Khi quảng cáo lỗi / không mạng | Rewarded: hiện "Chưa có quảng cáo", **không tặng gì**. Interstitial: bỏ qua, sang màn luôn | Tắt mạng để lấy quà miễn phí; game treo chờ quảng cáo |
| Âm thanh | Âm thanh game tắt (nút loa, menu tạm dừng, Cài đặt) thì quảng cáo cũng tắt tiếng (`MobileAds.SetApplicationMuted`) | Quảng cáo kêu to khi đã tắt âm |
| Đồng ý (GDPR) | Google UMP hỏi khi mở game, **trước** `MobileAds.Initialize`; không được phép thì chơi không quảng cáo. Cài đặt có dòng **Quyền riêng tư** khi UMP yêu cầu | Luật EU, chính sách Play |

Kỹ thuật: plugin Google Mobile Ads Unity (OpenUPM `com.google.ads.mobile`); quy tắc nằm ở lớp C# thuần
`AdPolicy` (test EditMode), phần gọi SDK ở `Ads`. Bản test/QA và máy ảo luôn dùng **ID quảng cáo test** của Google;
chỉ bản AAB phát hành dùng ID thật trong `AdsConfig.cs` (ID còn là chỗ trống thì vẫn dùng ID test và log lỗi, không
crash). Đếm "4 màn thắng" và "180 giây" theo phiên chơi (mở lại game thì đếm lại); "3 màn" đếm theo save (màn có ≥ 1 sao).

## 13c. Ngôn ngữ và đánh giá trong app (người dùng 13/11/2026: "làm SEO thật tốt, ưu tiên EU")

- **7 ngôn ngữ**: en, vi, de, fr, es, it, pl (cột theo thứ tự `Loc.Codes`). Lần đầu theo ngôn ngữ máy, không khớp thì
  English. Nút Ngôn ngữ trong Cài đặt mở **bảng chọn** 7 ngôn ngữ (từ 18/11, không xoay vòng nữa), mỗi nút hiện tên của
  chính ngôn ngữ đó ("Deutsch", "Polski"); ngôn ngữ đang dùng là nút vàng **có ngôi sao** (màu không là tín hiệu duy nhất). Tên nâng cấp, vùng và huy hiệu dịch **đúng bảng** trong `docs/ASO.md` (mục Châu Âu), để ảnh store
  và mô tả nói cùng một chữ. Giọng thân mật (du / tu / tú). Chữ dài hơn khung thì tự co tới 65% cỡ chữ (`UiKit.Text`),
  không tràn. Bản dịch do Claude viết, chưa có người bản xứ đọc lại.
- **Đánh giá trong app** (Google Play In-App Review, như one-fill-line `docs/ASO.md` §5): hỏi **một lần**, 1,5 giây sau khi
  thẻ thắng hiện ở lần thắng đầu tiên từ **màn 3** trở đi (15/11: trước là màn 5, tức ngay sau màn có thể hiện interstitial đầu tiên; màn 3 thì chưa có quảng cáo nào). Không bao giờ khi thua, giữa màn hay khi đang rời màn; app ở
  nền thì bỏ, lần thắng sau hỏi lại. Interstitial chỉ hiện khi rời thẻ thắng, nên thẻ thắng không bao giờ ngay sau quảng
  cáo. Cờ `reviewAsked` chỉ bật khi Play nhận yêu cầu (lỗi mạng thì lần thắng sau hỏi lại) và **giữ qua Xóa dữ liệu**.
  Play tự quyết có hiện hộp hay không và không cho game biết người chơi có đánh giá không, nên không gắn gì vào kết quả;
  không hỏi "bạn có thích không" trước (Play cấm lọc review). Hộp chỉ hiện ở bản cài từ Play (internal track trở lên);
  Editor và APK cài tay không hiện gì. Kỹ thuật: gọi thẳng thư viện `com.google.android.play:review` qua JNI (khai báo
  cho EDM ở `Assets/Scripts/Review/Editor/PlayReviewDependencies.xml`), không dùng plugin Unity của Google vì plugin đó
  kéo EDM bản `com.google.*` trùng với bản `com.unity.*` mà plugin AdMob dùng.

## 13d. Thống kê lượt chơi (Firebase Analytics, người dùng 16/11/2026: "Có, làm đi")

- Mục đích: biết người chơi rớt ở màn nào để cân độ khó (phễu màn). Chỉ 2 sự kiện: `level_start` (`level_name`,
  `attack_only`) và `level_end` (`level_name`, `success`, `stars`, `pups_out`); Firebase tự thêm phiên, máy, quốc gia.
- Project Firebase: **My First Project** (`project-351eae45-2c8e-4ca0-8de`, chung với One Line Fill vì tài khoản hết hạn
  mức project), app Android riêng của game. Không dùng Firebase Unity SDK: thư viện `firebase-analytics` qua EDM
  (`Assets/Scripts/Analytics/Editor/FirebaseDependencies.xml`), id app trong
  `Assets/Plugins/Android/FirebaseConfig.androidlib/res/values/firebase.xml` (thay cho plugin google-services), gọi qua
  JNI ở `GameAnalytics` như In-App Review.
- **Đồng ý trước**: manifest tắt thu thập từ đầu (`firebase_analytics_collection_enabled=false`), tắt Advertising ID và
  tín hiệu cá nhân hóa quảng cáo. Bật khi consent quảng cáo cho phép (`AdMob.StartSdk`), tắt khi người chơi rút đồng ý ở
  Cài đặt → Quyền riêng tư. Không có consent thì không có số liệu (chấp nhận).
- Editor và test: không gửi gì (`#if UNITY_ANDROID && !UNITY_EDITOR`).

## 14. Kế hoạch sản xuất

Làm theo vai như một đội: **Thiết kế** (tài liệu này, số liệu màn), **Nghệ thuật** (sheet ChatGPT + SVG), **Lập trình**,
**QA** (test tự động + chơi tay trên máy ảo). Mỗi mốc kết thúc bằng: test xanh → review theo khu vực → sửa → build APK → QA
theo checklist → ghi CHANGELOG → commit. Không mốc nào "xong" khi chưa chơi được trên máy ảo.

| Mốc | Nội dung | Định nghĩa xong |
|---|---|---|
| M0 · Duyệt thiết kế | bạn chốt §16 | tài liệu này cập nhật theo quyết định |
| M1 · Nền tảng ngang ✅ | hố bắn, đấu trường, gấu, 10 màn, UI ngang | review 6 khu vực sạch, QA checklist, commit đầu tiên |
| M2–M4 · Theo tham chiếu 🔧 (code xong 11/11, đang test/QA) | gờ chờ, OK!/Grr, bay vòng, lao cắm đầu + nảy + combo, camera lia, đi bộ ra, gấu ăn/ngủ/nằm lại, nhiệm vụ + Perfect, vòng bật, TNT, nút + cổng băng, Ninja, Lửa, màn thưởng cá, boss Vua Gấu Băng + lồng, bản đồ 12 nút, sóng nước, thẻ tải, bảng màu vùng, âm thanh mới | tests xanh, review, QA máy ảo |
| M2 · Cảm giác theo tham chiếu | gờ chờ pha 1, bay vòng lại, lao cắm đầu + nảy + combo, camera lia, đi bộ ra mép, Perfect = hạ hết gấu và nhặt đủ cá, sheet gấu ChatGPT, tiếng | chơi 2 màn trên máy ảo thấy giống nhịp tham chiếu; 3 người thử không cần giải thích |
| M3 · Vật thể mới | cá giấu + Hoàn hảo, vòng băng, cầu lửa, thùng gỗ, màn 5/7 cập nhật | mỗi màn có 3 cá, bot vẫn thắng |
| M4 · Thưởng + Boss | màn thưởng, Vua Gấu Băng, lồng, bản đồ có nút thưởng/boss | thắng boss cứu hải cẩu, 32 sao vùng 1 |
| M5 · Phát hành thử | AAB ký, Play internal testing, icon/ảnh store, chính sách riêng tư | cài từ Play, QA đủ checklist |
| M6+ | vùng 2, vùng 3, thành tích | — |

**Checklist QA mỗi build:** 1 màn thắng + 1 màn thua, chơi lại, tạm dừng/tiếp tục, bản đồ, cửa hàng mua được, âm thanh
bật/tắt, EN/VI, tắt app mở lại giữ tiến trình, xoay hai chiều ngang, 4:3 và 20:9 (Editor), fps ổn trên máy ảo.

## 15. Rủi ro

| Rủi ro | Xử lý |
|---|---|
| Khe thoát quá khó/dễ sau khi đổi số | test quét nhịp (`SwingTimingChangesTheTrajectory`) phải ra cả lọt lẫn không; chơi tay 10 cú |
| Lao chéo chậm làm pha 2 ít "đã" | trọng lực thật sau cú va đầu; rung + mảnh vỡ; chỉnh `diveSpeed` nếu người chơi thử thấy chậm |
| Sheet gấu ChatGPT lệch nét | Chỉnh sửa trên sheet hải cẩu, không tạo mới; so cỡ bằng chiều cao hàng idle |
| make_art ghi đè frame gấu từ sheet | bỏ `bear_*` khỏi `ASSETS` ngay khi slice xong (ghi trong §11) |
| Máy ảo đầy | QA trên `Pixel_QA_34`; không gỡ app dự án khác |
| Phạm vi phình | M1–M5 cố định; mọi ý mới ghi vào §16 chờ duyệt |

## 16. Quyết định đã chốt (người dùng 11/11/2026: "làm hết tất cả để hoàn thiện", theo đề xuất)

1. **Lao:** theo tham chiếu — cắm đầu, rơi nhanh dần, giữ 30% đà ngang, nảy khi trúng gấu, combo ×N. Số trong `Tuning.asset`.
1b. **Đổi hướng trên không:** chạm lần 2 (Ninja): hải cẩu khựng 0.25 s trong bóng thoại rồi lao thẳng xuống. Thay cho vuốt.
2. **Cầu lửa:** kỹ năng mua bằng sao (cấp 1/2/3 = xuyên 2/4/∞ khối băng tới cú va cứng).
3. **Hoàn hảo:** huy hiệu trên bản đồ, không thêm sao. Tổng sao vùng 1 = 36 (12 màn × 3).
4. **Boss:** cuối vùng 1 (màn 12), màn thưởng là màn 11.
5. **Tiền tệ:** chỉ sao.
6. **Tên:** giữ "Seal Catapult / Hải Cẩu Bay".
7. Chế độ chiến thuật: không làm. Vùng 2–3: bảng màu (nền + đá) đã có (`LevelData.zone` 0–2, tint trong `AttackPhase.Palettes`); màn mẫu của vùng 2–3 để sau khi vùng 1 qua QA.

### Màn hình chính: trận ném tuyết (18/11)
Người dùng: "nhìn quá xấu, quá đơn điệu … vẽ nền kiểu như 2 bên đang chiến đấu với nhau, tham khảo top game". Nền `menu_bay.png`
(ChatGPT, nét viền đậm như pit) là trại hải cẩu bên trái (tháp gỗ, cờ, đống bóng tuyết) đối mặt pháo đài băng của gấu bên phải,
bóng tuyết bay giữa trời; cờ trại hải cẩu mang dấu vây hải cẩu, không phải dấu chân gấu (người dùng 18/11); dải tuyết dưới cùng 8,4% chiều cao là chỗ nhân vật đứng (`MainMenuView.MenuSnowLine`, gắn theo tranh).
`MenuBattle.cs`: 2 hải cẩu (nhảy, vẫy) và 2 gấu (khiêu khích, khoe cơ) ném qua lại một quả bóng tuyết. Mọi động tác ném
và trúng bóng đều được vẽ riêng (người dùng 18/11: "vẽ thêm các hoạt ảnh cho khớp hành động, đừng tận dụng những hoạt ảnh
không khớp"): `pup_sheet_throw.png` và `bear_sheet_f.png`, mỗi tờ 8 khung ném (xúc tuyết, nặn, giơ, vung, ném, khoe) + 8 khung
trúng bóng (tuyết dính mặt, lùi, lắc đầu, lau mặt). Bóng rời tay ở khung 6 (`PupRelease`, `BearRelease`); quả bóng bay do code
vẽ nên hai tờ nằm trong `FX_FREE` (bỏ mảnh rời). Gấu ở màn hình chính không đội mũ băng. Nhân vật không đứng sau nút Play.

### Phong cách nút: pha lê băng (18/11)
Người dùng chọn kiểu 2 "Ice crystal" trong 6 mẫu (`button_styles` ChatGPT): nút cắt cạnh như khối băng màu (xanh lá = chính,
cam = phụ ấm, xanh dương = thường), tuyết phủ trên đỉnh, nhũ băng rủ dưới; nút tròn và bảng cùng kiểu băng. Tranh nguồn
`Assets/Art/Reference/ui_kit.png` (cắt bằng `Tools/cut_props.py`, giữ đúng bố cục cũ) và `ui_icons.png` (12 icon trên nút:
trắng-xanh băng, viền xanh đậm; thay các icon SVG phẳng của make_art). Chữ trong bảng băng lùi vào trong viền dày.
Nhấn nút (`ButtonJuice`): lún còn 86 % và tối đi khi giữ, nhả ra nảy vượt cỡ rồi về (lò xo, thời gian thực); hành động
chạy ngay (màn hình chuyển có fade nên vẫn thấy cú nảy).

### Bảng (modal) thiết kế lại (18/11)
Bản thiết kế duyệt với người dùng: `button_styles`/`modals_a2`/`modals_b` (ChatGPT, kiểu pha lê băng). Trang Nâng cấp
(shop) là trang riêng, chưa làm đợt này. Mọi nhân vật trong bảng là hoạt ảnh vẽ riêng cho bảng đó, 16 khung, hài hước
kiểu tình huống chứ không trẻ con (người dùng: "hướng tới hài hước 1 chút, đừng trẻ con quá"):
- Tạm dừng: bảng băng, ruy băng "Paused", lâu đài · màn, Resume (xanh lá) / Retry / Map xếp dọc, 2 nút tròn nhạc/âm
  thanh; hải cẩu nhâm nhi cacao rồi xem đồng hồ (`pup_modal_wait`) đứng sát mép trái bảng.
- Kết quả thắng/thua: đúng cấu trúc bảng kết quả của CPC, xem từng khung trong video `1Dp_WM-9ag0` (các bảng ở giây
  7, 31, 61, 86, 345…; người dùng: "hoạt ảnh phải nằm trong modal như cpc", "hải cẩu lại to bằng gấu là thế nào", "xem lại
  thật kĩ các modal cpc để thiết kế lại"). Bảng gần vuông, thoáng, ruy băng mang dòng tiêu đề, "Mission N" trong bảng ở trên cùng,
  dòng phụ to ngay dưới ruy băng tiêu đề (người dùng: "vẫn thêm ruy băng"), cảnh nhỏ ở giữa trên đốm tuyết bầu dục (thắng: hải cẩu kính râm đứng trên bụng
  gấu nằm choáng `bear_modal_ko`; Perfect thêm 2 hải cẩu hai bên + sao lấp lánh quanh dòng vàng; thua: gấu chỉ tay cười
  `bear_modal_mock` cạnh hải cẩu u đầu `pup_modal_lose`; gấu luôn to hơn hải cẩu), dòng số liệu căn giữa với con số to
  màu xanh, rồi phần riêng của game này: 3 sao kèm điều kiện. MỘT nút chính ở góc dưới phải; Map/Retry là nút tròn nhỏ
  góc dưới trái; nút quảng cáo +1 hải cẩu cạnh nút chính khi thua.
- Cài đặt: icon cho từng dòng (nhạc, loa, quả địa cầu), Stats/Erase có icon; hải cẩu air guitar (`pup_modal_music`).
  Ngôn ngữ mở bảng chọn 7 ngôn ngữ (tên gốc), không xoay vòng khi bấm nữa.
- Thống kê: các dòng trên dải băng có icon (mặt gấu, dấu chân 1–3, sao); hải cẩu lau cúp (`pup_modal_trophy`).
- Hướng dẫn: 4 thẻ bước có tranh (`step_1..4`), hải cẩu đọc sách ngược (`pup_modal_read`).
Mảnh ghép: `Assets/Art/Reference/ui_modal_kit.png` (cut_props). Hoạt ảnh: `pup_sheet_16_modal_*.png`,
`bear_sheet_16_modal_*.png` (slice_sheets `SMOOTH_SIZE`; mọi tờ 16 khung của bất kỳ nhân vật nào khai báo ở đó).
Code dùng chung: `UiKit.Cast` (nhân vật đứng theo chân), `UiKit.ListRow`, `Rotate` (tia sáng sau sao).

### Thương hiệu, màn hình loading, chuyển cảnh (18/11)
- Tên game là thương hiệu: "Seal Catapult" ở MỌI ngôn ngữ (người dùng: "tên game ko theo ngôn ngữ"); `Loc` `title` giống
  nhau cả 7 cột. Tên trên Google Play theo ngôn ngữ ở `docs/ASO.md` là chuyện riêng của trang cửa hàng.
- Logo vẽ (`Assets/Art/Reference/logo.png`, cắt bằng cut_props → `logo`): "Seal" chữ băng phủ tuyết, "CATAPULT" chữ gỗ
  buộc dây thừng, hải cẩu bay ra từ gáo máy bắn kèm bóng tuyết, viền xanh đậm + viền trắng. Dùng qua `UiKit.Brand` ở màn
  hình chính, màn loading và trang About; thiếu ảnh thì mới rơi về chữ vàng.
- Màn loading lúc vào game (`BootView`): tranh nền trang chính phủ lớp tối, logo, chữ "Loading…" (khoá `loading`, 7 ngôn
  ngữ) và thanh tiến độ băng chạy đầy trong 1,8 giây.
- Chuyển cảnh (`ScreenFader`): nước dâng phủ màn hình như cũ; ở giữa là hải cẩu chạy trên quả bóng tuyết lăn
  (`pup_sheet_16_loader.png`, 16 khung) trên chữ "Loading…", thay cho gấu xách lồng chạy ngang.
- Mọi bảng giữ ruy băng tiêu đề (người dùng: "vẫn thêm ruy băng").
- Nút X đỏ (`icon_close`, tranh `ui_close.png`) ở góc trên phải mọi bảng (người dùng: "các modal ko có dấu x để tắt ak"):
  Cài đặt, chọn ngôn ngữ, xác nhận xoá (= Không), Thống kê, Hướng dẫn/About, Tạm dừng (= Resume). Không có trên bảng Kết
  quả và thẻ hướng dẫn lần đầu (bắt buộc chọn nút). `UiKit.CloseButton`.
- Gấu gác có thêm 3 trò 16 khung, vẽ riêng (`bear_sheet_16_flexfail/doze/backfire.png`; người dùng: "thêm nhiều hoạt ảnh
  cho gấu"): khoe cơ nín thở rồi xì bụng, đứng gác ngủ gật giật mình, tung bóng tuyết rơi trúng đầu mình. Chúng nằm trong
  `Bear.Antics`: chỉ gấu đứng thường (signature Idle) mới xen ngẫu nhiên, gấu có động tác riêng thì không.
- 16 khung cho mọi hoạt ảnh (người dùng 18/11: "tăng khung hình cho tất cả hoạt ảnh"). Cách làm: ghép các khung đang có
  thành một dải, nhờ ChatGPT vẽ khung chen giữa (bản gốc ở khung 1/5/9/13), lưu `<kind>_sheet_16_<anim>.png`; slice_sheets
  (`SMOOTH_MATCH`) đo chiều cao các khung cũ để bản mới giữ đúng cỡ (gấu nằm/ngồi không bị phóng to). Code không cần biết
  số khung: `BearRig.Load` và `FrameLoop` tự đếm khung và giữ nguyên thời lượng. Xong cả 15 hoạt ảnh gấu (19/11); hải
  cẩu còn lại làm tiếp. Bài học: một tờ có viền đầu đứt làm việc xoá nền trắng ăn mất đầu (bản "cười" đầu tiên): xem mọi
  khung trên nền màu sau khi cắt, lỗi thì nhờ vẽ lại với "bold fully closed outline".
- Bản đồ (19/11, người dùng: "cho lồng và gấu to bằng hải cẩu di chuyển"): lồng, hải cẩu trong lồng, gấu trùm và gấu đứng
  bên đường cùng cỡ hải cẩu đi trên đường (Stone 64 × 1,9 ≈ 122 px), đứng trên ô đá; ô trùm lớn hơn không làm gấu/lồng to hơn.
- Lồng trên bản đồ làm lại theo CPC (19/11, người dùng: "xem lại hoạt họa trong lồng của cpc và làm lại đi, nhìn quá tệ";
  video CPC giây 64–66): lồng chim cao thanh mảnh, nan mảnh, mái vòm có khoen, trên đế tròn đỏ (`cage_cpc.png` →
  `cage`/`cage_open`, lòng lồng trong suốt). Hải cẩu nằm gọn TRONG lồng, vẽ trước rồi lồng đè lên (nhìn rõ qua nan), hoảng
  hốt kêu cứu, 16 khung (`pup_sheet_16_captive.png`), không khóc. Chân đặt trong lòng đế (`CaptiveFloor` 0,22).
- Mọi thứ trên ô (hải cẩu đi, lồng, gấu trùm) đứng với chân ở GIỮA ô đá (người dùng: "mọi thứ chưa đứng đúng giữa của ô").
- Nhân vật trên bản đồ nổi khỏi tranh (19/11, người dùng: "chưa làm nổi bật hoạt ảnh vì nó là chính, đang bị hòa vào map"):
  `UiKit.StandOut` = viền xanh đậm dày quanh nhân vật (UI Outline) + bóng đổ + quầng sáng ấm mờ phía sau + bóng tối dưới
  chân; dùng cho hải cẩu đi, gấu trùm, gấu bên đường; lồng chỉ có quầng + bóng chân (viền làm nan lồng dày đen che hải
  cẩu), hải cẩu trong lồng có viền.
- Hải cẩu hết lượt ở pha 2 làm như CPC (19/11, người dùng: "khi hải cẩu hết nhiệm vụ thì nó nảy lên và rơi xuống như cpc";
  video CPC giây 49,3–50,5 ở 20 fps): khói bụp, hải cẩu quay mặt ra gãi đầu cười ngượng (`pup_sheet_16_exit.png`, 16 khung),
  đứng 0,4 s, nảy lên rồi rơi thẳng xuyên đất ra khỏi màn hình, vẽ đè lên đá. Thay cho ngồi choáng rồi mờ dần.
- Dọn hình không dùng (19/11): `bg_dusk`, `bg_floes`, `fx_rays`, `mark_tick`, `icon_bulb`, `pup_help_*` (thay bằng
  `pup_captive`); `pup_sheet_help.png` giữ lại làm mẫu phong cách để vẽ tờ hải cẩu mới.
- 16 khung, tình trạng 19/11: xong 17 hoạt ảnh gấu và 28 hoạt ảnh hải cẩu (kể cả các bản không mũ: vùng vẫy, choáng,
  đứng dậy, bị kẹt). Còn dưới 16 khung: `op_pull` (kéo cần: ChatGPT vẽ sai trình tự 2 lần, giữ 8 khung), và các bộ dùng
  theo lát/khung lẻ hoặc gần như không hiện: `lever`, `reach`, `grab`, `leap`, `ready`, `hang`, `hang_fast`, `idle`,
  `idle_bare`, `ball`, `ball_bare`, `cheer_bare`, `walk_bare`, `dive`, `ninja`.
- CỠ của bản vẽ lại: khung 1 của bản mới = khung 0 của bản cũ vẽ lại, nên slice_sheets ép khung đầu cao đúng bằng khung 0
  bản đã commit (`("first", px)`, bảng `SMOOTH_FIRST` cho hải cẩu và `SMOOTH_SIZE` cho gấu). KHÔNG đo theo khung đang có
  trên đĩa (trôi dần sau mỗi lần cắt: gấu đứng thở −32 %, khoe cơ +26 % → gấu phình ra ở trang chính, người dùng báo
  19/11) và không đo theo mũ/chiều cao trung vị (lệch 5–20 %).
- Code chỉ tự kéo dài một bộ khi nó đủ 16 khung trở lên (`PupRig.Load`, `BearRig.Load`, `FrameLoop`): bộ dùng theo lát
  (lever 0–3 / 4–7, leap_0 làm dáng ngồi) giữ nguyên.
- VĂNG ở pha 2, làm lại theo CPC (19/11, người dùng: "xem thật kĩ là văng và vật lí của pha cpc và vẽ lại hoạt ảnh
  văng"). Xem video CPC 60 khung/giây, khung hình bám theo chim cánh cụt (2 cú văng, 44–48 s và 37–41 s). Kết luận này
  THAY cho ghi chú 17/11 ("một hình vẽ, cả thân xoay"), ghi chú đó sai:
  - Chim lộn nhào bằng nhiều DÁNG VẼ khác góc nhìn: lưng quay ra co tròn chân thò ra → nghiêng 3/4 → nghiêng thấy
    mặt và bụng → chúi đầu → lại lưng. Một vòng khoảng 0,65 s, lộn theo hướng bay.
  - Một cụm bụi xám nhỏ bám theo suốt đường bay; mỗi lần đập tường/sàn có cụm khói trắng to + vòng vàng; nảy lại giữ
    phần lớn tốc độ.
  - Trong game: `pup_sheet_16_flung.png` (16 khung, một vòng lộn nhào, vẽ đang bay sang phải, không mũ), chạy 0,65 s/vòng;
    thân KHÔNG xoay bằng code nữa (`PufferController.Spin` khoá xoay, chỉ lật mặt theo hướng bay); vệt `Fx.Smoke` xám
    suốt lúc văng. Dáng lăn trên đất (`Roll`) và quả cầu gai (nâng cấp) giữ nguyên.
- 16 khung, đợt 19/11 (tiếp): thêm `hang`, `hang_fast` (đu dây), `cheer_bare`, `walk_bare`. Còn dưới 16: `op_pull` (8),
  `lever`, `reach`, `grab`, `leap`, `ready`, `idle`/`idle_bare` (đứng yên + thở bằng code), `ball`/`ball_bare` (một hình
  xoay), `dive`, `ninja`.
  - Đu dây 16 khung vẽ đủ một nhịp lắc (ra, về, sang bên kia, về) nên `PupRig.Swing` chạy thẳng vòng, không ping-pong,
    giữ nhịp cũ (fps × 4/6).
  - Bước đi tính theo quãng đường cho CẢ vòng (`WalkCycle = 0.88`), không theo từng khung: thêm khung không làm chân
    bước chậm lại (trước đó `WalkStep` mỗi khung khiến bộ 16 khung bước chậm gấp đôi).
  - Hải cẩu không mũ: mõm hở viền chỗ ria nên nền trắng tràn vào xoá cả đầu, xin "viền kín" 2 lần vẫn hở. Sửa trong
    công cụ: `SEAL_GAPS` ở `Tools/slice_sheets.py` chỉ cho nền đi qua khe rộng hơn 13 px. Tờ không mũ nào mất đầu thì
    thêm tên tờ vào đó, không cần vẽ lại.
- 16 khung, đợt 20/11: thêm `op_pull` (kéo cần máy bắn), `dive` (bổ nhào), `ready` (ngồi xuống ở mép cầu), `ninja`
  (lơ lửng khoanh tay). Còn dưới 16: `idle`/`idle_bare` (đứng yên + thở bằng code), `ball`/`ball_bare` (một hình xoay).
  - Đã XOÁ các dáng không còn chỗ nào gọi (20/11): `Pose.Grab`, `Pose.Leap`, `Pose.LeverReady/LeverPull` và hình
    `pup_reach_*`, `pup_grab_*`, `pup_leap_*`, `pup_lever_*`. Các hàng đó vẫn nằm trên tờ gốc và slice_sheets vẫn ĐO
    chúng (tờ D và K lấy cỡ theo hàng `lever`, `reach`) nhưng không ghi ra: bảng `UNUSED`. Trước khi vẽ lại một bộ,
    grep xem `Pose` của nó có chỗ nào `SetPose` không (đã vẽ `reach` 16 khung rồi mới phát hiện không dùng, phải bỏ).
  - Rà toàn bộ (20/11): 15 bộ hải cẩu 16 khung không bao giờ hiện (21,7 MB). Người dùng chọn: NỐI LẠI 4 kiểu văng cũ,
    XOÁ 11 bộ còn lại.
    - Văng: xem mục "VĂNG làm lại lần 3 (20/11)" ngay dưới; 4 kiểu cũ (`tumble`, `fling_cartwheel`, `fling_backflip`,
      `fling_corkscrew`, mắt xoáy, dang tay chân) đã bỏ cùng ngày vì người dùng thấy chưa giống CPC.
    - Đã xoá hình + dáng: `fall`, `glide`, `getup`, `getup_bare`, `flinch`, `flat`, `stuck_wall`, `hang_fast`,
      `walk_bare`, `cheer_bare` (và `Pose.Fall/Glide/GetUp/Flinch/Tumble/Flat/StuckSlope`). `hang` về 4 khung cũ: bộ
      `swing` thay nó lúc đu dây, chỉ `pup_hang_0` còn hiện trên màn chuyển cảnh (`HudView`).
    - Bài học: "đã xem trong game" chỉ tính khi đã lần từ tên bộ hình tới dáng đang hiện. Hôm 19/11 báo đã xem `hang`
      16 khung, thực ra đang xem `swing`.
  - VĂNG làm lại lần 3 (20/11, người dùng: "văng chưa giống với game cpc, quan sát thật kĩ các kiểu văng của cpc và
    làm lại", rồi "sáng tạo nhiều kiểu văng thì càng tốt"). Xem lại video CPC bằng công cụ bám nhân vật (ô 110 px,
    10–30 khung/giây) ở 3 màn (36–48 s, 93–105 s, 201–203 s):
    - CPC có HAI kiểu văng, đổi qua lại sau MỖI lần va chạm: (A) co tròn như quả bóng, chân thò ra, lộn qua các góc
      nhìn vẽ sẵn (lưng → nghiêng 3/4 → mặt và bụng → chúi đầu); (B) thân duỗi thẳng nhìn nghiêng, chân đá ra, cả hình
      quay tròn như chong chóng (ghi chú 17/11 "một hình, cả thân xoay" đúng cho kiểu B, ghi chú 19/11 đúng cho kiểu A).
    - Mắt mở to, miệng há ngạc nhiên, KHÔNG phải mắt xoáy. Cụm khói xám 2–3 đốm nằm ngay TRÊN thân suốt đường bay, không
      phải chuỗi chấm phía sau. Đập tường: mây trắng to + bóng trắng + vòng vàng; trúng gấu: sao vàng.
    - Hết đà: đáp xuống (hay đáp lên bụng gấu vừa hạ), quay mặt ra đưa tay lên đầu, có vòng sáng trắng + vòng vàng, giữ
      khoảng 1 s rồi tụt xuống khỏi hình (đã có: `Pose.Exit`).
    - Tốc độ và độ nảy tính theo bề rộng màn hình gần bằng game mình (khoảng 0,25 màn hình/giây). Khác biệt lớn còn lại
      là CỠ nhân vật: chim CPC cao khoảng 9 % chiều cao màn hình, hải cẩu ở pha 2 của mình khoảng 4–5 % (`ArenaScale`
      0,75 + màn hình rộng). Chưa đổi: đó là thiết kế màn chơi, chờ người dùng quyết.
    - Trong game: `flungSets` 6 bộ 16 khung, MỌI va chạm lúc đang văng (đá, băng, gấu, TNT) gọi `rig.NextStyle()` ở đầu
      `PufferController.OnCollisionEnter2D`: chọn NGẪU NHIÊN một bộ KHÁC bộ đang chạy (người dùng 20/11: "1 con có thể
      có nhiều kiểu văng khi va chạm", "sau khi va chạm thì sẽ random kiểu văng"), giữ tối thiểu 0,12 s (`StyleHold`): `flung` (A, co tròn, khung 1 cao 215 px), `fling_spin` (B, chong chóng nhìn nghiêng),
      và 4 kiểu riêng của game: `fling_star` (xoay bánh xe nhìn thẳng), `fling_paddle` (bơi chó hoảng loạn, không xoay),
      `fling_drill` (lăn quanh trục dọc như mũi khoan), `fling_top` (đứng thẳng xoay như con quay). Khói:
      `Fx.BodySmoke` vẽ đè lên thân (order thân + 1), sống 0,16 s.
    - CỠ bằng nhau ở mọi hoạt ảnh (người dùng 20/11: "kích cỡ hải cẩu phải bằng nhau ở mọi hoạt ảnh"): dáng co, duỗi,
      xoay không đo bằng chiều cao được, nên các bộ không mũ lúc bay/lăn/trượt (6 kiểu văng, `roll`, `slide`) đo bằng
      DIỆN TÍCH LÔNG TRẮNG: quy tắc `("fur", 138)` trong slice_sheets (căn bậc hai số điểm ảnh trắng, 138 = `idle_bare`,
      `dizzy_bare`, `exit`). Trước đó lệch tới +21 % (`fling_star`), +18 % (`roll`, `slide`). Bộ không mũ mới: dùng `fur`.
    - Pha 2 theo tỉ lệ CPC (người dùng 20/11: "làm giống cpc đi nhưng phải thấy hết như cpc"). CPC: chim cao khoảng
      một khối băng, 9 % chiều cao màn hình, và một màn hình chứa cả sàn lẫn đường bay vào. Ba thay đổi, thay cho các
      quyết định 12–13/11 (hải cẩu nhỏ hơn, bay cao hơn, nhìn xa hơn):
      - `PufferController.ArenaScale` 0,75 → 1,0 (hình và vùng va chạm cùng to lên; khối lượng không đổi).
      - Đường bay vào hạ từ 8,8–10,4 xuống 7,6–8,8 (`EntryLow/High`, `SpawnPoint` 8,2): ngay trên pháo đài cao nhất
        (đầu gấu tới khoảng 7).
      - `AttackZoom` 1,3 → 1,05, tâm khung nhìn 5,3 → 4,7: sàn, pháo đài và đường bay nằm trọn một màn hình, hải cẩu
        không chui sau thanh HUD. Đã thử 1,1 với đường bay cũ: sàn và gấu bị cắt lúc đang bay.
      Hải cẩu to gấp khoảng 1,6 lần trên màn hình; gấu và băng to hơn 24 %.
    - Công cụ bám nhân vật: `Tools/cpc_track.py <giây bắt đầu> <độ dài> <fps> <out.png> [ô] [cột] [video]` (đốm tối đang
      chuyển động = chim cánh cụt). Luôn dùng nó, đừng nhìn khung toàn cảnh.
  - `ready`: khung 1 là dáng đứng (cao bằng `idle_2`, 283 px), khung 16 là dáng ngồi cũ; chạy một lần trong 0,25 s chờ ở
    mép cầu (`EdgeCrouch`), rồi giữ. `ninja`: 16 khung lặp 0,8 s, cùng một dáng, chỉ khăn/băng đầu bay và chớp mắt.
  - Bộ chạy MỘT LẦN rồi giữ khung cuối (`op_pull`, `ready`): khung 16 phải là dáng đứng yên đẹp. Lần vẽ đầu `op_pull`
    kết thúc bằng mặt nhăn, phải xin lại "khung 16 gần giống khung 1".
  - `dive`: khung 0–9 chúi mũi một lần (0,25 s), 10–15 lặp dáng ngư lôi giữ góc -70° (`DiveArtAngle`), chỉ khăn và quả
    bông bay. Không lặp 8–9 vì vẽ chúi hơn một chút, lặp sẽ giật. Lần vẽ đầu tự quay về nằm ngang (sai), lần hai mọc
    hai quả bông trên mũ: luôn soi kỹ từng khung.

### Rà soát 20/11: luật bố cục và UI rút ra (người dùng: "xem lại toàn bộ app … fix hết")

Sáu reviewer đọc toàn bộ code; các luật dưới đây là kết quả, mọi màn mới phải theo.

- **Tranh phủ màn hình thì phải kéo được.** Bản đồ fit kiểu "phủ kín" nên 4:3 cắt 12,5 % mỗi bên, 20:9 cắt 10 % trên
  dưới, đá vẽ ở đó mất hẳn. Giờ tranh bản đồ nằm trong `ScrollRect`: kéo ngang (4:3) hoặc dọc (màn dài) phần bị cắt, mở
  ra ở viên đá đang chơi. `python3 Tools/map_reach.py` phải in PASS sau mỗi lần đổi đường đi, số đá hoặc vị trí nút trên
  bản đồ (kiểm 5 tỉ lệ × 10 thế giới × 41 đá, có và không có tai thỏ 100 px ở màn dài hơn 16:9: không viên nào khuất
  hoặc nằm dưới nút; đá lồng và đá trùm phải lộ cả chỗ lồng/gấu đứng).
- **Bốn góc bản đồ, mỗi góc một điều khiển**: Back trên-trái, mũi tên "vùng sau" trên-phải, mũi tên "vùng trước"
  dưới-trái (cách đáy 114), Nâng cấp dưới-phải. Đây là hai chỗ duy nhất một nút 110 không che đá, lồng hay trùm của cả
  10 thế giới ở mọi tỉ lệ (tìm bằng `Tools/map_reach.py`; giữa mép trái/phải che đá của 5 thế giới ở 16:9, dưới nút
  Back thì trông như hai nút Back). **Số sao của thành** là một dòng nhỏ dưới tên vùng (ngôi sao + "n/123"), không còn
  là ô sao góc trên-phải: ô 260 px đó che hai con trùm và một viên đá; tên vùng và số sao không bắt chạm.
- **Đá bản đồ đứng sát nhau hơn vùng chạm 130**: cú chạm thuộc về viên đá **gần tâm nhất** (`NearestStone`), không phải
  viên vẽ sau. Đá chưa mở vẫn đáp lại (nhún + tiếng "thud"), không nuốt cú chạm.
- **Vùng chạm không chồng nhau.** Hai điều khiển cạnh nhau cách ≥ 130 theo chiều chúng xếp (Cài đặt: các hàng cách 130;
  Tạm dừng: hai nút loa/nhạc bắt đầu dưới nút Bản đồ).
- **Chữ chỉ để đọc không bắt chạm** (`raycastTarget = false`): thanh gợi ý của HUD từng nuốt cú chạm bắn/lao, ô sao
  trên bản đồ từng chắn đá bên dưới.
- **Bảng nào cũng vừa mọi tỉ lệ**: bảng cao hơn 900 hoặc có nhân vật đứng tràn mép phải nằm trong holder co theo
  `UiKit.CanvasSize()` (chiều cao, và chiều rộng nếu nhân vật đứng ngoài mép). Áp dụng: Kết quả, Cài đặt, Cách chơi.
- **Bảng nào cũng có ruy băng tiêu đề**, kể cả hộp xác nhận xoá và thẻ nhiệm vụ.
- **Màu dùng chung** lấy từ `UiKit`: `Accent` (số trong bảng: xanh), `Muted` (chữ phụ), `Danger` (cảnh báo). Không viết
  màu thẳng trong màn hình cho ba vai trò này.
- **Chữ ≥ 24 đơn vị (~9 sp)** kể cả khi tự co; chữ đọc lâu (luật sao, các bước Cách chơi, credits) ≥ 30.
- **Màu không là tín hiệu duy nhất**: cấp nâng cấp chưa mua là viên ngọc **nhỏ và mờ**; ngôn ngữ đang chọn có sao.
- **Shop nói con số thật.** Dòng mô tả: "Tên · Cấp n/3: tác dụng", là cấp **sắp mua** (đã tối đa thì cấp cao nhất), số
  lấy từ `Shop.Effect` (Gai băng = xuyên gốc + cấp; Lửa = 2, 4, rồi "mọi khối băng"; còn lại = cấp). Số 1 dùng câu số ít
  (`upd_<id>_one`). Hết cấp thì chỗ nút mua là nhãn "Tối đa", không phải nút.
- **Màn trùm**: thẻ nhiệm vụ ghi "Hạ gấu trùm để đi tiếp" (thắng chỉ cần hạ trùm), không ghi số gấu.
- **Lửa không làm giảm số lần xuyên**: bật Lửa lấy số lớn hơn giữa số còn lại và số của Lửa.
- **Lưu lỗi thì phải nói.** Ghi save thất bại hoặc file save hỏng: một dải thông báo trên cùng (`SaveNotice`), game thử
  ghi lại khi app ra nền. File `.tmp` đọc được là bản mới nhất (lần lưu bị cắt trước bước đổi tên).
- **Màn đứng yên chạy 30 fps**: bản đồ, bảng Tạm dừng, thẻ kết quả.
- **Màn đầy đủ nào cũng là tranh**: Shop dùng `UiKit.PaintedBackdrop` (tranh màn hình chính dưới lớp phủ tối 0,82; màn
  loading dùng cùng hàm với 0,45), không còn nền navy phẳng. Trang Giới thiệu dùng dàn nhân vật động của các bảng
  (`pup_modal_win`, `bear_modal_mock`) thay ba sprite đứng yên.
- Nút Truyện có icon riêng: cuốn sách mở `glyph_book` (ChatGPT vẽ theo kiểu `ui_icons.png`, sheet
  `Assets/Art/Reference/ui_icon_book.png`, cắt bằng `Tools/cut_props.py`); ▶ chỉ còn là Chơi.
- Còn treo: không làm SpriteAtlas: sprite nằm trong `Resources/` nên đóng atlas sẽ đưa cả atlas
  lẫn texture gốc vào APK (gấp đôi dung lượng phần đó); muốn làm phải chuyển sprite ra khỏi `Resources` trước. Quảng cáo:
  Play khai 13+ nên không bật cờ "hướng tới trẻ em"; nếu đổi đối tượng xuống dưới 13 thì phải bật trong `AdMob.StartSdk`.

## Shop + skin (23/11)
Người dùng: "thiết kế lại trang shop cho hợp phong cách app… 1 trang riêng ko phải modal… thêm thật nhiều skin và thể loại
skin". Bản mẫu duyệt: số 1 trong `Assets/Art/Reference/shop_mockups.png` (ChatGPT, 4 bố cục); bản code đầu bị chê "chưa giống
với ảnh" nên làm lại sát mẫu: nền sáng, bệ băng, khung lưới, cột tab một hàng.
- **Bố cục** (`UI/ShopView`): nền là tranh vịnh băng ban ngày `bg_shop` (`shop_bay.png`, `Tools/prep_backgrounds.py`), phủ
  0,12. Thanh trên Layout A: Back · ruy băng băng "Cửa hàng" ở giữa · ví sao. Trái: **cột tab một hàng** trong khung
  (8 dòng băng, bước 130 = `UiKit.MinHit`; 8 dòng cần 1040 px > mọi màn nên cột **cuộn**, nhớ vị trí cuộn). Giữa: **khung băng
  có tuyết** rộng 850, tâm x = −40 (vừa khe ở 4:3 = 1663), tiêu đề = icon + tên tab nằm dưới lớp tuyết (header 96), rồi
  **lưới 3×3 thẻ** (bước 262×222; ở 20:9 thân cao 792 ≥ 96 + 666 + 24). Phải: **không khung**, vật đang chọn đứng trên **bệ băng**
  `shop_podium` giữa vịnh, tên + một dòng chữ, **một nút chính** màu vàng cam. Không cuộn lưới: mỗi loại đúng 9 thẻ (test `SkinTests`).
- **Tín hiệu không chỉ bằng màu**: tab đang mở = dòng vàng cam + to hơn; thẻ đang chọn = viền vàng dày (hình riêng nằm sau thẻ:
  `Outline` của UI chỉ vẽ lại sprite viền tối) + to hơn; đang dùng = dải vàng cam + tick + chữ; đã có = tick + chữ; chưa đủ
  sao = ổ khóa + chữ mờ, vẫn bấm được và phần xem trước ghi "Cần thêm N sao".
- **Tab 1 = Nâng cấp** (luật cũ giữ nguyên: dòng mô tả "Cấp n/3: tác dụng" từ `Shop.Effect`).
- **7 loại skin × 9 = 63 skin** (`Progress/Skins.cs`, mỗi loại có 1 bản "Cổ điển" miễn phí), **chỉ đổi hình, không đổi lối chơi**:
  Màu lông · Trang phục · Bóng (quả bóng trên cần máy phóng) · Vệt bay · Va chạm · Máy phóng · Dây đu.
  Shader `Resources/Shaders/Recolor` nhân màu lông vào điểm ảnh nhạt-ít bão hòa (lông trắng) và xoay sắc độ điểm ảnh đỏ/xanh
  ngọc sáng-bão hòa (mũ, khăn), giữ viền navy, má hồng, kính vàng; đo trên giá trị gamma (dự án render Linear). Mọi hình hải
  cẩu (PupRig và ảnh UI tên `pup*`, `hud_pup` qua `UiKit.Image`) đều mặc skin. Shader không hỗ trợ mask của UI: hình hải cẩu
  trong vùng cuộn phải bỏ material.
- **Mua bằng sao** (người dùng 23/11: "bỏ cá đi, mua bằng sao"; bản đầu có tiền "cá" riêng, đã bỏ). Cùng ví với nâng cấp
  (`spentStars`). Giá 1–5 sao, cả 62 skin trả tiền = 165 sao (+32 sao nâng cấp); sao kiếm vô hạn ở các lâu đài sinh tự động.
  Xóa dữ liệu lưu xóa cả skin (câu xác nhận có ghi).
- **Save**: `skins` (id đã mua), `worn` (id đang dùng theo `SkinKind`, "" = cổ điển). `Skins.Normalize` bỏ id lạ/trùng
  và không cho mặc thứ chưa sở hữu. `SkinKind` lưu theo chỉ số: chỉ thêm vào cuối.
- **Skin chỉ hiện trong màn chơi** (pha 1 và pha 2; người dùng 23/11: "skin chỉ thay ở pha 1 và pha 2 thôi"):
  `Skins.InPlay` bật khi `GameManager` bắt đầu màn, tắt khi rời màn; menu, bản đồ, thẻ kết quả giữ hải cẩu cổ điển. Shop
  vẫn cho xem thử từ save.
- **Trang phục vẽ tay** (tab Trang phục): **Hải tặc** và **Ninja** (băng đầu xanh đen có tua + tấm bạc, áo võ xanh đen,
  đai đỏ; 25/11, cả 43 tờ đạt lần đầu nhờ gửi kèm ảnh mẫu bộ đồ + câu "This is a DIFFERENT sheet… redraw THESE poses
  only" cho các tờ giống nhau). Bộ đầu là **Hải tặc** (mũ ba góc đầu lâu có viền đỏ + áo thuyền trưởng đỏ viền
  vàng, thắt lưng nâu). **Không có bịt mắt** (người dùng: "các ảnh bịt mắt bị lệch r kìa"): ChatGPT không giữ được chỗ
  bịt mắt giữa các khung; chi tiết nhỏ trên mặt đều bị lệch, nên trang phục không đặt gì lên mặt. Mỗi bộ ~40 tờ 16 khung,
  chỉ các hoạt ảnh trong màn (bỏ menu, thẻ kết quả, loading, hải cẩu bị nhốt). Tờ nguồn:
  `Assets/Art/Reference/outfit_<tên>/<tên tờ gốc>`, cắt bằng `python3 -I Tools/slice_sheets.py --outfit <tên>` thành
  `<tên>_pup_<anim>_<i>`; `Art.Outfit` đổi mọi hình `pup*` sang khung của bộ đang mặc (thiếu hoạt ảnh nào thì dùng khung
  gốc của hoạt ảnh đó). Mỗi hoạt ảnh được co về đúng cỡ trung bình của bản gốc (`match_classic`), nén ASTC 6×6.
  Cách vẽ đạt: tờ đầu gửi kèm lời tả bộ đồ; các tờ sau trong cùng chat; **luôn so từng tờ với tờ gốc cạnh nhau**: ChatGPT
  hay chép tư thế của tờ trước khi hai tờ giống nhau (treo dây / bungee, các kiểu văng), tờ đó vẽ lại trong chat mới với
  2 ảnh (ảnh mẫu bộ đồ + tờ gốc).
- **Người dùng muốn skin "khác hẳn", vẽ mới chứ không chỉ đổi màu** (23/11: "vẽ thêm nhiều kiểu skin khác hẳn đi ví dụ như
  thêm quần áo… tương tự cho các loại skin khác"). Máy phóng: ChatGPT vẽ lại tờ `catapult_pair` (cần + xe) theo chủ đề, giữ
  nguyên dáng. Trang phục vẽ mới: hải cẩu có 841 khung/61 hoạt ảnh (~60 tờ), làm **1 bộ trước** để duyệt.
- **Máy phóng và dây đu đều vẽ mới** (25/11), không còn bản đổi màu: 8 máy phóng (tre, kẹo, hải tặc, băng, tên lửa, núi lửa,
  rồng, hoàng gia) từ `catapult_skin_<t>.png`; 8 dây (dây leo, kẹo, ruy băng, cáp cảnh báo, băng, cầu vồng, xích vàng, phép
  thuật) từ một tờ `rope_skins.png` (8 dải dọc), cắt bằng `Tools/cut_ropes.py`: mỗi dải được cắt đúng số chu kỳ lặp (tự
  tương quan theo chiều dọc) nên ghép liền không lộ mối khi `SwingRope` lát dọc. Chỉ tab Màu lông còn là đổi màu.
- **Tranh mới**: `skin_props.png` (7 hình hiệu ứng + dấu tick + 8 quả bóng), `shop_podium.png`, cắt bằng `Tools/cut_props.py`.
  Thêm PNG mới xong phải chạy `ProjectBuilder.ConfigureArt`.
