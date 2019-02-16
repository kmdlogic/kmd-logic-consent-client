iwr http://localhost:5000/swagger/consent/swagger.json -o swagger.json
autorest --input-file=swagger.json --csharp --output-folder=Client --override-client-name=ConsentServiceClient --namespace=Kmd.Logic.ConsentService.ConsoleSample.Client --add-credentials
