using System.Collections.Generic;

namespace Convo.Options
{
    public class ConvoOption
    {
        public ConvoOption(string text, string command)
        {
            Text = text;
            Command = command;
        }
        public ConvoOption()
        {
        }

        public string Text { get; set; }
        public string Command { get; set; }


        public static implicit operator List<ConvoOption>(ConvoOption option)
        {
            return new List<ConvoOption> { option };
        }
    }
}
