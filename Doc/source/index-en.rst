RosSharp Documentation
##################################################

Overview
***************************************************
RosSharp is C# client library for ROS (Robot Operating System).

License: Ms-PL
Copyright (c) 2012 zoetrope. All Rights Reserved. Licensed undear a Microsoft Permissive License (Ms-PL).

Source: https://github.com/zoetrope/RosSharp

Features:

* RosSharp is implemented based on Reactive Extensions
* NuGet installation support
* Create ROS Node
* Master/Slave/ParameterServer API XML-RPC Client
* Master/Slave/ParameterServer API XML-RPC Server
* Topic (TCPROS) Connection
* Service Connection
* RosOut (Logging Node)
* RosCore (Master Server & Parameter Server & RosOut Node)
* GenMsg (Code generation tool from .msg/.srv files)

The following features are not supported:

* Remapping Arguments
* Graph Resource Names (supports only the global name)
* Clock Node
* roslang



Requirements
***************************************************

* .NET Framework 4

* Reactive Extensions
* Common.Logging
* XML-RPC.NET

* F# Runtime 2.0 (for GenMsg)
* FParsec (for GenMsg)



Installation
***************************************************

use NuGet
==================================================

To install RosSharp, run the following command in the NuGet Package Manager Console ::

  PM> Install-Package RosSharp

Binary Package
==================================================

Downloads 

https://github.com/zoetrope/RosSharp/downloads

Settings
***************************************************



Initialization
==================================================

Configuring in your code
-------------------------------------------------

.. code-block:: csharp

   ROS.HostName = "";
   ROS.MasterUri
   ROS.XmlRpcTimeout
   ROS.SocketTimeout




app.config
-------------------------------------------------

configuration can be done in your app.config

.. code-block:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <configSections>
        <section name="rossharp" type="RosSharp.ConfigurationSection, RosSharp.NET40"/>
      </configSections>
      <rossharp>
        <ROS_MASTER_URI value="http://localhost:11311"/>
        <ROS_HOSTNAME value="localhost"/>
        <SOCKET_TIMEOUT value="1000"/>
        <XMLRPC_TIMEOUT value="1000"/>
      </rossharp>
    </configuration>



Environment
-------------------------------------------------




Logging
==================================================


Configuring in your code
-------------------------------------------------

.. code-block:: csharp

   LogManager.Adapter = new RosOutLoggerFactoryAdapter(properties);




app.config
-------------------------------------------------
see the Common.Logging Documentation


.. code-block:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <configSections>
        <sectionGroup name="common">
          <section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
        </sectionGroup>
      </configSections>

      <common>
        <logging>
          <factoryAdapter type="RosSharp.Utility.RosOutLoggerFactoryAdapter, RosSharp.NET40">
            <arg key="level" value="DEBUG" />
            <arg key="showLogName" value="true" />
            <arg key="showDataTime" value="true" />
            <arg key="dateTimeFormat" value="yyyy/MM/dd HH:mm:ss:fff" />
          </factoryAdapter>
        </logging>
      </common>
    </configuration>


Programming
***************************************************

using derective
==================================================

.. code-block:: csharp

  using RosSharp;


Create Node
==================================================

.. code-block:: csharp

  var node = ROS.CreateNode("Test");


Create Subscriber
==================================================

.. code-block:: csharp

  var subscriber = node.CreateSubscriber<RosSharp.std_msgs.String>("/chatter");
  subscriber.Subscribe(x => Console.WriteLine(x.data));


Create Publisher
==================================================

.. code-block:: csharp

  var publisher = node.CreatePublisher<RosSharp.std_msgs.String>("/chatter");
  publisher.OnNext(new RosSharp.std_msgs.String {data = "test"});

Create Service
==================================================


.. code-block:: csharp

  node.RegisterService<AddTwoInts, AddTwoInts.Request, AddTwoInts.Response>
                ("/add_two_ints", req => new AddTwoInts.Response {c = req.a + req.b});


Use Service
==================================================


.. code-block:: csharp

  var proxy = node.CreateProxy<AddTwoInts, AddTwoInts.Request, AddTwoInts.Response>("/add_two_ints");
  proxy(new AddTwoInts.Request() { a = 1, b = 2 }).Subscribe(x => Console.WriteLine(x.c));


ParameterServer
==================================================


.. code-block:: csharp


Application
***************************************************

RosCore
==================================================

RosCore is

* a ROS Master
* a ROS ParameterServer
* a rosout logging node

http://www.ros.org/wiki/roscore


Usage
--------------------------------------------------

> RosCore




GenMsg
==================================================
GenMsg is a tool that code generation from .msg / .srv format files.


Usage
--------------------------------------------------

> GenMsg

