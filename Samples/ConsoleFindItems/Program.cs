using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
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
        public const bool Debug = true;
        public static readonly string[] cardsOfInterest = 
        { 
            "Radeon 5970", "Radeon 6850", "Radeon 6870", "Radeon 6950",
            "Radeon 6970", "Radeon 6990", "Radeon 7850", "Radeon 7870", 
            "Radeon 7950", "Radeon 7970", "Radeon 7990", "Radeon 270X",
            "Radeon 280X", "Radeon 290", "Radeon 290X"
        };
        public static readonly string[] shortNamesForCards =
        {
            "r5970", "r6850", "r6870", "r6950", 
            "r6970", "r6990", "r7850", "r7870",
            "r7950", "r7970", "r7990", "r270X",
            "r280X", "r290", "r290X"
        };

        static void WriteEbayPricesToSQLServer(int passedId, string passedName, string passedPrice, string passedHash, string passedHashForCash)
        {
            SqlConnection azureConnection = new SqlConnection("user id=jackman;" +
                                       "password=d56W$@JP!;server=m8ielxo4eq.database.windows.net;" +
                                       "Trusted_Connection=no;" +
                                       "database=hashforcash_db; " +
                                       "connection timeout=30");
            try
            {
                azureConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            


            try
            {
                SqlCommand myCommand = new SqlCommand("UPDATE ebay SET (CardName = "+passedName+", AveragePrice = "+passedName+", AverageHash = "+passedHash+", HashForCash = " +passedHashForCash+") " +
                                    "WHERE (ID = " + passedId + ")", azureConnection);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
           

            try
            {
                azureConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void logSearchResultsToFile(SearchItem[] incomingItems)
        {
            string line = "Search Results: ";
            string path = @"searchResults.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(line);
                }
            }
            using (System.IO.StreamWriter file = File.AppendText(path))
            {
                foreach (SearchItem item in incomingItems)
                {
                    file.WriteLine();
                    file.WriteLine(item.title);
                    file.WriteLine(item.itemId);
                    file.WriteLine("Category: " + item.primaryCategory.categoryName +" ID: " + item.primaryCategory.categoryId);
                    file.WriteLine("$"+item.sellingStatus.currentPrice.Value);
                    file.WriteLine(item.sellingStatus.sellingState);
                }                
            }            
        }

        static List<VideoCard> GrabPricesFromEbay(string keyword)
        {
            List<VideoCard> foundCards = new List<VideoCard>();

            //setup for console logging of api results
            LoggerService.SetLogger(new ConsoleLogger());
            ILogger logger = LoggerService.GetLogger();

            try
            {
                ClientConfig config = new ClientConfig();
                // Initialize service end-point configuration
                config.EndPointAddress = "http://svcs.sandbox.ebay.com/services/search/FindingService/v1";
                // set eBay developer account AppID
                config.ApplicationId = "JackPetr-b629-4a64-94a2-cb909b9c0c47";

                // Create a service client
                FindingServicePortTypeClient client = FindingServiceClientFactory.getServiceClient(config);

                // Create request object
                FindCompletedItemsRequest completedItemsRequest = new FindCompletedItemsRequest();

                // Set request parameters
                keyword = keyword + " -water -block -waterblock -heatsink -heat -sink";
                completedItemsRequest.keywords = keyword;

                DateTime today = DateTime.UtcNow;
                DateTime month = new DateTime(today.Year, today.Month, 1);
                DateTime lastMonth = month.AddMonths(-1);

                ItemFilter if1 = new ItemFilter();
                if1.name = ItemFilterType.EndTimeFrom;
                if1.value = new string[] {lastMonth.ToString("o")};

                ItemFilter if2 = new ItemFilter();
                if2.name = ItemFilterType.EndTimeTo;
                if2.value = new string[] { today.ToString("o") };

                ItemFilter if3 = new ItemFilter();
                if3.name = ItemFilterType.MinPrice;
                if3.paramName = "Currency";
                if3.paramValue = "USD";
                if3.value = new string[] {"50"};
                
                ItemFilter if4 = new ItemFilter();
                if4.name = ItemFilterType.MaxPrice;
                if4.paramName = "Currency";
                if4.paramValue = "USD";
                if4.value = new string[] { "1100" };

                ItemFilter if5 = new ItemFilter();
                if5.name = ItemFilterType.ExcludeCategory;
                if5.value = new string[] {"179"};

                ItemFilter[] ifa = new ItemFilter[5];
                ifa[0] = if1;
                ifa[1] = if2;
                ifa[2] = if3;
                ifa[3] = if4;
                ifa[4] = if5;
                completedItemsRequest.itemFilter = ifa;

                PaginationInput pi = new PaginationInput();
                pi.entriesPerPage = 200;
                pi.entriesPerPageSpecified = true;
                completedItemsRequest.paginationInput = pi;

                // Call the service
                FindCompletedItemsResponse response = client.findCompletedItems(completedItemsRequest);
                
                // Show output
                if (Debug)
                    if (response.ack.ToString().Equals("Failure"))
                    {
                        int tries = 0;
                        while (tries < 3)
                        {
                            if (response.ack.ToString().Equals("Failure"))
                            {
                                //retry
                                tries++;
                                response = client.findCompletedItems(completedItemsRequest);
                            }else
                            {
                                break;
                            }
                    }
                }
                logger.Info("Ack = " + response.ack);
                logger.Info("Find " + response.searchResult.count + " items.");
                SearchItem[] items = response.searchResult.item;

                if(Debug){logSearchResultsToFile(items);}
                foreach (SearchItem item in items)
                {
                    if (item.sellingStatus.sellingState.Equals("EndedWithSales"))
                    {
                        VideoCard trackCard = new VideoCard();
                        trackCard.SetCardName(item.title);
                        trackCard.SetPrice(item.sellingStatus.currentPrice.Value);
                        foundCards.Add(trackCard);

                        logger.Info("Item" + item.title + "added!");
                        logger.Info(item.sellingStatus.currentPrice.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception if any
                logger.Error(ex);
            }

            return foundCards;
        }

        private static RefVideoCard calculateAverages(List<VideoCard> measureTheseCards, string cardName)
        {
            //Takes in a list of videocards that are of the same card.
            //Also takes in another string argument that is the cardname.
            string strippedIdFromName = Regex.Replace(cardName, "[^0-9.]", "");
            RefVideoCard finishedCard = new RefVideoCard();
            
            double totalPrice = 0;
            int totalCards = measureTheseCards.Count;

            //Counts them and average them 
            finishedCard.SetCardCount(totalCards);
            foreach (VideoCard card in measureTheseCards)
            {
                totalPrice += card.GetPrice();
            }
            finishedCard.SetAveragePrice(Convert.ToString(totalPrice/totalCards));

            //Set the ID from the stripped cardname argument
            if (cardName.Contains("290X"))
            {
                finishedCard.SetId((Convert.ToInt32(strippedIdFromName)) + 1);
            }
            else
            {
                finishedCard.SetId((Convert.ToInt32(strippedIdFromName)));
            }

            //Return the finishedcard as a RefVideoCard object with the AveragePrice, ID, CardCount variables set
            finishedCard.SetCardName(cardName);
            return finishedCard;
        }

        private static List<FinishedVideoCard> CalculateHashForCash(List<VideoCard> cashedCards, List<RefVideoCard> hashedCards )
        {
            List<FinishedVideoCard> resultList = new List<FinishedVideoCard>();

            for (int i = 0; i < cashedCards.Count; i++)
            {
                FinishedVideoCard finishedCard = new FinishedVideoCard();
                finishedCard.setId(cashedCards[i].GetId());
                finishedCard.setCardName(cashedCards[i].GetCardName());
                finishedCard.setAveragePrice(Convert.ToString(cashedCards[i].GetPrice()));
                finishedCard.setAverageHash(hashedCards[i].GetAverageHash());
                finishedCard.setHashForCash(Convert.ToString(Convert.ToDouble(hashedCards[i].GetAverageHash())/cashedCards[i].GetPrice()));
                resultList.Add(finishedCard);
            }


            return resultList;
        }

        static List<VideoCard> readFile(String filename)
        {
            string text = System.IO.File.ReadAllText(filename);
            System.Console.WriteLine("Contents of radeonCards.csv = {0}", text);

            string[] lines = System.IO.File.ReadAllLines(filename);
            System.Console.WriteLine("Contents of WriteLines2.txt");
            List<VideoCard> cardList = new List<VideoCard>();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                VideoCard video = new VideoCard();
                Console.WriteLine("\t" + line);

                if (line.Contains('"'))
                {
                    List<string> quotes = line.Split('"').ToList<string>();
                    List<string> parsedInfo = quotes[2].Split(',').ToList<string>();
                    video.SetCardName(quotes[1]);
                    video.SetHashRate(Convert.ToInt32(parsedInfo[1]));
                }
                else
                {
                    List<string> parsedInfo = line.Split(',').ToList<string>();
                    video.SetCardName(parsedInfo[0]);
                    video.SetHashRate(Convert.ToInt32(parsedInfo[1]));
                }

                video.SetId(i);
                cardList.Add(video);
            }
            return cardList;

        }

        static void Main(string[] args)
        {
            //grab cards from excel sheet and compute average hashes
            List<VideoCard> basicInfoList = new List<VideoCard>();
            basicInfoList = readFile(@"C:\radeon.csv");
            
            //delete JSON values from previous program execution
            deletePreviousResults();
            
            int status = 0;
            
            //
            List<VideoCard> cardsFromEbay = new List<VideoCard>();
            List<RefVideoCard> almostDoneCards = calculateAverageHash(basicInfoList);
            RefVideoCard resultCardStats = new RefVideoCard();
            List<RefVideoCard> cardStatsList = new List<RefVideoCard>();
            List<VideoCard> cashAveragedCards = new List<VideoCard>();
            for (int i = 0; i < cardsOfInterest.Length; i++)
            {
                
                //grab cards from ebay from list of interesting cards
                //cardsFromEbay List = GrabPricesFromEbay("Radeon 5970");
                cardsFromEbay = GrabPricesFromEbay(cardsOfInterest[i]);

                //we now find the AveragePrice, CardCount, and ID of the list of Radeon 5970s
                resultCardStats = calculateAverages(cardsFromEbay, cardsOfInterest[i]);
                cardStatsList.Add(resultCardStats);
                VideoCard cashedCard = new VideoCard();
                cashedCard.SetCardName(resultCardStats.GetCardName());
                cashedCard.SetId(resultCardStats.GetId());
                cashedCard.SetPrice(Convert.ToDouble(resultCardStats.GetAveragePrice()));
                cashAveragedCards.Add(cashedCard);
                //We now append the AveragePrice, CardCount, and ID info to the JSON file (in later version this could be an API call that returns JSON)
                
                //WriteEbayPricesToSQLServer(resultCardStats.GetId());
            }
            List<FinishedVideoCard> finishedCards = CalculateHashForCash(cashAveragedCards, almostDoneCards);
            for (int i = 0; i < cardsOfInterest.Length; i++)
            {
                if (i == 0)
                {
                    status = 0;
                }
                else if (i == cardsOfInterest.Length - 1)
                {
                    status = 2;
                }
                else
                {
                    status = 1;
                }
                writeEbayPricesToJSON(cardStatsList[i], status, finishedCards[i]);
            }
            Console.WriteLine("It got to here! Yay! :)");
            Console.ReadKey();

        }
        
        private static void deletePreviousResults()
        {
            string path = @"cashAveragesAppended.JSON";
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    GC.Collect();
                    Thread.Sleep(500);
                    File.Delete(path);
                }
            }
            string path2 = @"searchResults.txt";
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path2);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    GC.Collect();
                    Thread.Sleep(500);
                    File.Delete(path2);
                }
            }
        }

        private static void writeEbayPricesToJSON(RefVideoCard cardStatistics, int status, FinishedVideoCard almostFinishedVideoCard)
        {
            string line = "";
            string path = @"cashAveragesAppended.JSON";
            
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    if (status == 0)
                    {
                        line = "averageCash: [ ";
                        sw.WriteLine(line);
                    }
                }
            }
            
            using (System.IO.StreamWriter file = File.AppendText(path))
            {
                string firstBracket = "{";
                string cardId ='"' + "Id" + '"' + ": " + '"' + cardStatistics.GetId() + '"' + ",";
                string cardName = '"' + "Name" + '"' + ": " + '"' + cardStatistics.GetCardName() + '"' + ",";
                string cardAvgHash = '"' + "AvgHash" + '"' + ": " + '"' + almostFinishedVideoCard.getAverageHash() + '"' + ",";
                string cardAvgPrice = '"' + "AvgPrice" + '"' + ": " + '"' + cardStatistics.GetAveragePrice() + '"' + ",";
                string hashForCash = '"' + "HashForCash" + '"' + ": " + '"' + almostFinishedVideoCard.getHashForCash() + '"';
    
                string lastBracket = "},";
                string lastBracketNoComma = "}";
                file.WriteLine(firstBracket);
                file.WriteLine(cardId);
                file.WriteLine(cardName);
                file.WriteLine(cardAvgHash);
                file.WriteLine(cardAvgPrice);
                file.WriteLine(hashForCash);

                if (status == 2)
                {
                    file.WriteLine(lastBracketNoComma);
                }
                else
                {
                    file.WriteLine(lastBracket);
                }

                if (status == 2)
                {
                    line = "]";
                    file.WriteLine(line);
                }
            }
        }

        public static List<RefVideoCard> calculateAverageHash(List<VideoCard> inputCards)
        {
            List<RefVideoCard> averagedCards = new List<RefVideoCard>();

            RefVideoCard r5970 = new RefVideoCard();
            RefVideoCard r6850 = new RefVideoCard();
            RefVideoCard r6870 = new RefVideoCard();
            RefVideoCard r6950 = new RefVideoCard();
            RefVideoCard r6970 = new RefVideoCard();
            RefVideoCard r6990 = new RefVideoCard();
            RefVideoCard r7850 = new RefVideoCard();
            RefVideoCard r7870 = new RefVideoCard();
            RefVideoCard r7950 = new RefVideoCard();
            RefVideoCard r7970 = new RefVideoCard();
            RefVideoCard r7990 = new RefVideoCard();
            RefVideoCard r270X = new RefVideoCard();
            RefVideoCard r280X = new RefVideoCard();
            RefVideoCard r290 = new RefVideoCard();
            RefVideoCard r290X = new RefVideoCard();

            foreach (VideoCard card in inputCards)
            {
                if (card.GetCardName().Contains("5970"))
                {
                    r5970.SetHashSoFar(r5970.GetHashSoFar() + card.GetHashRate());
                    r5970.SetCardCount(r5970.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("6850"))
                {
                    r6850.SetHashSoFar(r6850.GetHashSoFar() + card.GetHashRate());
                    r6850.SetCardCount(r6850.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("6870"))
                {
                    r6870.SetHashSoFar(r6870.GetHashSoFar() + card.GetHashRate());
                    r6870.SetCardCount(r6870.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("6950"))
                {
                    r6950.SetHashSoFar(r6950.GetHashSoFar() + card.GetHashRate());
                    r6950.SetCardCount(r6950.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("6970"))
                {
                    r6970.SetHashSoFar(r6970.GetHashSoFar() + card.GetHashRate());
                    r6970.SetCardCount(r6970.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("6990"))
                {
                    r6990.SetHashSoFar(r6990.GetHashSoFar() + card.GetHashRate());
                    r6990.SetCardCount(r6990.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("7850"))
                {
                    r7850.SetHashSoFar(r7850.GetHashSoFar() + card.GetHashRate());
                    r7850.SetCardCount(r7850.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("7870"))
                {
                    r7870.SetHashSoFar(r7870.GetHashSoFar() + card.GetHashRate());
                    r7870.SetCardCount(r7870.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("7950"))
                {
                    r7950.SetHashSoFar(r7950.GetHashSoFar() + card.GetHashRate());
                    r7950.SetCardCount(r7950.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("7970"))
                {
                    r7970.SetHashSoFar(r7970.GetHashSoFar() + card.GetHashRate());
                    r7970.SetCardCount(r7970.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("7990"))
                {
                    r7990.SetHashSoFar(r7990.GetHashSoFar() + card.GetHashRate());
                    r7990.SetCardCount(r7990.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("270X"))
                {
                    r270X.SetHashSoFar(r270X.GetHashSoFar() + card.GetHashRate());
                    r270X.SetCardCount(r270X.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("280X"))
                {
                    r280X.SetHashSoFar(r280X.GetHashSoFar() + card.GetHashRate());
                    r280X.SetCardCount(r280X.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("290") && !card.GetCardName().Contains("290X"))
                {
                    r290.SetHashSoFar(r290.GetHashSoFar() + card.GetHashRate());
                    r290.SetCardCount(r290.GetCardCount() + 1);
                }
                if (card.GetCardName().Contains("290X"))
                {
                    r290X.SetHashSoFar(r290X.GetHashSoFar() + card.GetHashRate());
                    r290X.SetCardCount(r290X.GetCardCount() + 1);
                }
            }


            r5970.SetAverageHash(Convert.ToString(r5970.GetHashSoFar() / r5970.GetCardCount()));
            r5970.SetId(5970);
            r5970.SetCardName("Radeon 5970");
            averagedCards.Add(r5970);

            r6850.SetAverageHash(Convert.ToString(r6850.GetHashSoFar() / r6850.GetCardCount()));
            r6850.SetId(6850);
            r6850.SetCardName("Radeon 6850");
            averagedCards.Add(r6850);

            r6870.SetAverageHash(Convert.ToString(r6870.GetHashSoFar() / r6870.GetCardCount()));
            r6870.SetId(6870);
            r6870.SetCardName("Radeon 6870");
            averagedCards.Add(r6870);

            r6950.SetAverageHash(Convert.ToString(r6950.GetHashSoFar() / r6950.GetCardCount()));
            r6950.SetId(6950);
            r6950.SetCardName("Radeon 6950");
            averagedCards.Add(r6950);

            r6970.SetAverageHash(Convert.ToString(r6970.GetHashSoFar() / r6970.GetCardCount()));
            r6970.SetId(6970);
            r6970.SetCardName("Radeon 6970");
            averagedCards.Add(r6970);

            r6990.SetAverageHash(Convert.ToString(r6990.GetHashSoFar() / r6990.GetCardCount()));
            r6990.SetId(6990);
            r6990.SetCardName("Radeon 6990");
            averagedCards.Add(r6990);

            r7850.SetAverageHash(Convert.ToString(r7850.GetHashSoFar() / r7850.GetCardCount()));
            r7850.SetId(7850);
            r7850.SetCardName("Radeon 7850");
            averagedCards.Add(r7850);

            r7870.SetAverageHash(Convert.ToString(r7870.GetHashSoFar() / r7870.GetCardCount()));
            r7870.SetId(7870);
            r7870.SetCardName("Radeon 7870");
            averagedCards.Add(r7870);

            r7950.SetAverageHash(Convert.ToString(r7950.GetHashSoFar() / r7950.GetCardCount()));
            r7950.SetId(7950);
            r7950.SetCardName("Radeon 7950");
            averagedCards.Add(r7950);

            r7970.SetAverageHash(Convert.ToString(r7970.GetHashSoFar() / r7970.GetCardCount()));
            r7970.SetId(7970);
            r7970.SetCardName("Radeon 7970");
            averagedCards.Add(r7970);

            r7990.SetAverageHash(Convert.ToString(r7990.GetHashSoFar() / r7990.GetCardCount()));
            r7990.SetId(7990);
            r7990.SetCardName("Radeon 7990");
            averagedCards.Add(r7990);

            r270X.SetAverageHash(Convert.ToString(r270X.GetHashSoFar() / r270X.GetCardCount()));
            r270X.SetId(270);
            r270X.SetCardName("Radeon 270X");
            averagedCards.Add(r270X);

            r280X.SetAverageHash(Convert.ToString(r280X.GetHashSoFar() / r280X.GetCardCount()));
            r280X.SetId(280);
            r280X.SetCardName("Radeon 280X");
            averagedCards.Add(r280X);

            r290.SetAverageHash(Convert.ToString(r290.GetHashSoFar() / r290.GetCardCount()));
            r290.SetId(290);
            r290.SetCardName("Radeon 290");
            averagedCards.Add(r290);

            r290X.SetAverageHash(Convert.ToString(r290X.GetHashSoFar() / r290X.GetCardCount()));
            r290X.SetId(291);
            r290X.SetCardName("Radeon 290X");
            averagedCards.Add(r290X);

            return averagedCards;
        }
    }
}



