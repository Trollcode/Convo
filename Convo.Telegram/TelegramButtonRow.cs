using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Convo.Telegram
{
    public class TelegramButtonRow : IEnumerable<TelegramButton>
    {
        public TelegramButtonRow(IEnumerable<TelegramButton> buttons)
        {
            Buttons = buttons.ToList();
        }

        public TelegramButtonRow()
        {
            Buttons = new List<TelegramButton>();
        }

        public void Add(TelegramButton button)
        {
            Buttons.Add(button);
        }

        public IEnumerator<TelegramButton> GetEnumerator()
        {
            return Buttons.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<TelegramButton> Buttons { get; private set; }

        public static implicit operator List<TelegramButton>(TelegramButtonRow buttonRow)
        {
            return buttonRow.Buttons;
        }
    }
}
