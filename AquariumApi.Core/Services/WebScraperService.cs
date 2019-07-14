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
using AquariumApi.Core.ScraperDefinitions;

namespace AquariumApi.Core.Services
{
    public interface IWebScraperService
    {
        void ApplyWebpageToSpecies(string url, Species species);
        List<IScraperDefinition> GetDefinitions();
    }
    public class WebScraperService : IWebScraperService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<WebScraperService> _logger;
        private readonly List<IScraperDefinition> _definitions;
        private readonly IConfiguration _config;

        public WebScraperService(IConfiguration config, IAquariumDao aquariumDao, ILogger<WebScraperService> logger)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;

            _definitions = new List<IScraperDefinition>();
            _definitions.Add(new LiveAquariaScraperDefinition("www.liveaquaria.com",logger)); //todo read from config
            _definitions.Add(new FishbaseScraperDefinition("www.fishbase.se",logger)); //todo read from config

        }
        public static HtmlNode RetrievePage(Uri uri)
        {
            var url = uri.AbsoluteUri;
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
            return body;
        }
        public void ApplyWebpageToSpecies(string url,Species species)
        {
            var uri = new Uri(url);

            var definition = _definitions.Where(p => p.Host == uri.Host).First();
            definition.ApplyToSpecies(uri, species);
        }
        public List<IScraperDefinition> GetDefinitions()
        {
            return _definitions;
        }
    }
    public interface IScraperDefinition
    {
        string Host { get; set; }
        void ApplyToSpecies(Uri url, Species species);
        string GetDescription();
        string GetThumbnail();
        decimal GetPrice();
        int GetTemperatureMin();
        int GetTemperatureMax();
        decimal GetMaxSize();
        decimal GetPhMin();
        decimal GetPhMax();
        int GetLifespan();
        int GetMinimumGallons();
        string GetPrimaryColor();
        string GetSecondaryColor();
        string GetCareLevel();
        ScrapeableSource SearchByName(string speciesName);
    }
    public class ScrapeableSource
    {
        public IScraperDefinition Definition;
        public string SpeciesName;
        public string WebsiteSource;
    }
}
