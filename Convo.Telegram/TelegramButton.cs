using System;
using System.Collections.Generic;
using System.Text;

namespace Convo.Telegram
{
    public class TelegramButton
    {
        public TelegramButton(string text, string command)
        {
            Text = text;
            Command = command;
        }
        public TelegramButton()
        {
        }

        public string Text { get; set; }
        public string Command { get; set; }


        public static implicit operator List<TelegramButton>(TelegramButton button)
        {
            return new List<TelegramButton> { button };
        }
    }
}
