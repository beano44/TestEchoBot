// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Connector;

using Npgsql;
using System.Net.Http;
using System.Net;

namespace TestEchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        public static string NEWconnString = "Host=chatbot.inteleqt.co.za;Username=postgres;Password=Inteleqt$$135;Database=BlueLabelCIC;Port=5432";



        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string message = turnContext.Activity.Text.ToLower();



            if (message.Equals("hello"))
            {

                await turnContext.SendActivityAsync(CreateActivityWithTextAndSpeak("Hello How are you?"), cancellationToken);

            }

            else if (message.Equals("test"))
            {

                Int32 usercount = 0;

                using (var conn = new NpgsqlConnection(NEWconnString))
                {

                    conn.Open();

                    using (var cmd = new NpgsqlCommand("SELECT count(DISTINCT \"menu1\") FROM public.\"DecisionTreeMapping\";"
                                                        , conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            usercount = reader.GetInt32(0);
                        }

                    await turnContext.SendActivityAsync(CreateActivityWithTextAndSpeak("Demo number = " + usercount.ToString()), cancellationToken);

                   await conn.CloseAsync();
                }

                

                //await turnContext.SendActivityAsync(MessageFactory.SuggestedActions("string1","null")), cancellationToken);


            }

            else if (message.Equals("file"))
            {

                try
                {
                    await turnContext.SendActivityAsync(MessageFactory.Attachment(GetInlineAttachment()),cancellationToken);
                    //WebClient client = new WebClient();
                    //client.Proxy = WebRequest.DefaultWebProxy;
                    //client.Credentials = new NetworkCredential("inteleqt-admin", "P@ssword$$01");
                    //client.DownloadFile(@"\\\np.inteleqt.co.za\Reports\Cargo\Cargo Chatbot - Hourly.pdf", @"D:\Desktop\Blue Label Content\Final\Documents\Chatbot.PDF");

                }
                catch (Exception e)
                {

                    string exception = e.ToString();

                    await turnContext.SendActivityAsync(CreateActivityWithTextAndSpeak(exception), cancellationToken);

                }




            }

            else if (message.Equals("ask"))
            {
                var reply = MessageFactory.Text("What is your favorite color?");
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Red", Type = ActionTypes.ImBack, Value = "#test" },
                    new CardAction() { Title = "Yellow", Type = ActionTypes.ImBack, Value = "Yellow" },
                    new CardAction() { Title = "Blue", Type = ActionTypes.ImBack, Value = "Blue" },
                },
                };
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }

            else if (message.Equals("#test"))
            {

                await turnContext.SendActivityAsync(CreateActivityWithTextAndSpeak("Received the test"), cancellationToken);


            }

            else
            {

                await turnContext.SendActivityAsync(CreateActivityWithTextAndSpeak($"You said:\"{turnContext.Activity.Text}\""), cancellationToken);

            }
                       
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(CreateActivityWithTextAndSpeak($"Hello and welcome!"), cancellationToken);
                }
            }
        }

        private static Attachment GetInlineAttachment()
        {
            
            var imagePath = @"\\np.inteleqt.co.za\Reports\Cargo\Cargo Chatbot - Hourly.pdf";
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = @"AccountsQueries.pdf",
                ContentType = "Application/pdf",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };
        }

        private IActivity CreateActivityWithTextAndSpeak(string message)
        {
            var activity = MessageFactory.Text(message);
            string speak = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
              <voice name='Microsoft Server Speech Text to Speech Voice (en-US, JessaRUS)'>" +
              $"{message}" + "</voice></speak>";
            activity.Speak = speak;
            return activity;
        }
    }
}
