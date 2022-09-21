using Jobsity.Bot.Hubs;
using Jobsity.Bot.RabbitMQ;

namespace Jobsity.Tests
{
    public class BotTests
    {
        BotHub _hub;
        Sender _sender;

        public BotTests()
        {
            _sender = new Sender(null, null);
            _hub = new BotHub(_sender);
        }

        [Theory]
        [InlineData("aapl.us")]
        public void ReadCSV_RightStock(string stock)
        {
            string result = _hub.GetCSV(stock);
            string val = "156.9";

            Assert.Equal(string.Format("{0} quote is ${1} per share.", stock.ToUpper(), val), result);
        }

        [Fact]
        public void ReadCSV_EmptyStock()
        {
            string result = _hub.GetCSV("");

            Assert.Equal("Stock code can't be null. Please, try again.", result);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("abc")]
        [InlineData("xyz")]
        public void ReadCSV_WrongStock(string stock)
        {
            string result = _hub.GetCSV(stock);

            Assert.Equal(string.Format("Stock code '{0}' doesn't exist. Please, try again.", stock), result);
        }

    }
}