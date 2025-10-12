using Dakali;
using Dapper;
using DK.Model.Meta;
using DK.Repositories.Interface.Meta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DK.Repositories.Meta
{
    public class WebHookEventRepository : IWebHookEventRepository
    {
        public Task<WebHookEvent> Create(WebHookEvent webHookEvent)
        {
            throw new NotImplementedException();
        }

        public async Task<WebHookEvent> Get(long id)
        {
            var query = "select * from WebHookEvents where Id = @Id";
            return await ContextManager.Session.Connection.QueryFirstAsync<WebHookEvent>(query, new { Id = id });
        }

        public async Task<IEnumerable<WebHookEvent>> Get(EventType eventType)
        {
            var query = "select * from WebHookEvents where EventType = @EventType";
            return await ContextManager.Session.Connection.QueryAsync<WebHookEvent>(query, new { EventType = eventType });
        }

        public async Task<IEnumerable<WebHookEvent>> Get(bool isProcessed)
        {
            var query = "select * from WebHookEvents where IsProcessed = @IsProcessed";
            return await ContextManager.Session.Connection.QueryAsync<WebHookEvent>(query, new { IsProcessed = isProcessed });
        }

        public async Task<IEnumerable<WebHookEvent>> GetAsError()
        {
            var query = "select * from WebHookEvents where IsProcessed = 0 and (Error is null or Error = '')";
            return await ContextManager.Session.Connection.QueryAsync<WebHookEvent>(query);
        }
    }
}
