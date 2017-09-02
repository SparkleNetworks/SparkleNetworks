
* [Index](0000-Index.md)
* [The APIs](6000-APIs.md)
* [NetworkRootApi](6100-NetworkRootApi.md)
* Previous: [API keys and making requests](6101-NetworkRootApi-Request.md)


NetworkRootApi: APIs list
================================================


USAGE OF THIS API MAY CAUSE SECURITY RISKS  
READ AND *UNDERSTAND* the [Network ROOT API documentation](6100-NetworkRootApi.md) before you compromise your system.

Endpoints are listed grouped by service. Services are sorted by pseudo-alphabetical order. Endpoints have a description, a URL template, a list of query parameters, a sample request body and a sample result body.

Note: Descriptions are in French now but must be translated to English.


CompanyCategories
------------------------------------

### `GET /NetworkRootApi/CompanyCategories/All`

Liste toutes les catégories d’entreprises.
 
    {
      "Data": [
        {
          "Id": 4,
          "Name": "Entreprise",
          "Alias": "Company",
          "NetworkId": 4,
          "KnownCategory": "Unknown",
          "IsDefault": true,
          "UsedByActivCount": 0,
          "UsedByInactivCount": 0
        },
        {
          "Id": 5,
          "Name": "Association",
          "Alias": "Association",
          "NetworkId": 4,
          "KnownCategory": "Unknown",
          "IsDefault": false,
          "UsedByActivCount": 0,
          "UsedByInactivCount": 0
        },
        {
          "Id": 6,
          "Name": "Organisme de Certification",
          "Alias": "Organisme-de-Certification",
          "NetworkId": 4,
          "KnownCategory": "Unknown",
          "IsDefault": false,
          "UsedByActivCount": 0,
          "UsedByInactivCount": 0
        }
      ]
    }



Companies
------------------------------------
 

### `GET /NetworkRootApi/Companies/Search`

La recherche d’entreprises.
 
Recherche par lieu et par tags :

    ?Location=Talence, France&Offset=0&Count=10&CombinedTags=False&TagId=114&TagId=98&TagId=130
 
Recherche par lieu et par tags cumulés :

    ?Location=Talence, France&Offset=0&Count=10&CombinedTags=True&TagId=114&TagId=98
 
Recherche par nom :

    ?Keywords=service&Offset=0&Count=10
 
#### Query string argument: Location

Si Location est spécifié, alors le système va géocoder la valeur pour faire une recherche géographique. Le temps d’appel à l’API externe peut être de plusieurs secondes à certains moments de la journée. Cela marche avec des codes postaux. Il est impératif de suffixer les demandes de ", France". Sans cela, les résultats du géocode donneront des points dans le monde et l’API ne trouvera pas d’entreprises.
 
Format pour code postal : `33400, France`
 
Format pour ville : `Talence, France`
 
Format pour adresse complète : `1 avenue collégno, Talence, France`
 
L’API retournera une collection des géocodes utilisés pour la recherche.

    "LocationGeocodes": [
          "44.8026734046027 -0.58809634676736"
    ]
 
Note : 
Si on veut supporter de façon sûre/rapide la saisie par code postal, alors il faut utiliser [un fichier comme celui-ci](https://datanova.laposte.fr/explore/dataset/base-officielle-des-codes-postaux-croisee-avec-geoflar-communes-2013/table/).
Sparkle ne peut pas prendre en charge cette opération. En revanche je peux coder ce qu’il faut pour que le paramètre Location accepte un [WKT Point](https://en.wikipedia.org/wiki/Well-known_text). Ce sera à l’appelant de l’API de passer le point GPS correspondant au code postal saisi par l’utilisateur.
 
#### Query string arguments: TagId, CombinedTags

TagId peut être passé plusieurs fois. Il doit correspondre à un ID de tag existant.
CombinedTags true/false permet (avec plusieurs tag IDs) de spécifier si on cherche une entreprise qui comprend TOUS les tags (true) ou au moins un (false).
 
#### Query string arguments: Offset, Count

Permet de spécifier le nombre d’items désirés et l’offset d’items.
 
#### Query string argument: Keywords

Permet la recherche par nom.
 
#### Query string argument: IncludePlaces=True

Les Places associées aux entreprises sont incluses dans les résultats.
Si Count est élevé, le temps d’exécution de la méthode peut être impacté.
 
#### Query string argument: IncludeTags=True

Les tags associées aux entreprises sont inclus dans les résultats.
Si Count est élevé, le temps d’exécution de la méthode peut être impacté.
 
 
#### Sample response

    {
      "Data": {
        "Offset": 0,
        "Count": 10,
        "Total": 12,
        "Items": [
          {
            "Id": 37,
            "Alias": "A-M-T",
            "Name": "A.M.T.",
            "IsActive": true,
            "IsApproved": false,
            "IsEnabled": true,
            "CategoryId": 0,
            "PictureUrl": null,
            "SmallProfilePictureUrl": null,
            "MediumProfilePictureUrl": null,
            "LargeProfilePictureUrl": null,
            "ProfileUrl": null,
            "Baseline": null,
            "About": null,
            "Fields": null,
            "Data": null
          }...
        ],
        "LocationGeocodes": [
          "44.8026734046027 -0.58809634676736"
        ]
      }
    }
 



InformationNotes
------------------------------------

### `GET /NetworkRootApi/InformationNotes/ById/{id}`

Retourne une *Note d'information* par son ID. 

#### Query parameters:

* `id` The item ID.

#### Sample response

    {
      "Data": {
        "Id": 1,
        "Name": "Aide à la formation",
        "Description": "Aide à la formation",
        "NetworkId": 1,
        "UserId": 6,
        "StartDateUtc": "2016-05-18T21:45:00Z",
        "EndDateUtc": "2016-05-29T21:45:00Z"
      }
    }

### `GET /NetworkRootApi/InformationNotes/List`

Liste les *Notes d'information*. 

#### Query parameters:

* `Offset = 0` The pagination offset.
* `Count = 20` The pagination size.
* `KnownFilter = "All"` A known filter name.
    * `KnownFilter = "All"` Get all items.
    * `KnownFilter = "Active"` Get currently active items.

#### Sample response


    {
      "Data": {
        "Offset": 0,
        "Total": 1,
        "Count": 10,
        "Items": [
          {
            "Id": 1,
            "Name": "Aide à la formation",
            "Description": "Aide à la formation",
            "NetworkId": 1,
            "UserId": 0,
            "StartDateUtc": "2016-05-18T21:45:00Z",
            "EndDateUtc": "2016-05-29T21:45:00Z"
          },
          ...more items...
        ]
      }
    }

### `GET /NetworkRootApi/InformationNotes/PrepareEdit`

Retourne le formulaire d'édition d'une nouvelle note d'information.

Cette méthode est utile si des listes de précomplétion doivent être proposées à l'utilisateur. Ce n'est pas le cas au 2016-10-08.

#### Sample response

    TODO

### `GET /NetworkRootApi/InformationNotes/PrepareEdit/{id}`

Retourne le formulaire d'édition d'une note d'information existante.

#### Query parameters:

* `id` The ID of the item to edit.

#### Sample response

    {
      "Data": {
        "Id": 7,
        "Name": "Unit test InformationNotesTests.EditMethod_Create_Prepare_Edit on 2016-10-09 12:19:31",
        "Description": "Lorem ipsum dolor sit amet, condimentum ultricies ... nam.",
        "UserStartDate": "2016-10-09T12:19:31.727Z",
        "UserEndDate": "2016-10-16T12:19:31.727Z",
        "ActingUserId": null
      }
    }

### `POST /NetworkRootApi/InformationNotes/Edit`

Crée ou modifie une note d'information.

Pour modifier une note existante, préciser sont Id dans le request body.


#### Sample request body

Create:

    {
      "Id": null,
      "Name": "New information note!",
      "Description": "Lorem ipsum dolor sit amet, ... lectus nam.",
      "UserStartDate": "2016-10-08T15:16:36.0729685Z",
      "UserEndDate": "2016-10-15T15:16:36.0729685Z",
      "ActingUserId": 6
    }

Edit: 

    {
      "Id": 5,
      "Name": "Unit test InformationNotesTests.Edit_CreateItem on 2016-10-08 15:16:36",
      "Description": "Lorem ipsum dolor sit amet, ... lectus nam.",
      "UserStartDate": "2016-10-08T15:16:36.0729685Z",
      "UserEndDate": "2016-10-15T15:16:36.0729685Z",
      "ActingUserId": 6
    }

#### Sample response

    {
      "Data": {
        "Item": {
          "Id": 5,
          "Name": "Unit test InformationNotesTests.Edit_CreateItem on 2016-10-08 15:16:36",
          "Description": "Lorem ipsum dolor sit amet, ... lectus nam.",
          "NetworkId": 1,
          "UserId": 6,
          "StartDateUtc": "2016-10-08T15:16:36.0729685Z",
          "EndDateUtc": "2016-10-15T15:16:36.0729685Z",
          "IsDeleted": false
        },
        "Succeed": true,
        "Errors": []
      }
    }



TagCategories
------------------------------------


### `GET /NetworkRootApi/TagCategories/All`

Liste les catégories de tags. 

Les ID de ces catégories servent par la suite pour lister les tags.
 
    {
      "Data": [
        {
          "Id": 1,
          "Name": "Skill",
          "Alias": "Skill",
          "CanUsersCreate": true,
          "ApplyToCompanies": true,
        },
        {
          "Id": 6,
          "Name": "Company Tags",
          "Alias": "CompanyTags",
          "CanUsersCreate": false,
          "ApplyToCompanies": true,
        }
      ]
    }


TagDefinitions
------------------------------------


### `GET /NetworkRootApi/TagDefinitions/ByCategory?CategoryId=1&Offset=0&Count=10`

Liste page par page les tags d’une catégorie donnée. Remarquez le format des listes paginées (Offset, Count, Total, Items).

Evitez de demander un Count trop grand car ces méthodes API ne sont pas forcément optimisées pour la performance.
 
    {
      "Data": {
        "Offset": 0,
        "Total": 60,
        "Count": 10,
        "Items": [
          {
            "Id": 141,
            "NetworkId": 4,
            "Name": "Assainissement",
            "Alias": "Assainissement",
            "CategoryId": 1,
            "CategoryValue": "Skill",
            "CreatedDateUtc": "2016-03-16T23:44:49.323Z",
            "Description": null,
            "Payload": 0
          },
          {
            "Id": 89,
            "NetworkId": 4,
            "Name": "BigData",
            "Alias": "BigData",
            "CategoryId": 1,
            "CategoryValue": "Skill",
            "CreatedDateUtc": "2015-11-04T22:16:35.113Z",
            "Description": null,
            "Payload": 0
          },...
        ]
      }
    }

