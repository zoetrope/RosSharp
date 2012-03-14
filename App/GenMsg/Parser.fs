module RosSharp.GenMsg.Parser

open RosSharp.GenMsg.Ast
open RosSharp.GenMsg.Base
open FParsec
open System
open FParsec
open FParsec.Primitives
open FParsec.CharParsers

// プリミティブ型のパーサ
let pBool = stringReturn "bool" RosType.Bool
let pInt8 = stringReturn "int8" RosType.Int8
let pUInt8 = stringReturn "uint8" RosType.UInt8
let pInt16 = stringReturn "int16" RosType.Int16
let pUInt16 = stringReturn "uint16" RosType.UInt16
let pInt32 = stringReturn "int32" RosType.Int32
let pUInt32 = stringReturn "uint32" RosType.UInt32
let pInt64 = stringReturn "int64" RosType.Int64
let pUInt64 = stringReturn "uint64" RosType.UInt64
let pFloat32 = stringReturn "float32" RosType.Float32
let pFloat64 = stringReturn "float64" RosType.Float64
let pString = stringReturn "string" RosType.String
let pTime = stringReturn "time" RosType.Time
let pDuration = stringReturn "duration" RosType.Duration

let pRosType, pRosTypeRef = createParserForwardedToRef()

let pIdentifier =
   let isIdentifierFirstChar c = isLetter c || c = '_'
   let isIdentifierChar c = isLetter c || isDigit c || c = '_'
   many1Satisfy2L isIdentifierFirstChar isIdentifierChar "identifier"

// ユーザ定義型のパーサ。 ( / で区切られた文字列)
let pUserDefinition = parse {
    let! list = sepBy pIdentifier (pstring "/")
    return UserDefinition(list)
}

// プリミティブ型のパーサ
let pPrimitive = choice[attempt(pBool)
                        attempt(pInt8)
                        attempt(pUInt8)
                        attempt(pInt16)
                        attempt(pUInt16)
                        attempt(pInt32)
                        attempt(pUInt32)
                        attempt(pInt64)
                        attempt(pUInt64)
                        attempt(pFloat32)
                        attempt(pFloat64)
                        attempt(pString)
                        attempt(pTime)
                        attempt(pDuration)]

// 固定長配列のパーサ
let pFixedArray = parse {
    let! t = pPrimitive <|> pUserDefinition
    let! size = between (pstring "[") (pstring "]") pint32
    return FixedArray(t,size)
}

// 可変長配列のパーサ
let pVariableArray = parse {
    let! t = (pPrimitive <|> pUserDefinition)
    let! _ = pstring "[]"
    return VariableArray(t)
}

// 型のパーサ
do pRosTypeRef := choice[attempt(pFixedArray .>> spaces1)
                         attempt(pVariableArray .>> spaces1)
                         attempt(pPrimitive .>> spaces1)
                         attempt(pUserDefinition .>> spaces1)]

//TODO:
let pValue = pstring ""

// 定数のパーサ (定数=値)
let pConst = parse {
    let! name, value = tuple2 pIdentifier (pstring "=" >>. pValue)
    return Constant(name,value)
}

// 変数のパーサ (変数名)
let pVariable = parse {
    let! name = pIdentifier
    return Variable(name)
}

// メンバのパーサ (型名 変数 or 定数)
let pMember : Parser<_> = tuple2 pRosType (pVariable <|> pConst)

//*****************************************************************
// オフサイドルールによるパーサ
// 下記の記事のコードを利用させていただいております。
// http://d.hatena.ne.jp/htid46/20111207/1323210001

let updateNewLevel newLevel c = { c with NewLevel = newLevel }
let updateCurrentLevel currentLevel c = { c with CurrentLevel = currentLevel }
let updateLevels levels c = { c with Levels = levels }

// インデントの深さを返すパーサ
let pSpace : Parser<_> = manyChars (pchar ' ')
let pIndent = pSpace |>> String.length

// 同じ深さの場合成功するパーサ
let pSameLevel state = parse {
    do! (fun stream -> if state.CurrentLevel = state.NewLevel then
                           Reply(ReplyStatus.Ok)
                       else
                           Reply(ReplyStatus.Error, messageError "same level error"))
    |>> ignore
    return ()
}

// 改行と共に次の行のインデントの深さを調べるパーサ
// EOFだったら次の行はインデントの深さが0とする
let pEndOfLine =
    attempt (parse {
        do! eof
        do! updateUserState (updateNewLevel 0)
    }) <|>  parse {
        do! newline |>> ignore
        let! indent = pIndent
        let! state = getUserState
        do! updateUserState (updateNewLevel indent)
    }

// インデントが深くなっていたら成功するパーサ
let pOpenParen level = parse {
    let! state = getUserState
    do! (fun stream -> if level < state.NewLevel then
                           Reply(ReplyStatus.Ok)
                       else
                           Reply(ReplyStatus.Error, messageError "open paren error"))
    |>> ignore
    do! updateUserState (updateLevels (level :: state.Levels) >> updateCurrentLevel (state.NewLevel))
    return ()
}

// インデントが浅くなっていたら成功するパーサ
let pCloseParen level = parse {
    let! state = getUserState
    do! (fun stream -> if state.NewLevel <= level then
                           Reply(ReplyStatus.Ok)
                       else
                           Reply(ReplyStatus.Error, messageError "close paren error"))
    |>> ignore
    do! updateUserState (updateLevels (state.Levels.Tail) >> updateCurrentLevel (state.Levels.Head))
    return ()
}

let pRosMessage, pRosMessageRef = createParserForwardedToRef()

// 子要素のパーサ
let pChildren = parse {
    let! state = getUserState
    let currentLevel = state.CurrentLevel
    do! pOpenParen currentLevel
    let! lines = many pRosMessage
    do! pCloseParen currentLevel
    return lines
}

//*****************************************************************
// メッセージのパーサ
do pRosMessageRef := parse {
    let! state = getUserState
    do! pSameLevel state
    let! t, f = pMember
    do! pEndOfLine
    let! children = opt pChildren
    return match children with
           | Some c -> Node(t, f, c)
           | None -> Leef(t, f)
}

// サービスのパーサ(リクエストとレスポンスが---で区切られている)
let pRosService = parse {
    let! req = many pRosMessage
    let! _ = pstring "---"
    do! pEndOfLine
    let! res = many pRosMessage
    return req, res
}
