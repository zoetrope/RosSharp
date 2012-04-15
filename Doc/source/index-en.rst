RosSharp Documentation
##################################################

Overview
***************************************************
RosSharp is C# client library for ROS.

`ROS <http://ros.org/>`_ is Robot Operating System developed by `Willow Garage <http://www.willowgarage.com/>`_.

* Author: `zoetrope <https://twitter.com/#!/zoetro>`_
* License: `BSD License <https://github.com/zoetrope/RosSharp/blob/master/License.txt>`_
* Source: https://github.com/zoetrope/RosSharp
* NuGet Package: http://nuget.org/packages/RosSharp

Features
==================================================

* RosSharp is implemented based on Reactive Extensions
* `NuGet <http://nuget.codeplex.com/>`_  installation support
* Create ROS Node
* Master/Slave/ParameterServer API XML-RPC Client
* Master/Slave/ParameterServer API XML-RPC Server
* Topic (TCPROS) Connection
* Service Connection
* RosOut (Logging Node)
* RosCore (Master Server & Parameter Server & RosOut Node)
* GenMsg (Code generation tool from .msg/.srv files)

The following features are not supported
==================================================

* Remapping Arguments
* Graph Resource Names (supports only the global name)
* Clock Node
* roslang

System Requirements
***************************************************

* .NET Framework 4
* Reactive Extensions
* Common.Logging
* XML-RPC.NET
* NDesk.Options
* F# Runtime 2.0 (for GenMsg)
* FParsec (for GenMsg)

Installation
***************************************************

use NuGet
==================================================

To install RosSharp, run the following command in the NuGet Package Manager Console ::

  PM> Install-Package RosSharp -Pre

Binary Package
==================================================

* Download a file through the following link.

  * https://github.com/zoetrope/RosSharp/downloads

* Start Visual Studio and create new project.
* Add reference to downloaded dll files.

Settings
***************************************************

Network Setting
==================================================

Setting in a code
-------------------------------------------------

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

See the `Common.Logging Documentation <http://netcommon.sourceforge.net/docs/2.0.0/reference/html/index.html>`_

Programming
***************************************************

using directive
==================================================

Add the following Using directive to the your code.

.. code-block:: csharp

  using RosSharp;

Create Node
==================================================

.. code-block:: csharp

  var node = RosManager.CreateNode("Test");

Subscriber
==================================================

.. code-block:: csharp

  var subscriber = node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
  subscriber.Subscribe(x => Console.WriteLine(x.data));


Publisher
==================================================

.. code-block:: csharp

  var publisher = node.CreatePublisherAsync<RosSharp.std_msgs.String>("/chatter").Result;
  publisher.OnNext(new RosSharp.std_msgs.String() {data = "test message"};);

Register Service
==================================================

.. code-block:: csharp

  node.RegisterServiceAsync("/add_two_ints",
    new AddTwoInts(req => new AddTwoInts.Response {sum = req.a + req.b})).Wait();

Use Service
==================================================

.. code-block:: csharp

  var proxy = node.CreateProxyAsync<AddTwoInts>("/add_two_ints").Result;
  var res = proxy.Invoke(new AddTwoInts.Request() {a = 1, b = 2});
  Console.WriteLine(res.sum);

ParameterServer
==================================================

.. code-block:: csharp

  var param = node.CreateParameterAsync<string>("rosversion").Result;
  Console.WriteLine(param.Value);
  param.Value = "test";
  param.Subscribe(x => Console.WriteLine(x));

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

> RosCore -p 11311




GenMsg
==================================================
GenMsg is a tool that code generation from .msg / .srv format files.


Usage
--------------------------------------------------

> GenMsg -t msg -i "..\msg\roslib" "..\msg\roslib\Time.msg"

