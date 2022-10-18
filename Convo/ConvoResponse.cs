namespace Convo
{
    public class ConvoResponse
    {
        public ConvoResponse() { }

        public ConvoResponse(string text)
        {
            Text = text;
        }

        public string? DeleteMessageId { get; set; }
        public string? UpdateMessageId { get; set; }
        public string Text { get; set; }
    }
}