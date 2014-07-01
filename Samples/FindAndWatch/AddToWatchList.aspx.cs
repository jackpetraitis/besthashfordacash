using System;
using System.Collections;
using System.Configuration;
//using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using eBay.Service.Core.Soap;
using eBay.Service.Core.Sdk;
using eBay.Service.Call;
using eBay.Service.Util;
using Slf;

namespace Example
{
    public partial class AddToWatchList : System.Web.UI.Page
    {
        private static ApiContext apiContext = null;

        protected void Page_Init()
        {
            //get eBayToken and ServerAddress from Web.config
            string tradingServerAddress = System.Configuration.ConfigurationManager.AppSettings["TradingServerAddress"];
            string eBayToken = System.Configuration.ConfigurationManager.AppSettings["EBayToken"];
            
            apiContext = new ApiContext();

            //set Api Server Url
            apiContext.SoapApiServerUrl = tradingServerAddress;

            //set Api Token to access eBay Api Server
            ApiCredential apiCredential = new ApiCredential();
            apiCredential.eBayToken = eBayToken;
            apiContext.ApiCredential = apiCredential;

            //set eBay Site target to US
            apiContext.Site = SiteCodeType.US;

            //set Api logging
            apiContext.ApiLogManager = new ApiLogManager();
            apiContext.ApiLogManager.ApiLoggerList.Add(
                new FileLogger("trading_log.txt", true, true, true)
                );
            apiContext.ApiLogManager.EnableLogging = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            //get request parameter
            StringCollection itemIds = new StringCollection();
            String itemId = Request.QueryString["itemId"];
            itemIds.Add(itemId);

            //create AddToWatchListCall object
            AddToWatchListCall addToWatchListCall = new AddToWatchListCall(apiContext);
            try
            {

                //call AddToWatchList service
                addToWatchListCall.AddToWatchList(itemIds, null);

                //create GetMyeBayBuyingCall object
                GetMyeBayBuyingCall getMyeBayBuyingCall = new GetMyeBayBuyingCall(apiContext);

                //configure call parameters
                DetailLevelCodeTypeCollection level = new DetailLevelCodeTypeCollection();
                level.Add(DetailLevelCodeType.ReturnAll);
                getMyeBayBuyingCall.DetailLevelList = level;

                //call GetMyeBayBuying service               
                getMyeBayBuyingCall.GetMyeBayBuying();
   
                //show result    
                if (getMyeBayBuyingCall.WatchListReturn != null)
                {
                    ItemTypeCollection items = getMyeBayBuyingCall.WatchListReturn.ItemArray;

                    if (items != null)
                    {
                        // watching count
                        watchCount.Text = "Total number of watching items : " + items.Count;

                        //title
                        TableRow titlerow = new TableRow();
                        TableHeaderCell titlecell0 = new TableHeaderCell();
                        titlecell0.Text = "Item ID";
                        titlecell0.BorderWidth = 1;
                        titlerow.Cells.Add(titlecell0);
                        TableHeaderCell titlecell1 = new TableHeaderCell();
                        titlecell1.Text = "Garrlery";
                        titlecell1.BorderWidth = 1;
                        titlerow.Cells.Add(titlecell1);
                        TableHeaderCell titlecell2 = new TableHeaderCell();
                        titlecell2.Text = "Title(Click to view item on eBay)";
                        titlecell2.BorderWidth = 1;
                        titlerow.Cells.Add(titlecell2);
                        TableHeaderCell titlecell3 = new TableHeaderCell();
                        titlecell3.Text = "Current Price";
                        titlecell3.BorderWidth = 1;
                        titlerow.Cells.Add(titlecell3);
                        watchList.Rows.Add(titlerow);

                        //data
                        for (int i = 0; i < items.Count; i++)
                        {
                            TableRow tblrow = new TableRow();
                            for (int j = 0; j < 1; j++)
                            {
                                TableCell tblcell0 = new TableCell();
                                tblcell0.Text = items[i].ItemID;
                                tblcell0.BorderWidth = 1;
                                tblrow.Cells.Add(tblcell0);

                                TableCell tblcell1 = new TableCell();
                                if (items[i].PictureDetails != null)
                                {
                                    tblcell1.Text = "<img src=" + items[i].PictureDetails.GalleryURL + ">";
                                }
                                tblcell1.BorderWidth = 1;
                                tblrow.Cells.Add(tblcell1);
                                
                                TableCell tblcell2 = new TableCell();
                                tblcell2.Text = "<a href=" + items[i].ListingDetails.ViewItemURL + ">" + items[i].Title + "</a>";
                                tblcell2.BorderWidth = 1;
                                tblrow.Cells.Add(tblcell2);

                                TableCell tblcell3 = new TableCell();
                                tblcell3.Text = "$" + items[i].SellingStatus.CurrentPrice.Value.ToString();
                                tblcell3.BorderWidth = 1;
                                tblrow.Cells.Add(tblcell3);
                            }
                            watchList.Rows.Add(tblrow);
                        }
                    }
                }
                

            }
            catch (Exception exc)
            {
                errorMsg.Text = exc.Message;
            }
            
        }

    }
}
