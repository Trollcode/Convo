using System;
using System.Collections.Generic;


namespace Convo.Buttons
{
    public class ButtonResponse : ConvoResponse
    {
        public ButtonResponse() { }

        public ButtonResponse(string text) : base(text) { }

        public ConvoButtons ReplyButtons { get; set; } = new ConvoButtons();
    }
}
