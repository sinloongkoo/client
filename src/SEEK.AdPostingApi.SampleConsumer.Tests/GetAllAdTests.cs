﻿using Moq;
using PactNet.Mocks.MockHttpService.Models;
using SEEK.AdPostingApi.Client;
using SEEK.AdPostingApi.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SEEK.AdPostingApi.SampleConsumer.Tests
{
    [TestFixture]
    public class GetAllAdTests : IDisposable
    {
        private readonly IOAuth2TokenClient _oauthClient;

        public GetAllAdTests()
        {
            this._oauthClient = Mock.Of<IOAuth2TokenClient>(
                c => c.GetOAuth2TokenAsync() == Task.FromResult(new OAuth2TokenBuilder().Build()));
        }

        public void Dispose()
        {
            this._oauthClient.Dispose();
        }

        [SetUp]
        public void TestInitialize()
        {
            PactProvider.ClearInteractions();
        }

        [TearDown]
        public void TestCleanup()
        {
            PactProvider.VerifyInteractions();
        }

        [Test]
        public async Task GetAllAdvertisementBelongToRequester()
        {
            const string advertisementId1 = "fa6939b5-c91f-4f6a-9600-1ea74963fbb2";
            const string advertisementId2 = "e6e31b9c-3c2c-4b85-b17f-babbf7da972b";
            OAuth2Token oAuth2Token = new OAuth2TokenBuilder().Build();

            const string advertisementLink = "/advertisement";
            PactProvider.MockService
                .UponReceiving("a request to retrieve API links")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/",
                    Headers = new Dictionary<string, string>
                    {
                        {"Accept", "application/hal+json"},
                        {"Authorization", "Bearer " + oAuth2Token.AccessToken},
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "application/hal+json; charset=utf-8"}
                    },
                    Body = new
                    {
                        _links = new
                        {
                            advertisements = new
                            {
                                href = advertisementLink
                            },
                            advertisement = new
                            {
                                href = advertisementLink + "/{advertisementId}",
                                templated = true
                            }
                        }
                    }
                });

            PactProvider.MockService
                .Given("There is a page of advertisements with no next link")
                .UponReceiving("GET request for all advertisements")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/advertisement/",
                    Headers = new Dictionary<string, string>
                    {
                        {"Authorization", "Bearer " + oAuth2Token.AccessToken},
                        {"Accept", "application/hal+json"}
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "application/vnd.seek.advertisement+json; charset=utf-8"}
                    },
                    Body = new []
                    {

                            new
                            {
                                advertiserId = "9012",
                                jobTitle = "More Exciting Senior Developer role in a great CBD location. Great $$$",
                                jobReference = "JOB12345",
                                _links = new
                                {
                                    self = new
                                    {
                                        href = "/advertisement/" + advertisementId2
                                    }
                                }
                            },
                            new
                            {
                                advertiserId = "9011",
                                jobTitle = "Exciting Developer role in a great CBD location. Great $$",
                                jobReference = "JOB1234",
                                _links = new
                                {
                                    self = new
                                    {
                                        href = "/advertisement/" + advertisementId1
                                    }
                                }
                            }

                    
                }});

            var client = new AdPostingApiClient(PactProvider.MockServiceUri, _oauthClient);

            var advertisements = await client.GetAllAdvertisementAsync();
            Assert.AreEqual(2, advertisements.Properties.Count());

        }
    }
}