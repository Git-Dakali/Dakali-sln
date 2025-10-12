using DK.Model.Meta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DK.Repositories.Interface.Meta
{
    public interface IWebHookEventRepository
    {
        Task<WebHookEvent> Create(WebHookEvent webHookEvent);
        Task<WebHookEvent> Get(long id);
        Task<IEnumerable<WebHookEvent>> Get(EventType eventType);
        Task<IEnumerable<WebHookEvent>> GetAsError();
        Task<IEnumerable<WebHookEvent>> Get(bool isProcessed);
    }
}
