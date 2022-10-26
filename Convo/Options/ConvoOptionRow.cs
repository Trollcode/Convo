using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Convo.Options
{
    public class ConvoOptionRow : IEnumerable<ConvoOption>
    {
        public ConvoOptionRow(IEnumerable<ConvoOption> buttons)
        {
            Buttons = buttons.ToList();
        }

        public ConvoOptionRow()
        {
            Buttons = new List<ConvoOption>();
        }

        public void Add(ConvoOption button)
        {
            Buttons.Add(button);
        }

        public IEnumerator<ConvoOption> GetEnumerator()
        {
            return Buttons.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<ConvoOption> Buttons { get; private set; }

        public static implicit operator List<ConvoOption>(ConvoOptionRow buttonRow)
        {
            return buttonRow.Buttons;
        }
    }
}
