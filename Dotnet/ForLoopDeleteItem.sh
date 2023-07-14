#!/bin/bash
for index in {10059..10073}
do
    curl -X DELETE http://localhost:8080/item/$index
done