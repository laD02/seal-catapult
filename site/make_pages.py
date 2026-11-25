"""Writes site/index.html (en, x-default) and site/<lang>/index.html for vi, de, fr, es, it, pl, plus site/sitemap.xml.

Text follows docs/ASO.md §3 (names, keywords). Edit the PAGES table, then run: python3 -I site/make_pages.py
Publish everything in site/ except this script (docs/ASO.md §7).
"""
import html
import json
import pathlib

ROOT = "https://lad02.github.io/seal-catapult/"
PLAY = "https://play.google.com/store/apps/details?id=com.sealcatapult.game"
EMAIL = "lehoang10112002@gmail.com"
SITE = pathlib.Path(__file__).resolve().parent
UPDATED = "2026-11-16"

PAGES = {
    "en": dict(
        name="Seal Catapult: Polar Bear Game",
        title="Seal Catapult: Polar Bear Catapult Game for Android",
        short="Launch seal pups from a catapult, dive onto polar bears and smash the ice fort.",
        desc="Launch seal pups from a catapult, dive onto polar bears and smash the ice fortress. A one-tap physics arcade game for Android: 18 levels, a boss, upgrades, plays offline.",
        tagline="Launch seal pups, knock out the polar bears",
        get="Get it on Google Play",
        h2="A one-tap seal catapult game",
        story="The polar bears stole the seal village's winter fish and locked the pups in cages. Time your tap to whack each swinging pup up through the ice gap, then tap again to send it diving onto the bears' fortress.",
        items=[
            "Two phases in every level: launch from the catapult, then dive onto the polar bears",
            "Bounce from bear to bear for combos, break ice and snow blocks so the bears tumble",
            "18 levels in two areas: Frost Bay (10 fortresses, a bonus fish level, a boss with caged pups) and the Glacier River (6 harder fortresses)",
            "3 stars and a Perfect badge per level, 5 seal upgrades bought with stars",
            "50+ skins bought with stars: fur colours, outfits, balls, trails, impact effects, catapult paint and ropes",
            "Hand-painted cartoon art, quick levels of about a minute",
            "Plays offline, no account; English, German, French, Spanish, Italian, Polish and Vietnamese",
        ],
        ads="Free to play with ads: an occasional ad between levels and optional rewarded ads.",
        privacy="Privacy Policy", contact="Contact", langs="Languages",
        icon_alt="Seal Catapult: a seal pup curled into a ball",
        og="en_US",
    ),
    "vi": dict(
        name="Hải Cẩu Bay: Game Bắn Gấu Băng",
        title="Hải Cẩu Bay: Game Bắn Gấu Băng cho Android",
        short="Đập hải cẩu bay qua khe băng, lao xuống hạ gấu bắc cực. Game bắn không cần mạng.",
        desc="Hải Cẩu Bay (Seal Catapult): game bắn một chạm cho Android. Phóng hải cẩu con bằng máy bắn, lao xuống hạ gấu bắc cực, phá pháo đài băng. 18 màn, boss, nâng cấp, chơi không cần mạng.",
        tagline="Phóng hải cẩu, hạ gấu bắc cực",
        get="Tải trên Google Play",
        h2="Hải Cẩu Bay – game bắn gấu băng",
        story="Bầy gấu bắc cực cướp kho cá mùa đông và nhốt bầy hải cẩu con vào lồng. Chạm đúng lúc để máy bắn đập hải cẩu đang đu dây bay vọt qua khe băng, rồi chạm lần nữa để nó lao xuống pháo đài của gấu.",
        items=[
            "Mỗi màn hai pha: phóng từ máy bắn, rồi lao xuống hạ gấu bắc cực",
            "Nảy từ gấu này sang gấu khác để ghi combo, phá khối băng và khối tuyết cho gấu lộn nhào",
            "18 màn ở hai vùng: Vịnh Băng Giá (10 màn pháo đài, một màn thưởng nhặt cá, một màn boss có lồng nhốt hải cẩu) và Sông Băng (6 pháo đài khó hơn)",
            "3 sao và huy hiệu Hoàn hảo mỗi màn, 5 nâng cấp hải cẩu mua bằng sao",
            "50+ skin mua bằng sao: màu lông, trang phục, bóng, vệt bay, hiệu ứng va chạm, màu máy phóng và dây đu",
            "Tranh hoạt hình vẽ tay, mỗi màn khoảng một phút",
            "Chơi không cần mạng, không cần tài khoản; tiếng Việt, Anh, Đức, Pháp, Tây Ban Nha, Ý và Ba Lan",
        ],
        ads="Chơi miễn phí, có quảng cáo: thỉnh thoảng một quảng cáo giữa các màn và quảng cáo có thưởng tự chọn.",
        privacy="Chính sách quyền riêng tư", contact="Liên hệ", langs="Ngôn ngữ",
        icon_alt="Seal Catapult: hải cẩu con cuộn tròn",
        og="vi_VN",
    ),
    "de": dict(
        name="Seal Catapult: Robben-Katapult",
        title="Seal Catapult: Robben-Katapult – Eisbär-Spiel für Android",
        short="Robben per Katapult starten, auf Eisbären stürzen und die Eisburg zerschlagen.",
        desc="Robben per Katapult starten, im Sturzflug auf Eisbären stürzen und die Eisburg zerschlagen. Ein Physik-Arcadespiel für einen Finger auf Android: 18 Level, ein Boss, Upgrades, offline spielbar.",
        tagline="Robben starten, Eisbären umhauen",
        get="Jetzt bei Google Play",
        h2="Ein Katapult-Spiel mit Robben, für einen Finger",
        story="Die Eisbären haben den Fischvorrat des Robbendorfs gestohlen und die Robbenbabys in Käfige gesperrt. Tippe im richtigen Moment, damit die schwingende Robbe durch die Eisspalte fliegt, und tippe noch einmal für den Sturzflug auf die Festung der Eisbären.",
        items=[
            "Zwei Phasen pro Level: Abschuss mit dem Katapult, dann Sturzflug auf die Eisbären",
            "Von Bär zu Bär springen für Kombos, Eis- und Schneeblöcke zerschlagen",
            "18 Level in zwei Gebieten: die Frostbucht (10 Festungslevel, ein Bonuslevel mit Fischen, ein Bosslevel mit gefangenen Robbenbabys) und der Gletscherfluss (6 schwerere Festungen)",
            "3 Sterne und ein Perfekt-Abzeichen pro Level, 5 Robben-Upgrades für Sterne",
            "50+ Skins für Sterne: Fellfarben, Outfits, Bälle, Flugspuren, Treffereffekte, Katapultfarben und Seile",
            "Handgemalte Cartoon-Grafik, kurze Level von etwa einer Minute",
            "Offline spielbar, kein Konto; Deutsch, Englisch, Französisch, Spanisch, Italienisch, Polnisch und Vietnamesisch",
        ],
        ads="Kostenlos spielbar, mit Werbung: gelegentlich eine Anzeige zwischen den Leveln und freiwillige Belohnungsvideos.",
        privacy="Datenschutzerklärung", contact="Kontakt", langs="Sprachen",
        icon_alt="Seal Catapult: ein zusammengerolltes Robbenbaby",
        og="de_DE",
    ),
    "fr": dict(
        name="Seal Catapult: Phoques et Ours",
        title="Seal Catapult : phoques contre ours polaires, jeu Android",
        short="Lance des phoques à la catapulte, plonge sur les ours polaires, brise la glace.",
        desc="Lance des bébés phoques à la catapulte, plonge sur les ours polaires et brise leur forteresse de glace. Un jeu d'arcade physique à un doigt sur Android : 18 niveaux, un boss, des améliorations, hors ligne.",
        tagline="Lance les phoques, mets K.O. les ours polaires",
        get="Disponible sur Google Play",
        h2="Un jeu de catapulte à phoques, à un seul doigt",
        story="Les ours polaires ont volé le poisson d'hiver du village des phoques et enfermé les bébés phoques en cage. Touche au bon moment pour envoyer chaque phoque à travers la faille de glace, puis touche encore pour le faire plonger sur la forteresse des ours.",
        items=[
            "Deux phases par niveau : lancer à la catapulte, puis plonger sur les ours polaires",
            "Rebondis d'ours en ours pour enchaîner les combos, brise les blocs de glace et de neige",
            "18 niveaux dans deux zones : la Baie Gelée (10 forteresses, un niveau bonus de poissons, un boss avec des bébés phoques en cage) et la Rivière Glacée (6 forteresses plus dures)",
            "3 étoiles et un badge Parfait par niveau, 5 améliorations achetées avec des étoiles",
            "50+ skins à acheter avec des étoiles : pelages, tenues, balles, traînées, effets d'impact, peintures de catapulte et cordes",
            "Dessins peints à la main, niveaux rapides d'environ une minute",
            "Fonctionne hors ligne, sans compte ; français, anglais, allemand, espagnol, italien, polonais et vietnamien",
        ],
        ads="Gratuit avec des publicités : une publicité de temps en temps entre les niveaux et des vidéos récompensées au choix.",
        privacy="Politique de confidentialité", contact="Contact", langs="Langues",
        icon_alt="Seal Catapult : un bébé phoque roulé en boule",
        og="fr_FR",
    ),
    "es": dict(
        name="Seal Catapult: Focas y Osos",
        title="Seal Catapult: focas contra osos polares, juego Android",
        short="Lanza focas con la catapulta, cae sobre los osos polares y rompe el hielo.",
        desc="Lanza crías de foca con la catapulta, cae en picado sobre los osos polares y rompe su fortaleza de hielo. Un juego arcade de física con un solo dedo para Android: 18 niveles, un jefe, mejoras, sin internet.",
        tagline="Lanza focas, tumba a los osos polares",
        get="Disponible en Google Play",
        h2="Un juego de catapulta de focas con un solo dedo",
        story="Los osos polares han robado el pescado del invierno de la aldea de las focas y han encerrado a las crías. Toca en el momento justo para que cada foca salga volando por la grieta del hielo, y vuelve a tocar para que caiga en picado sobre la fortaleza de los osos.",
        items=[
            "Dos fases en cada nivel: lanzar con la catapulta y caer en picado sobre los osos polares",
            "Rebota de oso en oso para hacer combos, rompe bloques de hielo y nieve",
            "18 niveles en dos zonas: la Bahía Helada (10 fortalezas, un nivel extra de peces, un jefe con crías enjauladas) y el Río Glaciar (6 fortalezas más difíciles)",
            "3 estrellas y una insignia Perfecto por nivel, 5 mejoras que se compran con estrellas",
            "50+ aspectos que se compran con estrellas: pelajes, atuendos, bolas, estelas, efectos de impacto, pintura de catapulta y cuerdas",
            "Dibujos pintados a mano, niveles rápidos de un minuto más o menos",
            "Funciona sin internet, sin cuenta; español, inglés, alemán, francés, italiano, polaco y vietnamita",
        ],
        ads="Gratis con anuncios: de vez en cuando un anuncio entre niveles y vídeos con recompensa opcionales.",
        privacy="Política de privacidad", contact="Contacto", langs="Idiomas",
        icon_alt="Seal Catapult: una cría de foca hecha una bola",
        og="es_ES",
    ),
    "it": dict(
        name="Seal Catapult: Foche e Orsi",
        title="Seal Catapult: foche contro orsi polari, gioco Android",
        short="Lancia foche con la catapulta, piomba sugli orsi polari e rompi il ghiaccio.",
        desc="Lancia cuccioli di foca con la catapulta, piomba in picchiata sugli orsi polari e distruggi la fortezza di ghiaccio. Un gioco arcade di fisica con un dito per Android: 18 livelli, un boss, potenziamenti, offline.",
        tagline="Lancia le foche, metti KO gli orsi polari",
        get="Disponibile su Google Play",
        h2="Un gioco di catapulta con le foche, con un dito solo",
        story="Gli orsi polari hanno rubato il pesce per l'inverno al villaggio delle foche e hanno chiuso i cuccioli in gabbia. Tocca al momento giusto per far volare ogni foca attraverso la fessura nel ghiaccio, poi tocca di nuovo per farla piombare sulla fortezza degli orsi.",
        items=[
            "Due fasi in ogni livello: lancio con la catapulta, poi picchiata sugli orsi polari",
            "Rimbalza da un orso all'altro per fare combo, rompi blocchi di ghiaccio e neve",
            "18 livelli in due zone: la Baia Gelata (10 fortezze, un livello bonus con i pesci, un boss con cuccioli in gabbia) e il Fiume Glaciale (6 fortezze più difficili)",
            "3 stelle e un distintivo Perfetto per livello, 5 potenziamenti da comprare con le stelle",
            "50+ skin da comprare con le stelle: pellicce, completi, palle, scie, effetti d'impatto, vernici per la catapulta e corde",
            "Grafica dipinta a mano, livelli veloci di circa un minuto",
            "Si gioca offline, senza account; italiano, inglese, tedesco, francese, spagnolo, polacco e vietnamita",
        ],
        ads="Gratis con pubblicità: ogni tanto un annuncio tra i livelli e video con premio facoltativi.",
        privacy="Informativa sulla privacy", contact="Contatti", langs="Lingue",
        icon_alt="Seal Catapult: un cucciolo di foca raggomitolato",
        og="it_IT",
    ),
    "pl": dict(
        name="Seal Catapult: Foki i Misie",
        title="Seal Catapult: foki kontra misie polarne – gra na Androida",
        short="Wystrzel foki z katapulty, zanurkuj na niedźwiedzie polarne i rozbij lód.",
        desc="Wystrzel małe foki z katapulty, zanurkuj na niedźwiedzie polarne i rozbij lodową fortecę. Zręcznościowa gra z fizyką na jeden palec na Androida: 18 poziomów, boss, ulepszenia, działa offline.",
        tagline="Wystrzel foki, znokautuj niedźwiedzie polarne",
        get="Pobierz z Google Play",
        h2="Gra z katapultą i fokami na jeden palec",
        story="Niedźwiedzie polarne ukradły zimowy zapas ryb z wioski fok i zamknęły małe foki w klatkach. Dotknij we właściwej chwili, aby foka przeleciała przez szczelinę w lodzie, a potem dotknij jeszcze raz, by zanurkowała na fortecę niedźwiedzi.",
        items=[
            "Dwie fazy w każdym poziomie: start z katapulty, potem nurkowanie na niedźwiedzie polarne",
            "Odbijaj się od niedźwiedzia do niedźwiedzia po combo, rozbijaj bloki lodu i śniegu",
            "18 poziomów w dwóch krainach: Mroźna Zatoka (10 fortec, poziom bonusowy z rybami, boss z fokami w klatkach) oraz Lodowa Rzeka (6 trudniejszych fortec)",
            "3 gwiazdki i odznaka Perfekcyjnie na poziom, 5 ulepszeń za gwiazdki",
            "50+ skórki za gwiazdki: kolory futra, stroje, kule, smugi, efekty uderzeń, farby katapulty i liny",
            "Ręcznie malowana grafika, krótkie poziomy po około minucie",
            "Działa offline, bez konta; polski, angielski, niemiecki, francuski, hiszpański, włoski i wietnamski",
        ],
        ads="Darmowa gra z reklamami: od czasu do czasu reklama między poziomami i opcjonalne filmy z nagrodą.",
        privacy="Polityka prywatności", contact="Kontakt", langs="Języki",
        icon_alt="Seal Catapult: zwinięta w kłębek mała foka",
        og="pl_PL",
    ),
}
NATIVE = {"en": "English", "vi": "Tiếng Việt", "de": "Deutsch", "fr": "Français", "es": "Español", "it": "Italiano", "pl": "Polski"}


def url(lang):
    return ROOT if lang == "en" else f"{ROOT}{lang}/"


def page(lang, p):
    up = "" if lang == "en" else "../"
    e = html.escape
    alternates = "\n".join(f'<link rel="alternate" hreflang="{l}" href="{url(l)}">' for l in PAGES)
    ld = {
        "@context": "https://schema.org", "@type": "MobileApplication", "name": p["name"],
        "alternateName": sorted({q["name"] for q in PAGES.values()} - {p["name"]}) + ["Seal Catapult", "Hải Cẩu Bay"],
        "description": p["short"], "operatingSystem": "ANDROID", "applicationCategory": "GameApplication",
        "applicationSubCategory": "Arcade", "inLanguage": list(PAGES), "image": f"{ROOT}icon.png", "url": url(lang),
        "installUrl": PLAY, "offers": {"@type": "Offer", "price": "0", "priceCurrency": "EUR"},
    }
    others = " · ".join(f'<a href="{up}{"" if l == "en" else l + "/"}" hreflang="{l}" lang="{l}">{NATIVE[l]}</a>' for l in PAGES if l != lang)
    items = "\n".join(f"      <li>{e(i)}</li>" for i in p["items"])
    return f"""<!doctype html>
<html lang="{lang}">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<!-- Generated by site/make_pages.py from docs/ASO.md §3. Edit the script, not this file. -->
<title>{e(p["title"])}</title>
<meta name="description" content="{e(p["desc"])}">
<link rel="canonical" href="{url(lang)}">
{alternates}
<link rel="alternate" hreflang="x-default" href="{ROOT}">
<meta name="theme-color" content="#11285a">
<meta property="og:type" content="website">
<meta property="og:url" content="{url(lang)}">
<meta property="og:title" content="{e(p["name"])}">
<meta property="og:description" content="{e(p["short"])}">
<meta property="og:image" content="{ROOT}icon.png">
<meta property="og:locale" content="{p["og"]}">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{e(p["name"])}">
<meta name="twitter:description" content="{e(p["short"])}">
<meta name="twitter:image" content="{ROOT}icon.png">
<link rel="icon" href="{up}icon.png">
<script type="application/ld+json">
{json.dumps(ld, ensure_ascii=False, indent=2)}
</script>
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Baloo+2:wght@500;800&display=swap">
<style>
  :root {{ --bg: #11285a; --sky: #1d4a8c; --panel: #1a3a73; --text: #ffffff; --muted: #cfe3ff; --accent: #ff8a3d; --ice: #8fe3ff; }}
  body {{ margin: 0; min-height: 100vh; background: linear-gradient(180deg, var(--sky), var(--bg)) fixed, var(--bg); color: var(--text); font: 500 17px/1.6 'Baloo 2', system-ui, sans-serif; }}
  main {{ max-width: 640px; margin: 0 auto; padding: 48px 16px; text-align: center; }}
  img.icon {{ width: 112px; height: auto; }}
  h1 {{ font-size: 44px; font-weight: 800; line-height: 1.1; margin: 16px 0 4px; color: var(--text); text-shadow: 0 4px 0 var(--accent); }}
  h2 {{ font-size: 20px; margin: 24px 0 8px; }}
  p {{ color: var(--muted); }}
  ul {{ text-align: left; display: inline-block; margin: 0; padding-left: 20px; }}
  a {{ color: var(--ice); overflow-wrap: anywhere; }}
  nav a {{ display: inline-block; padding: 12px 4px; }}
  a.play {{ display: inline-block; margin: 24px 0 8px; padding: 14px 28px; min-height: 48px; box-sizing: border-box; border-radius: 28px; background: linear-gradient(180deg, #ffb27a, var(--accent) 75%); color: #3a1a05; font-weight: 800; text-decoration: none; box-shadow: inset 0 3px 0 rgba(255,255,255,.4), 0 5px 0 #a24a12; }}
  section {{ background: linear-gradient(180deg, #24478a, var(--panel) 75%); box-shadow: inset 0 2px 0 rgba(255,255,255,.18), 0 5px 0 #0a1a3d; border-radius: 24px; padding: 8px 20px 20px; margin-top: 24px; }}
</style>
</head>
<body>
<main>
  <img class="icon" src="{up}icon.png" alt="{e(p["icon_alt"])}" width="112" height="105">
  <h1>Seal Catapult</h1>
  <p>{e(p["tagline"])}</p>
  <a class="play" href="{PLAY}">{e(p["get"])}</a>

  <section>
    <h2>{e(p["h2"])}</h2>
    <p>{e(p["story"])}</p>
    <ul>
{items}
    </ul>
  </section>

  <p>{e(p["ads"])}</p>
  <nav aria-label="{e(p["langs"])}">{others}</nav>
  <p><a href="{up}privacy.html">{e(p["privacy"])}</a></p>
  <p>{e(p["contact"])}: <a href="mailto:{EMAIL}">{EMAIL}</a></p>
</main>
</body>
</html>
"""


def main():
    for lang, p in PAGES.items():
        out = SITE / "index.html" if lang == "en" else SITE / lang / "index.html"
        out.parent.mkdir(parents=True, exist_ok=True)
        out.write_text(page(lang, p), encoding="utf-8")
    urls = [url(l) for l in PAGES] + [f"{ROOT}privacy.html"]
    body = "\n".join(f"  <url><loc>{u}</loc><lastmod>{UPDATED}</lastmod></url>" for u in urls)
    (SITE / "sitemap.xml").write_text(
        f'<?xml version="1.0" encoding="UTF-8"?>\n<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n{body}\n</urlset>\n',
        encoding="utf-8")
    print(f"wrote {len(PAGES)} pages + sitemap ({len(urls)} urls)")


if __name__ == "__main__":
    main()
