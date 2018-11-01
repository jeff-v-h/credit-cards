using CreditCards.Core.Model;
using Xunit;

namespace CreditCards.Tests.Model
{
    public class CreditCardApplicationEvaluatorShould
    {
        private const int ExpectedLowIncomeThreshhold = 20_000;
        private const int ExpectedHighIncomeThreshhold = 100_000;

        [Theory]
        [InlineData(ExpectedHighIncomeThreshhold)]
        [InlineData(ExpectedHighIncomeThreshhold + 1)]
        [InlineData(int.MaxValue)]
        public void AcceptAllHighIncomeApplicants(int income)
        {
            var sut = new CreditCardApplicationEvaluator();

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = income
            };

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted,
                sut.Evaluate(application));
        }




        [Theory]
        [InlineData(20)]
        [InlineData(19)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void ReferYoungApplicantsWhoAreNotHighIncome(int age)
        {
            var sut = new CreditCardApplicationEvaluator();

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = ExpectedHighIncomeThreshhold - 1,
                Age = age
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman,
                sut.Evaluate(application));
        }


        [Theory]
        [InlineData(ExpectedLowIncomeThreshhold)]
        [InlineData(ExpectedLowIncomeThreshhold + 1)]
        [InlineData(ExpectedHighIncomeThreshhold - 1)]
        public void ReferNonYoungApplicantsWhoAreMiddleIncome(int income)
        {
            var sut = new CreditCardApplicationEvaluator();

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = income,
                Age = 21
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman,
                sut.Evaluate(application));
        }


        [Theory]
        [InlineData(ExpectedLowIncomeThreshhold - 1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void DeclineAllApplicantsWhoAreLowIncome(int income)
        {
            var sut = new CreditCardApplicationEvaluator();

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = income,
                Age = 21
            };

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined,
                sut.Evaluate(application));
        }
    }
}
