module RosSharp.GenMsg.Md5Generator

open System
open System.Text
open System.Security.Cryptography

let generateMd5 (input : string) =
    let md5 = new MD5CryptoServiceProvider()
    let data = Encoding.UTF8.GetBytes(input)
    let hash = md5.ComputeHash(data)
    let str = BitConverter.ToString(hash)
    str.Replace("-","").ToLower()


