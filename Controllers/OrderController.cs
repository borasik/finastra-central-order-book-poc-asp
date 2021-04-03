using central_order_book_poc.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace central_order_book_poc.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrderController : Controller
    {        
        private readonly ILogger<OrderController> _Logger;

        private static readonly string EndpointUri = "https://cosmos-db-dv-152-01.documents.azure.com:443/";

        private static readonly string PrimaryKey = "MAGEbqgvAq8I74TfU4Ctc1YYTogIIYYsHVH7vO6EIy0PRwIaHGyS3M0Hdnfj0TQsmevWzRelmjKRcD4dXuiSbQ==";

        private CosmosClient cosmosClient;

        private Database database;

        private Container container;

        private string databaseId = "central-order-book-db";

        private string containerId = "central-order-book-container";

        public OrderController(ILogger<OrderController> logger)
        {           
            _Logger = logger;
        }

        [HttpPost]
        /// <summary>
        /// Creates and Stores Order with newly created ID
        /// </summary>
        /// <remarks>Finastra</remarks>
        /// <param name="orderRequest" example="{'prop', 'value'}">The OrderJsno Detailes</param>
        /// <response code="200">Order Stored</response>
        /// <response code="400">Order has missing/invalid values</response>
        /// <response code="500">Oops! Can't Order your product right now</response>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Create([FromBody] object orderRequest)
        {


            var newOrder = new OrderEntity()
            {
                Id = Guid.NewGuid().ToString(),
                OrderJsonDetails = orderRequest.ToString().Replace("\r\n", string.Empty).Replace(" ", string.Empty)
        };

            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "central-order-book-poc" });
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/id", 400);
            ItemResponse<OrderEntity> orderEntityResponse = await this.container.CreateItemAsync<OrderEntity>(newOrder, new PartitionKey(newOrder.Id.ToString()));

            return Created("orders/create", newOrder.Id);
        }

        [HttpGet]        
        public async Task<OrderEntity> Get(Guid orderId)
        {
            _Logger.LogInformation("Starting GET for OrderId: " + orderId);

            var sqlQueryText = String.Format("SELECT * FROM c WHERE c.id = '{0}'", orderId.ToString());

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);


            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "central-order-book-poc" });
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/id", 400);



            FeedIterator<OrderEntity> queryResultSetIterator = this.container.GetItemQueryIterator<OrderEntity>(queryDefinition);

            List<OrderEntity> orderEntities = new List<OrderEntity>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<OrderEntity> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (OrderEntity orderEntity in currentResultSet)
                {
                    orderEntities.Add(orderEntity);                    
                }
            }

            return orderEntities.FirstOrDefault();
        }
    }
}
