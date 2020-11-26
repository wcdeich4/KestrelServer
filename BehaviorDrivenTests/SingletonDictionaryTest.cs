using System;
using Xunit;
using Xunit.Gherkin.Quick;
using BasicObjects;

namespace BehaviorDrivenTests
{
    [FeatureFile("./SingletonDictionaryTest.feature")]
    public sealed class SingletonDictionaryTest : Feature
    {
        private SingletonDictionary singletonDictionary ;

        [Given(@"We got the single instance of the Singleton Dictionary")]
        public void getSingletonDictionary()
        {
            singletonDictionary = SingletonDictionary.GetInstance();
        }

        [When(@"we AddOrUpdate key value pair (.+) (.+)")]
        [And(@"We AddOrUpdate key value pair (.+) (.+)")]
        public void AddOrUpdate(string inputKey, string inputValue)
        {
            singletonDictionary.AddOrUpdate(inputKey, inputValue);
        }

        [Then(@"The value for (.+) will be (.+)")]
        [And(@"The value for (.+) will be (.+)")]
        public void GetFromDictionaryAndCompare(string keyToSearchWith, string valueExpectedToBeFound)
        {
           // System.Console.WriteLine("a = " + a);
           // System.Console.WriteLine("b = " + b);
           string valueActuallyFound = singletonDictionary[keyToSearchWith] as string;
            Assert.Equal(valueExpectedToBeFound, valueActuallyFound);
        }

        [And(@"The SubstituteIfFound value for (.+) is (.+)")]
        public void SubstituteIfFoundFromDictionaryAndCompare(string keyToSearchWith, string valueExpectedToBeFound)
        {
           // System.Console.WriteLine("a = " + a);
           // System.Console.WriteLine("b = " + b);
           string valueActuallyFound = singletonDictionary.SubstituteIfFound(keyToSearchWith) as string;
            Assert.Equal(valueExpectedToBeFound, valueActuallyFound);
        }


    }
}