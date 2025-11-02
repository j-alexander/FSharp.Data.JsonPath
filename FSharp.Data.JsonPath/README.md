# FSharp.Data.JsonPath
JsonPath queries for FSharp.Data’s JsonValue. Find the parts of a JSON document you care about—without converting to JObject or changing your data model.
- Targets: developers using FSharp.Data JsonValue
- Use cases: robust querying, filtering, and slicing of JSON in functional pipelines
- Zero conversion: operate directly on JsonValue
- Focus: correctness, performance, and real-world practicality

## Install

[![NuGet](https://img.shields.io/nuget/v/FSharp.Data.JsonPath.svg?style=flat)](https://www.nuget.org/packages/FSharp.Data.JsonPath/)

- Using the Package Manager Console:
```
  PM> Install-Package FSharp.Data.JsonPath
```

- Using [paket](https://fsprojects.github.io/Paket/):
```
  > dotnet paket add FSharp.Data.JsonPath --project MyProject
```

## Quick start

``` fsharp
// F#
open FSharp.Data
open FSharp.Data.JsonPath

let json =
    JsonValue.Parse """
    {
      "store": {
        "book": [
          { "category": "ref", "author": "Nigel", "price": 8.95 },
          { "category": "fic", "author": "Evelyn", "price": 12.99, "isbn": "X" }
        ]
      }
    }"""

// All authors
let authors = json |> JsonPath.findList "$.store.book[*].author"

// First match (throws if none)
let firstCheap = json |> JsonPath.find "$..book[?(@.price<10)]"

// Safe try
let maybeIsbn = json |> JsonPath.tryFind "$..book[?(@.isbn)].isbn"
```

## API
- `JsonPath.find query json -> JsonValue`
  - Returns the first match. Throws if no match.
- `JsonPath.findList query json -> JsonValue list`
  - Returns all matches in document order.
- `JsonPath.findSeq query json -> seq<JsonValue>`
  - Lazy sequence of matches.
- `JsonPath.tryFind query json -> JsonValue option`
  - First match, or None.

Queries are strings in standard JsonPath syntax, beginning with `$`.

### Supported JsonPath features
- Root: `$`
- Exact child: `$.a.b`
- Recursive descent: `$..b` (any depth)
- Wildcards:
  - Properties: `$.*`
  - Arrays: `$.[*]`, `$.a[*][*]`, `$..*`
- Array selectors:
  - Literal indices: `$.[0,2,-1]` (supports negative indices)
  - Slices: `$.[start:finish:step]` with negatives and omissions, e.g. `$.[1:]`, `$.[-2:]`, `$.[::2]`, `$.[0:-1]`
  - Wildcard: `[*]`
- Filters and expressions:
  - Presence: `$..book[?(@.isbn)]`
  - Numeric comparison: `$..book[?(@.price<10)]`
  - Index expression passthrough: `$..book[(@.length-1)]`

Notes:
- Recursive descent works for both properties and arrays.
- Wildcards match any property name or any array index.
- Slices follow Python-like semantics; finish is exclusive, negative indices count from the end.

### Practical recipes
Get a value by path:
``` fsharp
  // F#
  json |> JsonPath.find "$.store.book[0].category"
```

Gather all nested occurrences of a key:
``` fsharp
  // F#
  json |> JsonPath.findList "$..price"
```

Select all items from any array named "items":
``` fsharp
  // F#
  json |> JsonPath.findList "$..items[*]"
```

Safely try for an optional field:
``` fsharp
  // F#
  json |> JsonPath.tryFind "$.profile.address.postcode"
```

Last element of any "book" array:
``` fsharp
  // F#
  json |> JsonPath.findList "$..book[-1:]"
```

Top sellers under $10 across a catalog:
``` fsharp
  // F#
  json
  |> JsonPath.findList "$..book[?(@.price<10)]"
  |> List.map (fun b -> b)
```

Extract all leaf values:
``` fsharp
  // F#
  json |> JsonPath.findList "$..*"
```

Work with root arrays:
``` fsharp
  // F#
  let arr = JsonValue.Parse """[ { "a": 1 }, { "a": 2 } ]"""
  let values = arr |> JsonPath.findList "$.[*].a"
```

### Real-world scenarios
- Observability logs: `$..attributes.userId`
- E-commerce catalog: `$..products[*].price`
- Feature flags: `$..flags[?(@.enabled=true)].name`
- ETL slice checks: `$.records[0:100]`
- Event stream payloads with variable nesting: `$..payload.*.id` or `$..id`
- Document migrations: find missing fields `$..users[*][?(!@.email)]` via presence check patterns

### Behavior and edge cases
- Identity: `$.` returns the current JsonValue (root identity).
- Missing paths: find throws; prefer tryFind for optional data.
- Out-of-range literals: ignored (non-matching indices are skipped).
- Negative indices: count from end; `-1` is last.
- Slices:
  - finish is exclusive
  - step must be positive; step 0 yields no matches
- Ordering: matches are returned in document traversal order (preorder).
- Performance: traversal is streaming-friendly; prefer findSeq for large documents. 

### Testing-style examples
- Recursive property:
  - `$..a.b` finds any `b` nested beneath any `a`, at any depth.
- Wildcard property at root:
  - `$.*` returns all top-level property values.
- Any + wildcard combination:
  - `$..c.*.b` finds `b` under any child of any `c`.

### Error handling
- find: throws if no match
- tryFind: returns None
- findList/findSeq: return empty if no matches

Choose tryFind or findList for resilient pipelines.

### Interop tips

- Keep your data in JsonValue; avoid converting to JObject. 

Compose with F# pipelines:
``` fsharp
  // F#
  let cheapAuthors =
      json
      |> JsonPath.findList "$..book[?(@.price<10)].author"
```

### Compatibility
- Designed for FSharp.Data’s JsonValue
- JsonPath syntax aligned with broadly used conventions: recursive descent, wildcards, filters, array literals and slices

## Contributing
- Issues and PRs welcome for additional operators, performance improvements, and docs/examples.
- Add unit tests mirroring expected JsonPath semantics for new behaviors.

## License
- Open source. See package/license details on NuGet.