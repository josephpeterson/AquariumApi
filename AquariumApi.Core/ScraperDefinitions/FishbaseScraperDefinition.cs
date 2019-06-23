using AquariumApi.Models;
using Microsoft.Extensions.Logging;
using System;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Linq;
using AquariumApi.Core.Services;

namespace AquariumApi.Core.ScraperDefinitions
{
    public class FishbaseScraperDefinition : IScraperDefinition
    {
        private HtmlNode _body;
        private Uri _uri;
        private ILogger<WebScraperService> _logger;
        private string CareLevel;
        private string PrimaryColor;
        private string SecondaryColor;
        private int MinimumGallons;
        private int Lifespan;
        private int TemperatureMin;
        private int TemperatureMax;

        public string Host { get; set; }

        public FishbaseScraperDefinition(string host, ILogger<WebScraperService> logger)
        {
            _logger = logger;
            Host = host;
        }

        public void ApplyToSpecies(Uri url, Species species)
        {
            _body = WebScraperService.RetrievePage(url);
            _uri = url;


            species.CareLevel = GetCareLevel();
            species.PrimaryColor = GetPrimaryColor();
            species.SecondaryColor = GetSecondaryColor();
            species.MinimumGallons = GetMinimumGallons();
            species.Lifespan = GetLifespan();
            species.PhMin = GetPhMin();
            species.PhMax = GetPhMax();
            species.TemperatureMin = GetTemperatureMin();
            species.TemperatureMax = GetTemperatureMax();
            species.Description = GetDescription();
            species.Thumbnail = GetThumbnail();
            species.MaxSize = GetMaxSize();
            species.Price = GetPrice();

        }

        public string GetCareLevel() => CareLevel;
        public string GetPrimaryColor() => PrimaryColor;
        public string GetSecondaryColor() => SecondaryColor;
        public int GetLifespan() => Lifespan;
        public int GetMinimumGallons() => MinimumGallons;
        public decimal GetMaxSize() {
            var data = _body.QuerySelectorAll(".smallSpace");
            var text = data.ElementAt(3).InnerText.Replace(" ","").ToLower();
            var start = text.IndexOf("maxlength:") + 10;
            var size = text.Substring(start, text.IndexOf("c", start) - start);
            var cm = Convert.ToDouble(size);
            var inc = Convert.ToDecimal(cm * 0.39370);
            return inc;
        }
        public decimal GetPhMax()
        {
            var searchText = "pH range: ";
            var text = _body.QuerySelectorAll(".smallSpace").ElementAt(1).InnerText;
            var start = text.IndexOf(searchText);
            if (start == -1) return 0;
            start += searchText.Length;
            var textRange = text.Substring(start, text.IndexOf(";", start) - start).Split("-");
            return Convert.ToDecimal(textRange.ElementAt(1).Trim());
        }
        public decimal GetPhMin() {
            var searchText = "pH range: ";
            var text = _body.QuerySelectorAll(".smallSpace").ElementAt(1).InnerText;
            var start = text.IndexOf(searchText);
            if (start == -1) return 0;
            start += searchText.Length;
            var textRange = text.Substring(start, text.IndexOf(";", start) - start).Split("-");
            return Convert.ToDecimal(textRange.ElementAt(0).Trim());
        }
        public int GetTemperatureMax() => TemperatureMax;
        public int GetTemperatureMin() => TemperatureMin;

        public string GetDescription()
        {
            var data = _body.QuerySelectorAll(".smallSpace");
            var text = data.ElementAt(4).InnerText.Trim();
            return text;
        }

        public decimal GetPrice() => 0;
        public string GetThumbnail()
        {
            var thumbnailSrc = _body.QuerySelector("#ss-photomap-container").QuerySelector("img").Attributes["src"].Value;
            string host = _uri.Host;
            string prefix = _uri.Scheme;
            return prefix + "://" + host + thumbnailSrc;
        }


        public ScrapeableSource SearchByName(string speciesName)
        {
            var url = "https://aquarium-fish.liveaquaria.com/search?w=" + Uri.EscapeUriString(speciesName);
            var body = WebScraperService.RetrievePage(_uri);
            return new ScrapeableSource()
            {
                Definition = this,
                SpeciesName = speciesName,
                WebsiteSource = speciesName
            };
        }

    }
}
