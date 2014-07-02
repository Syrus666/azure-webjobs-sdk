﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dashboard.Data
{
    public class JsonVersionedDocumentStore<TDocument> : IVersionedMetadataDocumentStore<TDocument>
    {
        private static readonly JsonSerializerSettings _settings =
            JsonConcurrentDocumentStore<TDocument>.JsonSerializerSettings;

        private readonly IVersionedMetadataTextStore _innerStore;

        public JsonVersionedDocumentStore(IVersionedMetadataTextStore innerStore)
        {
            _innerStore = innerStore;
        }

        internal static JsonSerializerSettings JsonSerializerSettings
        {
            get { return _settings; }
        }

        public IEnumerable<VersionedMetadata> List(string prefix)
        {
            return _innerStore.List(prefix);
        }

        public VersionedMetadataDocument<TDocument> Read(string id)
        {
            VersionedMetadataText textItem = _innerStore.Read(id);

            if (textItem == null)
            {
                return null;
            }

            TDocument document = JsonConvert.DeserializeObject<TDocument>(textItem.Text);

            return new VersionedMetadataDocument<TDocument>(textItem.ETag, textItem.Metadata, textItem.Version,
                document);
        }

        public bool CreateOrUpdateIfLatest(string id, IDictionary<string, string> metadata, TDocument document)
        {
            string text = JsonConvert.SerializeObject(document, _settings);

            return _innerStore.CreateOrUpdateIfLatest(id, metadata, text);
        }

        public bool UpdateOrCreateIfLatest(string id, IDictionary<string, string> metadata, TDocument document)
        {
            string text = JsonConvert.SerializeObject(document, _settings);

            return _innerStore.UpdateOrCreateIfLatest(id, metadata, text);
        }

        public bool UpdateOrCreateIfLatest(string id, IDictionary<string, string> metadata, TDocument document,
            string currentETag, DateTimeOffset currentVersion)
        {
            string text = JsonConvert.SerializeObject(document, _settings);

            return _innerStore.UpdateOrCreateIfLatest(id, metadata, text, currentETag, currentVersion);
        }

        public bool DeleteIfLatest(string id, DateTimeOffset deleteThroughVersion)
        {
            return _innerStore.DeleteIfLatest(id, deleteThroughVersion);
        }

        public bool DeleteIfLatest(string id, DateTimeOffset deleteThroughVersion, string currentETag,
            DateTimeOffset currentVersion)
        {
            return _innerStore.DeleteIfLatest(id, deleteThroughVersion, currentETag, currentVersion);
        }
    }
}