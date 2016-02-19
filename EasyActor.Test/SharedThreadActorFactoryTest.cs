﻿using System.Threading.Tasks;
using FluentAssertions;
using System.Threading;
using NUnit.Framework;
using EasyActor.Test.TestInfra.DummyClass;

namespace EasyActor.Test
{
    [TestFixture]
    public class SharedThreadActorFactoryTest
    {
        private SharedThreadActorFactory _Factory;
        [SetUp]
        public void setUp()
        {
            _Factory = new SharedThreadActorFactory();
        }

        [Test]
        public void Type_Should_Be_Shared()
        {
            _Factory.Type.Should().Be(ActorFactorType.Shared);
        }

        [Test]
        public async Task All_Actors_Should_Run_On_Same_Thread()
        {
            //arrange
            var target1 = new DummyClass();
            var target2 = new DummyClass();
            var intface1 = _Factory.Build<IDummyInterface2>(target1);
            var intface2 = _Factory.Build<IDummyInterface2>(target2);

            //act
            await intface1.DoAsync();
            await intface2.DoAsync();

            //assert
            target1.CallingThread.Should().Be(target2.CallingThread);
        }

        [Test]
        public async Task BuildAsync_Should_Call_Constructor_On_Actor_Thread()
        {
            var current = Thread.CurrentThread;
            DummyClass target = null;
            IDummyInterface2 intface = await _Factory.BuildAsync<IDummyInterface2>(() => { target = new DummyClass(); return target; });
            await intface.DoAsync();

            target.Done.Should().BeTrue();
            target.CallingConstructorThread.Should().NotBe(current);
            target.CallingConstructorThread.Should().Be(target.CallingThread);
        }

        [Test]
        public async Task Stop_Should_Kill_Thread()
        {
            //arrange
            var target1 = new DummyClass();
            var intface1 = _Factory.Build<IDummyInterface2>(target1);
          

            //act
            await intface1.DoAsync();

            //assert
            await _Factory.Stop();
            target1.CallingThread.IsAlive.Should().BeFalse();
        }

        [Test]
        public async Task Abort_Should_Kill_Thread()
        {
            //arrange
            var target1 = new DummyClass();
            var intface1 = _Factory.Build<IDummyInterface2>(target1);

            //act
            await intface1.DoAsync();

            //assert
            await _Factory.Abort();
            target1.CallingThread.IsAlive.Should().BeFalse();
        }

        [Test]
        public async Task Stop_Should_Call_Actor_IAsyncDisposable_DisposeAsync_Thread()
        {
            //arrange
            var target1 = new DisposableClass();
            var intface1 = _Factory.Build<IDummyInterface1>(target1);

            //act
            await intface1.DoAsync();

            //assert
            await _Factory.Stop();
            target1.IsDisposed.Should().BeTrue();
        }

        [Test]
        public async Task Abort_Should_Call_Actor_IAsyncDisposable_DisposeAsync_Thread()
        {
            //arrange
            var target1 = new DisposableClass();
            var intface1 = _Factory.Build<IDummyInterface1>(target1);


            //act
            await intface1.DoAsync();

            //assert
            await _Factory.Abort();
            target1.IsDisposed.Should().BeTrue();
        }
    }
}
