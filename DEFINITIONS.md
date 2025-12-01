## OAuth2
OAuth 2.0 is an authorization framework that allows an application to access resources on behalf of a user **without requiring their password**.  
Example: Logging into a website using Google or Facebook. The site receives an **access token**, not your password.

---

## OpenID Connect (OIDC)
OpenID Connect is an authentication layer built **on top of OAuth2**.  
It lets applications **verify a user’s identity** using an identity provider (e.g., Google, Microsoft).

OIDC issues:
- **ID Token** → identifies the user  
- **Access Token** → from OAuth2, used to access resources

---

## JWT (JSON Web Token)
JWT is a compact, digitally signed token used for secure information exchange.

It has 3 parts:
1. **Header** – token type and algorithm  
2. **Payload** – claims (userId, roles, expiry)  
3. **Signature** – ensures integrity  

Used for:
- Web API authentication  
- Single sign-on (SSO)  
- Stateless sessions

---

## Dependency Injection (DI)
Dependency Injection is a design pattern where objects are **given** their dependencies instead of creating them internally.

Benefits:
- Easier testing  
- Lower coupling  
- Centralized configuration  

Common in Angular, ASP.NET Core, and Spring Boot.

---

## Rate Limiting
Rate limiting restricts how many requests a client can make to an API within a specific timeframe.  
Example: **100 requests per minute per IP**.

Purpose:
- Prevent abuse  
- Protect server resources  
- Manage load

---

## Throttling
Throttling **slows down** the processing of requests instead of blocking them entirely.

Difference:
- **Rate limit:** “Stop. You hit the limit.”  
- **Throttle:** “Slow down so the system stays stable.”

---

## VPC (Virtual Private Cloud)
A VPC is a private, isolated virtual network in the cloud.  
You control:
- IP ranges  
- Subnets  
- Routing  
- Firewall rules (security groups)

Used to securely host servers, APIs, and databases.

---

## Subnet
A subnet is a smaller network segment inside a VPC.

Example VPC: `10.0.0.0/16`  
Subnets:
- `10.0.1.0/24` (public)  
- `10.0.2.0/24` (private)

Subnets help with organization, routing, and security zoning.

---

## Private Networks
A private network is a network **not exposed to the public internet**.

Characteristics:
- No public IP addresses  
- Requires VPN, bastion host, or internal routing  
- Used for databases and internal services  
