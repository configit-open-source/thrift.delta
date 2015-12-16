using System.Collections.Generic;
using System.Linq;

using Configit.Thrift.Delta.Messages;

using NUnit.Framework;

namespace Thrift.Delta.UnitTest {

  [TestFixture]
  public class DeltaApplierTest {

    public class BankAccountEntry {
      public string Message { get; set; }
      public double Balance { get; set; } 
    }

    public class Person {
      
      public string FirstName { get; set; }
      public string LastName { get; set; }

      public List<Person> Friends { get; set; }

      public Person Spouse { get; set; }

      public List<BankAccountEntry> BankAccount { get; set; }
    }

    [Test]
    public void SetStringOnRoot() {
      DeltaApplier applier = new DeltaApplier();

      Person p = new Person();

      PathUpdate update = new PathUpdate {
        Path = new List<short>( new[] { (short)1 } ),
        Content = new Content { String = "Tiedemann" }
      };

      applier.Apply( p, new ChangeSet( new[] { update }.ToList() ) );

      Assert.That( p.LastName, Is.EqualTo( "Tiedemann" ) );
      Assert.That( p.FirstName, Is.Null );
    }

    //[Test]
    //public void SetStringOnPersonFriend() {
    //  DeltaApplier applier = new DeltaApplier();

    //  Person p = new Person {
    //    Friends = new List<Person>()
    //  };

    //  p.Friends.Add( new Person() );
    //  p.Friends.Add( new Person() );

    //  var personUpdate = new PathUpdate(
    //    new short[] { 0 }.ToList(),
    //    new Content { String = "Batman" } );

    //  var arrayIndex = new PathUpdate(
    //    new short[0].ToList(),
    //    new Content {
    //      KeyValue =
    //    new KeyValue {
    //      Key = new Content { Int32 = 1 }, // Array index
    //      Value = new Content { Branches = new[] { personUpdate }.ToList() } // Update applies to property on indexed object
    //    }
    //    } );

    //  PathUpdate update = new PathUpdate {
    //    Path = new List<short>( new short[] { 2 } ),
    //    Content = new Content {
    //      // Single branch through array
    //      // Slightly convoluted way of accessing array?
    //      Branches = new[] { arrayIndex }.ToList()
    //    }
    //  };

    //  applier.Apply( p, new ChangeSet( new[] { update }.ToList() ) );

    //  Assert.That( p.Friends[1].FirstName, Is.EqualTo( "Batman" ) );
    //}

    [Test]
    public void SetStringOnSpouse() {
      Person p = new Person { Spouse = new Person() };
      DeltaApplier applier = new DeltaApplier();

      var update = new PathUpdate(
        new short[] { 3, 0 }.ToList(),
        new Content { String = "Camilla" } );

      applier.Apply( p, new ChangeSet( new[] { update }.ToList() ) );

      Assert.That( p.Spouse.FirstName, Is.EqualTo( "Camilla" ) );
    }
  }
}
