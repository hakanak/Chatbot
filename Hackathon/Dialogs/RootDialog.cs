using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Deserializers;

namespace Hackathon.Dialogs
{

    public class AccountList
    {
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string accountSuffix { get; set; }
        public string accountBalance { get; set; }
        public string accountFEC { get; set; }
        public string accountFECCode { get; set; }
        public string ibanNumber { get; set; }
        public string accountType { get; set; }
        public string accountOpenDate { get; set; }
        public string accountBranch { get; set; }
        public string branchId { get; set; }
        public string customerName { get; set; }
    }

[Serializable]
    public class RootDialog : IDialog<object>
{
    public static int aktarma = 0;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;
            if (aktarma >= 1)
            {
                ///para transferi
                aktarma = 0;
                DateTime date=new DateTime();
                string[] temp = activity.Text.Split(',');
                var client = new RestClient("http://api.kuveytturk.com.tr/api/SendMoney");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("API_KEY", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjZSdVVzTnVsM0QiLCJuYmYiOjE0OTQ2NTk1NDgsImV4cCI6MTQ5NzI1MTU0OCwiaWF0IjoxNDk0NjU5NTQ4fQ.Bqds4dwtsHqobnenQxvlIRhyOGZfq4ZbSyKvdPXxG30");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "{\r\n  \"TransferInfo\" :     {\r\n        \"Amount\" : \""+temp[0]+"\",\r\n        \"ReceiverIBANNumber\" : \""+temp[1]+"\",\r\n        \"SenderAccountNumber\" : 133778,\r\n        \"SenderAccountSuffix\" :"+ temp[2]+",\r\n        \"TranDateWithTime\" : \""+date+"\"\r\n    }\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                dynamic data = JObject.Parse(response.Content.ToString());
                await context.PostAsync("İşleminizi gerçekleştiriyorum...Lütfen bekleyiniz.");


                if (data.success == "true")
                {
                    //kalan bakiye 
                    await context.PostAsync("<b style='color:green;'>Para transferi başarıyla gerçekleşti..</b>");
                    var client2 = new RestClient("http://api.kuveytturk.com.tr/API/AccountBalanceInfo");
                    var request2 = new RestRequest(Method.POST);
                    request2.AddHeader("cache-control", "no-cache");
                    request2.AddHeader("API_KEY", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjZSdVVzTnVsM0QiLCJuYmYiOjE0OTQ2NTk1NDgsImV4cCI6MTQ5NzI1MTU0OCwiaWF0IjoxNDk0NjU5NTQ4fQ.Bqds4dwtsHqobnenQxvlIRhyOGZfq4ZbSyKvdPXxG30");
                    request2.AddHeader("content-type", "application/json");
                    request2.AddParameter("application/json", "{\n  \"AccountNumber\":133778,\n  \"AccountSuffix\":" + temp[2] + "\n}", ParameterType.RequestBody);
                    IRestResponse response2 = client2.Execute(request2);

                    JObject json = JObject.Parse(response2.Content.ToString());
                    IList<JToken> resultsList = json["accountInfo"].Children().ToList();
                    
                    await context.PostAsync(resultsList[9].First.ToString()+" isimli şubeye bağlı "+resultsList[0].First.ToString()+" hesabınızda " + resultsList[5].First.ToString()+" değerinde <b style='color=green;'>" + resultsList[3].First.ToString()+"</b> bakiyeniz mevcuttur.");
                 

                }
                else
                {
                    JObject json = JObject.Parse(response.Content.ToString());
                    IList<JToken> resultsList = json["results"].Children().ToList();
                    await context.PostAsync("<b style='color:red;'>Üzgünüm işlem şu nedenlerden dolayı başarısız olmuştur..</b>");
                    foreach (var hesab in resultsList)
                    {
                        
                        await context.PostAsync(hesab.ToString());
                    }

                }
               

            }
          else  if (activity.Text.ToLower() == "eft" || activity.Text.ToLower() == "havale" || activity.Text.ToLower() == "transfer")
            {
                await context.PostAsync($"Immm Sanırım para gödermek istiyorsunuz.");
                await context.PostAsync($"Değerli müşterimiz lütfen bilgileri aşağıdaki formatta giriniz.");
                await context.PostAsync($"Tutar , IBAN , Gönderici Hesap Ön eki ");
                aktarma++;
            }

          else  if (activity.Text.ToLower()=="hesabım")
            {

                var client = new RestClient("http://api.kuveytturk.com.tr/API/Accounts");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("API_KEY", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjZSdVVzTnVsM0QiLCJuYmYiOjE0OTQ2NTk1NDgsImV4cCI6MTQ5NzI1MTU0OCwiaWF0IjoxNDk0NjU5NTQ4fQ.Bqds4dwtsHqobnenQxvlIRhyOGZfq4ZbSyKvdPXxG30");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "{\n  \"AccountNumber\":133778\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);


                JObject json = JObject.Parse(response.Content.ToString());
                IList<JToken> resultsList = json["accountList"].Children().ToList();
                List<AccountList> account=new List<AccountList>();

            

                foreach (JToken reso in resultsList)
                {
                   AccountList sample = reso.ToObject<AccountList>();
                    
                    account.Add(sample);
                }
                string data = "";
                int i = 0;
                foreach (var hesab in account)
                {
                    i++;
                    data = data +i+".Hesap\n"+ "Hesab Adı : " + hesab.accountName +"\nHesap Şube :"+hesab.accountBranch+ "\nHesap Bakiyesi : " + hesab.accountBalance+"\nHesap Para Birimi : "+hesab.accountFECCode+"\n\n\n";
                }

                await context.PostAsync(data);
            }
            else 
            {
                await context.PostAsync($"Anlamadım");
            }
          // await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }
    }
}