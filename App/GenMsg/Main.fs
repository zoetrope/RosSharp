open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Preprocessor

open FParsec
open System

[<EntryPoint>]
let main(argv: string[]) =
    let msg_input = "Parent parent
  int16 child1
  Child child2
    float32 grandchild1
  string child3
float32 var1"

    let msg_context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }

    match runParserOnString (many pRosMessage .>> eof) msg_context "" msg_input with
       | Success(result, _, _) ->
         printfn "result: %A" result
       | Failure(message, _, _) ->
         printfn "Failure: %s" message

    Console.ReadKey() |> ignore



    let srv_input = "int64 a
int64 b
---
int64 sum"

    let srv_context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }

    match runParserOnString (pRosService .>> eof) srv_context "" srv_input with
       | Success(result, _, _) ->
         printfn "result: %A" result
       | Failure(message, _, _) ->
         printfn "Failure: %s" message

    Console.ReadKey() |> ignore

    0


