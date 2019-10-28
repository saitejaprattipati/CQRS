﻿using Author.Core.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Author.Query.Persistence.DTO
{
    public class CountryResult 
    {
        public CountryResult()
        {
            Countries = new List<CountryDTO>();
        }
        public List<CountryDTO> Countries { get; set; }
    }

    public class CountryDTO
    {
        [JsonProperty("pngimagePath")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string PNGImagePath { get; set; } = "";
        [JsonProperty("svgimagePath")]
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string SVGImagePath { get; set; } = "";
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string DisplayName { get; set; } = "";
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string DisplayNameShort { get; set; } = "";
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ProviderName { get; set; } = "";
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string ProviderTerms { get; set; } = "";
        public int Uuid { get; set; }
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Name { get; set; } = "";
        [RegularExpression(Constants.GeneralStringRegularExpression)]
        public string Path { get; set; } = "";
        public bool CompleteResponse { get; set; }

    }
}