using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Quiltoni.PixelBot.Core.Data;

namespace PixelBot.Orchestrator.Data {

    public class SqliteWidgetStateRepository : IWidgetStateRepository
    {

        public SqliteWidgetStateRepository(IConfiguration configuration)
        {
            Connectionstring = configuration["WidgetPersistenceConnectionstring"];
        }

        public string Connectionstring { get; }

        public Task<Dictionary<string, string>> Get(string channelName, string widgetName)
        {
            throw new System.NotImplementedException();
        }

        public Task Save(string channelName, string widgetName, Dictionary<string, string> payload)
        {
            throw new System.NotImplementedException();
        }
    }

}