using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ContactManagerOfiice.DbContext
{

    public class ContactRepository
    {

        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://contactmanager.documents.azure.com:443/";

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "Ii4e1F22OyR5574KQBiWqRPmULYBQq7Dv92x5l5MIDi1JW6IwddABQydh4xskBpgUpSejInCW1PmhYpikNBr9w==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Microsoft.Azure.Cosmos.Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "Contact";
        private string containerId = "Items";

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database);
        }     

       
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }     

        public async Task Initilaize()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
        }
    
        public async Task<Contact> AddItemsToContainerAsync(Contact contact)
        {
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Contact> getContactResponse = await this.container.ReadItemAsync<Contact>(contact.Id, new PartitionKey(contact.PartitionKey));

                if (!string.IsNullOrEmpty(getContactResponse.Resource.Id))
                {
                    var savedContactResponse = await container.CreateItemAsync<Contact>(
                      contact,
                      new PartitionKey(contact.PartitionKey));
                return (savedContactResponse);
            }
                return (null);

            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw ex;
            }
        }

        public async Task<List<Contact>> QueryItemsAsync(string id)
        { 
            QueryDefinition query = new QueryDefinition(
                    "select * from c where c.id = @id")
                    .WithParameter("@id", id);
            FeedIterator<Contact> queryResultSetIterator = this.container.GetItemQueryIterator<Contact>(query);

            List<Contact> contacts = new List<Contact>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Contact> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Contact contact in currentResultSet)
                {
                    contacts.Add(contact);
                }
            }
            return contacts;
        }

        public async Task<Contact> GetItemAsync(string id, string partitionKeyValue)
        {
            ItemResponse<Contact> contactResponse = await this.container.ReadItemAsync<Contact>(id, new PartitionKey(partitionKeyValue));
            return contactResponse.Resource;            
        }
       
        public async Task<Contact> UpdateContactItemAsync(Contact contact)
        {
           
               var contactResponse = await this.container.ReplaceItemAsync<Contact>(contact, contact.Id, new PartitionKey(contact.PartitionKey));
               return contactResponse;
    
        }
        
        public async Task DeleteContactItemAsync(string id, string partitionKeyValue)
        {
            ItemResponse<Contact> getContactResponse = await this.container.ReadItemAsync<Contact>(id, new PartitionKey(partitionKeyValue));
            var itemBody = getContactResponse.Resource;
            if (string.IsNullOrEmpty(getContactResponse.Resource.Id))
            {
                // Delete an item. Note we must provide the partition key value and id of the item to delete
                ItemResponse<Contact> deleteResponse = await this.container.DeleteItemAsync<Contact>(id, new PartitionKey(partitionKeyValue));
            }

        }
    }
}
