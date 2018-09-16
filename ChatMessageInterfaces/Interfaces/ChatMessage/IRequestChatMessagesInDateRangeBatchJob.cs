using SharedInterfaces.Interfaces.ServiceFarm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMessageInterfaces.Interfaces.ChatMessage
{
    public interface IRequestChatMessagesInDateRangeBatchJob : IChatMessageService
    {
        /*
         * NOTE: Supports
         * 
         * Requesting large lists of ChatMessages that are for a given Channel and between a given date range (inclusive).
         * The request will be forwarded to a batch service where a job will be created. These are for requests that take a long time to process.
         * After the request is made this service will return a recipt that the client can use with another service to get their data.
         * 
         */
    }
}
