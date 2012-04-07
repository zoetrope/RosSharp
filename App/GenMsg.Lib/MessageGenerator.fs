module RosSharp.GenMsg.MessageGenerator

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Md5Generator

open System
open System.Text




let rec getOriginalName t =
   match t with
   | Bool -> "bool"
   | Int8 -> "int8"
   | Char -> "char"
   | UInt8 -> "uint8"
   | Byte -> "byte"
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
   | FixedArray (x, size) -> getOriginalName x + "[" + size.ToString() + "]"
   | VariableArray (x) -> getOriginalName x + "[]"
   | UserDefinition (names) -> String.Join("/", names)


let getConstValue v =
    match v with
        | IntValue(x) -> string(x)
        | FloatValue(x) -> string(x)
        | StringValue(x) -> "\"" + x + "\""

let getDefinition (msg : RosMessage) =
   match msg with
   | Leaf (t, Variable(name)) -> getOriginalName t + " " + name
   | Leaf (t, Constant(name, value)) -> getOriginalName t + " " + name  + "=" + getOriginalConstValue value
   | Node (t,Variable(f),m) -> getOriginalName t + " " + f

let getMessageDefinition (msgs : RosMessage list) =
   msgs |> Seq.map(fun msg -> getDefinition msg) |> fun x -> String.Join(@"\n", x)

let getServiceDefinition (service : RosService) =
   match service with
   | Service (req, res) -> (getMessageDefinition req) + @"---\n" + (getMessageDefinition res)





let rec getTypeName t =
    match t with
    | Bool -> "bool"
    | Int8 -> "sbyte"
    | Char -> "sbyte"
    | UInt8 -> "byte"
    | Byte -> "byte"
    | Int16 -> "short"
    | UInt16 -> "ushort"
    | Int32 -> "int"
    | UInt32 -> "uint"
    | Int64 -> "long"
    | UInt64 -> "ulong"
    | Float32 -> "float"
    | Float64 -> "double"
    | String -> "string"
    | Time -> "DateTime"
    | Duration -> "TimeSpan"
    | FixedArray (x, size) -> getTypeName x + "[]"
    | VariableArray (x) -> "List<" + getTypeName x + ">"
    | UserDefinition (names) -> String.Join(".", names)

let rec getSerialize t v =
    match t with
    | Bool -> "bw.Write(" + v + ");"
    | Int8 -> "bw.Write(" + v + ");"
    | Char -> "bw.Write(" + v + ");"
    | UInt8 -> "bw.Write(" + v + ");"
    | Byte -> "bw.Write(" + v + ");"
    | Int16 -> "bw.Write(" + v + ");"
    | UInt16 -> "bw.Write(" + v + ");"
    | Int32 -> "bw.Write(" + v + ");"
    | UInt32 -> "bw.Write(" + v + ");"
    | Int64 -> "bw.Write(" + v + ");"
    | UInt64 -> "bw.Write(" + v + ");"
    | Float32 -> "bw.Write(" + v + ");"
    | Float64 -> "bw.Write(" + v + ");"
    | String -> "bw.WriteUtf8String(" + v + ");"
    | Time -> "bw.WriteDateTime(" + v + ");"
    | Duration -> "bw.WriteTimeSpan(" + v + ");"
    | FixedArray (x, size) -> "for(int i=0; i<" + size.ToString() + "; i++) { " + getSerialize x (v + "[i]") + "}"
    | VariableArray (x) -> "bw.Write(" + v + ".Count); " + "for(int i=0; i<" + v + ".Count; i++) { " + getSerialize x (v + "[i]") + "}"
    | UserDefinition (names) -> v + ".Serialize(bw);"
    
let rec getDeserialize t v =
    match t with
    | Bool -> v + " = br.ReadBoolean();"
    | Int8 -> v + " = br.ReadSByte();"
    | Char -> v + " = br.ReadSByte();"
    | UInt8 -> v + " = br.ReadByte();"
    | Byte -> v + " = br.ReadByte();"
    | Int16 -> v + " = br.ReadInt16();"
    | UInt16 -> v + " = br.ReadUInt16();"
    | Int32 -> v + " = br.ReadInt32();"
    | UInt32 -> v + " = br.ReadUInt32();"
    | Int64 -> v + " = br.ReadInt64();"
    | UInt64 -> v + " = br.ReadUInt64();"
    | Float32 -> v + " = br.ReadSingle();"
    | Float64 -> v + " = br.ReadDouble();"
    | String -> v + " = br.ReadUtf8String();"
    | Time -> v + " = br.ReadDateTime();"
    | Duration -> v + " = br.ReadTimeSpan();"
    | FixedArray (x, size) -> "for(int i=0; i<" + size.ToString() + "; i++) { " + v + "[i] = " + getDeserialize x (v + "[i]") + "}"
    | VariableArray (x) -> v + " = new List<" + getTypeName x + ">(br.ReadInt32()); " + "for(int i=0; i<" + v + ".Count; i++) { " + getDeserialize x (v + "[i]") + "}"
    | UserDefinition (names) -> v + " = new " + String.Join(".", names) + "();"

let rec getSerializeLength t v =
    match t with
    | Bool -> "1"
    | Int8 -> "1"
    | Char -> "1"
    | UInt8 -> "1"
    | Byte -> "1"
    | Int16 -> "2"
    | UInt16 -> "2"
    | Int32 -> "4"
    | UInt32 -> "4"
    | Int64 -> "8"
    | UInt64 -> "8"
    | Float32 -> "4"
    | Float64 -> "8"
    | String -> "4 + " + v + ".Length"
    | Time -> "8"
    | Duration -> "8"
    | FixedArray (x, size) -> v + ".Sum(x => " + getSerializeLength x "x" + ")"
    | VariableArray (x) -> v + ".Sum(x => " + getSerializeLength x "x" + ")"
    | UserDefinition (names) -> v + ".SerializeLength"

    
let getInit (t : RosType) (name : string) =
    match t with
    | UserDefinition (names) -> name + " = new " + String.Join(".", names) + "();\r\n"
    | FixedArray (x, size) -> name + " = new " + getTypeName x + "[" + size.ToString() + "];\r\n"
    | VariableArray (x) -> name + " = new List<" + getTypeName x + ">();\r\n"
    | String -> name + " = string.Empty;\r\n"
    | _ -> ""
    

let getInitialize (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getInit t name
    | Leaf (_, Constant(_,_)) -> ""


let createDefaultConstructor (name : string) (msgs : RosMessage list) =
    "        public " + name + "()\r\n" +
    "        {\r\n" + 
    (msgs |> Seq.map(fun msg -> getInitialize msg) 
          |> Seq.filter(fun x -> x <> "")
          |> Seq.map(fun x -> "            " + x)
          |> fun x -> String.Join("",x) ) + 
    "        }\r\n"

let createProperty (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> "        public " + getTypeName t + " " + name + " { get; set; }\r\n"
    | Leaf (t, Constant(name, value)) -> "        public const " + getTypeName t + " " + name + " = " + getConstValue value + ";\r\n"
    
let getFullName ns name =
    if String.IsNullOrEmpty(ns) then
        name
    else
        ns + "/" + name

let createMessageType ns name =
    "        public string MessageType\r\n" +
    "        {\r\n" +
    "            get { return \"" + (getFullName ns name) + "\"; }\r\n" +
    "        }\r\n"

let createMessageMember msgs =
    "        public string Md5Sum\r\n" +
    "        {\r\n" +
    "            get { return \"" + getMessageMd5 msgs + "\"; }\r\n" +
    "        }\r\n" +
    "        public string MessageDefinition\r\n" +
    "        {\r\n" +
    "            get { return \"" + getMessageDefinition msgs + "\"; }\r\n" +
    "        }\r\n"

let createSerialize (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getSerialize t name
    | Leaf (_, Constant(_, _)) -> ""
    
    
let createSerializeMethod (msgs : RosMessage list) =
    "        public void Serialize(BinaryWriter bw)\r\n" +
    "        {\r\n" + 
    (msgs |> Seq.map(fun msg -> createSerialize msg)
          |> Seq.filter(fun x -> x <> "")
          |> Seq.map (fun x -> "            " + x + "\r\n" ) 
          |> fun msg -> String.Join("", msg)) +
    "        }\r\n"
    
let createSerializeLength (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getSerializeLength t name
    | Leaf (_, Constant(_, _)) -> ""

let createSerializeLengthProperty (msgs : RosMessage list) =
    "        public int SerializeLength\r\n" +
    "        {\r\n" + 
    "            get { return " + 
    if msgs.IsEmpty then
        "0; }\r\n"
    else
        String.Join(" + ",(msgs |> Seq.map (fun msg -> createSerializeLength msg)
                                |> Seq.filter(fun x -> x <> ""))) + "; }\r\n"
    + "        }\r\n"
    
let createConstructor (name : string) (msgs : RosMessage list) =
    "        public " + name + "(BinaryReader br)\r\n" +
    "        {\r\n" + 
    "            Deserialize(br);\r\n" + 
    "        }\r\n"

let createDeserialize (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getDeserialize t name
    | Leaf (_, Constant(_, _)) -> ""
   
let createDeserializeMethod (msgs : RosMessage list) =
    "        public void Deserialize(BinaryReader br)\r\n" +
    "        {\r\n" + 
    (msgs |> Seq.map(fun msg -> createDeserialize msg)
          |> Seq.filter(fun x -> x <> "")
          |> Seq.map (fun x -> "            " + x + "\r\n" )
          |> fun msg -> String.Join("", msg) ) +
    "        }\r\n"


let getHashCode (msg : RosMessage) =
    match msg with
    | Leaf (t, Variable(name)) -> name + ".GetHashCode();"
    | Leaf (_, Constant(_, _)) -> ""


let getEquals (msg : RosMessage) =
    match msg with
    | Leaf (t, Variable(name)) -> "other." + name + ".Equals(" + name + ")"
    | Leaf (_, Constant(_, _)) -> ""
    

let createEqualityMethods (name : string) (msgs : RosMessage list) =
    "        public bool Equals(" + name + " other)\r\n" +
    "        {\r\n" + 
    "            if (ReferenceEquals(null, other)) return false;\r\n" + 
    "            if (ReferenceEquals(this, other)) return true;\r\n" + 
    if msgs.IsEmpty then
        "            return true;\r\n"
    else
        "            return " + String.Join(" && ",msgs |> Seq.map(fun msg -> getEquals msg)
                                                        |> Seq.filter(fun x -> x <> "")) + ";\r\n"
    + "        }\r\n" + 
    
    "        public override bool Equals(object obj)\r\n" +
    "        {\r\n" + 
    "            if (ReferenceEquals(null, obj)) return false;\r\n" + 
    "            if (ReferenceEquals(this, obj)) return true;\r\n" + 
    "            if (obj.GetType() != typeof(" + name + ")) return false;\r\n" + 
    "            return Equals((" + name + ")obj);\r\n" + 
    "        }\r\n" + 

    "        public override int GetHashCode()\r\n" +
    "        {\r\n" + 
    "            unchecked\r\n" + 
    "            {\r\n" + 
    "                int result = 0;\r\n" +
    (msgs |> Seq.map(fun msg -> getHashCode msg)
          |> Seq.filter(fun x -> x <> "")
          |> Seq.map(fun x -> "                result = (result * 397) ^ " + x) 
          |> fun x -> String.Join("\r\n", x)
    ) + "\r\n" +
    "                return result;\r\n" +
    "            }\r\n" + 
    "        }\r\n"

    
let getFullNameSpace ns =
    if String.IsNullOrEmpty(ns) then
        "RosSharp"
    else
        "RosSharp." + ns

        
let createNamespace (ns : string) =
    "using System;\r\n" +
    "using System.Collections.Generic;\r\n" +
    "using System.IO;\r\n" +
    "using System.Linq;\r\n" +
    "using RosSharp.Message;\r\n" +
    "using RosSharp.Service;\r\n" +
    "namespace " + getFullNameSpace ns + "\r\n" +
    "{\r\n"

let createHeader (name : string) =
    "    public class " + name + " : IMessage\r\n" +
    "    {\r\n"

let createFooter =
    "    }\r\n"

let createNamespaceFooter =
    "}\r\n"

let generateMessageClass (ns : string) (name : string) (msgs : RosMessage list) =
    let sb = new StringBuilder()
    
    sb.Append(createNamespace ns) |> ignore
    sb.Append(createHeader name) |> ignore
    
    sb.Append(createDefaultConstructor name msgs) |> ignore
    sb.Append(createConstructor name msgs) |> ignore
    msgs |> List.iter (fun msg -> createProperty msg |> sb.Append |> ignore)
    
    sb.Append(createMessageType ns name) |> ignore
    sb.Append(createMessageMember msgs) |> ignore
    
    sb.Append(createSerializeMethod msgs) |> ignore
    sb.Append(createDeserializeMethod msgs) |> ignore
    sb.Append(createSerializeLengthProperty msgs) |> ignore

    sb.Append(createEqualityMethods name msgs) |> ignore

    sb.Append(createFooter) |> ignore
    sb.Append(createNamespaceFooter) |> ignore

    sb.ToString()