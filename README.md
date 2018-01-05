# firenotes-api

[![CircleCI](https://circleci.com/gh/bolorundurowb/firenotes-api.svg?style=svg)](https://circleci.com/gh/bolorundurowb/firenotes-api)

A .NET Core 2.0 Web API that uses MongoDB to persist data and Json Web Tokens (JWT) for authentication. This API implements email sending on certain import user events. It utilizes `Handlebars` for the email templates and `Mailgun` for the email delivery. The API also uses `ShortId` to generate unique string ids for the notes.

This is a lightweight proof of cconcept and can be forked, cloned and modified to suit your purpose. If this is of help to you, please remember to :star: the repository so others can find it too.

**NOTE**
- All API routes are prepended with `/api/`
- All note routes are protected (as expected).
- Tests are written with the `NUnit` framework, with `FluentAssertions` used for the assertions.

## Building

To build the app, just clone the repo and run

```csharp
dotnet build firenotes-api
```

To run the app, run 
```csharp
dotnet run --project firenotes-api
```

## Testing

To run the tests, do the following:

1. Add a`.env` file to the root of the project, where the `firenotes-api.sln` file is. Add the necessary environment variables e.g `Mailgun`, `Secret` etc

2. Run the test with the following command
```csharp
dotnet test firenotes-api.Tests
```