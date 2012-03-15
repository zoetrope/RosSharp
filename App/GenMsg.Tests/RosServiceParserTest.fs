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
let ``RosType PrimitiveType`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "int64 a\nint64 b\n---\nint64 sum"
        |> When runParserOnString (pRosService .>> eof) context ""
        |> extractExprs
        |> It should equal (([Leef (Int64,Variable "a"); Leef (Int64,Variable "b")],[Leef (Int64,Variable "sum")]))
        |> Verify
