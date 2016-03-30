﻿using Xunit;

namespace SEEK.AdPostingApi.Client.Tests
{
    [CollectionDefinition(Name)]
    public class AdPostingApiCollection : ICollectionFixture<AdPostingApiPactService>
    {
        public const string Name = "Ad Posting API Service Collection";
    }
}