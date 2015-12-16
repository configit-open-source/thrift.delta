using System;
using Configit.Thrift.Delta.Messages;
using Thrift.Protocol;

namespace Thrift.Delta
{
    public class DeltaGenerator<T> where T :TBase
    {
        public ChangeSet Create( TBase baseObj, TBase newObj )
        {
           throw new NotImplementedException();
        }
    }
}