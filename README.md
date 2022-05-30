# SemperPrecisStageTracker

## Requirements

Change blazor launchSettings for run application over 5002/5003 port

Add specified KeyVault secret for enable production environment, for develop purpose add that in API project secrets

```bash
dotnet user-secrets list
dotnet user-secrets set "adminUsername" "Shooter01"
```

```json
{
  "backDoorPassword": "",
  "recaptchaSiteAuthority": "",
  "recaptchaSiteKey": "",
  "recaptchaToken": "",
  "webPushPrivate": "",
  "webPushPublic": "",
  "webPushUser": "",
  "connectionStrings:SqlDb": "only for migrations / specify just for local develop",
  "adminUsername":"Shooter01"
}
```

In AppService configuration specify the following properties

- **azKVName**: azure keyvault name
- **clazorEndpoint**: front-end application url
- **SqlDb**: inside connection string section