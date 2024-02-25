<p align="center"><img src="https://raw.githubusercontent.com/aditya1962/PostOShare/master/public/images/icons/logo.png" alt="logo"></p>

[![GitHub Issues](https://img.shields.io/github/issues/PostOShare/IdentityWebApi.svg)](https://github.com/PostOShare/IdentityWebApi/issues)
[![GitHub license](https://img.shields.io/github/license/PostOShare/IdentityWebApi)](https://github.com/PostOShare/IdentityWebApi/blob/master/LICENSE)
![Static Badge](https://img.shields.io/badge/.net-6)

IdentityWebApi is a .NET 6 Web API used to manage user data and authentication in PostOShare. It depends on [.NET 6](https://dotnet.microsoft.com/en-us/) and communicates with a [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-2022) using [Entity Framework core](https://learn.microsoft.com/en-us/ef/core/).

# Installation

Steps that can be used to setup the API are

```git
git clone https://github.com/PostOShare/IdentityWebApi.git

cd IdentityWebApi\IdentityWebApi
dotnet restore
cd ..\EntityORM
dotnet restore
```
