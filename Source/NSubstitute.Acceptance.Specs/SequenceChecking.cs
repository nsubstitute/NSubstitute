using System;
using NSubstitute.Exceptions;
using NSubstitute.Experimental;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class SequenceChecking
    {
        private IFoo _foo;
        private IBar _bar;

        [Test]
        public void Pass_when_checking_a_single_call_that_was_in_the_sequence()
        {
            _foo.Start();
            Received.InOrder(() => _foo.Start());
        }

        [Test]
        public void Fail_when_checking_a_single_call_that_was_not_in_the_sequence()
        {
            _foo.Start();
            Assert.Throws<CallSequenceNotFoundException>(() =>
                Received.InOrder(() => _foo.Finish())
                );
        }

        [Test]
        public void Pass_when_calls_match_exactly()
        {
            _foo.Start(2);
            _bar.Begin();
            _foo.Finish();
            _bar.End();

            Received.InOrder(() =>
            {
                _foo.Start(2);
                _bar.Begin();
                _foo.Finish();
                _bar.End();

            });
        }

        [Test]
        public void Fail_when_call_arg_does_not_match()
        {
            _foo.Start(1);
            _bar.Begin();
            _foo.Finish();
            _bar.End();

            Assert.Throws<CallSequenceNotFoundException>(() =>
                Received.InOrder(() =>
                                     {
                                         _foo.Start(2);
                                         _bar.Begin();
                                         _foo.Finish();
                                         _bar.End();
                                     })
                );
        }

        [Test]
        public void Use_arg_matcher()
        {
            _foo.Start(1);
            _bar.Begin();
            _foo.Finish();
            _bar.End();

            Received.InOrder(() =>
            {
                _foo.Start(Arg.Is<int>(x => x < 10));
                _bar.Begin();
                _foo.Finish();
                _bar.End();

            });
        }

        [Test]
        public void Fail_when_calls_made_in_different_order()
        {
            _foo.Finish();
            _foo.Start();

            Assert.Throws<CallSequenceNotFoundException>(() =>
                Received.InOrder(() =>
                                 {
                                     _foo.Start();
                                     _foo.Finish();
                                 })
            );
        }

        [Test]
        public void Fail_when_one_of_the_calls_in_the_sequence_was_not_received()
        {
            _foo.Start();
            _foo.Finish();

            Assert.Throws<CallSequenceNotFoundException>(() =>
                Received.InOrder(() =>
                {
                    _foo.Start();
                    _foo.FunkyStuff("hi");
                    _foo.Finish();
                })
            );
        }

        [Test]
        public void Fail_when_additional_related_calls_at_start()
        {
            _foo.Start();
            _foo.Start();
            _bar.Begin();
            _foo.Finish();
            _bar.End();

            Assert.Throws<CallSequenceNotFoundException>(() =>
                Received.InOrder(() =>
                {
                    _foo.Start();
                    _bar.Begin();
                    _foo.Finish();
                    _bar.End();
                })
            );
        }

        [Test]
        public void Fail_when_additional_related_calls_at_end()
        {
            _foo.Start();
            _bar.Begin();
            _foo.Finish();
            _bar.End();
            _foo.Start();

            Assert.Throws<CallSequenceNotFoundException>(() =>
                Received.InOrder(() =>
                {
                    _foo.Start();
                    _bar.Begin();
                    _foo.Finish();
                    _bar.End();
                })
            );
        }

        [Test]
        public void Ignore_unrelated_calls()
        {
            _foo.Start();
            _foo.FunkyStuff("get funky!");
            _foo.Finish();

            Received.InOrder(() =>
                             {
                                 _foo.Start();
                                 _foo.Finish();
                             });
        }
        
        [Test]
        public void Check_auto_subbed_props()
        {
            _foo.Start();
            _bar.Baz.Flurgle = "hi";
            _foo.Finish();


            Received.InOrder(() =>
                             {
                                 _foo.Start();
                                 _bar.Baz.Flurgle = "hi";
                                 _foo.Finish();
                             });
        }

        [Test]
        public void Fail_when_auto_subbed_prop_call_not_received()
        {
            _foo.Start();
            _bar.Baz.Flurgle = "hi";
            _foo.Finish();


            Assert.Throws<CallSequenceNotFoundException>(() =>
                Received.InOrder(() =>
                                 {
                                     _foo.Start();
                                     _bar.Baz.Flurgle = "howdy";
                                     _foo.Finish();
                                 })
             );
        }

        [Test]
        public void Ordered_calls_should_ignore_property_getter_calls()
        {
            var baz = _bar.Baz;
            baz.Wurgle();
            baz.Slurgle();

            Received.InOrder(() =>
                             {
                                 // This call spec should be regarded as matching the
                                 // calling code above. So needs to ignore the get 
                                 // request to _bar.Baz.
                                 _bar.Baz.Wurgle();
                                 _bar.Baz.Slurgle();
                             });
        }

        [Test]
        public void Ordered_calls_with_delegates()
        {
            var func = Substitute.For<Func<string>>();
            func();
            func();

            Received.InOrder(() =>
                             {
                                 func();
                                 func();
                             });
        }

        [Test]
        public void Non_matching_ordered_calls_with_delegates()
        {
            var func = Substitute.For<Action>();
            func();

            Assert.Throws<CallSequenceNotFoundException>(() =>
                 Received.InOrder(() =>
                                  {
                                      func();
                                      func();
                                  })
                );
        }

        [Test]
        public void Event_subscription()
        {
            _foo.OnFoo += () => { };

            Received.InOrder(() =>
                             {
                                 _foo.OnFoo += Arg.Any<Action>();
                             });
        }
        
        [Test]
        public void Pass_when_exception_in_first_sequenced_call_is_caught_and_second_call_is_exact()
        {
            try
            {
                Received.InOrder(() =>
                {
                    throw new Exception("An Exception!");
                });
            }
            catch (Exception)
            {
                //suppress error
            }
            
            _foo.Start(2);
            _bar.Begin();
            _foo.Finish();
            _bar.End();
            
            Received.InOrder(() =>
            {
                _foo.Start(2);
                _bar.Begin();
                _foo.Finish();
                _bar.End();

            });
        }

        [SetUp]
        public void SetUp()
        {
            _foo = Substitute.For<IFoo>();
            _bar = Substitute.For<IBar>();
        }

        public interface IFoo
        {
            void Start();
            void Start(int i);
            void Finish();
            void FunkyStuff(string s);
            event Action OnFoo;
        }

        public interface IBar
        {
            void Begin();
            void End();
            IBaz Baz { get; }
        }

        public interface IBaz
        {
            string Flurgle { get; set; }
            void Wurgle();
            void Slurgle();
        }
    }
}