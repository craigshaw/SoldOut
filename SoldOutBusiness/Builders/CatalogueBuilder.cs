using System;
using System.Collections.Generic;
using System.Linq;
using SoldOutBusiness.Models;
using System.IO;
using System.Xml.Serialization;

namespace SoldOutBusiness.Builders
{
    public class CatalogueBuilder : ICatalogueBuilder
    {
        private string _configFilePath;
        private IList<Search> _searches;

        public ICatalogueBuilder Add(IList<Search> searches)
        {
            if(searches == null)
            {
                throw new ArgumentNullException(nameof(searches));
            }

            _searches = searches;

            return this;
        }

        /// <summary>
        /// XML config loader. XML file takes the form,
        /// <?xml version="1.0" encoding="utf-16"?>
        ///  <ArrayOfSearch xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        ///   <Search>
        ///    <Keywords>Lego 76001</Keywords>
        ///    <Description>The Bat Vs.Bane</Description>
        ///   </Search>
        ///   ...
        ///  </ArrayOfSearch> 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ICatalogueBuilder AddFromConfigFile(string filePath)
        {
            if(string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            _configFilePath = filePath;

            return this;
        }

        public Catalogue Build()
        {
            var catalogue = new Catalogue();
            IList<Search> searches = new List<Search>();

            // Load from config
            if(!string.IsNullOrEmpty(_configFilePath) && File.Exists(_configFilePath))
            {
                searches = Deserialise<List<Search>>(File.ReadAllText(_configFilePath));
            }

            // Add any manually addded searches
            if(_searches != null)
            {
                searches = searches.Concat(_searches).ToList();
            }

            catalogue.Searches = searches;

            return catalogue;

        }

        private T Deserialise<T>(string xml)
        {
            var xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(new StringReader(xml));
        }

        public string Serialise<T>(T list)
        {
            var xs = new XmlSerializer(typeof(T));
            var xml = new StringWriter();
            xs.Serialize(xml, list);
            return xml.ToString();
        }
    }
}
