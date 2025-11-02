F# has been characterized as a modern programming language well-suited to data-rich analytical problems. In this language, interacting with diverse datasets from event streams, web services, and databases is further simplified with the open-source FSharp.Data library.

Many of these real-world documents are hierarchically structured in Json and Xml. When designing reusable applications and algorithms in this space, it becomes very important to be able to extract subsets of data using custom queries.

For Xml, XPATH is a well established tool for describing specific elements within a document.

A similar mechanism exists for Json, called JsonPath.

Using JsonPath with F# typically involves converting data to Newtonsoft.Json's JObject type and invoking SelectToken. However, FSharp.Data has an elegant and functional representation of JsonValues that is very widely used.

FSharp.Data.JsonPath can directly query subsets of a JsonValue document without converting your entire dataset to perform some extraction.