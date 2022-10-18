using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Buttons
{
    public class ConvoButtonRow : IEnumerable<ConvoButton>
    {
        public ConvoButtonRow(IEnumerable<ConvoButton> buttons)
        {
            Buttons = buttons.ToList();
        }

        public ConvoButtonRow()
        {
            Buttons = new List<ConvoButton>();
        }

        public void Add(ConvoButton button)
        {
            Buttons.Add(button);
        }

        public IEnumerator<ConvoButton> GetEnumerator()
        {
            return Buttons.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<ConvoButton> Buttons { get; private set; }

        public static implicit operator List<ConvoButton>(ConvoButtonRow buttonRow)
        {
            return buttonRow.Buttons;
        }
    }
}
