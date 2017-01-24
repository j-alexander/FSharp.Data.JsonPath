namespace FSharp.Data.JsonPath.Tests

open System
open NUnit.Framework
open FSharp.Data

open FSharp.Data.JsonPath.Query

// Examples sourced from:
//
// http://goessner.net/articles/JsonPath/
// https://jsonpath.curiousconcept.com/
// http://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm
//
[<TestFixture>]
type QueryParser() =

    [<Test>]
    member x.PathExamples() =
        let examples =
            [ "$.store.book[*].author",
              [Exact,Property("store");Exact,Property("book");Exact,Array(Index.Wildcard);Exact,Property("author")]

              "$..author",
              [Any,Property("author")]

              "$.store.*",
              [Exact,Property("store");Exact,Property("*")]

              "$.store..price",
              [Exact,Property("store");Any,Property("price")]

              "$..book[2]",
              [Any,Property("book");Exact,Array(Index.Literal[2])]

              "$..book[(@.length-1)]",
              [Any,Property("book");Exact,Array(Index.Expression "(@.length-1)")]

              "$..book[-1:]",
              [Any,Property("book");Exact,Array(Index.Slice(Some -1,None,1))]

              "$..book[:2]",
              [Any,Property("book");Exact,Array(Index.Slice(None,Some 2,1))]

              "$..book[1:2]",
              [Any,Property("book");Exact,Array(Index.Slice(Some 1,Some 2,1))]

              "$..book[::1]",
              [Any,Property("book");Exact,Array(Index.Slice(None,None,1))]

              "$..book[1:2:3]",
              [Any,Property("book");Exact,Array(Index.Slice(Some 1,Some 2,3))]

              "$..book[0,1]",
              [Any,Property("book");Exact,Array(Index.Literal [0;1])]

              "$..*",
              [Any,Property("*")]

              "$.store.book[*]",
              [Exact,Property("store");Exact,Property("book");Exact,Array(Index.Wildcard)]

              "$.store.book[*][*]",
              [Exact,Property("store");Exact,Property("book");Exact,Array(Index.Wildcard);Exact,Array(Index.Wildcard)]
//
//              "$.[first name]",
//              [Exact,Property("first name")]
//
//              "$.[book].[publication date]",
//              [Exact,Property("book");Exact,Property("publication date")]
//
//              "$.[book store][3].[publication date]",
//              [Exact,Property("book store");Exact,Array(Index.Literal[3]);Exact,Property("publication date")]
//
//              "$.[0]",
//              [Exact,Array(Index.Literal[0])]
            ]

        for i, example, expected in examples |> Seq.mapi (fun i (e,x) -> i,e,x) do
            let levels = levelsFor example
            Assert.AreEqual(levels, expected, sprintf "Example #%d: %s" i example)

    member x.ExpressionExamples() =
        let examples =
            [ "$..book[?(@.isbn)]",
              [Any,Property("book");Exact,Array(Index.Expression "?(@.isbn)")]

              "$..book[?(@.price<10)]",
              [Any,Property("book");Exact,Array(Index.Expression "?(@.price<10)")]

              "$.manufacturers[?(@.name == 'Acme Co')]",
              []

              "$..products[?(@.price >= 50)].name",
              []

              "$[?('Eva Green' in @.starring)]",  //where Eva Green is included in the starring array.
              []

              "$[?(@[director] == 'Sam Mendes' && @[release year] == '1995')]",
              []
            ]
        ()