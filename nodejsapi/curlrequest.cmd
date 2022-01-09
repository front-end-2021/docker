@echo off
SET count=1
FOR /L %%x IN (1,1,10) DO (
    echo %%x:
    curl http://localhost:49160
    echo.
)

PAUSE