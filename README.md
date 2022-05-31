# Introduction

[![DMR CI Pipeline](https://github.com/buerokratt/DMR/actions/workflows/ci-pullrequest-main.yml/badge.svg)](https://github.com/buerokratt/DMR/actions/workflows/ci-pullrequest-main.yml)

A repository for the Distributed Message Room (DMR) component in the Bürokratt project.

This repository contains a .NET implementation of the DMR packaged within a Docker container.

This implementation of DMR will take routing requests and return 202 Accepted.  Processing of these requests will occur asynchronously of the routing request.

## Configuration

Configuration settings are as follows:

Current ```appsettings.json```

```json
  "DmrServiceSettings": {
    "ClassifierUri": "http://127.0.0.1:8080/classify",
    "CentOpsUri": "http://127.0.0.1:8080",
  },
  "MockCentOps": {
    "ChatBots": [ { "Id": "bot1", "Endpoint":  "http://127.0.0.1:8080/chat" }],
  }
```

### DmrServiceSetttings

Name           | Description
-------------- | -------------------
ClassifierUri  | Endpoint of the Classifier service which will receive ```POST```ed messages for classification.
CentOpsUri     | Endpoint of the CentOps service (not yet implemented).

### MockCentOps

This service uses a mocked CentOps service for now.  Bot/endpoint mappings provided by this service comes directly from configuration.

Name           | Description
-------------- | -------------------
ChatBots       | A collection of ```Id/Endpoint``` mappings which describe accessible bots in the system.
