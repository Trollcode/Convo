using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Buttons
{
    public class ConvoButton
    {
        public ConvoButton(string text, string command)
        {
            Text = text;
            Command = command;
        }
        public ConvoButton()
        {
        }

        public string Text { get; set; }
        public string Command { get; set; }


        public static implicit operator List<ConvoButton>(ConvoButton button)
        {
            return new List<ConvoButton> { button };
        }
    }
}
