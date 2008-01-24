@ECHO OFF
REM COMPILAR NO WINDOWS COM MONO

cd "C:\GeNova\GeNova 3.0"
IF EXIST Runuo_Mono.exe DEL Runuo_Mono.exe

cd "C:\GeNova\GeNova 3.0\Server"
set FRAMEWORK=C:\"Program Files"\Mono\bin
%FRAMEWORK%\gmcs -d:MONO -unsafe+ -optimize- -t:exe -out:..\RunUO_Mono.exe -recurse:*.cs -r:MySql.Data.dll -r:System.Data.dll -r:System.Drawing.dll -r:Ultima.dll

cd ..

REM COMPILAR NO LINUX COM MONO
REM $ ~/mono-1.2.5/bin/gmcs -d:MONO -unsafe+ -optimize- -t:exe -out:RunUO.exe -recurse:*.cs

