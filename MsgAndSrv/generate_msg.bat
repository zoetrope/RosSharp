@echo off

SET PATH=..\App\GenMsg\bin\Debug;%PATH%

for %%f in (.\msg\roslib\*.msg) do (
    GenMsg.exe -t msg -o ..\RosSharp\Generated\msg\roslib -i .\msg\roslib -i .\msg\std_msgs %%f
)

for %%f in (.\msg\std_msgs\*.msg) do (
    GenMsg.exe -t msg -n std_msgs -o ..\RosSharp\Generated\msg\std_msgs -i .\msg\roslib -i .\msg\std_msgs %%f
)


pause
