using NUnit.Framework;
using QuikSharp.DataStructures;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Tests.TypeConverter
{
    [TestFixture]
    public class CachingQuikTypeConverterTests
    {
        private ITypeConverter _typeConverter;

        [SetUp]
        public void SetUp()
        {
            _typeConverter = new TypeConverters.CachingQuikTypeConverter();
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
            var result = string.Empty;
            var testEnum1Values = (TestEnum1[])Enum.GetValues(typeof(TestEnum1));

            //for (int i = 0; i < testEnum1Values.Length; i++)
            //{
            //    result = _typeConverter.ToStringAsInt(testEnum1Values[i]);
            //}

            //var testEnum2Values = (TestEnum2[])Enum.GetValues(typeof(TestEnum2));

            //for (int i = 0; i < testEnum2Values.Length; i++)
            //{
            //    result = _typeConverter.ToStringAsInt(testEnum2Values[i]);
            //}

            result += result;
        }
    }
}
