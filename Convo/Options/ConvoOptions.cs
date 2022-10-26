using System;
using System.Collections.Generic;

namespace Convo.Options
{
    public class ConvoOptions : List<List<ConvoOption>>
    {
        public ConvoOptions()
        {
        }

        public ConvoOptions(List<List<ConvoOption>> options) : base(options) { }

        public ConvoOptions(IEnumerable<List<ConvoOption>> options) : base(options) { }

        public ConvoOptions(ConvoOptions options) : base(options) { }

        public void Add(ConvoOption option)
        {
            base.Add(option);
        }
        public void AddRange(IEnumerable<ConvoOption> collection)
        {
            foreach (ConvoOption option in collection)
            {
                Add(option);
            }
        }

    }
}
