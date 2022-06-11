using System;
using Azure.Data.Tables;
using System.Data.Services.Common;
using Azure;

namespace Data
{
    [DataServiceKey("PartitionKey", "RowKey")]
    public class DataEntry : ITableEntity
    {
        public DataEntry()
        {
            PartitionKey = DateTime.UtcNow.ToString("MMddyyyy");
            RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
        }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        DateTimeOffset? ITableEntity.Timestamp { get; set; }
        ETag ITableEntity.ETag { get; set; }
        public string Message { get; set; }        public string PhotoUrl { get; set; }
        public string GuestName { get; set; }
        public string ThumbnailUrl { get; set; }

        string ITableEntity.RowKey { get => this.RowKey; set => string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid()); }
        string ITableEntity.PartitionKey { get => this.PartitionKey; set =>  DateTime.UtcNow.ToString("MMddyyyy"); }
    }
}
