
# Interview Exam: Paginated Todo List

## Introduction
The project consists of an angular web SPA that connects to a .NET back-end. 
The app simply displays a list of `Todo` items. The problem is that the application in it's current state displays a list of **all todo items from the server**. It is the applicant's job to introduce a pagination feature to this app.
The front-end application interacts with the back-end through `RESTful` requests. The server wires it's components in a SOA-like manner.

---

## Technology Stack
- Front-End: `AngularJS 1.4.8` Web SPA using `javascript ES5`, `CSS3` and `HTML5`
- Back-End: `.NET 4.6` using `Web API 2` with `C# 6`
- Tooling: 
  - `Visual Studio 2015` IDE
  - `Chrome Canary` browser + embedded `Chrome Developer Tools`
  - `AngularJS Batarang` chrome extension for debugging AngularJS apps 

---

## Goal
The goal of this test is to evaluate the following of the applicant's abilities:
- api modeling: the contract between the front-end and back-end, contract between the back-end's components/services and client-side data-flow
- overall knowledge about the chosen technology stack: mainly `AngularJS` and `.NET` and how they can be wired together to create a web application
- troubleshooting: some bugs were purposely introduced to test the applicant's ability in finding their cause and removing them
- basic knowledge of `git` and `github` (don't get stuck on the 'git part' of the exam; this is not mandatory knowledge; you can ask for help)

---

## The Test
The test consists of a hands on experience with an incomplete codebase (with a few bugs here and there).
The application as it is now displays only a long list of todo items in table-like fashion with a placeholder for what a pagination component is supposed to look like.
The applicant is supposed to model and implement a pagination feature for a list of objects. Both the back-end and front-end need to be modified for the feature to work.

### The Codebase
```
├───InterviewTestPagination
│   │
|   |───index.html ## application's single page
|   |
│   ├───app # angular app folder
│   │   │   app.module.js  ## angular app module definition
│   │   │   main.js        ## angular app code (all components are here, feel free to modularize if you want)
│   │   │
│   │   ├───styles
│   │   │       styles.css ## application's single stylesheet
│   │   │
│   │   └───templates
│   │           pagination.html          ## pagination directive's template
│   │           todo.list.paginated.html ## Todo list's directive's template
│   │
│   ├───App_Start
│   │       WebApiConfig.cs
│   │
│   ├───Controllers
│   │       TodoController.cs ## WebApi controller for the Todo Model
│   │
│   ├───Models  ## Model api
│   │   │   IModelRepository.cs 
│   │   │   IModelService.cs
│   │   │
│   │   └───Todo ## Todo Model specific implementations
│   │           Todo.cs
│   │           TodoRepository.cs
│   │           TodoService.cs
|   |
│   └───Scripts ## third-party scripts (just angular for now)
|
├───InterviewTestPagination.Tests
```
The code should be documented enough so that the applicants know what they're supposed to do

### Requirements
- Front-end:
  - create an angular pagination **reusable** component/directive
    - choose number of items per page (10/20(default)/30/all)
    - navigate to previous, next, first, last (buttons)
    - navigate to an arbitrary page (number input)
    - display current page number, number of pages and total number of items
    - placeholders for the aforementioned elements and more detailed information can be found at `app/templates/pagination.html`
  - modify the todo list component to use the pagination component correctly
    - interacting with the pagination component need to change list page (e.g. choosing page 3 should fetch the todo items in that range, etc)
    - allow sorting by object property when clicking the table headers
      - go to first page but maintain selected page size
      - toggle sorting order for subsequent clicks
      - sort only a property at a time
      - more information can be found at `app/templates/todo.list.paginated.html` 
- Back-end:
  - implement the api required (client-to-server contract, service-layer dtos, method signatures, etc) for the pagination to work

### Nothing is true. Everything is permitted
The applicants are allowed to use third-party libraries, change the structure of the application, modify the entire codebase as they see fit and add external open-source tooling. Just beware of time restrictions. Feel free to use the internet for research as well.
*PS: copying and adapting proprietary code that is brought from home or from a former company you worked at is* **NOT ALLOWED** *because it misses the point of a hands-on exam and the abilities we are trying to assess, plus we consider it unethical in an exam (try to work with the instructions provided and research)*

### What we are NOT testing
- Design concepts: no styling, animations or 'making it look pretty' are necessary at all
- Performance tunning: not required to find the most high performance solution, minify and concatenate files, compress data flows, etc
- Client-side routing: not required to implement or use any native or thirdy party angular routing library. Navigation will be limited to the todo list
- Security concepts: not required to encrypt, or hash data. Don't worry about information leak. No need to implement authentication and authorization
- Infrastructure and Protocols: there's no need to worry about any IT infrastructure, all the communication between client-side and server-side (and/or datasource) should be local. The only communication protocol being used is 'basic' http, there's no need to get creative with websockets, SSE, messaging, etc
- Cross-browser compatibility: the application only has to work in `Chrome`, don't worry about other browsers

### The practical part
- The applicant will be given a fully configured machine with the proper tooling installed and configured for the exam
- Project will be already cloned in the machine, set to a applicant specific branch and imported on Visual Studio ready to deploy
- **[IMPORTANT]** The expected time for finishing the question is 1.5 hours, but there´s actually no fixed time limit. The applicant is free to span some extra time on it if they wish to. **Quality is what counts the most.**
- **[IMPORTANT]** Once the applicant is finished they need to generate a zip including the final codebase and a readme.txt file containing the amount of time he took to finish it. Any other considerations can be included on the readme.txt file **


---

## Initial look
*List was reduced to fit the picture*
![Alt text](https://github.com/controltechnologysolutions/hrquestions/blob/master/assets/initial_screenshot.png "List was reduced to fit the picture")

---
---
