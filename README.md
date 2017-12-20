# PatientstestManagement
“Patients Medical Test Manager” design document:
Design Goals:
This test application is intended for maintaining patients medical details such as patient name, date of birth, patient home address and medical test information. Medical test details includes type of test done, instrument used for test, when the test was done and the test results.
User shall be able to:
-	View/browse through existing patient details.
-	Add new patient’s information along with medical tests details, One patient can have more than one test results.
-	Edit exiting patients information
-	Delete the patient records.
-	All the details shall be stored persistently in a backend database.
User Interface:
Application has two windows – Main window and Patient details window. The main application window shows list of patients available in a ListView, every row in list corresponds to a single patient stored in the database. 
 
A patients record can be deleted by clicking corresponding ‘Delete’ button in the last column of every row. Double clicking a row in the list will take us to ‘Patient Details’ window, which shows the complete details including Patient’s tests details in a GridView.
 
# Code Design overview:
Code mainly divided into two layered architecture – UI and backend. UI layer’s Classes/Objects are responsible for getting needed information/data by requesting backend objects and presenting it on user interface. Backend classes are responsible for maintaining the data and converting the data into data structures that can understand by UI.
Data flow between UI and backend classes uses simple data structures, this allows us to easily replace the backend implementation without touching UI and vice versa is also possible.
Unit Tests are covered for backend classes.
### Technologies used:

- **Development tools/languages:** C#.net, Visual Studio 2017
- **UI Design:** WPF
- **Database:** SQL Express
- **UnitTest Framework:** MSTests

# Database Schema:
```
/* Patients Information table */
CREATE TABLE PatientsInfo (
ID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
Name nvarchar(255),
DateOfBirth DateTime,
Address nvarchar(1024),
);

/* Medical Test Information table */
CREATE TABLE TestsInfo (
ID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
PatientID int FOREIGN KEY REFERENCES PatientsInfo(ID),
TestType nvarchar(255),
Instrument nvarchar(255),
TimeStamp DateTime,
Result nvarchar(255)
);
```

# Addition Improvement Ideas:
Below are the possible improvements can be done to code, could not be done due to time limit:
-	Define A backend Interface(IPatientBackend), which shall be implemented by backend providers, say file backend. UI shell use this interface to access data.
-	Expand unit tests to cover UI classes.
-	Inline code documentation.
-	More exception handling and validations.
