## User Story
AS a Developer
I WANT a design for services covering all aspects of managing participants of Bürokratt´s ecosystem and communication between them
SO THAT can I use the design to implement DMR itself with all its related services

This user story is for the expansion of requirements and technical design of the Classifier system.

## Acceptance Criteria
- [ ] List all participants involved
- [ ] Cover the flow of managing participants of Bürokratt´s ecosystem
- [ ] Cover the flow where user´s request needs to be classified
- [ ] Cover the flow where classified request gets sent to appropriate bot
- [ ] Cover the flow where a response from a third-party bot ends up at Client´s GUI

## Participants
![DMR participants](./images/dmr-participants.editable.png)

| Participant  | Description                                                                                                                                |
|--------------|--------------------------------------------------------------------------------------------------------------------------------------------|
| Client       | End-client´s browser, curl request or similar                                                                                              |
| Chatbot      | Client´s instance of Bürokratt chatbot service that in this example is used directly by Client                                             |
| Chatbots 1-N | Identical to "Chatbot" but is not used by Client in this example                                                                           |
| DMR          | Distributed Message Rooms (N number of them) pass on messages between different chatbot instances and Classifier. Dumb pipe by its nature. |
| CentOps      | Central Operating System controlled by Bürokratt´s core team to track and manage the whole ecosystem of Bürokratt                          |
| Classifier   | A stand-alone service to detect which participant is the correct one to process given (user) input                                         |
| Message Que  | Out of scope now, potentially valuable to store information about unsuccessful requests within the whole ecosystem                         |

## Cross-functional requirements
- All connections are closed by requester after the request has been made. It means that the requester will not keep up the connection to get the response. A callback URI is provided instead if necessary.
- _TO DO - reference to proper CFR_

## Requests to CentOps
<br>

![Requests to Centops](./images/requests-to-centops.editable.png)

- Chatbots and DMR send CentOps
    - requests to
        - become part of Bürokratt`s ecosystem
        - get instructions for updates
        - suspend their participation as part of Bürokratt`s ecosystem
        - remove themselves from Bürokratt`s ecosystem
    - information about their planned outages
    - aggregated logs of their components with non-sensitive information, including
        - version numbers of components in use
        - HTTP status codes of requests
        - time spent in milliseconds per request

> **_TO CLARIFY:_** On this image, "_many optional_ Chatbots 1-N" and "_exactly one_ Chatbot" are making requests to CentOps. Both "Chatbots 1-N" and "Chatbot" should actually be treated the same way but as we treat "Chatbot" separately throughout this document, such difference is used.

## CentOps notifies participants about updates
<br>

![CentOps notifies about updates](./images/centops-notifies-about-updates.editable.png)

- CentOps sends a trigger to chatbots and DMRs letting them know about available / mandatory updates

## Initial request from the Client
<br>

![DMR participants](./images/client-request.editable.png)

- Client sends a request to any member of Bürokratt´s ecosystem
- Client does not have to know which chatbot is the right one to answer him/her
- Client may ask about different intents within the same chat session knowing (or not) that this particular chatbot is not the one capable of responding
- Regardles of everything described above, the Client must still get a proper response to their request within this same chat session

## Chatbot unable to process the request
<br>

![Chatbot to DMR](./images/chatbot-to-dmr.editable.png)

- A local Chatbot that the Client turned to is unable to find an appropriate answer
- In all such cases, an initial request is passed on to DMR which has to know what to do next

## DMR sends the request to Classifier
<br>

![DMR to Classifier](./images/dmr-to-classifier.editable.png)

- If it´s an initial request (neither classified or responded by another bot), it always gets sent to Classifier

## Classifier responds with a name of a proper participant
<br>

![Classifier to DMR](./images/classifier-to-dmr.editable.png)

- Although Classifier is part of Bürokratt´s core services, it´s treated as a third-party service
- Classifier responds with a _name_ of the participant that should be able to respond to given request
- Classifier has no knowledge about what should or will be done with its response afterwards

## DMR sends the request to proper participant
<br>

![DMR to third party chatbots](./images/dmr-to-third-party-chatbots.editable.png)

- Information about which name provided by Classifier applies to which chatbot  endpoint has been previously provided by CentOps

## Third-party chatbot responds to DMR
<br>

![Third-party chatbot to DMR](./images/third-party-chatbot-to-dmr.editable.png)

- Any third party chatbot acts as any other chatbot would regardless of if they´re requested by an actual Client, DMR or something else
- As a request from DMR states a mandatori response URI (among other things), the response is sent to any of the DMRs currently active
- This response may take from milliseconds to days (or even more)

## DMR responds to initial request
<br>

![DMR responds to initial request](./images/dmr-responds-to-initial-request.editable.png)

- DMR passes its response to Chatbot
- Business-wise, Chatbot treats responses from DMR the same way as it would treat its request to any of its backend services (local database for instance)



## Response to Client
<br>

![Response to Client](./images/response-to-client.editable.png)

- Local chatbot passes the response to Client

