# MCommunity API Documentation

> **⚠️ Important Update**: This documentation has been corrected to match the actual Swagger specification at https://mcommunity.umich.edu/api/doc/schema/swagger-ui/. Key field name corrections:
> - Person DN: Use `entry_dn` (not `dn`)
> - Surname: Use `surname` (not `sn`)
> - Affiliations: Use `umichInstRoles` (not `umichAffiliation`)
> - Email, phone, and title fields are **arrays** not strings
> - Search response returns `{"entries": [...], "signals": "..."}` not a plain array

## Overview

The MCommunity API is a RESTful web service that allows IT professionals to:
- Look up information about people in the MCommunity Directory
- Create and edit MCommunity groups

**Base URL**: `https://mcommunity.umich.edu/api/`

**Documentation**: https://documentation.its.umich.edu/node/3955

**Swagger Documentation**: https://mcommunity.umich.edu/api/doc/schema/swagger-ui/

**Example Scripts**: https://github.com/umich-iam/iam-api-examples

## Setup Requirements

### 1. Obtain an Application ID

Contact the ITS Service Center to obtain an MCommunity API application ID. Provide:
- Department or team name
- Short name of your application or usage
- Two or more contacts (uniqname or group name, and phone numbers)
- Types of objects you'll work with (users, groups, or both)
- For users, any special access needed to attributes (e.g., private data, UMID)

**Application ID Format**: `cn=mcapi-dept-usage,ou=apiusers,o=services`

### 2. Data Privacy Settings

People can set privacy levels for their data:
- **Public**: Application IDs can see this information
- **UM Only**: Application IDs **cannot** see this, but visible in web application
- **Private**: Neither application IDs nor web users can see this

If you need access regardless of privacy settings, include a business case in your application ID request.

## Authentication

### Two-Step Process

1. Present application ID and password to receive an API access token
2. Present the API access token in the Authorization HTTP header when calling endpoints

### Obtaining Access Token

**Endpoint**: `POST /token/`

**Request Body**:
```json
{
  "username": "application id DN",
  "password": "password"
}
```

**Response**:
```json
{
  "access": "access token",
  "refresh": "refresh token"
}
```

### Refreshing Access Token

**Endpoint**: `POST /token/refresh/`

**Request Body**:
```json
{
  "refresh": "refresh token"
}
```

**Response**:
```json
{
  "access": "access token"
}
```

**Important**: You can only refresh your token once. After the refreshed token expires, you must obtain a new token.

### Using Access Token

Include in the Authorization header for all API calls:
```
Authorization: Bearer {token}
```

### Unauthenticated Access

Currently supported for searching public identity information only. Will not retrieve:
- Private information
- Information about groups

## Directory Basics

### LDAP Structure

MCommunity uses an LDAP v3 compliant directory. Objects have:
- **Distinguished Name (DN)**: Unique identifier
- **Attributes**: Information about the object
- **Organizational Units (OUs)**: Containers for objects

### Distinguished Name Formats

**Users**:
```
uid=uniqname,ou=People,dc=umich,dc=edu
```
Example: `uid=bjensen,ou=People,dc=umich,dc=edu`

**Groups**:
```
cn=group name,ou=User Groups,ou=Groups,dc=umich,dc=edu
```
Example: `cn=iamGroupsWSTest1,ou=User Groups,ou=Groups,dc=umich,dc=edu`

**Note**: Be aware of spaces in "User Groups" - quote appropriately in your code.

## Identities Endpoints

### Search for Identities

**Endpoint**: `POST /people/find/`

**Request Body Parameters**:
- `searchParts`: Array of search criteria, each containing:
  - `attribute`: Attribute name to search
  - `value`: Value to search for
  - `searchType`: One of:
    - `start`: Attribute value starts with string
    - `end`: Attribute value ends with string
    - `contain`: Attribute value contains string
    - `exact`: Attribute value exactly matches string
- `logicalOperator`: Boolean operator (`AND` or `OR`)
- `attributes`: Array of attributes to return, or `"*"` for all
- `numEntries`: Maximum entries to return (max 350)

**Example - Search for uniqnames starting with "foo" or "bar"**:
```json
{
  "searchParts": [
    {
      "attribute": "uid",
      "value": "foo",
      "searchType": "start"
    },
    {
      "attribute": "uid",
      "value": "bar",
      "searchType": "start"
    }
  ],
  "numEntries": 10,
  "logicalOperator": "OR",
  "attributes": ["dn", "uid"]
}
```

### Retrieve User Data

**Endpoint**: `GET /people/<uniqname>/`

Returns all attributes visible to your application ID. See Swagger documentation for complete attribute list.

### Retrieve vCard Data

**Endpoint**: `GET /people/<uniqname>/vcard/`

Returns the person's vCard data if available and accessible.

### Retrieve Name Coach Data

**Endpoint**: `GET /people/<uniqname>/namecoach/`

Returns the person's Name Coach data if available and accessible.

### Troubleshooting Missing Data

If you don't see expected data:
1. Check that your application ID has permission to view that data
2. If using `/people/find/`, ensure you're specifying attributes in the "attributes" keyword
3. Contact ITS Service Center if issues persist

## Groups Endpoints

### Prerequisites

Your application ID must be made an owner of the group to access and manage it. This can be done:
- From the web application (typical)
- Using the API if you have another application ID that is currently a group owner

### Get Group Information

**Endpoint**: `GET /groups/<groupname>/`

Returns information about the specified group.

### Renew Group

**Endpoint**: `POST /groups/<groupname>/renew/`

**Request Body**: None

Renews a group. Your application ID must be a group owner.

### Expire Group (Soft Delete)

**Endpoint**: `POST /groups/<groupname>/expire/`

**Request Body**:
```json
{
  "days": 7
}
```

Expires (soft deletes) a group. Equivalent to placing the group in trash in the web application. Specify number of days before permanent deletion.

### Create Group

**Endpoint**: `POST /groups/`

**Required Attributes**:
- `cn`: Group name (up to 60 characters)
  - Allowed: letters, numbers, spaces, underscores, hyphens, single quotes
  - Must be unique (case-insensitive)
- `umichGroupEmail`: Local part of email address (becomes `<value>@umich.edu`)
- `owner`: Array of group owner DNs

**Example**:
```json
{
  "cn": "this is my fun-fun-fun group",
  "umichGroupEmail": "funfunfun",
  "owner": [
    "uid=jbensen,ou=People,dc=umich,dc=edu",
    "uid=bjensen,ou=People,dc=umich,dc=edu"
  ],
  "umichDescription": "This describes the fun of this group"
}
```

### Delete Group

**Endpoint**: `DELETE /groups/<groupname>/`

You must be an owner of the group.

### Replace Group Attributes

**Endpoint**: `PATCH /groups/<groupname>/`

**Request Body**: Contains attributes and values to replace. Use `null` to remove an attribute.

**Warning**: Avoid replacing multi-valued attributes with large number of values - use the multi-valued attribute endpoint instead to prevent infrastructure load.

**Example**:
```json
{
  "umichDescription": "This is a new description",
  "umichAutoReply": null
}
```

### Add/Remove Multi-Valued Attribute Values

**Endpoint**: `POST /groups/<groupname>/<attribute>/`

**Supported Attributes**:
- `cn` (cannot remove naming cn)
- `member`
- `groupMember`
- `owner`
- `rfc822mail`
- `umichPermittedSenders`
- `umichPermittedSendersDomains`
- `umichReceiveDisallowedMessages`

**Example - Add and remove members**:
```json
{
  "add": [
    "uid=bjensen,ou=People,dc=umich,dc=edu",
    "uid=vkg,ou=People,dc=umich,dc=edu"
  ],
  "delete": [
    "uid=hable,ou=People,dc=umich,dc=edu"
  ]
}
```

**Example - Remove all members**:
```json
{
  "delete": []
}
```

## Best Practices

1. **Authentication**: Cache and reuse access tokens until they expire, then refresh once before obtaining a new token
2. **Group DNs**: Always be aware of spaces in Distinguished Names and quote appropriately
3. **Privacy**: Request special access permissions upfront if needed for your use case
4. **Multi-valued Attributes**: Use the specific multi-valued attribute endpoint instead of PATCH for large lists
5. **Search Limits**: Maximum 350 entries per search request
6. **Group Names**: Remember that group name comparison is case-insensitive

## Error Handling

- Check application ID permissions if data is missing
- Verify attribute specifications in search requests
- Contact ITS Service Center for unresolved issues

## Example Use Cases

### Look up a user by uniqname
```
GET /people/bjensen/
Authorization: Bearer {token}
```

### Search for users in a department
```
POST /people/find/
Authorization: Bearer {token}

{
  "searchParts": [
    {
      "attribute": "ou",
      "value": "Engineering",
      "searchType": "exact"
    }
  ],
  "logicalOperator": "AND",
  "attributes": ["uid", "displayName", "mail"],
  "numEntries": 100
}
```

### Add members to a group
```
POST /groups/mygroup/member/
Authorization: Bearer {token}

{
  "add": [
    "uid=user1,ou=People,dc=umich,dc=edu",
    "uid=user2,ou=People,dc=umich,dc=edu"
  ]
}
```

## C# Implementation Examples

### MCommunity API Client

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class MCommunityApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _applicationId;
    private readonly string _password;
    private const string BaseUrl = "https://mcommunity.umich.edu/api";
    
    private string _accessToken;
    private string _refreshToken;
    private DateTime _tokenExpiration;
    
    public MCommunityApiClient(string applicationId, string password)
    {
        _httpClient = new HttpClient();
        _applicationId = applicationId;
        _password = password;
    }
    
    // Authentication Methods
    
    private async Task<TokenResponse> GetAccessTokenAsync()
    {
        var tokenRequest = new TokenRequest
        {
            Username = _applicationId,
            Password = _password
        };
        
        var json = JsonSerializer.Serialize(tokenRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{BaseUrl}/token/", content);
        response.EnsureSuccessStatusCode();
        
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        
        // Cache the tokens
        _accessToken = tokenResponse.Access;
        _refreshToken = tokenResponse.Refresh;
        _tokenExpiration = DateTime.UtcNow.AddMinutes(55); // Tokens typically expire after 1 hour
        
        return tokenResponse;
    }
    
    private async Task<string> RefreshAccessTokenAsync()
    {
        var refreshRequest = new RefreshTokenRequest
        {
            Refresh = _refreshToken
        };
        
        var json = JsonSerializer.Serialize(refreshRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{BaseUrl}/token/refresh/", content);
        response.EnsureSuccessStatusCode();
        
        var tokenResponse = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
        
        // Update cached token
        _accessToken = tokenResponse.Access;
        _tokenExpiration = DateTime.UtcNow.AddMinutes(55);
        
        return tokenResponse.Access;
    }
    
    private async Task<string> EnsureValidTokenAsync()
    {
        // If no token or token expired, get new token
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
        {
            await GetAccessTokenAsync();
        }
        
        return _accessToken;
    }
    
    private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(
        HttpMethod method, 
        string endpoint)
    {
        var token = await EnsureValidTokenAsync();
        
        var request = new HttpRequestMessage(method, $"{BaseUrl}{endpoint}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return request;
    }
    
    // Identity Methods
    
    public async Task<PersonData> GetPersonAsync(string uniqname)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Get, 
            $"/people/{uniqname}/"
        );
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<PersonData>();
    }
    
    public async Task<List<PersonData>> SearchPeopleAsync(PeopleSearchRequest searchRequest)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Post, 
            "/people/find/"
        );
        
        var json = JsonSerializer.Serialize(searchRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        // Note: Swagger shows search returns {"entries": [...], "signals": "..."}
        var searchResponse = await response.Content.ReadFromJsonAsync<PeopleSearchResponse>();
        return searchResponse?.Entries ?? new List<PersonData>();
    }
    
    public async Task<string> GetPersonVCardAsync(string uniqname)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Get, 
            $"/people/{uniqname}/vcard/"
        );
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<NameCoachData> GetPersonNameCoachAsync(string uniqname)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Get, 
            $"/people/{uniqname}/namecoach/"
        );
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<NameCoachData>();
    }
    
    // Group Methods
    
    public async Task<GroupData> GetGroupAsync(string groupname)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Get, 
            $"/groups/{groupname}/"
        );
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<GroupData>();
    }
    
    public async Task<GroupData> CreateGroupAsync(CreateGroupRequest groupRequest)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Post, 
            "/groups/"
        );
        
        var json = JsonSerializer.Serialize(groupRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<GroupData>();
    }
    
    public async Task RenewGroupAsync(string groupname)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Post, 
            $"/groups/{groupname}/renew/"
        );
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task ExpireGroupAsync(string groupname, int days)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Post, 
            $"/groups/{groupname}/expire/"
        );
        
        var expireRequest = new ExpireGroupRequest { Days = days };
        var json = JsonSerializer.Serialize(expireRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task DeleteGroupAsync(string groupname)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Delete, 
            $"/groups/{groupname}/"
        );
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task<GroupData> UpdateGroupAttributesAsync(
        string groupname, 
        Dictionary<string, object> attributes)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Patch, 
            $"/groups/{groupname}/"
        );
        
        var json = JsonSerializer.Serialize(attributes);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<GroupData>();
    }
    
    public async Task ModifyGroupMembersAsync(
        string groupname, 
        List<string> membersToAdd = null, 
        List<string> membersToDelete = null)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Post, 
            $"/groups/{groupname}/member/"
        );
        
        var modifyRequest = new ModifyGroupMembersRequest
        {
            Add = membersToAdd ?? new List<string>(),
            Delete = membersToDelete ?? new List<string>()
        };
        
        var json = JsonSerializer.Serialize(modifyRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task ModifyGroupOwnersAsync(
        string groupname, 
        List<string> ownersToAdd = null, 
        List<string> ownersToDelete = null)
    {
        var request = await CreateAuthenticatedRequestAsync(
            HttpMethod.Post, 
            $"/groups/{groupname}/owner/"
        );
        
        var modifyRequest = new ModifyGroupMembersRequest
        {
            Add = ownersToAdd ?? new List<string>(),
            Delete = ownersToDelete ?? new List<string>()
        };
        
        var json = JsonSerializer.Serialize(modifyRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
```

### Request/Response Models

**Important**: Field names are based on the Swagger specification at https://mcommunity.umich.edu/api/doc/schema/swagger-ui/. Key differences from older documentation:
- Use `entry_dn` not `dn` for person distinguished names
- Use `surname` not `sn` for last names
- Use `umichInstRoles` not `umichAffiliation` for affiliations
- Email (`mail`), phone (`telephoneNumber`), and title (`umichTitle`) are **arrays** not strings

```csharp
// Authentication Models
public class TokenRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; }
    
    [JsonPropertyName("password")]
    public string Password { get; set; }
}

public class TokenResponse
{
    [JsonPropertyName("access")]
    public string Access { get; set; }
    
    [JsonPropertyName("refresh")]
    public string Refresh { get; set; }
}

public class RefreshTokenRequest
{
    [JsonPropertyName("refresh")]
    public string Refresh { get; set; }
}

public class RefreshTokenResponse
{
    [JsonPropertyName("access")]
    public string Access { get; set; }
}

// People Search Models
public class PeopleSearchRequest
{
    [JsonPropertyName("searchParts")]
    public List<SearchPart> SearchParts { get; set; }
    
    [JsonPropertyName("logicalOperator")]
    public string LogicalOperator { get; set; } // "AND" or "OR"
    
    [JsonPropertyName("attributes")]
    public List<string> Attributes { get; set; }
    
    [JsonPropertyName("numEntries")]
    public int NumEntries { get; set; }
}

public class SearchPart
{
    [JsonPropertyName("attribute")]
    public string Attribute { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
    
    [JsonPropertyName("searchType")]
    public string SearchType { get; set; } // "start", "end", "contain", "exact"
}

// People Search Response (Swagger spec shows wrapped response)
public class PeopleSearchResponse
{
    [JsonPropertyName("entries")]
    public List<PersonData> Entries { get; set; }
    
    [JsonPropertyName("signals")]
    public string Signals { get; set; }
}

public class PersonData
{
    [JsonPropertyName("entry_dn")]  // Note: Swagger uses 'entry_dn' not 'dn'
    public string Dn { get; set; }
    
    [JsonPropertyName("uid")]
    public string Uid { get; set; }
    
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }
    
    [JsonPropertyName("givenName")]
    public string GivenName { get; set; }
    
    [JsonPropertyName("surname")]  // Note: Swagger uses 'surname' not 'sn'
    public string Sn { get; set; }
    
    [JsonPropertyName("cn")]
    public List<string> Cn { get; set; }  // Common names (array)
    
    [JsonPropertyName("mail")]  // Array of email addresses
    public List<string> Mail { get; set; }
    
    [JsonPropertyName("telephoneNumber")]  // Array of phone numbers
    public List<string> TelephoneNumber { get; set; }
    
    [JsonPropertyName("umichTitle")]  // Note: Array of titles
    public List<string> Title { get; set; }
    
    [JsonPropertyName("ou")]  // Organizational units (array)
    public List<string> Ou { get; set; }
    
    [JsonPropertyName("umichInstRoles")]  // Note: Swagger uses 'umichInstRoles' not 'umichAffiliation'
    public List<string> UmichAffiliation { get; set; }
    
    [JsonPropertyName("groupMembership")]  // Array of group DNs
    public List<string> GroupMembership { get; set; }
}

public class NameCoachData
{
    [JsonPropertyName("nameCoachUrl")]
    public string NameCoachUrl { get; set; }
    
    [JsonPropertyName("audioUrl")]
    public string AudioUrl { get; set; }
}

// Group Models
public class CreateGroupRequest
{
    [JsonPropertyName("cn")]
    public string Cn { get; set; }
    
    [JsonPropertyName("umichGroupEmail")]
    public string UmichGroupEmail { get; set; }
    
    [JsonPropertyName("owner")]
    public List<string> Owner { get; set; }
    
    [JsonPropertyName("umichDescription")]
    public string UmichDescription { get; set; }
}

public class GroupData
{
    [JsonPropertyName("dn")]
    public string Dn { get; set; }
    
    [JsonPropertyName("cn")]
    public List<string> Cn { get; set; }
    
    [JsonPropertyName("umichGroupEmail")]
    public string UmichGroupEmail { get; set; }
    
    [JsonPropertyName("owner")]
    public List<string> Owner { get; set; }
    
    [JsonPropertyName("member")]
    public List<string> Member { get; set; }
    
    [JsonPropertyName("umichDescription")]
    public string UmichDescription { get; set; }
    
    [JsonPropertyName("modifyTimestamp")]
    public string ModifyTimestamp { get; set; }
    
    [JsonPropertyName("createTimestamp")]
    public string CreateTimestamp { get; set; }
}

public class ExpireGroupRequest
{
    [JsonPropertyName("days")]
    public int Days { get; set; }
}

public class ModifyGroupMembersRequest
{
    [JsonPropertyName("add")]
    public List<string> Add { get; set; }
    
    [JsonPropertyName("delete")]
    public List<string> Delete { get; set; }
}
```

### Usage Examples

```csharp
// Example 1: Initialize client and get person information
var client = new MCommunityApiClient(
    "cn=mcapi-dept-usage,ou=apiusers,o=services",
    "your-password"
);

var person = await client.GetPersonAsync("bjensen");
Console.WriteLine($"Name: {person.DisplayName}");
// Note: Mail, TelephoneNumber, and Title are arrays
if (person.Mail?.Count > 0)
    Console.WriteLine($"Email: {person.Mail[0]}");
if (person.Title?.Count > 0)
    Console.WriteLine($"Title: {person.Title[0]}");

// Example 2: Search for people by department
var searchRequest = new PeopleSearchRequest
{
    SearchParts = new List<SearchPart>
    {
        new SearchPart
        {
            Attribute = "ou",
            Value = "Engineering",
            SearchType = "exact"
        }
    },
    LogicalOperator = "AND",
    Attributes = new List<string> { "uid", "displayName", "mail", "title" },
    NumEntries = 100
};

var results = await client.SearchPeopleAsync(searchRequest);
foreach (var result in results)
{
    Console.WriteLine($"{result.DisplayName} ({result.Uid}) - {result.Mail}");
}

// Example 3: Search for uniqnames starting with "foo" or "bar"
var multiSearchRequest = new PeopleSearchRequest
{
    SearchParts = new List<SearchPart>
    {
        new SearchPart
        {
            Attribute = "uid",
            Value = "foo",
            SearchType = "start"
        },
        new SearchPart
        {
            Attribute = "uid",
            Value = "bar",
            SearchType = "start"
        }
    },
    LogicalOperator = "OR",
    Attributes = new List<string> { "dn", "uid", "displayName" },
    NumEntries = 50
};

var multiResults = await client.SearchPeopleAsync(multiSearchRequest);

// Example 4: Create a new group
var newGroup = new CreateGroupRequest
{
    Cn = "my-project-team",
    UmichGroupEmail = "projectteam",
    Owner = new List<string>
    {
        "uid=bjensen,ou=People,dc=umich,dc=edu",
        "uid=jsmith,ou=People,dc=umich,dc=edu"
    },
    UmichDescription = "Project team for the new initiative"
};

var createdGroup = await client.CreateGroupAsync(newGroup);
Console.WriteLine($"Group created: {createdGroup.Dn}");

// Example 5: Add members to a group
await client.ModifyGroupMembersAsync(
    "my-project-team",
    membersToAdd: new List<string>
    {
        "uid=user1,ou=People,dc=umich,dc=edu",
        "uid=user2,ou=People,dc=umich,dc=edu",
        "uid=user3,ou=People,dc=umich,dc=edu"
    }
);

// Example 6: Remove members from a group
await client.ModifyGroupMembersAsync(
    "my-project-team",
    membersToDelete: new List<string>
    {
        "uid=user1,ou=People,dc=umich,dc=edu"
    }
);

// Example 7: Update group description
await client.UpdateGroupAttributesAsync(
    "my-project-team",
    new Dictionary<string, object>
    {
        { "umichDescription", "Updated project team description" }
    }
);

// Example 8: Renew a group
await client.RenewGroupAsync("my-project-team");

// Example 9: Get group information
var groupInfo = await client.GetGroupAsync("my-project-team");
Console.WriteLine($"Group: {groupInfo.Cn[0]}");
Console.WriteLine($"Email: {groupInfo.UmichGroupEmail}@umich.edu");
Console.WriteLine($"Members: {groupInfo.Member?.Count ?? 0}");
Console.WriteLine($"Owners: {groupInfo.Owner?.Count ?? 0}");

// Example 10: Expire a group (soft delete)
await client.ExpireGroupAsync("old-project-team", days: 30);

// Example 11: Delete a group permanently
await client.DeleteGroupAsync("obsolete-group");

// Example 12: Get person's vCard
var vcard = await client.GetPersonVCardAsync("bjensen");
Console.WriteLine(vcard);

// Example 13: Get person's Name Coach data
try
{
    var nameCoach = await client.GetPersonNameCoachAsync("bjensen");
    Console.WriteLine($"Name Coach URL: {nameCoach.NameCoachUrl}");
    Console.WriteLine($"Audio URL: {nameCoach.AudioUrl}");
}
catch (HttpRequestException)
{
    Console.WriteLine("Name Coach data not available for this person");
}
```

### Helper Methods for DN Construction

```csharp
public static class MCommunityHelpers
{
    public static string GetPersonDn(string uniqname)
    {
        return $"uid={uniqname},ou=People,dc=umich,dc=edu";
    }
    
    public static string GetGroupDn(string groupname)
    {
        return $"cn={groupname},ou=User Groups,ou=Groups,dc=umich,dc=edu";
    }
    
    public static List<string> GetPersonDns(IEnumerable<string> uniqnames)
    {
        return uniqnames.Select(GetPersonDn).ToList();
    }
    
    public static string ExtractUniqnameFromDn(string dn)
    {
        // Extract uid from DN like "uid=bjensen,ou=People,dc=umich,dc=edu"
        var match = System.Text.RegularExpressions.Regex.Match(dn, @"uid=([^,]+)");
        return match.Success ? match.Groups[1].Value : null;
    }
    
    public static string ExtractGroupnameFromDn(string dn)
    {
        // Extract cn from DN like "cn=mygroup,ou=User Groups,ou=Groups,dc=umich,dc=edu"
        var match = System.Text.RegularExpressions.Regex.Match(dn, @"cn=([^,]+)");
        return match.Success ? match.Groups[1].Value : null;
    }
}
```

### Practical Use Case: Managing MVR Access Group

```csharp
public class MVRAccessManager
{
    private readonly MCommunityApiClient _mcommunityClient;
    private const string MVRGroupName = "mvr-access-group";
    
    public MVRAccessManager(string applicationId, string password)
    {
        _mcommunityClient = new MCommunityApiClient(applicationId, password);
    }
    
    public async Task<bool> HasMVRAccessAsync(string uniqname)
    {
        try
        {
            var group = await _mcommunityClient.GetGroupAsync(MVRGroupName);
            var personDn = MCommunityHelpers.GetPersonDn(uniqname);
            
            return group.Member?.Contains(personDn) ?? false;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
    
    public async Task GrantMVRAccessAsync(string uniqname)
    {
        var personDn = MCommunityHelpers.GetPersonDn(uniqname);
        
        await _mcommunityClient.ModifyGroupMembersAsync(
            MVRGroupName,
            membersToAdd: new List<string> { personDn }
        );
    }
    
    public async Task RevokeMVRAccessAsync(string uniqname)
    {
        var personDn = MCommunityHelpers.GetPersonDn(uniqname);
        
        await _mcommunityClient.ModifyGroupMembersAsync(
            MVRGroupName,
            membersToDelete: new List<string> { personDn }
        );
    }
    
    public async Task<List<string>> GetMVRUsersAsync()
    {
        var group = await _mcommunityClient.GetGroupAsync(MVRGroupName);
        
        return group.Member?
            .Select(MCommunityHelpers.ExtractUniqnameFromDn)
            .Where(u => !string.IsNullOrEmpty(u))
            .ToList() ?? new List<string>();
    }
    
    public async Task<PersonData> GetUserInfoAsync(string uniqname)
    {
        return await _mcommunityClient.GetPersonAsync(uniqname);
    }
}
```

### Error Handling Example

```csharp
public class MCommunityService
{
    private readonly MCommunityApiClient _client;
    private readonly ILogger<MCommunityService> _logger;
    
    public MCommunityService(
        MCommunityApiClient client, 
        ILogger<MCommunityService> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task<PersonData> GetPersonSafeAsync(string uniqname)
    {
        try
        {
            return await _client.GetPersonAsync(uniqname);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning($"Person not found: {uniqname}");
            return null;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogError("MCommunity API authentication failed");
            throw new InvalidOperationException("MCommunity authentication failed", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error retrieving person: {uniqname}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving person from MCommunity: {uniqname}");
            throw;
        }
    }
    
    public async Task<bool> TryAddGroupMemberAsync(string groupname, string uniqname)
    {
        try
        {
            var personDn = MCommunityHelpers.GetPersonDn(uniqname);
            await _client.ModifyGroupMembersAsync(
                groupname,
                membersToAdd: new List<string> { personDn }
            );
            
            _logger.LogInformation(
                $"Added {uniqname} to group {groupname}"
            );
            
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex, 
                $"Failed to add {uniqname} to group {groupname}"
            );
            return false;
        }
    }
}
```

## Additional Resources

- **Swagger API Documentation**: https://mcommunity.umich.edu/api/doc/schema/swagger-ui/
- **Code Examples Repository**: https://github.com/umich-iam/iam-api-examples
- **ITS Service Center**: https://its.umich.edu/help
- **MCommunity Web Application**: https://mcommunity.umich.edu

