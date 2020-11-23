﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using QuikSharp.DataStructures;
using QuikSharp.Quik;

namespace QuikSharp.Tests
{
    [TestFixture]
    public class CandleFunctionsTests : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            SetUpQuik();
        }

        [Test]
        public void GetCandlesTest()
        {
            //Quik quik = new Quik();
            string graphicTag = "RIU5M1";//В квике должен быть открыт график с этим (SBER2M) идентификатором.

            List<Candle> allCandles = Quik.Functions.Candles.GetAllCandlesAsync(graphicTag).Result;
            Console.WriteLine("All candles count: " + allCandles.Count);

            List<Candle> partCandles = Quik.Functions.Candles.GetCandlesAsync(graphicTag, 0, 100, 250).Result;
            Console.WriteLine("Part candles count:" + partCandles.Count);
        }

        [Test]
        public void GetAllCandlesTest()
        {
            //Получаем месячные свечки по инструменту "Северсталь"
            List<Candle> candles = Quik.Functions.Candles.GetAllCandlesAsync("TQBR", "CHMF", CandleInterval.MN).Result;
            Trace.WriteLine("Candles count: " + candles.Count);
        }

        [Test]
        public void GetLastCandlesTest()
        {
            int Days = 7;
            List<Candle> candles = Quik.Functions.Candles.GetLastCandlesAsync("TQBR", "SBER", CandleInterval.D1, Days).Result;
            Assert.AreEqual(Days, candles.Count);

            Days = 77;
            candles = Quik.Functions.Candles.GetLastCandlesAsync("TQBR", "SBER", CandleInterval.D1, Days).Result;
            Assert.AreEqual(Days, candles.Count);

            Days = 1;
            candles = Quik.Functions.Candles.GetLastCandlesAsync("TQBR", "SBER", CandleInterval.D1, Days).Result;
            Assert.AreEqual(Days, candles.Count);
        }

        [Test]
        public void CandlesSubscriptionTest()
        {
            Quik.Events.Candle += OnCandle;

            // На всякий случай вначале нужно отписатся (иначе может вылететь Assert)
            // TODO: Вообще у библиотеки огромная проблема - Lua скрипт не отписывается от того к чему он подписался при отключении клиента.
            // В результате при следующем подключении клиент начинает получать сразу кучу CallBack'ов, на которые он не подписывался в текущей сессии.
            // По большому счету сейчас клиент должен сам заботаться о том, что бы гарантированно отписываться от всего к чему подписался при выходе.
            bool isSubscribed = Quik.Functions.Candles.IsSubscribedAsync("TQBR", "SBER", CandleInterval.M1).Result;
            if (isSubscribed)
                Quik.Functions.Candles.UnsubscribeAsync("TQBR", "SBER", CandleInterval.M1).Wait();

            // Проверяем что мы действительно отписались
            isSubscribed = Quik.Functions.Candles.IsSubscribedAsync("TQBR", "SBER", CandleInterval.M1).Result;
            Assert.AreEqual(false, isSubscribed);

            Quik.Functions.Candles.SubscribeAsync("TQBR", "SBER", CandleInterval.M1).Wait();
            isSubscribed = Quik.Functions.Candles.IsSubscribedAsync("TQBR", "SBER", CandleInterval.M1).Result;
            Assert.AreEqual(true, isSubscribed);

            // Раскомментарить если необходимо получать данные в функции OnNewCandle 2 минуты. В течении этих двух минут должна прийти еще одна свечка
            //Thread.Sleep(120000);//must get at leat one candle as use minute timeframe

            Quik.Functions.Candles.UnsubscribeAsync("TQBR", "SBER", CandleInterval.M1).Wait();
            isSubscribed = Quik.Functions.Candles.IsSubscribedAsync("TQBR", "SBER", CandleInterval.M1).Result;
            Assert.AreEqual(false, isSubscribed);


        }

        private void OnCandle(IQuik quik, Candle candle)
        {
            if (candle.SecCode == "SBER" && candle.ClassCode == "TQBR" && candle.Interval == CandleInterval.M1)
            {
                Console.WriteLine("Sec:{0}, Open:{1}, Close:{2}, Volume:{3}", candle.SecCode, candle.Open, candle.Close, candle.Volume);
            }
        }
    }
}
