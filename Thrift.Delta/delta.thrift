
union Value {
  // We could just put in a binary blob
  // since we always knows what the type is.
  // but does thrift allow serializing
  // atomic type alone?
  1: bool Bool,
  2: byte Byte,
  3: i16 Int16,
  4: i32 Int32,
  5: i64 Int64
  6: double Double,
  7: binary Binary,
  8: string String
  9: binary Object // Thrift struct/union
}

// TODO: Distinguish between empty collection and null?

struct Change{
  // Used for setting a property/indexing into a DFS node
  1: optional Value Key,

  // Provides the value to set.
  // If not supplied, the value will set
  // set to null or equivalent.
  // To delete a collection element,
  // simply do not specify a value for the Node.
  // in the NodeChanges object
  2: optional Value Value
}

struct NodeChanges{
  // DFS id of base structure
  1: i32 Node,

  // If not present indicates a deletion of the node
  // If the node is a collection element, the elemnt
  // is removed from the collection.
  2: optional array<Change> Changes
}

struct ChangeSet {
  // If the delta applies against a cached
  // object, this is used to identify that object
  1: optional binary BaseIdentifier,

  2: array<NodeChanges> Changes
}

// service ExampleService{
//   Response Process (1: Request request, 2: ChangeSet changes )
// }
//
// struct Example{
//   1: string Id,
//   2: optional Example Child,
// }
//
// { // DFS 0
//   Id : "Me", // DFS 1
//   Child : { // DFS 2
//     Id: "Someone else" // DFS 3
//     // unspecified Child is DFS 4
//   }
// }
//
// // If we want to add a Child
// we do
// {
//   Node: 4,
//   New: 5 // some number > max DFS
// }
// {
//   Node: 6, // 5 + DFS inside 5 of Id
//   Content: { String: "Shrek "}
// }
