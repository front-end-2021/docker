#!/bin/bash
for index in {1..5}
do
    let aCst=$index+1
    curl -d '{"name":"task curl '$index'", "idItem": 30076, "start":"2023-08-02", "end":"2023-08-02", "actualCost":'$aCst'}' \
    -H "Content-Type: application/json" -X POST http://localhost:8080/task/
done