using System;
using GGMREST.Proxy;
using UnitTest.Bithumb;
using UnitTest.Bithumb.Entity;
using Xunit;

namespace UnitTest
{
    public class BithumbAPITest
    {
        private IBithumbService _bithumbService;
        public BithumbAPITest()
        {
            Factory factory = new Factory("https://api.bithumb.com/public/");
            _bithumbService = factory.Create<IBithumbService>();
        }

        [Fact]
        public void Ticker()
        {
            // Unit Test에선 await 쓰기가 불편함.
            // RequestData<TickerData> ticker = await bithumbService.GetTicker();

            var getTickerTask = _bithumbService.GetTicker();
            while (!getTickerTask.IsCompleted) { }
            RequestData<TickerData> ticker = getTickerTask.Result;
            Assert.NotNull(ticker);
        }
    }
}
