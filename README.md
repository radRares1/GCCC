# GCCC
Grid Cluster Cloud Computing - Azure Assignment 

Azure assignment. 

In a **WebRole**: \
Takes as input a name, a message and a picture. \
Display them as output in a list, initally with the full resolution picture, then replaces the picture with its created thumbnail.
Stores the data in an Azure Table, uploads the picture to a Azure Blob Storage \
Creates a Queue Entry in an Azure Queue with the path to the picture stored in the blob along with the Row&Partition key. 

In the **WorkerRole**: \
Continously reads from the Queue 

Takes the first message found, extracts the path from the message, gets the picture, creates a thumbnail out of it, uploads the thumbnail to the Blob and lastly updates the respective entry in the Table with the path to the thumbnail. \
When the thumbnail path is available in the Table Entry, it is replaced by refreshing the binded DataList.
