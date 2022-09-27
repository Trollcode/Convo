using Convo.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Convo.Telegram
{
    public class ButtonResponse : ConvoResponse
    {
        public List<List<TelegramButton>> ReplyButtons { get; set; } = new List<List<TelegramButton>>();
    }
}
