RosSharp Documentation (English)
##################################################

Overview
***************************************************
RosSharp is ROS client library implemented by pure C#.

`ROS <http://ros.org/>`_ is Robot Operating System developed by `Willow Garage <http://www.willowgarage.com/>`_.

If you use a RosSharp, you can write a code that subscribe/publish a topic, provide/use a service, 
subscribe/read/write a parameter in C#.

* Author: `zoetrope <https://twitter.com/#!/zoetro>`_
* License: `BSD License <https://github.com/zoetrope/RosSharp/blob/master/License.txt>`_
* Source: https://github.com/zoetrope/RosSharp
* NuGet Package: http://nuget.org/packages/RosSharp

Features
==================================================

* RosSharp provides asynchronous operations, it is implemented based on Reactive Extensions (Rx) and Task Parallel Library(TPL).
* `NuGet <http://nuget.codeplex.com/>`_  installation support.
* Support ROS Client Features

  * Publisher/Subscriber
  * Service Connection
  * Parameter Client

* Support ROS Server Features

  * Master
  * Parameter Server
  * RosOut (Logging Node)

* Support Tools

  * RosCore (Master Server & Parameter Server & RosOut Node)
  * GenMsg (Code generation tool from .msg/.srv files).

The following features are not supported

* Remapping Arguments
* Graph Resource Names (supports only the global name)
* Clock Node
* roslang

Release Notes
***************************************************

RosSharp 0.1.0
==================================================
* 2012/06/27
* First Release


System Requirements
***************************************************

* .NET Framework 4

* Reactive Extensions

  * http://msdn.microsoft.com/en-us/data/gg577609.aspx

* Common.Logging

  * Apache License, Version 2.0.
  * http://netcommon.sourceforge.net/

* XML-RPC.NET

  * MIT X11 license
  * http://xml-rpc.net/

* NDesk.Options (for GenMsg, RosCore)

  * MIT/X11 license.
  * http://www.ndesk.org/Options

* F# Runtime 2.0 (for GenMsg)

  * http://www.microsoft.com/download/details.aspx?id=13450

* FParsec (for GenMsg)

  * BSD License
  * http://www.quanttec.com/fparsec/

* log4net (for RosCore)

  * Apache License, Version 2.0.
  * http://logging.apache.org/log4net/index.html

Installation
***************************************************

Use NuGet
==================================================

* Install NuGet (NuGet requires Visual Studio 2010)

  * http://nuget.codeplex.com/

* To install RosSharp, run the following command in the NuGet Package Manager Console ::

  PM> Install-Package RosSharp -Pre

If you use NuGet, RosCore and GenMsg is not installed.
You need download a following binary package.

* Change Target Framework from .NET Framework 4 Client Profile to .NET Framework 4. 

Use binary package
==================================================

* Download a file through the following link.

  * https://github.com/zoetrope/RosSharp/downloads

* Start Visual Studio and create new project.

* Add to project reference following assemblies.

  * RosSharp.dll
  * System.Reactive.dll
  * CookComputing.XmlRpcServerV2.dll
  * CookComputing.XmlRpcV2.dll
  * Common.Logging.dll

* Change Target Framework from .NET Framework 4 Client Profile to .NET Framework 4. 

Quick Start
***************************************************

In this section, I explain how to use the RosSharp with synchronous operation.

These Program omit using directive, main function, and class definition.
If you need complete program, you will reference following link.

https://github.com/zoetrope/RosSharp/tree/master/Sample

Topic (Publisher/Subscriber)
==================================================

Topic is a feature that is message transportation between nodes.
The Publisher send messages to subscribers. The Subscriber receive messages from publisher.

Publisher
-------------------------------------------------

Publisher can generate from a Node with specific name and message type.

Publisher class is implemented an IObserver<T>.

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;
    var publisher = node.PublisherAsync<RosSharp.std_msgs.String>("/chatter").Result;
    
    for(int i=0;i<100;i++)
    {
      publisher.OnNext(new RosSharp.std_msgs.String() {data = "test message"};);
      Thread.Sleep(TimeSpan.FromSeconds(1));
    }
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.Message);
  }

Subscriber
-------------------------------------------------

Subscriber can generate from a Node with specific name and message type.

Subscriber class is implemented an IObservable<T>, you can use extention methods in the
Reactive Extensions (Rx).

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;
    var subscriber = node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
    subscriber.Subscribe(x => Console.WriteLine(x.data));
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.Message);
  }

Service
==================================================

Service is provided for remote procedure call between nodes.

Service has a request message and a response message.

Register Service
-------------------------------------------------

In the Service callee, you specify name and register a function to be called.


.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;

    var service = node.AdvertiseServiceAsync("/add_two_ints",
      new AddTwoInts(req => new AddTwoInts.Response {sum = req.a + req.b})).Result;
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.Message);
  }

  

Use Service
-------------------------------------------------

In the Service caller, you specify name and create a service proxy for call a service.

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;

    node.WaitForService("/add_two_ints").Wait();
    
    var proxy = node.ServiceProxyAsync<AddTwoInts>("/add_two_ints").Result;
    
    var res = proxy.Invoke(new AddTwoInts.Request() {a = 1, b = 2});
    
    Console.WriteLine(res.sum);
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.Message);
  }

Parameter
==================================================

Parameter is feature of shared parameter between nodes.

Parameter provide a subscribe value and read/write value.

Supported parameter type is primitive type, list type, and dictionary type.

Primitive Parameter
-------------------------------------------------

Parameter can generate from a Node with specific name and type.

Parameter class is implemented an IObservable<T>, you can use extention methods in the
Reactive Extensions (Rx).

Parameter.Value property provides a feature of read/write value;

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;
    var param = node.PrimitiveParameterAsync<string>("rosversion").Result;
    
    param.Subscribe(x => Console.WriteLine(x));

    Console.WriteLine(param.Value);
    param.Value = "test";
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.Message);
  }

Asynchronous Programming with Task Parallel Library
============================================================================

You can write an asynchronous style code using the Task Parallel Library (TPL).

Sample code for Subscriber can be rewritten as following code.

.. code-block:: csharp

  Ros.InitNodeAsync("Test")
      .ContinueWith(node =>
      {
          return node.Result.SubscriberAsync<RosSharp.std_msgs.String>("/chatter");
      })
      .Unwrap()
      .ContinueWith(subscriber =>
      {
          subscriber.Result.Subscribe(x => Console.WriteLine(x.data));
      })
      .ContinueWith(res =>
      {
          Console.WriteLine(res.Exception.Message);
      }, TaskContinuationOptions.OnlyOnFaulted);


Asynchronous Programming with async/await
==================================================

If you use async/await syntax coming Visual Studio 2012, you will be able to write an 
asynchronous operation that likes a synchronous operation.
(The following code can be run the Visual Studio 2012 RC)

.. code-block:: csharp

  try
  {
      var node = await Ros.InitNodeAsync("Test");
      var subscriber = await node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter");
      subscriber.Subscribe(x => Console.WriteLine(x.data));
  }
  catch(Exception ex)
  {
      Console.WriteLine(ex.Message);
  }

.. _setting-en:

Settings
***************************************************

RosSharp can be set in three different ways, source code, application configuration file (app.config),
and environment valiable.

Setting by Source code is highest priority.
Setting by application configuration file is lowest priority.

Network Setting
==================================================

Setting in a code
-------------------------------------------------

Please set before you generate the node.

.. code-block:: csharp

   // local network address of a ROS Node
   RosManager.HostName = "192.168.1.11";
   // XML-RPC URI of the Master
   RosManager.MasterUri = new Uri("http://192.168.1.10:11311");
   // Timeout in milliseconds on a ROS TOPIC
   RosManager.TopicTimeout = 3000;
   // Timeout in milliseconds on a XML-RPC proxy method call
   RosManager.XmlRpcTimeout = 3000;

Setting by a app.config
-------------------------------------------------

.. code-block:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <configSections>
        <section name="rossharp" type="RosSharp.ConfigurationSection, RosSharp"/>
      </configSections>
      <rossharp>
        <ROS_MASTER_URI value="http://192.168.1.10:11311"/>
        <ROS_HOSTNAME value="192.168.1.11"/>
        <ROS_TOPIC_TIMEOUT value="3000"/>
        <ROS_XMLRPC_TIMEOUT value="3000"/>
      </rossharp>
    </configuration>

Setting by Environment Variable
-------------------------------------------------

* ROS_MASTER_URI
* ROS_HOSTNAME
* ROS_TOPIC_TIMEOUT
* ROS_XMLRPC_TIMEOUT

Logging Setting
==================================================

RosSharp use the Common.Logging, logger type can change by setting.

Default setting is RosOutLogger. It send logs to RosOut.

You will be able to change logger.
Example,  using log4net , it write log to file or event log.

See the `Common.Logging Documentation <http://netcommon.sourceforge.net/docs/2.0.0/reference/html/index.html>`_


Setting in a code
-------------------------------------------------

.. code-block:: csharp

   var properties = new NameValueCollection();
   properties["level"] = "DEBUG";
   properties["showLogName"] = "true";
   properties["showDataTime"] = "true";
   properties["dateTimeFormat"] = "yyyy/MM/dd HH:mm:ss:fff";
   LogManager.Adapter = new RosOutLoggerFactoryAdapter(properties);

Setting by a app.config
-------------------------------------------------

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
          <factoryAdapter type="RosSharp.RosOutLoggerFactoryAdapter, RosSharp">
            <arg key="level" value="DEBUG" />
            <arg key="showLogName" value="true" />
            <arg key="showDataTime" value="true" />
            <arg key="dateTimeFormat" value="yyyy/MM/dd HH:mm:ss:fff" />
          </factoryAdapter>
        </logging>
      </common>
    </configuration>


Interoperability
***************************************************

RosSharp has interoperability with other implementations of ROS, rospy and rosjava.

But, roscpp has problems for interoperability.
If you want interoperability with roscpp, you should fix the roscpp following steps.


XmlRpc++ can not parse a response header of XML-RPC.NET
============================================================================

XmlRpc++ has a following issue.

* http://sourceforge.net/tracker/?func=detail&aid=1644372&group_id=70654&atid=528553
* http://sourceforge.net/projects/xmlrpcpp/forums/forum/240495/topic/2487516

You should fix this issue.

roscpp ignore a part of slave uri
==================================================

In ros_comm/clients/cpp/roscpp/src/libros/subscription.cpp 
Subscription::negotiateConnection ignore the string after a port number in slave url.

You should add the tail string when creating XmlRpcClient instance.

Application
***************************************************

RosCore
==================================================

RosCore is an application that manages topics and services, has parameter server, manages output log.

You need launch the RosCore before generate nodes.

You will be able to use the roscore provided by original ROS instead of RosSharp's RosCore.

* http://www.ros.org/wiki/roscore


Usage
--------------------------------------------------

You will launch the RosCore by command line ::

  > RosCore [-p port]

|

example ::

  > RosCore -p 11311

|

-p:
  Port number for Master server. If you don't specify it, default number 11311 will be used.


You can set the network setting and logger setting by RosCore.exe.config. See :ref:`setting-en`.

RosCore default logger is log4net.

GenMsg
==================================================
GenMsg is a tool that code generation from .msg/.srv format files.

See the following links for .msg/.srv format files.

* http://www.ros.org/wiki/msg
* http://www.ros.org/wiki/srv

If you want use a new Message type in Topic, you should write a .msg file and generate C# code by GenMsg.

If you want use a new Service type, you should write a .srv file and generate C# code by GenMsg.

Usage
--------------------------------------------------

You will launch the GenMsg by command line ::

  > GenMsg -t msg|srv [-n namespace] [-o output_dir] [[-i include_dir]...] FileName...

|

example ::

  > GenMsg -t msg -i "..\msg\roslib" "..\msg\roslib\Time.msg"

|

-t:
  If you generate the code for Message, then msg.
  If you generate the code for Service, then srv.

-n:
  Namespace for generated code.

-o:
  Output directory for generated code.

-i:
  If you use the message type defined by the other file, you should specify the directory contains these file.

FileName:
  .msg file or .srv file. You can specify multiple files.
