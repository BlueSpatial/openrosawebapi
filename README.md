# How to create auto odk form
there are 4 type of form in odk collect 
1. INPUT
 ```
 INPUT used for inputting a data with type and you can also add this type automatically with code from class Tipe :
 ```
 | Type       | Description     | call the type with code| 
| ------------- |:-------------:| :-------------:| 
| string      | As in XML 1.0, optionally in “http://www.w3.org/2001/XMLSchema” namespace | new Tipe().TipeString;
| int      | As in XML 1.0, optionally in “http://www.w3.org/2001/XMLSchema” namespace      |   new Tipe().TipeInt;
|boolean  | As in XML 1.0, optionally in “http://www.w3.org/2001/XMLSchema” namespace      | new Tipe().TipeBoolean;
|decimal|As in XML 1.0, optionally in “http://www.w3.org/2001/XMLSchema” namespace|new Tipe().TipeDecimal;
|date|As in XML 1.0, optionally in “http://www.w3.org/2001/XMLSchema” namespace|new Tipe().TipeDate;
|time|As in XML 1.0, optionally in “http://www.w3.org/2001/XMLSchema” namespace|new Tipe().TipeTime;
|dateTime|As in XML 1.0, optionally in “http://www.w3.org/2001/XMLSchema” namespace|new Tipe().TipeDateTime;
|geopoint|Space-separated list of valid latitude (decimal degrees), longitude (decimal degrees), altitude (decimal meters) and accuracy (decimal meters)|new Tipe().TipeGeopoint;
|geotrace|Semi-colon-separated list of at least 2 geopoints, where the last geopoint’s latitude and longitude is not equal to the first| new Tipe().TipeGeotrace; 	
|geoshape|Semi-colon-separated list of at least 3 geopoints, where the last geopoint’s latitude and longitude is equal to the first|new Tipe().TipGeoshape;
|binary|String ID (with binary file attached to submission)|new Tipe().TipeBinary;
|barcode|As string|new Tipe().TipeBarcode;
|intent|As string, used for external applications|new Tipe().TipeIntent;

```
create input form from class CreateOdkForm like this:

createInputForm(string dataName,string label,string tipe,string required,string readOnly)
```
| Params       | Description     | Example| 
| ------------- |:-------------:| :-------------:|
|dataName|the name of your data from nhibernate object|nameof(Book.Title)|
|label|question to asked for  the user|what is the the title of the book?|
|tipe|the type of the input |new Tipe().TipeString;|
|required|is the form required or not |new Required().TRUE;|
|readOnly|is the form readonly or not |new ReadOnly().FALSE;|

2. SELECT
```
Select data from multiple checkboxes
createSelectForm(string dataName, string label, string required, string readOnly, List<FormItem> item) 
```
| Params       | Description     | Example| 
| ------------- |:-------------:| :-------------:|
|dataName|the name of your data from nhibernate object|nameof(Book.Genre)|
|label|question to asked for  the user|what is the the title of the book?|
|required|is the form required or not |new Required().TRUE;|
|readOnly|is the form readonly or not |new ReadOnly().FALSE;|
|item|List of item to select from | List<FormItem> pakan = new List<FormItem>();               pakan.Add(new FormItem() { value = "horror", label = "horror" });               pakan.Add(new FormItem() { value = "fantasy", label = "fantasy" });                pakan.Add(new FormItem() { value = "funny", label = "funny" });|

3. SELECT1
```
Select data from only one checkboxes
createSelect1Form(string dataName, string label, string required, string readOnly, List<FormItem> item) 
```
| Params       | Description     | Example| 
| ------------- |:-------------:| :-------------:|
|dataName|the name of your data from nhibernate object|nameof(Book.Genre)|
|label|question to asked for  the user|what is the the title of the book?|
|required|is the form required or not |new Required().TRUE;|
|readOnly|is the form readonly or not |new ReadOnly().FALSE;|
|item|List of item to select from | List<FormItem> pakan = new List<FormItem>();               pakan.Add(new FormItem() { value = "horror", label = "horror" });               pakan.Add(new FormItem() { value = "fantasy", label = "fantasy" });                pakan.Add(new FormItem() { value = "funny", label = "funny" });|
4. UPLOAD
```
create upload form from class CreateOdkForm like this:
createUploadForm(string dataName, string label, string binaryTipe, string required, string readOnly)
```
| Params       | Description     | Example| 
| ------------- |:-------------:| :-------------:|
|dataName|the name of your data from nhibernate object|nameof(Book.Author)|
|label|question to asked for  the user|can you upload the pic of the author?|
|binaryTipe|the type of the upload form there are three type you can  |new binaryTipe().image;|
|required|is the form required or not |new Required().TRUE;|
|readOnly|is the form readonly or not |new ReadOnly().FALSE;|


## example on how to create simple odk form with CreateOdkForm class
```
CreateOdkForm punuk = new CreateOdkForm();
List<FormItem> pakan = new List<FormItem>();
pakan.Add(new FormItem() { value = "horror", label = "horror" });
pakan.Add(new FormItem() { value = "fantasy", label = "fantasy" });
pakan.Add(new FormItem() { value = "funny", label = "funny" });
punuk.createInputForm(nameof(Book.Title), "whats the title?", new Tipe().TipeString, new Required().TRUE, null);
punuk.createSelectForm(nameof(Book.Genre), "what is the genre?", null, null, pakan);
punuk.createInputForm(nameof(Book.Author), "whats the name of the author?", new Tipe().TipeString, new Required().TRUE, null);
punuk.createTheForm(nameof(Book), 1).ToString()
```

### code explained
```
CreateOdkForm punuk = new CreateOdkForm();
```
to call the class CreateOdkForm

```
List<FormItem> pakan = new List<FormItem>();
pakan.Add(new FormItem() { value = "horror", label = "horror" });
pakan.Add(new FormItem() { value = "fantasy", label = "fantasy" });
pakan.Add(new FormItem() { value = "funny", label = "funny" });
```
to create a list of item in the select form

```
punuk.createInputForm(nameof(Book.Title), "whats the title?", new Tipe().TipeString, new Required().TRUE, null);
```
create input form type string
```
punuk.createSelectForm(nameof(Book.Genre), "what is the genre?", null, null, pakan);
```
create Select form 
```
punuk.createInputForm(nameof(Book.Author), "whats the name of the author?", new Tipe().TipeString, new Required().TRUE, null);
```
create input  form type string 
```
punuk.createTheForm(nameof(Book), 1).ToString()
```
finally create the form with method createTheForm(string name,int version)


