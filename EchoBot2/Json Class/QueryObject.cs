using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot2.Json_Class
{
    public class Image
    {
        public string _type { get; set; }
        public string alt { get; set; }
        public string dis_base_link { get; set; }
        public string link { get; set; }
        public string title { get; set; }
    }

    public class ProductType
    {
        public string _type { get; set; }
        public bool master { get; set; }
    }

    public class RepresentedProduct
    {
        public string _type { get; set; }
        public string id { get; set; }
        public string link { get; set; }
    }

    public class Hit
    {
        public string _type { get; set; }
        public string hit_type { get; set; }
        public Image image { get; set; }
        public string link { get; set; }
        public string product_id { get; set; }
        public string product_name { get; set; }
        public ProductType product_type { get; set; }
        public RepresentedProduct represented_product { get; set; }
    }

    public class Value
    {
        public string _type { get; set; }
        public int hit_count { get; set; }
        public string label { get; set; }
        public string presentation_id { get; set; }
        public string value { get; set; }
    }

    public class Refinement
    {
        public string _type { get; set; }
        public string attribute_id { get; set; }
        public string label { get; set; }
        public List<Value> values { get; set; }
    }

    public class SuggestedPhras
    {
        public string _type { get; set; }
        public bool exact_match { get; set; }
        public string phrase { get; set; }
    }

    public class Term
    {
        public string _type { get; set; }
        public bool completed { get; set; }
        public bool corrected { get; set; }
        public bool exact_match { get; set; }
        public string value { get; set; }
    }

    public class SuggestedTerm
    {
        public string _type { get; set; }
        public string original_term { get; set; }
        public List<Term> terms { get; set; }
    }

    public class SearchPhraseSuggestions
    {
        public string _type { get; set; }
        public List<SuggestedPhras> suggested_phrases { get; set; }
        public List<SuggestedTerm> suggested_terms { get; set; }
    }

    public class SortingOption
    {
        public string _type { get; set; }
        public string id { get; set; }
        public string label { get; set; }
    }

    public class QueryObject {
        public string _v { get; set; }
        public string _type { get; set; }
        public int count { get; set; }
        public List<Hit> hits { get; set; }
        public string query { get; set; }
        public List<Refinement> refinements { get; set; }
        public SearchPhraseSuggestions search_phrase_suggestions { get; set; }
        public List<SortingOption> sorting_options { get; set; }
        public int start { get; set; }
        public int total { get; set; }
    }
}
