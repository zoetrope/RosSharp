module RosSharp.GenMsg.Generator

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Preprocessor
open RosSharp.GenMsg.MessageGenerator
open RosSharp.GenMsg.ServiceGenerator

open FParsec
open System
open System.IO

let generateMessage (fileName : string) (ns : string) (includeDirs : string list) =
    let ast = parseMessageFile fileName
    let text = generateMessageClass ns fileName ast
    text

let generateService (fileName : string) (ns : string) (includeDirs : string list) =
    let ast = parseServiceFile fileName
    let text = generateServiceClass ns fileName ast
    text

