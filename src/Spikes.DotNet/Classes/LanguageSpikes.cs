using System;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace Eca.AnnualDinner.UnitTests.Presentation
{
    internal class Person : IEquatable<Person>
    {
        #region Constructors

        public Person(string _name, string _address, string _phone, int _age)
        {
            Name = _name;
            Address = _address;
            Phone = _phone;
            Age = _age;
        }


        public Person() {}

        #endregion


        #region Properties

        public string Address { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        #endregion


        #region IEquatable<Person> Members

        public bool Equals(Person obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Address, Address) && obj.Age == Age && Equals(obj.Name, Name) && Equals(obj.Phone, Phone);
        }

        #endregion


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Person)) return false;
            return Equals((Person) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Address != null ? Address.GetHashCode() : 0);
                result = (result*397) ^ Age;
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                return result;
            }
        }

        #endregion
    }



    internal class BuilderWithProperties
    {
        #region Member Variables

        private string _address;
        private int _age;
        private string _name;
        private string _phone;

        #endregion


        #region Properties

        public string Address
        {
            set { _address = value; }
        }

        public int Age
        {
            set { _age = value; }
        }

        public string Name
        {
            set { _name = value; }
        }

        public string Phone
        {
            set { _phone = value; }
        }

        #endregion


        #region Methods: Private

        private Person Build()
        {
            return new Person(_name, _address, _phone, _age);
        }

        #endregion


        #region Static Operators

        public static implicit operator Person(BuilderWithProperties from)
        {
            return from.Build();
        }

        #endregion
    }



    [TestFixture]
    public class LanguageSpikes
    {
        [Test]
        public void BuilderPatternUsingObjectInitializers()
        {
            Person person = new BuilderWithProperties
                              {
                                  Age = 20,
                                  Phone = "01322",
                                  Address = "Chatham",
                                  Name = "Christian"
                              };
            var expected = new Person
                               {
                                   Name = "Christian",
                                   Address = "Chatham",
                                   Age = 20,
                                   Phone = "01322"
                               };

            Assert.That(person, Is.EqualTo(expected));
        }


        [Test]
        public void ComparingAnonymousTypes()
        {
            var firtPerson = new {Name = "Christian", Age = 34, EyeColour = Color.Red};
            var secondPerson = new {Name = "Christian", Age = 34, EyeColour = Color.Red};

            Assert.That(firtPerson, Is.EqualTo(secondPerson));
        }


        [Test]
        public void FilteringAndOrderingAnonymousMethods()
        {
            var films = new[]
                            {
                                new {Name = "Jaws", Year = 1972},
                                new {Name = "American Beauty", Year = 1974},
                                new {Name = "Matrix", Year = 1975}
                            };

            var filteredFilms = films
                .Where(f => f.Year < 1975)
                .OrderBy(f => f.Name).ToList();

            filteredFilms.ForEach(Console.WriteLine);
        }


        [Test]
        public void UsingLambdaExpressionsToRegisterEventHandlers()
        {
            var classWithEvents = new ClassWithEvents();
            EventHandler<EventArgs> someEvent = ((sender, e) => Log("SomeEvent", sender, e));
            classWithEvents.SomeEvent += someEvent;

            classWithEvents.OnSomeEvent(EventArgs.Empty);
        }


        [Test]
        public void UsingLambdaExpressionsToRegisterEventHandlers_LongHand()
        {
            var classWithEvents = new ClassWithEvents();
            Expression<EventHandler<EventArgs>> expression = (sender, e) => Log("SomeEvent", sender, e);
            EventHandler<EventArgs> someEvent = expression.Compile();
            classWithEvents.SomeEvent += someEvent;

            classWithEvents.OnSomeEvent(EventArgs.Empty);
        }


        [Test]
        public void UsingLambdaExpressionsToDeriveNameOfProperty()
        {
            Expression<Func<Person, object>> ageProperty = (x => x.Age);

            var expressionBody = (UnaryExpression) ageProperty.Body;
            MemberInfo member = ((MemberExpression) expressionBody.Operand).Member;

            Assert.That(member.Name, Is.EqualTo("Age"));
        }


        private void Log(string title, object sender, EventArgs e)
        {
            Console.Out.WriteLine("title = {0}", title);
            Console.Out.WriteLine("sender = {0}", sender);
            Console.Out.WriteLine("e = {0}", e);
        }



        private class ClassWithEvents
        {
            public event EventHandler<EventArgs> SomeEvent;


            public void OnSomeEvent(EventArgs e)
            {
                EventHandler<EventArgs> Handler = SomeEvent;
                if (Handler != null) Handler(this, e);
            }
        }
    }
}