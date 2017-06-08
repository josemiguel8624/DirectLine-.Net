using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Bot.Connector.DirectLine.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace DirectLineTest.Controllers
{
    #region public class Chat
    public class Chat
    {
        public string ChatMessage { get; set; }
        public string ChatResponse { get; set; }
        public string watermark { get; set; }
    }
    #endregion

    public class HomeController : Controller
    {
        private static string DirectLineURL = @"https://directline.botframework.com";
        private static string DirectLineSecret = "c0816040-7957-43de-8391-17596a96fa27";
        private static string BotID = "agjqmfZvHTkjEbOCMprd4zD";

        public async Task<ActionResult> Index()
        {
            //create a new instance of chat object
            Chat objChat = new Chat();
            if(User.Identity.IsAuthenticated)
            {
                //pass the message to the bot and get response
                objChat = await TalkToTheBot("Hello");
            }
            else
            {
                objChat.ChatResponse = "Debe tener una sesión iniciada";
            }
            return View(objChat);
        }

        private async Task<Chat> TalkToTheBot(string paramMessage)
        {
            //Connect to the direct line service
            DirectLineClient client = new DirectLineClient(DirectLineSecret);
            //Try to get the existing conversation
            Conversation conversation = System.Web.HttpContext.Current.Session["conversation"] as Conversation;
            //Try to get an existing watermark
            //The watermark marks the last message we received
            string watermark = System.Web.HttpContext.Current.Session["watermark"] as string;
            if (conversation == null)
            {
                //Start a new conversation if doesn't exist
                conversation = client.Conversations.NewConversation();
            }
            //Use the text in parameters to crate a message
            Message message = new Message
            {
                FromProperty = User.Identity.Name,
                Text = paramMessage;
            };
            //Post the message into the bot
            await client.Conversations.PostMessageAsync(conversation.ConversationId, message);
            //Get the response as a chat object
            Chat objChat = await ReadBotMessagesAsync(client, conversation.ConversationId, watermark);
            //Save values
            System.Web.HttpContext.Current.Session["conversation"] = conversation;
            System.Web.HttpContext.Current.Session["watermark"] = objChat.watermark;
            //Return the response as a Chat object
            return objChat;
        }

        private async Task<Chat>ReadBotMessagesAsync(DirectLineClient client, string conversationId, string watermark)
        {
            //create an instance of the Chat object
            Chat objChat = new Chat();
            //keep till message is received
            bool messageReceived = false;
            while(!messageReceived)
            {
                //Get any meesages related
                var messages = await client.Conversations.GetMessagesAsync(conversationId, watermark);
            //set the watermark to the message received
            watermark = messages?.Watermark;
            //get all the messages
            var messagesFromBotText = from message in messages.Messages
                                      where message.FromProperty == botId
                                      select message;
            //Loop though each message
            foreach (Message message in messagesFromBotText)
            {
                //Having text
                if(message.Text != null)
                {
                    //set the text response
                    objChat.ChatResponse += " " + message.Text.Replace("\n\n", "<br />");
                }
            }
            //mark messageReceived and brak the loop
            messageReceived = true;
        }
        //set the watermark that will be returned
        objChat.watermark = watermark;
        //return a response as an object Chat
        return objChat;
    }
    }
}
