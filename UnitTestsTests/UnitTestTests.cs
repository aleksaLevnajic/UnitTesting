using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Assert = NUnit.Framework.Assert;
using System.Windows.Input;
using System.Security.Cryptography.X509Certificates;

namespace UnitTests.Tests
{
    [TestClass()]
    public class UnitTestTests
    {
        ICalculator calculator = Substitute.For<ICalculator>();
        SomeClass someClass = Substitute.For<SomeClass>(5, "hello world"); // substituing for class(it is very risky!)

        [TestMethod()]
        public void TestingMultipleInterfaces()
        {
            var substitute = Substitute.For(
            new[] { typeof(ICommand), typeof(ICalculator), typeof(SomeClass) },
            new object[] { 5, "hello world" }
            );

            /*Assert.IsInstanceOf<ICommand>(substitute); // no def. of IsInstanceOf
            Assert.IsInstanceOf<ICalculator>(substitute);
            Assert.IsInstanceOf<SomeClass>(substitute);*/

        }


        [TestMethod()]
        public void TestingTest()
        {
            calculator.Add(1, 2).Returns(3);
            Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
        }

        [TestMethod()]
        public void TestingDidNotRecive()
        {
            calculator.Add(1, 2);
            calculator.Received().Add(1, 2);
            calculator.DidNotReceive().Add(5, 7);
        }

        [TestMethod()]
        public void TestingCalculatorMode()
        {
            calculator.Mode.Returns("DEC");
            Assert.That(calculator.Mode, Is.EqualTo("DEC"));

            calculator.Mode = "HEX";
            Assert.That(calculator.Mode, Is.EqualTo("HEX"));

            calculator.Mode.Returns("HEX", "DEC", "BIN");
            Assert.That(calculator.Mode, Is.EqualTo("HEX"));
            Assert.That(calculator.Mode, Is.EqualTo("DEC"));
            Assert.That(calculator.Mode, Is.EqualTo("BIN"));
        }

        [TestMethod()]
        public void TestingSpecificArgs()
        {
            //Assert.AreEqual&AreNotEqual dosen't exsit as a method?
            calculator.Add(Arg.Any<int>(), 5).Returns(10);
            Assert.That(10, Is.EqualTo(calculator.Add(1, 5)));
            Assert.That(10, Is.EqualTo(calculator.Add(5, 5)));
            Assert.That(10, Is.EqualTo(calculator.Add(-5, 5)));
            Assert.That(10, Is.EqualTo(10));
            Assert.That(10, Is.Not.EqualTo(calculator.Add(-9, -9)));

            calculator.Add(1, 2).ReturnsForAnyArgs(100);
            Assert.That(100, Is.EqualTo(calculator.Add(1, 2)));
            Assert.That(100, Is.EqualTo(calculator.Add(-7, 15)));
            Assert.That(100, Is.EqualTo(calculator.Add(50, 50)));


            calculator
            .Add(Arg.Any<int>(), Arg.Any<int>())
            .Returns(x => (int)x[0] + (int)x[1]);

            Assert.That(calculator.Add(1, 1), Is.EqualTo(2));
            Assert.That(calculator.Add(20, 30), Is.EqualTo(50));
            Assert.That(calculator.Add(-73, 9348), Is.EqualTo(9275));

        }

        [TestMethod()]

        public void TesitngCallbacks()
        {
            var counter = 0;
            calculator.Add(default, default).ReturnsForAnyArgs(x => { counter++; return 0; });

            calculator.Add(7, 3);
            calculator.Add(2, 2);
            calculator.Add(11, -3);

            Assert.That(counter, Is.EqualTo(3));

            var counter2 = 0;
            calculator
                .Add(default, default)
                .ReturnsForAnyArgs(x => 0)
                .AndDoes(x => counter2++);

            calculator.Add(7, 3);
            calculator.Add(2, 2);
            Assert.That(counter2, Is.EqualTo(2));
            //Assert.That(calculator.Add(2, 2), Is.EqualTo(0));

            /*calculator.Mode.Returns(x => "DEC", x => "HEX", x => { throw new Exception(); });

            Assert.That("DEC", Is.EqualTo(calculator.Mode));
            Assert.That("HEX", Is.EqualTo(calculator.Mode));
            Assert.Throws<Exception>(() => { var result = calculator.Mode; });*/

            calculator.Mode.Returns("HEX");
            calculator.Mode.Returns("BIN");
            //Assert.That(calculator.Mode, Is.EqualTo("HEX")); - false
            Assert.That(calculator.Mode, Is.EqualTo("BIN"));
        }

        public interface ICommand
        {
            void Execute();
            //void Execute2();
            event EventHandler Executed;
        }
        public class SomethingThatNeedsACommand
        {
            ICommand command;
            public SomethingThatNeedsACommand(ICommand command)
            {
                this.command = command;
            }
            public void DoSomething() { command.Execute(); }
            public void DontDoAnything() { }
        }
        [TestMethod()]
        public void Should_execute_command()
        {
            //Arrange
            var command = Substitute.For<ICommand>();
            var something = new SomethingThatNeedsACommand(command);
            //Act
            something.DoSomething();
            //Assert
            command.Received().Execute();
        }


        public class OnceOffCommandRunner
        {
            ICommand command;
            public OnceOffCommandRunner(ICommand command)
            {
                this.command = command;
            }
            public void Run()
            {
                if (command == null) return;
                command.Execute();
                command = null;
            }
        }

        [TestMethod()]
        public void TestingClearingRecivedCalls()
        {
            var command = Substitute.For<ICommand>();
            var runner = new OnceOffCommandRunner(command);

            //First run
            runner.Run();
            command.Received().Execute();

            //Forget previous calls to command
            command.ClearReceivedCalls();

            //Second run
            runner.Run();
            command.DidNotReceive().Execute();
        }

        [TestMethod()]
        public void TestingSubstituteChains()
        {
            var context = Substitute.For<IContext>();
            context.CurrentRequest.Identity.Name.Returns("My pet fish Eric");
            Assert.That("My pet fish Eric", 
                Is.EqualTo(context.CurrentRequest.Identity.Name));

        }

        public interface IContext
        {
            IRequest CurrentRequest { get; }
        }
        public interface IRequest
        {
            IIdentity Identity { get; }
            IIdentity NewIdentity(string name);
        }
        public interface IIdentity
        {
            string Name { get; }
            string[] Roles();
        }
    }
}