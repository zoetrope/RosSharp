module RosSharp.GenMsg.Ast

type RosType = Bool
             | Int8
             | UInt8
             | Int16
             | UInt16
             | Int32
             | UInt32
             | Int64
             | UInt64
             | Float32
             | Float64
             | String
             | Time
             | Duration
             | FixedArray of RosType * int
             | VariableArray of RosType
             | UserDefinition of string list

type RosField = Variable of string
              | Constant of string * string

type RosMessage =
    | Node of RosType * RosField * RosMessage list
    | Leef of RosType * RosField

type RosService =
    | Service of RosMessage * RosMessage
