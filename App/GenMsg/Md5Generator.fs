module RosSharp.GenMsg.Md5Generator

open System
open System.Text
open System.Security.Cryptography
open RosSharp.GenMsg.Ast

let getOriginalName t =
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

let generateMd5 (input : string) =
    let md5 = new MD5CryptoServiceProvider()
    let data = Encoding.UTF8.GetBytes(input)
    let hash = md5.ComputeHash(data)
    let str = BitConverter.ToString(hash)
    str.Replace("-","").ToLower()

let getTypeName (t : RosType) = 
    match t with
    | UserDefinition (names) -> String.Join("/", names)
    | FixedArray (x, size) -> getOriginalName x + "[" + size.ToString() + "]"
    | VariableArray (x) -> getOriginalName x + "[]"
    | x -> getOriginalName x

let getDefinition (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getTypeName t + " " + name

let getMessageDefinition (msgs : RosMessage list) =
    msgs |> Seq.map(fun msg -> getDefinition msg) |> fun x -> String.Join("\n", x)

let getServiceDefinitions (service : RosService) =
    match service with
    | Service (req, res) -> (getMessageDefinition req) + "---\n" + (getMessageDefinition res)

let getMessageMd5 (msgs : RosMessage list) =
    msgs |> getMessageDefinition
         |> generateMd5

let getServiceMd5 (service : RosService) =
    match service with
    | Service (req, res) -> (getMessageDefinition req) + (getMessageDefinition res) |> generateMd5

