RosSharpドキュメント
##################################################

概要
***************************************************
RosSharpは、ROSのC#クライアントライブラリです。

`ROS <http://ros.org/>`_ は `Willow Garage <http://www.willowgarage.com/>`_ の開発するRobot Operating Systemです。

* Author: `zoetrope <https://twitter.com/#!/zoetro>`_
* Source: https://github.com/zoetrope/RosSharp
* License: `BSD License <https://github.com/zoetrope/RosSharp/blob/master/License.txt>`_

特徴: 
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

未実装機能:
==================================================

* Remapping Arguments
* Graph Resource Names (supports only the global name)
* Clock Node
* roslang

システム要件
***************************************************

* .NET Framework 4
* Reactive Extensions
* Common.Logging
* XML-RPC.NET
* NDesk.Options
* F# Runtime 2.0 (for GenMsg)
* FParsec (for GenMsg)

インストール方法
***************************************************

NuGet
==================================================

RosSharpをインストールするには、NuGet Package Manager Consoleから下記のコマンドを実行してください。（準備中） ::

  PM> Install-Package RosSharp

バイナリパッケージ
==================================================

https://github.com/zoetrope/RosSharp/downloads



設定
***************************************************


初期化
==================================================


プログラムで
-------------------------------------------------

.. code-block:: csharp

   ROS.HostName = "192.168.1.11";
   ROS.MasterUri = new Uri("http://192.168.1.10:11311");
   ROS.TopicTimeout = 3000;
   ROS.XmlRpcTimeout = 3000;


設定ファイル
-------------------------------------------------

.. code-block:: xml

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <configSections>
        <section name="rossharp" type="RosSharp.ConfigurationSection, RosSharp"/>
      </configSections>
      <rossharp>
        <ROS_MASTER_URI value="http://localhost:11311"/>
        <ROS_HOSTNAME value="localhost"/>
        <ROS_TOPIC_TIMEOUT value="1000"/>
        <ROS_XMLRPC_TIMEOUT value="1000"/>
      </rossharp>
    </configuration>



環境変数
-------------------------------------------------

* ROS_MASTER_URI
* ROS_HOSTNAME
* ROS_TOPIC_TIMEOUT
* ROS_XMLRPC_TIMEOUT


ログ
==================================================

プログラム
-------------------------------------------------

.. code-block:: csharp

   LogManager.Adapter = new RosOutLoggerFactoryAdapter(properties);


設定ファイル
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
          <factoryAdapter type="RosSharp.RosOutLoggerFactoryAdapter, RosSharp">
            <arg key="level" value="DEBUG" />
            <arg key="showLogName" value="true" />
            <arg key="showDataTime" value="true" />
            <arg key="dateTimeFormat" value="yyyy/MM/dd HH:mm:ss:fff" />
          </factoryAdapter>
        </logging>
      </common>
    </configuration>


プログラミング
***************************************************

using derective
==================================================

.. code-block:: csharp

  using RosSharp;
  
  ROS.Initialize();



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

  node.RegisterService("/add_two_ints",new AddTwoInts(req => new AddTwoInts.Response {c = req.a + req.b})).Wait();


Use Service
==================================================


.. code-block:: csharp

  var proxy = node.CreateProxy<AddTwoInts>("/add_two_ints").Result;
  var res = proxy.Invoke(new AddTwoInts.Request() {a = 1, b = 2});
  Console.WriteLine(res.c);


ParameterServer
==================================================

.. code-block:: csharp

  var param = node.GetParameter<string>("rosversion");
  Console.WriteLine(param.Value);
  param.Value = "test";
  param.Subscribe(x => Console.WriteLine(x));



アプリケーション
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



