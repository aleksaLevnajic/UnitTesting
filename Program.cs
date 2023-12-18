using NSubstitute;
using NUnit.Framework;
using System.Windows.Input;
using static UnitTesting.Program;

namespace UnitTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //trying examples from NSubtitute

            var calculator = Substitute.For<ICalculator>();            


            calculator.Add(1, 2).Returns(3);
            Assert.That(calculator.Add(1, 2), Is.EqualTo(3));

            calculator.Add(1, 2);
            calculator.Received().Add(1, 2);
            calculator.DidNotReceive().Add(5, 7);

            calculator.Mode.Returns("DEC");
            Assert.That(calculator.Mode, Is.EqualTo("DEC"));
            calculator.Mode = "HEX";
            Assert.That(calculator.Mode, Is.EqualTo("HEX"));

            bool eventWasRaised = false;
            calculator.PoweringUp += (sender, args) => eventWasRaised = true;
            calculator.PoweringUp += Raise.Event();
            Assert.That(eventWasRaised);


            Console.ReadKey();
        }
    }

    public interface ICalculator
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event EventHandler PoweringUp;

    }


}

        

