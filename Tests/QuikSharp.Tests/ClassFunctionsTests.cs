using System;
using NUnit.Framework;

namespace QuikSharp.Tests
{
    [TestFixture]
    public class ClassFunctionsTests : BaseTest
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
                + String.Join(",", Quik.Functions.Class.GetClassesListAsync().Result));
        }

        [Test]
        public void GetClassInfo()
        {
            var list = Quik.Functions.Class.GetClassesListAsync().Result;
            foreach (var s in list)
            {
                Console.WriteLine("GetClassInfo for " + s + ": "
                + String.Join(",", Quik.Functions.Class.GetClassInfoAsync(s).Result));
            }
        }



        [Test]
        public void GetClassSecurities()
        {
            var list = Quik.Functions.Class.GetClassesListAsync().Result;
            foreach (var s in list)
            {
                Console.WriteLine("GetClassSecurities for " + s + ": "
                + String.Join(",", Quik.Functions.Class.GetClassSecuritiesAsync(s).Result));
            }
        }

        [Test]
        public void GetSecurityInfo()
        {
            var securityInfo = Quik.Functions.Workstation.GetSecurityInfoAsync("SPBFUT", "RIM5").Result;
            Console.WriteLine("GetSecurityInfo for RIM5: " + string.Join(",", Serializer.Serialize(securityInfo)));

            securityInfo = Quik.Functions.Workstation.GetSecurityInfoAsync("TQBR", "LKOH").Result;
            Console.WriteLine("GetSecurityInfo for LKOH: " + string.Join(",", Serializer.Serialize(securityInfo)));
        }

    }
}
