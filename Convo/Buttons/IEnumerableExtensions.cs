using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Convo.Buttons
{
    public static class IEnumerableExtensions
    {
        public static ConvoButtons ToConvoButtons(this IEnumerable<ConvoButton> data)
        {
            return new ConvoButtons(data.Select(x => new List<ConvoButton>
            {
                new ConvoButton
                {
                    Text = x.Text,
                    Command = x.Command
                }
            }).ToList());
        }
    }
}

