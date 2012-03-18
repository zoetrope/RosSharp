module RosSharp.GenMsg.Tests.ParserTest

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Tests.TestUtility

open NaturalSpec

open FParsec.Primitives
open FParsec.CharParsers
open FParsec.Error

[<Scenario>]
let ``RosType PrimitiveType`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "bool "
        |> When runParserOnString pRosType context ""
        |> extractExprs
        |> It should equal (RosType.Bool)
        |> Verify

[<Scenario>]
let ``RosType VariableLengthArray`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "int16[] "
        |> When runParserOnString pRosType context ""
        |> extractExprs
        |> It should equal (RosType.VariableArray(RosType.Int16))
        |> Verify

[<Scenario>]
let ``RosType FixedArray`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "float32[123] "
        |> When runParserOnString pRosType context ""
        |> extractExprs
        |> It should equal (RosType.FixedArray(RosType.Float32,123))
        |> Verify
        
[<Scenario>]
let ``RosType UserDefinition`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "StdMsgs/String "
        |> When runParserOnString pRosType context ""
        |> extractExprs
        |> It should equal (RosType.UserDefinition(["StdMsgs"; "String"]))
        |> Verify

[<Scenario>]
let ``RosType Simple Definition`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "MultiArrayLayout "
        |> When runParserOnString pRosType context ""
        |> extractExprs
        |> It should equal (RosType.UserDefinition(["MultiArrayLayout";]))
        |> Verify

[<Scenario>]
let ``RosType bool like msg`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "bools/hoge "
        |> When runParserOnString pRosType context ""
        |> extractExprs
        |> It should equal (RosType.UserDefinition(["bools"; "hoge"]))
        |> Verify
        
[<Scenario>]
let ``pMember multi spaces`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "MultiArrayLayout  layout"
        |> When runParserOnString pMember context ""
        |> extractExprs
        |> It should equal (RosType.UserDefinition(["MultiArrayLayout";]), Variable("layout"))
        |> Verify
        