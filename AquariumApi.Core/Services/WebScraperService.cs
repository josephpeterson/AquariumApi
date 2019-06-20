using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Net;
using System.Linq;
using System.Globalization;

namespace AquariumApi.Core.Services
{
    public interface IWebScraperService
    {
        void ApplyWebpageToSpecies(string url, Species species);
    }
    public class WebScraperService : IWebScraperService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<WebScraperService> _logger;
        private readonly IConfiguration _config;

        public WebScraperService(IConfiguration config, IAquariumDao aquariumDao, IPhotoManager photoManager, ILogger<WebScraperService> logger)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;
        }
        public void ApplyWebpageToSpecies(string url,Species species)
        {
            string strPage = string.Empty;
            byte[] aReqtHTML;
            WebClient objWebClient = new WebClient();
            objWebClient.Headers.Add("User-Agent: Other");   //You must do this or HTTPS won't work
            aReqtHTML = objWebClient.DownloadData(url);  //Do the name search
            UTF8Encoding utf8 = new UTF8Encoding();
            strPage = utf8.GetString(aReqtHTML); // get the string from the bytes


            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(strPage);
            var body = htmlDoc.DocumentNode.SelectSingleNode("//body");

            var statsContainer = body.QuerySelector(".quick_stats_container");
            var statsData = statsContainer.NextSibling;


            var facts = statsContainer.InnerText.Replace("\t", "").Split("\r\n").ToList().Where(f => f != string.Empty && f != "\n" && f != "\r");

            for(var i=0;i<facts.Count();i+=2)
            {
                var label = facts.ElementAt(i);
                var value = facts.ElementAt(i+1);

                switch(label)
                {
                    case "Max. Size":
                        //Fix size
                        value = value.Replace("&frac34;", ".75")
                            .Replace("&frac14;", ".25")
                            .Replace("&frac12;", ".50")
                            .Replace("\"", "");
                        try
                        {

                            species.MaxSize = Convert.ToDecimal(value.Trim());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation("[WebScraper] Error parsing Max. Size");
                            _logger.LogError(ex.ToString());
                        }
                        break;
                    case "Minimum Tank Size":
                        //Fix size
                        value = value.Replace("&frac34;", ".75")
                            .Replace("&frac14;", ".25")
                            .Replace("&frac12;", ".50");
                        try {
                            species.MinimumGallons = Convert.ToInt16(value.Substring(0, value.IndexOf(" gallons")).Trim());
                        }
                        catch(Exception ex)
                        {
                            _logger.LogInformation("[WebScraper] Error parsing minimum gallons");
                            _logger.LogError(ex.ToString());
                        }
                        break;
                    case "Color Form":
                        //Fix size
                        var colors = value.Split(",");
                        if (colors.Count() > 0)
                            species.PrimaryColor = colors.ElementAt(0).Trim();
                        if (colors.Count() > 1)
                            species.SecondaryColor = colors.ElementAt(1).Trim();
                        break;
                    case "Care Level":
                        //Fix size
                        species.CareLevel = value.Trim();
                        break;
                    case "Water Conditions":
                        //Fix size
                        var stuff = value.Split("-");
                        try
                        {
                            var tempMin = stuff.ElementAt(0).Trim();
                            var tempMax = stuff.ElementAt(1).Substring(0, stuff.ElementAt(1).IndexOf("&")).Trim();
                            var phMin = stuff.ElementAt(2).Substring(stuff.ElementAt(2).IndexOf("pH") + 2).Trim();
                            var phMax = stuff.ElementAt(3).Trim();

                            species.TemperatureMax = int.Parse(tempMax);
                            species.TemperatureMin = int.Parse(tempMin);
                            species.PhMin = Convert.ToDecimal(phMin);
                            species.PhMax = Convert.ToDecimal(phMax);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogInformation("[WebScraper] Error parsing water conditions");
                            _logger.LogError(ex.ToString());
                        }
                        
                        break;
                    default:
                         break;
                }

            }

            //Description
            var description = body.QuerySelector(".product_overview").QuerySelector(".overview-content").InnerText.Replace("\r"," ").Replace("\n"," ").Trim();
            species.Description = description.Substring(0,description.Length > 600 ? 600:description.Length); 

            //Try for price
            var price = body.QuerySelector(".product_selection_container").QuerySelector("button").Attributes["data-list_price"].Value;
            species.Price = Convert.ToDecimal(price);

            //Thumbnail
            var thumbnailSrc = body.QuerySelector(".product-image").QuerySelector("img").Attributes["src"].Value;
            Uri uri = new Uri(url);
            string host = uri.Host;
            string prefix = uri.Scheme;
            species.Thumbnail = prefix + "://" + host +  thumbnailSrc;
        }
    }

}
public interface IScraperDefinition
{

}
