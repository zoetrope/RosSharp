module RosSharp.GenMsg.Generator

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Preprocessor
open RosSharp.GenMsg.MessageGenerator
open RosSharp.GenMsg.ServiceGenerator
open RosSharp.GenMsg.Md5Generator

open FParsec
open System
open System.IO

let getTypeName fileName =
    Path.GetFileNameWithoutExtension(fileName)

let saveFile outputDir typeName text = 
    File.AppendAllText(outputDir + @"\" + typeName + ".cs", text)

let generateMessage (fileName : string) (ns : string) (outputDir : string) (includeDirs : ResizeArray<string>) =
    Md5GeneratorSetting.includeDirectories.AddRange(includeDirs)
    let ast = parseMessageFile fileName
    let typeName = getTypeName fileName
    let text = generateMessageClass ns typeName ast
    saveFile outputDir typeName text
    text

let generateService (fileName : string) (ns : string) (outputDir : string) (includeDirs : ResizeArray<string>) =
    Md5GeneratorSetting.includeDirectories.AddRange(includeDirs)
    let ast = parseServiceFile fileName
    let typeName = getTypeName fileName
    let text = generateServiceClass ns typeName ast
    saveFile outputDir typeName text
    text

