using NtripCore.Manager.Shared.Interfaces.Services.System;

namespace NtripCore.Manager.Services.System.Linux
{
    public class SystemdNtripCoreCasterManager : INtripCasterManager
    {
        private readonly IConfiguration _configuration;

        public SystemdNtripCoreCasterManager(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<bool> IsNtripCasterServiceRunning()
        {
            throw new NotImplementedException();
        }

        public Task StartNtripCasterService()
        {
            // at first write a config file

            // then start service

            throw new NotImplementedException();
        }

        public Task StopNtripCasterService()
        {
            throw new NotImplementedException();
        }
    }
}
