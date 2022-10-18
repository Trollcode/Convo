using System;
using System.Collections.Generic;

namespace Convo
{
    public interface IConvoContext
    {
        string ChatId { get; set; }
        string Alias { get; set; }
        string Name { get; set; }
        bool IsAuthenticated { get; set; }
        string? ExpectingReplyActionId { get; set; }
        Dictionary<string, object?> Data { get; set; }

        void Reset(ConvoMessage message);
    }

}
