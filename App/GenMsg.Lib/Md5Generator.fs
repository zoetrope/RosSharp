module RosSharp.GenMsg.Md5Generator

open System
open System.IO;
open System.Text
open System.Security.Cryptography
open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Preprocessor
open FParsec;

type Md5GeneratorSetting() =
    static let mutable dirs = new ResizeArray<string>()
    static member includeDirectories = dirs

let getFileName (names : string list) =
   String.Join(@"\",names) + ".msg"

let searchMsgFile dirs fileName =
   dirs |> Seq.map(fun dir -> dir + "\\" + (getFileName fileName))
        |> Seq.find(fun file -> File.Exists(file))

let generateMd5 (input : string) =
   let md5 = new MD5CryptoServiceProvider()
   let data = Encoding.UTF8.GetBytes(input)
   let hash = md5.ComputeHash(data)
   let str = BitConverter.ToString(hash)
   str.Replace("-","").ToLower()

let rec getMd5OriginalName t =
   match t with
   | Bool -> "bool"
   | Int8 -> "int8"
   | UInt8 -> "uint8"
   | Int16 -> "int16"
   | UInt16 -> "uint16"
   | Int32 -> "int32"
   | UInt32 -> "uint32"
   | Int64 -> "int64"
   | UInt64 -> "uint64"
   | Float32 -> "float32"
   | Float64 -> "float64"
   | String -> "string"
   | Time -> "time"
   | Duration -> "duration"
   | FixedArray (x, size) -> getMd5OriginalName x + "[" + size.ToString() + "]"
   | VariableArray (x) -> getMd5OriginalName x + "[]"
   | UserDefinition (names) -> searchMsgFile Md5GeneratorSetting.includeDirectories names 
                                |> parseMessageFile 
                                |> getMessageMd5 //ユーザ定義型はMD5に置き換え

and getMd5Definition (msg : RosMessage) =
   match msg with
   | Leaf (t, Variable(name)) -> getMd5OriginalName t + " " + name
   | Node (t,Variable(f),m) -> getMessageMd5 m + " " + f //ユーザ定義型はMD5に置き換え
   | _ -> "" //ConstantはMD5計算には利用しない

and getMessageMd5Definition (msgs : RosMessage list) =
   msgs |> Seq.map(fun msg -> getMd5Definition msg) |> fun x -> String.Join("\n", x)

and getMessageMd5 (msgs : RosMessage list) =
   msgs |> getMessageMd5Definition
        |> generateMd5

let getServiceMd5Definitions (service : RosService) =
   match service with
   | Service (req, res) -> (getMessageMd5Definition req) + "---\n" + (getMessageMd5Definition res)

let getServiceMd5 (service : RosService) =
   match service with
   | Service (req, res) -> (getMessageMd5Definition req) + (getMessageMd5Definition res) |> generateMd5