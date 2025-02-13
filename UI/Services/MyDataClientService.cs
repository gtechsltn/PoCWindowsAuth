﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WebApi;

namespace UI.Services
{
    public class MyDataClientService
    {
        private readonly IConfiguration _configurations;
        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public MyDataClientService(
            IConfiguration configurations,
            IHttpClientFactory clientFactory)
        {
            _configurations = configurations;
            _clientFactory = clientFactory;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<List<string>> GetMyData()
        {
            try
            {
                var client = _clientFactory.CreateClient("windowsAuthClient");
                client.BaseAddress = new Uri(_configurations["MyApiUrl"]);

                var response = await client.GetAsync("api/MyData");
                if (response.IsSuccessStatusCode)
                {
                    var data = await JsonSerializer.DeserializeAsync<List<string>>(
                    await response.Content.ReadAsStreamAsync());

                    return data;
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}, Message: {error}");

            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }

        public async Task<List<PersonCity>> Suggest(string term)
        {
            try
            {
                var client = _clientFactory.CreateClient("windowsAuthClient");
                client.BaseAddress = new Uri(_configurations["MyApiUrl"]);

                var response = await client.GetAsync($"api/AutoCompleteSearch?term={term}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await JsonSerializer.DeserializeAsync<List<PersonCity>>(
                    await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

                    return data;
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}, Message: {error}");

            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
