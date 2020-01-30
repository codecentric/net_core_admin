using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nactuator
{
    public class SpringBootClient
    {
        private readonly IAdministrationBuilder administrationBuilder;

        // todo post to SBA
        // todo retry
        // todo deregister
               
        public SpringBootClient(IAdministrationBuilder administrationBuilder)
        {
            this.administrationBuilder = administrationBuilder;
        }
             

        public string Register(string springBootAdminUrl)
        {
 
            return "";
        }
    }
}
