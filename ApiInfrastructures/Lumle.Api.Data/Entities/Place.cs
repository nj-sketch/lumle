using JsonApiDotNetCore.Models;
using Lumle.Api.Data.Abstracts;
using System.ComponentModel.DataAnnotations;
using JsonApiDotNetCore.Services;
using System;
using System.Collections.Generic;

namespace Lumle.Api.Data.Entities
{
    public class Place: EntityBase, IHasMeta
    {

        [Required]
        [Attr("name")]
        public string Name { get; set; }

        [Required]
        [Attr("location")]
        public string Location { get; set; }

        public Dictionary<string, object> GetMeta(IJsonApiContext context)
        {
            return new Dictionary<string, object> {
            { "copyright", "Copyright 2015 Example Corp." },
            { "authors", new string[] { "Jared Nance" } }
        };
        }
    }
}
