using Convo.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Telegram.Example.Actions
{
    public class WatchlistAction : ConvoAction
    {
        public WatchlistAction()
        {
            Id = "WatchlistAction";
            RequireAuthentication = true;
            Command = "watchlist";
            Description = "Manage your watchlists";
        }

        public override Task<ConvoResponse?> HandleCommand(ConvoContext context, Dictionary<string, string> data, ConvoMessage command)
        {
            if(command.Arguments.Any())
            {
                return Task.FromResult<ConvoResponse?>(new ConvoResponse
                {
                    Text = $"Watchlist {command.Text}"
                });
            }
            else
            {
                return Task.FromResult<ConvoResponse?>(DisplayMainMenu(deleteMessageId: command.MessageId));
            }
        }

        public override Task<ConvoResponse?> HandleReply(ConvoContext context, Dictionary<string, string> data, ConvoMessage reply)
        {
            return Task.FromResult<ConvoResponse?>(new ConvoResponse
            {
                Text = ""
            });
        }

        private ButtonResponse DisplayMainMenu(string deleteMessageId = null, string updateMessageId = null)
        {
            return new ButtonResponse
            {
                DeleteMessageId = deleteMessageId,
                UpdateMessageId = updateMessageId,
                Text = "Please choose a option",
                ReplyButtons = new List<List<TelegramButton>>
                {
                    new TelegramButtonRow
                    {
                        new TelegramButton
                        {
                            Text = "Add",
                            Command = "/watchlist add",
                        },
                        new TelegramButton
                        {
                            Text = "Remove",
                            Command = "/watchlist remove"
                        }
                    },
                    new TelegramButton
                    {
                        Text = "View / Manage",
                        Command = "/watchlist view",
                    },
                    new TelegramButton
                    {
                        Text = "Exit",
                        Command = "/watchlist exit",
                    }
                }
            };
        }
    }
}
