module RosSharp.GenMsg.Tests.TestUtility

open FParsec.Primitives
open FParsec.CharParsers
open FParsec.Error

let extractExprs x =
    match x with
        | Success (x, _, _) -> x
        | Failure (x,y,_) -> failwith x
