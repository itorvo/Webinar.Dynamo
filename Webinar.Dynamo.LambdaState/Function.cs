using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using Webinar.Dynamo.Domain.Domain;
using Webinar.Dynamo.Domain.Entities;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Webinar.Dynamo.LambdaState
{
    public class Function
    {
        private readonly ICountryDomainService CountryDomainService;
        private readonly IStateDomainService StateDomainService;
        private readonly JsonSerializerSettings JsonSerializerSettings;

        public Function()
        {
            ServiceProvider Provider = new Startup().ServiceProvider;
            CountryDomainService = Provider.GetService<ICountryDomainService>();
            StateDomainService = Provider.GetService<IStateDomainService>();

            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                Formatting = Formatting.Indented
            };
        }

        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            try
            {
                if (request.Path.EndsWith("/state"))
                {
                    return ProcessState(request);

                }
                else if (request.Path.EndsWith("/country"))
                {
                    return ProcessCountry(request);
                }
                else
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = "Request Not Valid"
                    };
                }
            }
            catch (Exception ex)
            {
                lambdaContext.Logger.LogLine(ex.StackTrace);
                APIGatewayProxyResponse response = new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = ex.Message
                };
                return response;
            }
        }

        public APIGatewayProxyResponse ProcessCountry(APIGatewayProxyRequest request)
        {
            return request.HttpMethod switch
            {
                "GET" => GetCountry(),
                "POST" => CreateCountry(request),
                _ => new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = ""
                },
            };
        }

        public APIGatewayProxyResponse GetCountry()
        {
            var result = CountryDomainService.GetAll();
            APIGatewayProxyResponse response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(result, JsonSerializerSettings)
            };
            return response;
        }

        public APIGatewayProxyResponse CreateCountry(APIGatewayProxyRequest request)
        {
            var country = JsonConvert.DeserializeObject<Country>(request.Body);
            var result = CountryDomainService.Add(country);
            APIGatewayProxyResponse response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(result, JsonSerializerSettings)
            };
            return response;
        }

        public APIGatewayProxyResponse ProcessState(APIGatewayProxyRequest request)
        {
            return request.HttpMethod switch
            {
                "POST" => CreateState(request),
                "GET" => GetStates(request),
                "DELETE" => DeleteState(request),
                "PATCH" => UpdateState(request),
                _ => new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = ""
                },
            };
        }

        public APIGatewayProxyResponse GetStates(APIGatewayProxyRequest request)
        {
            string @value = GetParameter(request.QueryStringParameters, "limit");
            int limit = int.Parse(@value ?? "0");

            @value = GetParameter(request.QueryStringParameters, "paginationToken");
            string paginationToken = WebUtility.UrlDecode(@value ?? "{}");

            var result = StateDomainService.GetAll(paginationToken, limit);

            APIGatewayProxyResponse response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(result, JsonSerializerSettings)
            };
            return response;
        }

        private string GetParameter(IDictionary<string, string> parameters, string nameParams)
        {
            return parameters.TryGetValue(nameParams, out string @value) ? @value : null;
        }

        public APIGatewayProxyResponse CreateState(APIGatewayProxyRequest request)
        {
            var state = JsonConvert.DeserializeObject<State>(request.Body);
            var result = StateDomainService.Add(state);
            APIGatewayProxyResponse response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(result, JsonSerializerSettings)
            };
            return response;
        }

        public APIGatewayProxyResponse DeleteState(APIGatewayProxyRequest request)
        {
            string code = request.QueryStringParameters["code"],
                country = request.QueryStringParameters["country"];

            var result = StateDomainService.Remove(country, code);
            APIGatewayProxyResponse response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(result, JsonSerializerSettings)
            };
            return response;
        }

        public APIGatewayProxyResponse UpdateState(APIGatewayProxyRequest request)
        {
            var state = JsonConvert.DeserializeObject<State>(request.Body);

            var result = StateDomainService.Update(state);

            APIGatewayProxyResponse response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(result, JsonSerializerSettings)
            };
            return response;
        }
    }
}
