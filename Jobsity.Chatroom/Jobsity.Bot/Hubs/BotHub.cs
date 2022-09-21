using CsvHelper;
using Jobsity.Bot.DTOs;
using Jobsity.Bot.Mappings;
using Jobsity.Bot.Models;
using Jobsity.Bot.RabbitMQ;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using System.Net;

namespace Jobsity.Bot.Hubs
{
    public class BotHub : Hub
    {
        private readonly Sender _sender;

        public BotHub(Sender sender)
        {
            _sender = sender;
        }

        public void StockBot(StockMessage stockMessage)
        {
            _sender.SendMessage(string.Format("{0}:{1}", stockMessage.Room, GetCSV(stockMessage.StockCode)));
        }

        public string GetCSV(string stockCode)
        {
            if (String.IsNullOrEmpty(stockCode))
                return "Stock code can't be null. Please, try again.";
            else
            {
                try
                {
                    string url = "https://stooq.com/q/l/?s=" + stockCode + "&f=sd2t2ohlcv&h&e=csv";
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                    using (var reader = new StreamReader(resp.GetResponseStream()))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<CVSFileMap>();
                        var record = csv.GetRecords<CSVFile>().FirstOrDefault();
                        if (record != null && record.Close != "N/D")
                            return string.Format("{0} quote is ${1} per share.", record.Stock_Code, record.Close);
                        else
                            return string.Format("Stock code '{0}' doesn't exist. Please, try again.", stockCode);
                    }
                }
                catch (BadDataException)
                {
                    return "Bad data in file. Please, try again.";
                }
                catch (UriFormatException)
                {
                    return "Couldn't read the information. Please, try again.";
                }
            }
        }
    }
}
