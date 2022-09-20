using System;

namespace Convo.Abstractions
{
    public class ConvoContext
    {
        public Guid Id { get; set; }
        public Protocol Protocol { get; set; }
        public string ProtocolAlias { get; set; }
        public string ProtocolName { get; set; }
        public string ProtocolUserIdentifaction { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool ExpectingReply { get; set; }
        public string? ExpectingReplyActionId { get; set; }
        public string? RedirectActionId { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }

        public static ConvoContext FromMessage(ConvoMessage msg)
        {
            return new ConvoContext
            {
                ProtocolUserIdentifaction = msg.ConversationId,
                ProtocolAlias = msg.Alias,
                ProtocolName = msg.Name,
                IsAuthenticated = false,
                ExpectingReply = false,
                ExpectingReplyActionId = null,
                RedirectActionId = null,
                Protocol = msg.Protocol
            };
        }
    }
}