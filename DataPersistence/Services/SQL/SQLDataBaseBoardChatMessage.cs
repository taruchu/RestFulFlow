using DataPersistence.Interfaces.Configuration;
using DataPersistence.Interfaces.SQL;
using DataPersistence.Models.ChatMessage;
using Microsoft.EntityFrameworkCore;
using SharedInterfaces.Interfaces.Envelope;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataPersistence.Services.SQL
{
    public class SQLDataBaseBoardChatMessage : DbContext, ISQLDataBaseBoardChatMessage, IChatMessageQueryRepository
    {
        private ISQLDBConfigurationProvider _sQLDBConfigurationProvider { get; set; }
        private IChatMessageEnvelopeFactory _chatMessageEnvelopeFactory { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Channel> Channels { get; set; }


        public SQLDataBaseBoardChatMessage(ISQLDBConfigurationProvider sQLDBConfigurationProvider, IChatMessageEnvelopeFactory chatMessageEnvelopeFactory)
            : base()
        {
            _sQLDBConfigurationProvider = sQLDBConfigurationProvider;
            _chatMessageEnvelopeFactory = chatMessageEnvelopeFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_sQLDBConfigurationProvider.GetSQLDBConfigurationFromJSONFile().ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //NOTE: Use this to override EF Core Conventions for defining relationships. 
            //It's an alternative to using data annotations on the model classes. (see .net Fluent API configuration)

        }

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public IChatMessageEnvelope MaptoEnvelope(ChatMessage chatMessage)
        {
            try
            {
                IChatMessageEnvelope chatMessageEnvelope = _chatMessageEnvelopeFactory.InstantiateIEnvelope();
                chatMessageEnvelope.ChatChannelID = chatMessage.ChannelID;
                chatMessageEnvelope.ChatChannelName = chatMessage.Channel.ChannelName;
                chatMessageEnvelope.ChatMessageID = chatMessage.ChatMessageID;
                chatMessageEnvelope.ChatMessageBody = chatMessage.ChatMessageBody;
                chatMessageEnvelope.CreatedDateTime = chatMessage.CreatedDateTime;
                chatMessageEnvelope.ModifiedDateTime = chatMessage.ModifiedDateTime;
                return chatMessageEnvelope;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public ChatMessage MapToChatMessage(IChatMessageEnvelope chatMessageEnvelope)
        {
            try
            {
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.Channel = MapToChannel(chatMessageEnvelope);
                chatMessage.ChannelID = chatMessage.Channel.ChannelID;
                chatMessage.ChatMessageID = chatMessageEnvelope.ChatMessageID;
                chatMessage.ChatMessageBody = chatMessageEnvelope.ChatMessageBody;
                return chatMessage;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public Channel MapToChannel(IChatMessageEnvelope chatMessageEnvelope)
        {
            try
            {
                Channel channel = new Channel();
                if (chatMessageEnvelope.ChatChannelID > 0)
                    channel = this.Channels.Find(chatMessageEnvelope.ChatChannelID);
                else if (String.IsNullOrEmpty(chatMessageEnvelope.ChatChannelName) == false)
                    channel = this.Channels.FirstOrDefault(chnl => chnl.ChannelName == chatMessageEnvelope.ChatChannelName);
                else
                    channel = null;
                return channel;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public IChatMessageEnvelope SetErrorMessageForInvalidChatChannel(IChatMessageEnvelope invalidEnvelope)
        {
            invalidEnvelope.ErrorMessage += String.Format("The chat channel name {0} that you provided does not exists. ", invalidEnvelope.ChatChannelName);
            invalidEnvelope.ErrorMessage += String.Format("The chat channel ID {0} that you provided does not exists. ", invalidEnvelope.ChatChannelID);
            return invalidEnvelope;
        }
        public IEnvelope DELETE(IEnvelope envelope)
        {
            using (var dbContextTransaction = this.Database.BeginTransaction())
            {
                try
                {
                    ChatMessage chatMessageInput = MapToChatMessage((IChatMessageEnvelope)envelope);
                    if (chatMessageInput.Channel == null)
                    {
                        return SetErrorMessageForInvalidChatChannel((IChatMessageEnvelope)envelope);
                    }

                    ChatMessage deletedChatMessage = this.ChatMessages.Find(chatMessageInput.ChatMessageID);
                    deletedChatMessage.ModifiedDateTime = DateTime.Now;
                    this.ChatMessages.Remove(deletedChatMessage);
                    this.SaveChanges();
                    dbContextTransaction.Commit();
                    IChatMessageEnvelope recipt = MaptoEnvelope(deletedChatMessage);
                    return recipt;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public IEnvelope GET(IEnvelope envelope)
        {
            using (var dbContextTransaction = this.Database.BeginTransaction())
            {
                try
                {
                    IChatMessageEnvelope chatMessageEnvelope = (IChatMessageEnvelope)envelope;
                    if (chatMessageEnvelope.Query == null)
                        return envelope;

                    IChatMessageEnvelope recipt = chatMessageEnvelope.Query(this, chatMessageEnvelope);//NOTE: Avoid large list of if/else statements, let client choose the query
                    dbContextTransaction.Commit();
                    return recipt;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public List<IEnvelope> GETList(IEnvelope envelope)
        {
            using (var dbContextTransaction = this.Database.BeginTransaction())
            {
                try
                {
                    IChatMessageEnvelope chatMessageEnvelope = (IChatMessageEnvelope)envelope;
                    if (chatMessageEnvelope.QueryForList == null)
                        return new List<IEnvelope> { envelope };

                    List<IChatMessageEnvelope> results = chatMessageEnvelope.QueryForList(this, chatMessageEnvelope);//NOTE: Avoid large list of if/else statements, let client choose the query
                    dbContextTransaction.Commit();
                    return results.ToList<IEnvelope>();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public IEnvelope POST(IEnvelope envelope)
        {
            using (var dbContextTransaction = this.Database.BeginTransaction())
            {
                try
                {
                    ChatMessage chatMessageInput = MapToChatMessage((IChatMessageEnvelope)envelope);
                    if (chatMessageInput.Channel == null)
                    {
                        return SetErrorMessageForInvalidChatChannel((IChatMessageEnvelope)envelope);
                    }

                    chatMessageInput.CreatedDateTime = DateTime.Now;
                    this.ChatMessages.Add(chatMessageInput);
                    this.SaveChanges();
                    dbContextTransaction.Commit();
                    IChatMessageEnvelope recipt = MaptoEnvelope(chatMessageInput);
                    return recipt;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public IEnvelope PUT(IEnvelope envelope)
        {
            using (var dbContextTransaction = this.Database.BeginTransaction())
            {
                try
                {
                    ChatMessage chatMessageInput = MapToChatMessage((IChatMessageEnvelope)envelope);
                    if (chatMessageInput.Channel == null)
                    {
                        return SetErrorMessageForInvalidChatChannel((IChatMessageEnvelope)envelope);
                    }

                    chatMessageInput.ModifiedDateTime = DateTime.Now;
                    ChatMessage updatedChatMessage = this.ChatMessages.Find(chatMessageInput.ChatMessageID);
                    //NOTE: Ensure the CreatedDateTime isn't modified by the client.
                    chatMessageInput.CreatedDateTime = updatedChatMessage.CreatedDateTime;
                    this.Entry(updatedChatMessage).CurrentValues.SetValues(chatMessageInput);
                    this.SaveChanges();
                    dbContextTransaction.Commit();
                    IChatMessageEnvelope recipt = MaptoEnvelope(updatedChatMessage);
                    return recipt;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new ApplicationException(ex.Message, ex);
                }
            }
        }

        public IChatMessageEnvelope GetNextChatMessage(IChatMessageEnvelope chatMessageEnvelope)
        {
            try
            {
                ChatMessage chatMessageInput = this.ChatMessages.FirstOrDefault(msg => msg.ChatMessageID == chatMessageEnvelope.ChatMessageID);
                ChatMessage nextChatMessage = this.ChatMessages
                                                    .Where(
                                                        chat =>
                                                            chat.ChannelID == chatMessageInput.ChannelID
                                                            &&
                                                            chat.CreatedDateTime > chatMessageInput.CreatedDateTime
                                                    )
                                                    .OrderBy(chat => chat.CreatedDateTime)
                                                    .FirstOrDefault();

                if (nextChatMessage == null)
                {
                    return chatMessageEnvelope;//NOTE: They already have the most recent chat message.
                }
                else
                {
                    this.Entry(nextChatMessage).Reference(nxt => nxt.Channel).Load();
                    IChatMessageEnvelope nextChatMessageEnvelope = MaptoEnvelope(nextChatMessage);
                    return nextChatMessageEnvelope;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public IChatMessageEnvelope GetChatMessageByID(IChatMessageEnvelope chatMessageEnvelope)
        {
            try
            {
                ChatMessage chatMessage = this.ChatMessages.FirstOrDefault(msg => msg.ChatMessageID == chatMessageEnvelope.ChatMessageID);
                this.Entry(chatMessage).Reference(chnl => chnl.Channel).Load();
                return MaptoEnvelope(chatMessage);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public IChatMessageEnvelope GetLatestChatMessage(IChatMessageEnvelope chatMessageEnvelope)
        {
            try
            {
                ChatMessage chatMessageInput = MapToChatMessage(chatMessageEnvelope);
                ChatMessage latestChatMessage = this.ChatMessages
                                                    .Where(
                                                        chat =>
                                                            chat.ChannelID == chatMessageInput.ChannelID
                                                    )
                                                    .OrderBy(chat => chat.CreatedDateTime)
                                                    .FirstOrDefault();

                this.Entry(latestChatMessage).Reference(nxt => nxt.Channel).Load();
                IChatMessageEnvelope nextChatMessageEnvelope = MaptoEnvelope(latestChatMessage);
                return nextChatMessageEnvelope;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
