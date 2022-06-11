using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Data;
using Azure.Data.Tables;

namespace WebRole1
{
    public partial class _Default : Page
    {
        private static string _constring = WebRole1.Properties.Resources.Storage;
        private static BlobServiceClient blob = new BlobServiceClient(_constring);
        private static QueueServiceClient queue = new QueueServiceClient(_constring);

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "GuestBook";
            if (!Page.IsPostBack) { this.Timer1.Enabled = true; }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            if (this.PictureUpload.HasFile)
            {
                //upload original picture to blob storage
                BlobContainerClient blobClient = blob.GetBlobContainerClient("blob-gcc");
                string blobId = string.Format("blob-gcc/image_{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(this.PictureUpload.FileName));
                blobClient.UploadBlob(blobId, this.PictureUpload.FileContent);

                //create entry in table storage
                DataEntry entry = new DataEntry()
                {
                    GuestName = this.Name.Text,
                    Message = this.Message.Text,
                    PhotoUrl = blobClient.Uri.ToString() + "/" + blobId,
                    ThumbnailUrl = blobClient.Uri.ToString() + "/" + blobId
                };
                DataSource ds = new DataSource();
                ds.AddEntry(entry);

                //create and add message
                QueueClient qClient = queue.GetQueueClient("queue-gcc");
                string message = String.Format("{0},{1},{2}", blobId, entry.PartitionKey, entry.RowKey);
                qClient.SendMessage(message);

                //empty data
                this.Name.Text = string.Empty;
                this.Message.Text = string.Empty;

                //update datalist

                this.ImageList.DataBind();
            }
        }
        protected void Timer1_Tick1(object sender, EventArgs e)
        {
            this.ImageList.DataBind();
        }
    }
}