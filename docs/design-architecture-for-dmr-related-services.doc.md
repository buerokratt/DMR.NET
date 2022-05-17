# Architecture design for DMR-related services

## Participants
<br>

![DMR participants](./images/design-architecture-for-dmr-related-services/participants.editable.png)

| **Participant** | **Description**                                                                                                                             | **Scope of development**                     |
|-----------------|---------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------|
| Client          | End-client`s browser, curl request or similar                                                                                               | curl requests or similar, no GUI nor backend |
| Chatbot         | Client`s instance of Bürokratt chatbot service that in this example is used directly by Client                                              | Mock; must push logs to CentOps              |
| Chatbots 1-N    | Identical to "Chatbot" but is not used by Client in this example                                                                            | Mock; must push logs to CentOps              |
| DMR             | Distributed Message Rooms (N number of them) pass on messages between different chatbot instances and Classifier; a dumb pipe by its nature | Actual working service (MVP)                 |
| CentOps         | Central Operating System controlled by Bürokratt`s core team to track and manage the whole ecosystem of Bürokratt                           | Actual working service (MVP)                 |
| Classifier      | A stand-alone service to detect which participant is the correct one to process given (user) input                                          | Mock                                         |                                 |

## Requests to CentOps

> Note CentOps is the central component to turn to by any participant to set up, maintain and/or remove their instances from Bürokratt ecosystem. It also expects, accepts and processes aggregated logs from all participants to monitor the ecosystem as a whole in order to initiate actions when needed.

<br>

![Requests to Centops](./images/design-architecture-for-dmr-related-services/requests-to-centops.editable.png)

### Membership management

| **Service**                                  | **Resource**                                | **Method** | **Requests from**        | **Requests to** | **OpenApi spec** |
|----------------------------------------------|---------------------------------------------|------------|--------------------------|-----------------|------------------|
| [Get a list of all registered participants](https://github.com/buerokratt/CentOps/issues/18)        | /membership-management<br>/membership-management/{participant-type}             | GET        | CentOps, Chatbot, DMR, Classifier | CentOps         |             |
| [Register a membership of a new instance](https://github.com/buerokratt/CentOps/issues/19)      | /membership-management/{participant-type}/{client-id} | POST       | Chatbot, DMR, Classifier                  | CentOps         |             |
| [Update membership information of an instance](https://github.com/buerokratt/CentOps/issues/20) | /membership-management/{participant-type}/{client-id} | PUT        | Chatbot, DMR, Classifier                  | CentOps         |             |
| [Announce revoking membership](https://github.com/buerokratt/CentOps/issues/21)                 | /membership-management/{participant-type}/{client-id} | DELETE     | Chatbot, DMR, Classifier                  | CentOps         |             |

### Status information

| **Service**                                       | **Resource**                 | **Method** | **Requests from**        | **Requests to** | **OpenApi spec** |
|---------------------------------------------------|------------------------------|------------|--------------------------|-----------------|------------------|
| [Get status information of all registered participants](https://github.com/buerokratt/CentOps/issues/23) | /status/{participant-type}             | GET        | CentOps, Chatbot, DMR, Classifier | CentOps         |             |
| [Get status information of a specific participant](https://github.com/buerokratt/CentOps/issues/24)      | /status/{participant-type}/{client-id} | GET        | CentOps, Chatbot, DMR, Classifier | CentOps         |             |
| [Update status information of a specific participant](https://github.com/buerokratt/CentOps/issues/20)   | /status/{participant-type}/{client-id} | PUT        | Chatbot, DMR, Classifier                  | CentOps         |             |

### Planned outages

| **Service**                                      | **Resource**                              | **Method** | **Requests from**        | **Requests to** | **OpenApi spec** |
|--------------------------------------------------|-------------------------------------------|------------|--------------------------|-----------------|------------------|
| [Get a list of all planned outages](https://github.com/buerokratt/CentOps/issues/30)                | /outage                      | GET        | CentOps, Chatbot, DMR, Classifier | CentOps         |                  |
| Update a list of all planned outages             | /outage<br>/outage/{participant-type}                      | PUT        | CentOps                  | CentOps         |                  |
| Get planned outages of a specific participant        | /outage/{participant-type}/{client-id} | GET        | CentOps, Chatbot, DMR, Classifier | CentOps         |                  |
| Update planned outages of a specific chatbot    | /outage/{participant-type}/{client-id} | PUT        | CentOps                  | CentOps         |                  |
| [Announce planned outage to network](https://github.com/buerokratt/CentOps/issues/29)                    | /outage/{participant-type}/{client-id} | POST       | CentOps, Chatbot, DMR, Classifier                  | CentOps         |                  |
| [Notify participants of planned outages](https://github.com/buerokratt/CentOps/issues/31)                    | /outage/{participant-type}/{client-id} | POST       | CentOps                      | CentOps         |                  |
| [Announce of returning to network](https://github.com/buerokratt/CentOps/issues/32)     |  |         |                   |          |                  |


### Sending logs

| **Service**                                 | **Resource** | **Method** | **Requests from**        | **Requests to** | **OpenApi spec** |
|---------------------------------------------|--------------|------------|--------------------------|-----------------|------------------|
| [Send aggregated logs for central monitoring](https://github.com/buerokratt/CentOps/issues/28) |              |            | Chatbot, DMR, Classifier | CentOps         |                  |


## Requests by CentOps

> <br>**_TO CLARIFY:_** On this image, "_many optional_ Chatbots 1-N" and "_exactly one_ Chatbot" are making requests to CentOps. Both "Chatbots 1-N" and "Chatbot" should actually be treated the same way but as we treat "Chatbot" separately throughout this document, such difference is used.<br><br>

<br>

![CentOps notifies about updates](./images/design-architecture-for-dmr-related-services/centops-notifies-about-updates.editable.png)

| **Service**                                                            | **Resource**                                         | **Method** | **Requests from** | **Requests to**          | **OpenApi spec** |
|------------------------------------------------------------------------|------------------------------------------------------|------------|-------------------|--------------------------|------------------|
| Membership registration accepted                                       | /centops-communication/registrations/{client-id}     | POST       | CentOps           | Chatbot, DMR, Classifier |                  |
| Notify participants about having updated information regarding to them | /centops-communication/published-updates/{client-id} | POST       | CentOps           | Chatbot, DMR, Classifier |                  |
| Notify participants about optional software updates                    | /centops-communication/software-updates/optional     | POST       | CentOps           | Chatbot, DMR, Classifier |                  |
| Notify participants about mandatory software updates                   | /centops-communication/software-updates/mandatory    | POST       | CentOps           | Chatbot, DMR, Classifier |                  |
| Notify participants about critical software updates                    | /centops-communication/software-updates/critical     | POST       | CentOps           | Chatbot, DMR, Classifier |                  |
| Update the list of participants                                            | /centops-communication/participants/{participant-type}          | POST       | CentOps           | DMR                      |                  |


# Chat flow
## Initial request from the Client

> <br>Out of scope of DMR development. Use just a simple curl to test mock chats.<br><br>

<br>

![DMR participants](./images/design-architecture-for-dmr-related-services/client-request.editable.png)

| **Service**           | **Resource**     | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|-----------------------|------------------|------------|-------------------|-----------------|------------------|
| Client initiates chat |                  |            |                   |                 |                  |
| Client sends message  | /chats/{chat-id} | POST       | Client            | Chatbot         |                  |
| Client ends chat      |                  |            |                   |                 |                  |

- Client sends a request to any member of Bürokratt`s ecosystem
- Client does not have to know which chatbot is the right one to answer him/her
- Client may ask about different intents within the same chat session knowing (or not) that this particular chatbot is not the one capable of responding
- Regardles of everything described above, the Client must still get a proper response to their request within this same chat session


## Chatbot unable to process the request
<br>

![Chatbot to DMR](./images/design-architecture-for-dmr-related-services/chatbot-to-dmr.editable.png)

| **Service**                      | **Resource**  | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|----------------------------------|---------------|------------|-------------------|-----------------|------------------|
| Send unclassified message to DMR | /unclassified | POST       | Chatbot           | DMR             |                  |

- A local Chatbot that the Client turned to is unable to find an appropriate answer
- In all such cases, an initial request is passed on to DMR which has to know what to do next

> <br>DMR is unable to read the content of `payload`<br><br>


## Chatbot continues an ongoing conversation
<br>

![Chatbot to DMR](./images/design-architecture-for-dmr-related-services/chatbot-to-dmr.editable.png)

| **Service**                               | **Resource**                                      | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|-------------------------------------------|---------------------------------------------------|------------|-------------------|-----------------|------------------|
| Send classified, follow-up message to DMR | /conversations/{participant-id}/{conversation-id} | POST       | Chatbot           | DMR             |                  |

- A local Chatbot is used to continue the conversation even if the response comes from some other Chatbot instances
- `{participant-id}` and `{conversation-id}` are mandatory to be sent by Chatbot in order to prevent keeping state at DMR
- In all such cases, a request is passed on to DMR, which has to know what to do next - in this case, `{participant-id}` is already pre-defined as a recipient, so passing this message to Classifier is unnecessary

> <br>DMR is unable to read the content of `payload`<br><br>


## DMR sends the request to Classifier
<br>

![DMR to Classifier](./images/design-architecture-for-dmr-related-services/dmr-to-classifier.editable.png)

| **Service**                                   | **Resource**           | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|-----------------------------------------------|------------------------|------------|-------------------|-----------------|------------------|
| Send a message payload to be classified       | /inputs                | POST       | DMR               | Classifier      |                  |
| Get the result for already classified content | /inputs/{content-hash} | GET        | DMR               | Classifier      |                  |

- If it`s an initial request (neither classified or responded by another bot), it always gets sent to Classifier
- Classifier stores cached results, which can be accessed by exact match of content hash

> <br>DMR is unable to read the content of `payload`<br><br>


## Classifier responds with a name of a proper participant
<br>

![Classifier to DMR](./images/design-architecture-for-dmr-related-services/classifier-to-dmr.editable.png)

| **Service**                                   | **Resource** | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|-----------------------------------------------|--------------|------------|-------------------|-----------------|------------------|
| Respond to DMR with a classified participant  | /classified  | POST       | Classifier        | DMR             |                  |

- Although Classifier is part of Bürokratt core services, it`s treated as a third-party service
- Classifier responds with a _name_ of the participant (`{participant-id}`) that should be able to respond to given request
- Classifier has no knowledge about what should or will be done with its response afterwards
- Resource would be `/classified` instead of `/conversations` as it´s not up to Classifier to know/decide if its response is for conversations or something else

> <br>Classifier is able to read the content of `payload`<br><br>

> <br>In case of multiple institutions, Classifier makes separate requests for all of them<br><br>

## DMR sends the request to proper participant
<br>

![DMR to third party chatbots](./images/design-architecture-for-dmr-related-services/dmr-to-third-party-chatbots.editable.png)

| **Service**                                          | **Resource**                                      | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|------------------------------------------------------|---------------------------------------------------|------------|-------------------|-----------------|------------------|
| Send a message to an appropriate third-party Chatbot | /conversations/{participant-id}/{conversation-id} | POST       | DMR               | Chatbot 1-N     |                  |

- `{conversation-id}` has been initially passed on by Chatbot (chat initiator), DMR itself does not create/store this kind of state

> <br>CentOps continuously provides DMR information about which institution keywords (`Chatbot_A` = `https://private.chatbot_a/message-from-dmr` in this case) correspond to which endpoints<br><br>

> <br>DMR is unable to read the content of `payload`<br><br>


## Third-party chatbot responds to DMR
<br>

![Third-party chatbot to DMR](./images/design-architecture-for-dmr-related-services/third-party-chatbot-to-dmr.editable.png)

| **Service**           | **Resource**                                      | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|-----------------------|---------------------------------------------------|------------|-------------------|-----------------|------------------|
| Send a message to DMR | /conversations/{participant-id}/{conversation-id} | POST       | Chatbot 1-N       | DMR             |                  |

- Any third-party chatbot acts as any other chatbot would regardless of if they`re requested by an actual Client, DMR or something else
- This response may take from milliseconds to days (or even more)

> <br>DMR is unable to read the content of `payload`<br><br>


## DMR responds to initial request
<br>

![DMR responds to initial request](./images/design-architecture-for-dmr-related-services/dmr-responds-to-initial-request.editable.png)

| **Service**               | **Resource**                                      | **Method** | **Requests from** | **Requests to** | **OpenApi spec** |
|---------------------------|---------------------------------------------------|------------|-------------------|-----------------|------------------|
| Send a message to Chatbot | /conversations/{participant-id}/{conversation-id} | POST       | DMR               | Chatbot         |                  |

- DMR passes its response to Chatbot
- Business-wise, Chatbot treats responses from DMR the same way as it would treat its request to any of its backend services (local database for instance)

> <br>DMR is unable to read the content of `payload`<br><br>

> <br>Request initiator (Chatbot) is able to read the content of `response.content`<br><br>


## Response to Client
<br>

![Response to Client](./images/design-architecture-for-dmr-related-services/response-to-client.editable.png)

| **Service**                     | **Resource**     | **Method**          | **Requests from** | **Requests to** | **OpenApi spec** |
|---------------------------------|------------------|---------------------|-------------------|-----------------|------------------|
| Client asks for message updates | /chats/{chat-id} | GET, SSE, WebSocket | Client            | Chatbot         |                  |

- Local chatbot passes the response to Client
