using System;
using System.Collections.Generic;
using System.Text;

namespace KorzhLocker
{
    internal static class SimpleEnvironment
    {
        public static bool IsRussianLanguage { get; set; }

        internal static string LockWindowHint => IsRussianLanguage 
            ? "Клавиатура заблокирована. Нажмите левую кнопку мыши для разблокировки" 
            : "Keyboard is locked. Press left mouse key to unlock";

        internal static string LaunchToast => IsRussianLanguage
            ? "KorzhLocker запущен"
            : "KorzhLocker is launched";

        internal static string ShowingToast => IsRussianLanguage
            ? "KorzhLocker открыт"
            : "KorzhLocker is launched";

        internal static string HiddenToast => IsRussianLanguage
            ? "KorzhLocker скрыт"
            : "KorzhLocker is hidden";

        internal static string NotifyContextMenuShowHide => IsRussianLanguage
            ? "Открыть\\Скрыть"
            : "Show\\Hide";

        internal static string NotifyContextMenuExit => IsRussianLanguage
            ? "Выйти"
            : "Exit";
    }
}
