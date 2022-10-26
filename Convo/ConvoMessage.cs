using System.Linq;

namespace Convo
{
    public class ConvoMessage
    {
        public string MessageId { get; set; }
        public string? Alias { get; set; }
        public string? Name { get; set; }
        public string Text { get; set; }

        public string Command
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    return Text.Split(' ')[0].Replace("/", "");
                }
                return "";
            }
        }
        public string[] Arguments
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    string[] args = Text.Split(' ');
                    if (args.Length > 1)
                    {
                        return args.Skip(1).ToArray();
                    }
                }
                return new string[] { };
            }
        }
    }
}