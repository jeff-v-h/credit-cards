using CreditCards.Core.Model;
using System;
using Xunit;

namespace CreditCard.Tests.Model
{
    public class FrequentFlyerValidatorShould
    {
        [Theory]
        [InlineData("012345-A")]
        [InlineData("012345-Q")]
        [InlineData("012345-Y")]
        // All the above inline data will be fed in to the parameter ('number') of this method to be tested
        public void AcceptValidSchemes(string number)
        {
            var sut = new FrequentFlyerNumberValidator();

            Assert.True(sut.IsValid(number));
        }
    }
}
