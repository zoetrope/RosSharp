module RosSharp.GenMsg.Tests.Md5GeneratorTest

open NaturalSpec
open RosSharp.GenMsg.Md5Generator
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open FParsec

let messageDir = @"..\..\..\..\MsgAndSrv\msg\"
let serviceDir = @"..\..\..\..\MsgAndSrv\srv\"

let getMessageFile name = messageDir + name + ".msg"
let getServiceFile name = serviceDir + name + ".srv"

let testMessageMD5 name =
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    let fileName = getMessageFile name
    Md5GeneratorSetting.includeDirectories.AddRange([messageDir + "std_msgs"])
    let ast = parseMessageFile fileName
    getMessageMd5 ast 
    
let testServiceMD5 name =
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    let fileName = getServiceFile name
    Md5GeneratorSetting.includeDirectories.AddRange([messageDir + "std_msgs"])
    let ast = parseServiceFile fileName
    getServiceMd5 ast 

(**************** roscpp ****************)
[<Scenario>]
let ``Md5Generator Logger``()=
    Given "roscpp/Logger"
        |> When testMessageMD5
        |> It should equal "a6069a2ff40db7bd32143dd66e1f408e"
        |> Verify
        

(**************** rosgraph_msgs ****************)
[<Scenario>]
let ``Md5Generator Log``()=
    Given "rosgraph_msgs/Log"
        |> When testMessageMD5
        |> It should equal "acffd30cd6b6de30f120938c17c593fb"
        |> Verify
        
(**************** std_msgs ****************)
[<Scenario>]
let ``Md5Generator Bool``()=
    Given "std_msgs/Bool"
        |> When testMessageMD5
        |> It should equal "8b94c1b53db61fb6aed406028ad6332a"
        |> Verify

        
[<Scenario>]
let ``Md5Generator Byte``()=
    Given "std_msgs/Byte"
        |> When testMessageMD5
        |> It should equal "ad736a2e8818154c487bb80fe42ce43b"
        |> Verify

[<Scenario>]
let ``Md5Generator ByteMultiArray``()=
    Given "std_msgs/ByteMultiArray"
        |> When testMessageMD5
        |> It should equal "70ea476cbcfd65ac2f68f3cda1e891fe"
        |> Verify

[<Scenario>]
let ``Md5Generator Char``()=
    Given "std_msgs/Char"
        |> When testMessageMD5
        |> It should equal "1bf77f25acecdedba0e224b162199717"
        |> Verify

[<Scenario>]
let ``Md5Generator ColorRGBA``()=
    Given "std_msgs/ColorRGBA"
        |> When testMessageMD5
        |> It should equal "a29a96539573343b1310c73607334b00"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Duration``()=
    Given "std_msgs/Duration"
        |> When testMessageMD5
        |> It should equal "3e286caf4241d664e55f3ad380e2ae46"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Empty``()=
    Given "std_msgs/Empty"
        |> When testMessageMD5
        |> It should equal "d41d8cd98f00b204e9800998ecf8427e"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Float32``()=
    Given "std_msgs/Float32"
        |> When testMessageMD5
        |> It should equal "73fcbf46b49191e672908e50842a83d4"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Float32MultiArray``()=
    Given "std_msgs/Float32MultiArray"
        |> When testMessageMD5
        |> It should equal "6a40e0ffa6a17a503ac3f8616991b1f6"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Float64``()=
    Given "std_msgs/Float64"
        |> When testMessageMD5
        |> It should equal "fdb28210bfa9d7c91146260178d9a584"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Float64MultiArray``()=
    Given "std_msgs/Float64MultiArray"
        |> When testMessageMD5
        |> It should equal "4b7d974086d4060e7db4613a7e6c3ba4"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Header``()=
    Given "std_msgs/Header"
        |> When testMessageMD5
        |> It should equal "2176decaecbce78abc3b96ef049fabed"
        |> Verify

[<Scenario>]
let ``Md5Generator Int8``()=
    Given "std_msgs/Int8"
        |> When testMessageMD5
        |> It should equal "27ffa0c9c4b8fb8492252bcad9e5c57b"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Int8MultiArray``()=
    Given "std_msgs/Int8MultiArray"
        |> When testMessageMD5
        |> It should equal "d7c1af35a1b4781bbe79e03dd94b7c13"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Int16``()=
    Given "std_msgs/Int16"
        |> When testMessageMD5
        |> It should equal "8524586e34fbd7cb1c08c5f5f1ca0e57"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Int16MultiArray``()=
    Given "std_msgs/Int16MultiArray"
        |> When testMessageMD5
        |> It should equal "d9338d7f523fcb692fae9d0a0e9f067c"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Int32``()=
    Given "std_msgs/Int32"
        |> When testMessageMD5
        |> It should equal "da5909fbe378aeaf85e547e830cc1bb7"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Int32MultiArray``()=
    Given "std_msgs/Int32MultiArray"
        |> When testMessageMD5
        |> It should equal "1d99f79f8b325b44fee908053e9c945b"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Int64``()=
    Given "std_msgs/Int64"
        |> When testMessageMD5
        |> It should equal "34add168574510e6e17f5d23ecc077ef"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Int64MultiArray``()=
    Given "std_msgs/Int64MultiArray"
        |> When testMessageMD5
        |> It should equal "54865aa6c65be0448113a2afc6a49270"
        |> Verify
        
[<Scenario>]
let ``Md5Generator MultiArrayDimension``()=
    Given "std_msgs/MultiArrayDimension"
        |> When testMessageMD5
        |> It should equal "4cd0c83a8683deae40ecdac60e53bfa8"
        |> Verify
        
[<Scenario>]
let ``Md5Generator MultiArrayLayout``()=
    Given "std_msgs/MultiArrayLayout"
        |> When testMessageMD5
        |> It should equal "0fed2a11c13e11c5571b4e2a995a91a3"
        |> Verify
        
[<Scenario>]
let ``Md5Generator String``()=
    Given "std_msgs/String"
        |> When testMessageMD5
        |> It should equal "992ce8a1687cec8c8bd883ec73ca41d1"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Time``()=
    Given "std_msgs/Time"
        |> When testMessageMD5
        |> It should equal "cd7166c74c552c311fbcc2fe5a7bc289"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt8``()=
    Given "std_msgs/UInt8"
        |> When testMessageMD5
        |> It should equal "7c8164229e7d2c17eb95e9231617fdee"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt8MultiArray``()=
    Given "std_msgs/UInt8MultiArray"
        |> When testMessageMD5
        |> It should equal "82373f1612381bb6ee473b5cd6f5d89c"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt16``()=
    Given "std_msgs/UInt16"
        |> When testMessageMD5
        |> It should equal "1df79edf208b629fe6b81923a544552d"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt16MultiArray``()=
    Given "std_msgs/UInt16MultiArray"
        |> When testMessageMD5
        |> It should equal "52f264f1c973c4b73790d384c6cb4484"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt32``()=
    Given "std_msgs/UInt32"
        |> When testMessageMD5
        |> It should equal "304a39449588c7f8ce2df6e8001c5fce"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt32MultiArray``()=
    Given "std_msgs/UInt32MultiArray"
        |> When testMessageMD5
        |> It should equal "4d6a180abc9be191b96a7eda6c8a233d"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt64``()=
    Given "std_msgs/UInt64"
        |> When testMessageMD5
        |> It should equal "1b2a79973e8bf53d7b53acb71299cb57"
        |> Verify
        
[<Scenario>]
let ``Md5Generator UInt64MultiArray``()=
    Given "std_msgs/UInt64MultiArray"
        |> When testMessageMD5
        |> It should equal "6088f127afb1d6c72927aa1247e945af"
        |> Verify
        

(**************** roscpp ****************)
[<Scenario>]
let ``Md5Generator Empty Service``()=
    Given "roscpp/Empty"
        |> When testServiceMD5
        |> It should equal "d41d8cd98f00b204e9800998ecf8427e"
        |> Verify

[<Scenario>]
let ``Md5Generator GetLoggers``()=
    Given "roscpp/GetLoggers"
        |> When testServiceMD5
        |> It should equal "32e97e85527d4678a8f9279894bb64b0"
        |> Verify

[<Scenario>]
let ``Md5Generator SetLoggerLevel``()=
    Given "roscpp/SetLoggerLevel"
        |> When testServiceMD5
        |> It should equal "51da076440d78ca1684d36c868df61ea"
        |> Verify

(**************** sample ****************)
[<Scenario>]
let ``Md5Generator AddTwoInts``()=
    Given "sample/AddTwoInts"
        |> When testServiceMD5
        |> It should equal "6a2e34150c00229791cc89ff309fff21"
        |> Verify

        
(**************** std_srvs ****************)
[<Scenario>]
let ``Md5Generator std_srvs Empty Service``()=
    Given "std_srvs/Empty"
        |> When testServiceMD5
        |> It should equal "d41d8cd98f00b204e9800998ecf8427e"
        |> Verify
