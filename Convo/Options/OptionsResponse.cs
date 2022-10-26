
namespace Convo.Options
{
    public class OptionsResponse : ConvoResponse
    {
        public OptionsResponse() { }

        public OptionsResponse(string text) : base(text) { }

        public ConvoOptions ReplyOptions { get; set; } = new ConvoOptions();
    }
}
