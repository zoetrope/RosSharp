RosSharpドキュメント
##################################################

概要
***************************************************
RosSharpは、C#で書かれたROSのクライアントライブラリです。

`ROS <http://ros.org/>`_ は `Willow Garage <http://www.willowgarage.com/>`_ の開発するRobot Operating Systemです。

RosSharpを利用することで、ROSのトピックの購読と発行、サービスの提供や利用、パラメータの購読や読み書きがC#の
コードで書けるようになります。

* 開発者: `zoetrope <https://twitter.com/#!/zoetro>`_
* ライセンス: `BSD License <https://github.com/zoetrope/RosSharp/blob/master/License.txt>`_
* ソースコード: https://github.com/zoetrope/RosSharp
* NuGetパッケージ: http://nuget.org/packages/RosSharp

特徴
==================================================

* RosSharpはReactive Extensions(Rx)とTask Parallel Library(TPL)をベースにしており、非同期呼び出しのAPIを提供します。
* `NuGet <http://nuget.codeplex.com/>`_  でのインストールをサポートしています。
* ROSのクライアント機能

  * Publisher/Subscriber
  * Service Connection
  * Parameter Client

* ROSのサーバ機能

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

システム要件
***************************************************

* .NET Framework 4
* Reactive Extensions
* Common.Logging
* XML-RPC.NET
* NDesk.Options
* F# Runtime 2.0 (for GenMsg)
* FParsec (for GenMsg)
* log4net (for RosCore)
* Common.Logging.Log4Net (for RosCore)

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


クイックスタート
***************************************************

RosSharpは、
Reactive Extensions(Rx)とTask Parallel Library(TPL)をベースにしており、非同期呼び出しのAPIを提供します。


なお、下記のプログラムはusing句や、Main関数の記述を省略しています。

完全に動作するプログラムは、
を参照してください。


Subscriber - Publisherのサンプル
==================================================

SubscriberとPublisherを利用して、ノード間でメッセージをやり取りする


Subscriber
-------------------------------------------------


.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;
    // 
    var subscriber = node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
    subscriber.Subscribe(x => Console.WriteLine(x.data));
  }
  catch(Exception ex)
  {
    
  }


Publisher
-------------------------------------------------

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
  }

Service
==================================================

サービスの提供と利用のサンプル

Register Service
-------------------------------------------------

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;

    // サービスを
    var service = node.AdvertiseServiceAsync("/add_two_ints",
      new AddTwoInts(req => new AddTwoInts.Response {sum = req.a + req.b})).Result;
  }
  catch(Exception ex)
  {
  }

  

Use Service
-------------------------------------------------

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;

    // Serviceが登録されるまで待ちます。
    node.WaitForService("/add_two_ints").Wait();
    
    // Serviceが登録される前にProxyを作成した場合は失敗します。
    var proxy = node.ServiceProxyAsync<AddTwoInts>("/add_two_ints").Result;
    
    // サービスを呼び出します。
    var res = proxy.Invoke(new AddTwoInts.Request() {a = 1, b = 2});
    
    Console.WriteLine(res.sum);
  }
  catch(Exception ex)
  {
  }

Parameter
==================================================

複数のノード間で同一のパラメータを共有するための機能です。

パラメータの読み書きと、値が書き換わったときの監視(Subscribe)


データ型には、プリミティブ型、リスト型、ディクショナリ型を利用することが可能です。

プリミティブ型
-------------------------------------------------

int, double, stringなどを利用することができます。

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;

    //
    var param = node.PrimitiveParameterAsync<string>("rosversion").Result;
    
    // 
    param.Subscribe(x => Console.WriteLine(x));

    // 
    Console.WriteLine(param.Value);
    param.Value = "test";
  }
  catch(Exception ex)
  {
  }


リスト型
-------------------------------------------------

.. code-block:: csharp


  try
  {
    var node = ROS.InitNodeAsync("Test").Result;

    var param = node.ListParameterAsync<int>("rosversion").Result;
    
    param.Subscribe(x => Console.WriteLine(x));

    foreach(var i in param.Value)
    {
      Console.WriteLine("value[{0}] = {1}", i, 
    }
    param.Value.Add()
  }
  catch(Exception ex)
  {
  }


ディクショナリ型
-------------------------------------------------

.. code-block:: csharp

  try
  {
    var node = ROS.InitNodeAsync("Test").Result;

    var param = node.DynamicParameterAsync("").Result;
    Console.WriteLine(param.Value);
    
    param.Subscribe(x => Console.WriteLine(x));

    dynamic val = param.Value;
    
    val.
  }
  catch(Exception ex)
  {
  }


Dispose
==================================================

.. code-block:: csharp

  publisher.Dispose();
  
  // 
  subscriber.Dispose();
  
  // 
  proxy.Dispose();
  
  param.Dispose();

  // ノードに含まれるすべてのPublisher, Subscriber, Service, ServiceProxy, Parameterを終了する。
  node.Dispose();
  
  // 同一プロセスに含まれるすべてのノードを終了する。
  Ros.Dispose();

非同期型のDisposeAsync()も用意してあります。


TPLによる非同期プログラミング
==================================================



Asynchronous Programming with async/await
async/awaitによる非同期プログラミング
==================================================

Visual Studio 2012からasync/await構文を利用すると、同期型と同じような書き方で
非同期プログラミングを行うことができます。

なお、2012年6月ｘｘ日現在、
今後仕様が変わる可能性もあるので注意してください。

.. code-block:: csharp

  try
  {
      var node = await Ros.InitNodeAsync("test");

      var subscriber = await node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter");

      subscriber.Subscribe(x => Console.WriteLine(x.data));
  }
  catch(Exception ex)
  {
      Console.WriteLine(ex.Message);
  }


設定
***************************************************

RosSharpでは、各種設定をソースコード、アプリケーション構成ファイル(app.config)、
環境変数の3つの方法で設定することができます。

優先順位は、

ネットワーク設定
==================================================

コードでの設定
-------------------------------------------------

なお、ノードを生成する前に呼び出すようにしてください。

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

RosSharpは、rospy、rosjava、rosrubyなど、様々な言語によるROS実装と接続することが可能です。

ただし、roscppはいくつか問題があるため、そのままでは接続することができません。
下記の手順に従って、roscppを修正する必要があります。

* XmlRpc++は、XML-RPC.NETのレスポンスヘッダをパースすることができない。
==================================================

* roscppは、URIの一部を無視してしまう。
==================================================



アプリケーション
***************************************************

RosCore
==================================================

RosCoreは、トピックやサービスの管理、パラメータサーバ、ログの出力を管理するためのアプリケーションです。

ノードを生成する前に、必ず起動しておく必要があります。

なお、ROSが標準で提供しているroscoreを利用することも可能です。

http://www.ros.org/wiki/roscore


使い方
--------------------------------------------------

RosCoreは下記のようにコマンドラインから実行します。

> RosCore [-p port]

> RosCore -p 11311

-pオプションでポート番号を指定することができます。
-pオプションを省略した場合は、デフォルトの11311番が利用されます。

RosCore.exe.configファイルにおいて、ネットワークの設定やログの設定を変更することが可能です。

設定内容は、通常のノードと同じですので
を参照してください。

RosCoreでは、ロガーにlog4netを利用しており、各ノードから収集したログをファイルに保存します。


GenMsg
==================================================
GenMsgは.msg/.srv形式のファイルから、C#のソースコードを生成するためのツールです。

トピックやサービスで利用する型を

トピックでやり取りするデータ型を定義したい場合は .msg ファイルを作成し、
GenMsgを利用してソースコードを生成し、


.srvファイルでは、サービスの引数と戻り値の型をユーザが定義することができます。


使い方
--------------------------------------------------

> GenMsg -t type [-i include_dir -o output_dir] file_name ...

-tオプションには msg か srv を指定してください。

-i 他の.msgファイルで定義されている型を利用する場合は、そのファイルの含まれるディレクトリを指定します。

-o 生成したC#のソースコードの出力先を指定します。



例：

> GenMsg -t msg -i "..\msg\roslib" "..\msg\roslib\Time.msg"


