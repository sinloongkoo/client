﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEEK.AdPostingApi.Client.Hal;

namespace SEEK.AdPostingApi.Client.Resources
{
    public class AdvertisementListResource : HalResource, IEnumerable<EmbeddedAdvertisementResource>
    {
        internal AdvertisementListEmbeddedResources Embedded { get; set; }

        internal class AdvertisementListEmbeddedResources
        {
            public IEnumerable<EmbeddedAdvertisementResource> Advertisements { get; set; }
        }

        public IEnumerator<EmbeddedAdvertisementResource> GetEnumerator()
        {
            return this.Embedded.Advertisements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Embedded.Advertisements.GetEnumerator();
        }

        public async Task<AdvertisementListResource> NextPageAsync()
        {
            if (Eof)
            {
                throw new NotSupportedException("There are no more results");
            }

            return await this.GetResourceAsync<AdvertisementListResource>(this.GenerateLink("next"));
        }

        public bool Eof => (this.Links == null) || !this.Links.ContainsKey("next");
    }
}