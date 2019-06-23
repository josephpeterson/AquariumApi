using AquariumApi.Models;
using Microsoft.Extensions.Logging;
using System;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Linq;
using AquariumApi.Core.Services;

namespace AquariumApi.Core.ScraperDefinitions
{
    public class LiveAquariaScraperDefinition : IScraperDefinition
    {
        private HtmlNode _body;
        private Uri _uri;
        private string stats_str;
        private ILogger<WebScraperService> _logger;
        private string CareLevel;
        private string PrimaryColor;
        private string SecondaryColor;
        private int MinimumGallons;
        private int Lifespan;
        private decimal MaxSize;
        private decimal PhMin;
        private decimal PhMax;
        private int TemperatureMin;
        private int TemperatureMax;

        public string Host { get; set; }

        public LiveAquariaScraperDefinition(string host, ILogger<WebScraperService> logger)
        {
            _logger = logger;
            Host = host;
        }

        public void ApplyToSpecies(Uri url, Species species)
        {
            _body = WebScraperService.RetrievePage(url);
            _uri = url;

            ParseGeneralStats();

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
            species.Price = GetPrice();

        }

        private void ParseGeneralStats()
        {
            //These are where other things are stored
            var statsContainer = _body.QuerySelector(".quick_stats_container");
            var statsData = statsContainer.NextSibling;
            var facts = statsContainer.InnerText.Replace("\t", "").Split("\r\n").ToList().Where(f => f != string.Empty && f != "\n" && f != "\r");

            for (var i = 0; i < facts.Count(); i += 2)
            {
                var label = facts.ElementAt(i);
                var value = facts.ElementAt(i + 1);

                switch (label)
                {
                    case "Max. Size":
                        //Fix size
                        value = value.Replace("&frac34;", ".75")
                            .Replace("&frac14;", ".25")
                            .Replace("&frac12;", ".50")
                            .Replace("\"", "");
                        try
                        {
                            MaxSize = Convert.ToDecimal(value.Trim());
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
                        try
                        {
                            MinimumGallons = Convert.ToInt16(value.Substring(0, value.IndexOf(" gallons")).Trim());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation("[WebScraper] Error parsing minimum gallons");
                            _logger.LogError(ex.ToString());
                        }
                        break;
                    case "Color Form":
                        //Fix size
                        var colors = value.Split(",");
                        if (colors.Count() > 0)
                            PrimaryColor = colors.ElementAt(0).Trim();
                        if (colors.Count() > 1)
                            SecondaryColor = colors.ElementAt(1).Trim();
                        break;
                    case "Care Level":
                        //Fix size
                        CareLevel = value.Trim();
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

                            TemperatureMax = int.Parse(tempMax);
                            TemperatureMin = int.Parse(tempMin);
                            PhMin = Convert.ToDecimal(phMin);
                            PhMax = Convert.ToDecimal(phMax);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation("[WebScraper] Error parsing water conditions");
                            _logger.LogError(ex.ToString());
                        }

                        break;
                    default:
                        break;
                }

            }
        }

        public string GetCareLevel() => CareLevel;
        public string GetPrimaryColor() => PrimaryColor;
        public string GetSecondaryColor() => SecondaryColor;
        public int GetLifespan() => Lifespan;
        public int GetMinimumGallons() => MinimumGallons;
        public decimal GetMaxSize() => MaxSize;
        public decimal GetPhMax() => PhMax;
        public decimal GetPhMin() => PhMin;
        public int GetTemperatureMax() => TemperatureMax;
        public int GetTemperatureMin() => TemperatureMin;

        public string GetDescription()
        {
            var description = _body.QuerySelector(".product_overview").QuerySelector(".overview-content").InnerText.Replace("\r", " ").Replace("\n", " ").Trim();
            return description.Substring(0, description.Length > 600 ? 600 : description.Length);
        }

        public decimal GetPrice()
        {
            var price = _body.QuerySelector(".product_selection_container").QuerySelector("button").Attributes["data-list_price"].Value;
            return Convert.ToDecimal(price);
        }
        public string GetThumbnail()
        {
            var thumbnailSrc = _body.QuerySelector(".product-image").QuerySelector("img").Attributes["src"].Value;
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
