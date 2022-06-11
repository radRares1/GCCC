using System;
using System.Collections.Generic;
using Azure;
using Azure.Data.Tables;

namespace Data
{

    public class DataSource
    {
        private static TableServiceClient tableServiceClient;
            
        public DataSource()
        {
            tableServiceClient = new TableServiceClient(Data.Properties.Resources.Storage);
        }

        public IEnumerable<DataEntry> GetEntries()
        {
            TableClient tableClient = tableServiceClient.GetTableClient("tablegcc");
            Pageable<DataEntry> entries = tableClient.Query<DataEntry>();
            return entries;
        }

        public void AddEntry(DataEntry entry) 
        {
            TableClient tableClient = tableServiceClient.GetTableClient("tablegcc");
            tableClient.AddEntity(entry);
        }

        public void UpdateEntry(string partitionKey, string rowKey, string thumbURL)
        {
            TableClient tableClient = tableServiceClient.GetTableClient("tablegcc");
            DataEntry entity = tableClient.GetEntity<DataEntry>(partitionKey, rowKey).Value;
            entity.ThumbnailUrl = thumbURL;
            tableClient.UpdateEntity<DataEntry>(entity, ETag.All);
        }
    }
}
