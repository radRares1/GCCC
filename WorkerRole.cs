using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Azure.Storage.Blobs.Specialized;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private static BlobServiceClient blob;
        private static QueueServiceClient queue;
        private static string _constring = WorkerRole1.Properties.Resources.Storage;

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                //connect to the sources
                BlobContainerClient blobClient = blob.GetBlobContainerClient("blob-gcc");
                QueueClient qClient = queue.GetQueueClient("queue-gcc");

                //receive the message and split it
                QueueMessage[] msg = qClient.ReceiveMessages();
                if (msg.ToString() != null)
                {
                    var splitMessage = msg[0].Body.ToString().Split(',');
                    var blobId = splitMessage[0].ToString();
                    var partitionKey = splitMessage[1].ToString();
                    var rowKey = splitMessage[2].ToString();

                    string thumbnailName = System.Text.RegularExpressions.Regex.Replace(blobId, "([^\\.]+)(\\.[^\\.]+)?$", "$1-thumb$2");

                    BlobClient inputBlob = blobClient.GetBlobClient(blobId);
                    var outputBlob = blobClient.GetBlockBlobClient(thumbnailName);
                     //new BlobClient(_constring, blobClient.Name, thumbnailName);
                        //blobClient.GetBlobClient(thumbnailName);
                    

                    using (Stream input = inputBlob.OpenRead())
                    using (Stream output = outputBlob.OpenWrite(true))
                    {
                        //create thumbnail
                        this.ProcessImage(input, output);
                        //create uri
                        string thumbUri = outputBlob.Uri.ToString();
                        //init ds and update the uri in
                        DataSource ds = new DataSource();
                        ds.UpdateEntry(partitionKey, rowKey, thumbUri);

                        qClient.DeleteMessage(msg[0].MessageId, msg[0].PopReceipt);
                    }
                }
                else { System.Threading.Thread.Sleep(1000); } 

            }
            finally
            {
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void ProcessImage(Stream input, Stream output)
        {
            int width, height;
            var originalImg = new Bitmap(input);

            if (originalImg.Width > originalImg.Height)
            {
                width = 128;
                height = 128 * originalImg.Height / originalImg.Width;
            }
            else
            {
                height = 128;
                width = 128 * originalImg.Width / originalImg.Height;
            }

            Bitmap thumbImg = null;
            try
            {
                thumbImg = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(thumbImg))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(originalImg, 0, 0, width, height);
                }
                thumbImg.Save(output, ImageFormat.Jpeg);
            }
            finally { if (thumbImg != null) { thumbImg.Dispose(); } }



        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            //init the storages
            blob = new BlobServiceClient(_constring);
            queue = new QueueServiceClient(_constring);

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
