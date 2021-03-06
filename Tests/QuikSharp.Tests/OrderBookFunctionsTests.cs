﻿using System;
using NUnit.Framework;

namespace QuikSharp.Tests
{
    [TestFixture]
    public class OrderBookFunctionsTests : BaseTest
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
                + String.Join(",", Quik.Functions.OrderBook.SubscribeAsync("SPBFUT", "RIH5").Result));
        }

        [Test]
        public void Unsubscribe_Level_II_Quotes()
        {
            Console.WriteLine("Unsubscribe_Level_II_Quotes: "
                + String.Join(",", Quik.Functions.OrderBook.UnsubscribeAsync("SPBFUT", "RIH5").Result));
        }

        [Test]
        public void IsSubscribed_Level_II_Quotes()
        {
            Console.WriteLine("IsSubscribed_Level_II_Quotes: "
                + String.Join(",", Quik.Functions.OrderBook.IsSubscribedAsync("SPBFUT", "RIH5").Result));
        }


    }
}
