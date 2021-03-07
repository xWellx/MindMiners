using Microsoft.Extensions.DependencyInjection;
using MindMiners.MiniProjeto.Applications.Srt;
using MindMiners.MiniProjeto.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace MindMiners.MiniProjeto.Applications
{
    public static class ApplicationStartup
    {
        public static void Startup(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISrtApplication, SrtApplication>();

            DomainStartup.Startup(serviceCollection);
        }
    }
}
