using CsvHelper.Configuration;
using Jobsity.Bot.Models;

namespace Jobsity.Bot.Mappings
{
    public class CVSFileMap : ClassMap<CSVFile>
    {
        public CVSFileMap()
        {
            Map(f => f.Stock_Code).Index(0);
            Map(f => f.Close).Index(6);
            Map(f => f.Volume).Index(7);
        }
    }
}
