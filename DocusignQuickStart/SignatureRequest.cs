// DocuSign References
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;

// Additional References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using DocusignQuickStart;

namespace SignatureRequest
{
    public static class SignatureRequest
    {
        public static void Execute()
        {
            // Enter your DocuSign credentials
            var credentials = new DocusignQuickStart.DocusignCredentials
            {
                Username = "callum.rigby@udgroup.co.uk",
                Password = "Utilities01",
                IntegratorKey = "81f3bae3-f472-4a51-8d5f-e966f74cb0ab"
            };

            // specify the document (file) we want signed
            string SignTest1File = @"C:\Users\callu_000\Documents\UD Group\Docusign\CNG\CNG Contract.pdf";

            // Enter recipient (signer) name and email address
            string recipientName = "Callum Rigby";
            string recipientEmail = "callum.rigby93@gmail.com";

            string recipientName2 = "John Smith";
            string recipientEmail2 = "callum.rigby93@hotmail.co.uk";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            string basePath = "https://demo.docusign.net/restapi";

            // instantiate a new api client
            var apiClient = new ApiClient(basePath);

            // set client in global config so we don't need to pass it to each API object
            Configuration.Default.ApiClient = apiClient;

            string authHeader = JsonConvert.SerializeObject(credentials);
            //DocusignCredentials cred = JsonConvert.DeserializeObject<DocusignCredentials>(authHeader);
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login() results
            string accountId = null;

            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
            var authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();

			// user might be a member of multiple accounts
            accountId = loginInfo.LoginAccounts[0].AccountId;

            Console.WriteLine("LoginInformation: {0}", loginInfo.ToJson());

            // Read a file from disk to use as a document
            byte[] fileBytes = File.ReadAllBytes(SignTest1File);

            var envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

            // Add a document to the envelope
            var doc = new Document();
            doc.DocumentBase64 = Convert.ToBase64String(fileBytes);
            doc.Name = "TestFile.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the documeent
            var signer = new Signer();
            signer.Name = recipientName;
            signer.Email = recipientEmail;            
            signer.RecipientId = "1";
            signer.RoutingOrder = "1";

            var signer2 = new Signer();
            signer2.Name = recipientName2;
            signer2.Email = recipientEmail2;
            signer2.RecipientId = "2";
            signer.RoutingOrder = "2";

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new Tabs();
            signer.Tabs.SignHereTabs = new List<SignHere>();
            var signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "100";
            signHere.YPosition = "150";
            signer.Tabs.SignHereTabs.Add(signHere);

            signer2.Tabs = new Tabs();
            signer2.Tabs.SignHereTabs = new List<SignHere>();
            var signHere2 = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "2";
            signHere.XPosition = "100";
            signHere.YPosition = "200";
            signer2.Tabs.SignHereTabs.Add(signHere);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);
            envDef.Recipients.Signers.Add(signer2);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // Use the EnvelopesApi to send the signature request!
            var envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            // print the JSON response
            Console.WriteLine("EnvelopeSummary:\n{0}", JsonConvert.SerializeObject(envelopeSummary));
            Console.Read();
        }
    }
}
