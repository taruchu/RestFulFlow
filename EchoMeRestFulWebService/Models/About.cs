using EchoMeRestFulWebService.Interface;
using System;

namespace EchoMeRestFulWebService.Models
{
    public class About : IAbout
    {
        public string AboutEchoMe
        {
            get
            {
                return "A echo service that will echo your request back to you.";
            }
        }

    }
}
