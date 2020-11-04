using System;
using NUnit.Framework;

namespace QuikSharp.Tests
{
    [TestFixture]
    public class OrderBookFunctionsTest : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            SetUpQuik();
        }

        [Test]
        public void Subscribe_Level_II_Quotes()
        {
            Console.WriteLine("Subscribe_Level_II_Quotes: "
                + String.Join(",", Quik.Functions.OrderBook.Subscribe("SPBFUT", "RIH5").Result));
        }

        [Test]
        public void Unsubscribe_Level_II_Quotes()
        {
            Console.WriteLine("Unsubscribe_Level_II_Quotes: "
                + String.Join(",", Quik.Functions.OrderBook.Unsubscribe("SPBFUT", "RIH5").Result));
        }

        [Test]
        public void IsSubscribed_Level_II_Quotes()
        {
            Console.WriteLine("IsSubscribed_Level_II_Quotes: "
                + String.Join(",", Quik.Functions.OrderBook.IsSubscribed("SPBFUT", "RIH5").Result));
        }


    }
}
