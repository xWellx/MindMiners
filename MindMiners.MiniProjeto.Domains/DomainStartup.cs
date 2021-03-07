using Microsoft.Extensions.DependencyInjection;
using MindMiners.MiniProjeto.Domains.Srt;
using System;
using System.Collections.Generic;
using System.Text;

namespace MindMiners.MiniProjeto.Domains
{
    public static class DomainStartup
    {
        public static void Startup(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISrtDomain, SrtDomain>();
        }
    }
}
