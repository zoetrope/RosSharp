RosSharp Documentation (Japanease)
##################################################

概要
***************************************************
RosSharpは、C#で書かれたROSのクライアントライブラリです。

`ROS <http://ros.org/>`_ は `Willow Garage <http://www.willowgarage.com/>`_ の開発するRobot Operating Systemです。

RosSharpを利用することで、ROSのトピックの購読と発行、サービスの提供と利用、パラメータの購読と読み書きが
C#のコードで書けるようになります。

* 開発者: `zoetrope <https://twitter.com/#!/zoetro>`_
* ライセンス: `BSD License <https://github.com/zoetrope/RosSharp/blob/master/License.txt>`_
* ソースコード: https://github.com/zoetrope/RosSharp
* NuGetパッケージ: http://nuget.org/packages/RosSharp

特徴
==================================================

* RosSharpはReactive Extensions (Rx) とTask Parallel Library (TPL) をベースにしており、非同期呼び出しのAPIを提供します。
* `NuGet <http://nuget.codeplex.com/>`_  でのインストールをサポートしています。
* ROSのクライアント機能のサポート

  * Publisher/Subscriber
  * Service Connection
  * Parameter Client

* ROSのサーバ機能のサポート

  * Master
  * Parameter Server
  * RosOut (Logging Node)

* ツールの提供

  * RosCore (Master Server & Parameter Server & RosOut Node)
  * GenMsg (.msg/.srvファイルからコードを生成するツール)

以下の機能は未実装です。

* Remapping Arguments
* Graph Resource Names (global nameのみをサポートしています)
* Clock Node
* roslang

リリースノート
***************************************************

RosSharp 0.1.0
==================================================
* 2012/06/27
* First Release

システム要件
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

インストール方法
***************************************************

NuGetの利用
==================================================

* NuGet拡張をインストールしてください。(NuGet拡張を利用するためには、Visual Studio 2010 Professional以上が必要です。)

  * http://nuget.codeplex.com/

* RosSharpをインストールするには、NuGet Package Manager Consoleから下記のコマンドを実行してください。 ::

  PM> Install-Package RosSharp -Pre

なお、NuGetでインストールを行った場合、RosCoreとGenMsgはインストールされません。
RosCoreとGenMsgを利用する場合は、下記のバイナリパッケージをダウンロードしてください。

* プロジェクトの対象のフレームワークを.NET Framework 4 Client Profileから.NET Framework 4に変更してください。

バイナリパッケージの利用
==================================================

* 下記のリンクからバイナリパッケージをダウンロードします。

  * https://github.com/zoetrope/RosSharp/downloads

* Visual Studioを起動します。

* プロジェクトの参照設定にRosSharp.dllを追加します。

  * RosSharp.dll
  * System.Reactive.dll
  * CookComputing.XmlRpcServerV2.dll
  * CookComputing.XmlRpcV2.dll
  * Common.Logging.dll

* プロジェクトの対象のフレームワークを.NET Framework 4 Client Profileから.NET Framework 4に変更してください。

クイックスタート
***************************************************

このセクションでは、同期呼び出しによるRosSharpの使い方について説明します。

ここでのプログラムはusing句や、Main関数の記述を省略しています。
完全に動作するプログラムは、下記を参照してください。

https://github.com/zoetrope/RosSharp/tree/master/Sample

トピック (Publisher/Subscriber)
==================================================

トピックはノード間でメッセージを送受信するための機能です。
Publisherからメッセージを送信し、それをSubscriberが受け取ります。

Publisher
-------------------------------------------------

Publisherは名前と型を指定して、ノードから生成することができます。

PublisherはIObserver<T>を実装しています。

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

Subscriberは名前と型を指定して、ノードから生成することができます。

SubscriberはIObservable<T>を実装しており、Reactive Extensions (Rx)による柔軟なメッセージの
購読を行うことができます。

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


サービス
==================================================

サービスはノード間でのメソッド呼び出しのための機能です。

サービスは、リクエストメッセージとレスポンスメッセージを1つ持ちます。


Register Service
-------------------------------------------------

サービスの呼び出され側では、サービス名を指定して呼び出される関数の登録を行います。

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

サービスの呼び出し側では、サービス名を指定して呼び出すためのプロキシを生成します。

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

複数のノード間で同一のパラメータを共有するための機能です。

パラメータの読み書きと、値が書き換わったときの監視(Subscribe)機能を提供します。

データ型には、プリミティブ型、リスト型、ディクショナリ型を利用することが可能です。

プリミティブ型パラメータ
-------------------------------------------------

パラメータは名前と型を指定してノードから生成することができます。

ParameterはIObservable<T>を実装しており、Reactive Extensions (Rx)による柔軟なメッセージの
購読を行うことができます。

Parameter.Valueプロパティを利用して、パラメータの読み書きを行うことができます。

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

TPLによる非同期プログラミング
==================================================

RosSharpでは、Task Parallel Library (TPL) を利用して非同期スタイルのコードを記述することができます。

Subscriberを利用したコードは下記のように書き換えることができます。

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



async/awaitによる非同期プログラミング
==================================================

Visual Studio 2012から導入されるasync/await構文を利用すると、同期呼び出しと同じような書き方で
非同期プログラミングを行うことができます。
(下記はVisual Studio 2012 RCで動作します。)

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

.. _setting-ja:

設定
***************************************************

RosSharpでは、各種設定をソースコード、アプリケーション構成ファイル(app.config)、
環境変数の3つの方法で設定することができます。

ソースコードによる設定が最も優先順位が高く、アプリケーション構成ファイルによる
設定は最も優先順位が低くなります。

ネットワーク設定
==================================================

コードでの設定
-------------------------------------------------

ノードを生成する前に呼び出すようにしてください。

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

RosSharpでは、ログの出力にCommon.Loggingを利用しており、
設定によって利用するロガーを切り替えることが可能です。

デフォルトでは、RosOutLoggerが利用され、ログはRosOutへと送信されます。
これをコンソールに出力したり、log4netを利用してファイルやイベントログに出力することができます。

詳細については `Common.Logging Documentation <http://netcommon.sourceforge.net/docs/2.0.0/reference/html/index.html>`_ 
を参照してください。


コードでの設定
-------------------------------------------------

.. code-block:: csharp

   var properties = new NameValueCollection();
   
   // ログのレベルの設定
   properties["level"] = "DEBUG";
   
   // ログにログ名を出力するかどうかの設定
   properties["showLogName"] = "true";
   
   // ログに日付を出力するかどうかの設定
   properties["showDataTime"] = "true";
   
   // ログに出力する日付のフォーマットを設定
   properties["dateTimeFormat"] = "yyyy/MM/dd HH:mm:ss:fff";
   
   // 利用するロガーを設定
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



相互運用性
***************************************************

RosSharpは、rospy、rosjavaなど、様々な言語によるROS実装と接続することが可能です。

ただし、roscppはいくつか問題があるため、そのままでは接続することができません。
下記の手順に従って、roscppを修正する必要があります。

XmlRpc++がXML-RPC.NETのレスポンスヘッダをパースすることができない。
============================================================================

XmlRpc++には下記の問題があります。

* http://sourceforge.net/tracker/?func=detail&aid=1644372&group_id=70654&atid=528553
* http://sourceforge.net/projects/xmlrpcpp/forums/forum/240495/topic/2487516

この問題を修正する必要があります。

roscppがURIの一部を無視してしまう。
==================================================

ros_comm/clients/cpp/roscpp/src/libros/subscription.cppのSubscription::negotiateConnection関数において、SlaveのURLのポート番号の後ろの文字列が無視されています。

XmlRpcClientのインスタンスを作成する際に、ポート番号の後ろの文字列を渡す必要があります。

アプリケーション
***************************************************

RosCore
==================================================

RosCoreは、トピックやサービスの管理、パラメータサーバ、ログの出力を管理するためのアプリケーションです。

ノードを生成する前に、必ず起動しておく必要があります。

なお、ROSが標準で提供しているroscoreを利用することも可能です。

* http://www.ros.org/wiki/roscore


使い方
--------------------------------------------------

RosCoreは下記のようにコマンドラインから実行します。 ::

  > RosCore [-p port]

|

例 ::

  > RosCore -p 11311

|

-pオプション
  マスターサーバのポート番号を指定することができます。オプションを省略した場合は、デフォルトの11311番が利用されます。

RosCore.exe.configファイルにおいて、ネットワークの設定やログの設定を変更することが可能です。

設定内容は、通常のノードと同じですので :ref:`setting-ja` を参照してください。

RosCoreでは、ロガーにlog4netを利用しており、各ノードから収集したログをファイルに保存します。


GenMsg
==================================================
GenMsgは.msg/.srv形式のファイルから、C#のソースコードを生成するためのツールです。

.msg/.srv形式のファイルについては、下記のリンクを参考にしくください。

* http://www.ros.org/wiki/msg
* http://www.ros.org/wiki/srv

トピックで新しいメッセージ型を利用したい場合は、.msgファイルを作成し、GenMsgでC#のコードを生成します。

サービスで新しいサービス型を利用したい場合は、.srvファイルを作成し、GenMsgでC#のコードを生成します。


使い方
--------------------------------------------------

GenMsgは下記のようにコマンドラインから実行します。 ::

  > GenMsg -t msg|srv [-n namespace] [-o output_dir] [[-i include_dir]...] FileName...

|

例 ::

  > GenMsg -t msg -i "..\msg\roslib" "..\msg\roslib\Time.msg"

|


-tオプション
  Messageのコードを生成する場合はmsgを、Serviceのコードを生成する場合はsrvを指定します。

-nオプション
  生成するコードのネームスペースを指定します。

-oオプション
  生成したC#のソースコードの出力先を指定します。

-iオプション
  他の.msgファイルで定義されている型を利用する場合は、そのファイルの含まれるディレクトリを指定します。

FileName
  .msgファイルか.srvファイルを指定します。複数個のファイルを指定することが可能です。




