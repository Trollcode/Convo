using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Convo.Options
{
    public class ConvoOptionRow : IEnumerable<ConvoOption>
    {
        public ConvoOptionRow(IEnumerable<ConvoOption> options)
        {
            Options = options.ToList();
        }

        public ConvoOptionRow()
        {
            Options = new List<ConvoOption>();
        }

        public void Add(ConvoOption option)
        {
            Options.Add(option);
        }

        public IEnumerator<ConvoOption> GetEnumerator()
        {
            return Options.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<ConvoOption> Options { get; private set; }

        public static implicit operator List<ConvoOption>(ConvoOptionRow optionRow)
        {
            return optionRow.Options;
        }
    }
}
