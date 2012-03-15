module RosSharp.GenMsg.Tests.RosMessageParserTest

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open RosSharp.GenMsg.Parser
open RosSharp.GenMsg.Tests.TestUtility

open NaturalSpec
open FParsec.Primitives
open FParsec.CharParsers
open FParsec.Error

[<Scenario>]
let ``RosType PrimitiveType`` ()=
    let context = { Levels = []; CurrentLevel = 0; NewLevel = 0 }
    Given "Parent parent\n  int16 child1\n  Child child2\n    float32 grandchild1\n  string child3\nfloat32 var1"
        |> When runParserOnString (many pRosMessage .>> eof) context ""
        |> extractExprs
        |> It should equal ([Node(UserDefinition ["Parent"],Variable "parent",[Leef (Int16,Variable "child1");Node(UserDefinition ["Child"],Variable "child2",[Leef (Float32,Variable "grandchild1")]);Leef (String,Variable "child3")]); Leef (Float32,Variable "var1")])
        |> Verify


        
