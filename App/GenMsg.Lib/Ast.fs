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

type ConstValue = IntValue   of int32
                | FloatValue of float
                | StringValue of string

type RosField = Variable of string
              | Constant of string * ConstValue

type RosMessage =
    | Node of RosType * RosField * RosMessage list
    | Leaf of RosType * RosField

type RosService =
    | Service of RosMessage list * RosMessage list
