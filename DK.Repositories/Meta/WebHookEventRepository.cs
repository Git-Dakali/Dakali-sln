using Dakali.Interface.Connection;
using Dapper;
using DK.Domain.Meta;
using DK.Repositories.Interface.Meta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DK.Repositories.Meta
{
    public class WebHookEventRepository : IWebHookEventRepository
    {
        private ISession _session;
        public WebHookEventRepository(ISession session)
        { 
            _session = session;
        }

        public Task<WebHookEvent> Create(WebHookEvent webHookEvent)
        {
            throw new NotImplementedException();
        }

        public async Task<WebHookEvent> Get(long id)
        {
            var query = "select * from WebHookEvents where Id = @Id";
            return await _session.Connection.QueryFirstAsync<WebHookEvent>(query, new { Id = id });
        }

        public async Task<IEnumerable<WebHookEvent>> Get(EventType eventType)
        {
            var query = "select * from WebHookEvents where EventType = @EventType";
            return await _session.Connection.QueryAsync<WebHookEvent>(query, new { EventType = eventType });
        }

        public async Task<IEnumerable<WebHookEvent>> Get(bool isProcessed)
        {
            var query = "select * from WebHookEvents where IsProcessed = @IsProcessed";
            return await _session.Connection.QueryAsync<WebHookEvent>(query, new { IsProcessed = isProcessed });
        }

        public async Task<IEnumerable<WebHookEvent>> GetAsError()
        {
            var query = "select * from WebHookEvents where IsProcessed = 0 and (Error is null or Error = '')";
            return await _session.Connection.QueryAsync<WebHookEvent>(query);
        }
    }
}
