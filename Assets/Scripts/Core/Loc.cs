using System;
using System.Collections.Generic;
using UnityEngine;

namespace SealCatapult.Core
{
    /// <summary>
    /// String tables, one column per language in <see cref="Codes"/> order (English is the default).
    /// shortcut: in-code tables instead of the Unity Localization package; move to it when translators arrive.
    /// </summary>
    public static class Loc
    {
        public static event Action Changed;

        static readonly Dictionary<string, string[]> table = new Dictionary<string, string[]>
        {
            // key                       en, vi, then de, fr, es, it, pl (EU listings, docs/ASO.md)
            // The game's name is a brand: the same in every language (user 18/11: "tên game ko theo ngôn ngữ").
            ["title"]          = new[] { "Seal Catapult",                             "Seal Catapult",
                                         "Seal Catapult", "Seal Catapult", "Seal Catapult", "Seal Catapult", "Seal Catapult" },
            ["settings"]       = new[] { "Settings",                                "Cài đặt",
                                         "Einstellungen", "Réglages", "Ajustes", "Impostazioni", "Ustawienia" },
            ["quit"]           = new[] { "Quit",                                    "Thoát",
                                         "Beenden", "Quitter", "Salir", "Esci", "Wyjdź" },
            ["map_title"]      = new[] { "Frost Bay",                               "Vịnh Băng Giá",
                                         "Frostbucht", "Baie Gelée", "Bahía Helada", "Baia Gelata", "Mroźna Zatoka" },
            ["shop"] = new[] { "Shop", "Cửa hàng", "Shop", "Boutique", "Tienda", "Negozio", "Sklep" },
            ["tab_up"] = new[] { "Upgrades", "Nâng cấp", "Upgrades", "Améliorations", "Mejoras", "Potenziamenti", "Ulepszenia" },
            ["tab_fur"] = new[] { "Fur", "Màu lông", "Fell", "Pelage", "Pelaje", "Pelliccia", "Futro" },
            ["tab_fit"] = new[] { "Outfits", "Trang phục", "Outfits", "Tenues", "Atuendos", "Completi", "Stroje" },
            ["tab_ball"] = new[] { "Balls", "Bóng", "Bälle", "Balles", "Bolas", "Palle", "Kule" },
            ["tab_trail"] = new[] { "Trails", "Vệt bay", "Spuren", "Traînées", "Estelas", "Scie", "Smugi" },
            ["tab_hit"] = new[] { "Impacts", "Va chạm", "Treffer", "Impacts", "Impactos", "Impatti", "Uderzenia" },
            ["tab_cart"] = new[] { "Launchers", "Máy phóng", "Katapulte", "Catapultes", "Catapultas", "Catapulte", "Katapulty" },
            ["tab_rope"] = new[] { "Ropes", "Dây đu", "Seile", "Cordes", "Cuerdas", "Corde", "Liny" },
            ["hint_Fur"] = new[] { "A new fur colour for every pup.", "Màu lông mới cho cả đàn hải cẩu.", "Eine neue Fellfarbe für alle Robben.", "Une nouvelle couleur de pelage pour tous les phoques.", "Un nuevo color de pelaje para todas las focas.", "Un nuovo colore di pelliccia per tutte le foche.", "Nowy kolor futra dla wszystkich fok." },
            ["hint_Outfit"] = new[] { "A whole new outfit for every pup.", "Bộ đồ mới hẳn cho cả đàn hải cẩu.", "Ein ganz neues Outfit für alle Robben.", "Une toute nouvelle tenue pour tous les phoques.", "Un atuendo totalmente nuevo para todas las focas.", "Un completo tutto nuovo per tutte le foche.", "Zupełnie nowy strój dla wszystkich fok." },
            ["hint_Ball"] = new[] { "The ball on the catapult's arm.", "Quả bóng trên cần máy phóng.", "Der Ball am Arm des Katapults.", "La balle au bout du bras de la catapulte.", "La bola en el brazo de la catapulta.", "La palla sul braccio della catapulta.", "Kula na ramieniu katapulty." },
            ["hint_Trail"] = new[] { "What a flying pup leaves behind.", "Vệt để lại sau hải cẩu đang bay.", "Was eine fliegende Robbe hinter sich lässt.", "Ce qu'un phoque en vol laisse derrière lui.", "Lo que deja tras de sí una foca en vuelo.", "Ciò che una foca in volo lascia dietro di sé.", "To, co zostawia za sobą lecąca foka." },
            ["hint_Impact"] = new[] { "What bursts out when a pup crashes.", "Thứ bung ra khi hải cẩu va chạm.", "Was herausplatzt, wenn eine Robbe aufprallt.", "Ce qui jaillit quand un phoque percute.", "Lo que estalla cuando una foca choca.", "Ciò che esplode quando una foca si schianta.", "To, co wybucha, gdy foka uderza." },
            ["hint_Launcher"] = new[] { "A whole new catapult, drawn from scratch.", "Một máy phóng vẽ mới hoàn toàn.", "Ein ganz neu gezeichnetes Katapult.", "Une toute nouvelle catapulte.", "Una catapulta totalmente nueva.", "Una catapulta tutta nuova.", "Zupełnie nowa katapulta." },
            ["hint_Rope"] = new[] { "A new rope for the swing.", "Một sợi dây đu mới.", "Ein neues Schwungseil.", "Une nouvelle corde pour la balançoire.", "Una cuerda nueva para el columpio.", "Una corda nuova per l'altalena.", "Nowa lina do huśtawki." },
            ["owned"] = new[] { "Owned", "Đã có", "Gekauft", "Acquis", "Comprado", "Acquistato", "Kupione" },
            ["worn"] = new[] { "In use", "Đang dùng", "Aktiv", "Équipé", "En uso", "In uso", "W użyciu" },
            ["equip"] = new[] { "Use", "Dùng", "Nutzen", "Équiper", "Usar", "Usa", "Użyj" },
            ["sk_classic"] = new[] { "Classic", "Cổ điển", "Klassisch", "Classique", "Clásico", "Classico", "Klasyczny" },
            ["sk_silver"] = new[] { "Silver", "Bạc", "Silber", "Argent", "Plata", "Argento", "Srebrny" },
            ["sk_cream"] = new[] { "Cream", "Kem", "Creme", "Crème", "Crema", "Crema", "Kremowy" },
            ["sk_pink"] = new[] { "Pink", "Hồng", "Rosa", "Rose", "Rosa", "Rosa", "Różowy" },
            ["sk_sky"] = new[] { "Sky", "Xanh trời", "Himmel", "Ciel", "Cielo", "Cielo", "Błękit" },
            ["sk_mint"] = new[] { "Mint", "Bạc hà", "Minze", "Menthe", "Menta", "Menta", "Mięta" },
            ["sk_lilac"] = new[] { "Lilac", "Tử đinh hương", "Flieder", "Lilas", "Lila", "Lilla", "Liliowy" },
            ["sk_peach"] = new[] { "Peach", "Đào", "Pfirsich", "Pêche", "Melocotón", "Pesca", "Brzoskwinia" },
            ["sk_coal"] = new[] { "Coal", "Than", "Kohle", "Charbon", "Carbón", "Carbone", "Węgiel" },
            ["sk_stone"] = new[] { "Stone", "Đá", "Stein", "Pierre", "Piedra", "Pietra", "Kamień" },
            ["sk_melon"] = new[] { "Melon", "Dưa hấu", "Melone", "Pastèque", "Sandía", "Anguria", "Arbuz" },
            ["sk_coconut"] = new[] { "Coconut", "Dừa", "Kokosnuss", "Noix de coco", "Coco", "Cocco", "Kokos" },
            ["sk_beach"] = new[] { "Beach ball", "Bóng biển", "Wasserball", "Ballon de plage", "Pelota de playa", "Pallone da spiaggia", "Piłka plażowa" },
            ["sk_bowling"] = new[] { "Bowling", "Bowling", "Bowling", "Bowling", "Bolos", "Bowling", "Kręgle" },
            ["sk_disco"] = new[] { "Disco", "Disco", "Disco", "Disco", "Disco", "Disco", "Disco" },
            ["sk_puffer"] = new[] { "Pufferfish", "Cá nóc", "Kugelfisch", "Poisson-globe", "Pez globo", "Pesce palla", "Rozdymka" },
            ["sk_golden"] = new[] { "Golden", "Vàng kim", "Golden", "Doré", "Dorado", "Dorato", "Złoty" },
            ["sk_flakes"] = new[] { "Snowflakes", "Bông tuyết", "Schneeflocken", "Flocons", "Copos", "Fiocchi", "Śnieżynki" },
            ["sk_stars"] = new[] { "Stars", "Sao", "Sterne", "Étoiles", "Estrellas", "Stelle", "Gwiazdki" },
            ["sk_petals"] = new[] { "Petals", "Cánh hoa", "Blütenblätter", "Pétales", "Pétalos", "Petali", "Płatki" },
            ["sk_leaves"] = new[] { "Leaves", "Lá", "Blätter", "Feuilles", "Hojas", "Foglie", "Liście" },
            ["sk_bubbles"] = new[] { "Bubbles", "Bong bóng", "Blasen", "Bulles", "Burbujas", "Bolle", "Bąbelki" },
            ["sk_hearts"] = new[] { "Hearts", "Trái tim", "Herzen", "Cœurs", "Corazones", "Cuori", "Serca" },
            ["sk_notes"] = new[] { "Music", "Nốt nhạc", "Musik", "Musique", "Música", "Musica", "Muzyka" },
            ["sk_flames"] = new[] { "Flames", "Ngọn lửa", "Flammen", "Flammes", "Llamas", "Fiamme", "Płomienie" },
            ["sk_blossoms"] = new[] { "Blossoms", "Hoa đào", "Blüten", "Fleurs", "Flores", "Fiori", "Kwiaty" },
            ["sk_bolts"] = new[] { "Lightning", "Tia chớp", "Blitze", "Éclairs", "Rayos", "Fulmini", "Błyskawice" },
            ["sk_fish"] = new[] { "Fish", "Cá", "Fische", "Poissons", "Peces", "Pesci", "Ryby" },
            ["sk_ice"] = new[] { "Ice crystal", "Pha lê băng", "Eiskristall", "Cristal de glace", "Cristal de hielo", "Cristallo di ghiaccio", "Lodowy kryształ" },
            ["sk_ninja"] = new[] { "Ninja", "Ninja", "Ninja", "Ninja", "Ninja", "Ninja", "Ninja" },
            ["sk_dragon"] = new[] { "Dragon", "Rồng", "Drache", "Dragon", "Dragón", "Drago", "Smok" },
            ["sk_rocket"] = new[] { "Rocket", "Tên lửa", "Rakete", "Fusée", "Cohete", "Razzo", "Rakieta" },
            ["sk_bamboo"] = new[] { "Bamboo", "Tre", "Bambus", "Bambou", "Bambú", "Bambù", "Bambus" },
            ["sk_lava"] = new[] { "Volcano", "Núi lửa", "Vulkan", "Volcan", "Volcán", "Vulcano", "Wulkan" },
            ["sk_vine"] = new[] { "Jungle vine", "Dây leo", "Lianen", "Liane", "Enredadera", "Liana", "Liana" },
            ["sk_ribbon"] = new[] { "Ribbon", "Ruy băng", "Schleife", "Ruban", "Cinta", "Nastro", "Wstążka" },
            ["sk_hazard"] = new[] { "Hazard", "Cảnh báo", "Warnband", "Danger", "Peligro", "Pericolo", "Ostrzeżenie" },
            ["sk_rainbow"] = new[] { "Rainbow", "Cầu vồng", "Regenbogen", "Arc-en-ciel", "Arcoíris", "Arcobaleno", "Tęcza" },
            ["sk_chain"] = new[] { "Gold chain", "Xích vàng", "Goldkette", "Chaîne d'or", "Cadena de oro", "Catena d'oro", "Złoty łańcuch" },
            ["sk_magic"] = new[] { "Magic", "Phép thuật", "Magie", "Magie", "Magia", "Magia", "Magia" },
            ["sk_pirate"] = new[] { "Pirate", "Hải tặc", "Pirat", "Pirate", "Pirata", "Pirata", "Pirat" },
            ["sk_candy"] = new[] { "Candy", "Kẹo ngọt", "Bonbon", "Bonbon", "Caramelo", "Caramella", "Cukierek" },
            ["sk_royal"] = new[] { "Royal", "Hoàng gia", "Königlich", "Royal", "Real", "Reale", "Królewski" },
            ["music"]          = new[] { "Music",                                   "Nhạc",
                                         "Musik", "Musique", "Música", "Musica", "Muzyka" },
            ["sfx"]            = new[] { "Sound effects",                           "Hiệu ứng",
                                         "Soundeffekte", "Effets sonores", "Efectos de sonido", "Effetti sonori", "Efekty dźwiękowe" },
            ["language"]       = new[] { "Language",                                "Ngôn ngữ",
                                         "Sprache", "Langue", "Idioma", "Lingua", "Język" },
            ["lang_name"]      = new[] { "English",                                 "Tiếng Việt",
                                         "Deutsch", "Français", "Español", "Italiano", "Polski" },
            ["reset"]          = new[] { "Erase save data",                         "Xóa dữ liệu lưu",
                                         "Spielstand löschen", "Effacer la sauvegarde", "Borrar partida", "Cancella salvataggio", "Usuń zapis gry" },
            ["reset_confirm"]  = new[] { "Erase all stars, upgrades and skins?",       "Xóa hết sao, nâng cấp và skin?",
                                         "Alle Sterne, Upgrades und Skins löschen?", "Effacer toutes les étoiles, améliorations et skins ?", "¿Borrar todas las estrellas, mejoras y aspectos?", "Cancellare tutte le stelle, i potenziamenti e le skin?", "Usunąć wszystkie gwiazdki, ulepszenia i skórki?" },
            ["yes"]            = new[] { "Yes",                                     "Có",
                                         "Ja", "Oui", "Sí", "Sì", "Tak" },
            ["no"]             = new[] { "No",                                      "Không",
                                         "Nein", "Non", "No", "No", "Nie" },
            ["back"]           = new[] { "Back",                                    "Quay lại",
                                         "Zurück", "Retour", "Atrás", "Indietro", "Wstecz" },
            ["paused"]         = new[] { "PAUSE",                                   "TẠM DỪNG",
                                         "PAUSE", "PAUSE", "PAUSA", "PAUSA", "PAUZA" },
            ["resume"]         = new[] { "Resume",                                  "Tiếp tục",
                                         "Weiter", "Reprendre", "Continuar", "Riprendi", "Wznów" },
            ["retry"]          = new[] { "Retry",                                   "Chơi lại",
                                         "Nochmal", "Rejouer", "Reintentar", "Riprova", "Jeszcze raz" },
            ["map"]            = new[] { "Map",                                     "Bản đồ",
                                         "Karte", "Carte", "Mapa", "Mappa", "Mapa" },
            ["level_n"]        = new[] { "Level {0}",                               "Màn {0}",
                                         "Level {0}", "Niveau {0}", "Nivel {0}", "Livello {0}", "Poziom {0}" },
            ["through"]        = new[] { "{0} pups made it out!",                   "{0} hải cẩu thoát ra ngoài!",
                                         "{0} Robben sind durch!", "{0} phoques sont passés !", "¡{0} focas han salido!", "{0} foche sono uscite!", "Wyleciało fok: {0}!" },
            ["through_one"]    = new[] { "1 pup made it out!",                      "1 hải cẩu thoát ra ngoài!",
                                         "1 Robbe ist durch!", "1 phoque est passé !", "¡1 foca ha salido!", "1 foca è uscita!", "Wyleciała 1 foka!" },
            ["none_through"]   = new[] { "No pup made it out!",                     "Không con nào thoát ra được!",
                                         "Keine Robbe ist durch!", "Aucun phoque n'est passé !", "¡Ninguna foca ha salido!", "Nessuna foca è uscita!", "Żadna foka nie wyleciała!" },
            ["attack"]         = new[] { "Attack!",                                 "Tấn công!",
                                         "Angriff!", "À l'attaque !", "¡Al ataque!", "All'attacco!", "Do ataku!" },
            ["mission_n"]      = new[] { "Mission {0}",                             "Nhiệm vụ {0}",
                                         "Mission {0}", "Mission {0}", "Misión {0}", "Missione {0}", "Misja {0}" },
            ["hit_bears"]      = new[] { "Hit {0} bears to continue to the next level", "Hạ {0} gấu để sang màn tiếp theo",
                                         "Besiege {0} Bären für das nächste Level", "Assomme {0} ours pour passer au niveau suivant", "Derriba {0} osos para pasar al siguiente nivel", "Abbatti {0} orsi per passare al livello successivo", "Pokonaj niedźwiedzie, by przejść dalej: {0}" },
            ["congrats"]       = new[] { "Congratulations!",                       "Chúc mừng!",
                                         "Glückwunsch!", "Bravo !", "¡Enhorabuena!", "Complimenti!", "Gratulacje!" },
            ["continue"]       = new[] { "Continue",                                "Tiếp tục",
                                         "Weiter", "Continuer", "Continuar", "Continua", "Dalej" },
            ["help_cry"]       = new[] { "HELP!",                                   "CỨU!",
                                         "HILFE!", "À L'AIDE !", "¡SOCORRO!", "AIUTO!", "POMOCY!" },
            ["total_label"]    = new[] { "Total",                                   "Tổng",
                                         "Gesamt", "Total", "Total", "Totale", "Razem" },
            ["bears_hit"]      = new[] { "Bears knocked out: {0}/{1}",              "Gấu đã hạ: {0}/{1}",
                                         "Bären besiegt: {0}/{1}", "Ours assommés : {0}/{1}", "Osos derribados: {0}/{1}", "Orsi abbattuti: {0}/{1}", "Pokonane niedźwiedzie: {0}/{1}" },
            ["perfect_mission"]= new[] { "Perfect mission!",                        "Nhiệm vụ hoàn hảo!",
                                         "Perfekte Mission!", "Mission parfaite !", "¡Misión perfecta!", "Missione perfetta!", "Perfekcyjna misja!" },
            ["mission_failed"] = new[] { "Mission failed!",                         "Thất bại!",
                                         "Mission gescheitert!", "Mission échouée !", "¡Misión fallida!", "Missione fallita!", "Misja nieudana!" },
            ["bears_all_time"] = new[] { "Total bears: {0}",                        "Tổng gấu: {0}",
                                         "Bären gesamt: {0}", "Total d'ours : {0}", "Osos en total: {0}", "Orsi in totale: {0}", "Niedźwiedzie łącznie: {0}" },
            ["stats"]          = new[] { "Stats",                                   "Thống kê",
                                         "Statistik", "Statistiques", "Estadísticas", "Statistiche", "Statystyki" },
            ["ad_bonus_pup"] = new[] { "Ad: +1 pup", "QC: +1 hải cẩu", "Werbung: +1 Robbe", "Pub : +1 phoque", "Anuncio: +1 foca", "Annuncio: +1 foca", "Reklama: +1 foka" },
            ["ad_none"] = new[] { "No ad right now", "Chưa có quảng cáo", "Keine Werbung da", "Pas de pub", "Sin anuncios ahora", "Nessun annuncio ora", "Brak reklamy" },
            ["privacy"]        = new[] { "Privacy",                                 "Quyền riêng tư",
                                         "Datenschutz", "Confidentialité", "Privacidad", "Privacy", "Prywatność" },
            ["skip"]           = new[] { "Skip",                                    "Bỏ qua",
                                         "Überspringen", "Passer", "Saltar", "Salta", "Pomiń" },
            ["story"]          = new[] { "Story",                                   "Truyện",
                                         "Geschichte", "Histoire", "Historia", "Storia", "Historia" },
            ["story_1"]        = new[] { "The seal village was happy, with fish stored for the whole winter.", "Làng hải cẩu sống vui vẻ, kho cá đầy đủ cho cả mùa đông.",
                                         "Das Robbendorf war glücklich, mit Fisch für den ganzen Winter.", "Le village des phoques était heureux, avec du poisson pour tout l'hiver.", "El pueblo de las focas era feliz, con pescado para todo el invierno.", "Il villaggio delle foche era felice, con pesce per tutto l'inverno.", "Wioska fok żyła szczęśliwie, z rybami na całą zimę." },
            ["story_2"]        = new[] { "Then the polar bears came down from their ice fortress…", "Rồi bầy gấu bắc cực kéo xuống từ pháo đài băng…",
                                         "Dann kamen die Eisbären aus ihrer Eisfestung herunter…", "Puis les ours polaires sont descendus de leur forteresse de glace…", "Entonces los osos polares bajaron de su fortaleza de hielo…", "Poi gli orsi polari scesero dalla loro fortezza di ghiaccio…", "Wtedy niedźwiedzie polarne zeszły ze swojej lodowej twierdzy…" },
            ["story_3"]        = new[] { "They rolled a giant snowball over the pups!", "Chúng lăn một quả cầu tuyết khổng lồ cuốn lấy bầy hải cẩu!",
                                         "Sie rollten einen riesigen Schneeball über die Robbenkinder!", "Ils ont roulé une énorme boule de neige sur les bébés phoques !", "¡Hicieron rodar una bola de nieve gigante sobre las crías!", "Fecero rotolare una palla di neve gigante sui cuccioli!", "Stoczyły olbrzymią śnieżkę prosto na małe foki!" },
            ["story_4"]        = new[] { "The bears locked the pups in cages and took the winter fish.", "Gấu nhốt hải cẩu vào lồng và cướp hết kho cá.",
                                         "Die Bären sperrten die Robben in Käfige und nahmen den Winterfisch.", "Les ours ont enfermé les phoques en cage et volé le poisson d'hiver.", "Los osos encerraron a las focas en jaulas y se llevaron el pescado.", "Gli orsi chiusero le foche in gabbia e presero il pesce d'inverno.", "Niedźwiedzie zamknęły foki w klatkach i zabrały zimowe ryby." },
            ["story_5"]        = new[] { "Only Pip got away. Time to launch a rescue!", "Chỉ có Pip thoát được. Đến lúc phóng đi giải cứu!",
                                         "Nur Pip ist entkommen. Zeit für eine Rettung!", "Seul Pip s'est échappé. Place au sauvetage !", "Solo Pip escapó. ¡Hora de lanzar el rescate!", "Solo Pip è scappato. È ora del salvataggio!", "Uciekł tylko Pip. Czas na akcję ratunkową!" },
            ["stats_total"]    = new[] { "Bears knocked out",                       "Tổng gấu đã hạ",
                                         "Besiegte Bären", "Ours assommés", "Osos derribados", "Orsi abbattuti", "Pokonane niedźwiedzie" },
            ["stats_x2"]       = new[] { "2 bears in one dive",                     "Hạ 2 gấu một lần",
                                         "2 Bären in einem Sturzflug", "2 ours en un plongeon", "2 osos en una caída", "2 orsi in un tuffo", "2 niedźwiedzie naraz" },
            ["stats_x3"]       = new[] { "3 bears in one dive",                     "Hạ 3 gấu một lần",
                                         "3 Bären in einem Sturzflug", "3 ours en un plongeon", "3 osos en una caída", "3 orsi in un tuffo", "3 niedźwiedzie naraz" },
            ["stats_x4"]       = new[] { "4+ bears in one dive",                    "Hạ 4+ gấu một lần",
                                         "4+ Bären in einem Sturzflug", "4+ ours en un plongeon", "4+ osos en una caída", "4+ orsi in un tuffo", "4+ niedźwiedzi naraz" },
            ["stats_perfect"]  = new[] { "Perfect missions",                        "Màn hoàn hảo",
                                         "Perfekte Missionen", "Missions parfaites", "Misiones perfectas", "Missioni perfette", "Perfekcyjne misje" },
            ["ok_hit"]         = new[] { "OK!",                                     "OK!",
                                         "OK!", "OK !", "¡OK!", "OK!", "OK!" },
            ["bungee_tip"]     = new[] { "Bungee! Fire when the pup stretches lowest", "Dây nhún! Bắn lúc hải cẩu xuống thấp nhất",
                                         "Bungee! Feuer, wenn die Robbe am tiefsten ist", "Élastique ! Tire quand le phoque est au plus bas", "¡Elástico! Dispara cuando la foca esté más abajo", "Elastico! Spara quando la foca è più in basso", "Gumka! Strzelaj, gdy foka jest najniżej" },
            ["great_hit"]      = new[] { "GREAT!",                                  "TUYỆT!",
                                         "SUPER!", "GÉNIAL !", "¡GENIAL!", "OTTIMO!", "ŚWIETNIE!" },
            ["oops_hit"]       = new[] { "OOPS!",                                   "ỐI!",
                                         "HOPPLA!", "OUPS !", "¡UY!", "OPS!", "UPS!" },
            ["grr_miss"]       = new[] { "Grr…",                                    "Grr…",
                                         "Grr…", "Grr…", "Grr…", "Grr…", "Grr…" },
            ["maxed"]          = new[] { "Max",                                     "Tối đa",
                                         "Max", "Max", "Máx.", "Max", "Maks." },
            ["lv"]             = new[] { "Lv {0}/{1}",                              "Cấp {0}/{1}",
                                         "Stufe {0}/{1}", "Niv. {0}/{1}", "Nv. {0}/{1}", "Liv. {0}/{1}", "Poz. {0}/{1}" },
            ["up_Spiky"]       = new[] { "Ice Spikes",                              "Gai băng",
                                         "Eisstacheln", "Pics de glace", "Púas de hielo", "Punte di ghiaccio", "Lodowe kolce" },
            ["up_PuffUp"]      = new[] { "Bouncy Ball",                             "Cuộn tròn",
                                         "Hüpfball", "Boule rebondissante", "Bola saltarina", "Palla rimbalzante", "Skacząca piłka" },
            ["up_School"]      = new[] { "Pod",                                     "Bầy hải cẩu",
                                         "Rudel", "Bande", "Manada", "Branco", "Stado" },
            ["up_Ninja"]       = new[] { "Ninja Pup",                               "Hải cẩu ninja",
                                         "Ninja-Robbe", "Phoque ninja", "Foca ninja", "Foca ninja", "Foka ninja" },
            ["up_Blaze"]       = new[] { "Blazing Pup",                             "Hải cẩu lửa",
                                         "Feuer-Robbe", "Phoque de feu", "Foca de fuego", "Foca di fuoco", "Ognista foka" },
            ["upd_Spiky"]      = new[] { "Smash through {0} ice blocks",            "Xuyên {0} khối băng",
                                         "Durchbricht {0} Eisblöcke", "Traverse {0} blocs de glace", "Atraviesa {0} bloques de hielo", "Sfonda {0} blocchi di ghiaccio", "Przebija bloki lodu: {0}" },
            ["upd_PuffUp"]     = new[] { "+{0} bounces after impact",               "+{0} lần nảy sau va chạm",
                                         "+{0} Sprünge nach dem Aufprall", "+{0} rebonds après l'impact", "+{0} rebotes tras el impacto", "+{0} rimbalzi dopo l'impatto", "Odbicia po uderzeniu: +{0}" },
            ["upd_School"]     = new[] { "+{0} pups per level",                     "+{0} hải cẩu mỗi màn",
                                         "+{0} Robben pro Level", "+{0} phoques par niveau", "+{0} focas por nivel", "+{0} foche per livello", "Foki na poziom: +{0}" },
            ["upd_Ninja"]      = new[] { "Tap again mid-dive to stop and drop straight down, {0}× per pup", "Chạm lần nữa khi đang lao để khựng rồi rơi thẳng, {0} lần mỗi con",
                                         "Tippe im Sturzflug erneut: stoppen und senkrecht fallen, {0}× pro Robbe", "Touche encore en plongée pour t'arrêter et tomber tout droit, {0}× par phoque", "Toca otra vez en plena caída para frenar y caer en vertical, {0}× por foca", "Tocca di nuovo in picchiata per fermarti e cadere dritto, {0}× per foca", "Stuknij ponownie w locie, by stanąć i spaść pionowo, {0}× na fokę" },
            ["upd_Blaze"]      = new[] { "Tap again mid-dive to catch fire and melt through {0} ice cubes", "Chạm lần nữa khi đang lao để bốc lửa, xuyên {0} khối băng",
                                         "Tippe im Sturzflug erneut: Feuer fangen und {0} Eiswürfel schmelzen", "Touche encore en plongée pour t'enflammer et faire fondre {0} cubes de glace", "Toca otra vez en plena caída para arder y derretir {0} cubos de hielo", "Tocca di nuovo in picchiata per prendere fuoco e sciogliere {0} cubetti di ghiaccio", "Stuknij ponownie w locie, by zapłonąć i stopić kostki lodu: {0}" },
            ["how_to"]         = new[] { "How to play",                             "Cách chơi",
                                         "So wird gespielt", "Comment jouer", "Cómo jugar", "Come si gioca", "Jak grać" },
            ["about"]          = new[] { "About",                                   "Giới thiệu",
                                         "Info", "À propos", "Acerca de", "Info", "O grze" },
            ["hint_aim_bungee"] = new[] { "Tap to fire! Hit the pup when it stretches lowest", "Chạm để bắn! Bắn lúc hải cẩu nhún xuống thấp nhất",
                                         "Tippen zum Feuern! Schieß, wenn die Robbe am tiefsten ist", "Touche pour tirer ! Vise quand le phoque est au plus bas", "¡Toca para disparar! Hazlo cuando la foca esté más abajo", "Tocca per sparare! Colpisci quando la foca è più in basso", "Stuknij, by strzelić! Strzel, gdy foka jest najniżej" },
            ["hint_aim"]       = new[] { "Tap to whack! Hit it as it swings over the catapult", "Chạm để đập! Đập lúc nó đu qua máy bắn",
                                         "Tippen zum Schlagen! Triff, wenn sie über das Katapult schwingt", "Touche pour frapper ! Vise quand il passe au-dessus de la catapulte", "¡Toca para golpear! Hazlo cuando pase sobre la catapulta", "Tocca per colpire! Colpisci quando oscilla sopra la catapulta", "Stuknij, by uderzyć! Uderz, gdy foka przelatuje nad katapultą" },
            ["hint_dive"] = new[] { "Tap early! The pup dives forward at a slant", "Chạm sớm! Hải cẩu lao chéo về phía trước", "Früh tippen! Die Robbe stürzt schräg nach vorn", "Touche tôt ! Le phoque plonge en biais vers l'avant", "¡Toca pronto! La foca se lanza en diagonal hacia delante", "Tocca presto! La foca si tuffa in diagonale in avanti", "Stuknij wcześnie! Foka nurkuje ukosem do przodu" },
            ["howto_1"]        = new[] { "Pups swing on the rope. Tap to swing the whacker: a good hit sends them up through the gap and on to the fortress.",
                                         "Hải cẩu đu trên dây. Chạm để máy đập vung búa, đánh chúng bay lọt khe góc trên bên phải, thẳng tới pháo đài.",
                                         "Robben schaukeln am Seil. Tippe, damit der Schläger zuschlägt: ein guter Treffer schickt sie durch die Lücke zur Festung.", "Les phoques se balancent sur la corde. Touche pour frapper : un bon coup les envoie par l'ouverture jusqu'à la forteresse.", "Las focas se balancean en la cuerda. Toca para golpear: un buen golpe las lanza por el hueco hasta la fortaleza.", "Le foche dondolano sulla corda. Tocca per colpire: un buon colpo le manda nel varco fino alla fortezza.", "Foki huśtają się na linie. Stuknij, by uderzyć: dobry cios posyła je przez szczelinę prosto do twierdzy." },
            ["howto_2"]        = new[] { "Every pup that gets out joins the attack on the bear fortress.",
                                         "Con nào bay lọt ra ngoài sẽ cùng tấn công pháo đài gấu.",
                                         "Jede Robbe, die hinauskommt, greift die Bärenfestung an.", "Chaque phoque qui sort rejoint l'attaque de la forteresse des ours.", "Cada foca que sale se une al ataque contra la fortaleza de los osos.", "Ogni foca che esce si unisce all'attacco alla fortezza degli orsi.", "Każda foka, która wyleci, dołącza do ataku na twierdzę niedźwiedzi." },
            ["howto_3"] = new[] { "Pups fly across the bears' arena. Tap before the bear: the pup dives forward at a slant. Bounce from bear to bear for a combo!", "Hải cẩu bay ngang đấu trường của gấu. Chạm trước khi tới con gấu: hải cẩu lao chéo về phía trước. Nảy từ gấu này sang gấu khác để ăn combo!", "Robben fliegen über die Arena der Bären. Tippe vor dem Bären: die Robbe stürzt schräg nach vorn. Spring von Bär zu Bär für eine Kombo!", "Les phoques survolent l'arène des ours. Touche avant l'ours : le phoque plonge en biais vers l'avant. Rebondis d'ours en ours pour un combo !", "Las focas cruzan la arena de los osos. Toca antes del oso: la foca se lanza en diagonal hacia delante. ¡Rebota de oso en oso para hacer combo!", "Le foche sorvolano l'arena degli orsi. Tocca prima dell'orso: la foca si tuffa in diagonale in avanti. Rimbalza da un orso all'altro per una combo!", "Foki przelatują nad areną niedźwiedzi. Stuknij przed niedźwiedziem: foka nurkuje ukosem do przodu. Odbijaj się od kolejnych, by zrobić combo!" },
            ["howto_4"]        = new[] { "Knock out enough bears to pass, all of them for a Perfect badge. Stars buy upgrades.",
                                         "Hạ đủ gấu để qua màn, hạ hết để lấy huy hiệu Hoàn hảo. Sao dùng mua nâng cấp.",
                                         "Besiege genug Bären zum Weiterkommen, alle für das Perfekt-Abzeichen. Mit Sternen kaufst du Upgrades.", "Assomme assez d'ours pour passer, tous pour le badge Parfait. Les étoiles achètent des améliorations.", "Derriba suficientes osos para pasar, todos para la insignia Perfecto. Las estrellas compran mejoras.", "Abbatti abbastanza orsi per passare, tutti per il distintivo Perfetto. Le stelle comprano potenziamenti.", "Pokonaj dość niedźwiedzi, by przejść, wszystkie dla odznaki Perfekcyjnie. Za gwiazdki kupisz ulepszenia." },
            ["about_text"]     = new[] { "Polar bears stole the seal village's winter fish. Grandpa Walrus built the whacker so the pups can win it back.",
                                         "Bầy gấu bắc cực cướp kho cá mùa đông của làng hải cẩu. Ông Hải Mã chế ra máy đập để bầy hải cẩu con giành lại.",
                                         "Eisbären haben den Winterfisch des Robbendorfs gestohlen. Opa Walross baute den Schläger, damit die Robben ihn zurückholen.", "Les ours polaires ont volé le poisson d'hiver du village des phoques. Papi Morse a construit la machine pour que les petits le récupèrent.", "Los osos polares robaron el pescado de invierno del pueblo de las focas. El abuelo Morsa construyó la máquina para que las crías lo recuperen.", "Gli orsi polari hanno rubato il pesce d'inverno del villaggio delle foche. Nonno Tricheco ha costruito la macchina perché i cuccioli lo riprendano.", "Niedźwiedzie polarne ukradły zimowe ryby wioski fok. Dziadek Mors zbudował maszynę, by małe foki je odzyskały." },
            ["credits"]        = new[] { "Art & sounds: made for this game · Font: Baloo 2 (SIL OFL)",
                                         "Hình & âm thanh: tự làm cho game · Phông: Baloo 2 (SIL OFL)",
                                         "Grafik & Sound: für dieses Spiel gemacht · Schrift: Baloo 2 (SIL OFL)", "Graphismes et sons : créés pour ce jeu · Police : Baloo 2 (SIL OFL)", "Gráficos y sonidos: hechos para este juego · Fuente: Baloo 2 (SIL OFL)", "Grafica e suoni: creati per questo gioco · Font: Baloo 2 (SIL OFL)", "Grafika i dźwięki: stworzone dla tej gry · Czcionka: Baloo 2 (SIL OFL)" },
            ["need_stars"] = new[] { "Stars missing: {0}", "Cần thêm {0} sao", "Fehlende Sterne: {0}", "Étoiles manquantes : {0}", "Estrellas que faltan: {0}", "Stelle mancanti: {0}", "Brakuje gwiazdek: {0}" },
            ["collect_fish"] = new[] { "Fly through the fish to collect them!", "Bay xuyên qua đàn cá để gom cá!", "Flieg durch die Fische und sammle sie ein!", "Vole à travers les poissons pour les ramasser !", "¡Vuela entre los peces para recogerlos!", "Vola tra i pesci per raccoglierli!", "Przeleć przez ryby, by je zebrać!" },
            ["fish_got"] = new[] { "Fish collected: {0}/{1}", "Cá đã gom: {0}/{1}", "Gesammelte Fische: {0}/{1}", "Poissons ramassés : {0}/{1}", "Peces recogidos: {0}/{1}", "Pesci raccolti: {0}/{1}", "Zebrane ryby: {0}/{1}" },
            ["ending"] = new[] { "Frost Bay is free! On to the Glacier River.", "Vịnh Băng Giá đã được giải cứu! Tiếp tục tới Sông Băng.", "Die Frostbucht ist frei! Weiter zum Gletscherfluss.", "La Baie Gelée est libre ! En route vers la Rivière Glacée.", "¡La Bahía Helada es libre! Rumbo al Río Glaciar.", "La Baia Gelata è libera! Ora verso il Fiume Glaciale.", "Mroźna Zatoka jest wolna! Teraz nad Lodową Rzekę." },
            ["castle_freed"] = new[] { "{0} is free! On to the next castle.", "{0} đã được giải cứu! Tiến tới thành tiếp theo.", "{0} ist frei! Weiter zur nächsten Burg.", "{0} : libération ! En route vers le château suivant.", "¡{0} es libre! Al siguiente castillo.", "{0}: liberazione! Avanti al prossimo castello.", "{0}: wolność! Ruszamy do następnego zamku." },   // fr/it/pl: no adjective, the castle names differ in gender
            ["drop_tip"] = new[] { "No rope! Fire as the pup drops past the catapult", "Không dây! Bắn lúc hải cẩu rơi ngang qua máy bắn", "Kein Seil! Feuere, wenn die Robbe am Katapult vorbeifällt", "Pas de corde ! Tire quand le phoque passe devant la catapulte", "¡Sin cuerda! Dispara cuando la foca caiga junto a la catapulta", "Niente corda! Spara quando la foca cade davanti alla catapulta", "Bez liny! Strzelaj, gdy foka spada obok katapulty" },
            ["cage_freed"] = new[] { "The caged pups are free! On along the trail.", "Các bạn hải cẩu trong lồng đã được thả! Đi tiếp thôi.", "Die gefangenen Robben sind frei! Weiter des Weges.", "Les phoques en cage sont libres ! On continue.", "¡Las focas enjauladas son libres! A seguir.", "Le foche in gabbia sono libere! Avanti.", "Foki z klatki są wolne! Ruszamy dalej." },
            ["world_canyon"] = new[] { "Glacier Canyon", "Hẻm Núi Băng", "Gletscherschlucht", "Canyon glacé", "Cañón Glaciar", "Canyon di ghiaccio", "Lodowy Kanion" },
            ["world_forest"] = new[] { "Aurora Forest", "Rừng Cực Quang", "Polarlichtwald", "Forêt boréale", "Bosque Aurora", "Foresta dell'aurora", "Las Zorzy" },
            ["world_harbour"] = new[] { "Frozen Harbour", "Cảng Đóng Băng", "Eishafen", "Port gelé", "Puerto Helado", "Porto ghiacciato", "Zamarznięty Port" },
            ["world_caves"] = new[] { "Crystal Caves", "Hang Pha Lê", "Kristallhöhlen", "Grottes de cristal", "Cuevas de Cristal", "Grotte di cristallo", "Kryształowe Jaskinie" },
            ["world_volcano"] = new[] { "Fire Peak", "Núi Lửa Băng", "Feuergipfel", "Pic de feu", "Pico de Fuego", "Picco di fuoco", "Ognisty Szczyt" },
            ["world_blossom"] = new[] { "Blossom Valley", "Thung Lũng Hoa", "Blütental", "Vallée fleurie", "Valle Florido", "Valle dei fiori", "Kwitnąca Dolina" },
            ["world_palace"] = new[] { "Starlight Palace", "Cung Điện Sao", "Sternenpalast", "Palais étoilé", "Palacio Estelar", "Palazzo stellato", "Gwiezdny Pałac" },
            ["world_tundra"] = new[] { "Amber Tundra", "Lãnh Nguyên Hổ Phách", "Bernsteintundra", "Toundra ambrée", "Tundra Ámbar", "Tundra d'ambra", "Bursztynowa Tundra" },
            ["world_throne"] = new[] { "Bear King's Throne", "Ngai Vua Gấu", "Thron des Bärenkönigs", "Trône du Roi Ours", "Trono del Rey Oso", "Trono del Re Orso", "Tron Króla Niedźwiedzi" },
            ["star_rule_1"] = new[] { "Mission passed", "Qua màn", "Mission geschafft", "Mission réussie", "Misión superada", "Missione superata", "Misja zaliczona" },
            ["star_rule_2"] = new[] { "All bears down, a pup to spare", "Hạ hết gấu khi còn dư hải cẩu", "Alle Bären besiegt, eine Robbe übrig", "Tous les ours assommés, un phoque en réserve", "Todos los osos derribados y una foca de sobra", "Tutti gli orsi abbattuti, una foca di scorta", "Wszystkie niedźwiedzie pokonane, foka w zapasie" },
            ["star_rule_3"] = new[] { "60% of the fortress smashed", "Phá 60% pháo đài", "60 % der Festung zerstört", "60 % de la forteresse détruite", "60 % de la fortaleza destruida", "60% della fortezza distrutto", "60% twierdzy rozbite" },
            ["star_fish_1"] = new[] { "Run finished", "Hoàn thành", "Runde geschafft", "Manche terminée", "Ronda completada", "Giro completato", "Runda ukończona" },
            ["star_fish_2"] = new[] { "Half of the fish", "Gom nửa số cá", "Die Hälfte der Fische", "La moitié des poissons", "La mitad de los peces", "Metà dei pesci", "Połowa ryb" },
            ["star_fish_3"] = new[] { "80% of the fish", "Gom 80% số cá", "80 % der Fische", "80 % des poissons", "El 80 % de los peces", "L'80% dei pesci", "80% ryb" },
            ["retry_attack"] = new[] { "Attack again", "Tấn công lại", "Nochmal angreifen", "Réattaquer", "Atacar de nuevo", "Attacca di nuovo", "Atakuj ponownie" },
            ["star_boss_2"] = new[] { "Boss down with a pup to spare", "Hạ trùm khi còn dư hải cẩu", "Boss besiegt, eine Robbe übrig", "Boss vaincu, un phoque en réserve", "Jefe derrotado y una foca de sobra", "Boss sconfitto, una foca di scorta", "Boss pokonany, foka w zapasie" },
            ["loading"] = new[] { "Loading…", "Đang tải…", "Lädt…", "Chargement…", "Cargando…", "Caricamento…", "Ładowanie…" },
            ["play_level"] = new[] { "Play · Level {0}", "Chơi · Màn {0}", "Spielen · Level {0}", "Jouer · Niveau {0}", "Jugar · Nivel {0}", "Gioca · Livello {0}", "Graj · Poziom {0}" },
            // Singular forms: the shop names the real number, and level 1 of these gives exactly one.
            ["upd_Spiky_one"]  = new[] { "Smash through 1 ice block", "Xuyên 1 khối băng", "Durchbricht 1 Eisblock", "Traverse 1 bloc de glace", "Atraviesa 1 bloque de hielo", "Sfonda 1 blocco di ghiaccio", "Przebija bloki lodu: 1" },
            ["upd_PuffUp_one"] = new[] { "+1 bounce after impact", "+1 lần nảy sau va chạm", "+1 Sprung nach dem Aufprall", "+1 rebond après l'impact", "+1 rebote tras el impacto", "+1 rimbalzo dopo l'impatto", "Odbicia po uderzeniu: +1" },
            ["upd_School_one"] = new[] { "+1 pup per level", "+1 hải cẩu mỗi màn", "+1 Robbe pro Level", "+1 phoque par niveau", "+1 foca por nivel", "+1 foca per livello", "Foki na poziom: +1" },
            ["upd_Blaze_all"]  = new[] { "Tap again mid-dive to catch fire and melt through every ice cube", "Chạm lần nữa khi đang lao để bốc lửa, xuyên mọi khối băng",
                                         "Tippe im Sturzflug erneut: Feuer fangen und alle Eiswürfel schmelzen", "Touche encore en plongée pour t'enflammer et faire fondre tous les cubes de glace", "Toca otra vez en plena caída para arder y derretir todos los cubos de hielo", "Tocca di nuovo in picchiata per prendere fuoco e sciogliere tutti i cubetti di ghiaccio", "Stuknij ponownie w locie, by zapłonąć i stopić wszystkie kostki lodu" },
            // The shop's caption: upgrade name · level: what it does (French puts a space before the colon).
            ["shop_line"]      = new[] { "{0}: {1}", "{0}: {1}", "{0}: {1}", "{0} : {1}", "{0}: {1}", "{0}: {1}", "{0}: {1}" },   // level: effect
            ["beat_boss"]      = new[] { "Beat the boss bear to continue", "Hạ gấu trùm để đi tiếp", "Besiege den Boss-Bären, um weiterzukommen", "Bats l'ours boss pour continuer", "Derrota al oso jefe para continuar", "Sconfiggi l'orso boss per continuare", "Pokonaj niedźwiedzia bossa, by przejść dalej" },
            ["save_failed"]    = new[] { "Could not save your progress. Free up some storage.", "Không lưu được tiến trình. Hãy giải phóng bộ nhớ.", "Fortschritt konnte nicht gespeichert werden. Gib Speicher frei.", "Impossible d'enregistrer ta progression. Libère de l'espace.", "No se pudo guardar tu progreso. Libera espacio.", "Impossibile salvare i progressi. Libera spazio.", "Nie udało się zapisać postępu. Zwolnij miejsce." },
            ["save_damaged"]   = new[] { "The save file was damaged, so progress starts over.", "Tệp lưu bị hỏng nên tiến trình bắt đầu lại.", "Der Spielstand war beschädigt, der Fortschritt beginnt neu.", "La sauvegarde était endommagée : la progression recommence.", "La partida guardada estaba dañada: el progreso empieza de nuevo.", "Il salvataggio era danneggiato: i progressi ricominciano.", "Zapis gry był uszkodzony, postęp zaczyna się od nowa." },
        };

        /// <summary>Language codes in table-column order; also the values stored in the save.</summary>
        public static readonly string[] Codes = { "en", "vi", "de", "fr", "es", "it", "pl" };

        static int lang = -1;

        /// <summary>Editor play mode without a domain reload keeps statics: read the saved language again.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => lang = -1;

        public static string Language => Codes[Index];

        static int Index
        {
            get
            {
                if (lang < 0)
                {
                    string saved = SaveSystem.Data.language;
                    lang = string.IsNullOrEmpty(saved) ? FromSystem(Application.systemLanguage) : Math.Max(0, Array.IndexOf(Codes, saved));
                }
                return lang;
            }
        }

        static int FromSystem(SystemLanguage system) => system switch
        {
            SystemLanguage.Vietnamese => 1,
            SystemLanguage.German => 2,
            SystemLanguage.French => 3,
            SystemLanguage.Spanish => 4,
            SystemLanguage.Italian => 5,
            SystemLanguage.Polish => 6,
            _ => 0,
        };

        /// <summary>Each language's own name ("Deutsch", "Tiếng Việt"), for the language picker in Settings.</summary>
        public static string NameOf(string code) => table["lang_name"][Math.Max(0, Array.IndexOf(Codes, code))];

        public static void SetLanguage(string code)
        {
            lang = Math.Max(0, Array.IndexOf(Codes, code));
            SaveSystem.Data.language = Language;
            SaveSystem.Save();
            Changed?.Invoke();
        }

        /// <summary>True when the table has <paramref name="key"/> (an optional variant, e.g. a singular form).</summary>
        public static bool Has(string key) => table.ContainsKey(key);

        /// <summary>Every key with a non-empty text per language, and the same {0} {1}… in each of them: a translation
        /// that drops a placeholder loses its number, one that adds one throws in string.Format (EditMode test).</summary>
        public static bool IsComplete(out string problem)
        {
            foreach (var row in table)
            {
                if (row.Value.Length != Codes.Length) { problem = $"{row.Key}: {row.Value.Length} columns"; return false; }
                for (int i = 0; i < Codes.Length; i++)
                {
                    if (string.IsNullOrEmpty(row.Value[i])) { problem = $"{row.Key}: empty {Codes[i]}"; return false; }
                    for (int n = 0; n < 4; n++)
                        if (row.Value[i].Contains("{" + n + "}") != row.Value[0].Contains("{" + n + "}"))
                        { problem = $"{row.Key}: placeholder {{{n}}} differs in {Codes[i]}"; return false; }
                }
            }
            problem = null;
            return true;
        }

        public static string T(string key)
        {
            if (table.TryGetValue(key, out var row)) return row[Index];
            Debug.LogWarning($"[SealCatapult] Missing text key '{key}'");
            return key;
        }

        public static string T(string key, params object[] args) => string.Format(T(key), args);
    }
}
