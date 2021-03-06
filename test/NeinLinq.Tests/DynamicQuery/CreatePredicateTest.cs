﻿using System;
using System.Globalization;
using System.Linq;
using NeinLinq.Fakes.DynamicQuery;
using Xunit;
using static NeinLinq.DynamicQuery;

namespace NeinLinq.Tests.DynamicQuery
{
    public class CreatePredicateTest
    {
        private readonly IQueryable<Dummy> data
            = DummyStore.Data.AsQueryable();

        [Fact]
        public void ShouldHandleInvalidArguments()
        {
            _ = Assert.Throws<ArgumentNullException>(() => CreatePredicate<Dummy>(null!, DynamicCompare.Equal, null!));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => CreatePredicate<Dummy>("Number", (DynamicCompare)(object)-1, null!));
            _ = Assert.Throws<ArgumentNullException>(() => CreatePredicate<Dummy>(null!, "Contains", "b"));
            _ = Assert.Throws<ArgumentNullException>(() => CreatePredicate<Dummy>("Name", null!, "b"));
        }

        [Theory]
        [InlineData(DynamicCompare.Equal, new[] { 5 })]
        [InlineData(DynamicCompare.NotEqual, new[] { 1, 2, 3, 4, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThan, new[] { 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.GreaterThanOrEqual, new[] { 5, 6, 7, 8, 9 })]
        [InlineData(DynamicCompare.LessThan, new[] { 1, 2, 3, 4 })]
        [InlineData(DynamicCompare.LessThanOrEqual, new[] { 1, 2, 3, 4, 5 })]
        public void ShouldCreateComparison(DynamicCompare comparison, int[] result)
        {
            var culture = new CultureInfo("de-AT");

            var empty = CreatePredicate<Dummy>("Number", comparison, null);
            var compare = CreatePredicate<Dummy>("Number", comparison, "222,222", culture);

            var emptyResult = data.Where(empty).Select(d => d.Id).ToArray();
            var compareResult = data.Where(compare).Select(d => d.Id).ToArray();

            var count = comparison == DynamicCompare.NotEqual ? 9 : 0;

            Assert.Equal(count, emptyResult.Length);
            Assert.Equal(result, compareResult);
        }

        [Fact]
        public void ShouldCreateCustomComparison()
        {
            var contains = CreatePredicate<Dummy>("Name", "Contains", "b");

            var containsResult = data.Where(contains).Select(d => d.Id).ToArray();

            Assert.Equal(new[] { 2, 5, 8 }, containsResult);
        }

        [Fact]
        public void ShouldSupportGuidsToo()
        {
            var expected = data.First().Reference;

            var predicate = CreatePredicate<Dummy>("Reference", DynamicCompare.Equal, expected.ToString());

            var actual = data.Where(predicate).Select(d => d.Reference).ToArray();

            Assert.Equal(new[] { expected }, actual);
        }

        [Theory]
        [InlineData(nameof(DummyEnum.Undefined), new[] { 1, 2, 3 })]
        [InlineData(nameof(DummyEnum.One), new[] { 4, 5, 6 })]
        [InlineData(nameof(DummyEnum.Two), new[] { 7, 8, 9 })]
        [InlineData("0", new[] { 1, 2, 3 })]
        [InlineData("1", new[] { 4, 5, 6 })]
        [InlineData("2", new[] { 7, 8, 9 })]
        public void ShouldSupportEnumsToo(string value, int[] expectedResult)
        {
            var predicate = CreatePredicate<Dummy>(nameof(Dummy.Enum), DynamicCompare.Equal, value);

            var result = data.Where(predicate).Select(d => d.Id).ToArray();

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(nameof(DummyEnum.Undefined), new[] { 4, 5 })]
        [InlineData(nameof(DummyEnum.One), new[] { 7, 8 })]
        [InlineData(nameof(DummyEnum.Two), new[] { 1, 2 })]
        [InlineData("0", new[] { 4, 5 })]
        [InlineData("1", new[] { 7, 8 })]
        [InlineData("2", new[] { 1, 2 })]
        [InlineData(null, new[] { 3, 6, 9 })]
        public void ShouldSupportNullableEnumsToo(string value, int[] expectedResult)
        {
            var predicate = CreatePredicate<Dummy>(nameof(Dummy.NullableEnum), DynamicCompare.Equal, value);

            var result = data.Where(predicate).Select(d => d.Id).ToArray();

            Assert.Equal(expectedResult, result);
        }

    }
}
