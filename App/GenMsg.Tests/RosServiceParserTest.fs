module RosSharp.GenMsg.Tests.RosServiceParserTest

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Tests.TestUtility

open NaturalSpec
open FParsec.Primitives
open FParsec.CharParsers
open FParsec.Error

[<Scenario>]
let ``RosService AddTwoInts`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "int64 a\nint64 b\n---\nint64 sum"
        |> When runParserOnString (pRosService .>> eof) context ""
        |> extractExprs
        |> It should equal ((Service([Leaf (Int64,Variable "a"); Leaf (Int64,Variable "b")],[Leaf (Int64,Variable "sum")])))
        |> Verify
