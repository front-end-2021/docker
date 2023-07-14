#!/bin/bash
for index in {3..4}
do
    let oCst=$index+3
    curl -d '{"name":"item curl '$index'", "start":"2023-07-13", "openCost":'$oCst'}' \
    -H "Content-Type: application/json" -X POST http://localhost:8080/item/
done