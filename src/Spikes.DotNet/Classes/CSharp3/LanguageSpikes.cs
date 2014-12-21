using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eca.Commons.Extensions;
using Eca.Commons.Testing;
using NUnit.Framework;
using Assert=NUnit.Framework.Assert;

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
        private int _int;

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
        public void FilteringAndOrderingAnonymousTypes()
        {
            var films = new[]
                            {
                                new {Name = "Jaws", Year = 1972},
                                new {Name = "American Beauty", Year = 1974},
                                new {Name = "Matrix", Year = 1975}
                            };


            var filteredFilms = from film in films 
                                where film.Year < 1975 
                                orderby film.Name 
                                select new {film.Name};
/*
            var filteredFilms = films
                .Where(film => film.Year < 1975)
                .OrderBy(f => f.Name)
                .Select(f => new {f.Name});
*/

            var expected = new[]
                               {
                                   new {Name = "American Beauty"}, 
                                   new {Name = "Jaws"}
                               };
            Assert.That(filteredFilms.ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void HowToDynamicallyBuildSortOrders()
        {
            //alternatively this routine could have been written as a named method
            //eg SortNames(IEnumerable<string>, Func<string, object>)
            Func<IEnumerable<string>, Func<string, object>, IEnumerable<string>> sortRoutine =
                (toSort, sortOrder) => toSort.OrderBy(sortOrder);


            string[] namesToSort = { "Katie", "Christian", "George", "Crowhurst" };

            //sort type would be supplied by the user - here we're assuming user selected to sort alphabetically
            int sortType = 0;

            if (sortType == 0)
            {
                var result = sortRoutine(namesToSort, name => name);
                Assert.That(result.ToList(), Is.EqualTo(new[] {"Christian", "Crowhurst", "George", "Katie"}),
                            "sorted alphebetically");
            }
            else
            {
                var result = sortRoutine(namesToSort, name => name.Length);
                Assert.That(result.ToList(), Is.EqualTo(new[] { "Katie", "George", "Christian", "Crowhurst" }),
                            "sorted by length");
            }
        }


        [Test]
        public void UsingLinqToParseTextFileIntoObjectGraph()
        {
            var lines = new[]
                            {
                                "#Titile,Price,Authors",
                                "c# 3 in depth,39.99,John Skeet;C Crowhurst",
                                "Linq in Action,20.00,John Smith;C Crowhurst"
                            };

            var query = from line in lines
                        where !line.StartsWith("#")
                        let lineParts = line.Split(',')
                        select new { Title = lineParts[0], Price = Decimal.Parse(lineParts[1]), Authors = lineParts[2] };


            var expected = new[]
                               {
                                   new {Title = "c# 3 in depth", Price = 39.99m, Authors = "John Skeet;C Crowhurst"},
                                   new {Title = "Linq in Action", Price = 20.00m, Authors = "John Smith;C Crowhurst"}
                               };

            Assert.That(query.ToList(), Is.EqualTo(expected));
        }


        [Test]
        public void UsingLinqToParseTextFileIntoNestedObjectGraph()
        {
            var lines = new[]
                            {
                                "#Titile,Price,Authors",
                                "c# 3 in depth,39.99,John Skeet;C Crowhurst",
                                "Linq in Action,20.00,John Smith;C Crowhurst"
                            };

            var query = from line in lines
                        where !line.StartsWith("#")
                        let lineParts = line.Split(',')
                        select new
                                   {
                                       Title = lineParts[0],
                                       Price = Decimal.Parse(lineParts[1]),
                                       Authors = (from authorFullName in lineParts[2].Split(';')
                                                  let authorNameParts = authorFullName.Split(' ')
                                                  select new {FirstName = authorNameParts[0], LastName = authorNameParts[1]}).ToArray()
                                   };


            var expected = new[]
                               {
                                   new
                                       {
                                           Title = "c# 3 in depth",
                                           Price = 39.99m,
                                           Authors = new[]
                                                         {
                                                             new {FirstName = "John", LastName = "Skeet"},
                                                             new {FirstName = "C", LastName = "Crowhurst"}
                                                         }
                                       },
                                   new
                                       {
                                           Title = "Linq in Action",
                                           Price = 20.00m,
                                           Authors = new[]
                                                         {
                                                             new {FirstName = "John", LastName = "Smith"},
                                                             new {FirstName = "C", LastName = "Crowhurst"}
                                                         }
                                       }
                               };

            var actual = query.ToList();
            EquivalenceComparer comparer = EquivalenceComparer.For<object>();
            Assert.That(comparer.PropertiesNotEqual(actual[0], expected[0]), Is.Empty);
            Assert.That(comparer.PropertiesNotEqual(actual[1], expected[1]), Is.Empty);
        }


        [Test]
        public void HowToElegantlyAndEfficientlySelectTheMaxElement()
        {
            var names = new[] {"Christian", "Katie"};

            string larget = names.MaxElement(name => name.Length);
            Assert.That(larget, Is.EqualTo("Christian"));
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

        [Test]
        public void ScopeOfCaptureVariablesExample()
        {
            string argumentToCapture = "Hello";
            Func<string> containsCapturedArgument = ReturnDelegateThatCapturesArgument(argumentToCapture);
            Assert.That(containsCapturedArgument(), Is.EqualTo(argumentToCapture), "captured argument value");

            argumentToCapture = "World";
            Assert.That(containsCapturedArgument(), Is.EqualTo("Hello"), "captured argument value has not changed");

            Func<int> containsCapturedLocalVariable = ReturnDelegateThatCapturesLocalVariable();
            Assert.That(containsCapturedLocalVariable(), Is.EqualTo(11), "captured local variable");

            _int = 10;
            Func<int> containsCapturedValueOfField = ReturnDelegateThatCapturesValueOfField();

            _int = 15;
            Assert.That(containsCapturedValueOfField(), Is.EqualTo(10 + 1), "captured field value");

            _int = 10;
            Func<int> containsCapturedField = ReturnDelegateThatCapturesField();

            _int = 15;
            Assert.That(containsCapturedField(), Is.EqualTo(15 + 1), "captured field");
        }

        [Test]
        public void DeferredExecutionAndScopeOfCapturedVariables()
        {
            int[] numbers = {5, 10, 15, 20};

            IEnumerable<int> filtered = ReturnQueryThatFiltersValuesUsingLocalVariable(numbers);

            Assert.That(filtered.Single(), Is.EqualTo(10));
        }

        private IEnumerable<int> ReturnQueryThatFiltersValuesUsingLocalVariable(IEnumerable<int> numbers)
        {
            int filterValue = 10;
            return numbers.Where(n => n == filterValue);
        }


        private Func<int> ReturnDelegateThatCapturesField()
        {
            Func<int> capturesField = () => 1 + _int;
            return capturesField;
        }

        private Func<int> ReturnDelegateThatCapturesValueOfField()
        {
            int result = 1 + _int;
            Func<int> capturesValueOfField = () => result;
            return capturesValueOfField;
        }

        private Func<int> ReturnDelegateThatCapturesLocalVariable()
        {
            int local = 1 + 4;
            Func<int> capturesLocalVariable = () => local;
            local = 5 + 6;
            return capturesLocalVariable;
        }

        private Func<string> ReturnDelegateThatCapturesArgument(string valueToCapture)
        {
            Func<string> capturesArgument = () => valueToCapture;
            return capturesArgument;
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