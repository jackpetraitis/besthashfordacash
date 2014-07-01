using System;
using eBay.Services;
using eBay.Services.Finding;
using Slf;

namespace ConsoleFindItems
{
    /// <summary>
    /// A sample to show eBay Finding servcie call using the simplied interface provided by the FindingKit.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Init log
            // This sample and the FindingKit use <a href="http://slf.codeplex.com/">Simple Logging Facade(SLF)</a>,
            // Here is a <a href="http://slf.codeplex.com/">good introdution</a> about SLF(for example, supported log levels, how to log to a file)
            LoggerService.SetLogger(new ConsoleLogger());
            ILogger logger = LoggerService.GetLogger();

            // Basic service call flow:
            // 1. Setup client configuration
            // 2. Create service client
            // 3. Create outbound request and setup request parameters
            // 4. Call the operation on the service client and receive inbound response
            // 5. Handle response accordingly
            // Handle exception accrodingly if any of the above steps goes wrong.
            try
            {
                ClientConfig config = new ClientConfig();
                // Initialize service end-point configuration
                config.EndPointAddress = "http://svcs.ebay.com/services/search/FindingService/v1";
                // set eBay developer account AppID
                config.ApplicationId = "YOUR APPID HERE";

                // Create a service client
                FindingServicePortTypeClient client = FindingServiceClientFactory.getServiceClient(config);

                // Create request object
                FindItemsByKeywordsRequest request = new FindItemsByKeywordsRequest();
                // Set request parameters
                request.keywords = "harry potter phoenix";
                PaginationInput pi = new PaginationInput();
                pi.entriesPerPage = 2;
                pi.entriesPerPageSpecified = true;
                request.paginationInput = pi;

                // Call the service
                FindItemsByKeywordsResponse response = client.findItemsByKeywords(request);

                // Show output
                logger.Info("Ack = " + response.ack);
                logger.Info("Find " + response.searchResult.count + " items.");
                SearchItem[] items = response.searchResult.item;
                for (int i = 0; i < items.Length; i++)
                {
                    logger.Info(items[i].title);
                }
            }
            catch (Exception ex)
            {
                // Handle exception if any
                logger.Error(ex);
            }

            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();

        }
    }
}
