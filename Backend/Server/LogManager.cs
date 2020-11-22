using JCIW.Data.Interfaces;
using Repository.Entities;
using Repository.Interface;
using System.Collections.Generic;

namespace Server
{
    /// <summary>
    /// This class implements <see cref="ILogManager"/>.
    /// </summary>
    public class LogManager : ILogManager
    {
        private readonly IServiceLogRepository serviceLogRepository;

        public LogManager()
        {
            this.serviceLogRepository = RepositoryFactory.ServiceLogRepository();
        }

        public void Add(long serviceId, string text)
        {
            serviceLogRepository.AddLog(serviceId, text);
        }

        public List<ServiceLog> Get(long serviceId, long dateLimit)
        {
            return serviceLogRepository.RetrieveLog(serviceId, dateLimit);
        }

        public List<ServiceLog> Get(long serviceId)
        {
            return serviceLogRepository.RetrieveLog(serviceId);
        }
    }
}
