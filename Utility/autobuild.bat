@Set Path=C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319;%PATH%
msbuild ..\RosSharp\RosSharp.csproj /p:Configuration=Release /t:Rebuild
msbuild ..\App\GenMsg.Lib\GenMsg.Lib.fsproj /p:Configuration=Release /t:Rebuild
msbuild ..\App\GenMsg\GenMsg.csproj /p:Configuration=Release /t:Rebuild
msbuild ..\App\RosCore\RosCore.csproj /p:Configuration=Release /t:Rebuild

set RosSharpDir=.\RosSharp-0.1.0

mkdir %RosSharpDir%

robocopy ..\ .\%RosSharpDir%\ License.txt

mkdir %RosSharpDir%\bin

robocopy ..\App\RosCore\bin\Release\ .\%RosSharpDir%\bin *.dll
robocopy ..\App\RosCore\bin\Release\ .\%RosSharpDir%\bin *.xml
robocopy ..\App\RosCore\bin\Release\ .\%RosSharpDir%\bin RosCore.exe
robocopy ..\App\RosCore\bin\Release\ .\%RosSharpDir%\bin RosCore.exe.config

robocopy ..\App\GenMsg\bin\Release\ .\%RosSharpDir%\bin *.dll
robocopy ..\App\GenMsg\bin\Release\ .\%RosSharpDir%\bin *.xml
robocopy ..\App\GenMsg\bin\Release\ .\%RosSharpDir%\bin GenMsg.exe

mkdir %RosSharpDir%\doc
cd ..\Doc
call make html
cd ..\Utility
robocopy /S ..\Doc\build\html .\%RosSharpDir%\doc


mkdir %RosSharpDir%\sample

mkdir %RosSharpDir%\sample\AddTwoIntsClient

robocopy ..\Sample\AddTwoIntsClient .\%RosSharpDir%\sample\AddTwoIntsClient Program.cs
robocopy ..\Sample\AddTwoIntsClient .\%RosSharpDir%\sample\AddTwoIntsClient AddTwoIntsClient.csproj
robocopy ..\Sample\AddTwoIntsClient .\%RosSharpDir%\sample\AddTwoIntsClient app.config
robocopy ..\Sample\AddTwoIntsClient\Properties .\%RosSharpDir%\sample\AddTwoIntsClient\Properties

mkdir %RosSharpDir%\sample\AddTwoIntsServer

robocopy ..\Sample\AddTwoIntsServer .\%RosSharpDir%\sample\AddTwoIntsServer Program.cs
robocopy ..\Sample\AddTwoIntsServer .\%RosSharpDir%\sample\AddTwoIntsServer AddTwoIntsServer.csproj
robocopy ..\Sample\AddTwoIntsServer .\%RosSharpDir%\sample\AddTwoIntsServer app.config
robocopy ..\Sample\AddTwoIntsServer\Properties .\%RosSharpDir%\sample\AddTwoIntsServer\Properties

mkdir %RosSharpDir%\sample\Listener

robocopy ..\Sample\Listener .\%RosSharpDir%\sample\Listener Program.cs
robocopy ..\Sample\Listener .\%RosSharpDir%\sample\Listener Listener.csproj
robocopy ..\Sample\Listener .\%RosSharpDir%\sample\Listener app.config
robocopy ..\Sample\Listener\Properties .\%RosSharpDir%\sample\Listener\Properties

mkdir %RosSharpDir%\sample\ParameterClient

robocopy ..\Sample\ParameterClient .\%RosSharpDir%\sample\ParameterClient Program.cs
robocopy ..\Sample\ParameterClient .\%RosSharpDir%\sample\ParameterClient ParameterClient.csproj
robocopy ..\Sample\ParameterClient .\%RosSharpDir%\sample\ParameterClient app.config
robocopy ..\Sample\ParameterClient\Properties .\%RosSharpDir%\sample\ParameterClient\Properties


mkdir %RosSharpDir%\sample\Talker

robocopy ..\Sample\Talker .\%RosSharpDir%\sample\Talker Program.cs
robocopy ..\Sample\Talker .\%RosSharpDir%\sample\Talker Talker.csproj
robocopy ..\Sample\Talker .\%RosSharpDir%\sample\Talker app.config
robocopy ..\Sample\Talker\Properties .\%RosSharpDir%\sample\Talker\Properties






rem msbuild %RosSharpDir%\sample\AddTwoIntsClient\AddTwoIntsClient.csproj /p:Configuration=Release /t:Rebuild
rem msbuild %RosSharpDir%\sample\AddTwoIntsServer\AddTwoIntsServer.csproj /p:Configuration=Release /t:Rebuild
rem msbuild %RosSharpDir%\sample\Listener\Listener.csproj /p:Configuration=Release /t:Rebuild
rem msbuild %RosSharpDir%\sample\ParameterClient\ParameterClient.csproj /p:Configuration=Release /t:Rebuild
rem msbuild %RosSharpDir%\sample\Talker\Talker.csproj /p:Configuration=Release /t:Rebuild

pause
