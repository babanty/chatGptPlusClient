#include <WinAPI.au3>


Func openChrome()
    ; Указываем класс окна, которое мы ищем
    Local $sClassName = "Chrome_WidgetWin_1"

    ; Ищем окна с классом Chrome_WidgetWin_1 и словом "chrome" в заголовке
    Local $aWindows = WinList("[CLASS:" & $sClassName & "]")
    For $i = 1 To $aWindows[0][0]
        If $aWindows[$i][0] <> "" And StringInStr(StringLower($aWindows[$i][0]), "chrome") > 0 Then
            ; Разворачиваем окно
            _WinAPI_ShowWindow($aWindows[$i][1], @SW_SHOWMAXIMIZED)

            ; Устанавливаем фокус на окно
            WinActivate($aWindows[$i][1])

            Return True
        EndIf
    Next

    ConsoleWrite("Окно с классом '" & $sClassName & "' и заголовком, содержащим 'chrome', не найдено.")
    Return False
EndFunc
openChrome()