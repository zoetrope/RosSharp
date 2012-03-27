module RosSharp.GenMsg.ServiceGenerator

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Md5Generator

open System
open System.Text

let generateServiceClass (ns : string) (name : string) (msgs : RosService) =
    let sb = new StringBuilder()
    
    sb.ToString()