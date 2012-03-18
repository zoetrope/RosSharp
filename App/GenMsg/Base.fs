module RosSharp.GenMsg.Base

open FParsec

type UserState = {
 Levels: int list // これまでのインデントのスタック
 CurrentLevel: int // 解析中のインデント
 NewLevel: int // 新しく検出したインデント
}

type Parser<'a> = Parser<'a, UserState>

let test p str =
   match run p str with
   | Success(result, _, _)   -> printfn "Success: %A" result
   | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

   
let extractExprs x =
    match x with
        | Success (x, _, _) -> x
        | Failure (x,y,_) -> failwith x