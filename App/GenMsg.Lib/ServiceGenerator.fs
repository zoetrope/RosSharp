module RosSharp.GenMsg.ServiceGenerator

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Md5Generator
open RosSharp.GenMsg.MessageGenerator

open System
open System.Text

let createServiceHeader (name : string) =
    "    public class " + name + " : ServiceBase<" + name + ".Request," + name + ".Response>\r\n" +
    "    {\r\n"

let createServiceConstructor (name : string) =
    "        public " + name + "()\r\n" +
    "        {\r\n" + 
    "        }\r\n" +
    "        public " + name + "(Func<Request,Response> action)\r\n" +
    "            :base(action)\r\n" +
    "        {\r\n" + 
    "        }\r\n"
    
let createServiceMember ns name msgs =
    "        public override string ServiceType\r\n" +
    "        {\r\n" +
    "            get { return \"" + (getFullName ns name) + "\"; }\r\n" +
    "        }\r\n" +
    "        public override string Md5Sum\r\n" +
    "        {\r\n" +
    "            get { return \"" + getServiceMd5 msgs + "\"; }\r\n" +
    "        }\r\n" +
    "        public override string ServiceDefinition\r\n" +
    "        {\r\n" +
    "            get { return \"" + getServiceDefinition msgs + "\"; }\r\n" +
    "        }\r\n"

let createServiceFooter =
    "    }\r\n" + 
    "}\r\n"

    
let generateMessageClassForService (ns : string) (name : string) (msgs : RosMessage list) =
    let sb = new StringBuilder()
    
    sb.Append(createHeader name) |> ignore
    
    sb.Append(createDefaultConstructor name msgs) |> ignore
    sb.Append(createConstructor name msgs) |> ignore
    msgs |> List.iter (fun msg -> createProperty msg |> sb.Append |> ignore)
    
    sb.Append(createMessageType "" (ns + name) ) |> ignore
    sb.Append(createMessageMember msgs) |> ignore
    
    sb.Append(createSerializeMethod msgs) |> ignore
    sb.Append(createDeserializeMethod msgs) |> ignore
    sb.Append(createSerializeLengthProperty msgs) |> ignore

    sb.Append(createEqualityMethods name msgs) |> ignore

    sb.Append(createFooter) |> ignore

    sb.ToString()

let generateServiceClass (ns : string) (name : string) (service : RosService) =
    let sb = new StringBuilder()

    sb.Append(createNamespace ns) |> ignore
    sb.Append(createServiceHeader name) |> ignore
    sb.Append(createServiceConstructor name) |> ignore
    
    sb.Append(createServiceMember ns name service) |> ignore
    
    match service with
    | Service(req, res) -> 
        sb.Append(generateMessageClassForService name "Request" req) |> ignore
        sb.Append(generateMessageClassForService name "Response" res) |> ignore
    
    sb.Append(createServiceFooter) |> ignore

    sb.ToString()