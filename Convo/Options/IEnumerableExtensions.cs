using System.Collections.Generic;
using System.Linq;

namespace Convo.Options
{
    public static class IEnumerableExtensions
    {
        public static ConvoOptions ToConvoOptions(this IEnumerable<ConvoOption> data)
        {
            return new ConvoOptions(data.Select(x => new List<ConvoOption>
            {
                new ConvoOption
                {
                    Text = x.Text,
                    Command = x.Command
                }
            }).ToList());
        }
    }
}

