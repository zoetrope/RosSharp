module RosSharp.GenMsg.Md5Generator

open System
open System.Text
open System.Security.Cryptography

let msg = "string data"
let req = "int64 a\nint64 b"
let res = "int64 sum"
let srv = "int64 a\nint64 bint64 sum"

let data = Encoding.UTF8.GetBytes(srv)
let md5 = new MD5CryptoServiceProvider()
let hash = md5.ComputeHash(data)
printfn "hash: %A" hash
let str = BitConverter.ToString(hash)
printfn "hash: %A" str

//std_msgs/String
//"string data"
//  992ce8a1687cec8c8bd883ec73ca41d1

//"test_ros/AddTwoIntsRequest"
//srv: "int64 a\nint64 bint64 sum"
//  6a2e34150c00229791cc89ff309fff21
//req: "int64 a\nint64 b"
//  36d09b846be0b371c5f190354dd3153e
//res: "int64 sum"
//  b88405221c77b1878a3cbbfff53428d7
Console.ReadKey() |> ignore
