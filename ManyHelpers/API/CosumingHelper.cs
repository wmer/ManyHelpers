using ManyHelpers.API.Events;
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
    public class CosumingHelper : IDisposable {
        private HttpClient _client;
        private string _mediaType;
        private string _baseAdress;

        public event RequisitionEventHandler Requisition;
        public event ResponseEventHandler Response;


        public CosumingHelper(string baseAdress) {
            _baseAdress = baseAdress;

            var handler = new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (requestMessage, certificate, chain, policyErrors) => true
            };

            _client = new HttpClient(handler) {
                BaseAddress = new Uri(baseAdress)
            };

            _client.Timeout = TimeSpan.FromMinutes(600);
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
            var authentication = StringHelper.Base64Encode($"{userName}:{password}");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authentication);
            return this;
        }

        public CosumingHelper AddBasicAuthentication(string token) {
            var authentication = StringHelper.Base64Encode(token);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authentication);
            return this;
        }

        public CosumingHelper AddCustomHeaders(string key, string value) {
            _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
            return this;
        }

        public async Task<(T result, string statusCode, string message, HttpResponseHeaders cabecario)> GetAsync<T>(string endPoint, CancellationToken cancelationToken = default) {
            var callstr = $"{_baseAdress}{endPoint}";
            var response = await _client.GetAsync(callstr, cancelationToken);

            OnRequisition(this, new RequisitionEventArgs(endPoint, "GET", ""));
            return DeserializeResponse<T>(response);
        }

        public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> GetAsync<T, TResult>(string endPoint, T obj) {
            var callstr = $"{_baseAdress}{endPoint}";

            using (var request = new HttpRequestMessage()) {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(callstr);
                request.Content = ObjectToHttpContent(obj, "GET", endPoint);

                var response = await _client.SendAsync(request);


                OnRequisition(this, new RequisitionEventArgs(endPoint, "GET", ""));
                return DeserializeResponse<TResult>(response);
            }

        }

        public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> PostAsync<T, TResult>(string endPoint, T obj) {
            var url = $"{_baseAdress}{endPoint}";
            var response = await _client.PostAsync(url, ObjectToHttpContent(obj, "POST", endPoint));
            return DeserializeResponse<TResult>(response);
        }

        public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> PostWithFormDataAsync<TResult>(string endPoint, FormUrlEncodedContent obj) {
            var response = await _client.PostAsync($"{_client.BaseAddress}{endPoint}", obj);
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            OnRequisition(this, new RequisitionEventArgs(endPoint, "POST", json));

            return DeserializeResponse<TResult>(response);
        }

        public async Task<(TResult result, string statusCode, string message, HttpResponseHeaders cabecario)> PutAsync<T, TResult>(string endPoint, T obj) {
            var response = await _client.PutAsync($"{_baseAdress}{endPoint}", ObjectToHttpContent(obj, "PUT", endPoint));
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


        public HttpContent ObjectToHttpContent(object obj, string verb, string endpoint) {
            if (obj.GetType().IsPrimitive || obj.GetType() == typeof(string) || obj.GetType() == typeof(decimal)) {
                return new StringContent(obj.ToString());
            }

            HttpContent httpContent = null;

            switch (_mediaType) {
                case "application/json" or "multipart/form-data":
                    httpContent = SerializeJson(obj, _mediaType, verb, endpoint);
                    break;
                case "application/x-www-form-urlencoded":
                    httpContent = FormUrlCOntent(obj, verb, endpoint);
                    break;
                case "application/xml":
                    httpContent = SerializeXml(obj, verb, endpoint);
                    break;
            }

            return httpContent;
        }

        private HttpContent FormUrlCOntent(object obj, string verb, string endpoint) {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            OnRequisition(this, new RequisitionEventArgs(endpoint, verb, json));

            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var urlContent = new FormUrlEncodedContent(dic);
            return urlContent;
        }

        private HttpContent SerializeJson(object obj, string contentType, string verb, string endpoint) {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            OnRequisition(this, new RequisitionEventArgs(endpoint, verb, json));

            return new StringContent(json, Encoding.UTF8, contentType);
        }

        private HttpContent SerializeXml(object obj, string verb, string endpoint) {
            var serializer = new XmlSerializer(obj.GetType());
            var xml = "";

            using (var sww = new StringWriter()) {
                using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sww)) {
                    serializer.Serialize(writer, obj);
                    xml = sww.ToString();
                }
            }


            OnRequisition(this, new RequisitionEventArgs(endpoint, verb, xml));
            return new StringContent(xml, Encoding.UTF8, "application/xml");
        }

        public (T result, string statusCode, string message, HttpResponseHeaders cabecario) DeserializeResponse<T>(HttpResponseMessage response) {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var cabecario = response.Headers;
            var statusCode = response.StatusCode.ToString();
            var result = (default(T), statusCode, responseContent, cabecario);
            try {
                if (response.IsSuccessStatusCode) {
                    if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal)) {
                        result = ((T)Convert.ChangeType(responseContent, typeof(T)), statusCode, responseContent, cabecario); ;
                    } else if (_mediaType is "application/json" or "multipart/form-data" or "application/x-www-form-urlencoded") {
                        result = (JsonConvert.DeserializeObject<T>(responseContent), statusCode, responseContent, cabecario);
                    } else if (_mediaType == "application/xml") {
                        var serializer = new XmlSerializer(typeof(T));
                        using (TextReader reader = new StringReader(responseContent)) {
                            result = ((T)serializer.Deserialize(reader), statusCode, responseContent, cabecario);
                        }
                    }

                } else {
                    return result;
                }
            } catch {
                result = (default(T), statusCode, responseContent, cabecario);
            }


            OnResponse(this, new ResponseEventArgs("", "", statusCode, result.Item3));

            return result;
        }


        private void OnRequisition(object sender, RequisitionEventArgs e) {
            Requisition?.Invoke(sender, e);
        }

        private void OnResponse(object sender, ResponseEventArgs e) {
            Response?.Invoke(sender, e);
        }

        public void Dispose() {
            _client.Dispose();
        }
    }
}
