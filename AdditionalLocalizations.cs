using System.Collections.Generic;
namespace UnifromCheat_REPO
{
    public partial class TooltipsLanguages
    {
        public static readonly (int Id, string NativeName)[] LanguageOptions =
        {
            (0, "English"),
            (1, "Русский"),
            (2, "Українська"),
            (3, "简体中文"),
            (4, "日本語"),
            (5, "한국어"),
            (6, "Polski"),
            (7, "Español"),
            (8, "Français"),
            (9, "العربية"),
            (10, "Српски"),
            (11, "Беларуская"),
            (12, "Türkçe")
        };

        private static readonly Dictionary<int, Dictionary<string, string>> AdditionalTooltips = new()
        {
            [1] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Переводит весь интерфейс меню."
            },
            [2] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Перекладає весь інтерфейс меню."
            },
            [3] = new Dictionary<string, string>
            {
                ["translateMenu"] = "翻译整个菜单界面。"
            },
            [4] = new Dictionary<string, string>
            {
                ["translateMenu"] = "メニュー全体のインターフェースを翻訳します。",
                ["hideAllHints"] = "画面上の固定ヒントをすべて非表示にします",
                ["hideAllTooltips"] = "ポップアップヒントをすべて非表示にします",
                ["dragWindow"] = "メニューウィンドウをドラッグできるようにします",
                ["freecam"] = "観戦中に自由カメラを使えます",
                ["godMode"] = "プレイヤーを無敵にします",
                ["infSprint"] = "スタミナを無限にします",
                ["noclip"] = "壁やオブジェクトを通り抜けて移動できます",
                ["protectedSession"] = "敗北後のセーブ削除を防ぎます",
                ["valuablesTp"] = "貴重品をプレイヤー、回収地点、カートへ転送します",
                ["enemiesTp"] = "敵をプレイヤー、回収地点、奈落へ転送します"
            },
            [5] = new Dictionary<string, string>
            {
                ["translateMenu"] = "메뉴 인터페이스 전체를 번역합니다.",
                ["hideAllHints"] = "화면의 고정 힌트를 모두 숨깁니다",
                ["hideAllTooltips"] = "팝업 툴팁을 모두 숨깁니다",
                ["dragWindow"] = "메뉴 창을 드래그할 수 있게 합니다",
                ["freecam"] = "관전 중 자유 카메라를 사용합니다",
                ["godMode"] = "플레이어를 무적으로 만듭니다",
                ["infSprint"] = "스태미나를 무한으로 만듭니다",
                ["noclip"] = "벽과 오브젝트를 통과해 이동합니다",
                ["protectedSession"] = "패배 후 저장 삭제를 막습니다",
                ["valuablesTp"] = "귀중품을 플레이어, 수집 지점 또는 카트로 순간이동합니다",
                ["enemiesTp"] = "적을 플레이어, 수집 지점 또는 공허로 순간이동합니다"
            },
            [6] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Tłumaczy cały interfejs menu.",
                ["hideAllHints"] = "Ukrywa wszystkie stałe podpowiedzi",
                ["hideAllTooltips"] = "Ukrywa wszystkie wyskakujące podpowiedzi",
                ["dragWindow"] = "Pozwala przeciągać okno menu",
                ["freecam"] = "Tryb swobodnej kamery podczas obserwacji",
                ["godMode"] = "Gracz jest nieśmiertelny",
                ["infSprint"] = "Daje graczowi nieskończoną wytrzymałość",
                ["noclip"] = "Pozwala przelatywać przez ściany",
                ["protectedSession"] = "Chroni zapis przed usunięciem po przegranej",
                ["valuablesTp"] = "Teleportuje kosztowności do gracza, punktu odbioru lub wózka",
                ["enemiesTp"] = "Teleportuje wrogów do gracza, punktów odbioru lub w pustkę"
            },
            [7] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Traduce toda la interfaz del menú.",
                ["hideAllHints"] = "Oculta todas las pistas fijas",
                ["hideAllTooltips"] = "Oculta todas las ayudas emergentes",
                ["dragWindow"] = "Permite arrastrar la ventana del menú",
                ["freecam"] = "Cámara libre mientras observas",
                ["godMode"] = "Hace inmortal al jugador",
                ["infSprint"] = "Da resistencia infinita al jugador",
                ["noclip"] = "Permite atravesar paredes",
                ["protectedSession"] = "Evita borrar la partida guardada tras perder",
                ["valuablesTp"] = "Teletransporta objetos valiosos al jugador, punto de extracción o carrito",
                ["enemiesTp"] = "Teletransporta enemigos al jugador, puntos de extracción o al vacío"
            },
            [8] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Traduit toute l'interface du menu.",
                ["hideAllHints"] = "Masque toutes les indications fixes",
                ["hideAllTooltips"] = "Masque toutes les infobulles",
                ["dragWindow"] = "Autorise le déplacement de la fenêtre du menu",
                ["freecam"] = "Caméra libre pendant le mode spectateur",
                ["godMode"] = "Rend le joueur immortel",
                ["infSprint"] = "Donne une endurance infinie au joueur",
                ["noclip"] = "Permet de traverser les murs",
                ["protectedSession"] = "Empêche la suppression de la sauvegarde après une défaite",
                ["valuablesTp"] = "Téléporte les objets de valeur vers le joueur, le point d'extraction ou le chariot",
                ["enemiesTp"] = "Téléporte les ennemis vers le joueur, les points d'extraction ou le vide"
            },
            [9] = new Dictionary<string, string>
            {
                ["translateMenu"] = "يترجم واجهة القائمة بالكامل.",
                ["hideAllHints"] = "يخفي كل التلميحات الثابتة",
                ["hideAllTooltips"] = "يخفي كل التلميحات المنبثقة",
                ["dragWindow"] = "يسمح بسحب نافذة القائمة",
                ["freecam"] = "كاميرا حرة أثناء المشاهدة",
                ["godMode"] = "يجعل اللاعب غير قابل للموت",
                ["infSprint"] = "يمنح اللاعب طاقة ركض غير محدودة",
                ["noclip"] = "يسمح بالمرور عبر الجدران",
                ["protectedSession"] = "يمنع حذف الحفظ بعد الخسارة",
                ["valuablesTp"] = "ينقل الأشياء الثمينة إلى اللاعب أو نقطة الجمع أو العربة",
                ["enemiesTp"] = "ينقل الأعداء إلى اللاعب أو نقاط الجمع أو الفراغ"
            },
            [10] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Преводи цео интерфејс менија.",
                ["hideAllHints"] = "Сакрива све сталне савете",
                ["hideAllTooltips"] = "Сакрива све искачуће описе",
                ["dragWindow"] = "Дозвољава померање прозора менија",
                ["freecam"] = "Слободна камера док посматраш",
                ["godMode"] = "Чини играча бесмртним",
                ["infSprint"] = "Даје играчу бесконачну издржљивост",
                ["noclip"] = "Омогућава пролазак кроз зидове",
                ["protectedSession"] = "Спречава брисање снимка после пораза",
                ["valuablesTp"] = "Телепортује вредне предмете до играча, тачке сакупљања или колица",
                ["enemiesTp"] = "Телепортује непријатеље до играча, тачака сакупљања или у празнину"
            },
            [11] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Перакладае ўвесь інтэрфейс меню.",
                ["hideAllHints"] = "Хавае ўсе статычныя падказкі",
                ["hideAllTooltips"] = "Хавае ўсе ўсплывальныя падказкі",
                ["dragWindow"] = "Дазваляе перацягваць акно меню",
                ["freecam"] = "Свабодная камера падчас назірання",
                ["godMode"] = "Робіць гульца несмяротным",
                ["infSprint"] = "Дае гульцу бясконцую вынослівасць",
                ["noclip"] = "Дазваляе праходзіць праз сцены",
                ["protectedSession"] = "Прадухіляе выдаленне захавання пасля паразы",
                ["valuablesTp"] = "Тэлепартуе каштоўнасці да гульца, пункта збору або вазка",
                ["enemiesTp"] = "Тэлепартуе ворагаў да гульца, пунктаў збору або ў пустэчу"
            },
            [12] = new Dictionary<string, string>
            {
                ["translateMenu"] = "Tüm menü arayüzünü çevirir.",
                ["hideAllHints"] = "Tüm sabit ipuçlarını gizler",
                ["hideAllTooltips"] = "Tüm açılır ipuçlarını gizler",
                ["dragWindow"] = "Menü penceresinin sürüklenmesine izin verir",
                ["freecam"] = "İzlerken serbest kamera modu",
                ["godMode"] = "Oyuncuyu ölümsüz yapar",
                ["infSprint"] = "Oyuncuya sınırsız dayanıklılık verir",
                ["noclip"] = "Duvarların içinden geçmeyi sağlar",
                ["protectedSession"] = "Kaybettikten sonra kayıt silinmesini engeller",
                ["valuablesTp"] = "Değerli eşyaları oyuncuya, toplama noktasına veya arabaya ışınlar",
                ["enemiesTp"] = "Düşmanları oyuncuya, toplama noktalarına veya boşluğa ışınlar"
            }
        };

        private static readonly Dictionary<int, Dictionary<string, string>> UiText = new()
        {
            [1] = new Dictionary<string, string>
            {
                ["LANGUAGES"] = "ЯЗЫКИ",
                ["Translate menu"] = "Перевести меню",
                ["MENU SETTINGS"] = "НАСТРОЙКИ МЕНЮ",
                ["PLAYER"] = "ИГРОК",
                ["WALLHACK"] = "СКВОЗЬ СТЕНЫ",
                ["MISC"] = "РАЗНОЕ",
                ["HOST ONLY FUNCTIONS"] = "ФУНКЦИИ ХОСТА",
                ["Configs"] = "Конфиги",
                ["Save config"] = "Сохранить конфиг",
                ["Load config"] = "Загрузить конфиг",
                ["Reset config"] = "Сбросить конфиг"
            },
            [2] = new Dictionary<string, string>
            {
                ["LANGUAGES"] = "МОВИ",
                ["Translate menu"] = "Перекладати меню",
                ["MENU SETTINGS"] = "НАЛАШТУВАННЯ МЕНЮ",
                ["PLAYER"] = "ГРАВЕЦЬ",
                ["WALLHACK"] = "КРІЗЬ СТІНИ",
                ["MISC"] = "РІЗНЕ",
                ["HOST ONLY FUNCTIONS"] = "ФУНКЦІЇ ХОСТА",
                ["Configs"] = "Конфіги",
                ["Save config"] = "Зберегти конфіг",
                ["Load config"] = "Завантажити конфіг",
                ["Reset config"] = "Скинути конфіг"
            },
            [3] = new Dictionary<string, string>
            {
                ["LANGUAGES"] = "语言",
                ["Translate menu"] = "翻译菜单",
                ["MENU SETTINGS"] = "菜单设置",
                ["PLAYER"] = "玩家",
                ["WALLHACK"] = "透视",
                ["MISC"] = "杂项",
                ["HOST ONLY FUNCTIONS"] = "房主功能",
                ["Configs"] = "配置",
                ["Save config"] = "保存配置",
                ["Load config"] = "加载配置",
                ["Reset config"] = "重置配置"
            },
            [4] = MakeUi("言語", "メニューを翻訳", "メニュー設定", "プレイヤー", "ウォールハック", "その他", "ホスト専用機能", "設定", "設定を保存", "設定を読み込む", "設定をリセット"),
            [5] = MakeUi("언어", "메뉴 번역", "메뉴 설정", "플레이어", "월핵", "기타", "호스트 전용 기능", "설정", "설정 저장", "설정 불러오기", "설정 초기화"),
            [6] = MakeUi("JĘZYKI", "Tłumacz menu", "USTAWIENIA MENU", "GRACZ", "PRZEZ ŚCIANY", "RÓŻNE", "FUNKCJE HOSTA", "Konfiguracje", "Zapisz konfigurację", "Wczytaj konfigurację", "Resetuj konfigurację"),
            [7] = MakeUi("IDIOMAS", "Traducir menú", "AJUSTES DEL MENÚ", "JUGADOR", "A TRAVÉS DE MUROS", "VARIOS", "FUNCIONES DEL HOST", "Configuraciones", "Guardar config.", "Cargar config.", "Restablecer config."),
            [8] = MakeUi("LANGUES", "Traduire le menu", "PARAMÈTRES DU MENU", "JOUEUR", "À TRAVERS LES MURS", "DIVERS", "FONCTIONS HÔTE", "Configurations", "Sauver config.", "Charger config.", "Réinitialiser config."),
            [9] = MakeUi("اللغات", "ترجمة القائمة", "إعدادات القائمة", "اللاعب", "عبر الجدران", "متفرقات", "وظائف المضيف", "الإعدادات", "حفظ الإعدادات", "تحميل الإعدادات", "إعادة ضبط الإعدادات"),
            [10] = MakeUi("ЈЕЗИЦИ", "Преведи мени", "ПОДЕШАВАЊА МЕНИЈА", "ИГРАЧ", "КРОЗ ЗИДОВЕ", "РАЗНО", "ФУНКЦИЈЕ ХОСТА", "Конфигурације", "Сачувај конфиг", "Учитај конфиг", "Ресетуј конфиг"),
            [11] = MakeUi("МОВЫ", "Перакласці меню", "НАЛАДЫ МЕНЮ", "ГУЛЕЦ", "ПРАЗ СЦЕНЫ", "РОЗНАЕ", "ФУНКЦЫІ ХОСТА", "Канфігі", "Захаваць канфіг", "Загрузіць канфіг", "Скінуць канфіг"),
            [12] = MakeUi("DİLLER", "Menüyü çevir", "MENÜ AYARLARI", "OYUNCU", "DUVAR ARKASI", "DİĞER", "HOST İŞLEVLERİ", "Yapılandırmalar", "Yapılandırmayı kaydet", "Yapılandırmayı yükle", "Yapılandırmayı sıfırla")
        };

        private static Dictionary<string, string> MakeUi(string languages, string translateMenu, string menuSettings, string player, string wallhack, string misc, string hostOnly, string configs, string save, string load, string reset)
        {
            return new Dictionary<string, string>
            {
                ["LANGUAGES"] = languages,
                ["Translate menu"] = translateMenu,
                ["MENU SETTINGS"] = menuSettings,
                ["PLAYER"] = player,
                ["WALLHACK"] = wallhack,
                ["MISC"] = misc,
                ["HOST ONLY FUNCTIONS"] = hostOnly,
                ["Configs"] = configs,
                ["Save config"] = save,
                ["Load config"] = load,
                ["Reset config"] = reset
            };
        }

        public static string Ui(string text, bool force = false)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (!force && !Core.translateMenu)
                return text;

            if (TryTranslateMenuOrTooltip(Core.lg_state, text, out string value))
                return value;

            return text;
        }

        public static string UiBold(string text, bool force = false)
        {
            return "<b>" + Ui(text, force) + "</b>";
        }

        public static bool TryGetAdditionalTooltip(int language, string key, out string value)
        {
            value = null;
            return AdditionalTooltips.TryGetValue(language, out var pack) && pack.TryGetValue(key, out value);
        }

        public static bool TryTranslateMenuOrTooltip(int language, string text, out string value)
        {
            value = null;
            if (UiText.TryGetValue(language, out var pack) && pack.TryGetValue(text, out value))
                return true;

            if (AdditionalTooltips.TryGetValue(language, out var tooltips) && tooltips.TryGetValue(text, out value))
                return true;

            if (TryTranslateCommonUi(language, text, out value))
                return true;

            return false;
        }

        private static bool TryTranslateCommonUi(int language, string text, out string value)
        {
            value = null;
            int index = language - 4;
            if (index < 0 || index >= 9)
                return false;

            if (!CommonUi.TryGetValue(text, out var values) || index >= values.Length)
                return false;

            value = values[index];
            return true;
        }

        static TooltipsLanguages()
        {
            AddUi(1, new Dictionary<string, string>
            {
                ["Default"] = "По умолчанию",
                ["CUSTOM CURSOR"] = "СВОЙ КУРСОР",
                ["PROCEDURAL SNOWFALL"] = "ПРОЦЕДУРНЫЙ СНЕГОПАД",
                ["UPDATE"] = "ОБНОВИТЬ",
                ["RESET"] = "СБРОСИТЬ",
                ["SET"] = "УСТАНОВИТЬ",
                ["ADD"] = "ДОБАВИТЬ",
                ["CONFIRM"] = "ПОДТВЕРДИТЬ",
                ["CANCEL"] = "ОТМЕНА",
                ["UNLOAD CHEAT"] = "ВЫГРУЗИТЬ ЧИТ",
                ["OBJECT SPAWNER"] = "СПАВНЕР ОБЪЕКТОВ",
                ["GAME CONTROLLER"] = "КОНТРОЛЛЕР ИГРЫ",
                ["KILL ENEMIES"] = "УБИТЬ ВРАГОВ",
                ["RENDER SETTINGS"] = "НАСТРОЙКИ РЕНДЕРА",
                ["ITEM GLOW COLOR"] = "ЦВЕТ ПОДСВЕТКИ ПРЕДМЕТОВ",
                ["ITEM TEXT COLOR"] = "ЦВЕТ ТЕКСТА ПРЕДМЕТОВ",
                ["MONEY BAGS COLOR"] = "ЦВЕТ МЕШКОВ С ДЕНЬГАМИ",
                ["EXTRACTION POINTS COLOR"] = "ЦВЕТ ТОЧЕК СБОРА",
                ["COSMETIC BOX GLOW COLOR"] = "ЦВЕТ ПОДСВЕТКИ КОСМ. БОКСОВ",
                ["COSMETIC BOX TEXT COLOR"] = "ЦВЕТ ТЕКСТА КОСМ. БОКСОВ",
                ["RARITY TEXT COLOR"] = "ЦВЕТ ТЕКСТА РЕДКОСТИ",
                ["ENEMY GLOW COLOR"] = "ЦВЕТ ПОДСВЕТКИ ВРАГОВ",
                ["ENEMY TEXT COLOR"] = "ЦВЕТ ТЕКСТА ВРАГОВ",
                ["PLAYER GLOW COLOR"] = "ЦВЕТ ПОДСВЕТКИ ИГРОКОВ",
                ["PLAYER TEXT COLOR"] = "ЦВЕТ ТЕКСТА ИГРОКОВ",
                ["DEAD HEADS COLOR"] = "ЦВЕТ ГОЛОВ МЁРТВЫХ",
                ["Teleport"] = "Телепорт",
                ["Items"] = "Предметы",
                ["Valuables"] = "Ценности",
                ["Entity"] = "Сущности",
                ["Players"] = "Игроки",
                ["Maps"] = "Карты",
                ["Gameplay"] = "Геймплей",
                ["HEAL"] = "ЛЕЧИТЬ",
                ["REVIVE"] = "ВОСКРЕСИТЬ",
                ["GOTO"] = "К НЕМУ",
                ["BRING"] = "К СЕБЕ",
                ["KILL"] = "УБИТЬ",
                ["FREEZE"] = "ЗАМОРОЗИТЬ",
                ["UNFREEZE"] = "РАЗМОРОЗИТЬ",
                ["UPGRADES"] = "УЛУЧШЕНИЯ",
                ["Set level"] = "Установить уровень",
                ["Set Money"] = "Установить деньги",
                ["current"] = "текущий",
                ["Change map?"] = "Сменить карту?",
                ["No players found."] = "Игроки не найдены.",
                ["No maps found."] = "Карты не найдены.",
                ["No spawnable items found."] = "Предметы для спавна не найдены.",
                ["No valuables found."] = "Ценности не найдены.",
                ["No entities found."] = "Сущности не найдены.",
                ["Hide all hints"] = "Скрыть все подсказки",
                ["Hide all tooltips"] = "Скрыть всплывающие подсказки",
                ["Drag window"] = "Перетаскивать окно",
                ["Hints color:"] = "Цвет подсказок:",
                ["Cursor X offset"] = "Смещение курсора X",
                ["Cursor Y offset"] = "Смещение курсора Y",
                ["Recommended files: PNG-32x32"] = "Рекомендуемые файлы: PNG 32x32",
                ["Source path:"] = "Путь к файлу:",
                ["Custom source"] = "Свой источник",
                ["Enable snowfall"] = "Включить снегопад",
                ["Only in menu"] = "Только в меню",
                ["Fall speed"] = "Скорость падения",
                ["Spawn interval"] = "Интервал появления",
                ["Scale"] = "Размер",
                ["Dynamic scale"] = "Динамический размер",
                ["From"] = "От",
                ["To"] = "До",
                ["Custom flake source"] = "Своя снежинка",
                ["Spin"] = "Вращение",
                ["Spin side: "] = "Сторона вращения: ",
                ["To left side"] = "Влево",
                ["To right side"] = "Вправо",
                ["Dynamic select side"] = "Случайная сторона",
                ["Dynamic rotate offset"] = "Случайный поворот",
                ["Spin speed"] = "Скорость вращения",
                ["God mode"] = "Режим бога",
                ["Infinite sprint"] = "Бесконечный бег",
                ["Infinite head energy"] = "Бесконечная энергия головы",
                ["Freecam"] = "Свободная камера",
                ["Bind"] = "Бинд",
                ["Speed hack"] = "Изменение скорости",
                ["Walk speed"] = "Скорость ходьбы",
                ["Sprint speed"] = "Скорость бега",
                ["Crouch speed"] = "Скорость в приседе",
                ["Jump force"] = "Сила прыжка",
                ["Custom FOV"] = "Свой FOV",
                ["RGB Player color"] = "RGB-цвет игрока",
                ["Update interval"] = "Интервал обновления",
                ["No Post-Processing"] = "Без постобработки",
                ["Dead voice"] = "Голос мёртвых",
                ["Tumble Bypass"] = "Обход tumble",
                ["Flashlight settings"] = "Настройки фонарика",
                ["Shadows"] = "Тени",
                ["Range"] = "Угол",
                ["Intensity"] = "Дальность",
                ["Render distance:"] = "Дистанция рендера:",
                ["Distance"] = "Дистанция",
                ["Show items"] = "Показывать предметы",
                ["Glow color:"] = "Цвет подсветки:",
                ["Text color:"] = "Цвет текста:",
                ["Sync with glow"] = "Синхронизировать с подсветкой",
                ["Text size"] = "Размер текста",
                ["Show money bags"] = "Показывать мешки с деньгами",
                ["Show extraction points"] = "Показывать точки сбора",
                ["Show name"] = "Показывать имя",
                ["Show price"] = "Показывать цену",
                ["Sort by price"] = "Сортировать по цене",
                ["Sort from"] = "Цена от",
                ["Sort to"] = "Цена до",
                ["Show cosmetic boxes"] = "Показывать косметические боксы",
                ["Show rarity text"] = "Показывать редкость",
                ["Text alpha"] = "Прозрачность текста",
                ["Rarity text color:"] = "Цвет текста редкости:",
                ["Show enemies"] = "Показывать врагов",
                ["Show health"] = "Показывать здоровье",
                ["Show glow"] = "Показывать подсветку",
                ["Show player"] = "Показывать игроков",
                ["Show dead heads"] = "Показывать головы мёртвых",
                ["Noclip"] = "Noclip",
                ["Noclip speed"] = "Скорость noclip",
                ["Hide Me"] = "Скрыть меня",
                ["Fullbright"] = "Максимальная яркость",
                ["Multi-jumps"] = "Мультипрыжки",
                ["No Token HUD"] = "Без Token HUD",
                ["Protected session"] = "Защищённая сессия",
                ["Infinity ammo"] = "Бесконечные патроны",
                ["Died cockroach"] = "Движение после смерти",
                ["Force"] = "Сила",
                ["Valuables teleporter"] = "Телепорт ценностей",
                ["Modes: "] = "Режимы: ",
                ["Kinematic"] = "Kinematic",
                ["Permanent freeze"] = "Постоянная заморозка",
                ["Disable interval"] = "Интервал отключения",
                ["Disable on touch"] = "Отключать при касании",
                ["One any"] = "Один случайный",
                ["Teleport on touch"] = "Телепорт при касании",
                ["To player"] = "К игроку",
                ["Into extract point"] = "В точку сбора",
                ["Into nearest cart"] = "В ближайшую тележку",
                ["Enemies teleporter"] = "Телепорт врагов",
                ["Permanent freeze movement"] = "Постоянная заморозка движения",
                ["Into void"] = "В пустоту",
                ["No fragility"] = "Без хрупкости",
                ["Ghost items"] = "Призрачные предметы",
                ["Lite items"] = "Лёгкие предметы",
                ["Peaceful enemies"] = "Мирные враги",
                ["One-shot kills"] = "Убийство с одного удара",
                ["DEAD"] = "МЁРТВ",
                ["HP"] = "HP",
                ["Unifrom Message"] = "Сообщение Unifrom"
            });

            AddUi(2, new Dictionary<string, string>
            {
                ["Default"] = "За замовчуванням",
                ["CUSTOM CURSOR"] = "ВЛАСНИЙ КУРСОР",
                ["PROCEDURAL SNOWFALL"] = "ПРОЦЕДУРНИЙ СНІГОПАД",
                ["UPDATE"] = "ОНОВИТИ",
                ["RESET"] = "СКИНУТИ",
                ["SET"] = "ВСТАНОВИТИ",
                ["ADD"] = "ДОДАТИ",
                ["CONFIRM"] = "ПІДТВЕРДИТИ",
                ["CANCEL"] = "СКАСУВАТИ",
                ["UNLOAD CHEAT"] = "ВИВАНТАЖИТИ ЧИТ",
                ["OBJECT SPAWNER"] = "СПАВНЕР ОБ'ЄКТІВ",
                ["GAME CONTROLLER"] = "КОНТРОЛЕР ГРИ",
                ["KILL ENEMIES"] = "ВБИТИ ВОРОГІВ",
                ["RENDER SETTINGS"] = "НАЛАШТУВАННЯ РЕНДЕРУ",
                ["ITEM GLOW COLOR"] = "КОЛІР ПІДСВІТКИ ПРЕДМЕТІВ",
                ["ITEM TEXT COLOR"] = "КОЛІР ТЕКСТУ ПРЕДМЕТІВ",
                ["MONEY BAGS COLOR"] = "КОЛІР МІШКІВ З ГРОШИМА",
                ["EXTRACTION POINTS COLOR"] = "КОЛІР ТОЧОК ЗБОРУ",
                ["COSMETIC BOX GLOW COLOR"] = "КОЛІР ПІДСВІТКИ КОСМ. БОКСІВ",
                ["COSMETIC BOX TEXT COLOR"] = "КОЛІР ТЕКСТУ КОСМ. БОКСІВ",
                ["RARITY TEXT COLOR"] = "КОЛІР ТЕКСТУ РІДКОСТІ",
                ["ENEMY GLOW COLOR"] = "КОЛІР ПІДСВІТКИ ВОРОГІВ",
                ["ENEMY TEXT COLOR"] = "КОЛІР ТЕКСТУ ВОРОГІВ",
                ["PLAYER GLOW COLOR"] = "КОЛІР ПІДСВІТКИ ГРАВЦІВ",
                ["PLAYER TEXT COLOR"] = "КОЛІР ТЕКСТУ ГРАВЦІВ",
                ["DEAD HEADS COLOR"] = "КОЛІР ГОЛІВ МЕРТВИХ",
                ["Teleport"] = "Телепорт",
                ["Items"] = "Предмети",
                ["Valuables"] = "Цінності",
                ["Entity"] = "Сутності",
                ["Players"] = "Гравці",
                ["Maps"] = "Карти",
                ["Gameplay"] = "Геймплей",
                ["HEAL"] = "ЛІКУВАТИ",
                ["REVIVE"] = "ВОСКРЕСИТИ",
                ["GOTO"] = "ДО НЬОГО",
                ["BRING"] = "ДО СЕБЕ",
                ["KILL"] = "ВБИТИ",
                ["FREEZE"] = "ЗАМОРОЗИТИ",
                ["UNFREEZE"] = "РОЗМОРОЗИТИ",
                ["UPGRADES"] = "ПОКРАЩЕННЯ",
                ["Set level"] = "Встановити рівень",
                ["Set Money"] = "Встановити гроші",
                ["current"] = "поточний",
                ["Change map?"] = "Змінити карту?",
                ["No players found."] = "Гравців не знайдено.",
                ["No maps found."] = "Карти не знайдено.",
                ["No spawnable items found."] = "Предмети для спавну не знайдено.",
                ["No valuables found."] = "Цінності не знайдено.",
                ["No entities found."] = "Сутності не знайдено.",
                ["Hide all hints"] = "Приховати всі підказки",
                ["Hide all tooltips"] = "Приховати спливаючі підказки",
                ["Drag window"] = "Перетягувати вікно",
                ["Hints color:"] = "Колір підказок:",
                ["Cursor X offset"] = "Зсув курсора X",
                ["Cursor Y offset"] = "Зсув курсора Y",
                ["Recommended files: PNG-32x32"] = "Рекомендовані файли: PNG 32x32",
                ["Source path:"] = "Шлях до файлу:",
                ["Custom source"] = "Власне джерело",
                ["Enable snowfall"] = "Увімкнути снігопад",
                ["Only in menu"] = "Тільки в меню",
                ["Fall speed"] = "Швидкість падіння",
                ["Spawn interval"] = "Інтервал появи",
                ["Scale"] = "Розмір",
                ["Dynamic scale"] = "Динамічний розмір",
                ["From"] = "Від",
                ["To"] = "До",
                ["Custom flake source"] = "Власна сніжинка",
                ["Spin"] = "Обертання",
                ["Spin side: "] = "Сторона обертання: ",
                ["To left side"] = "Вліво",
                ["To right side"] = "Вправо",
                ["Dynamic select side"] = "Випадкова сторона",
                ["Dynamic rotate offset"] = "Випадковий поворот",
                ["Spin speed"] = "Швидкість обертання",
                ["God mode"] = "Режим бога",
                ["Infinite sprint"] = "Нескінченний біг",
                ["Infinite head energy"] = "Нескінченна енергія голови",
                ["Freecam"] = "Вільна камера",
                ["Bind"] = "Бінд",
                ["Speed hack"] = "Зміна швидкості",
                ["Walk speed"] = "Швидкість ходьби",
                ["Sprint speed"] = "Швидкість бігу",
                ["Crouch speed"] = "Швидкість у присіді",
                ["Jump force"] = "Сила стрибка",
                ["Custom FOV"] = "Власний FOV",
                ["RGB Player color"] = "RGB-колір гравця",
                ["Update interval"] = "Інтервал оновлення",
                ["No Post-Processing"] = "Без постобробки",
                ["Dead voice"] = "Голос мертвих",
                ["Tumble Bypass"] = "Обхід tumble",
                ["Flashlight settings"] = "Налаштування ліхтарика",
                ["Shadows"] = "Тіні",
                ["Range"] = "Кут",
                ["Intensity"] = "Дальність",
                ["Render distance:"] = "Дистанція рендеру:",
                ["Distance"] = "Дистанція",
                ["Show items"] = "Показувати предмети",
                ["Glow color:"] = "Колір підсвітки:",
                ["Text color:"] = "Колір тексту:",
                ["Sync with glow"] = "Синхронізувати з підсвіткою",
                ["Text size"] = "Розмір тексту",
                ["Show money bags"] = "Показувати мішки з грошима",
                ["Show extraction points"] = "Показувати точки збору",
                ["Show name"] = "Показувати ім'я",
                ["Show price"] = "Показувати ціну",
                ["Sort by price"] = "Сортувати за ціною",
                ["Sort from"] = "Ціна від",
                ["Sort to"] = "Ціна до",
                ["Show cosmetic boxes"] = "Показувати косметичні бокси",
                ["Show rarity text"] = "Показувати рідкість",
                ["Text alpha"] = "Прозорість тексту",
                ["Rarity text color:"] = "Колір тексту рідкості:",
                ["Show enemies"] = "Показувати ворогів",
                ["Show health"] = "Показувати здоров'я",
                ["Show glow"] = "Показувати підсвітку",
                ["Show player"] = "Показувати гравців",
                ["Show dead heads"] = "Показувати голови мертвих",
                ["Noclip"] = "Noclip",
                ["Noclip speed"] = "Швидкість noclip",
                ["Hide Me"] = "Сховати мене",
                ["Fullbright"] = "Максимальна яскравість",
                ["Multi-jumps"] = "Мультистрибки",
                ["No Token HUD"] = "Без Token HUD",
                ["Protected session"] = "Захищена сесія",
                ["Infinity ammo"] = "Нескінченні патрони",
                ["Died cockroach"] = "Рух після смерті",
                ["Force"] = "Сила",
                ["Valuables teleporter"] = "Телепорт цінностей",
                ["Modes: "] = "Режими: ",
                ["Kinematic"] = "Kinematic",
                ["Permanent freeze"] = "Постійна заморозка",
                ["Disable interval"] = "Інтервал вимкнення",
                ["Disable on touch"] = "Вимикати при дотику",
                ["One any"] = "Один випадковий",
                ["Teleport on touch"] = "Телепорт при дотику",
                ["To player"] = "До гравця",
                ["Into extract point"] = "У точку збору",
                ["Into nearest cart"] = "У найближчий візок",
                ["Enemies teleporter"] = "Телепорт ворогів",
                ["Permanent freeze movement"] = "Постійна заморозка руху",
                ["Into void"] = "У пустоту",
                ["No fragility"] = "Без крихкості",
                ["Ghost items"] = "Примарні предмети",
                ["Lite items"] = "Легкі предмети",
                ["Peaceful enemies"] = "Мирні вороги",
                ["One-shot kills"] = "Вбивство з одного удару",
                ["DEAD"] = "МЕРТВИЙ",
                ["HP"] = "HP",
                ["Unifrom Message"] = "Повідомлення Unifrom"
            });

            AddUnifromGuiUi();
            AddExtendedTooltipPacks();
        }

        private static void AddUi(int language, Dictionary<string, string> values)
        {
            if (!UiText.TryGetValue(language, out var pack))
            {
                UiText[language] = values;
                return;
            }

            foreach (var pair in values)
                pack[pair.Key] = pair.Value;
        }

        private static readonly Dictionary<string, string[]> CommonUi = new()
        {
            ["Default"] = L("既定", "기본값", "Domyślnie", "Predeterm.", "Défaut", "افتراضي", "Подразумевано", "Прадвызначана", "Varsayılan"),
            ["CUSTOM CURSOR"] = L("カスタムカーソル", "사용자 커서", "WŁASNY KURSOR", "CURSOR PERSONALIZADO", "CURSEUR PERSONNALISÉ", "مؤشر مخصص", "ПРИЛАГОЂЕНИ КУРСОР", "УЛАСНЫ КУРСОР", "ÖZEL İMLEÇ"),
            ["PROCEDURAL SNOWFALL"] = L("手続き型の降雪", "절차적 눈", "PROCEDURALNY ŚNIEG", "NEVADA PROCEDURAL", "NEIGE PROCÉDURALE", "ثلج إجرائي", "ПРОЦЕДУРАЛНИ СНЕГ", "ПРАЦЭДУРНЫ СНЕГ", "PROSEDÜREL KAR"),
            ["UPDATE"] = L("更新", "업데이트", "AKTUALIZUJ", "ACTUALIZAR", "METTRE À JOUR", "تحديث", "АЖУРИРАЈ", "АБНАВІЦЬ", "GÜNCELLE"),
            ["RESET"] = L("リセット", "초기화", "RESET", "RESTABLECER", "RÉINIT.", "إعادة ضبط", "РЕСЕТУЈ", "СКІНУЦЬ", "SIFIRLA"),
            ["SET"] = L("設定", "설정", "USTAW", "FIJAR", "DÉFINIR", "تعيين", "ПОСТАВИ", "УСТАЛЯВАЦЬ", "AYARLA"),
            ["ADD"] = L("追加", "추가", "DODAJ", "AÑADIR", "AJOUTER", "إضافة", "ДОДАЈ", "ДАДАЦЬ", "EKLE"),
            ["CONFIRM"] = L("確認", "확인", "POTWIERDŹ", "CONFIRMAR", "CONFIRMER", "تأكيد", "ПОТВРДИ", "ПАЦВЕРДЗІЦЬ", "ONAYLA"),
            ["CANCEL"] = L("キャンセル", "취소", "ANULUJ", "CANCELAR", "ANNULER", "إلغاء", "ОТКАЖИ", "СКАСАВАЦЬ", "İPTAL"),
            ["UNLOAD CHEAT"] = L("チートをアンロード", "치트 언로드", "WYŁADUJ CHEAT", "DESCARGAR CHEAT", "DÉCHARGER LE CHEAT", "إلغاء تحميل الغش", "ИСКЉУЧИ CHEAT", "ВЫГРУЗІЦЬ ЧЫТ", "HİLEYİ KALDIR"),
            ["OBJECT SPAWNER"] = L("オブジェクト生成", "오브젝트 스포너", "SPAWNER OBIEKTÓW", "GENERADOR DE OBJETOS", "GÉNÉRATEUR D'OBJETS", "مولد الكائنات", "СПАВНЕР ОБЈЕКАТА", "СПАЎНЕР АБ'ЕКТАЎ", "NESNE OLUŞTURUCU"),
            ["GAME CONTROLLER"] = L("ゲーム制御", "게임 컨트롤러", "KONTROLER GRY", "CONTROL DEL JUEGO", "CONTRÔLE DU JEU", "متحكم اللعبة", "КОНТРОЛЕР ИГРЕ", "КІРАВАННЕ ГУЛЬНЁЙ", "OYUN KONTROLCÜSÜ"),
            ["KILL ENEMIES"] = L("敵を倒す", "적 처치", "ZABIJ WROGÓW", "MATAR ENEMIGOS", "TUER LES ENNEMIS", "قتل الأعداء", "УБИЈ НЕПРИЈАТЕЉЕ", "ЗАБІЦЬ ВОРАГАЎ", "DÜŞMANLARI ÖLDÜR"),
            ["Teleport"] = L("テレポート", "순간이동", "Teleportuj", "Teletransportar", "Téléporter", "نقل", "Телепорт", "Тэлепарт", "Işınla"),
            ["Items"] = L("アイテム", "아이템", "Przedmioty", "Objetos", "Objets", "العناصر", "Предмети", "Прадметы", "Eşyalar"),
            ["Valuables"] = L("貴重品", "귀중품", "Kosztowności", "Valiosos", "Objets de valeur", "الأشياء الثمينة", "Вредности", "Каштоўнасці", "Değerli eşyalar"),
            ["Entity"] = L("エンティティ", "엔티티", "Byty", "Entidades", "Entités", "الكيانات", "Ентитети", "Сутнасці", "Varlıklar"),
            ["Players"] = L("プレイヤー", "플레이어", "Gracze", "Jugadores", "Joueurs", "اللاعبون", "Играчи", "Гульцы", "Oyuncular"),
            ["Maps"] = L("マップ", "맵", "Mapy", "Mapas", "Cartes", "الخرائط", "Мапе", "Карты", "Haritalar"),
            ["Gameplay"] = L("ゲームプレイ", "게임플레이", "Rozgrywka", "Jugabilidad", "Gameplay", "أسلوب اللعب", "Играње", "Геймплэй", "Oynanış"),
            ["HEAL"] = L("回復", "치유", "ULECZ", "CURAR", "SOIGNER", "شفاء", "ИЗЛЕЧИ", "ВЫЛЕЧЫЦЬ", "İYİLEŞTİR"),
            ["REVIVE"] = L("蘇生", "부활", "WSKRZEŚ", "REVIVIR", "RÉANIMER", "إحياء", "ОЖИВИ", "АЖЫВІЦЬ", "DİRİLT"),
            ["GOTO"] = L("移動", "이동", "IDŹ DO", "IR A", "ALLER", "اذهب", "ИДИ ДО", "ПЕРАЙСЦІ", "GİT"),
            ["BRING"] = L("呼ぶ", "가져오기", "PRZYCIĄGNIJ", "TRAER", "AMENER", "إحضار", "ДОВЕДИ", "ПРЫВЕСЦІ", "GETİR"),
            ["KILL"] = L("倒す", "처치", "ZABIJ", "MATAR", "TUER", "قتل", "УБИЈ", "ЗАБІЦЬ", "ÖLDÜR"),
            ["FREEZE"] = L("固定", "동결", "ZAMROŹ", "CONGELAR", "GELER", "تجميد", "ЗАМРЗНИ", "ЗАМАРОЗІЦЬ", "DONDUR"),
            ["UNFREEZE"] = L("解除", "해제", "ODMROŹ", "DESCONGELAR", "DÉGELER", "إلغاء التجميد", "ОДМРЗНИ", "РАЗМАРОЗІЦЬ", "ÇÖZ"),
            ["UPGRADES"] = L("アップグレード", "업그레이드", "ULEPSZENIA", "MEJORAS", "AMÉLIORATIONS", "الترقيات", "НАДОГРАДЊЕ", "ПАЛЯПШЭННІ", "YÜKSELTMELER"),
            ["Set level"] = L("レベル設定", "레벨 설정", "Ustaw poziom", "Establecer nivel", "Définir niveau", "تعيين المستوى", "Постави ниво", "Усталяваць узровень", "Seviye ayarla"),
            ["Set Money"] = L("所持金設定", "돈 설정", "Ustaw pieniądze", "Establecer dinero", "Définir argent", "تعيين المال", "Постави новац", "Усталяваць грошы", "Para ayarla"),
            ["current"] = L("現在", "현재", "teraz", "actual", "actuel", "الحالي", "тренутно", "цяпер", "mevcut"),
            ["Change map?"] = L("マップを変更しますか？", "맵을 변경할까요?", "Zmienić mapę?", "¿Cambiar mapa?", "Changer de carte ?", "تغيير الخريطة؟", "Променити мапу?", "Змяніць карту?", "Harita değişsin mi?"),
            ["No players found."] = L("プレイヤーが見つかりません。", "플레이어가 없습니다.", "Nie znaleziono graczy.", "No se encontraron jugadores.", "Aucun joueur trouvé.", "لم يتم العثور على لاعبين.", "Играчи нису пронађени.", "Гульцы не знойдзены.", "Oyuncu bulunamadı."),
            ["No maps found."] = L("マップが見つかりません。", "맵이 없습니다.", "Nie znaleziono map.", "No se encontraron mapas.", "Aucune carte trouvée.", "لم يتم العثور على خرائط.", "Мапе нису пронађене.", "Карты не знойдзены.", "Harita bulunamadı."),
            ["No spawnable items found."] = L("生成可能なアイテムがありません。", "생성 가능한 아이템이 없습니다.", "Brak przedmiotów do utworzenia.", "No hay objetos generables.", "Aucun objet générable.", "لا توجد عناصر قابلة للإنشاء.", "Нема предмета за стварање.", "Няма прадметаў для стварэння.", "Oluşturulabilir eşya yok."),
            ["No valuables found."] = L("貴重品が見つかりません。", "귀중품이 없습니다.", "Nie znaleziono kosztowności.", "No se encontraron valiosos.", "Aucun objet de valeur.", "لم يتم العثور على أشياء ثمينة.", "Вредности нису пронађене.", "Каштоўнасці не знойдзены.", "Değerli eşya bulunamadı."),
            ["No entities found."] = L("エンティティが見つかりません。", "엔티티가 없습니다.", "Nie znaleziono bytów.", "No se encontraron entidades.", "Aucune entité trouvée.", "لم يتم العثور على كيانات.", "Ентитети нису пронађени.", "Сутнасці не знойдзены.", "Varlık bulunamadı.")
            ,
            ["Hide all hints"] = L("全ヒントを非表示", "모든 힌트 숨기기", "Ukryj wskazówki", "Ocultar pistas", "Masquer indices", "إخفاء التلميحات", "Сакриј савете", "Схаваць падказкі", "İpuçlarını gizle"),
            ["Hide all tooltips"] = L("全ツールチップを非表示", "모든 툴팁 숨기기", "Ukryj podpowiedzi", "Ocultar ayudas", "Masquer infobulles", "إخفاء التلميحات", "Сакриј описе", "Схаваць падказкі", "İpuçlarını gizle"),
            ["Drag window"] = L("ウィンドウ移動", "창 드래그", "Przeciągaj okno", "Arrastrar ventana", "Déplacer fenêtre", "سحب النافذة", "Померај прозор", "Перацягваць акно", "Pencereyi sürükle"),
            ["Enable snowfall"] = L("降雪を有効化", "눈 활성화", "Włącz śnieg", "Activar nieve", "Activer neige", "تفعيل الثلج", "Укључи снег", "Уключыць снег", "Karı aç"),
            ["Only in menu"] = L("メニュー内のみ", "메뉴에서만", "Tylko w menu", "Solo en menú", "Menu seulement", "في القائمة فقط", "Само у менију", "Толькі ў меню", "Sadece menüde"),
            ["God mode"] = L("無敵モード", "무적 모드", "Tryb boga", "Modo dios", "Mode dieu", "وضع الخلود", "Божји режим", "Рэжым бога", "Tanrı modu"),
            ["Infinite sprint"] = L("無限スタミナ", "무한 스프린트", "Nieskończony sprint", "Sprint infinito", "Sprint infini", "ركض غير محدود", "Бесконачан спринт", "Бясконцы спрынт", "Sınırsız koşu"),
            ["Infinite head energy"] = L("頭部エネルギー無限", "머리 에너지 무한", "Niesk. energia głowy", "Energía de cabeza infinita", "Énergie tête infinie", "طاقة رأس غير محدودة", "Бесконачна енергија главе", "Бясконцая энергія галавы", "Sınırsız kafa enerjisi"),
            ["Freecam"] = L("自由カメラ", "자유 카메라", "Wolna kamera", "Cámara libre", "Caméra libre", "كاميرا حرة", "Слободна камера", "Свабодная камера", "Serbest kamera"),
            ["Speed hack"] = L("速度変更", "속도 핵", "Zmiana szybkości", "Hack de velocidad", "Vitesse modifiée", "تعديل السرعة", "Измена брзине", "Змена хуткасці", "Hız hilesi"),
            ["Jump force"] = L("ジャンプ力", "점프 힘", "Siła skoku", "Fuerza de salto", "Force de saut", "قوة القفز", "Снага скока", "Сіла скачка", "Zıplama gücü"),
            ["Custom FOV"] = L("カスタムFOV", "사용자 FOV", "Własne FOV", "FOV personalizado", "FOV personnalisé", "مجال رؤية مخصص", "Прилагођен FOV", "Уласны FOV", "Özel FOV"),
            ["RGB Player color"] = L("RGBプレイヤー色", "RGB 플레이어 색", "Kolor gracza RGB", "Color RGB del jugador", "Couleur joueur RGB", "لون لاعب RGB", "RGB боја играча", "RGB колер гульца", "RGB oyuncu rengi"),
            ["No Post-Processing"] = L("ポスト処理なし", "후처리 없음", "Bez postprocessingu", "Sin posprocesado", "Sans post-traitement", "بدون معالجة لاحقة", "Без пост-обраде", "Без постапрацоўкі", "Post-processing yok"),
            ["Dead voice"] = L("死者の声", "죽은 자 음성", "Głos martwych", "Voz de muertos", "Voix des morts", "صوت الموتى", "Глас мртвих", "Голас мёртвых", "Ölü sesi"),
            ["Tumble Bypass"] = L("転倒制限回避", "텀블 우회", "Ominięcie tumble", "Saltar tumble", "Contournement tumble", "تجاوز التعثر", "Заобилажење tumble", "Абыход tumble", "Tumble atlatma"),
            ["Flashlight settings"] = L("ライト設定", "손전등 설정", "Latarka", "Linterna", "Lampe torche", "إعدادات المصباح", "Подешавања лампе", "Налады ліхтарыка", "Fener ayarları"),
            ["Show items"] = L("アイテム表示", "아이템 표시", "Pokaż przedmioty", "Mostrar objetos", "Afficher objets", "إظهار العناصر", "Прикажи предмете", "Паказваць прадметы", "Eşyaları göster"),
            ["Show enemies"] = L("敵を表示", "적 표시", "Pokaż wrogów", "Mostrar enemigos", "Afficher ennemis", "إظهار الأعداء", "Прикажи непријатеље", "Паказваць ворагаў", "Düşmanları göster"),
            ["Show player"] = L("プレイヤー表示", "플레이어 표시", "Pokaż gracza", "Mostrar jugador", "Afficher joueur", "إظهار اللاعب", "Прикажи играча", "Паказваць гульца", "Oyuncuyu göster"),
            ["Show name"] = L("名前を表示", "이름 표시", "Pokaż nazwę", "Mostrar nombre", "Afficher nom", "إظهار الاسم", "Прикажи име", "Паказваць імя", "Adı göster"),
            ["Show health"] = L("体力を表示", "체력 표시", "Pokaż zdrowie", "Mostrar salud", "Afficher santé", "إظهار الصحة", "Прикажи здравље", "Паказваць здароўе", "Canı göster"),
            ["Show glow"] = L("発光を表示", "광선 표시", "Pokaż poświatę", "Mostrar brillo", "Afficher lueur", "إظهار التوهج", "Прикажи сјај", "Паказваць свячэнне", "Parıltıyı göster"),
            ["Noclip"] = L("Noclip", "노클립", "Noclip", "Noclip", "Noclip", "اختراق الجدران", "Noclip", "Noclip", "Noclip"),
            ["Hide Me"] = L("自分を隠す", "나 숨기기", "Ukryj mnie", "Ocultarme", "Me cacher", "إخفائي", "Сакриј ме", "Схаваць мяне", "Beni gizle"),
            ["Fullbright"] = L("最大明るさ", "풀브라이트", "Pełna jasność", "Brillo total", "Pleine lumière", "إضاءة كاملة", "Пуна светлост", "Поўная яркасць", "Tam parlaklık"),
            ["Multi-jumps"] = L("多段ジャンプ", "다중 점프", "Wieloskoki", "Multisaltos", "Multi-sauts", "قفزات متعددة", "Вишеструки скокови", "Шмат скачкоў", "Çoklu zıplama"),
            ["No Token HUD"] = L("トークンHUDなし", "토큰 HUD 없음", "Bez HUD tokenów", "Sin HUD de tokens", "Sans HUD jetons", "بدون واجهة الرموز", "Без Token HUD", "Без HUD токенаў", "Token HUD yok"),
            ["Protected session"] = L("セッション保護", "세션 보호", "Chroniona sesja", "Sesión protegida", "Session protégée", "جلسة محمية", "Заштићена сесија", "Абароненая сесія", "Korumalı oturum"),
            ["Infinity ammo"] = L("弾薬無限", "무한 탄약", "Nieskończona amunicja", "Munición infinita", "Munitions infinies", "ذخيرة غير محدودة", "Бесконачна муниција", "Бясконцыя патроны", "Sınırsız cephane"),
            ["Died cockroach"] = L("死亡頭移動", "죽은 머리 이동", "Ruch po śmierci", "Mover cabeza muerta", "Bouger après mort", "حركة بعد الموت", "Кретање после смрти", "Рух пасля смерці", "Ölüyken hareket"),
            ["Valuables teleporter"] = L("貴重品テレポート", "귀중품 순간이동", "Teleport kosztowności", "Teletransp. valiosos", "Téléport objets valeur", "نقل الأشياء الثمينة", "Телепорт вредности", "Тэлепарт каштоўнасцей", "Değerli ışınlayıcı"),
            ["Enemies teleporter"] = L("敵テレポート", "적 순간이동", "Teleport wrogów", "Teletransp. enemigos", "Téléport ennemis", "نقل الأعداء", "Телепорт непријатеља", "Тэлепарт ворагаў", "Düşman ışınlayıcı"),
            ["No fragility"] = L("壊れやすさ無効", "취약성 없음", "Bez kruchości", "Sin fragilidad", "Sans fragilité", "بدون هشاشة", "Без ломљивости", "Без крохкасці", "Kırılganlık yok"),
            ["Ghost items"] = L("ゴーストアイテム", "유령 아이템", "Przedmioty duchy", "Objetos fantasma", "Objets fantômes", "عناصر شبحية", "Дух предмети", "Прывідныя прадметы", "Hayalet eşyalar"),
            ["Lite items"] = L("軽量アイテム", "가벼운 아이템", "Lekkie przedmioty", "Objetos ligeros", "Objets légers", "عناصر خفيفة", "Лаки предмети", "Лёгкія прадметы", "Hafif eşyalar"),
            ["Peaceful enemies"] = L("敵を無害化", "평화로운 적", "Pokojowi wrogowie", "Enemigos pacíficos", "Ennemis pacifiques", "أعداء مسالمون", "Мирни непријатељи", "Мірныя ворагі", "Barışçıl düşmanlar"),
            ["One-shot kills"] = L("一撃必殺", "한 방 처치", "Zabójstwa jednym ciosem", "Matar de un golpe", "Tués en un coup", "قتل بضربة واحدة", "Убиство једним ударцем", "Забойства з аднаго ўдару", "Tek vuruşta öldür")
        };

        private static string[] L(string ja, string ko, string pl, string es, string fr, string ar, string sr, string be, string tr)
        {
            return new[] { ja, ko, pl, es, fr, ar, sr, be, tr };
        }
    }
}
