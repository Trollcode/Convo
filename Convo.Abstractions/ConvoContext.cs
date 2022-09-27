using System;

namespace Convo.Abstractions
{
    public class ConvoContext
    {
        public string Id { get; set; }
        public Protocol Protocol { get; set; }
        public string ProtocolAlias { get; set; }
        public string ProtocolName { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool ExpectingReply { get; set; }
        public string? ExpectingReplyActionId { get; set; }
        //public string? RedirectActionId { get; set; }
        //public string? RedirectActionMessage { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }

        public static void ResetFromMessage(ConvoContext ctx, ConvoMessage msg)
        {
            ctx.Id = msg.ConversationId;
            ctx.ProtocolAlias = msg.Alias;
            ctx.ProtocolName = msg.Name;
            ctx.IsAuthenticated = false;
            ctx.ExpectingReply = false;
            ctx.ExpectingReplyActionId = null;
            //ctx.RedirectActionId = null;
            //ctx.RedirectActionMessage = null;
            ctx.Protocol = msg.Protocol;
            ctx.Created = DateTime.UtcNow;
            ctx.Updated = DateTime.UtcNow;
        }

        public static ConvoContext FromMessage(ConvoMessage msg)
        {
            return new ConvoContext
            {
                Id = msg.ConversationId,
                ProtocolAlias = msg.Alias,
                ProtocolName = msg.Name,
                IsAuthenticated = false,
                ExpectingReply = false,
                ExpectingReplyActionId = null,
                //RedirectActionId = null,
                //RedirectActionMessage = null,
                Protocol = msg.Protocol,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
        }
    }
}