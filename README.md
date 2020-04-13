# MuseiNews
Simple ASP.NET Core news API: a cron job takes the news from an RSS feed (currently [https://www.intopic.it/rss/arte/musei/](https://www.intopic.it/rss/arte/musei/), but can be easily changed by editing [MuseiNews/Tasks/UpdateNews.cs](MuseiNews/Tasks/UpdateNews.cs)) and provides them at `/news` endpoint. Designed for use with client applications.

Made during school year 2019/2020 at [ITIS Paleocapa](https://itispaleocapa.edu.it).

## Endpoints
### Without authentication
`GET /news`  
Returns all news, in ascending timestamp order.

`POST /clients`  
Generates a new pair of clientId and clientToken.

### With authentication
`GET /clients/{clientId}/users/{userId}/news`  
Returns all news, in ascending timestamp order, with additional *read* boolean field, *true* if the user had read the news, *false* if not.  
Note: userId is a string and should be unique for the client, not globally.

`GET /clients/{clientId}/users/{userId}/news/{newsId}`  
Returns a single news with user related *read* field.

`PUT /clients/{clientId}/users/{userId}/news/{newsId}`  
Edits the *read* field, it expects to receive in the body the news object with the *read* field edited.

`GET /clients/{clientId}/users/{userId}/news/unread`  
Returns user unread news, in ascending timestamp order with *read* field (always false).

## Authentication
Add the following header to the requests that need authentication:
```
Authorization: Bearer <clientToken>
```
