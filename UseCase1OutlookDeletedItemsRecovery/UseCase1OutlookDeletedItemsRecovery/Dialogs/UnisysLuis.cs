using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;
using System.Globalization;

namespace UseCase1OutlookDeletedItemsRecovery.Dialogs
{
    [Serializable]
    [LuisModel("557d9750-9ac1-461f-9ea1-f2f37a8e3efb", "3923319f049841febd0189f5a0ed580b")]
    public class UnisysLuis : LuisDialog<object>
    {
        string strCurrentURL = System.Configuration.ConfigurationManager.AppSettings["Bot_Publish_Url"];
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I am not getting you...");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var exMsg = "";
            try
            {
                var act = (Activity)await activity;
                string strGreeting = "";

                //DateTime dt = (DateTime)act.Timestamp;
                //DateTimeOffset dto = (DateTimeOffset)dt.ToLocalTime();

                //if (dto.Hour >= 12 && dto.Hour < 17)
                //    strGreeting = "Good Afternoon!";
                //else if (dto.Hour >= 17)
                //    strGreeting = "Good Evening!";
                //else if (dto.Hour < 12)
                //    strGreeting = "Good Morning!";
                strGreeting = "Greetings!";

                await context.PostAsync(strGreeting + " My name is Lee. Iam a Virtual Agent. How may i help you today?");
            }
            catch (Exception ex)
            {
                exMsg = ex.InnerException.ToString();
                await context.PostAsync(exMsg);
            }
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Issue")]
        [LuisIntent("Help")]
        public async Task Issue(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"More than happy to assist you. Kindly, elaborate in detail.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Outlook")]
        [LuisIntent("Option")]
        public async Task OutlookOption(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Please launch Outlook and click Folder > Recover Deleted Items");
            var resultMessage = context.MakeMessage();
            resultMessage.Text = $"I'm attaching an image to find the tabs";
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();

            Attachment attImage1 = new Attachment()
            {
                ContentType = "image/png",
                ContentUrl = String.Format(@"{0}/{1}", strCurrentURL, "/images/1.png")
            };

            resultMessage.Attachments.Add(attImage1);
            await context.PostAsync(resultMessage);

            resultMessage.Text = $"Click the message you want to recover, and then click Recover Selected Items like below.";
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();

            Attachment attImage2 = new Attachment()
            {
                ContentType = "image/png",
                ContentUrl = String.Format(@"{0}/{1}", strCurrentURL, "/images/2.png")
            };

            resultMessage.Attachments.Add(attImage2);
            await context.PostAsync(resultMessage);

            resultMessage.Text = $"To select multiple items, press Ctrl as you click each item, and then click Recover Selected Items as shown below";
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();

            Attachment attImage3 = new Attachment()
            {
                ContentType = "image/png",
                ContentUrl = String.Format(@"{0}/{1}", strCurrentURL, "/images/3.png")
            };

            resultMessage.Attachments.Add(attImage3);
            await context.PostAsync(resultMessage);

            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "Yes", "No" }, "Did my recommendation solve your issue?", "Not a valid option", 3);
        }

        [LuisIntent("HybridIntent")]
        public async Task HybridIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Please give details about one issue at a time.");

        }

        [LuisIntent("Okay")]
        public async Task Okay(IDialogContext context, LuisResult result)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "Yes", "No" }, "Did my recommendation solve your issue?", "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {

            string optionSelected = await result;

            switch (optionSelected)
            {
                case "Yes":
                    PromptDialog.Choice(context, this.OnOptionSelected2, new List<string>() { "Yes", "No" }, "Is there anything else i can do for you today?", "Not a valid option", 3);
                    break;

                case "No":
                    var resultMessage = context.MakeMessage();
                    resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resultMessage.Attachments = new List<Attachment>();

                    HeroCard heroCard = new HeroCard()
                    {
                        Subtitle = $"I am still learning. I will connect you with a live agent who can assist you further.",
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "Connect Now",
                                Type = ActionTypes.OpenUrl,
                                Value = $"http://www.unisys.com/about-us/support/unisys-support-services"
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                    await context.PostAsync(resultMessage);
                    break;
            }
        }

        private async Task OnOptionSelected2(IDialogContext context, IAwaitable<string> result)
        {

            string optionSelected = await result;

            switch (optionSelected)
            {
                case "Yes":
                    await context.PostAsync($"Please elaborate your issue.");
                    break;

                case "No":
                    await context.PostAsync($"It was a pleasure meeting you. Have a good day!");
                    break;
            }
            
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Have a good day!");
            context.Wait(this.MessageReceived);
        }

    }
}