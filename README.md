<p align="center"><img src="https://github.com/PostOShare/IdentityWebApi/assets/17848426/b2a55da5-dc7f-4b33-9de6-e6d0c641a752" alt="logo"></p>

<div align="center">
  
[![GitHub Issues](https://img.shields.io/github/issues/PostOShare/IdentityWebApi.svg)](https://github.com/PostOShare/IdentityWebApi/issues)
[![GitHub license](https://img.shields.io/github/license/PostOShare/IdentityWebApi)](https://github.com/PostOShare/IdentityWebApi/blob/master/LICENSE)
![Static Badge](https://img.shields.io/badge/.net-6)

</div>

IdentityWebApi is a .NET 6 Web API used to manage user data and authentication in PostOShare. It depends on [.NET 6](https://dotnet.microsoft.com/en-us/) and communicates with a [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022) database using [Entity Framework core](https://learn.microsoft.com/en-us/ef/core/).

# Prerequisites

The following need to be available to ensure that the API and the SQL Server database can be setup:

- [.NET 6](https://dotnet.microsoft.com/en-us/)
- [Visual Studio 2022 or higher](https://visualstudio.microsoft.com/downloads/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
- [AWS user account](https://aws.amazon.com/)
  
# Installation

Steps that can be used to setup the API are

```cmd
git clone https://github.com/PostOShare/IdentityWebApi.git

cd IdentityWebApi\IdentityWebApi
dotnet restore
cd ..\EntityORM
dotnet restore
```

The API and the SQL Server instance need to be published to a cloud provider to ensure that remote connections can call the API. [AWS](https://aws.amazon.com/) is used as the provider to publish the API and host the instance. Steps to deploy the API and the instance are

- [Create a VPC to deploy the Identity API](https://github.com/PostOShare/IdentityWebApi/wiki/Create-a-VPC-to-deploy-the-Identity-API)
- [Installation of DB](https://github.com/PostOShare/IdentityWebApi/wiki/Installation-of-DB)
- [Create a Lambda function, HTTP API and deploy the Identity API](https://github.com/PostOShare/IdentityWebApi/wiki/Create-a-Lambda-function,-HTTP-API-and-deploy-the-Identity-API)

# Deployment

The API and the SQL Server instance deployment architecture in AWS is illustrated below:

![Architecture of API deployment](https://github.com/PostOShare/IdentityWebApi/assets/17848426/576dbbbd-b03b-4190-8e3c-0345017da049)

# Dependencies

## Production dependencies

-  Amazon.Lambda.AspNetCoreServer
-  Amazon.Lambda.AspNetCoreServer.Hosting
-  AWS.Logger.AspNetCore
-  MailKit
-  Microsoft.AspNetCore.Authentication.JwtBearer
-  Microsoft.AspNetCore.Identity.EntityFrameworkCore
-  Microsoft.AspNetCore.Identity.UI
-  Microsoft.EntityFrameworkCore.SqlServer
-  Microsoft.EntityFrameworkCore.Tools

## Development dependencies

These dependencies are used only in development:

- Swashbuckle.AspNetCore
- Swashbuckle.AspNetCore.Annotations

# Endpoints of the API

## api/v1/auth/login-identity

This endpoint is used to check whether login details are available.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/login-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "username",
  "password": "password",
  "registeredDate": "2024-02-28T15:01:55.693Z",
  "lastLoginTime": "2024-02-28T15:01:55.693Z",
  "userRole": "userRole",
  "isActive": true
}'
```

### Responses

- 200 - User exists
  	
  **Sample Response**
  ```json
  {
    "refreshToken": "w0czWF0pbdd9hB4h2d1YF+I3ctdzpcfUaOmKagmsy10=",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5MzA0ODk5LCJleHAiOjE3MDkzMDU3OTksImlhdCI6MTcwOTMwNDg5OX0.Hw1GmtW4O245qfD11cHOCQtQ91p2inAOlm6cIjL31rU",
    "result": true,
    "error": ""
  }
  ```

- 400 - Invalid request, Invalid username and/or password (User does not exist)
- 500 - An internal error occurred

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

## api/v1/auth/register-identity

This endpoint is used to register the user data with the given username.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/register-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "password": "password",
  "title": "mr.",
  "firstName": "Edwin",
  "lastName": "Doe",
  "suffix": "",
  "emailAddress": "edwar123@outlook.com",
  "phone": "1234561234",
  "userRole": "user"
}'
```

### Responses

- 201 - User created
- 400 - Invalid request, The given account could not be registered (User exists)
- 500 - An error occurred when adding user 

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An error occurred when adding user"
  }
  ```

  500 - An internal error occurred
  
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

## api/v1/auth/search-identity

This endpoint is used to check whether a user exists. Please note that the values for OTP and Password fields are not validated, but should be passed when making a request. 

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/search-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 0,
  "password": "password"
}'
```

### Responses

- 200 - User exists
  	
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": true,
    "error": ""
  }
  ```

- 400 - Invalid username and/or email (User does not exist), Invalid request
- 500 - An internal error occurred

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

## api/v1/auth/verify-identity

This endpoint is used to generate an OTP, save the OTP to DB and send the OTP to the user's email. Please note that the values for OTP and Password fields are not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/verify-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 0,
  "password": "password"
}'
```

### Responses

- 201 - Created
- 400 - Invalid username (User does not exist), Invalid request
- 500 - An internal error occurred 

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

  500 - InternalServerError (Error when sending email)
  
## api/v1/auth/validate-passcode-identity

This endpoint is used to check if the OTP response sent when validating a user is valid. Please note that the values for Email and Password fields are not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/validate-passcode-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 236784,
  "password": "password"
}'
```

### Responses

- 200 - OTP is valid
- 400 - Invalid username, Invalid OTP, Invalid request
- 500 - Cannot try more than maximum attempts

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "Cannot try more than maximum attempts"
  }
  ```

  500 - An internal error occurred 

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

  500 - An error occurred when updating the request attempt

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An error occurred when updating the request attempt"
  }
  ```

## api/v1/auth/change-credentials-identity

This endpoint is used to update key and salt of a user based on password sent in the request. Please note that the values for Email and OTP fields are not validated, but should be passed when making a request.

### Sample request

```
curl -X 'PATCH' \
  'https://localhost:7224/api/v1/auth/change-credentials-identity' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user",
  "emailAddress": "edwar123@outlook.com",
  "otp": 0,
  "password": "password"
}'
```

### Responses

- 200 - Key and Salt for the user were updated

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": true,
    "error": ""
  }
  ```

- 400 - Invalid request, Invalid username
- 500 - An internal error occurred 

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

  500 - An error occurred when updating password
  
  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An error occurred when updating password"
  }
  ```
  
## api/v1/auth/generate-accessToken

This endpoint is used to create an access token based on the user's refresh token. Please note that the access token is not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/generate-accessToken' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "refreshToken": "w0czWF0pbdd9hB4h2d1YF+I3ctdzpcfUaOmKagmsy10=",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5MzA0ODk5LCJleHAiOjE3MDkzMDU3OTksImlhdCI6MTcwOTMwNDg5OX0.Hw1GmtW4O245qfD11cHOCQtQ91p2inAOlm6cIjL31rU"
}'
```

### Responses

- 201 - Access token was generated

  **Sample Response**
 
  ```json
  {
    "refreshToken": "w0czWF0pbdd9hB4h2d1YF+I3ctdzpcfUaOmKagmsy10=",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5NjUwMzgwLCJleHAiOjE3MDk2NTEyODAsImlhdCI6MTcwOTY1MDM4MH0.D-JUimEo_6UDQvGf_ZggyXM_XoXEIaJ6R_RErMK0qa8",
    "result": true,
    "error": ""
  }
  ```

- 400 - Invalid request
- 500 - An internal error occurred 

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "An internal error occurred"
  }
  ```

## api/v1/auth/validate-accessToken

This endpoint is used to validate an access token. Please note that the refresh token is not validated, but should be passed when making a request.

### Sample request

```
curl -X 'POST' \
  'https://localhost:7224/api/v1/auth/validate-accessToken' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "refreshToken": "w0czWF0pbdd9hB4h2d1YF+I3ctdzpcfUaOmKagmsy10=",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImdkZmdkIiwibmJmIjoxNzA5MzA0ODk5LCJleHAiOjE3MDkzMDU3OTksImlhdCI6MTcwOTMwNDg5OX0.Hw1GmtW4O245qfD11cHOCQtQ91p2inAOlm6cIjL31rU"
}'
```

### Responses

- 200 - Access token is valid
- 400 - Invalid request, Token is expired

  400 - The token is invalid

  ```json
  {
    "refreshToken": "",
    "accessToken": "",
    "result": false,
    "error": "The token is invalid"
  }
  ```
