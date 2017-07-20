using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using S3Model = Amazon.S3.Model;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TriggerFunction {

    //--- Types ---
    public class Hero {

        public class GeoLocation {
            
            [JsonProperty("lon")]
            public double Longitude { get; set; }
            
            [JsonProperty("lat")]
            public double Latitude { get; set; }
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("urlslug")]
        public string UrlSlug { get; set; }

        [JsonProperty("identity")]
        public string Identity { get; set; }

        [JsonProperty("alignment")]
        public string Alignment { get; set; }

        [JsonProperty("eye_color")]
        public string EyeColor { get; set; }

        [JsonProperty("hair_color")]
        public string HairColor { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("gsm")]
        public string Gsm { get; set; }

        [JsonProperty("appearances")]
        public string Appearances { get; set; }

        [JsonProperty("first_appearance")]
        public string FirstAppearance { get; set; }

        [JsonProperty("year")]
        public int Year  { get; set; }

        [JsonProperty("location")]
        public GeoLocation Location { get; set; }

        public override string ToString() {
            return $"{Name} | {UrlSlug} | {Identity} | {Alignment} | {EyeColor} | {HairColor} | {Sex} | {Gsm} | {Appearances} | {FirstAppearance} | {Year} | {Location.Longitude} | {Location.Latitude}";
        }
    }

    public class Function {

        //--- Constants ---
        private const string INDEX_NAME = "heroes";
        private const string TYPE_NAME = "hero";

        //--- Class Fields ---
        private static readonly Uri _esDomain = new Uri("https://TODO");
        
        //--- Methods ---
        public async Task FunctionHandler(S3Event uploadEvent, ILambdaContext context) {
            string bucket = uploadEvent.Records[0].S3.Bucket.Name;
            string objectKey = uploadEvent.Records[0].S3.Object.Key;

            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1)) {
                S3Model.GetObjectRequest request = new S3Model.GetObjectRequest {
                    BucketName = bucket,
                    Key = objectKey
                };
                
                using (S3Model.GetObjectResponse response = await client.GetObjectAsync(request)) 
                using (Stream responseStream = response.ResponseStream) 
                using (StreamReader reader = new StreamReader(responseStream)) {
                    while(reader.Peek() >= 0) {
                        LambdaLogger.Log($"{reader.ReadLine().Replace("\t", " | ")}");
                    }
                }
            }
        }
    }
}
