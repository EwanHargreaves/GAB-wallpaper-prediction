// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using System.Net;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using EchoBot2.Json_Class;
using System.Net.Mime;

namespace EchoBot2.Bots
{
    public class EchoBot : ActivityHandler
    {
        /// <summary>
        /// Sends a URL to Cognitive Services and performs analysis.
        /// </summary>
        /// <param name="imageUrl">The URL of the image to analyze.</param>
        /// <returns>Awaitable image analysis result.</returns>
        private async Task<string> AnalyzeUrlAsync(string imageUrl, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var test = new ApiKeyServiceClientCredentials("cb8628c62abf49c29fd7a129e52e0eb4");
            using (var client = new ComputerVisionClient(test) { Endpoint = "https://colourfind.cognitiveservices.azure.com/" })
            {
                VisualFeatureTypes[] visualFeatures = new List<VisualFeatureTypes>() { VisualFeatureTypes.Color, VisualFeatureTypes.Description, VisualFeatureTypes.Tags }.ToArray();

                string language = "en";
                ImageAnalysis analysisResult;

                analysisResult = await client.AnalyzeImageAsync(imageUrl, visualFeatures, null, language);
                string mainColour = analysisResult.Color.DominantColorForeground;

                return mainColour;
            }
        }

        public static async Task<string> MakePredictionRequest(string ImageUrl, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string url = "https://uksouth.api.cognitive.microsoft.com/customvision/v3.0/Prediction/1660c786-db2d-4626-b37b-37486b2fe7eb/classify/iterations/Iteration5/image";

            using (var client = new HttpClient())
            {
                // Request headers - replace this example key with your valid Prediction-Key.
                client.DefaultRequestHeaders.Add("Prediction-Key", "<PREDICTION KEY>");

                HttpResponseMessage response;
                byte[] byteData;
                // Request body. Try this sample with a locally stored image.
                using (var webClient = new WebClient())
                {
                    byteData = webClient.DownloadData(ImageUrl);
                }

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(url, content);
                    var responseStr = await response.Content.ReadAsStringAsync();
                    PredictionObject rootObject = JsonConvert.DeserializeObject<PredictionObject>(responseStr);

                    var mainTag = rootObject.predictions.First().tagName;
                    foreach (var prediction in rootObject.predictions)
                    {
                        string output = prediction.tagName + "-confidence: " + prediction.probability;
                        //await turnContext.SendActivityAsync(MessageFactory.Text(output), cancellationToken);
                    }
                    return mainTag;
                }
            }
        }

        public static async Task MakeQueryRequest(string mainColour, string mainTag, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string clientId = "<CLIENT ID>";
            string url = "https://www.grahambrown.com/s/GrahamBrownUK/dw/shop/v18_1/product_search?client_id={0}&q=";
            url = string.Format(url, clientId);
            string q = mainColour + "," + mainTag + "&expand=images";
            url = url + q;

            using (var client = new HttpClient())
            {
                HttpResponseMessage response;

                response = await client.GetAsync(url);
                var responseStr = await response.Content.ReadAsStringAsync();
                QueryObject queryObject = JsonConvert.DeserializeObject<QueryObject>(responseStr);

                List<Attachment> attachments = new List<Attachment>();
                foreach (var Query in queryObject.hits)
                {
                    CardAction tap = new CardAction(ActionTypes.OpenUrl, "see", null, null, null, Query.image.link);
                    HeroCard heroCard = new HeroCard(
                        Query.product_name, 
                        null, 
                        null, 
                        new CardImage[] { new CardImage(Query.image.link, null, null)},
                        new List<CardAction> { tap });
                    attachments.Add(heroCard.ToAttachment());
                }
                await turnContext.SendActivityAsync(MessageFactory.Text("See below a similair range of products"), cancellationToken);
                await turnContext.SendActivityAsync(MessageFactory.Carousel(attachments), cancellationToken);
            }
        
        }

        private static string GetMyName(string name = "Ewan") 
        {
            return "Hello" + name;
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
            string temp = turnContext.Activity.Text;
            if (string.IsNullOrEmpty(temp))
            {
                if (turnContext.Activity.Attachments.Any())
                {
                    turnContext.Activity.Attachments.FirstOrDefault();
                    var webClient = new WebClient();
                    string ImageUrl = turnContext.Activity.Attachments.FirstOrDefault().ContentUrl;
                    string mainColour = await AnalyzeUrlAsync(ImageUrl, turnContext, cancellationToken);
                    string mainTag = await MakePredictionRequest(ImageUrl, turnContext, cancellationToken);
                    await MakeQueryRequest(mainColour, mainTag, turnContext, cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Invalid input"), cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Invalid input"), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and Welcome!"), cancellationToken);
                }
            }
        }
    }
}

