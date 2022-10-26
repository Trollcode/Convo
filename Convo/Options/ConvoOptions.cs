using System.Collections.Generic;

namespace Convo.Options
{
    public class ConvoOptions : List<List<ConvoOption>>
    {
        public ConvoOptions()
        {
        }

        public ConvoOptions(List<List<ConvoOption>> items) : base(items) { }

        public ConvoOptions(IEnumerable<List<ConvoOption>> items) : base(items) { }

        public ConvoOptions(ConvoOptions items) : base(items) { }

        public void Add(ConvoOption item)
        {
            Add(item);
        }
        public void AddRange(IEnumerable<ConvoOption> collection)
        {
            AddRange(collection);
        }

    }
}
