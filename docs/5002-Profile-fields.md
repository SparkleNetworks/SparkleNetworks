
* [Index](0000-Index.md)
* [Features implementation](5000-Features-implementation.md)


Profile fields
=========================================

In order to customize the various fields we can associate to various entities, we created the profile fields system.

What's a field?
---------------------

These things are profile fields:

* A user's bio
* A user's twitter username
* A company's website
* The school promotion year associated to a given group
* The dress code for an event

Some fields already exist in Sparkle and will help you create a basic network. You may declare custom fields in order to add stuff to any object (User, Company, Group, Event, Ad...)

The difference with tags? It is built in almost the same way but there are some differences.

* A tag cannot be customized when you use it; a field can. 
* A tag needs a backing list of items, a field does not need it but may have one.
* A tag can contain extra data in a JSON field, this data is not personalized when the tag is applied. A field can contain complex data but this data is to be created when creating a field value. 


The built-in fields
---------------------

In parenthesis the entities the field applies to.

* Site	(Users, Companies)
* Phone	(Users, Companies)
* About	(Users, Companies)
* City	(Users, Companies)
* ZipCode	(Users, Companies)
* FavoriteQuotes	(Users)
* CurrentTarget	(Users)
* Contribution	(Users)
* Country	(Users, Companies)
* Headline	(Users)
* ContactGuideline	(Users)
* Industry	(Users, Companies)
* LinkedInPublicUrl	(Users, Companies)
* Language	(Users)
	* This field has a special format that came from the LinkedIn API
* Education	(Users)
	* This field has a special format that came from the LinkedIn API
* Twitter	(Users, Companies)
* GTalk	(Users)
* Msn	(Users)
* Skype	(Users)
* Yahoo	(Users)
* Volunteer	(Users)
	* This field has a special format that came from the LinkedIn API
* Certification	(Users)
	* This field has a special format that came from the LinkedIn API
* Patents	(Users)
	* This field has a special format that came from the LinkedIn API
* Location	(Users, Companies)
* Contact	()
* Recommendation	(Users)
	* This field has a special format that came from the LinkedIn API
* Email	(Companies)
* Facebook	(Companies)
* AngelList	(Companies)
* NetworkTeamRole	(Users)
* NetworkTeamDescription	(Users)
* NetworkTeamGroup	(Users)
* NetworkTeamOrder	(Users)
* Position	(Users)
	* This field has a special format that came from the LinkedIn API

Possible enhancements
---------------------

This feature is not yet fully integrated. Here are some enhancements to build in the future.

* Admin page to create a new custom field.
* Dynamic display for page that will display the values associated through the fields.
* A dynamic editor.
* A reporting page to export objects or statistics based on field values.
* The Jobs, GroupCategories, EventCategories tables might become ProfileFields. Or tags?

ProfileFields table
---------------------

* Id: The id for the field
	* Ids from 1 to 999 are reserved for "built-in fields".
	* When creating a custom field, start at Id 1000.
* Name: The technical name for the field (as in the list above)
* ApplyToUsers: a temporary boolean that says whether the field can be associated to a User
* ApplyToCompanies: a temporary boolean that says whether the field can be associated to a Company
* Rules: a JSON field that will contain rules and data

### ProfileField.Rules

These are the supported rules.

```
{
  "Value": {
    "Type": "string",
    "IsMultiLineText": true,
    "StringMaxLength": 1200
  }
}
```

```
{
  "Value": {
    "Type": "bool"
  }
}
```

We plan to add more rules into this JSON: the association with other Objects for example.

```
{
  "Value": {
    "Type": "string",
    "IsMultiLineText": false,
    "StringMaxLength": 200
  },
  "Associations": {
    "Users": {
      "IsEnabled": true,
      "MaximumValues": 1
    }
    "Events": {
      "IsEnabled": false,
      "MaximumValues": 3
    }
  }
}
```

And also custom validation rules.

```
{
  "Value": {
    "Type": "string",
    "IsMultiLineText": false,
    "StringMaxLength": 200,
    "Validator": "MyModule.ProfileFields.EmailAddressValidator"
  }
}
```










