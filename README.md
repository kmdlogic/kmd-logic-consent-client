# KMD Logic Consent Client

A dotnet client library for managing distributed consent via the Logic platform.

## The purpose of the Consent API

To comply GDPR, it is becoming common practice to request consent from users to store data and perform actions on their behalf. The Logic Consent service can manage these consent details on your behalf. This is particularly useful when the consent is shared by disparate systems.

### How Consent is Defined

All consents are collected within a Consent Group. This group defines rules about what consent can be collected and which members play what role in regards to the consent. For example, perhaps only one member can create consents but any member can read the details.

A single Logic Subscription is the owner of the Consent Group and is responsible for administering its details. They define:

- The format of the consent key (e.g. a CPR number)
- A defined list of scopes
- A defined list of members

A scope is anything for which consent can be given.

Examples are:

- the right to access the citizens upcoming meetings
- the right to create a meeting on their behalf
- the right to send them emails

A member is a participant in a Consent Group.

They can be allocated any of the following permissions:

- Read: Can retrieve the details of consent by key
- Write: Can create or update the consent granted
- Delete: Can delete the consent details (i.e. revoke all consent)

Members may share the same SubscriptionId or be part of different Logic Subscriptions.

### Interacting With The Consent API

When accessing the Consent API you must specify the ConsentGroupId plus your own SubscriptionId and Member name. Only authorized users of the owner Subscription should use the owners' SubscriptionId. Your SubscriptionId and Member name is used to determine your access rights.

### Updating Consent

Updating consent is a simple POST operation. If no scopes are provided then all scopes are assumed. If no members are specified then all members have access. Any existing consents granted are replaced.

### Adding New Members and Scopes

If a new member they are not given access to any existing consents.

If a new scope is added, this is not automatically added to existing consents.

## How to use this client library

In projects or components where you need to access the register, add a NuGet package reference to [Kmd.Logic.Consent.Client](https://www.nuget.org/packages/Kmd.Logic.Consent.Client).

The simplest example to get consent details is:

```csharp
using (var httpClient = new HttpClient())
using (var tokenProviderFactory = new LogicTokenProviderFactory(configuration.TokenProvider))
{
    var consentClient = new ConsentClient(httpClient, tokenProviderFactory, configuration.Consent);
    var consent = await consentClient.GetConsentAsync(key, scope).ConfigureAwait(false);
}
```

The `LogicTokenProviderFactory` authorizes access to the Logic platform through the use of a Logic Identity issued client credential. The authorization token is reused until it expires. You would generally create a single instance of `LogicTokenProviderFactory`.

The `ConsentClient` accesses the Logic Consent service.

## How to configure the Consent client

Perhaps the easiest way to configure the Consent client is from Application Settings.

```json
{
  "TokenProvider": {
    "ClientId": "",
    "ClientSecret": "",
    "AuthorizationScope": ""
  },
  "Consent": {
    "SubscriptionId": "",
    "ConsentGroupId": ""
  }
}
```

To get started:

1. Create a subscription in [Logic Console](https://console.kmdlogic.io). This will provide you the `SubscriptionId`.
2. Request a client credential. Once issued you can view the `ClientId`, `ClientSecret` and `AuthorizationScope` in [Logic Console](https://console.kmdlogic.io).
3. Create a consent group. This will give you the `ConsentGroupId`.
4. You should add at least one member to the consent group using your own `SubscriptionId`.

## Sample application

A simple console application is included to demonstrate how to call Logic Consent API. You will need to provide the settings described above in `appsettings.json`.

When run you should see the consent details for the nominated key printed to the console.
