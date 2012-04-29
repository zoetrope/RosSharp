@echo off

SET PATH=..\App\GenMsg\bin\Debug;%PATH%

for %%f in (.\srv\core\*.srv) do (
    GenMsg.exe -t srv -n core -o ..\RosSharp\Generated\srv\core -i .\msg\roslib -i .\msg\std_msgs %%f
)

for %%f in (.\srv\sample\*.srv) do (
    GenMsg.exe -t srv -o ..\RosSharp\Generated\srv\sample -i .\msg\roslib -i .\msg\std_msgs %%f
)

for %%f in (.\srv\std_srvs\*.srv) do (
    GenMsg.exe -t srv -n std_srvs -o ..\RosSharp\Generated\srv\std_srvs -i .\msg\roslib -i .\msg\std_msgs %%f
)


for %%f in (.\srv\roscpp\*.srv) do (
    GenMsg.exe -t srv -n roscpp -o ..\RosSharp\Generated\srv\roscpp -i .\msg\roslib -i .\msg\std_msgs -i .\msg\roscpp %%f
)
pause
