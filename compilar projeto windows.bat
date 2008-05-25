@ECHO OFF
REM Compiler Project in Windows

REM Change the directory below based on your machine.
cd "E:\Projetos\GeNova\GeNova 3.0"
IF EXIST Runuo_Windows.exe DEL Runuo_Windows.exe
IF EXIST Runuo_Windows.pdb DEL Runuo_Windows.pdb

REM Change the directory below based on your machine.
cd "E:\Projetos\GeNova\GeNova 3.0\Server"
set FRAMEWORK=%windir%\Microsoft.NET\\Framework\v2.0.50727

REM NEW LINES FOR FRAMEWORK 3.5
set LASTFRAMEWORK=%windir%\Microsoft.NET\\Framework\v3.5
set LASTREFERENCESFRAMEWORKVERSION=C:\\Arquivos de programas\\Reference Assemblies\\Microsoft\\Framework\v3.5
%LASTFRAMEWORK%\csc.exe /noconfig /unsafe+ /nowarn:1701,1702 /errorreport:prompt /warn:4 /define:TRACE /main:Server.Core /reference:bin\MySql.Data.dll /reference:"%LASTREFERENCESFRAMEWORKVERSION%\System.Core.dll" /reference:%FRAMEWORK%\System.Data.dll /reference:%FRAMEWORK%\System.dll /reference:%FRAMEWORK%\System.Drawing.dll /reference:%FRAMEWORK%\System.Xml.dll /reference:bin\Ultima.dll /debug+ /debug:full /optimize+ /out:..\RunUO_Windows.exe /target:exe /win32icon:genova.ico /recurse:*.cs