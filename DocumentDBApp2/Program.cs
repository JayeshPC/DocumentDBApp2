using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;


namespace DocumentDBApp2
{
    class Program
    {
        private const string EndpointUrl = "https://jaycosmosdb.documents.azure.com:443/";
        private const string AuthorizationKey = "9zryibbuseFEFvsYT4kPCfSahjV4BqgNjYM3AeT6s9RYXMKQzIz47zBNrY5hHSh0fzd1xrPVgjPxWNzZgjEAmA==";

        private static Database database;
        private static DocumentCollection collection;

        static void Main(string[] args)
        {
            try
            {
                CreateDocumentClient().Wait();
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }

            Console.ReadKey();
        }

        private static async Task CreateDocumentClient()
        {
            // Create a new instance of the DocumentClient
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            {
                database = client.CreateDatabaseQuery("SELECT * FROM c WHERE c.id = 'mynewdb'").AsEnumerable().First();

                collection = client.CreateDocumentCollectionQuery(database.CollectionsLink, "SELECT * FROM c WHERE c.id = 'mynewdbcollection'").AsEnumerable().First();

                await CreateDocuments(client);
            }

        }
        private async static Task<Document> CreateDocument(DocumentClient client, object documentObject)
        {

            var result = await client.CreateDocumentAsync(collection.SelfLink, documentObject);
            var document = result.Resource;

            Console.WriteLine("Created new document: {0}\r\n{1}", document.Id, document);
            return result;
        }
        private async static Task CreateDocuments(DocumentClient client)
        {
            Console.WriteLine();
            Console.WriteLine("**** Create Documents ****");
            Console.WriteLine();

            dynamic document1Definition = new
            {
                name = "Azure Training",
                address = new
                {
                    addressType = "Test Office",
                    addressLine1 = "123 Main Street",
                    location = new
                    {
                        city = "Irving",
                        stateProvinceName = "California"
                    },
                    postalCode = "06756",
                    countryRegionName = "US"
                },
            };

            Document document1 = await CreateDocument(client, document1Definition);
            Console.WriteLine("Created document {0} from dynamic object", document1.Id);
            Console.WriteLine();
        }

    }
}
