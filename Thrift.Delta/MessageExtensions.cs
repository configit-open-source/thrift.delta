using System;

using Configit.Thrift.Delta.Messages;

namespace Thrift.Delta {
  public static class MessageExtensions {
    public static object GetSimpleTypeValue( this Content content ) {
      if ( content.Bool.HasValue ) {
        return content.Bool;
      }
      if ( content.Byte.HasValue ) {
        return content.Byte;
      }
      if ( content.Int16.HasValue ) {
        return content.Int16;
      }
      if ( content.Int32.HasValue ) {
        return content.Int32;
      }
      if ( content.Int64.HasValue ) {
        return content.Int64;
      }
      if ( content.Double.HasValue ) {
        return content.Double;
      }
      if ( content.String != null ) {
        return content.String;
      }
      throw new ArgumentException( nameof( content ) + " must have a simple type" );
    }
  }
}