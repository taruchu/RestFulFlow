using DataPersistence.Interfaces.Configuration;
using DataPersistence.Services.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedInterfaces.Interfaces.Envelope;
using SharedInterfaces.Models.Envelope;

namespace DataPersistence.Services.SQL
{
    public class DbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SQLDataBaseBoardChatMessage>  
    {
        public SQLDataBaseBoardChatMessage CreateDbContext(string[] args)
        {
            SQLDBConfigurationProvider configurationProvider = new SQLDBConfigurationProvider();
            ChatMessageEnvelopeFactory chatMessageEnvelopeFactory = new ChatMessageEnvelopeFactory();
            return new SQLDataBaseBoardChatMessage(configurationProvider, chatMessageEnvelopeFactory);
        }
    }
}
