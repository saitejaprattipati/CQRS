namespace Author.Query.Domain.DBAggregate
{
    public class Languages
    {
        /// <summary>gets or sets the id </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("id")]
        public string id { get; set; }
        /// <summary>gets or sets the LanguageId </summary>
        ///// <value>It is of type integer </value>
        //[JsonProperty("LanguageId")]        
        public int LanguageId { get; set; }

        /// <summary>gets or sets the Name </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("Name")]
        public string Name { get; set; }
        /// <summary>gets or sets the NameinEnglish </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("NameinEnglish")]
        public string NameinEnglish { get; set; }

        /// <summary>gets or sets the LocalisationIdentifier </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("LocalisationIdentifier")]
        public string LocalisationIdentifier { get; set; }

        /// <summary>gets or sets Locale </summary>
        /// <value>It is of type string </value>
        //[JsonProperty("Locale")]
        public string Locale { get; set; }

        /// <summary>gets or sets the RightToLeft</summary>
        /// <value>It is of type bool </value>
        //[JsonProperty("RightToLeft")]
        public bool RightToLeft { get; set; }
    }
}
