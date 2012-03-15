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

