module RosSharp.GenMsg.Tests.Md5GeneratorTest

open NaturalSpec
open RosSharp.GenMsg.Md5Generator


[<Scenario>]
let ``Md5Generator String``()=
    Given "string data"
        |> When generateMd5
        |> It should equal "992ce8a1687cec8c8bd883ec73ca41d1"
        |> Verify
        
[<Scenario>]
let ``Md5Generator AddTwoInts``()=
    Given "int64 a\nint64 bint64 sum"
        |> When generateMd5
        |> It should equal "6a2e34150c00229791cc89ff309fff21"
        |> Verify

[<Scenario>]
let ``Md5Generator AddTwoInts Request``()=
    Given "int64 a\nint64 b"
        |> When generateMd5
        |> It should equal "36d09b846be0b371c5f190354dd3153e"
        |> Verify

[<Scenario>]
let ``Md5Generator AddTwoInts Response``()=
    Given "int64 sum"
        |> When generateMd5
        |> It should equal "b88405221c77b1878a3cbbfff53428d7"
        |> Verify


//TODO: HeaderだけでMD5を算出。それを展開した文字列でもう一度MD5を算出。
[<Scenario>]
let ``Md5Generator Time``()=
    Given "Header header\n" + 
          "time rostime"
        |> When generateMd5
        |> It should equal "09c1c9ce296734b4da898e62d1d0ae17"
        |> Verify
        
[<Scenario>]
let ``Md5Generator Time2``()=
    Given "2176decaecbce78abc3b96ef049fabed header\n" + 
          "time rostime"
        |> When generateMd5
        |> It should equal "09c1c9ce296734b4da898e62d1d0ae17"
        |> Verify
