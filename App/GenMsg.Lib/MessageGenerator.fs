module RosSharp.GenMsg.MessageGenerator

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Md5Generator

open System
open System.Text

let rec getTypeName t =
    match t with
    | Bool -> "bool"
    | Int8 -> "sbyte"
    | UInt8 -> "byte"
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
    | UInt8 -> "bw.Write(" + v + ");"
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
    | UInt8 -> v + " = br.ReadByte();"
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
    | UInt8 -> "1"
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
    | UserDefinition (names) -> v + ".SerializeLength()"

    
let getInit (t : RosType) (name : string) =
    match t with
    | UserDefinition (names) -> name + " = new " + String.Join(".", names) + "();\n"
    | FixedArray (x, size) -> name + " = new " + getTypeName x + "[" + size.ToString() + "];\n"
    | VariableArray (x) -> name + " = new List<" + getTypeName x + ">();\n"
    | String -> name + " = string.Empty;\n"
    | _ -> ""
    

let getInitialize (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getInit t name


let createDefaultConstructor (name : string) (msgs : RosMessage list) =
    "        public " + name + "()\n" +
    "        {\n" + 
    (msgs |> Seq.map(fun msg -> getInitialize msg) |> fun x -> String.Join("",x) ) + 
    "        }\n"

let createProperty (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> "        public " + getTypeName t + " " + name + " { get; set; }\n"
    | Leaf (t, Constant(name, value)) -> "       public const " + getTypeName t + " " + name + " = " + value + ";\n"

let createMessageMember ns name msgs =
    "        public string MessageType\n" +
    "        {\n" +
    "            get { return \"" + ns + "/" + name + "\"; }\n" +
    "        }\n" +
    "        public string Md5Sum\n" +
    "        {\n" +
    "            get { return \"" + getMessageMd5 msgs + "\"; }\n" +
    "        }\n" +
    "        public string MessageDefinition\n" +
    "        {\n" +
    "            get { return @\"" + getMessageDefinition msgs + "\"; }\n" +
    "        }\n"

let createSerialize (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getSerialize t name
    //| Leaf (t, Constant(name, value)) -> 
    
    
let createSerializeMethod (msgs : RosMessage list) =
    "        public void Serialize(BinaryWriter bw)\n" +
    "        {\n" + 
    (msgs |> Seq.map (fun msg -> "            " + (createSerialize msg) + "\n" ) |> Seq.reduce (+) )+
    "        }\n"
    
let createSerializeLength (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getSerializeLength t name
    //| Leaf (t, Constant(name, value)) -> 

let createSerializeLengthProperty (msgs : RosMessage list) =
    "        public int SerializeLength\n" +
    "        {\n" + 
    "            get { return " + String.Join(" + ",(msgs |> Seq.map (fun msg -> createSerializeLength msg))) + "; }\n" +
    "        }\n"
    
let createConstructor (name : string) (msgs : RosMessage list) =
    "        public " + name + "(BinaryReader br)\n" +
    "        {\n" + 
    "            Deserialize(br);\n" + 
    "        }\n"

let createDeserialize (msg : RosMessage) = 
    match msg with
    | Leaf (t, Variable(name)) -> getDeserialize t name
    //| Leaf (t, Constant(name, value)) -> 
   
let createDeserializeMethod (msgs : RosMessage list) =
    "        public void Deserialize(BinaryReader br)\n" +
    "        {\n" + 
    (msgs |> Seq.map (fun msg -> "            " + (createDeserialize msg) + "\n" ) |> Seq.reduce (+) )+
    "        }\n"


let getHashCode (msg : RosMessage) =
    match msg with
    | Leaf (t, Variable(name)) -> name + ".GetHashCode();"


let getEquals (msg : RosMessage) =
    match msg with
    | Leaf (t, Variable(name)) -> "other." + name + ".Equals(" + name + ")"

let createEqualityMethods (name : string) (msgs : RosMessage list) =
    "        public bool Equals(" + name + " other)\n" +
    "        {\n" + 
    "            if (ReferenceEquals(null, other)) return false;\n" + 
    "            if (ReferenceEquals(this, other)) return true;\n" + 
    "            return " + String.Join(" && ",msgs |> Seq.map(fun msg -> getEquals msg)) + ";\n" +
    "        }\n" + 
    
    "        public override bool Equals(object obj)\n" +
    "        {\n" + 
    "            if (ReferenceEquals(null, obj)) return false;\n" + 
    "            if (ReferenceEquals(this, obj)) return true;\n" + 
    "            if (obj.GetType() != typeof(" + name + ")) return false;\n" + 
    "            return Equals((" + name + ")obj);\n" + 
    "        }\n" + 

    "        public override int GetHashCode()\n" +
    "        {\n" + 
    "            unchecked\n" + 
    "            {\n" + 
    "                int result = 0;\n" +
    (msgs |> Seq.map(fun msg -> "                result = (result * 397) ^ " + getHashCode msg) 
          |> fun x -> String.Join("\n", x)
    ) + "\n" +
    "                return result;\n" +
    "            }\n" + 
    "        }\n"



let createHeader (ns : string) (name : string) =
    "using System;\n" +
    "using System.IO;\n" +
    "using System.Linq;\n" +
    "using RosSharp.Message;\n" +
    "using System.Collections.Generic;\n" +
    "namespace RosSharp." + ns + "\n" +
    "{\n" + 
    "    public class " + name + " : IMessage\n" +
    "    {\n"

let createFooter =
    "    }\n" + 
    "}\n"

let generateMessageClass (ns : string) (name : string) (msgs : RosMessage list) =
    let sb = new StringBuilder()
    
    sb.Append(createHeader ns name) |> ignore
    
    sb.Append(createDefaultConstructor name msgs) |> ignore
    sb.Append(createConstructor name msgs) |> ignore
    msgs |> List.iter (fun msg -> createProperty msg |> sb.Append |> ignore)
    
    sb.Append(createMessageMember ns name msgs) |> ignore
    
    sb.Append(createSerializeMethod msgs) |> ignore
    sb.Append(createDeserializeMethod msgs) |> ignore
    sb.Append(createSerializeLengthProperty msgs) |> ignore

    sb.Append(createEqualityMethods name msgs) |> ignore

    sb.Append(createFooter) |> ignore

    sb.ToString()