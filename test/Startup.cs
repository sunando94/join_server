using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(test.Startup))]

namespace test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           
        }
    }
}
