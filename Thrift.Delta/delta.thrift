namespace csharp Configit.Thrift.Delta.Messages

// Filling in a new array has to be done per element

union Content {
  1: Special Special
  2: bool Bool,
  3: byte Byte,
  4: i16 Int16,
  5: i32 Int32,
  6: i64 Int64
  7: double Double,
  8: binary Binary,
  9: string String,
  10: binary Object, // Serialized struct/union object
  11: KeyValue KeyValue, // index into array or map
  12: list<PathUpdate> Branches, // branches out from current path
}

enum Special{
  Undefined = 0, // clear value
  Empty = 1 // Allows emptying a collection
}

struct PathUpdate {
  1: required list<i16> Path, // Index of field in message ( zero based )

   // What to do at the path endpoint
  2: required Content Content
}

struct KeyValue{
  // Provides a key for collections (i32 for list, key-type for map)
  1: optional Content Key,

  // Provides the value to set.
  // If not supplied, the value will set
  // set to null or equivalent.
  // To delete a collection element,
  // simply do not specify a value for the Node.
  // in the NodeChanges object
  2: optional Content Value
}

struct ChangeSet {
  // If the delta applies against a cached
  // object, this is used to identify that object
  1: optional binary BaseIdentifier,

  2: required list<PathUpdate> Updates
}
