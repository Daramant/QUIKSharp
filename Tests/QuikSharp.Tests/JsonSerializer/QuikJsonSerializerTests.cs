using NUnit.Framework;
using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Json.Serializers;
using QuikSharp.TypeConverters;
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
        private IJsonSerializer _jsonSerializer;

        [SetUp]
        public void SetUp()
        {
            _jsonSerializer = new QuikJsonSerializer();
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

            var j = _jsonSerializer.Serialize(t);
            var t2 = _jsonSerializer.Deserialize<Transaction>(j);

            Assert.AreEqual(t.PRICE, t2.PRICE);
            Assert.AreEqual(t.QUANTITY, t2.QUANTITY);
            Assert.AreEqual(t.REFUNDRATE, t2.REFUNDRATE);
            Assert.AreEqual(t.ACTIVE_FROM_TIME, t2.ACTIVE_FROM_TIME);
            Assert.AreEqual(t.ACTIVE_TO_TIME, t2.ACTIVE_TO_TIME);
        }
    }
}
