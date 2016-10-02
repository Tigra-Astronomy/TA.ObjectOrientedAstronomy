// This file is part of the TA.ObjectOrientedAstronomy project
// 
// Copyright © 2015-2016 Tigra Astronomy, all rights reserved.
// 
// File: MaybeSpecs.cs  Last modified: 2016-10-02@02:40 by Tim Long

using System.Linq;
using Machine.Specifications;
using TA.ObjectOrientedAstronomy.FlexibleImageTransportSystem.PropertyBinder;

namespace TA.ObjectOrientedAstronomy.Specifications
    {
    [Subject(typeof(Maybe<>), "non-empty")]
    class when_creating_a_maybe_from_a_non_null_value
        {
        Because of = () => maybe = Maybe<int>.FromValue(7);
        It should_not_be_empty = () => maybe.None.ShouldBeFalse();
        It should_be_a_non_empty_collection = () => maybe.Any().ShouldBeTrue();
        It should_contain_one_element = () => maybe.Count().ShouldEqual(1);
        It should_contain_the_wrapped_value = () => maybe.Single().ShouldEqual(7);
        static Maybe<int> maybe;
        }

    [Subject(typeof(Maybe<>), "empty")]
    class when_creating_a_maybe_from_a_null_value
        {
        Because of = () => maybe = Maybe<string>.FromValue((string) null);
        It should_be_empty = () => maybe.None.ShouldBeTrue();
        It should_be_an_empty_collection = () => maybe.Any().ShouldBeFalse();
        It should_contain_no_elements = () => maybe.Count().ShouldEqual(0);
        It should_be_reference_equals_empty = () => maybe.ShouldBeTheSameAs(Maybe<string>.Empty);
        static Maybe<string> maybe;
        }
    }