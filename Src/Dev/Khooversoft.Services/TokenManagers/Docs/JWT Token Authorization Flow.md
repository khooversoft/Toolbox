# JWT REST API Authorization Flow

The goal is to provide support for using HMAC REST API strategy.  This provides the identification and
verification that the REST API contents and header have not been modified so that the identity specified can
be used to identify the caller.

HMAC is a strategy that provides the ability to sign key content in the REST API request.  For example, key headers
Content-MD5, bearer, client id, etc... can be verified that these values have not been tampered with.  If
Content-MD5 is provided, then this will ensure content of the REST request has not been tampered.

HMAC is based on a symmetric key, which is basically a shared secret that the server and client have.  While
certificates can support symmetric keys, this can reduce the security profile if the key escapes.

To reduce the security risks, the strategy of using JWT tickets to request a secret from the authorization server
is used because these can be based on private / public keys (asymmetric keys) and have lifetime management
features.

> **Notes:**
> - JWT and HMAC does not provide encryption of payload for either JWT or REST API request.  If required, this can be
> provided when constructing the payloads for the REST API or JWT request.  Normally encryption is provided
> by HTTPS (TLS) which in most cases provides a much stronger security profile.

## Flow

1. Client's use a client certificate's private key to make a request for an authorization token from the authorization server.
The private key is validated on the authorization server by using the client's public key.
The client sends subject and issuer (which can be the same) to the authorization server.

1. The authorization server uses a private key to verify the signature of the authorization request
 
   1. If the request is rejected, the service responds with a forbid.
   1. If the request is accepted, the service responds with a JWT ticket with a random generated secret to be used
in the HMAC request.  The JWT response tick will also have a lifetime time value that the generated secret
will be accepted.  The server's uses its private key to sign the JWT ticket.  The client use the associated public key
to validate the signature of the JWT ticket signed by the server.

1. When the JWT response ticket expires, a new one will need to be requested.  Go to the start of the flow.

Once the HMAC secret has been received from the authorization server, the client uses the secret for
all required REST API calls until the secret expires.

- Setting a lifetime to the authorization ticket is important to limit the security risk if the token escapes.


### Primary Classes

> #### TokenClientManager
> 
> Provides support for the client.  It creates JWT token request and makes a request to the server.  The
> request REST send the request token in the payload, not the URI as a query string.
> 
> 1. **Create Authorization Token Request**: Create a JWT authorization token request.  Uses the public key
> of the certificate to sign the JWT ticket.
> 
> 1. **Parse the authorization token**.  When a token is received from the server, the token is parsed and
> the details are returned.  The private key is used to verify the signature.
> 
> **Notes:**
> - Sending the request as part of the URI as a query string is not advisable because this allows the JWT
> request ticket to escape.  Using SSL, the ticket is encrypted as part of the payload.


> #### TokenManager
> 
> The token manager provides the server side support for the authorization flow.
> 
> 1. Validate authorization request token and create authorization token if the request is valid.  This manager
> will also update the identity with the web key issued (random secret to be used by HMAC calls).


### Certificates

1. There are two models available when dealing with certificates.  Certificates are used to identify either the
client or the server.  The normal model is the client has a public / private key and the server has another.  When
the client signs a request for the authorization server, it uses it's private key.  The server uses the client's public
key to verify the signature.  When the server response to the request, it uses's the server's private key
and the client uses the server's public key to verify the signature.

    There are 2 models for handling certificates.

    - The server must have at least one of its own private and public keys.  The clients are given the server's
public key to verify the signature of the server.  The client has the same where the server is given the
client's public keys.

    - The first model requires that each client has their own private / public keys, and the server must
have a collection of client's public keys for verification.  Too keep the security profile very secure,
this model should be adopted if there are third party services or the environment is not closed, such as in
a VNET or data center.

    - If the environment is a closed system (no third party services) there is the possibility of client's
sharing a single or less number of certificates.  This reduces the work necessary for certificate management
but can reduce the security profile but may not be required in a closed system.  This is the second model.

2. Certificates will expire and must be updated on both the server and the client.  To support the
ability to update certificates using rolling updates.  If the system is small, rolling updates may not be
required but as the system get's bigger or more complex, updating all certificates at the same time
is not practical.

    The model of rolling out certificates requires the following support and processes.

    1. Both the client and server must support multiple certificates.  The server must already have this
support given the requirements to handle multiple clients.
    1. The certificate to use must be identifiable.  The system uses the certificate's thumbprint that
is passed via the KID property (Key id) in the JWT ticket.

