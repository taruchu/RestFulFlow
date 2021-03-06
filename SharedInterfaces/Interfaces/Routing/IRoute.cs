﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces.Interfaces.Routing
{ 
    public interface IRoute<T>
    {
        string Route { get; set; } //Example: <RouterBusKeyCode>.<ServiceName>  "BE60675D-DB24-45A2-925C-00C5DC753C92.ChatMessageService"
        Action<T> RegisterRouteHandler { get; set; } //NOTE: The handler will validate the JSON parameter using a JSON schema
    }
}
