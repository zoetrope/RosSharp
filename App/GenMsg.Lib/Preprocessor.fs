module RosSharp.GenMsg.Preprocessor

open System
open System.Text

//TODO: 改行コードは統一したい。ROS的には\n

let deleteLineComment (s : string) =
    let strs = s.Split('\n')
    let sb = new StringBuilder()

    //TODO: もっとF#らしいコードを。
    for str in strs do
        let index = str.IndexOf("#")
        if index = -1 then
            sb.Append(str) |> ignore
        else
            //TODO: StringLiteralの中に#があった場合は？
            let temp = str.Substring(0,index)
            sb.Append(temp) |> ignore
        sb.Append("\n") |> ignore
        
    done
    sb.ToString()