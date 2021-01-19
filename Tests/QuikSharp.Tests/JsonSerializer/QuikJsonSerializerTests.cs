using NUnit.Framework;
using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.QuikClient;
using QuikSharp.QuikEvents;
using QuikSharp.Serialization;
using QuikSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Tests.JsonSerializer
{
    [TestFixture]
    public class QuikJsonSerializerTests
    {
        private ISerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            var pendingResultContainer = new PendingResultContainer();
            var eventTypeProvider = new EventTypeProvider();
            _serializer = new QuikJsonSerializer(pendingResultContainer, eventTypeProvider);
        }

        private enum TestEnum1
        {
            A = 0,
            B = 1,
            C = 2,
            D = 3
        }

        private enum TestEnum2
        {
            A = 2,
            B = 3,
            C = 4,
            E = 5,
            F = 20
        }

        [Test]
        public void Test()
        {
            var t = new Transaction();
            t.PRICE = 123.456m;
            t.QUANTITY = 123;
            t.REFUNDRATE = 45.67m;
            t.TYPE = TransactionType.M;
            t.MARKET_MAKER_ORDER = YesOrNo.NO;
            t.EXECUTION_CONDITION = ExecutionCondition.FILL_OR_KILL;
            t.ACTIVE_FROM_TIME = DateTime.Now;
            t.ACTIVE_TO_TIME = DateTime.Now.AddMinutes(10);

            var j = _serializer.Serialize(t);
            t.QUANTITY = 1234;
            j += _serializer.Serialize(t);
            using (var reader = new System.IO.StringReader(j))
            {
                var tp1 = _serializer.Deserialize(reader, typeof(Transaction));
                var tp2 = _serializer.Deserialize(reader, typeof(Transaction));
            }

            j = _serializer.Serialize(t);
            t.ORDER_KEY = j;
            j = _serializer.Serialize(t);
            var t2 = _serializer.Deserialize<Transaction>(j);

            Assert.AreEqual(t.PRICE, t2.PRICE);
            Assert.AreEqual(t.QUANTITY, t2.QUANTITY);
            Assert.AreEqual(t.REFUNDRATE, t2.REFUNDRATE);
            Assert.AreEqual(t.ACTIVE_FROM_TIME, t2.ACTIVE_FROM_TIME);
            Assert.AreEqual(t.ACTIVE_TO_TIME, t2.ACTIVE_TO_TIME);
        }

        [Test]
        public void TestTypeNames()
        {
            var name = typeof(QuikSharp.Messages.Result<OrderBook>).Name;
            var fullName = typeof(QuikSharp.Messages.Result<OrderBook>).FullName;
            var typeGuid = typeof(QuikSharp.Messages.Result<OrderBook>).GUID;
            //var type1 = Type.GetType(name, );
        }
    }
}
