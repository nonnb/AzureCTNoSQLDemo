# AzureCTNoSQLDemo
Slides, and code used in Microsoft Azure CT Presentation 12 Mar 2019

NB - these are rough cost and throughput guidelines only, done with contrived data payloads, 
focussed on stuffing as much data into each storage as fast as possible with a single thread (except Queue Storage)
i.e. Do prototyping with your own representative data to get a better feel for your own needs.

Costs, throughputs and features are ever-changing.
And there's many other benefits and features available in most offerings than just 'cost per TB' and cost per IO.

To repeat the code, you'll want an Azure Storage Account, and an Azure SQL Server (I've used Basic). (Combined : 2 ZAR per day)

There's a free 30 day trial of App Service Plan if you host on Linux (which I did - the code is dotnet Core), 
and there's also a free 30 Day Trial for CosmosDB - https://azure.microsoft.com/en-us/blog/try-azure-cosmosdb-for-free/.
