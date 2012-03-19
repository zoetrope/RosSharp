open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Preprocessor
open RosSharp.GenMsg.MessageGenerator
open RosSharp.GenMsg.ServiceGenerator

open FParsec
open System
open System.IO

[<EntryPoint>]
let main(argv: string[]) =
    
    //let text = File.ReadAllText("../../msg/roslib/Header.msg")
    //let text = File.ReadAllText("../../msg/roslib/Time.msg")
    //let text = File.ReadAllText("../../msg/std_msgs/Int8MultiArray.msg")
    let text = File.ReadAllText("../../msg/std_msgs/String.msg")
                |> fun t -> t.Replace("\r\n", "\n")
                |> deleteLineComment
    
    printfn "%A" text

    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }

    let ast = text 
                |> runParserOnString (spaces >>. pRosMessages .>> eof) context ""
                |> extractExprs
    
    printfn "%A" ast
    
    //let result = ast |> generateMessageClass "roslib" "Header"
    //let result = ast |> generateMessageClass "roslib" "Time"
    //let result = ast |> generateMessageClass "std_msgs" "Int8MultiArray"
    let result = ast |> generateMessageClass "std_msgs" "String"

    printfn "%s" result

    Console.ReadKey() |> ignore

    0


