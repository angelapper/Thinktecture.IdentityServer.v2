using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace Tests
{
    [TestClass]
    public class TestRedis
    {
        [TestMethod]
        public void RedisCanBeConnected()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            string value = "abcdefg";
            db.StringSet("mykey", value);

            string result = db.StringGet("mykey");
            Console.WriteLine(value); // writes: "abcdefg"
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        async public void RedisCanBeConnectedAsync()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            string value = "abcdefg";
            await db.StringSetAsync("mykey", value);

            string result = await db.StringGetAsync("mykey");
            Console.WriteLine(value); // writes: "abcdefg"
            Assert.AreEqual(value, result);
        }
    }
}
