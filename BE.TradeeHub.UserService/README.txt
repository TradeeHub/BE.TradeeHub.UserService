## Open Terminal and run this will generate the UserServiceSubgraph.fsp

dotnet fusion subgraph pack -w BE.TradeeHub.UserService  


## Then run the below code to the file in the fusion project in the gateway file with the other graphs 

dotnet fusion compose -p ../BE.TradeeHub.Fusion/BE.TradeeHub.Fusion/gateway -s BE.TradeeHub.UserService
