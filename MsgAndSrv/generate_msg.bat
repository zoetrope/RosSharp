@echo off

SET PATH=..\App\GenMsg\bin\Debug;%PATH%

for %%f in (.\msg\rosgraph_msgs\*.msg) do (
    GenMsg.exe -t msg -n rosgraph_msgs -o ..\RosSharp\Generated\msg\rosgraph_msgs -i .\msg\roslib -i .\msg\std_msgs %%f
)

for %%f in (.\msg\std_msgs\*.msg) do (
    GenMsg.exe -t msg -n std_msgs -o ..\RosSharp\Generated\msg\std_msgs -i .\msg\roslib -i .\msg\std_msgs %%f
)

for %%f in (.\msg\roscpp\*.msg) do (
    GenMsg.exe -t msg -n roscpp -o ..\RosSharp\Generated\msg\roscpp -i .\msg\roslib -i .\msg\std_msgs %%f
)


pause
