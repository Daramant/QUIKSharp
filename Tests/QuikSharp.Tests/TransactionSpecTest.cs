using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Json.Converters;
using QuikSharp.Json.Serializers;
using QuikSharp.Quik;
using QuikSharp.QuikService;

namespace QuikSharp.Tests
{
    public class TypeWithNumberSerializedAsString
    {
        public int NormalInt { get; set; }
        [JsonConverter(typeof(ToStringConverter<int?>))]
        public int? AsString { get; set; }
    }

    public class TypeWithNumberDeSerializedAsString
    {
        public int NormalInt { get; set; }
        public string AsString { get; set; }
    }

    public class TypeWithDateTimeSerializedAsString
    {
        [JsonConverter(typeof(HHMMSSDateTimeConverter))]
        public DateTime? AsString { get; set; }
    }

    public class TypeWithDateTimeDeSerializedAsString
    {
        public string AsString { get; set; }
    }

    [TestFixture]
    public class TransactionSpecTest : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            SetUpQuik();
        }

        /// <summary>
        /// Make sure that Buy or Sell is never set by default
        /// That would be a very stupid mistake, but such king of mistakes are the most
        /// dangerous because one never believes he will make them
        /// </summary>
        [Test]
        public void UndefinedEnumCannotBeSerialized()
        {
            var op = (TransactionOperation)10;
            var json = JsonSerializer.Serialize(op);
            Assert.AreEqual(json, "null");
            Console.WriteLine(json);
            op = (TransactionOperation)1;
            json = JsonSerializer.Serialize(op);
            Console.WriteLine(json);
            Assert.AreEqual(json, JsonSerializer.Serialize("B"));

            var act = (TransactionAction)0;
            json = JsonSerializer.Serialize(act);
            Console.WriteLine(json);
            Assert.AreEqual(json, "null");

            var yesNo = (YesOrNo)0;
            json = JsonSerializer.Serialize(yesNo);
            Console.WriteLine(json);
            Assert.AreEqual(json, "null");

            var yesNoDef = (YesOrNoDefault)0;
            json = JsonSerializer.Serialize(yesNoDef);
            Console.WriteLine(json);
            Assert.AreEqual(json, JsonSerializer.Serialize("NO"));
        }


        [Test]
        public void CouldSerializeNumberPropertyAsString()
        {
            var t = new TypeWithNumberSerializedAsString();
            t.NormalInt = 123;
            t.AsString = 456;
            var j = JsonSerializer.Serialize(t);
            Console.WriteLine(j);
            var t2 = JsonSerializer.Deserialize<TypeWithNumberDeSerializedAsString>(j);
            Assert.AreEqual("456", t2.AsString);
            var t1 = JsonSerializer.Deserialize<TypeWithNumberSerializedAsString>(j);
            Assert.AreEqual(456, t1.AsString);
        }


        [Test]
        public void CouldSerializeDateTimePropertyAsString()
        {
            var t = new TypeWithDateTimeSerializedAsString();
            var now = DateTime.Now;
            t.AsString = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var j = JsonSerializer.Serialize(t);
            Console.WriteLine(j);
            var t2 = JsonSerializer.Deserialize<TypeWithDateTimeDeSerializedAsString>(j);
            Assert.AreEqual(t.AsString.Value.ToString("HHmmss"), t2.AsString);
            var t1 = JsonSerializer.Deserialize<TypeWithDateTimeSerializedAsString>(j);
            Assert.AreEqual(t.AsString, t1.AsString);
        }

        [Test]
        public void CouldSerializeEmptyTransactionSpec()
        {
            var t = new Transaction();
            var j = JsonSerializer.Serialize(t);
            Console.WriteLine(j);
            var t2 = JsonSerializer.Deserialize<Transaction>(j);
            Assert.AreEqual(JsonSerializer.Serialize(t), JsonSerializer.Serialize(t2));
        }


        [Test]
        public void CouldSerializeEmptyTransactionSpecMulti()
        {
            var t = new Transaction();
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                var j = JsonSerializer.Serialize(t);
                var t2 = JsonSerializer.Deserialize<Transaction>(j);
            }
            sw.Stop();
            Console.WriteLine("Multiserialization takes msecs: " + sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Very important than this works!
        /// (both with nulls and with ignored nulls
        /// </summary>
        [Test]
        public void CouldEchoTransactionSpec()
        {
            var t = new Transaction();
            var echoed = Quik.Functions.Debug.Echo(t).Result;
            Console.WriteLine(JsonSerializer.Serialize(t));
            Console.WriteLine(JsonSerializer.Serialize(echoed));
            Assert.AreEqual(JsonSerializer.Serialize(t), JsonSerializer.Serialize(echoed));
        }

        [Test]
        public void MultiEchoTransactionSpec()
        {

            var sw = new Stopwatch();
            Console.WriteLine("Started");
            for (int round = 0; round < 10; round++)
            {
                sw.Reset();
                sw.Start();

                var count = 1000;
                var t = new Transaction();

                var array = new Task<Transaction>[count];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = Quik.Functions.Debug.Echo(t);
                }
                for (int i = 0; i < array.Length; i++)
                {
                    var res = array[i].Result;
                    array[i] = null;
                }

                sw.Stop();
                Console.WriteLine("MultiPing takes msecs: " + sw.ElapsedMilliseconds);
            }
        }


        [Test]
        public void CouldSendEmptyTransactionSpec()
        {
            var t = new Transaction();
            var result = Quik.Functions.Trading.SendTransaction(t).Result;

            Console.WriteLine("Sent Id: " + t.TRANS_ID);
            Console.WriteLine("Result Id: " + result);
            Assert.IsTrue(result < 0);
            Console.WriteLine("Error: " + t.ErrorMessage);
        }

        [Test]
        public void TransactionPriceWithoutTrailoringZeros()
        {
            // Проверка, что в цене отбрасываются незначащие нули, т.к. в противном случае возвращается ошибка:
            // ошибка отправки транзакции Неправильно указана цена: "81890,000000"
            // Сообщение об ошибке: Число не может содержать знак разделителя дробной части

            var t1 = new Transaction { PRICE = 1.00000m };
            var t2 = new Transaction { PRICE = 1.01000m };
            string json1 = JsonSerializer.Serialize(t1);
            string json2 = JsonSerializer.Serialize(t2);

            Assert.IsTrue(json1.Contains("\"PRICE\":\"1\"}"));
            Assert.IsTrue(json2.Contains("\"PRICE\":\"1,01\"}") || json2.Contains("\"PRICE\":\"1.01\"}"));
        }
    }
}