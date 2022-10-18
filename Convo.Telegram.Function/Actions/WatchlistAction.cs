using Convo.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convo.Telegram.Example.Actions
{
    public class Watchlist : ConvoCommand
    {
        public Watchlist()
        {
            Id = "WatchlistAction";
            RequireAuthentication = true;
            Command = "watchlist";
            Description = "Manage your watchlists";
        }

        public override Task<ConvoResponse?> HandleCommand(IConvoContext context, ConvoMessage command)
        {
            ConvoResponse? response = null;

            if (command.Arguments.Any())
            {
                if (command.Arguments.Length >= 1)
                {
                    switch (command.Arguments[0].ToLowerInvariant())
                    {
                        case "add":
                            {
                                if (command.Arguments.Length > 1)
                                {
                                    context.Data.Remove("watchlist-addview");

                                    AddWatchedAsset(context, command.Arguments[1]);
                                    response = DisplayMainMenu(updateMessageId: command.MessageId);
                                }
                                else
                                {
                                    context.ExpectingReplyActionId = Id;

                                    context.Data.AddOrUpdate("watchlist-addview", command.MessageId);

                                    response = new ButtonResponse
                                    {
                                        Text = "Please respond with a ticker you would like to add to the watchlist",
                                        UpdateMessageId = command.MessageId,
                                        ReplyButtons = new ConvoButtons
                                        {
                                            new ConvoButton
                                            {
                                                Text = "<< Back to menu",
                                                Command = "/watchlist",
                                            }
                                        }
                                    };
                                }
                            }
                            break;
                        case "remove":
                            {
                                if (command.Arguments.Length > 1)
                                {
                                    RemoveWatchedAsset(context, command.Arguments[1]);

                                    response = DisplayMainMenu(updateMessageId: command.MessageId);
                                }
                                else
                                {
                                    ConvoButtons assets = GetWatchedAssets(context)
                                        .Select(x => new ConvoButton
                                        {
                                            Text = x.ToUpperInvariant(),
                                            Command = $"/watchlist remove {x}"
                                        }).ToConvoButtons();

                                    response = new ButtonResponse
                                    {
                                        Text = "Please select the ticker you would like to remove from your watchlist",
                                        UpdateMessageId = command.MessageId,
                                        ReplyButtons = new ConvoButtons(assets)
                                        {
                                            new ConvoButton
                                            {
                                                Text = "<< Back to menu",
                                                Command = "/watchlist",
                                            }
                                        }
                                    };
                                }
                            }
                            break;
                        case "exit":
                            response = new ConvoResponse
                            {
                                DeleteMessageId = command.MessageId,
                            };
                            break;
                        case "view":
                            response = ManageWatchlistSubscriptions(context, command.Arguments);
                            response.UpdateMessageId = command.MessageId;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    response = DisplayMainMenu(updateMessageId: command.MessageId);
                }
            }
            else
            {
                response = DisplayMainMenu(deleteMessageId: command.MessageId);
            }

            return Task.FromResult(response);
        }

        public override Task<ConvoResponse?> HandleReply(IConvoContext context, ConvoMessage reply)
        {
            if (context.Data.ContainsKey("watchlist-addview"))
            {
                IEnumerable<string> assets = new List<string>
                {
                    "NSKOG", "GSF", "MOWI", "SRBANK"
                }.Where(x => x.Contains(reply.Text.ToUpperInvariant()));

                ConvoButtons result = assets.Select(x => new ConvoButton
                {
                    Text = x.ToUpperInvariant(),
                    Command = $"/watchlist add {x}"
                }).ToConvoButtons();

                return Task.FromResult<ConvoResponse?>(new ButtonResponse
                {
                    Text = result.Any() ? "Found the following assets fitting the query" : $"Could not find any asset fitting the query {reply.Text}",
                    UpdateMessageId = context.Data["watchlist-addview"] as string,
                    DeleteMessageId = reply.MessageId,
                    ReplyButtons = new ConvoButtons(result)
                    {
                        new ConvoButton
                        {
                            Text = "<< Back to menu",
                            Command = "/watchlist",
                        }
                    }
                });
            }
            else
            {
                return Task.FromResult<ConvoResponse?>(DisplayMainMenu(deleteMessageId: reply.MessageId));
            }
        }

        private ButtonResponse DisplayMainMenu(string? deleteMessageId = null, string? updateMessageId = null)
        {
            return new ButtonResponse
            {
                DeleteMessageId = deleteMessageId,
                UpdateMessageId = updateMessageId,
                Text = "Please choose a option",
                ReplyButtons = new ConvoButtons
                {
                    new ConvoButtonRow
                    {
                        new ConvoButton
                        {
                            Text = "Add",
                            Command = "/watchlist add",
                        },
                        new ConvoButton
                        {
                            Text = "Remove",
                            Command = "/watchlist remove"
                        }
                    },
                    new ConvoButton
                    {
                        Text = "View / Manage",
                        Command = "/watchlist view",
                    },
                    new ConvoButton
                    {
                        Text = "Exit",
                        Command = "/watchlist exit",
                    }
                }
            };
        }

        private List<string> GetWatchedAssets(IConvoContext context)
        {
            List<string> items = new List<string>();
            if (context.Data.ContainsKey("watchlist-assets"))
            {
                //Type myType = context.Data["watchlist-assets"].GetType();
                if (context.Data["watchlist-assets"] is Newtonsoft.Json.Linq.JArray assets)
                {
                    items = assets.Values<string>().ToList();
                }
            }
            return items;
        }

        private void RemoveWatchedAsset(IConvoContext context, string asset)
        {
            List<string> items = GetWatchedAssets(context);
            items.Remove(asset);
            context.Data.AddOrUpdate("watchlist-assets", items);
        }

        private void AddWatchedAsset(IConvoContext context, string asset)
        {
            List<string> items = GetWatchedAssets(context);
            if (!items.Contains(asset))
            {
                items.Add(asset);
            }
            context.Data.AddOrUpdate("watchlist-assets", items);
        }

        private ButtonResponse ManageWatchlistSubscriptions(IConvoContext context, string[] arguments)
        {
            List<string> items = GetWatchedAssets(context);

            ButtonResponse resp = new ButtonResponse("You have the following assets on your watchlist:");

            if (items.Any())
            {
                foreach (string asset in items)
                {
                    resp.ReplyButtons.Add(new ConvoButton
                    {
                        Text = asset.ToUpperInvariant(),
                        Command = $"/watchlist view {asset}",
                    });
                }
            }
            else
            {
                resp.Text = "You have no items on your watchlist. Please add some assets to watch.";
            }

            resp.ReplyButtons.Add(new ConvoButton
            {
                Text = "<< Back to menu",
                Command = "/watchlist",
            });

            return resp;
        }
    }
}
