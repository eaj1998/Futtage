[Setup]
AppName=Futtage
AppVersion=1.0.0
DefaultDirName={autopf}\Futtage
DefaultGroupName=Futtage
UninstallDisplayIcon={app}\Futtage.exe
Compression=lzma2
SolidCompression=yes
OutputDir=C:\Estudo\Futtage\InstallerOutput
OutputBaseFilename=Instalador_Futtage
SetupIconFile=C:\Estudo\Futtage\faz-o-simples.ico
PrivilegesRequired=lowest

[Files]
Source: "C:\Estudo\Futtage\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Futtage"; Filename: "{app}\Futtage.exe"
Name: "{autodesktop}\Futtage"; Filename: "{app}\Futtage.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Criar um atalho na Área de Trabalho"; GroupDescription: "Atalhos Adicionais:"

[Run]
Filename: "{app}\Futtage.exe"; Description: "Iniciar Futtage"; Flags: nowait postinstall skipifsilent
