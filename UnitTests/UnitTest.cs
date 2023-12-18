using NSubstitute;
using NUnit.Framework;

namespace UnitTests
{
    public class UnitTest
    {
        ICalculator calculator = Substitute.For<ICalculator>();



        public UnitTest() { }

        [Test]
        public void Testing()
        {
            calculator.Add(1, 2).Returns(3);
            Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
        }

        [Test]
        public void TestingDidNotRecive()
        {
            calculator.Add(1, 2);
            calculator.Received().Add(1, 2);
            calculator.DidNotReceive().Add(5, 7);
        }


    }

    public interface ICalculator
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event EventHandler PoweringUp;

    }

    public class SomeClass
    {
        private readonly int valInt;
        private readonly string valString;

        public SomeClass(int valInt, string valString)
        {
            this.valInt = valInt;
            this.valString = valString;
        }
    }


}
