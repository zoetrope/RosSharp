RosSharpドキュメント
##################################################

概要
***************************************************
RosSharpは、ROSのC#クライアントライブラリです。

`ROS <http://ros.org/>`_ は `Willow Garage <http://www.willowgarage.com/>`_ の開発するRobot Operating Systemです。

* Author: `zoetrope <https://twitter.com/#!/zoetro>`_
* License: `BSD License <https://github.com/zoetrope/RosSharp/blob/master/License.txt>`_
* Source: https://github.com/zoetrope/RosSharp
* NuGet Package: http://nuget.org/packages/RosSharp

特徴
==================================================

* RosSharpはReactive Extensionsをベースにしています。
* `NuGet <http://nuget.codeplex.com/>`_  でのインストールをサポートしています。
* ノードの作成
* Master/Slave/ParameterServer API XML-RPC Client
* Master/Slave/ParameterServer API XML-RPC Server
* Topic (TCPROS) Connection
* Service Connection
* RosOut (Logging Node)
* RosCore (Master Server & Parameter Server & RosOut Node)
* GenMsg (Code generation tool from .msg/.srv files)

以下の機能は実装していません。

* Remapping Arguments
* Graph Resource Names (global nameのみをサポートしています)
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

NuGetの利用
==================================================

RosSharpをインストールするには、NuGet Package Manager Consoleから下記のコマンドを実行してください。 ::

  PM> Install-Package RosSharp -Pre

バイナリパッケージ
==================================================

* 下記のリンクからバイナリパッケージをダウンロードします。

  * https://github.com/zoetrope/RosSharp/downloads

* Visual Studioを起動します。
* ダウンロードしたdllを参照に追加します。

設定
***************************************************

ネットワーク設定
==================================================

コードでの設定
-------------------------------------------------

.. code-block:: csharp

   // ローカルネットワークのホスト名またはIPアドレス
   Ros.HostName = "192.168.1.11";
   // Masterへの接続URI
   Ros.MasterUri = new Uri("http://192.168.1.10:11311");
   // ROS TOPICのタイムアウト時間[msec]
   Ros.TopicTimeout = 3000;
   // XML-RPCのメソッド呼び出しのタイムアウト時間[msec]
   Ros.XmlRpcTimeout = 3000;

app.configでの設定
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

環境変数での設定
-------------------------------------------------

* ROS_MASTER_URI
* ROS_HOSTNAME
* ROS_TOPIC_TIMEOUT
* ROS_XMLRPC_TIMEOUT

ログ設定
==================================================

コードでの設定
-------------------------------------------------

.. code-block:: csharp

   var properties = new NameValueCollection();
   properties["level"] = "DEBUG";
   properties["showLogName"] = "true";
   properties["showDataTime"] = "true";
   properties["dateTimeFormat"] = "yyyy/MM/dd HH:mm:ss:fff";
   LogManager.Adapter = new RosOutLoggerFactoryAdapter(properties);

app.configでの設定
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

詳細については `Common.Logging Documentation <http://netcommon.sourceforge.net/docs/2.0.0/reference/html/index.html>`_ を参照してください。

プログラミング
***************************************************



同期型
==================================================


.. code-block:: csharp

  try
  {
    var node = ROS.CreateNodeAsync("Test").Result;
    
    
    // 
    var subscriber = node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
    subscriber.Subscribe(x => Console.WriteLine(x.data));
    
    

  }
  catch(Exception ex)
  {
    
  }

Subscriber
==================================================

.. code-block:: csharp



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


Asynchronous Programming with async/await
async/awaitによる非同期プログラミング
==================================================

Visual Studio 11を利用すると

.. code-block:: csharp

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using RosSharp;

  namespace RosSample1
  {
      class Program
      {
          static void Main(string[] args)
          {
              SampleSubscriber();

              Console.WriteLine("Press Any Key.");
              Console.ReadKey();
          }

          static async void SampleSubscriber()
          {
              try
              {
                  var node = await Ros.CreateNodeAsync("test");

                  var subscriber = await node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter");

                  subscriber.Subscribe(x => Console.WriteLine(x.data));
              }
              catch(Exception ex)
              {
                  Console.WriteLine(ex.Message);
              }
          }
      }
  }

互換性
***************************************************

rospy, rosjava

roscpp

* XmlRpc++は、XML-RPC.NETのレスポンスヘッダをパースすることができない。
* roscppは、URIの一部を無視してしまう。



アプリケーション
***************************************************

RosCore
==================================================

RosCore has the following features:

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



