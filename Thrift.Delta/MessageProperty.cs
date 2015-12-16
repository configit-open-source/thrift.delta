using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Thrift.Delta {
  internal class MessageProperty {

    public MessageProperty( PropertyInfo property ) {
      Type = property.PropertyType;
      Set = CompileSet( property );
      Get = CompileGet( property );

    }
    private DeltaApplier.Get CompileGet( PropertyInfo property ) {
      var mi = property.GetMethod;
      
      var thisObj = Expression.Parameter( typeof( object ), "thisObj" );
      var thisObjConverted = Expression.Convert( thisObj, property.DeclaringType );
      
      var propertyExpr = Expression.Property( thisObjConverted, mi );

      var converted = Expression.Convert( propertyExpr, typeof( object ) );

      return Expression.Lambda<DeltaApplier.Get>( converted, "get"+property.Name, new[] { thisObj } ).Compile();
    }

    private DeltaApplier.Set CompileSet( PropertyInfo property ) {
      var mi = property.SetMethod;

      var thisObj = Expression.Parameter( typeof( object ), "thisObj" );
      var valObj = Expression.Parameter( typeof( object ), "valObj" );
      var convertedObj = Expression.Convert( valObj, property.PropertyType );
     
      var thisObjConverted = Expression.Convert( thisObj, property.DeclaringType );

      var setCall = Expression.Call( thisObjConverted, mi, convertedObj );

      return Expression.Lambda<DeltaApplier.Set>( setCall, "set" + property.Name, new[] { thisObj, valObj } ).Compile();
    }

    public DeltaApplier.Get Get { get; private set; }
    public DeltaApplier.Set Set { get; private set; }

    public Type Type { get; set; }
  } 
}