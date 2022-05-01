# SemperPrecisStageTracker

## Requirements

Add specified KeyVault secret for enable production environment, for develop purpose add that in API project secrets

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