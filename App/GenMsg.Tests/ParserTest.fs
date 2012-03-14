module RosSharp.MsgParser.Tests

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser


open FParsec.Primitives
open FParsec.CharParsers
open FParsec.Error

open NaturalSpec

let extractExprs x =
    match x with
        | Success (x, _, _) -> x
        | Failure (x,y,_) -> failwith x



[<Scenario>]
let ``simple test`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "string data"
        |> When runParserOnString pRosType context "bool"
        |> extractExprs
        |> It should equal (RosType.Bool)
        |> Verify

        
(*
test pRosType "bool "
test pRosType "int16[] "
test pRosType "float32[123] "
test pRosType "StdMsgs/String "
test pRosType "bools/hoge "

test pMember "bool data"
test pMember "int16[] data"
test pMember "float32[123] data"
test pMember "StdMsgs/String data"
test pMember "bools/hoge data"

test pMember "uint8 data=123"
*)

//Console.ReadKey() |> ignore