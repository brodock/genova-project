@ECHO OFF
REM COMPILAR NO WINDOWS COM MONO

REM Change the directory below based on your machine.
cd "E:\Projetos\GeNova\GeNova 3.0"
IF EXIST Runuo_Mono.exe DEL Runuo_Mono.exe

REM Change the directory below based on your machine.
cd "E:\Projetos\GeNova\GeNova 3.0\Server"
set FRAMEWORK=E:\"Arquivos de programas"\Mono-1.9.1\bin
%FRAMEWORK%\gmcs -define:MONO -unsafe+ -optimize -target:exe -out:..\RunUO_Mono.exe -win32icon:genova.ico -recurse:*.cs -reference:MySql.Data.dll -reference:System.Data.dll -reference:System.Drawing.dll -reference:Ultima.dll

REM COMPILAR NO LINUX COM MONO
REM $ ~/mono-1.2.5/bin/gmcs -d:MONO -unsafe+ -optimize- -t:exe -out:RunUO.exe -recurse:*.cs