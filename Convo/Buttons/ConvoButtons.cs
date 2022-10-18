using System.Collections.Generic;

namespace Convo.Buttons
{
    public class ConvoButtons : List<List<ConvoButton>>
    {
        public ConvoButtons()
        {
        }

        public ConvoButtons(List<List<ConvoButton>> items) : base(items) { }

        public ConvoButtons(IEnumerable<List<ConvoButton>> items) : base(items) { }

        public ConvoButtons(ConvoButtons items) : base(items) { }

        public void Add(ConvoButton item)
        {
            Add(item);
        }
        public void AddRange(IEnumerable<ConvoButton> collection)
        {
            AddRange(collection);
        }

    }
}
