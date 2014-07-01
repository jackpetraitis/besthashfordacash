The findAndWatch sample is a composite sample which shows how
FindingKit and eBay Trading SDK can be used together to create applications.

Here are a few notes about this sample:
1. Environment Requirements
This sample is a ASP.Net web application, it needs an IIS application server to run,
it has been tested in VS2008 with built-in application server support.

2. Configuration
The sample needs to be configured before it can be run,
you need to fill in following information in appSettings section of the Web.config file:
 > eBay developer application id
 > eBay Finding server address
 > eBay Token
 > eBay Trading server address

3. Dependencies
The sample depends on following project and assemblies
 > eBay.Services project in eBay.Services folder, this is FindingKit 1.0 assembly
 > eBay.Service.dll in Lib\eBay.Trading.Service folder, this is eBay Trading SDK e705 assembly
 > SLF.dll in Lib\SLF folder

4. Application Flow
 > Find items by invoking FindItemsByKeywords call of eBay Finding service.
 > Add item to watch list by invoking AddToWatchList API of eBay Trading API.
 > Show Watch List by invoking GetMyeBayBuying API of eBay Trading API.