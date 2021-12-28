using ManyHelpers.Strings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ManyHelpers.API {
    public class CosumingHelper {
        private HttpClient _client;
        private string _mediaType;
        private string _baseAdress;


        public CosumingHelper(string baseAdress) {
            _baseAdress = baseAdress;
            var handler = new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (requestMessage, certificate, chain, policyErrors) => true
            };

            _client = new HttpClient(handler) {
                BaseAddress = new Uri(baseAdress)
            };
            _client.Timeout = TimeSpan.FromMinutes(30);
            _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        public CosumingHelper AddcontentType(string mediaTypeHeader = "application/json", string charset = "utf-8", bool withoutalidation = false) {
            _mediaType = mediaTypeHeader;

            if (withoutalidation) {
                return AddCustomHeaders("Content-Type", $"{mediaTypeHeader}; charset={charset}");
            }

            var mediaType = new MediaTypeWithQualityHeaderValue(mediaTypeHeader);
            mediaType.CharSet = charset;
            _client.DefaultRequestHeaders.Accept.Add(mediaType);


            return this;
        }

        public CosumingHelper AddBearerAuthentication(string token, bool convertToBase64 = false) {
            var _token = token;
            if (convertToBase64) {
                _token = StringHelper.Base64Encode(token);
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return this;
        }

        public CosumingHelper AddBasicAuthentication(string userName, string password) {
            var authentication = Encoding.ASCII.GetBytes($"{userName}:{password}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authentication));
            return this;
        }

        public CosumingHelper AddCustomHeaders(string key, string value) {
            _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            return this;
        }

        public async Task<(T result, string statusCode, string message)> GetAssync<T>(string endPoint) {
            var callstr = $"{_baseAdress}{endPoint}";
            var response = await _client.GetAsync(callstr);
            return DeserializeResponse<T>(response);
        }

        public async Task<(TResult result, string statusCode, string message)> PostAsync<T, TResult>(string endPoint, T obj) {
            var url = $"{_baseAdress}{endPoint}";
            var response = await _client.PostAsync(url, ObjectToHttpContent(obj));
            return DeserializeResponse<TResult>(response);
        }

        public async Task<(TResult result, string statusCode, string message)> PutAsync<T, TResult>(string endPoint, T obj) {
            var response = await _client.PutAsync($"{_baseAdress}{endPoint}", ObjectToHttpContent(obj));
            return DeserializeResponse<TResult>(response);
        }

        public async Task<(bool result, string message)> DeleteAsync(string endPoint) {
            var response = await _client.DeleteAsync($"{_baseAdress}{endPoint}");
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) {
                return (true, responseContent);
            } else {
                return (false, responseContent);
            }
        }


        public HttpContent ObjectToHttpContent(object obj) {
            if (obj.GetType().IsPrimitive || obj.GetType() == typeof(string) || obj.GetType() == typeof(decimal)) {
                return new StringContent(obj.ToString());
            }

            HttpContent httpContent = null;
            if (_mediaType == "application/json" || _mediaType == "multipart/form-data") {
                httpContent = SerializeJson(obj, _mediaType);
            }
            if (_mediaType == "application/x-www-form-urlencoded") {
                httpContent = FormUrlCOntent(obj);
            }

            if (_mediaType == "application/xml") {
                httpContent = SerializeXml(obj);
            }

            return httpContent;
        }

        private static HttpContent FormUrlCOntent(object obj) {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var urlContent = new FormUrlEncodedContent(dic);
            return urlContent;
        }

        private static HttpContent SerializeJson(object obj, string contentType) {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return new StringContent(json, Encoding.UTF8, contentType);
        }

        private static HttpContent SerializeXml(object obj) {
            var serializer = new XmlSerializer(obj.GetType());
            var xml = "";

            using (var sww = new StringWriter()) {
                using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww)) {
                    serializer.Serialize(writer, obj);
                    xml = sww.ToString();
                }
            }

            return new StringContent(xml, Encoding.UTF8, "application/xml");
        }

        public (T result, string statusCode, string message) DeserializeResponse<T>(HttpResponseMessage response) {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var statusCode = response.StatusCode.ToString();
            var result = (default(T), statusCode, responseContent);
            try {
                if (response.IsSuccessStatusCode) {
                    if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal)) {
                        result = ((T)Convert.ChangeType(responseContent, typeof(T)), statusCode, responseContent); ;
                    }

                    if (_mediaType == "application/json" || _mediaType == "multipart/form-data"|| _mediaType == "application/x-www-form-urlencoded") {
                        result = (JsonConvert.DeserializeObject<T>(responseContent), statusCode, responseContent);
                    }

                    if (_mediaType == "application/xml") {
                        var serializer = new XmlSerializer(typeof(T));
                        using (TextReader reader = new StringReader(responseContent)) {
                            result = ((T)serializer.Deserialize(reader), statusCode, responseContent);
                        }
                    }

                } else {
                    return result;
                }
            } catch(Exception e) {
                result = (default(T), statusCode, responseContent);
            }

            return result;
        }
    }
}
