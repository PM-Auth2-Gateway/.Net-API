using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using PMAuth.Extensions;
using Xunit;

namespace PMAuth.Tests
{
    public class HttpExtensionTests
    {
        [Fact]
        public void AddQuery_SingleQuery_ExpectedQueryMatchesActual()
        {
            //arrange
            string expectedUri = "https://google.com/?name=value";
            Uri baseUri = new Uri("https://google.com");
            
            //act
            string actualUri = baseUri.AddQuery("name", "value").ToString();

            //assert
            actualUri.Should().Be(expectedUri);
        }
        
        [Fact]
        public void AddQuery_DuplicateQuery_QueryValueMatchesLastAddedValue()
        {
            //arrange
            string expectedUri = "https://google.com/?name=value2";
            Uri baseUri = new Uri("https://google.com");
            
            //act
            string actualUri = baseUri.AddQuery("name", "value1").ToString();
            actualUri = baseUri.AddQuery("name", "value2").ToString();

            //assert
            actualUri.Should().Be(expectedUri);
        }
        
        [Fact]
        public void AddQuery_NullQueryParamName_QueryMatchesExpected()
        {
            //arrange
            string expectedUri = "https://google.com/";
            Uri baseUri = new Uri("https://google.com");
            
            //act
            string actualUri = baseUri.AddQuery(null, "value").ToString();
            
            //assert
            actualUri.Should().Be(expectedUri);
        }
        
        [Fact]
        public void AddQuery_NullQueryParamValue_QueryMatchesExpected()
        {
            //arrange
            string expectedUri = "https://google.com/";
            Uri baseUri = new Uri("https://google.com");
            
            //act
            string actualUri = baseUri.AddQuery("name", null).ToString();
            
            //assert
            actualUri.Should().Be(expectedUri);
        }
        
        [Fact]
        public void AddQuery_AddQueriesInLoop_QueryMatchesExpected()
        {
            //arrange
            string expectedUri = "https://google.com/?name1=value1&name2=value2";
            
            Uri baseUri = new Uri("https://google.com");
            Dictionary<string, string> queryParams = new Dictionary<string, string>
            {
                {"name1", "value1"},
                {"name2", "value2"}
            };
            
            //act
            foreach (KeyValuePair<string,string> queryParam in queryParams)
            {
                baseUri = baseUri.AddQuery(queryParam.Key, queryParam.Value);
            }
            string actualUri = baseUri.ToString();
            
            //assert
            actualUri.Should().Be(expectedUri);
        }
    }
}