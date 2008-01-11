@ECHO OFF
REM COMPILAR NO WINDOWS COM MONO

cd Server

set FRAMEWORK=%windir%\Microsoft.NET\\Framework\v2.0.50727
%FRAMEWORK%\csc.exe /nologo /unsafe /debug:full /out:..\RunUO_Win.exe /recurse:*.cs

cd ..

PAUSE
