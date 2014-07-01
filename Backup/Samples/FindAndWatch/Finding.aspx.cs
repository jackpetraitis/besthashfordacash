using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using eBay.Services;
using eBay.Services.Finding;
using Slf;

namespace Example
{
    public partial class _Default : System.Web.UI.Page
    {
        private FindingServicePortTypeClient client;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Init log
            LoggerService.SetLogger(new TraceLogger());

            // Get AppID and ServerAddress from Web.config
            string appID = System.Configuration.ConfigurationManager.AppSettings["AppID"];
            string findingServerAddress = System.Configuration.ConfigurationManager.AppSettings["FindingServerAddress"];
            
            ClientConfig config = new ClientConfig();
            // Initialize service end-point configration
            config.EndPointAddress = findingServerAddress;
            
            // set eBay developer account AppID
            config.ApplicationId = appID;

            // Create a service client
            client = FindingServiceClientFactory.getServiceClient(config);
        }

        protected void findItem_Click(object sender, EventArgs e)
        {
            
            try
            {
               
                // Create request object
                FindItemsAdvancedRequest request = new FindItemsAdvancedRequest();

                // Set request parameters
                request.keywords = keyword.Text;
                if (request.keywords == null)
                {
                    request.keywords = "ipod";
                }
                PaginationInput pi = new PaginationInput();
                pi.entriesPerPage = 10;
                pi.entriesPerPageSpecified = true;
                request.paginationInput = pi;

                // Call the service
                FindItemsAdvancedResponse response = client.findItemsAdvanced(request);

                // Show output
                if (response.searchResult != null && response.searchResult.item != null)
                {
                    SearchItem[] items = response.searchResult.item;

                    TableRow titlerow = new TableRow();
                    TableHeaderCell titlecell0 = new TableHeaderCell();
                    titlecell0.Text = "Item ID";
                    titlecell0.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell0);
                    TableHeaderCell titlecell1 = new TableHeaderCell();
                    titlecell1.Text = "Gallery";
                    titlecell1.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell1);
                    TableHeaderCell titlecell2 = new TableHeaderCell();
                    titlecell2.Text = "Title";
                    titlecell2.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell2);
                    TableHeaderCell titlecell3 = new TableHeaderCell();
                    titlecell3.Text = "Current Price";
                    titlecell3.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell3);
                    TableHeaderCell titlecell4 = new TableHeaderCell();
                    titlecell4.Text = "Time Left";
                    titlecell4.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell4);
                    TableHeaderCell titlecell5 = new TableHeaderCell();
                    titlecell5.Text = "Add To Watch";
                    titlecell5.BorderWidth = 1;
                    titlerow.Cells.Add(titlecell5);
                    result.Rows.Add(titlerow);

                    for (int i = 0; i < items.Length; i++)
                    {
                        TableRow tblrow = new TableRow();
                        
                        TableCell tblcell0 = new TableCell();
                        tblcell0.Text = items[i].itemId; 
                        tblcell0.BorderWidth = 1;
                        tblrow.Cells.Add(tblcell0);
                        TableCell tblcell1 = new TableCell();
                        tblcell1.Text = "<img src=" + items[i].galleryURL + ">";
                        tblcell1.BorderWidth = 1;
                        tblrow.Cells.Add(tblcell1);
                        TableCell tblcell2 = new TableCell();
                        tblcell2.Text = items[i].title;
                        tblcell2.BorderWidth = 1;
                        tblrow.Cells.Add(tblcell2);
                        TableCell tblcell3 = new TableCell();
                        tblcell3.Text = "$" + items[i].sellingStatus.currentPrice.Value.ToString();
                        tblcell3.BorderWidth = 1;
                        tblrow.Cells.Add(tblcell3);
                        TableCell tblcell4 = new TableCell();
                        tblcell4.Text = items[i].sellingStatus.timeLeft;
                        tblcell4.BorderWidth = 1;
                        tblrow.Cells.Add(tblcell4);
                        TableCell tblcell5 = new TableCell();
                        tblcell5.Text = "<a href=AddToWatchList.aspx?itemId=" + items[i].itemId + ">" + "Add To Watch" + "</a>";
                        tblcell5.BorderWidth = 1;
                        tblrow.Cells.Add(tblcell5);
                    
                        result.Rows.Add(tblrow);
                    }
                }
                
            }
            catch (Exception ex)
            {
                errorMsg.Text = ex.Message;
               
            }
        }
    }
}
