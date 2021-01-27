# Bread Winner

Lightweight implementation of producer consumer paradigm in C#.

This library relies heavily on threads. TPL should be avoided when using this library, even at the cost of synchronously waiting tasks when no synchronous API is available. 

Further details [here][justification], but the general idea is that the purpose of this library is avoiding to use the Managed Thread Pool. This is epescially useful in the context of Web API.

Every worker instance uses it's own thread, therefore use with caution at your own risk.

## Installing

To install, you can use the related nuget package.
```powershell
Install-Package BreadWinner
```

## Wiki
For further details, please refer to the [Wiki][home]. Some usages can be found in the projects in the samples folder of the repo.

[home]: https://github.com/gugu91/bread-winner/wiki
[justification]: https://github.com/gugu91/bread-winner/wiki#justification
