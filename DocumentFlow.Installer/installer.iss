; -- 64Bit.iss --
; Demonstrates installation of a program built for the x64 (a.k.a. AMD64)
; architecture.
; To successfully run this installation and the program it installs,
; you must have a "x64" edition of Windows.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

; Имя приложения
#define   Name       "DocumentFlow 2024"
; Версия приложения
#define   Version    "5.0.0.40424"
; Имя исполняемого модуля
#define   ExeName    "DocumentFlow.exe"

[Setup]

; Прочая информация, отображаемая при установке
AppId={{847FB541-464C-4173-A7AA-A72CD050045D}}
AppName={#Name}
AppVersion={#Version}
WizardStyle=modern

; Путь установки по-умолчанию
DefaultDirName={autopf}\{#Name}

; Имя группы в меню "Пуск"
DefaultGroupName={#Name}
UninstallDisplayIcon={app}\{#Name}.exe

; Параметры сжатия
Compression=lzma2
SolidCompression=yes

; Каталог, куда будет записан собранный setup и имя исполняемого файла
OutputDir=C:\Projects\DocumentFlow-2024\DocumentFlow.Installer
OutputBaseFileName={#Name}-{#SetupSetting("AppVersion")}-x64-installer

; Файл иконки
SetupIconFile=C:\Projects\DocumentFlow-2024\DocumentFlow.Installer\setup.ico

; "ArchitecturesAllowed=x64" specifies that Setup cannot run on
; anything but x64.
ArchitecturesAllowed=x64
; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
; done in "64-bit mode" on x64, meaning it should use the native
; 64-bit Program Files directory and the 64-bit view of the registry.
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "C:\Projects\DocumentFlow-2024\DocumentFlow\bin\Release\DocumentFlow.exe"; DestDir: "{app}"
Source: "C:\Projects\DocumentFlow-2024\DocumentFlow\bin\Release\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\DocumentFlow"; Filename: "{app}\{#ExeName}"

[Run]
Filename: "{app}\{#ExeName}"; Description: "Launch application"; Flags: postinstall nowait skipifsilent unchecked