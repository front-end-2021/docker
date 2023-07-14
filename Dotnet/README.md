# Code First Migration with EF core
1. Pre-required: Install Docker Desktop (or Docker on wsl2 Ubuntu on Windows)
2. Package Manager Console
    ```Add-Migration NameOfMigration```
3. Update DataBase
    ```Update-Database```

4. API url
    - [GET] http://localhost:8080/item/[id]
    - [POST] http://localhost:8080/item/
    - [PUT] http://localhost:8080/item/{id}
    - [DELETE] http://localhost:8080/item/{id}
    - [GET] http://localhost:8080/task/[id]
    - [GET] http://localhost:8080/task/item/{id}
    - [POST] http://localhost:8080/task/
    - [PUT] http://localhost:8080/task/{id}
    - [DELETE] http://localhost:8080/task/{id}
    - [GET] http://localhost:8080/setting/[id]
    - [GET] http://localhost:8080/setting/item/{id}
    - [POST] http://localhost:8080/setting/
    - [PUT] http://localhost:8080/setting/{id}
    - [DELETE] http://localhost:8080/setting/{id}

5. Test using bash file
    - Open Git Bash (To run curl command)
    - cd to Project's folder
    - If create type: ```bash ForLoopNewItem.sh```