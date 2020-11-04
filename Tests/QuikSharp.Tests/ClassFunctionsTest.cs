using System;
using NUnit.Framework;

namespace QuikSharp.Tests
{
    [TestFixture]
    public class ClassFunctionsTest : BaseTest
    {
        [SetUp]
        public void SetUp()
        {
            SetUpQuik();
        }

        [Test]
        public void GetClassesList()
        {
            Console.WriteLine("GetClassesList: "
                + String.Join(",", Quik.Functions.Class.GetClassesList().Result));
        }

        [Test]
        public void GetClassInfo()
        {
            var list = Quik.Functions.Class.GetClassesList().Result;
            foreach (var s in list)
            {
                Console.WriteLine("GetClassInfo for " + s + ": "
                + String.Join(",", Quik.Functions.Class.GetClassInfo(s).Result));
            }
        }



        [Test]
        public void GetClassSecurities()
        {
            var list = Quik.Functions.Class.GetClassesList().Result;
            foreach (var s in list)
            {
                Console.WriteLine("GetClassSecurities for " + s + ": "
                + String.Join(",", Quik.Functions.Class.GetClassSecurities(s).Result));
            }
        }

        [Test]
        public void GetSecurityInfo()
        {
            var securityInfo = Quik.Functions.Class.GetSecurityInfo("SPBFUT", "RIM5").Result;
            Console.WriteLine("GetSecurityInfo for RIM5: " + string.Join(",", JsonSerializer.Serialize(securityInfo)));

            securityInfo = Quik.Functions.Class.GetSecurityInfo("TQBR", "LKOH").Result;
            Console.WriteLine("GetSecurityInfo for LKOH: " + string.Join(",", JsonSerializer.Serialize(securityInfo)));
        }

    }
}
