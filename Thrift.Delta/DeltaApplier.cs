using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using Configit.Thrift.Delta.Messages;
using Thrift.Protocol;

namespace Thrift.Delta {
  public class DeltaApplier {
    public delegate void Set( object parent, object value );

    public delegate object Get( object parent );

    // Compiled actions by message type
    // Maps to table of setters for each corresponding
    // field.
    private readonly Dictionary<Type, MessageProperty[]> _properties = new Dictionary<Type, MessageProperty[]>();

    private void EnsureType( Type type ) {
      // TODO: Its a bit hacky and dangerous to rely directly on the generated class
      // better to read the .thrift file itself or add field number as attribute.
      var props =
        type.GetProperties(
          BindingFlags.Instance |
          BindingFlags.Public |
          BindingFlags.SetProperty |
          BindingFlags.GetProperty );

      int index = 0;
      MessageProperty[] compiledProps = new MessageProperty[props.Length];

      foreach ( var property in props ) {
        compiledProps[index] = new MessageProperty( property );
        index++;
      }

      _properties[type] = compiledProps;
    }

    private void Apply( object baseObj, PathUpdate update ) {
      var type = baseObj.GetType();

      EnsureType( type );

      var parent = LocateParent( type, baseObj, update.Path );

      var content = update.Content;

      if ( content.Branches != null ) {
        foreach ( var supUpdate in content.Branches ) {
          Apply( parent, supUpdate );
        }
        return;
      }

      var target = _properties[type][update.Path[update.Path.Count - 1]];
      
      if ( content.Binary != null ) {
        // TODO: Determine type, locate generic Read method and pre-compile deserialize
        throw new NotImplementedException();
      }
      if ( content.Special.HasValue ) {
        switch ( content.Special.Value ) {
          case Special.Undefined:
            target.Set( parent, null );
            break;
          case Special.Empty:
            // TODO: Must pre-compile clear method
            throw new NotImplementedException();
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        return;
      }

      // If none of the special cases apply, we can retrieve the simple
      // type (such as string, int32 etc) and pass that to the setter.
      target.Set( parent, content.GetSimpleTypeValue() );
    }

    public void Apply( object baseObj, ChangeSet changesSet ) {
      foreach ( var change in changesSet.Updates ) {
        Apply( baseObj, change );
      }
    }

    private object LocateParent( Type type, object src, IReadOnlyList<short> path ) {
      var dest = src;
      for ( int i = 0; i < path.Count - 1; i++ ) {
        dest = _properties[type][path[i]].Get( src );
        if( dest == null ) {
          throw new ArgumentOutOfRangeException();
        }
      }
      return dest;
    }
  }
}