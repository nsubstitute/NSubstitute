﻿using System;
using System.Collections.Generic;
using System.Globalization;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class GenericArguments
{
    public interface ISomethingWithGenerics
    {
        void Log<TState>(int level, TState state);
    }

    [Test]
    public void Return_result_for_any_argument()
    {
        string argDoResult = null;
        int? whenDoResult = null;
        bool whenDoCalled = false;
        ISomethingWithGenerics something = Substitute.For<ISomethingWithGenerics>();
        something.Log(Arg.Any<int>(), Arg.Do<Arg.AnyType>(a => argDoResult = ">>" + ((int)a).ToString("P", CultureInfo.InvariantCulture)));
        something
            .When(substitute => substitute.Log(Arg.Any<int>(), Arg.Any<Arg.AnyType>()))
            .Do(info =>
            {
                whenDoResult = info.ArgAt<int>(1);
                whenDoCalled = true;
            });

        something.Log(7, 3409);

        something.Received().Log(Arg.Any<int>(), Arg.Any<Arg.AnyType>());
        something.Received().Log(7, 3409);
        Assert.That(whenDoCalled, Is.True);
        Assert.That(argDoResult, Is.EqualTo(">>340,900.00 %"));
        Assert.That(whenDoResult, Is.EqualTo(3409));
    }
}