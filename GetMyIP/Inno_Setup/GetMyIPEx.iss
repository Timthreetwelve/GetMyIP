﻿;----------------------------------------------------------------------
; Inno Setup Script for Get My IP
;----------------------------------------------------------------------
; The following #include file is created by the PubSetupEx.ps1 script.
;
; It contains #define statements for:
;
;             InstallType:      String denoting installer type.
;                               Inserted into the installer file name.
;
;             PublishFolder:    The output folder from MS Build.
;                               Varies depending on the type of build.
;----------------------------------------------------------------------
#include "D:\Temp\PubSetup.Temp.iss"

#define BaseDir              "V:\Source\Repos\GetMyIP\GetMyIP"
#define MySourceDir          BaseDir + PublishFolder
#define MySetupIcon          BaseDir + "\Images\IP.ico"
#define MyOutputDir          "D:\InnoSetup\Output"
#define MyLargeImage         "D:\InnoSetup\Images\WizardImageGMI.bmp"
#define MySmallImage         "D:\InnoSetup\Images\WizardSmallImage.bmp"

#define MyAppID              "{EBEA37CE-1C9C-44C2-ACE3-102E6BF79364}"
#define MyAppName            "Get My IP"
#define MyAppNameNoSpaces    StringChange(MyAppName, " ", "")
#define MyAppExeName         "GetMyIP.exe"
#define MyAppVersion         GetVersionNumbersString(MySourceDir + "\" + MyAppExeName)
#define MyInstallerFilename  MyAppNameNoSpaces + "_" + MyAppVersion + "_" + InstallType + "_Setup"
#define MyCompanyName        "T_K"
#define MyPublisherName      "Tim Kennedy"
#define StartCopyrightYear   "2019"
#define CurrentYear          GetDateTimeString('yyyy', '/', ':')
#define MyCopyright          "(c) " + StartCopyrightYear + "-" + CurrentYear + " Tim Kennedy"
#define MyLicFile            "D:\Visual Studio\Resources\License.rtf"
#define MyDateTimeString     GetDateTimeString('yyyy/mm/dd hh:nn:ss', '/', ':')
#define MyAppSupportURL      "https://github.com/Timthreetwelve/GetMyIP"

#define RunRegKey            "Software\Microsoft\Windows\CurrentVersion\Run"

; -----------------------------------------------------
; Include the localization file. Thanks bovirus!
; -----------------------------------------------------
#include "GetMyIPLocalization.iss"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
;---------------------------------------------
AppId={{#MyAppID}

;---------------------------------------------
; Uncomment the following line to run in non administrative install mode (install for current user only.)
; Installs in %localappdata%\Programs\ instead of \Program Files(x86)
;---------------------------------------------
PrivilegesRequired=lowest
;---------------------------------------------

AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}

AppCopyright={#MyCopyright}
AppPublisherURL={#MyAppSupportURL}
AppSupportURL={#MyAppSupportURL}
AppUpdatesURL={#MyAppSupportURL}

VersionInfoDescription={#MyAppName} installer
VersionInfoProductName={#MyAppName}
VersionInfoVersion={#MyAppVersion}

UninstallDisplayName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
AppPublisher={#MyPublisherName}

ShowLanguageDialog=yes
UsePreviousLanguage=no
WizardStyle=modern
WizardSizePercent=100,100
WizardImageFile={#MyLargeImage}
WizardSmallImageFile={#MySmallImage}

AllowNoIcons=yes
Compression=lzma
DefaultDirName={autopf}\{#MyCompanyName}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
DisableReadyMemo=no
DisableStartupPrompt=yes
DisableWelcomePage=no
OutputBaseFilename={#MyInstallerFilename}
OutputDir={#MyOutputDir}
;OutputManifestFile={#MyAppName}_{#MyAppVersion}_{#InstallType}_FileList.txt
SetupIconFile={#MySetupIcon}
SetupLogging=yes
SolidCompression=no
SourceDir ={#MySourceDir}


[Files]
Source: "{#MySourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "{#MySourceDir}\*.json"; Excludes: "usersettings.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\ReadMe.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\License.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\Strings.test.xaml"; DestDir: "{app}"; Flags: ignoreversion

[InstallDelete]
; Delete these files & folders from previous installs
Type: filesandordirs; Name: "{group}"
Type: files; Name: "{app}\CommandLine.dll"
Type: files; Name: "{app}\CommunityToolkit.WinUI.Notifications.dll"
Type: files; Name: "{app}\MaterialDesignExtensions.dll"
Type: files; Name: "{app}\Newtonsoft.Json.dll"
Type: files; Name: "{app}\WpfScreenHelper.dll"
Type: filesandordirs; Name: "{app}\fr"


[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Registry]
Root: HKCU; Subkey: "Software\{#MyCompanyName}"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\{#MyCompanyName}\{#MyAppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyCompanyName}\{#MyAppName}"; ValueType: String; ValueName: "Copyright"; ValueData: "{#MyCopyright}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyCompanyName}\{#MyAppName}"; ValueType: String; ValueName: "Install Date"; ValueData: "{#MyDateTimeString}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyCompanyName}\{#MyAppName}"; ValueType: String; ValueName: "Version"; ValueData: "{#MyAppVersion}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyCompanyName}\{#MyAppName}"; ValueType: String; ValueName: "Install Folder"; ValueData: "{autopf}\{#MyCompanyName}\{#MyAppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyCompanyName}\{#MyAppName}"; ValueType: String; ValueName: "Installer Language"; ValueData:"{language}" ;Flags: uninsdeletekey
; Delete this key from previous installs
Root: HKCU; Subkey: "Software\{#MyCompanyName}\{#MyAppName}"; ValueType: none; ValueName: "Edition"; Flags: uninsdeletekey deletevalue

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec
Filename: "{app}\ReadMe.txt"; Description: "{cm:ViewReadme}"; Flags: nowait postinstall skipifsilent unchecked shellexec

[UninstallDelete]
Type: files; Name: "{app}\*.txt"
Type: files; Name: "{userdesktop}\Get My IP.lnk"
Type: files; Name: "{userdesktop}\GetMyIP.lnk"
Type: files; Name: "{commondesktop}\Get My IP.lnk"
Type: files; Name: "{commondesktop}\GetMyIP.lnk"

; -----------------------------------------------------------------------------
; Code section follows
; -----------------------------------------------------------------------------
[Code]
// Change text on welcome page based on installation type
procedure InitializeWizard;
var
  Text: String;
begin
  case ExpandConstant('{#InstallType}') of
    'x64x86': Text := FmtMessage( CustomMessage('NotSelfContained'), [ExpandConstant('{#MyAppName}'), ExpandConstant('{#MyAppVersion}')]);
    'SC_x86': Text := FmtMessage( CustomMessage('SelfContainedx86'), [ExpandConstant('{#MyAppName}'), ExpandConstant('{#MyAppVersion}')]);
    'SC_x64': Text := FmtMessage( CustomMessage('SelfContainedx64'), [ExpandConstant('{#MyAppName}'), ExpandConstant('{#MyAppVersion}')]);
  else
      Text := WizardForm.WelcomeLabel2.Caption;
  end;
  WizardForm.WelcomeLabel2.Caption := Text;
end;

// function used to check if app Is currently running
Function IsAppRunning(Const FileName : String): Boolean;
var
FSWbemLocator: Variant;
    FWMIService: Variant;
    FWbemObjectSet: Variant;
begin
    Result := false;
    FSWbemLocator := CreateOleObject('WBEMScripting.SWBEMLocator');
    FWMIService := FSWbemLocator.ConnectServer('', 'root\CIMV2', '', '');
    FWbemObjectSet :=
      FWMIService.ExecQuery(
        Format('SELECT Name FROM Win32_Process Where Name="%s"', [FileName]));
    Result:=(FWbemObjectSet.Count > 0);
    FWbemObjectSet := Unassigned;
    FWMIService := Unassigned;
    FSWbemLocator := Unassigned;
End;

// Checks if app Is running, if so, displays msgbox asking to close running app
Function InitializeSetup() : Boolean;
var
Answer: Integer;
  ThisApp: String;
begin
    Result := true;
  ThisApp := ExpandConstant('{#MyAppExeName}');
  While IsAppRunning(ThisApp) Do
  begin
        Answer := MsgBox(ThisApp + ' ' + CustomMessage('AppIsRunning'), mbError, MB_OKCANCEL);
    If Answer = IDCANCEL Then
            begin
            Result := false;
      Exit;
    End;
  End;
End;

// Copies setup log to app folder
procedure CurStepChanged(CurStep: TSetupStep);
var
            logfilepathname, newfilepathname: String;
begin
            If CurStep = ssDone Then
                begin
                logfilepathname := ExpandConstant('{log}');
      newfilepathname := ExpandConstant('{app}\') + 'Setup_Log.txt';
      Log('Setup log file copied to: ' + newfilepathname);
      CopyFile(logfilepathname, newfilepathname, False);
   End;
End;

// Uninstall
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
mres:           Integer;
begin
                Case CurUninstallStep Of
      usPostUninstall:
                begin
                mres := MsgBox(CustomMessage('DeleteConfigFiles'), mbConfirmation, MB_YESNO or MB_DEFBUTTON2)
          If mres = IDYES Then
                    begin
                    DelTree(ExpandConstant('{app}\*.json'), False, True, False);
            DelTree(ExpandConstant('{app}'), True, True, True);
          end;
       End;
   End;
End;
