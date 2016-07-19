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

namespace EmbeddedSigning
{
    public static class EmbeddedSigning
    {
        public static void Execute()
        {
            // Enter your DocuSign credentials
            string Username = "[EMAIL]";
            string Password = "[PASSWORD]";
            string IntegratorKey = "[INTEGRATOR_KEY]";

            // specify the document we want signed
            string SignTest1File = @"[PATH/TO/DOCUMENT/TEST.PDF]";

            // Enter recipient (signer) name and email address
            string recipientName = "[SIGNER_NAME]";
            string recipientEmail = "[SIGNER_EMAIL]";

            // instantiate api client with appropriate environment (for production change to www.docusign.net/restapi)
            string basePath = "https://demo.docusign.net/restapi";

            // instantiate a new api client
            ApiClient apiClient = new ApiClient(basePath);

            // set client in global config so we don't need to pass it to each API object
            Configuration.Default.ApiClient = apiClient;

            string authHeader = "{\"Username\":\"" + Username + "\", \"Password\":\"" + Password + "\", \"IntegratorKey\":\"" + IntegratorKey + "\"}";
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

            // we will retrieve this from the login() results
            string accountId = null;

            // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
            AuthenticationApi authApi = new AuthenticationApi();
            LoginInformation loginInfo = authApi.Login();

			// user might be a member of multiple accounts
            accountId = loginInfo.LoginAccounts[0].AccountId;

            Console.WriteLine("LoginInformation: {0}", loginInfo.ToJson());

            // Read a file from disk to use as a document
            byte[] fileBytes = File.ReadAllBytes(SignTest1File);

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
            doc.Name = "TestFile.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the documeent
            Signer signer = new Signer();
            signer.Name = recipientName;
            signer.Email = recipientEmail;
            signer.RecipientId = "1";

            // must set |clientUserId| to embed the recipient
            signer.ClientUserId = "1234"; 

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new Tabs();
            signer.Tabs.SignHereTabs = new List<SignHere>();
            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "100";
            signHere.YPosition = "150";
            signer.Tabs.SignHereTabs.Add(signHere);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            // Use the EnvelopesApi to create and send the signature request
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);
            
            Console.WriteLine("EnvelopeSummary:\n{0}", JsonConvert.SerializeObject(envelopeSummary));

            RecipientViewRequest viewOptions = new RecipientViewRequest()
            {
                ReturnUrl = "https://www.docusign.com/devcenter",
                ClientUserId = "1234",  // must match clientUserId set in step #2!
                AuthenticationMethod = "email",
                UserName = recipientName,
                Email = recipientEmail
            };

            // create the recipient view (aka signing URL)
            ViewUrl recipientView = envelopesApi.CreateRecipientView(accountId, envelopeSummary.EnvelopeId, viewOptions);

            // print the JSON response
            Console.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(recipientView));

            // Start the embedded signing session!
            System.Diagnostics.Process.Start(recipientView.Url);
        }
    }
}
